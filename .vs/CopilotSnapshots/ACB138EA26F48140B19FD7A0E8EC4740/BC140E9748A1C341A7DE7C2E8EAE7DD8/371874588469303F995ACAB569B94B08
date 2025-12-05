using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunaTalento.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaLimiteActividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLimite",
                table: "Actividad",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaLimite",
                table: "Actividad");
        }
    }
}
