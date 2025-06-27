using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorWorkflow.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Utilizador",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Utilizador",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Utilizador",
                keyColumn: "id_utilizador",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Utilizador",
                keyColumn: "id_utilizador",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Utilizador");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Utilizador");
        }
    }
}
