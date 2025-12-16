using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class nhanvien2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VaiTroNhanVien",
                table: "VaiTroNhanVien");

            migrationBuilder.AddColumn<int>(
                name: "MaVTNV",
                table: "VaiTroNhanVien",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaiTroNhanVien",
                table: "VaiTroNhanVien",
                column: "MaVTNV");

            migrationBuilder.CreateIndex(
                name: "IX_VaiTroNhanVien_MaNV",
                table: "VaiTroNhanVien",
                column: "MaNV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VaiTroNhanVien",
                table: "VaiTroNhanVien");

            migrationBuilder.DropIndex(
                name: "IX_VaiTroNhanVien_MaNV",
                table: "VaiTroNhanVien");

            migrationBuilder.DropColumn(
                name: "MaVTNV",
                table: "VaiTroNhanVien");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaiTroNhanVien",
                table: "VaiTroNhanVien",
                column: "MaNV");
        }
    }
}
