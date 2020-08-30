using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PAUP.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: false),
                    ImgUrl = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Adress = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    HasOffer = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    Adress = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    AccountType = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesOnOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    GameId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesOnOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesOnOrder_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesOnOrder_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Adress = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Guarantee = table.Column<string>(nullable: true),
                    ShippingDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesOnOffer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    GameId = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    OfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesOnOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesOnOffer_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesOnOffer_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Amount", "Category", "Description", "ImgUrl", "Name", "Price" },
                values: new object[] { 1, 3, 0, null, "/images/Games/default.png", "GTA San Andreas", 55.25m });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Amount", "Category", "Description", "ImgUrl", "Name", "Price" },
                values: new object[] { 2, 5, 2, null, "/images/Games/default.png", "FIFA 2020", 125.99m });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccountType", "Adress", "Email", "Name", "Password", "Surname" },
                values: new object[] { 1, 1, "", "admin@admin.admin", "Admin", "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_GamesOnOffer_GameId",
                table: "GamesOnOffer",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesOnOffer_OfferId",
                table: "GamesOnOffer",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesOnOrder_GameId",
                table: "GamesOnOrder",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesOnOrder_OrderId",
                table: "GamesOnOrder",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_OrderId",
                table: "Offers",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_UserId",
                table: "Offers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamesOnOffer");

            migrationBuilder.DropTable(
                name: "GamesOnOrder");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
