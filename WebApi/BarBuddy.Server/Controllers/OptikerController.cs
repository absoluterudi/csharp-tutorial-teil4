using AutoMapper;
using BarBuddy.DTOs;
using BarBuddy.DTOs.Enums;
using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using BarBuddy.Server.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BarBuddy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptikerController : ControllerBase
    {
        private readonly string _PASSWORDSALT = "$2a$11$78B4w1CY64crLACSasMKee";

        private readonly ILogger<OptikerController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly GoogleMapsFactory _googleMapsFactory;
        private readonly EmailFactory _emailFactory;
        private readonly HtmlRenderFactory _htmlRenderFactory;

        public OptikerController(ILogger<OptikerController> logger, IMapper mapper, IConfiguration configuration,
            GoogleMapsFactory googleMapsFactory, EmailFactory emailFactory, HtmlRenderFactory htmlRenderFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _googleMapsFactory = googleMapsFactory;
            _emailFactory = emailFactory;
            _htmlRenderFactory = htmlRenderFactory;
        }

        #region for mobile app

        [HttpPost("GetOptikersWithinRadius")]
        public async Task<IActionResult> GetOptikersWithinRadius(CurrentPosition currentPosition)
        {
            try
            {
                var list = new List<OptikerResult>();

                var myPoint = new Point(currentPosition.MyLongitude, currentPosition.MyLatitude) { SRID = GoogleMapsFactory.SRID };
                var centerPoint = new Point(currentPosition.CenterLongitude, currentPosition.CenterLatitude) { SRID = GoogleMapsFactory.SRID };
                var distanceInMetres = currentPosition.Radius * 1000;

#if AAA
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    var locationWithDistance = await context.FullOptikerQuery(true).Where(o => o.IsActive && o.GeoOptiker.IsWithinDistance(centerPoint, distanceInMetres))
                                                                             .Select(o => new { location = o, distance = o.GeoOptiker.Distance(myPoint) }).ToListAsync();

                    var locations = locationWithDistance.Select(o => o.location).ToList();
                    list = _mapper.Map<List<OptikerResult>>(locations);

                    foreach (var lwdItem in locationWithDistance)
                    {
                        var listItem = list.FirstOrDefault(o => o.Id == lwdItem.location.Id);
                        if (listItem != null)
                        {
                            listItem.Distance = lwdItem.distance;
                        }
                    }
                }

                return Ok(list);
#endif
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetOptikerWithSpotInfos/{id}")]
        public async Task<IActionResult> GetOptikerWithSpotInfos(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Optiker dbOptiker = await context.FullOptikerQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbOptiker == null)
                    {
                        throw new Exception($"OptikerId {id} doesn't reference a valid location.");
                    }

                    var result = _mapper.Map<OptikerResult>(dbOptiker);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

#endregion

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetOptikerById/{id}")]
        public async Task<IActionResult> GetOptikerById(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Optiker dbOptiker = await context.FullOptikerQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbOptiker == null)
                    {
                        throw new Exception($"OptikerId {id} doesn't reference a valid location.");
                    }

                    var result = _mapper.Map<OptikerResult>(dbOptiker);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CreateOptiker")]
        public async Task<IActionResult> CreateOptiker(NewEntity newOptiker)
        {
            try
            {
                _logger.LogInformation("CreateOptiker");
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    newOptiker.Login = newOptiker.Login.ToLower();

                    Entities.Optiker dbOptiker = await context.FullOptikerQuery().FirstOrDefaultAsync(o => o.Credentials.Login.ToLower() == newOptiker.Login);
                    if (dbOptiker != null)
                    {
                        throw new Exception($"E-Mail {newOptiker.Login} already exists.");
                    }

                    Entities.Optiker location = new Entities.Optiker();
                    location.Owner.FirstName = newOptiker.FirstName;
                    location.Owner.LastName = newOptiker.LastName;
                    location.Adress.CompanyName = newOptiker.CompanyName;
                    location.Adress.Street = newOptiker.Street;
                    location.Adress.AddressAddition = newOptiker.AddressAddition;
                    location.Adress.PostalCode = newOptiker.PostalCode;
                    location.Adress.City = newOptiker.City;
                    location.Adress.Country = "DE";
                    location.Adress.Phone = newOptiker.Phone;
                    // location.GeoOptiker = _googleMapsFactory.GetOptiker(location);
                    location.QRCodeSalt = Guid.NewGuid().ToString("N").ToUpper();

                    location.Credentials = new Credentials();
                    location.Credentials.Login = newOptiker.Login.ToLower();
                    location.Credentials.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newOptiker.Password, _PASSWORDSALT, true, BCrypt.Net.HashType.SHA384);

                    context.Optikerlist.Add(location);
                    await context.SaveChangesAsync();

                    var newRegistrationToken = new RegistrationToken();
                    newRegistrationToken.Id = location.Id;
                    newRegistrationToken.Token = Guid.NewGuid().ToString("N").ToUpper();

                    context.RegistrationTokens.Add(newRegistrationToken);
                    await context.SaveChangesAsync();

                    var props = new Dictionary<string, string>();
                    props.Add("FIRSTNAME", location.Owner.FirstName);
                    props.Add("LASTNAME", location.Owner.LastName);
                    props.Add("BARNAME", location.Adress.CompanyName);

                    var webHost = _configuration["WebHost"];
                    if (string.IsNullOrWhiteSpace(webHost))
                    {
                        props.Add("LINK", $"https://localhost:44395/registration-complete/{location.Id}/{newRegistrationToken.Token}");
                    }
                    else
                    {
                        props.Add("LINK", $"{webHost}/registration-complete/{location.Id}/{newRegistrationToken.Token}");
                    }

                    var htmlBody = _htmlRenderFactory.RenderHTMLBody("RegistrationEmail", props);
                    var emailResult = _emailFactory.SendEmail(location.Credentials.Login, "Deine Registrierung bei BarBuddy", htmlBody);

                    return Ok(emailResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("CompleteRegistration/{locationId}/{token}")]
        public async Task<IActionResult> CompleteRegistration(long locationId, string token)
        {
            try
            {
                _logger.LogInformation("CompleteRegistration");
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    RegistrationToken dbRegistrationToken = await context.RegistrationTokens.FirstOrDefaultAsync(o => o.Id == locationId && o.Token == token);
                    if (dbRegistrationToken == null)
                    {
                        throw new Exception($"Token {token} is invalid.");
                    }

                    Entities.Optiker dbOptiker = await context.Optikerlist.FirstOrDefaultAsync(o => o.Id == locationId);
                    if (dbOptiker == null)
                    {
                        throw new Exception($"Token {token} is invalid.");
                    }

                    if (dbOptiker.IsActive)
                    {
                        throw new Exception($"Token {token} was already activated.");
                    }

                    context.RegistrationTokens.Remove(dbRegistrationToken);
                    dbOptiker.IsActive = true;

                    await context.SaveChangesAsync();

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("UpdateOptiker")]
        public async Task<IActionResult> UpdateOptiker(OptikerResult location)
        {
            try
            {
                _logger.LogInformation("UpdateOptiker");
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Optiker dbOptiker = await context.FullOptikerQuery().FirstOrDefaultAsync(o => o.Id == location.Id);
                    if (dbOptiker == null)
                    {
                        throw new Exception($"OptikerId {location.Id} doesn't reference a valid location.");
                    }

                    bool updateGeoOptiker = false;
                    if (dbOptiker.Adress.Street != location.Street ||
                        dbOptiker.Adress.PostalCode != location.PostalCode ||
                        dbOptiker.Adress.City != location.City)
                    {
                        updateGeoOptiker = true;
                    }

                    dbOptiker.Owner.FirstName = location.FirstName;
                    dbOptiker.Owner.LastName = location.LastName;
                    dbOptiker.Adress.CompanyName = location.CompanyName;
                    dbOptiker.Adress.Street = location.Street;
                    dbOptiker.Adress.AddressAddition = location.AddressAddition;
                    dbOptiker.Adress.PostalCode = location.PostalCode;
                    dbOptiker.Adress.City = location.City;
                    dbOptiker.Adress.Phone = location.Phone;
                    //if (updateGeoOptiker)
                    //{
                    //    dbOptiker.GeoOptiker = _googleMapsFactory.GetOptiker(dbOptiker);
                    //}
                    await context.SaveChangesAsync();

                    var result = _mapper.Map<OptikerResult>(dbOptiker);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteOptiker/{id}")]
        public async Task<IActionResult> DeleteOptiker(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Optiker dbOptiker = await context.FullOptikerQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbOptiker == null)
                    {
                        throw new Exception($"OptikerId {id} doesn't reference a valid location.");
                    }

                    context.Optikerlist.Remove(dbOptiker);
                    await context.SaveChangesAsync();

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddOrUpdateOptikerSpot")]
        public async Task<IActionResult> AddOrUpdateOptikerImage(OptikerImageResult locationSpot)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
#if DDDD
                    OptikerSpot dbOptikerSpot = null;

                    if (locationSpot.Id == 0)
                    {
                        dbOptikerSpot = new OptikerSpot();
                        dbOptikerSpot.Optiker = await context.Optikerlist.FirstOrDefaultAsync(o => o.Id == locationSpot.OptikerId);
                        if (dbOptikerSpot.Optiker == null)
                        {
                            throw new Exception($"Optiker is null.");
                        }
                        context.OptikerSpots.Add(dbOptikerSpot);
                    }
                    else
                    {
                        dbOptikerSpot = await context.OptikerSpots.FirstOrDefaultAsync(o => o.Id == locationSpot.Id);
                        if (dbOptikerSpot == null)
                        {
                            throw new Exception($"OptikerSpotId { locationSpot.Id} doesn't reference a valid location spot.");
                        }
                    }

                    dbOptikerSpot.Name = locationSpot.Name;
                    dbOptikerSpot.Description = locationSpot.Description;
                    dbOptikerSpot.AreaType = locationSpot.AreaType;
                    dbOptikerSpot.SpotType = locationSpot.SpotType;
                    dbOptikerSpot.MaxPersons = locationSpot.MaxPersons;

                    if (dbOptikerSpot.SpotType == SpotType.Single)
                    {
                        dbOptikerSpot.MaxPersons = 1;
                    }

                    await context.SaveChangesAsync();

                    var result = _mapper.Map<OptikerSpotResult>(dbOptikerSpot);
                    return Ok(result);
#endif
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteOptikerSpot/{id}")]
        public async Task<IActionResult> DeleteOptikerSpot(long id)
        {
#if LATER
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    OptikerSpot dbOptikerSpot = await context.OptikerSpots.FirstOrDefaultAsync(o => o.Id == id);
                    if (dbOptikerSpot == null)
                    {
                        throw new Exception($"OptikerSpotId { id} doesn't reference a valid location spot.");
                    }

                    context.OptikerSpots.Remove(dbOptikerSpot);
                    await context.SaveChangesAsync();

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
#endif
        return Ok(true);
        }

        [HttpPost("DoLogin")]
        public async Task<IActionResult> DoLogin(UserLogin login)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Optiker dbOptiker = await context.FullOptikerQuery().FirstOrDefaultAsync(o => o.Credentials.Login == login.Login);
                    if (dbOptiker == null)
                    {
                        throw new Exception($"Username or password are invalid.");
                    }

                    var isValid = BCrypt.Net.BCrypt.Verify(login.Password, dbOptiker.Credentials.PasswordHash, true, BCrypt.Net.HashType.SHA384);
                    if (!isValid)
                    {
                        throw new Exception($"Username or password are invalid.");
                    }

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, login.Login));

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:ExpiryInDays"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:ValidIssuer"],
                        audience: _configuration["Jwt:ValidAudience"],
                        expires: expiry,
                        claims: claims,
                        signingCredentials: creds
                    );

                    return Ok(new LoginResult
                    {
                        Successful = true,
                        EntityId = dbOptiker.Id,
                        Token = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
