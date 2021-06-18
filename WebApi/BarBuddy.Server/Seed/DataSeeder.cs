using BarBuddy.DTOs;
using BarBuddy.Server.DataContext;
using BarBuddy.Server.Entities;
using BarBuddy.Server.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BarBuddy.Server.Seed
{
    public static class DataSeeder
    {
        private static string _salt = "$2a$11$78B4w1CY64crLACSasMKee";

        public static void AddOptiker(ApplicationDBContext context,
                            GoogleMapsFactory _googleMapsFactory,
                            string name,
                            string street,
                            string zip,
                            string city,
                            string phone,
                            string firstname,
                            string lastname,
                            string email,
                            string pw,
                            string path_and_filename_fundusimage,  
                            string kundennummer)
        {
            NewEntity newLocation = new NewEntity();

            newLocation.Login = email;
            newLocation.Password = pw;
            //Entities.Location dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Credentials.Login.ToLower() == newLocation.Login);
            //if (dbLocation != null)
            //{
            //    throw new Exception($"E-Mail {newLocation.Login} already exists.");
            //}
            Entities.Optiker optiker = new Entities.Optiker();
            optiker.Owner = new Contact();
            optiker.Owner.FirstName = firstname;
            optiker.Owner.LastName = firstname;
            optiker.Credentials = new Credentials();
            optiker.Credentials.Login = newLocation.Login.ToLower();
            optiker.Credentials.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newLocation.Password, _salt, true, BCrypt.Net.HashType.SHA384);
            //-----------------------------------------------------
            optiker.Adress = new Adress();
            optiker.Adress.CompanyName = name;
            optiker.Adress.Street = street;
            optiker.Adress.AddressAddition = "";
            optiker.Adress.PostalCode = zip;
            optiker.Adress.City = city;
            optiker.Adress.Country = "DE";
            optiker.Adress.Phone = phone;
            optiker.IsActive = true;
            optiker.GooglePlusCode = "";
            // apotheke.GeoLocation = _googleMapsFactory.GetLocation(apotheke);
            // apotheke.QRCodeSalt = Guid.NewGuid().ToString("N").ToUpper();

            context.Optikerlist.Add(optiker);
            context.SaveChanges();

            string org_path = Directory.GetCurrentDirectory();
            GlaukomImage gc1 = new GlaukomImage();
            gc1.Optiker = optiker;
            gc1.CreatedBy = 1;
            gc1.CreationDate = DateTime.Now;
            gc1.Filename = path_and_filename_fundusimage;
            gc1.Kundennummer = kundennummer;
            gc1.ModificationDate = DateTime.Now;
            gc1.ModifiedBy = 0;
            if (File.Exists(path_and_filename_fundusimage))
            {
                string filename = Path.GetFileName(path_and_filename_fundusimage);
                byte[] rawfiledata = File.ReadAllBytes(path_and_filename_fundusimage);
                bool result = BarBuddy.Server.Helper.FormFileHelper.LoadBlob(gc1, rawfiledata, filename);
            }
            context.FundusImages.Add(gc1);
            context.SaveChanges();
        }
        public static void AddAugenarzt(ApplicationDBContext context,
                            GoogleMapsFactory _googleMapsFactory,
                            string name,
                            string street,
                            string zip,
                            string city,
                            string phone,
                            string firstname,
                            string lastname,
                            string email,
                            string pw )
        {
            NewEntity newLocation = new NewEntity();

            newLocation.Login = email;
            newLocation.Password = pw;
            //Entities.Location dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Credentials.Login.ToLower() == newLocation.Login);
            //if (dbLocation != null)
            //{
            //    throw new Exception($"E-Mail {newLocation.Login} already exists.");
            //}

            Entities.Augenarzt augenarzt = new Entities.Augenarzt();
            augenarzt.Owner = new Contact();
            augenarzt.Owner.FirstName = firstname;
            augenarzt.Owner.LastName = firstname;
            //-----------------------------------
            augenarzt.Credentials = new Credentials();
            augenarzt.Credentials.Login = newLocation.Login.ToLower(); 
            augenarzt.Credentials.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newLocation.Password, _salt, true, BCrypt.Net.HashType.SHA384);
            //-----------------------------------
            augenarzt.Adress = new Adress();
            augenarzt.Adress.CompanyName = name;
            augenarzt.Adress.Street = street;
            augenarzt.Adress.AddressAddition = "";
            augenarzt.Adress.PostalCode = zip;
            augenarzt.Adress.City = city;
            augenarzt.Adress.Country = "DE";
            augenarzt.Adress.Phone = phone;
            augenarzt.IsActive = true;
            // apotheke.GeoLocation = _googleMapsFactory.GetLocation(apotheke);
            // apotheke.QRCodeSalt = Guid.NewGuid().ToString("N").ToUpper();

            context.Augenarztlist.Add(augenarzt);
            context.SaveChanges();
        }

 	public static void AddBar(ApplicationDBContext context,
                                    GoogleMapsFactory _googleMapsFactory,
                                    string barname,
                                    string street,
                                    string zip,
                                    string city,
                                    string phone,
                                    string firstname,
                                    string lastname,
                                    string email,
                                    string pw )
        {
            NewEntity newLocation = new NewEntity();

            newLocation.Login = email;
            newLocation.Password = pw;
            //Entities.Location dbLocation = await context.FullLocationQuery().FirstOrDefaultAsync(o => o.Credentials.Login.ToLower() == newLocation.Login);
            //if (dbLocation != null)
            //{
            //    throw new Exception($"E-Mail {newLocation.Login} already exists.");
            //}

            Entities.Bar bar = new Entities.Bar();
            bar.Owner = new Contact();
            bar.Owner.FirstName = firstname;
            bar.Owner.LastName = lastname;
            bar.Adress = new Adress();
            bar.Adress.CompanyName = barname;
            bar.Adress.Street = street;
            bar.Adress.AddressAddition = "";
            bar.Adress.PostalCode = zip;
            bar.Adress.City = city;
            bar.Adress.Country = "DE";
            bar.Adress.Phone = phone;
            bar.Adress.GeoLocation = _googleMapsFactory.GetLocation(bar.Adress);
            bar.IsActive = true;
            bar.GooglePlusCode = "";
            
            bar.QRCodeSalt = Guid.NewGuid().ToString("N").ToUpper();

            bar.Credentials = new Credentials();
            bar.Credentials.Login = newLocation.Login.ToLower();
            bar.Credentials.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newLocation.Password, _salt, true, BCrypt.Net.HashType.SHA384);

            context.Bars.Add(bar);
            // await context.SaveChangesAsync();
            context.SaveChanges();

            BarSpot lc1 = new BarSpot();
            lc1.Location = bar;
            lc1.CreatedBy = 1;
            lc1.CreationDate = DateTime.Now;
            lc1.MaxPersons = 1;
            lc1.Name = "Single 1";
            lc1.Description = "Single 1";
            lc1.ModificationDate = DateTime.Now;
            lc1.ModifiedBy = 0;
            lc1.SpotType = DTOs.Enums.SpotType.Single;
            lc1.AreaType = DTOs.Enums.AreaType.Inside;
            context.BarSpots.Add(lc1);
            context.SaveChanges();

            BarSpot lc2 = new BarSpot();
            lc2.Location = bar;
            lc2.CreatedBy = 1;
            lc2.CreationDate = DateTime.Now;
            lc2.MaxPersons = 1;
            lc2.Name = "Single 2";
            lc2.Description = "Single 2";
            lc2.ModificationDate = DateTime.Now;
            lc2.ModifiedBy = 0;
            lc2.SpotType = DTOs.Enums.SpotType.Single;
            lc2.AreaType = DTOs.Enums.AreaType.Inside;
            context.BarSpots.Add(lc2);
            context.SaveChanges();

            BarSpot lc3 = new BarSpot();
            lc3.Location = bar;
            lc3.CreatedBy = 1;
            lc3.CreationDate = DateTime.Now;
            lc3.MaxPersons = 3;
            lc3.Name = "Tisch 3 Isabelle";
            lc3.Description = "Tisch 3 Isabelle";
            lc3.ModificationDate = DateTime.Now;
            lc3.ModifiedBy = 0;
            lc3.SpotType = DTOs.Enums.SpotType.Table;
            lc3.AreaType = DTOs.Enums.AreaType.Inside;
            context.BarSpots.Add(lc3);
            context.SaveChanges();

            BarSpot lc4 = new BarSpot();
            lc4.Location = bar;
            lc4.CreatedBy = 1;
            lc4.CreationDate = DateTime.Now;
            lc4.MaxPersons = 4;
            lc4.Name = "Tisch 4 Isabelle";
            lc4.Description = "Tisch 4 Isabelle";
            lc4.ModificationDate = DateTime.Now;
            lc4.ModifiedBy = 0;
            lc4.SpotType = DTOs.Enums.SpotType.Table;
            lc4.AreaType = DTOs.Enums.AreaType.Inside;
            context.BarSpots.Add(lc4);
            context.SaveChanges();

            BarSpot lc5 = new BarSpot();
            lc5.Location = bar;
            lc5.CreatedBy = 1;
            lc5.CreationDate = DateTime.Now;
            lc5.MaxPersons = 6;
            lc5.Name = "Tisch Chantal";
            lc5.Description = "Tisch Chantal";
            lc5.ModificationDate = DateTime.Now;
            lc5.ModifiedBy = 0;
            lc5.SpotType = DTOs.Enums.SpotType.Table;
            lc5.AreaType = DTOs.Enums.AreaType.Outside;
            context.BarSpots.Add(lc5);
            context.SaveChanges();

            Reservation res1 = new Reservation();
            // res1.LocationSpotId = lc3.Id;
            res1.LocationSpot = lc3;
            res1.CountPerson = 3;
            res1.CreationDate = DateTime.Now;
            res1.ModificationDate = DateTime.Now;
            res1.ModifiedBy = 0;
            res1.Number = Guid.NewGuid().ToString("N").ToUpper();
            DateTime dt = DateTime.Now;
            res1.CheckInTime = dt.AddMinutes(20);
            context.Reservations.Add(res1);
            context.SaveChanges();

            Reservation res2 = new Reservation();
            res2.LocationSpot = lc4;
            res2.CountPerson = 4;
            res2.CreationDate = DateTime.Now;
            res2.ModificationDate = DateTime.Now;
            res2.CheckInTime = dt.AddMinutes(20);
            res2.ModifiedBy = 0;
            res2.CreatedBy = 0;
            res2.Number = Guid.NewGuid().ToString("N").ToUpper();
            dt = DateTime.Now;
            context.Reservations.Add(res2);
            context.SaveChanges();

            var newRegistrationToken = new RegistrationToken();
            newRegistrationToken.EntityId = bar.Id;
            newRegistrationToken.Token = Guid.NewGuid().ToString("N").ToUpper();

            context.RegistrationTokens.Add(newRegistrationToken);
            // await context.SaveChangesAsync();
            context.SaveChanges();
        }

        public static void SeedData(this IHost host)
        {
            // return;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();
                GoogleMapsFactory _googleMapsFactory = services.GetRequiredService<GoogleMapsFactory>();
                context.Database.EnsureCreated();

                if (!context.Optikerlist.Any())
                {
                    if (true)
                    {
                        AddOptiker(context, _googleMapsFactory,
                                                    "Mörg Optiker",
                                                    "Gormannstrasse 19",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+1@absolute.de",
                                                    "12345678",
                                                    "FundusImage1.jpg",   // Name des Fundusbilds 
                                                    "1");   // Kundennummer 
                        AddOptiker(context, _googleMapsFactory,
                                                    "Rüd Optiker",
                                                    "Torstrasse 89",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+2@absolute.de",
                                                    "12345678",
                                                    "FundusImage2.jpg",   // Name des Fundusbilds 
                                                    "2");   // Kundennummer 
                        AddOptiker(context, _googleMapsFactory,
                                                    "Mikkeller Optiker",
                                                    "Torstrasse 89",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+2@absolute.de",
                                                    "12345678",
                                                    "FundusImage3.jpg",   // Name des Fundusbilds 
                                                    "10");   // Kundennummer 
                        AddAugenarzt(context, _googleMapsFactory,
                                                    "Augenarzt Drazen",
                                                    "Gormannstrasse 19",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+1@absolute.de",
                                                    "12345678");
                        AddAugenarzt(context, _googleMapsFactory,
                                                    "Augenarzt Omar",
                                                    "Torstrasse 89",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+2@absolute.de",
                                                    "12345678");
                        AddAugenarzt(context, _googleMapsFactory,
                                                    "Augenarzt Timothy",
                                                    "Torstrasse 89",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+2@absolute.de",
                                                    "12345678");   // Kundennummer 
                        AddBar(context, _googleMapsFactory,
                                                    "Schmittz",
                                                    "Gormannstrasse 19",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+1@absolute.de",
                                                    "12345678");
                        AddBar(context, _googleMapsFactory,
                                                    "Neue Odessa Bar",
                                                    "Torstrasse 89",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+2@absolute.de",
                                                    "12345678");
                        AddBar(context, _googleMapsFactory,
                                                    "Mikkeller Berlin",
                                                    "Torstrasse 102",
                                                    "10119",
                                                    "Berlin",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+3@absolute.de",
                                                    "12345678");
                        AddBar(context, _googleMapsFactory,
                                                    "Absolute Software GmbH",
                                                    "Jungfernstieg 49",
                                                    "20354",
                                                    "Hamburg",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+4@absolute.de",
                                                    "12345678");
                        AddBar(context, _googleMapsFactory,
                                                    "Meyer Lansky's",
                                                    "Gänsemarkt 36",
                                                    "20354",
                                                    "Hamburg",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+5@absolute.de",
                                                    "12345678");
                        AddBar(context, _googleMapsFactory,
                                                    "Brandy Melville",
                                                    "Jungfernstie 45",
                                                    "20354",
                                                    "Hamburg",
                                                    "01728045327",
                                                    "Tim",
                                                    "Thaler",
                                                    "hoefert+6@absolute.de",
                                                    "12345678");

                    }
                    if (false)
                    {
                        var path = Directory.GetCurrentDirectory() + "//Seed//create_init.sql";
                        var script = File.ReadAllText(path);
                        context.Database.ExecuteSqlRaw(script, new List<object>());
                    }
                }
            }
        }
    }
}
