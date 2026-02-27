using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoVenda.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialListing : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Listings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                PisoDeNegociacao = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                Status = table.Column<int>(type: "INTEGER", nullable: false),
                FipeReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Listings", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Listings");
    }
}
