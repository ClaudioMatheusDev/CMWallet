using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMWallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DatasSemHorarioEExclusaoRestrita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetasFinanceiras_Contas_ContaId",
                table: "MetasFinanceiras");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Contas_ContaId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_ContaId",
                table: "Transacoes");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Data",
                table: "Transacoes",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataMeta",
                table: "MetasFinanceiras",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaId_Data",
                table: "Transacoes",
                columns: new[] { "ContaId", "Data" });

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_Data",
                table: "Transacoes",
                column: "Data");

            migrationBuilder.AddForeignKey(
                name: "FK_MetasFinanceiras_Contas_ContaId",
                table: "MetasFinanceiras",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "ContaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Contas_ContaId",
                table: "Transacoes",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "ContaId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetasFinanceiras_Contas_ContaId",
                table: "MetasFinanceiras");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Contas_ContaId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_ContaId_Data",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_Data",
                table: "Transacoes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Transacoes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataMeta",
                table: "MetasFinanceiras",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaId",
                table: "Transacoes",
                column: "ContaId");

            migrationBuilder.AddForeignKey(
                name: "FK_MetasFinanceiras_Contas_ContaId",
                table: "MetasFinanceiras",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "ContaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Contas_ContaId",
                table: "Transacoes",
                column: "ContaId",
                principalTable: "Contas",
                principalColumn: "ContaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
