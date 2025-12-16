using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class binhluan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikePhanHoi_PhanHoi_MaPH",
                table: "LikePhanHoi");

            migrationBuilder.DropForeignKey(
                name: "FK_TraLoi_PhanHoi_MaPH",
                table: "TraLoi");

            migrationBuilder.DropTable(
                name: "PhanHoi");

            migrationBuilder.CreateTable(
                name: "BinhLuan",
                columns: table => new
                {
                    MaBL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    NoiDungBL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayBL = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuan", x => x.MaBL);
                    table.ForeignKey(
                        name: "FK_BinhLuan_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BinhLuan_SanPhams_MaSP",
                        column: x => x.MaSP,
                        principalTable: "SanPhams",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaKH",
                table: "BinhLuan",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_MaSP",
                table: "BinhLuan",
                column: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePhanHoi_BinhLuan_MaPH",
                table: "LikePhanHoi",
                column: "MaPH",
                principalTable: "BinhLuan",
                principalColumn: "MaBL",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraLoi_BinhLuan_MaPH",
                table: "TraLoi",
                column: "MaPH",
                principalTable: "BinhLuan",
                principalColumn: "MaBL",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikePhanHoi_BinhLuan_MaPH",
                table: "LikePhanHoi");

            migrationBuilder.DropForeignKey(
                name: "FK_TraLoi_BinhLuan_MaPH",
                table: "TraLoi");

            migrationBuilder.DropTable(
                name: "BinhLuan");

            migrationBuilder.CreateTable(
                name: "PhanHoi",
                columns: table => new
                {
                    MaPH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    NgayPH = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungPH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoi", x => x.MaPH);
                    table.ForeignKey(
                        name: "FK_PhanHoi_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanHoi_SanPhams_MaSP",
                        column: x => x.MaSP,
                        principalTable: "SanPhams",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_MaKH",
                table: "PhanHoi",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_MaSP",
                table: "PhanHoi",
                column: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePhanHoi_PhanHoi_MaPH",
                table: "LikePhanHoi",
                column: "MaPH",
                principalTable: "PhanHoi",
                principalColumn: "MaPH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TraLoi_PhanHoi_MaPH",
                table: "TraLoi",
                column: "MaPH",
                principalTable: "PhanHoi",
                principalColumn: "MaPH",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
