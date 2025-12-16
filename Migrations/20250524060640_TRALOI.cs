using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class TRALOI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikePhanHoi");

            migrationBuilder.DropTable(
                name: "LikeTraLoi");

            migrationBuilder.DropTable(
                name: "TraLoi");

            migrationBuilder.CreateTable(
                name: "LikeBinhLuan",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaBL = table.Column<int>(type: "int", nullable: false),
                    TGThich = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaPH = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeBinhLuan", x => new { x.MaKH, x.MaBL });
                    table.ForeignKey(
                        name: "FK_LikeBinhLuan_BinhLuan_MaPH",
                        column: x => x.MaPH,
                        principalTable: "BinhLuan",
                        principalColumn: "MaBL");
                    table.ForeignKey(
                        name: "FK_LikeBinhLuan_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "PhanHoiBinhLuan",
                columns: table => new
                {
                    MaPHBL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBL = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    NoiDungPHBL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayPHBL = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaPH = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoiBinhLuan", x => x.MaPHBL);
                    table.ForeignKey(
                        name: "FK_PhanHoiBinhLuan_BinhLuan_MaPH",
                        column: x => x.MaPH,
                        principalTable: "BinhLuan",
                        principalColumn: "MaBL");
                    table.ForeignKey(
                        name: "FK_PhanHoiBinhLuan_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "LikePhanHoiBinhLuan",
                columns: table => new
                {
                    MaPHBL = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    TGThich = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaTL = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikePhanHoiBinhLuan", x => new { x.MaKH, x.MaPHBL });
                    table.ForeignKey(
                        name: "FK_LikePhanHoiBinhLuan_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_LikePhanHoiBinhLuan_PhanHoiBinhLuan_MaTL",
                        column: x => x.MaTL,
                        principalTable: "PhanHoiBinhLuan",
                        principalColumn: "MaPHBL");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeBinhLuan_MaPH",
                table: "LikeBinhLuan",
                column: "MaPH");

            migrationBuilder.CreateIndex(
                name: "IX_LikePhanHoiBinhLuan_MaTL",
                table: "LikePhanHoiBinhLuan",
                column: "MaTL");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoiBinhLuan_MaKH",
                table: "PhanHoiBinhLuan",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoiBinhLuan_MaPH",
                table: "PhanHoiBinhLuan",
                column: "MaPH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeBinhLuan");

            migrationBuilder.DropTable(
                name: "LikePhanHoiBinhLuan");

            migrationBuilder.DropTable(
                name: "PhanHoiBinhLuan");

            migrationBuilder.CreateTable(
                name: "LikePhanHoi",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaPH = table.Column<int>(type: "int", nullable: false),
                    TGThich = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikePhanHoi", x => new { x.MaKH, x.MaPH });
                    table.ForeignKey(
                        name: "FK_LikePhanHoi_BinhLuan_MaPH",
                        column: x => x.MaPH,
                        principalTable: "BinhLuan",
                        principalColumn: "MaBL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikePhanHoi_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "TraLoi",
                columns: table => new
                {
                    MaTL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaPH = table.Column<int>(type: "int", nullable: false),
                    NgayTL = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDungTL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraLoi", x => x.MaTL);
                    table.ForeignKey(
                        name: "FK_TraLoi_BinhLuan_MaPH",
                        column: x => x.MaPH,
                        principalTable: "BinhLuan",
                        principalColumn: "MaBL",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraLoi_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "LikeTraLoi",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaTL = table.Column<int>(type: "int", nullable: false),
                    TGThich = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeTraLoi", x => new { x.MaKH, x.MaTL });
                    table.ForeignKey(
                        name: "FK_LikeTraLoi_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_LikeTraLoi_TraLoi_MaTL",
                        column: x => x.MaTL,
                        principalTable: "TraLoi",
                        principalColumn: "MaTL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikePhanHoi_MaPH",
                table: "LikePhanHoi",
                column: "MaPH");

            migrationBuilder.CreateIndex(
                name: "IX_LikeTraLoi_MaTL",
                table: "LikeTraLoi",
                column: "MaTL");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoi_MaKH",
                table: "TraLoi",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoi_MaPH",
                table: "TraLoi",
                column: "MaPH");
        }
    }
}
