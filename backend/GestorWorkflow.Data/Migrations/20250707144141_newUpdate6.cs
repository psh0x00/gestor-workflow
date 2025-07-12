using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorWorkflow.Data.Migrations
{
    /// <inheritdoc />
    public partial class newUpdate6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "equipa_json",
                table: "WorkflowInstancia",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "equipa_json",
                table: "WorkflowInstancia");
        }
    }
}
