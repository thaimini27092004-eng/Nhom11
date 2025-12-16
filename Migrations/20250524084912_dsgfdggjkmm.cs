using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class dsgfdggjkmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHangs_MaKH",
                table: "BinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_SanPhams_MaSP",
                table: "BinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_CTGioHangs_GioHangs_MaGH",
                table: "CTGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_CTGioHangs_SanPhams_MaSP",
                table: "CTGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_CTGioHangs_SanPhams_SanPhamMaSP",
                table: "CTGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_CTHDs_HoaDons_SoHD",
                table: "CTHDs");

            migrationBuilder.DropForeignKey(
                name: "FK_CTHDs_TonKho_MaKho_MaSP",
                table: "CTHDs");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_CTHDs_SoHD_MaSP_MaKho",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_DSAnh_SanPhams_MaSP",
                table: "DSAnh");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_KhachHangs_MaKH",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_KhachHangs_KhachHangMaKH",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_KhachHangs_MaKH",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_PTTTs_MaPT",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_TrangThais_MaTT",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangs_AspNetUsers_UserId",
                table: "KhachHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeBinhLuan_KhachHangs_MaKH",
                table: "LikeBinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeDanhGia_KhachHangs_MaKH",
                table: "LikeDanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_LikePhanHoiBinhLuan_KhachHangs_MaKH",
                table: "LikePhanHoiBinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_LikePhanHoiDanhGia_KhachHangs_MaKH",
                table: "LikePhanHoiDanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoiBinhLuan_KhachHangs_MaKH",
                table: "PhanHoiBinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoiDanhGia_KhachHangs_MaKH",
                table: "PhanHoiDanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_DanhMucs_MaDM",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_TonKho_SanPhams_MaSP",
                table: "TonKho");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrangThais",
                table: "TrangThais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPhams",
                table: "SanPhams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PTTTs",
                table: "PTTTs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaXacThucs",
                table: "MaXacThucs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachHangs",
                table: "KhachHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HoaDons",
                table: "HoaDons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMucs",
                table: "DanhMucs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CTHDs",
                table: "CTHDs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CTGioHangs",
                table: "CTGioHangs");

            migrationBuilder.RenameTable(
                name: "TrangThais",
                newName: "TrangThai");

            migrationBuilder.RenameTable(
                name: "SanPhams",
                newName: "SanPham");

            migrationBuilder.RenameTable(
                name: "PTTTs",
                newName: "PTTT");

            migrationBuilder.RenameTable(
                name: "MaXacThucs",
                newName: "MaXacThuc");

            migrationBuilder.RenameTable(
                name: "KhachHangs",
                newName: "KhachHang");

            migrationBuilder.RenameTable(
                name: "HoaDons",
                newName: "HoaDon");

            migrationBuilder.RenameTable(
                name: "GioHangs",
                newName: "GioHang");

            migrationBuilder.RenameTable(
                name: "DanhMucs",
                newName: "DanhMuc");

            migrationBuilder.RenameTable(
                name: "CTHDs",
                newName: "CTHD");

            migrationBuilder.RenameTable(
                name: "CTGioHangs",
                newName: "CTGioHang");

            migrationBuilder.RenameIndex(
                name: "IX_SanPhams_MaDM",
                table: "SanPham",
                newName: "IX_SanPham_MaDM");

            migrationBuilder.RenameIndex(
                name: "IX_KhachHangs_UserId",
                table: "KhachHang",
                newName: "IX_KhachHang_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDons_MaTT",
                table: "HoaDon",
                newName: "IX_HoaDon_MaTT");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDons_MaPT",
                table: "HoaDon",
                newName: "IX_HoaDon_MaPT");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDons_MaKH",
                table: "HoaDon",
                newName: "IX_HoaDon_MaKH");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDons_KhachHangMaKH",
                table: "HoaDon",
                newName: "IX_HoaDon_KhachHangMaKH");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangs_MaKH",
                table: "GioHang",
                newName: "IX_GioHang_MaKH");

            migrationBuilder.RenameIndex(
                name: "IX_CTHDs_MaKho_MaSP",
                table: "CTHD",
                newName: "IX_CTHD_MaKho_MaSP");

            migrationBuilder.RenameIndex(
                name: "IX_CTGioHangs_SanPhamMaSP",
                table: "CTGioHang",
                newName: "IX_CTGioHang_SanPhamMaSP");

            migrationBuilder.RenameIndex(
                name: "IX_CTGioHangs_MaSP",
                table: "CTGioHang",
                newName: "IX_CTGioHang_MaSP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrangThai",
                table: "TrangThai",
                column: "MaTT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham",
                column: "MaSP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PTTT",
                table: "PTTT",
                column: "MaPT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaXacThuc",
                table: "MaXacThuc",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachHang",
                table: "KhachHang",
                column: "MaKH");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HoaDon",
                table: "HoaDon",
                column: "SoHD");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang",
                column: "MaGH");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMuc",
                table: "DanhMuc",
                column: "MaDM");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CTHD",
                table: "CTHD",
                columns: new[] { "SoHD", "MaSP", "MaKho" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CTGioHang",
                table: "CTGioHang",
                columns: new[] { "MaGH", "MaSP" });

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_KhachHang_MaKH",
                table: "BinhLuan",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_SanPham_MaSP",
                table: "BinhLuan",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_CTGioHang_GioHang_MaGH",
                table: "CTGioHang",
                column: "MaGH",
                principalTable: "GioHang",
                principalColumn: "MaGH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTGioHang_SanPham_MaSP",
                table: "CTGioHang",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_CTGioHang_SanPham_SanPhamMaSP",
                table: "CTGioHang",
                column: "SanPhamMaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_CTHD_HoaDon_SoHD",
                table: "CTHD",
                column: "SoHD",
                principalTable: "HoaDon",
                principalColumn: "SoHD",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTHD_TonKho_MaKho_MaSP",
                table: "CTHD",
                columns: new[] { "MaKho", "MaSP" },
                principalTable: "TonKho",
                principalColumns: new[] { "MaKho", "MaSP" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_CTHD_SoHD_MaSP_MaKho",
                table: "DanhGia",
                columns: new[] { "SoHD", "MaSP", "MaKho" },
                principalTable: "CTHD",
                principalColumns: new[] { "SoHD", "MaSP", "MaKho" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DSAnh_SanPham_MaSP",
                table: "DSAnh",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHang_KhachHang_MaKH",
                table: "GioHang",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_KhachHang_KhachHangMaKH",
                table: "HoaDon",
                column: "KhachHangMaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_KhachHang_MaKH",
                table: "HoaDon",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_PTTT_MaPT",
                table: "HoaDon",
                column: "MaPT",
                principalTable: "PTTT",
                principalColumn: "MaPT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_TrangThai_MaTT",
                table: "HoaDon",
                column: "MaTT",
                principalTable: "TrangThai",
                principalColumn: "MaTT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHang_AspNetUsers_UserId",
                table: "KhachHang",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikeBinhLuan_KhachHang_MaKH",
                table: "LikeBinhLuan",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeDanhGia_KhachHang_MaKH",
                table: "LikeDanhGia",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePhanHoiBinhLuan_KhachHang_MaKH",
                table: "LikePhanHoiBinhLuan",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePhanHoiDanhGia_KhachHang_MaKH",
                table: "LikePhanHoiDanhGia",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoiBinhLuan_KhachHang_MaKH",
                table: "PhanHoiBinhLuan",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoiDanhGia_KhachHang_MaKH",
                table: "PhanHoiDanhGia",
                column: "MaKH",
                principalTable: "KhachHang",
                principalColumn: "MaKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_DanhMuc_MaDM",
                table: "SanPham",
                column: "MaDM",
                principalTable: "DanhMuc",
                principalColumn: "MaDM",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TonKho_SanPham_MaSP",
                table: "TonKho",
                column: "MaSP",
                principalTable: "SanPham",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHang_MaKH",
                table: "BinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_SanPham_MaSP",
                table: "BinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_CTGioHang_GioHang_MaGH",
                table: "CTGioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_CTGioHang_SanPham_MaSP",
                table: "CTGioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_CTGioHang_SanPham_SanPhamMaSP",
                table: "CTGioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_CTHD_HoaDon_SoHD",
                table: "CTHD");

            migrationBuilder.DropForeignKey(
                name: "FK_CTHD_TonKho_MaKho_MaSP",
                table: "CTHD");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_CTHD_SoHD_MaSP_MaKho",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_DSAnh_SanPham_MaSP",
                table: "DSAnh");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_KhachHang_MaKH",
                table: "GioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_KhachHang_KhachHangMaKH",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_KhachHang_MaKH",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_PTTT_MaPT",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_TrangThai_MaTT",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHang_AspNetUsers_UserId",
                table: "KhachHang");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeBinhLuan_KhachHang_MaKH",
                table: "LikeBinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_LikeDanhGia_KhachHang_MaKH",
                table: "LikeDanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_LikePhanHoiBinhLuan_KhachHang_MaKH",
                table: "LikePhanHoiBinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_LikePhanHoiDanhGia_KhachHang_MaKH",
                table: "LikePhanHoiDanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoiBinhLuan_KhachHang_MaKH",
                table: "PhanHoiBinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanHoiDanhGia_KhachHang_MaKH",
                table: "PhanHoiDanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_DanhMuc_MaDM",
                table: "SanPham");

            migrationBuilder.DropForeignKey(
                name: "FK_TonKho_SanPham_MaSP",
                table: "TonKho");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrangThai",
                table: "TrangThai");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PTTT",
                table: "PTTT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaXacThuc",
                table: "MaXacThuc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachHang",
                table: "KhachHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HoaDon",
                table: "HoaDon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMuc",
                table: "DanhMuc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CTHD",
                table: "CTHD");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CTGioHang",
                table: "CTGioHang");

            migrationBuilder.RenameTable(
                name: "TrangThai",
                newName: "TrangThais");

            migrationBuilder.RenameTable(
                name: "SanPham",
                newName: "SanPhams");

            migrationBuilder.RenameTable(
                name: "PTTT",
                newName: "PTTTs");

            migrationBuilder.RenameTable(
                name: "MaXacThuc",
                newName: "MaXacThucs");

            migrationBuilder.RenameTable(
                name: "KhachHang",
                newName: "KhachHangs");

            migrationBuilder.RenameTable(
                name: "HoaDon",
                newName: "HoaDons");

            migrationBuilder.RenameTable(
                name: "GioHang",
                newName: "GioHangs");

            migrationBuilder.RenameTable(
                name: "DanhMuc",
                newName: "DanhMucs");

            migrationBuilder.RenameTable(
                name: "CTHD",
                newName: "CTHDs");

            migrationBuilder.RenameTable(
                name: "CTGioHang",
                newName: "CTGioHangs");

            migrationBuilder.RenameIndex(
                name: "IX_SanPham_MaDM",
                table: "SanPhams",
                newName: "IX_SanPhams_MaDM");

            migrationBuilder.RenameIndex(
                name: "IX_KhachHang_UserId",
                table: "KhachHangs",
                newName: "IX_KhachHangs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_MaTT",
                table: "HoaDons",
                newName: "IX_HoaDons_MaTT");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_MaPT",
                table: "HoaDons",
                newName: "IX_HoaDons_MaPT");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_MaKH",
                table: "HoaDons",
                newName: "IX_HoaDons_MaKH");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_KhachHangMaKH",
                table: "HoaDons",
                newName: "IX_HoaDons_KhachHangMaKH");

            migrationBuilder.RenameIndex(
                name: "IX_GioHang_MaKH",
                table: "GioHangs",
                newName: "IX_GioHangs_MaKH");

            migrationBuilder.RenameIndex(
                name: "IX_CTHD_MaKho_MaSP",
                table: "CTHDs",
                newName: "IX_CTHDs_MaKho_MaSP");

            migrationBuilder.RenameIndex(
                name: "IX_CTGioHang_SanPhamMaSP",
                table: "CTGioHangs",
                newName: "IX_CTGioHangs_SanPhamMaSP");

            migrationBuilder.RenameIndex(
                name: "IX_CTGioHang_MaSP",
                table: "CTGioHangs",
                newName: "IX_CTGioHangs_MaSP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrangThais",
                table: "TrangThais",
                column: "MaTT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPhams",
                table: "SanPhams",
                column: "MaSP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PTTTs",
                table: "PTTTs",
                column: "MaPT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaXacThucs",
                table: "MaXacThucs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachHangs",
                table: "KhachHangs",
                column: "MaKH");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HoaDons",
                table: "HoaDons",
                column: "SoHD");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs",
                column: "MaGH");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMucs",
                table: "DanhMucs",
                column: "MaDM");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CTHDs",
                table: "CTHDs",
                columns: new[] { "SoHD", "MaSP", "MaKho" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CTGioHangs",
                table: "CTGioHangs",
                columns: new[] { "MaGH", "MaSP" });

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_KhachHangs_MaKH",
                table: "BinhLuan",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_SanPhams_MaSP",
                table: "BinhLuan",
                column: "MaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_CTGioHangs_GioHangs_MaGH",
                table: "CTGioHangs",
                column: "MaGH",
                principalTable: "GioHangs",
                principalColumn: "MaGH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTGioHangs_SanPhams_MaSP",
                table: "CTGioHangs",
                column: "MaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_CTGioHangs_SanPhams_SanPhamMaSP",
                table: "CTGioHangs",
                column: "SanPhamMaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP");

            migrationBuilder.AddForeignKey(
                name: "FK_CTHDs_HoaDons_SoHD",
                table: "CTHDs",
                column: "SoHD",
                principalTable: "HoaDons",
                principalColumn: "SoHD",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CTHDs_TonKho_MaKho_MaSP",
                table: "CTHDs",
                columns: new[] { "MaKho", "MaSP" },
                principalTable: "TonKho",
                principalColumns: new[] { "MaKho", "MaSP" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_CTHDs_SoHD_MaSP_MaKho",
                table: "DanhGia",
                columns: new[] { "SoHD", "MaSP", "MaKho" },
                principalTable: "CTHDs",
                principalColumns: new[] { "SoHD", "MaSP", "MaKho" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DSAnh_SanPhams_MaSP",
                table: "DSAnh",
                column: "MaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_KhachHangs_MaKH",
                table: "GioHangs",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_KhachHangs_KhachHangMaKH",
                table: "HoaDons",
                column: "KhachHangMaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_KhachHangs_MaKH",
                table: "HoaDons",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_PTTTs_MaPT",
                table: "HoaDons",
                column: "MaPT",
                principalTable: "PTTTs",
                principalColumn: "MaPT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_TrangThais_MaTT",
                table: "HoaDons",
                column: "MaTT",
                principalTable: "TrangThais",
                principalColumn: "MaTT",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangs_AspNetUsers_UserId",
                table: "KhachHangs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikeBinhLuan_KhachHangs_MaKH",
                table: "LikeBinhLuan",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeDanhGia_KhachHangs_MaKH",
                table: "LikeDanhGia",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePhanHoiBinhLuan_KhachHangs_MaKH",
                table: "LikePhanHoiBinhLuan",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_LikePhanHoiDanhGia_KhachHangs_MaKH",
                table: "LikePhanHoiDanhGia",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoiBinhLuan_KhachHangs_MaKH",
                table: "PhanHoiBinhLuan",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanHoiDanhGia_KhachHangs_MaKH",
                table: "PhanHoiDanhGia",
                column: "MaKH",
                principalTable: "KhachHangs",
                principalColumn: "MaKH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_DanhMucs_MaDM",
                table: "SanPhams",
                column: "MaDM",
                principalTable: "DanhMucs",
                principalColumn: "MaDM",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TonKho_SanPhams_MaSP",
                table: "TonKho",
                column: "MaSP",
                principalTable: "SanPhams",
                principalColumn: "MaSP",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
