using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorWorkflow.Data.Migrations
{
    /// <inheritdoc />
    public partial class newUpdate7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "funcoes",
                table: "EstadoModelo",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "funcoes",
                table: "EstadoModelo");
        }
    }
}
