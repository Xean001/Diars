using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunaTalento.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNivelDificultadActividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NivelDificultad",
                table: "Actividad",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelDificultad",
                table: "Actividad");
        }
    }
}
