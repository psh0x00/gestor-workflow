using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestorWorkflow.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    id_status = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.id_status);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstado",
                columns: table => new
                {
                    id_tipo_estado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstado", x => x.id_tipo_estado);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    id_utilizador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Funcao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.id_utilizador);
                });

            migrationBuilder.CreateTable(
                name: "EstadoModelo",
                columns: table => new
                {
                    id_estado_modelo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    tipo_estado_id = table.Column<int>(type: "int", nullable: false),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    criado_por_utilizador_id = table.Column<int>(type: "int", nullable: true),
                    cor_hexadecimal = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoModelo", x => x.id_estado_modelo);
                    table.ForeignKey(
                        name: "FK_EstadoModelo_TiposEstado_tipo_estado_id",
                        column: x => x.tipo_estado_id,
                        principalTable: "TiposEstado",
                        principalColumn: "id_tipo_estado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstadoModelo_Utilizador_criado_por_utilizador_id",
                        column: x => x.criado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PosCondicao",
                columns: table => new
                {
                    id_poscondicao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    acao_sql = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    criado_por_utilizador_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCondicao", x => x.id_poscondicao);
                    table.ForeignKey(
                        name: "FK_PosCondicao_Utilizador_criado_por_utilizador_id",
                        column: x => x.criado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PreCondicao",
                columns: table => new
                {
                    id_precondicao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    condicao_sql = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    criado_por_utilizador_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreCondicao", x => x.id_precondicao);
                    table.ForeignKey(
                        name: "FK_PreCondicao_Utilizador_criado_por_utilizador_id",
                        column: x => x.criado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowModelo",
                columns: table => new
                {
                    id_workflow_modelo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_estado_modelo = table.Column<int>(type: "int", nullable: false),
                    criado_por_utilizador_id = table.Column<int>(type: "int", nullable: false),
                    data_ultima_alteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    alterado_por_utilizador_id = table.Column<int>(type: "int", nullable: true),
                    ativo = table.Column<bool>(type: "bit", nullable: false),
                    versao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowModelo", x => x.id_workflow_modelo);
                    table.ForeignKey(
                        name: "FK_WorkflowModelo_EstadoModelo_id_estado_modelo",
                        column: x => x.id_estado_modelo,
                        principalTable: "EstadoModelo",
                        principalColumn: "id_estado_modelo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowModelo_Utilizador_alterado_por_utilizador_id",
                        column: x => x.alterado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkflowModelo_Utilizador_criado_por_utilizador_id",
                        column: x => x.criado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransicaoModelo",
                columns: table => new
                {
                    id_transicao_modelo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_workflow_modelo = table.Column<int>(type: "int", nullable: false),
                    id_estado_origem = table.Column<int>(type: "int", nullable: true),
                    id_estado_destino = table.Column<int>(type: "int", nullable: false),
                    id_pre_condicao = table.Column<int>(type: "int", nullable: true),
                    id_pos_condicao = table.Column<int>(type: "int", nullable: true),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransicaoModelo", x => x.id_transicao_modelo);
                    table.ForeignKey(
                        name: "FK_TransicaoModelo_EstadoModelo_id_estado_destino",
                        column: x => x.id_estado_destino,
                        principalTable: "EstadoModelo",
                        principalColumn: "id_estado_modelo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicaoModelo_EstadoModelo_id_estado_origem",
                        column: x => x.id_estado_origem,
                        principalTable: "EstadoModelo",
                        principalColumn: "id_estado_modelo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicaoModelo_PosCondicao_id_pos_condicao",
                        column: x => x.id_pos_condicao,
                        principalTable: "PosCondicao",
                        principalColumn: "id_poscondicao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TransicaoModelo_PreCondicao_id_pre_condicao",
                        column: x => x.id_pre_condicao,
                        principalTable: "PreCondicao",
                        principalColumn: "id_precondicao",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TransicaoModelo_WorkflowModelo_id_workflow_modelo",
                        column: x => x.id_workflow_modelo,
                        principalTable: "WorkflowModelo",
                        principalColumn: "id_workflow_modelo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstancia",
                columns: table => new
                {
                    id_workflow_instancia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_workflow_modelo = table.Column<int>(type: "int", nullable: false),
                    data_inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    data_fim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    estado_atual_id = table.Column<int>(type: "int", nullable: true),
                    iniciado_por_utilizador_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstancia", x => x.id_workflow_instancia);
                    table.ForeignKey(
                        name: "FK_WorkflowInstancia_EstadoModelo_estado_atual_id",
                        column: x => x.estado_atual_id,
                        principalTable: "EstadoModelo",
                        principalColumn: "id_estado_modelo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkflowInstancia_Status_status_id",
                        column: x => x.status_id,
                        principalTable: "Status",
                        principalColumn: "id_status",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowInstancia_Utilizador_iniciado_por_utilizador_id",
                        column: x => x.iniciado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkflowInstancia_WorkflowModelo_id_workflow_modelo",
                        column: x => x.id_workflow_modelo,
                        principalTable: "WorkflowModelo",
                        principalColumn: "id_workflow_modelo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissao",
                columns: table => new
                {
                    id_permissao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    id_transicao_modelo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissao", x => x.id_permissao);
                    table.ForeignKey(
                        name: "FK_Permissao_TransicaoModelo_id_transicao_modelo",
                        column: x => x.id_transicao_modelo,
                        principalTable: "TransicaoModelo",
                        principalColumn: "id_transicao_modelo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransicaoInstancia",
                columns: table => new
                {
                    id_transicao_instancia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_workflow_instancia = table.Column<int>(type: "int", nullable: false),
                    id_transicao_modelo = table.Column<int>(type: "int", nullable: false),
                    data_execucao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    executado_por_utilizador_id = table.Column<int>(type: "int", nullable: true),
                    sucesso = table.Column<bool>(type: "bit", nullable: false),
                    erro_mensagem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransicaoInstancia", x => x.id_transicao_instancia);
                    table.ForeignKey(
                        name: "FK_TransicaoInstancia_TransicaoModelo_id_transicao_modelo",
                        column: x => x.id_transicao_modelo,
                        principalTable: "TransicaoModelo",
                        principalColumn: "id_transicao_modelo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicaoInstancia_Utilizador_executado_por_utilizador_id",
                        column: x => x.executado_por_utilizador_id,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TransicaoInstancia_WorkflowInstancia_id_workflow_instancia",
                        column: x => x.id_workflow_instancia,
                        principalTable: "WorkflowInstancia",
                        principalColumn: "id_workflow_instancia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador_Permissao",
                columns: table => new
                {
                    id_Utilizador = table.Column<int>(type: "int", nullable: false),
                    id_Permissao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador_Permissao", x => new { x.id_Utilizador, x.id_Permissao });
                    table.ForeignKey(
                        name: "FK_Utilizador_Permissao_Permissao_id_Permissao",
                        column: x => x.id_Permissao,
                        principalTable: "Permissao",
                        principalColumn: "id_permissao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Utilizador_Permissao_Utilizador_id_Utilizador",
                        column: x => x.id_Utilizador,
                        principalTable: "Utilizador",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "id_status", "nome" },
                values: new object[,]
                {
                    { 1, "Ativo" },
                    { 2, "Pausado" },
                    { 3, "Concluído" },
                    { 4, "Cancelado" },
                    { 5, "Pendente" }
                });

            migrationBuilder.InsertData(
                table: "TiposEstado",
                columns: new[] { "id_tipo_estado", "nome" },
                values: new object[,]
                {
                    { 1, "Inicial" },
                    { 2, "Intermediário" },
                    { 3, "Final" },
                    { 4, "Decisão" },
                    { 5, "Aprovação" }
                });

            migrationBuilder.InsertData(
                table: "Utilizador",
                columns: new[] { "id_utilizador", "Funcao", "Nome" },
                values: new object[,]
                {
                    { 1, "Admin", "Administrador" },
                    { 2, "User", "Utilizador Teste" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstadoModelo_criado_por_utilizador_id",
                table: "EstadoModelo",
                column: "criado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoModelo_tipo_estado_id",
                table: "EstadoModelo",
                column: "tipo_estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_Permissao_id_transicao_modelo",
                table: "Permissao",
                column: "id_transicao_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_PosCondicao_criado_por_utilizador_id",
                table: "PosCondicao",
                column: "criado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_PreCondicao_criado_por_utilizador_id",
                table: "PreCondicao",
                column: "criado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoInstancia_executado_por_utilizador_id",
                table: "TransicaoInstancia",
                column: "executado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoInstancia_id_transicao_modelo",
                table: "TransicaoInstancia",
                column: "id_transicao_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoInstancia_id_workflow_instancia",
                table: "TransicaoInstancia",
                column: "id_workflow_instancia");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoModelo_id_estado_destino",
                table: "TransicaoModelo",
                column: "id_estado_destino");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoModelo_id_estado_origem",
                table: "TransicaoModelo",
                column: "id_estado_origem");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoModelo_id_pos_condicao",
                table: "TransicaoModelo",
                column: "id_pos_condicao");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoModelo_id_pre_condicao",
                table: "TransicaoModelo",
                column: "id_pre_condicao");

            migrationBuilder.CreateIndex(
                name: "IX_TransicaoModelo_id_workflow_modelo",
                table: "TransicaoModelo",
                column: "id_workflow_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizador_Permissao_id_Permissao",
                table: "Utilizador_Permissao",
                column: "id_Permissao");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstancia_estado_atual_id",
                table: "WorkflowInstancia",
                column: "estado_atual_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstancia_id_workflow_modelo",
                table: "WorkflowInstancia",
                column: "id_workflow_modelo");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstancia_iniciado_por_utilizador_id",
                table: "WorkflowInstancia",
                column: "iniciado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstancia_status_id",
                table: "WorkflowInstancia",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowModelo_alterado_por_utilizador_id",
                table: "WorkflowModelo",
                column: "alterado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowModelo_criado_por_utilizador_id",
                table: "WorkflowModelo",
                column: "criado_por_utilizador_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowModelo_id_estado_modelo",
                table: "WorkflowModelo",
                column: "id_estado_modelo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransicaoInstancia");

            migrationBuilder.DropTable(
                name: "Utilizador_Permissao");

            migrationBuilder.DropTable(
                name: "WorkflowInstancia");

            migrationBuilder.DropTable(
                name: "Permissao");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "TransicaoModelo");

            migrationBuilder.DropTable(
                name: "PosCondicao");

            migrationBuilder.DropTable(
                name: "PreCondicao");

            migrationBuilder.DropTable(
                name: "WorkflowModelo");

            migrationBuilder.DropTable(
                name: "EstadoModelo");

            migrationBuilder.DropTable(
                name: "TiposEstado");

            migrationBuilder.DropTable(
                name: "Utilizador");
        }
    }
}
