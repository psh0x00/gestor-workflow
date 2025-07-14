using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorWorkflow.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadosConcluidosJsonToWorkflowInstancia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "estados_concluidos_json",
                table: "WorkflowInstancia",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estados_concluidos_json",
                table: "WorkflowInstancia");
        }
    }
}
