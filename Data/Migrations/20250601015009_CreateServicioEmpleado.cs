using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoftWC.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateServicioEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Tareo_AspNetUsers_IdEmpleado",
                table: "Tareo");

            migrationBuilder.DropTable(
                name: "Geo_asis");

            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Servicio");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalGanado",
                table: "Tareo",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PagoPorHora",
                table: "Tareo",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdEmpleado",
                table: "Tareo",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HorasTrabajadas",
                table: "Tareo",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Tareo",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Tareo",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TurnoId",
                table: "Tareo",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreServicio",
                table: "Servicio",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DuracionEstimada",
                table: "Servicio",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioBase",
                table: "Servicio",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);*/

            migrationBuilder.CreateTable(
                name: "EmpleadoServicio",
                columns: table => new
                {
                    EmpleadoId = table.Column<string>(type: "text", nullable: false),
                    ServicioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoServicio", x => new { x.EmpleadoId, x.ServicioId });
                    table.ForeignKey(
                        name: "FK_EmpleadoServicio_AspNetUsers_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpleadoServicio_Servicio_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicio",
                        principalColumn: "ServicioId",
                        onDelete: ReferentialAction.Cascade);
                });

            /*migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    IdEvaluacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<string>(type: "text", nullable: false),
                    TipoEmpleado = table.Column<string>(type: "text", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Responsabilidad = table.Column<int>(type: "integer", nullable: false),
                    Puntualidad = table.Column<int>(type: "integer", nullable: false),
                    CalidadTrabajo = table.Column<int>(type: "integer", nullable: false),
                    UsoMateriales = table.Column<int>(type: "integer", nullable: false),
                    Actitud = table.Column<int>(type: "integer", nullable: false),
                    EvaluadorId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.IdEvaluacion);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_AspNetUsers_EvaluadorId",
                        column: x => x.EvaluadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evaluaciones_AspNetUsers_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<string>(type: "text", nullable: true),
                    HoraSalida = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraRetorno = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.IdPermiso);
                    table.ForeignKey(
                        name: "FK_Permisos_AspNetUsers_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    TurnoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTurno = table.Column<string>(type: "text", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.TurnoId);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioTurno",
                columns: table => new
                {
                    UsuarioTurnoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    TurnoId = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTurno", x => x.UsuarioTurnoId);
                    table.ForeignKey(
                        name: "FK_UsuarioTurno_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioTurno_Turno_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turno",
                        principalColumn: "TurnoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tareo_ServicioId",
                table: "Tareo",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareo_TurnoId",
                table: "Tareo",
                column: "TurnoId");
*/
            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoServicio_ServicioId",
                table: "EmpleadoServicio",
                column: "ServicioId");

            /* migrationBuilder.CreateIndex(
                 name: "IX_Evaluaciones_EvaluadorId",
                 table: "Evaluaciones",
                 column: "EvaluadorId");

             migrationBuilder.CreateIndex(
                 name: "IX_Evaluaciones_IdEmpleado",
                 table: "Evaluaciones",
                 column: "IdEmpleado");

             migrationBuilder.CreateIndex(
                 name: "IX_Permisos_IdEmpleado",
                 table: "Permisos",
                 column: "IdEmpleado");

             migrationBuilder.CreateIndex(
                 name: "IX_UsuarioTurno_TurnoId",
                 table: "UsuarioTurno",
                 column: "TurnoId");

             migrationBuilder.CreateIndex(
                 name: "IX_UsuarioTurno_UsuarioId",
                 table: "UsuarioTurno",
                 column: "UsuarioId");

             migrationBuilder.AddForeignKey(
                 name: "FK_Tareo_AspNetUsers_IdEmpleado",
                 table: "Tareo",
                 column: "IdEmpleado",
                 principalTable: "AspNetUsers",
                 principalColumn: "Id",
                 onDelete: ReferentialAction.Cascade);

             migrationBuilder.AddForeignKey(
                 name: "FK_Tareo_Servicio_ServicioId",
                 table: "Tareo",
                 column: "ServicioId",
                 principalTable: "Servicio",
                 principalColumn: "ServicioId",
                 onDelete: ReferentialAction.Cascade);

             migrationBuilder.AddForeignKey(
                 name: "FK_Tareo_Turno_TurnoId",
                 table: "Tareo",
                 column: "TurnoId",
                 principalTable: "Turno",
                 principalColumn: "TurnoId");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Tareo_AspNetUsers_IdEmpleado",
                table: "Tareo");

            migrationBuilder.DropForeignKey(
                name: "FK_Tareo_Servicio_ServicioId",
                table: "Tareo");

            migrationBuilder.DropForeignKey(
                name: "FK_Tareo_Turno_TurnoId",
                table: "Tareo");*/

            migrationBuilder.DropTable(
                name: "EmpleadoServicio");

            /* migrationBuilder.DropTable(
                 name: "Evaluaciones");

             migrationBuilder.DropTable(
                 name: "Permisos");

             migrationBuilder.DropTable(
                 name: "UsuarioTurno");

             migrationBuilder.DropTable(
                 name: "Turno");

             migrationBuilder.DropIndex(
                 name: "IX_Tareo_ServicioId",
                 table: "Tareo");

             migrationBuilder.DropIndex(
                 name: "IX_Tareo_TurnoId",
                 table: "Tareo");

             migrationBuilder.DropColumn(
                 name: "Estado",
                 table: "Tareo");

             migrationBuilder.DropColumn(
                 name: "ServicioId",
                 table: "Tareo");

             migrationBuilder.DropColumn(
                 name: "TurnoId",
                 table: "Tareo");

             migrationBuilder.DropColumn(
                 name: "DuracionEstimada",
                 table: "Servicio");

             migrationBuilder.DropColumn(
                 name: "PrecioBase",
                 table: "Servicio");

             migrationBuilder.AlterColumn<decimal>(
                 name: "TotalGanado",
                 table: "Tareo",
                 type: "numeric",
                 nullable: true,
                 oldClrType: typeof(decimal),
                 oldType: "numeric(10,2)");

             migrationBuilder.AlterColumn<decimal>(
                 name: "PagoPorHora",
                 table: "Tareo",
                 type: "numeric",
                 nullable: true,
                 oldClrType: typeof(decimal),
                 oldType: "numeric(10,2)");

             migrationBuilder.AlterColumn<string>(
                 name: "IdEmpleado",
                 table: "Tareo",
                 type: "text",
                 nullable: true,
                 oldClrType: typeof(string),
                 oldType: "text");

             migrationBuilder.AlterColumn<decimal>(
                 name: "HorasTrabajadas",
                 table: "Tareo",
                 type: "numeric",
                 nullable: true,
                 oldClrType: typeof(decimal),
                 oldType: "numeric(5,2)");

             migrationBuilder.AlterColumn<string>(
                 name: "NombreServicio",
                 table: "Servicio",
                 type: "text",
                 nullable: false,
                 oldClrType: typeof(string),
                 oldType: "character varying(100)",
                 oldMaxLength: 100);

             migrationBuilder.AddColumn<TimeSpan>(
                 name: "Duracion",
                 table: "Servicio",
                 type: "interval",
                 nullable: false,
                 defaultValue: new TimeSpan(0, 0, 0, 0, 0));

             migrationBuilder.AddColumn<decimal>(
                 name: "Precio",
                 table: "Servicio",
                 type: "numeric",
                 nullable: false,
                 defaultValue: 0m);

             migrationBuilder.CreateTable(
                 name: "Geo_asis",
                 columns: table => new
                 {
                     IdGeolocalizacion = table.Column<int>(type: "integer", nullable: false)
                         .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                     IdAsistencia = table.Column<int>(type: "integer", nullable: false),
                     DistanciaMetros = table.Column<decimal>(type: "numeric", nullable: false),
                     EstadoValidacion = table.Column<string>(type: "text", nullable: false),
                     Latitud = table.Column<decimal>(type: "numeric", nullable: false),
                     Longitud = table.Column<decimal>(type: "numeric", nullable: false),
                     Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Geo_asis", x => x.IdGeolocalizacion);
                     table.ForeignKey(
                         name: "FK_Geo_asis_Asistencia_IdAsistencia",
                         column: x => x.IdAsistencia,
                         principalTable: "Asistencia",
                         principalColumn: "IdAsistencia",
                         onDelete: ReferentialAction.Cascade);
                 });

             migrationBuilder.CreateIndex(
                 name: "IX_Geo_asis_IdAsistencia",
                 table: "Geo_asis",
                 column: "IdAsistencia");

             migrationBuilder.AddForeignKey(
                 name: "FK_Tareo_AspNetUsers_IdEmpleado",
                 table: "Tareo",
                 column: "IdEmpleado",
                 principalTable: "AspNetUsers",
                 principalColumn: "Id");*/
        }
    }
}
