using AutoMapper;
using BarBuddy.DTOs;
using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IMapper _mapper;

        public ReservationController(ILogger<ReservationController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetReservationById/{id}")]
        public async Task<IActionResult> GetReservationById(long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Reservation dbReservation = await context.FullReservationQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbReservation == null)
                    {
                        throw new NullReferenceException($"ReservationId {id} doesn't reference a valid reservation.");
                    }

                    var result = _mapper.Map<ReservationResult>(dbReservation);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CreateReservation")]
        public async Task<IActionResult> CreateReservation(NewReservation newReservation)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    if (newReservation.LocationId == 0 || newReservation.LocationSpotId == 0)
                    {
                        throw new Exception($"EntityId or LocationSpotId is null.");
                    }

                    Reservation reservation = new Reservation();
                    reservation.Number = Guid.NewGuid().ToString("N").ToUpper();
                    reservation.CountPerson = newReservation.CountPerson;
                    reservation.ReservedUntil = newReservation.ReservedUntil;

                    Bar dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Id == newReservation.LocationId);
                    if (dbLocation == null)
                    {
                        throw new Exception($"EntityId {newReservation.LocationId} doesn't reference a valid location.");
                    }

                    var dbLocationSpot = dbLocation.BarSpots.FirstOrDefault(o => o.Id == newReservation.LocationSpotId);
                    if (dbLocationSpot == null)
                    {
                        throw new Exception($"LocationSpotId {newReservation.LocationSpotId} doesn't reference a valid location spot.");
                    }

                    if (dbLocationSpot.MaxPersons > newReservation.CountPerson)
                    {
                        reservation.CountPerson = dbLocationSpot.MaxPersons;
                    }

                    reservation.LocationSpot = dbLocationSpot;

                    context.Reservations.Add(reservation);
                    await context.SaveChangesAsync();

                    var result = _mapper.Map<ReservationResult>(reservation);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CheckQRCode")]
        public async Task<IActionResult> CheckQRCode(long reservationId, string qrCode)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Reservation dbReservation = await context.FullReservationQuery().FirstOrDefaultAsync(o => o.Id == reservationId);
                    if (dbReservation == null)
                    {
                        throw new NullReferenceException($"ReservationId {reservationId} doesn't reference a valid reservation.");
                    }

                    //var isValid = dbReservation.Location.QRCodeSalt == qrCode;
                    //return Ok(isValid);
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Reservation dbReservation = await context.FullReservationQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbReservation == null)
                    {
                        throw new Exception($"ReservationId {id} doesn't reference a valid reservation.");
                    }

                    if (!dbReservation.CheckInTime.HasValue)
                    {
                        dbReservation.CheckInTime = DateTime.Now;
                        await context.SaveChangesAsync();
                    }

                    var result = _mapper.Map<ReservationResult>(dbReservation);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Reservation dbReservation = await context.FullReservationQuery().FirstOrDefaultAsync(o => o.Id == id);
                    if (dbReservation == null)
                    {
                        throw new Exception($"ReservationId {id} doesn't reference a valid reservation.");
                    }

                    if (!dbReservation.CheckOutTime.HasValue)
                    {
                        dbReservation.CheckOutTime = DateTime.Now;
                        await context.SaveChangesAsync();
                    }

                    var result = _mapper.Map<ReservationResult>(dbReservation);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CancelReservation")]
        public async Task<IActionResult> CancelReservation([FromBody] long id)
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Reservation dbReservation = await context.Reservations.FirstOrDefaultAsync(o => o.Id == id);
                    if (dbReservation == null)
                    {
                        return Ok(true);
                    }

                    context.Reservations.Remove(dbReservation);
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
    }
}
