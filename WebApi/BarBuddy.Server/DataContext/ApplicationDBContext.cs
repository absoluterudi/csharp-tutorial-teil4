using BarBuddy.Server.Entities;
using BarBuddy.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BarBuddy.Server.DataContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base()
        {
        }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public virtual DbSet<Bar> Bars { get; set; }
        public virtual DbSet<BarSpot> BarSpots { get; set; }

        public virtual DbSet<Optiker> Optikerlist { get; set; }
        public virtual DbSet<Augenarzt> Augenarztlist { get; set; }
        public virtual DbSet<GlaukomImage> FundusImages { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<RegistrationToken> RegistrationTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Bar>(table =>
            {
                table.OwnsOne(x => x.Credentials, credentials =>
                                                  {
                                                      credentials.Property(x => x.Login).HasColumnName("Credentials_Login");
                                                      credentials.Property(x => x.PasswordHash).HasColumnName("Credentials_PasswordHash");
                                                  })
                     .OwnsOne(x => x.Owner, credentials =>
                                            {
                                                credentials.Property(x => x.FirstName).HasColumnName("Owner_FirstName");
                                                credentials.Property(x => x.LastName).HasColumnName("Owner_LastName");
                                             })
                     .OwnsOne(x => x.Adress, credentials =>
                                             {
                                                credentials.Property(x => x.Street).HasColumnName("Adress_Street");
                                                credentials.Property(x => x.AddressAddition).HasColumnName("Adress_AdressAddition");
                                                credentials.Property(x => x.PostalCode).HasColumnName("Adress_Postal");
                                                credentials.Property(x => x.City).HasColumnName("Adress_City");
                                                credentials.Property(x => x.Country).HasColumnName("Adress_Country");
                                             }
                            );
            });
            builder.Entity<Optiker>(table =>
            {
                table.OwnsOne(x => x.Credentials, credentials =>
                {
                    credentials.Property(x => x.Login).HasColumnName("Credentials_Login");
                    credentials.Property(x => x.PasswordHash).HasColumnName("Credentials_PasswordHash");
                })
                .OwnsOne(x => x.Owner, credentials =>
                {
                    credentials.Property(x => x.FirstName).HasColumnName("Owner_FirstName");
                    credentials.Property(x => x.LastName).HasColumnName("Owner_LastName");
                })
                .OwnsOne(x => x.Adress, credentials =>
                {
                    credentials.Property(x => x.Street).HasColumnName("Adress_Street");
                    credentials.Property(x => x.AddressAddition).HasColumnName("Adress_AdressAddition");
                    credentials.Property(x => x.PostalCode).HasColumnName("Adress_Postal");
                    credentials.Property(x => x.City).HasColumnName("Adress_City");
                    credentials.Property(x => x.Country).HasColumnName("Adress_Country");
                }
            );
            });
            builder.Entity<Augenarzt>(table =>
            {
                table.OwnsOne(x => x.Credentials, credentials =>
                {
                    credentials.Property(x => x.Login).HasColumnName("Credentials_Login");
                    credentials.Property(x => x.PasswordHash).HasColumnName("Credentials_PasswordHash");
                })
                     .OwnsOne(x => x.Owner, credentials =>
                     {
                         credentials.Property(x => x.FirstName).HasColumnName("Owner_FirstName");
                         credentials.Property(x => x.LastName).HasColumnName("Owner_LastName");
                     })
                     .OwnsOne(x => x.Adress, credentials =>
                      {
                          credentials.Property(x => x.Street).HasColumnName("Adress_Street");
                          credentials.Property(x => x.AddressAddition).HasColumnName("Adress_AdressAddition");
                          credentials.Property(x => x.PostalCode).HasColumnName("Adress_Postal");
                          credentials.Property(x => x.City).HasColumnName("Adress_City");
                          credentials.Property(x => x.Country).HasColumnName("Adress_Country");
                      }
                            );
            });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                                                       .AddJsonFile("appsettings.json", optional: false);

                var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (!string.IsNullOrEmpty(envName))
                {
                    configurationBuilder = configurationBuilder.AddJsonFile($"appsettings.{envName}.json", optional: false);
                }

                IConfigurationRoot configuration = configurationBuilder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite());
            }
        }
        public IQueryable<Bar> FullLocationQuery(bool withReservations = false)
        {
            if (withReservations)
            {
                return Bars.Include(x => x.BarSpots).ThenInclude(o => o.Reservations.Where(o => !o.CheckOutTime.HasValue));
            }
            return Bars.Include(x => x.BarSpots);
        }

        public IQueryable<Optiker> FullOptikerQuery()
        {
            return Optikerlist.Include(x => x.FundusImages);
        }
        public IQueryable<Augenarzt> FullAugenarztQuery()
        {
            return Augenarztlist;
        }

        public IQueryable<Reservation> FullReservationQuery()
        {
            return Reservations.Include(o => o.LocationSpot).ThenInclude(o => o.Location);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var addedEntities = ChangeTracker.Entries<IAuditable>().Where(E => E.State == EntityState.Added).ToList();
            addedEntities.ForEach(E =>
            {
                E.Entity.CreationDate = DateTime.Now;
                E.Entity.ModificationDate = DateTime.Now;
                E.Entity.ModifiedBy = this.GetCurrentUserId();
                E.Entity.CreatedBy = this.GetCurrentUserId();
            });

            var ModifiedEntities = ChangeTracker.Entries<IAuditable>().Where(E => E.State == EntityState.Modified).ToList();
            ModifiedEntities.ForEach(E =>
            {
                E.Entity.ModificationDate = DateTime.Now;
                E.Entity.ModifiedBy = this.GetCurrentUserId();
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private int GetCurrentUserId()
        {
            return 0;
        }
    }
}
