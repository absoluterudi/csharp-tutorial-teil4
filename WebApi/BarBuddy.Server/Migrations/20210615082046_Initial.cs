using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace BarBuddy.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Augenarztlist",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    Adress_CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_AdressAddition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Postal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_GeoLocation = table.Column<Point>(type: "geography", nullable: true),
                    QRCodeSalt = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Owner_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credentials_Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Credentials_PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Augenarztlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adress_CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_AdressAddition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Postal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_GeoLocation = table.Column<Point>(type: "geography", nullable: true),
                    GooglePlusCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    QRCodeSalt = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Owner_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credentials_Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Credentials_PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Optikerlist",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    Adress_CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_AdressAddition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Postal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adress_GeoLocation = table.Column<Point>(type: "geography", nullable: true),
                    GooglePlusCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    QRCodeSalt = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Owner_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credentials_Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Credentials_PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Optikerlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BarSpots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaType = table.Column<int>(type: "int", nullable: false),
                    SpotType = table.Column<int>(type: "int", nullable: false),
                    MaxPersons = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarSpots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarSpots_Bars_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Bars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundusImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ByteContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileExtension = table.Column<int>(type: "int", nullable: false),
                    ByteContentThumb128 = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kundennummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AugenarztId = table.Column<long>(type: "bigint", nullable: false),
                    BewertungVomAugenarzt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnAugenarztGeschickt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VomAugenarztBefundet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VomApothekerGelesenAm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OptikerId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundusImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundusImages_Optikerlist_OptikerId",
                        column: x => x.OptikerId,
                        principalTable: "Optikerlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountPerson = table.Column<int>(type: "int", nullable: false),
                    ReservedUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationSpotId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_BarSpots_LocationSpotId",
                        column: x => x.LocationSpotId,
                        principalTable: "BarSpots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarSpots_LocationId",
                table: "BarSpots",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_FundusImages_OptikerId",
                table: "FundusImages",
                column: "OptikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_LocationSpotId",
                table: "Reservations",
                column: "LocationSpotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Augenarztlist");

            migrationBuilder.DropTable(
                name: "FundusImages");

            migrationBuilder.DropTable(
                name: "RegistrationTokens");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Optikerlist");

            migrationBuilder.DropTable(
                name: "BarSpots");

            migrationBuilder.DropTable(
                name: "Bars");
        }
    }
}
