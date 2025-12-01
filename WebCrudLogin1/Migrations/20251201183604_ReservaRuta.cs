using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrudLogin.Migrations
{
    /// <inheritdoc />
    public partial class ReservaRuta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusquedasRuta_Campuses_CampusOrigenId",
                table: "BusquedasRuta");

            migrationBuilder.DropForeignKey(
                name: "FK_BusquedasRuta_Sectores_SectorId",
                table: "BusquedasRuta");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Campuses_CampusOrigenId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Sectores_SectorId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Users_ConductorId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Vehiculos_VehiculoId",
                table: "Rutas");

            migrationBuilder.CreateTable(
                name: "ReservasRuta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RutaId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaUnion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasRuta", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservasRuta_RutaId_UsuarioId",
                table: "ReservasRuta",
                columns: new[] { "RutaId", "UsuarioId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusquedasRuta_Campuses_CampusOrigenId",
                table: "BusquedasRuta",
                column: "CampusOrigenId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusquedasRuta_Sectores_SectorId",
                table: "BusquedasRuta",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Campuses_CampusOrigenId",
                table: "Rutas",
                column: "CampusOrigenId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Sectores_SectorId",
                table: "Rutas",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Users_ConductorId",
                table: "Rutas",
                column: "ConductorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Vehiculos_VehiculoId",
                table: "Rutas",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusquedasRuta_Campuses_CampusOrigenId",
                table: "BusquedasRuta");

            migrationBuilder.DropForeignKey(
                name: "FK_BusquedasRuta_Sectores_SectorId",
                table: "BusquedasRuta");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Campuses_CampusOrigenId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Sectores_SectorId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Users_ConductorId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Vehiculos_VehiculoId",
                table: "Rutas");

            migrationBuilder.DropTable(
                name: "ReservasRuta");

            migrationBuilder.AddForeignKey(
                name: "FK_BusquedasRuta_Campuses_CampusOrigenId",
                table: "BusquedasRuta",
                column: "CampusOrigenId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusquedasRuta_Sectores_SectorId",
                table: "BusquedasRuta",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Campuses_CampusOrigenId",
                table: "Rutas",
                column: "CampusOrigenId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Sectores_SectorId",
                table: "Rutas",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Users_ConductorId",
                table: "Rutas",
                column: "ConductorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Vehiculos_VehiculoId",
                table: "Rutas",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
