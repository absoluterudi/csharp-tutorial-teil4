using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend.Services
{
    public class CompanyService
    {
        public async Task<List<Bar>> GetBars()
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Bars.Include(s=>s.BarSpots).ToListAsync();

            return result;
        }
        public async Task<List<Bar>> GetSingleBar (long Id)
        {
            await using var context = new ApplicationDBContext();
            var result = await context.Bars.Where(x => x.Id == Id).Include(s => s.BarSpots).ToListAsync();

            return result;
        }


        public async Task<bool> AddBar(Bar newItem)
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Bars.AddAsync(newItem);

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBar(Bar newItem)
        {
            await using var context = new ApplicationDBContext();

            var oldItem = await context.Bars.FirstOrDefaultAsync(s => s.Id == newItem.Id);
            if(oldItem != default)
            {
                context.Entry(oldItem).CurrentValues.SetValues(newItem);
                await context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<List<Reservation>> GetReservations( long locationId )
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Reservations.Where(x=>x.LocationSpot.Location.Id == locationId).ToListAsync();
            return result;
        }

        public async Task<List<Augenarzt>> GetAllAugenaerzte()
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Augenarztlist.ToListAsync();

            return result;
        }
        public async Task<bool> AddAugenarzt(Augenarzt newItem)
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Augenarztlist.AddAsync(newItem);

            await context.SaveChangesAsync();

            return true;
        }


        public async Task<List<Optiker>> GetAllOptikers ()
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Optikerlist.Include(s => s.FundusImages).ToListAsync();

            return result;
        }
        public async Task<bool> AddOptiker(Optiker newItem)
        {
            await using var context = new ApplicationDBContext();

            var result = await context.Optikerlist.AddAsync(newItem);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
