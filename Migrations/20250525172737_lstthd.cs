using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class lstthd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_TrangThai_MaTT",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_MaTT",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "MaTT",
                table: "HoaDon");

            migrationBuilder.CreateTable(
                name: "LichSuTTHD",
                columns: table => new
                {
                    MaLS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoHD = table.Column<int>(type: "int", nullable: false),
                    MaTT = table.Column<int>(type: "int", nullable: false),
                    ThoiGianThayDoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuTTHD", x => x.MaLS);
                    table.ForeignKey(
                        name: "FK_LichSuTTHD_HoaDon_SoHD",
                        column: x => x.SoHD,
                        principalTable: "HoaDon",
                        principalColumn: "SoHD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichSuTTHD_TrangThai_MaTT",
                        column: x => x.MaTT,
                        principalTable: "TrangThai",
                        principalColumn: "MaTT");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTTHD_MaTT",
                table: "LichSuTTHD",
                column: "MaTT");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTTHD_SoHD",
                table: "LichSuTTHD",
                column: "SoHD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuTTHD");

            migrationBuilder.AddColumn<int>(
                name: "MaTT",
                table: "HoaDon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaTT",
                table: "HoaDon",
                column: "MaTT");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_TrangThai_MaTT",
                table: "HoaDon",
                column: "MaTT",
                principalTable: "TrangThai",
                principalColumn: "MaTT",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
