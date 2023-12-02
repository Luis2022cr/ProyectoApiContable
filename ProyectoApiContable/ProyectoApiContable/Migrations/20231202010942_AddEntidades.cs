using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoApiContable.Migrations
{
    /// <inheritdoc />
    public partial class AddEntidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cuentas_tipos_cuentas_tipo_cuenta_id",
                table: "cuentas");

            migrationBuilder.AddForeignKey(
                name: "FK_cuentas_tipos_cuentas_tipo_cuenta_id",
                table: "cuentas",
                column: "tipo_cuenta_id",
                principalTable: "tipos_cuentas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cuentas_tipos_cuentas_tipo_cuenta_id",
                table: "cuentas");

            migrationBuilder.AddForeignKey(
                name: "FK_cuentas_tipos_cuentas_tipo_cuenta_id",
                table: "cuentas",
                column: "tipo_cuenta_id",
                principalTable: "tipos_cuentas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
