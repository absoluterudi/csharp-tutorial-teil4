using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarBuddyBackend.Services
{
    public class CompanySpotService
    {
        public async Task<List<BarSpot>> GetSpots(Bar l)
        {
            await using var context = new ApplicationDBContext();

            var result = await context.BarSpots.Where(s => s.Location.Id == l.Id).ToListAsync();

            return result;
        }

        public async Task<bool> AddSpot(Bar l, BarSpot newItem)
        {
            await using var context = new ApplicationDBContext();

            var location = await context.Bars.Include(s => s.BarSpots).FirstOrDefaultAsync(s => s.Id == l.Id);
            if (location != default)
            {
                location.BarSpots.Add(newItem);
                await context.SaveChangesAsync();

                return true;
            }


            return false;
        }

        public async Task<bool> UpdateSpot(BarSpot newItem)
        {
            await using var context = new ApplicationDBContext();

            var oldItem = await context.BarSpots.FirstOrDefaultAsync(s => s.Id == newItem.Id);
            if (oldItem != default)
            {
                context.Entry(oldItem).CurrentValues.SetValues(newItem);
                await context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> DeleteSpot(BarSpot newItem)
        {
            await using var context = new ApplicationDBContext();

            var oldItem = await context.BarSpots.FirstOrDefaultAsync(s => s.Id == newItem.Id);
            if (oldItem != default)
            {
                context.BarSpots.Remove(oldItem);
                await context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> AddFundusImage(Optiker l, GlaukomImage newItem)
        {
            await using var context = new ApplicationDBContext();

            var optiker = await context.Optikerlist.Include(s => s.FundusImages).FirstOrDefaultAsync(s => s.Id == l.Id);
            if (optiker != default)
            {
                optiker.FundusImages.Add(newItem);
                await context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteImage(GlaukomImage newItem)
        {
            await using var context = new ApplicationDBContext();

            var oldItem = await context.FundusImages.FirstOrDefaultAsync(s => s.Id == newItem.Id);
            if (oldItem != default)
            {
                context.FundusImages.Remove(oldItem);
                await context.SaveChangesAsync();

                return true;
            }

            return false;

        }
    }
}
