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
    public class BarController : ControllerBase
    {
        private readonly string _PASSWORDSALT = "$2a$11$78B4w1CY64crLACSasMKee";

        private readonly ILogger<BarController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly GoogleMapsFactory _googleMapsFactory;
        private readonly EmailFactory _emailFactory;
        private readonly HtmlRenderFactory _htmlRenderFactory;

        public BarController(ILogger<BarController> logger, IMapper mapper, IConfiguration configuration,
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

        [HttpPost("GetLocationsWithinRadius")]
        public async Task<IActionResult> GetLocationsWithinRadius(CurrentPosition currentPosition)
        {
            try
            {
                var list = new List<BarResult>();

                var myPoint = new Point(currentPosition.MyLongitude, currentPosition.MyLatitude) { SRID = GoogleMapsFactory.SRID };
                var centerPoint = new Point(currentPosition.CenterLongitude, currentPosition.CenterLatitude) { SRID = GoogleMapsFactory.SRID };
                var distanceInMetres = currentPosition.Radius * 1000;

                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    var locationWithDistance = await context.FullLocationQuery(true).Where(o => o.IsActive && o.Adress.GeoLocation.IsWithinDistance(centerPoint, distanceInMetres))
                                                                             .Select(o => new { location = o, distance = o.Adress.GeoLocation.Distance(myPoint) }).ToListAsync();

                    var locations = locationWithDistance.Select(o => o.location).ToList();
                    list = _mapper.Map<List<BarResult>>(locations);

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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetLocationWithSpotInfos/{id}")]
        public async Task<IActionResult> GetLocationWithSpotInfos(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.FullLocationQuery(true).FirstOrDefaultAsync(o => o.Id == id);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {id} doesn't reference a valid location.");
                    }

                    var result = _mapper.Map<BarResult>(dbLocation);
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
        [HttpGet("GetLocationById/{id}")]
        public async Task<IActionResult> GetLocationById(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.FullLocationQuery(true).FirstOrDefaultAsync(o => o.Id == id);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {id} doesn't reference a valid location.");
                    }

                    var result = _mapper.Map<BarResult>(dbLocation);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CreateLocation")]
        public async Task<IActionResult> CreateLocation(NewEntity newLocation)
        {
            try
            {
                _logger.LogInformation("CreateLocation");
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    newLocation.Login = newLocation.Login.ToLower();

                    Entities.Bar dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Credentials.Login.ToLower() == newLocation.Login);
                    if (dbLocation != null)
                    {
                        throw new Exception($"E-Mail {newLocation.Login} already exists.");
                    }

                    Entities.Bar location = new Entities.Bar();
                    location.Owner = new Contact();
                    location.Owner.FirstName = newLocation.FirstName;
                    location.Owner.LastName = newLocation.LastName;
                    location.Adress.CompanyName = newLocation.CompanyName;
                    location.Adress.Street = newLocation.Street;
                    location.Adress.AddressAddition = newLocation.AddressAddition;
                    location.Adress.PostalCode = newLocation.PostalCode;
                    location.Adress.City = newLocation.City;
                    location.Adress.Country = "DE";
                    location.Adress.Phone = newLocation.Phone;
                    location.Adress.GeoLocation = _googleMapsFactory.GetLocation(location.Adress);
                    location.QRCodeSalt = Guid.NewGuid().ToString("N").ToUpper();

                    location.Credentials = new Credentials();
                    location.Credentials.Login = newLocation.Login.ToLower();
                    location.Credentials.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newLocation.Password, _PASSWORDSALT, true, BCrypt.Net.HashType.SHA384);

                    context.Bars.Add(location);
                    await context.SaveChangesAsync();

                    var newRegistrationToken = new RegistrationToken();
                    newRegistrationToken.EntityId = location.Id;
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
                    RegistrationToken dbRegistrationToken = await context.RegistrationTokens.FirstOrDefaultAsync(o => o.EntityId == locationId && o.Token == token);
                    if (dbRegistrationToken == null)
                    {
                        throw new Exception($"Token {token} is invalid.");
                    }

                    Entities.Bar dbLocation = await context.Bars.FirstOrDefaultAsync(o => o.Id == locationId);
                    if (dbLocation == null)
                    {
                        throw new Exception($"Token {token} is invalid.");
                    }

                    if (dbLocation.IsActive)
                    {
                        throw new Exception($"Token {token} was already activated.");
                    }

                    context.RegistrationTokens.Remove(dbRegistrationToken);
                    dbLocation.IsActive = true;

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
        [HttpPost("UpdateLocation")]
        public async Task<IActionResult> UpdateLocation(BarResult location)
        {
            try
            {
                _logger.LogInformation("UpdateLocation");
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Id == location.Id);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {location.Id} doesn't reference a valid location.");
                    }

                    bool updateGeoLocation = false;
                    if (dbLocation.Adress.Street != location.Street ||
                        dbLocation.Adress.PostalCode != location.PostalCode ||
                        dbLocation.Adress.City != location.City)
                    {
                        updateGeoLocation = true;
                    }

                    dbLocation.Owner.FirstName = location.FirstName;
                    dbLocation.Owner.LastName = location.LastName;
                    dbLocation.Adress.CompanyName = location.BarName;
                    dbLocation.Adress.Street = location.Street;
                    dbLocation.Adress.AddressAddition = location.AddressAddition;
                    dbLocation.Adress.PostalCode = location.PostalCode;
                    dbLocation.Adress.City = location.City;
                    dbLocation.Adress.Phone = location.Phone;
                    dbLocation.GooglePlusCode = location.GooglePlusCode;
                    if (updateGeoLocation)
                    {
                        dbLocation.Adress.GeoLocation = _googleMapsFactory.GetLocation(dbLocation.Adress);
                    }

                    await context.SaveChangesAsync();

                    var result = _mapper.Map<BarResult>(dbLocation);
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
        [HttpDelete("DeleteLocation/{id}")]
        public async Task<IActionResult> DeleteLocation(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {id} doesn't reference a valid location.");
                    }

                    context.Bars.Remove(dbLocation);
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
        [HttpPost("AddOrUpdateLocationSpot")]
        public async Task<IActionResult> AddOrUpdateLocationSpot(BarSpotResult locationSpot)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    BarSpot dbLocationSpot = null;

                    if (locationSpot.Id == 0)
                    {
                        dbLocationSpot = new BarSpot();
                        dbLocationSpot.Location = await context.Bars.FirstOrDefaultAsync(o => o.Id == locationSpot.LocationId);
                        if (dbLocationSpot.Location == null)
                        {
                            throw new Exception($"Location is null.");
                        }
                        context.BarSpots.Add(dbLocationSpot);
                    }
                    else
                    {
                        dbLocationSpot = await context.BarSpots.FirstOrDefaultAsync(o => o.Id == locationSpot.Id);
                        if (dbLocationSpot == null)
                        {
                            throw new Exception($"LocationSpotId { locationSpot.Id} doesn't reference a valid location spot.");
                        }
                    }

                    dbLocationSpot.Name = locationSpot.Name;
                    dbLocationSpot.Description = locationSpot.Description;
                    dbLocationSpot.AreaType = locationSpot.AreaType;
                    dbLocationSpot.SpotType = locationSpot.SpotType;
                    dbLocationSpot.MaxPersons = locationSpot.MaxPersons;

                    if (dbLocationSpot.SpotType == SpotType.Single)
                    {
                        dbLocationSpot.MaxPersons = 1;
                    }

                    await context.SaveChangesAsync();

                    var result = _mapper.Map<BarSpotResult>(dbLocationSpot);
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
        [HttpDelete("DeleteLocationSpot/{id}")]
        public async Task<IActionResult> DeleteLocationSpot(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    BarSpot dbLocationSpot = await context.BarSpots.FirstOrDefaultAsync(o => o.Id == id);
                    if (dbLocationSpot == null)
                    {
                        throw new Exception($"LocationSpotId { id} doesn't reference a valid location spot.");
                    }

                    context.BarSpots.Remove(dbLocationSpot);
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

        [HttpPost("DoLogin")]
        public async Task<IActionResult> DoLogin(UserLogin login)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Credentials.Login == login.Login);
                    if (dbLocation == null)
                    {
                        throw new Exception($"Username or password are invalid.");
                    }

                    var isValid = BCrypt.Net.BCrypt.Verify(login.Password, dbLocation.Credentials.PasswordHash, true, BCrypt.Net.HashType.SHA384);
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
                        EntityId = dbLocation.Id,
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
