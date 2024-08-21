using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helen.Repository.Migrations
{
    public partial class initialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    To = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ResponseMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationNotificationData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    WeekdayOpenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WeekdayCloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SaturdayOpenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SaturdayCloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SundayOpenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SundayCloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ExtraInformation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AvailableForRent = table.Column<bool>(type: "bit", nullable: false),
                    RentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationNotificationData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ResponseCode = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    From = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    To = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ApiToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gateway = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerReference = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Message_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Gateway_used = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsSmoker = table.Column<bool>(type: "bit", nullable: false),
                    ReminderFrequency = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReminderTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendViaMail = table.Column<bool>(type: "bit", nullable: false),
                    SendViaPhone = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_Area",
                table: "LocationNotificationData",
                column: "Area");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_DateAdded",
                table: "LocationNotificationData",
                column: "DateAdded");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_Location",
                table: "LocationNotificationData",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_Name",
                table: "LocationNotificationData",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_SaturdayCloseTime",
                table: "LocationNotificationData",
                column: "SaturdayCloseTime");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_SaturdayOpenTime",
                table: "LocationNotificationData",
                column: "SaturdayOpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_SundayCloseTime",
                table: "LocationNotificationData",
                column: "SundayCloseTime");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_SundayOpenTime",
                table: "LocationNotificationData",
                column: "SundayOpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_Type",
                table: "LocationNotificationData",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_WeekdayCloseTime",
                table: "LocationNotificationData",
                column: "WeekdayCloseTime");

            migrationBuilder.CreateIndex(
                name: "IX_LocationNotificationData_WeekdayOpenTime",
                table: "LocationNotificationData",
                column: "WeekdayOpenTime");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_Location",
                table: "UserData",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_Username",
                table: "UserData",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailData");

            migrationBuilder.DropTable(
                name: "LocationNotificationData");

            migrationBuilder.DropTable(
                name: "SmsData");

            migrationBuilder.DropTable(
                name: "UserData");
        }
    }
}
