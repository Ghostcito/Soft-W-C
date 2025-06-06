using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoftWC.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionUsuarioServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tareo");

            migrationBuilder.CreateTable(
                name: "usuarioServicio",
                columns: table => new
                {
                    UsuarioServicioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ServicioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioServicio", x => x.UsuarioServicioId);
                    table.ForeignKey(
                        name: "FK_usuarioServicio_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarioServicio_Servicio_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicio",
                        principalColumn: "ServicioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarioServicio_ServicioId",
                table: "usuarioServicio",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioServicio_UserId",
                table: "usuarioServicio",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarioServicio");

            migrationBuilder.CreateTable(
                name: "Tareo",
                columns: table => new
                {
                    IdTareo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<string>(type: "text", nullable: false),
                    ServicioId = table.Column<int>(type: "integer", nullable: false),
                    TurnoId = table.Column<int>(type: "integer", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Observacion = table.Column<string>(type: "text", nullable: true),
                    PagoPorHora = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TotalGanado = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareo", x => x.IdTareo);
                    table.ForeignKey(
                        name: "FK_Tareo_AspNetUsers_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tareo_Servicio_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicio",
                        principalColumn: "ServicioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tareo_Turno_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turno",
                        principalColumn: "TurnoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tareo_IdEmpleado",
                table: "Tareo",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Tareo_ServicioId",
                table: "Tareo",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareo_TurnoId",
                table: "Tareo",
                column: "TurnoId");
        }
    }
}
