using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyConverter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyConversionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SourceCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    TargetCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    ResultAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ConvertedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyConversionHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyConversionHistory");
        }
    }
}
