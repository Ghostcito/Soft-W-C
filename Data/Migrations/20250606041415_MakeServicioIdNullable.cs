using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoftWC.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeServicioIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarioServicio");

            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ServicioId",
                table: "AspNetUsers",
                column: "ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Servicio_ServicioId",
                table: "AspNetUsers",
                column: "ServicioId",
                principalTable: "Servicio",
                principalColumn: "ServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Servicio_ServicioId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ServicioId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "usuarioServicio",
                columns: table => new
                {
                    UsuarioServicioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServicioId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
    }
}
