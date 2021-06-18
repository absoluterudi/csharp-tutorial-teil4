using AutoMapper;
using BarBuddy.DTOs;
using BarBuddy.DTOs.Helper;
using BarBuddy.Server.DataContext;
using BarBuddy.Server.Factories;
using HtmlRendererCore.PdfSharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly ILogger<PdfController> _logger;
        private readonly IMapper _mapper;
        private readonly HtmlRenderFactory _htmlRenderFactory;

        public PdfController(ILogger<PdfController> logger, IMapper mapper, HtmlRenderFactory htmlRenderFactory)
        {
            _logger = logger;
            _mapper = mapper;
            _htmlRenderFactory = htmlRenderFactory;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("QRCodeDoor/{locationId}")]
        public async Task<IActionResult> QRCodeDoor(long locationId)
        {
            try
            {
                var props = new Dictionary<string, string>();

                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.Bars.FirstOrDefaultAsync(o => o.Id == locationId);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {locationId} doesn't reference a valid location.");
                    }

                    props = new Dictionary<string, string>();
                    props.Add("BARNAME", dbLocation.Adress.CompanyName);
                    props.Add("QRCODEIMAGE", QRCodeHelper.CreateQrCodeForDoor(dbLocation.QRCodeSalt));
                }

                var htmlBody = _htmlRenderFactory.RenderHTMLBody("QrCodeDoor", props);

                using (var stream = new MemoryStream())
                {
                    var pdf = PdfGenerator.GeneratePdf(htmlBody, PdfSharpCore.PageSize.A4);
                    pdf.Save(stream);

                    var base64String = Convert.ToBase64String(stream.ToArray());
                    return Ok(base64String);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("QRCodesAllSpots/{locationId}")]
        public async Task<IActionResult> QRCodesAllSpots(long locationId)
        {
            try
            {
                List<QRCodeSpotResult> pdfList = new List<QRCodeSpotResult>();

                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.Bars.Include(o => o.BarSpots).FirstOrDefaultAsync(o => o.Id == locationId);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {locationId} doesn't reference a valid location.");
                    }

                    foreach (var spot in dbLocation.BarSpots)
                    {
                        var props = new Dictionary<string, string>();
                        props.Add("SPOTNAME", spot.Name);
                        props.Add("QRCODEIMAGE", QRCodeHelper.CreateQrCodeForSpot(dbLocation.QRCodeSalt, spot.Id));

                        var htmlBody = _htmlRenderFactory.RenderHTMLBody("QrCodeSpot", props);

                        using (var stream = new MemoryStream())
                        {
                            var pdf = PdfGenerator.GeneratePdf(htmlBody, PdfSharpCore.PageSize.A4);
                            pdf.Save(stream);

                            var base64String = Convert.ToBase64String(stream.ToArray());
                            pdfList.Add(new QRCodeSpotResult
                            {
                                SpotId = spot.Id,
                                SpotName = spot.Name,
                                QRCode = base64String
                            });
                        }
                    }
                }

                return Ok(pdfList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("QRCodeSpot/{locationId}/{spotId}")]
        public async Task<IActionResult> QRCodeSpot(long locationId, long spotId)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Entities.Bar dbLocation = await context.Bars.Include(o => o.BarSpots).FirstOrDefaultAsync(o => o.Id == locationId);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {locationId} doesn't reference a valid location.");
                    }

                    var spot = dbLocation.BarSpots.FirstOrDefault(o => o.Id == spotId);
                    if (spot == null)
                    {
                        throw new Exception($"SpotId {spotId} doesn't reference a valid spot.");
                    }

                    var props = new Dictionary<string, string>();
                    props.Add("SPOTNAME", spot.Name);
                    props.Add("QRCODEIMAGE", QRCodeHelper.CreateQrCodeForSpot(dbLocation.QRCodeSalt, spot.Id));

                    var htmlBody = _htmlRenderFactory.RenderHTMLBody("QrCodeSpot", props);

                    using (var stream = new MemoryStream())
                    {
                        var pdf = PdfGenerator.GeneratePdf(htmlBody, PdfSharpCore.PageSize.A4);
                        pdf.Save(stream);

                        var model = new QRCodeSpotResult
                        {
                            SpotId = spot.Id,
                            SpotName = spot.Name,
                            QRCode = Convert.ToBase64String(stream.ToArray())
                        };

                        return Ok(model);
                    }
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
