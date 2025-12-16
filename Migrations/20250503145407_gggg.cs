using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebsiteBanHang.Migrations
{
    /// <inheritdoc />
    public partial class gggg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucs",
                columns: table => new
                {
                    MaDM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucs", x => x.MaDM);
                });

            migrationBuilder.CreateTable(
                name: "Kho",
                columns: table => new
                {
                    MaKho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: false),
                    DCKho = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kho", x => x.MaKho);
                });

            migrationBuilder.CreateTable(
                name: "MaXacThucs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaXacNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaSuDung = table.Column<bool>(type: "bit", nullable: false),
                    SoLanThu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaXacThucs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTTTs",
                columns: table => new
                {
                    MaPT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTTTs", x => x.MaPT);
                });

            migrationBuilder.CreateTable(
                name: "TrangThais",
                columns: table => new
                {
                    MaTT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThais", x => x.MaTT);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenKH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinhKH = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiaChiKH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTKH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailKH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.MaKH);
                    table.ForeignKey(
                        name: "FK_KhachHangs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    MaSP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDM = table.Column<int>(type: "int", nullable: false),
                    TenSP = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gia = table.Column<long>(type: "bigint", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TTDeXuat = table.Column<bool>(type: "bit", nullable: false),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK_SanPhams_DanhMucs_MaDM",
                        column: x => x.MaDM,
                        principalTable: "DanhMucs",
                        principalColumn: "MaDM",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    MaGH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangs", x => x.MaGH);
                    table.ForeignKey(
                        name: "FK_GioHangs_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    SoHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPT = table.Column<int>(type: "int", nullable: false),
                    MaTT = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayGiaoDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaChiGiaoHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTNhanHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoLanDoiTT = table.Column<int>(type: "int", nullable: false),
                    KhachHangMaKH = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.SoHD);
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachHangs_KhachHangMaKH",
                        column: x => x.KhachHangMaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_HoaDons_PTTTs_MaPT",
                        column: x => x.MaPT,
                        principalTable: "PTTTs",
                        principalColumn: "MaPT",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDons_TrangThais_MaTT",
                        column: x => x.MaTT,
                        principalTable: "TrangThais",
                        principalColumn: "MaTT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DSAnhs",
                columns: table => new
                {
                    MaAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DSAnhs", x => x.MaAnh);
                    table.ForeignKey(
                        name: "FK_DSAnhs_SanPhams_MaSP",
                        column: x => x.MaSP,
                        principalTable: "SanPhams",
                        principalColumn: "MaSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanHoi",
                columns: table => new
                {
                    MaPH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    NoiDungPH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayPH = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TonKho",
                columns: table => new
                {
                    MaKho = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    SLTon = table.Column<int>(type: "int", nullable: false),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TonKho", x => new { x.MaKho, x.MaSP });
                    table.ForeignKey(
                        name: "FK_TonKho_Kho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "Kho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TonKho_SanPhams_MaSP",
                        column: x => x.MaSP,
                        principalTable: "SanPhams",
                        principalColumn: "MaSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTGioHangs",
                columns: table => new
                {
                    MaGH = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    SoLuongThem = table.Column<int>(type: "int", nullable: false),
                    GiaLucThem = table.Column<long>(type: "bigint", nullable: false),
                    ThoiGianThemDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaThemCuoi = table.Column<long>(type: "bigint", nullable: false),
                    ThoiGianThemCuoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiChon = table.Column<bool>(type: "bit", nullable: false),
                    SanPhamMaSP = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTGioHangs", x => new { x.MaGH, x.MaSP });
                    table.ForeignKey(
                        name: "FK_CTGioHangs_GioHangs_MaGH",
                        column: x => x.MaGH,
                        principalTable: "GioHangs",
                        principalColumn: "MaGH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTGioHangs_SanPhams_MaSP",
                        column: x => x.MaSP,
                        principalTable: "SanPhams",
                        principalColumn: "MaSP");
                    table.ForeignKey(
                        name: "FK_CTGioHangs_SanPhams_SanPhamMaSP",
                        column: x => x.SanPhamMaSP,
                        principalTable: "SanPhams",
                        principalColumn: "MaSP");
                });

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
                        name: "FK_LikePhanHoi_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_LikePhanHoi_PhanHoi_MaPH",
                        column: x => x.MaPH,
                        principalTable: "PhanHoi",
                        principalColumn: "MaPH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraLoi",
                columns: table => new
                {
                    MaTL = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPH = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    NoiDungTL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTL = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraLoi", x => x.MaTL);
                    table.ForeignKey(
                        name: "FK_TraLoi_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_TraLoi_PhanHoi_MaPH",
                        column: x => x.MaPH,
                        principalTable: "PhanHoi",
                        principalColumn: "MaPH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTHDs",
                columns: table => new
                {
                    SoHD = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    MaKho = table.Column<int>(type: "int", nullable: false),
                    SL = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTHDs", x => new { x.SoHD, x.MaSP, x.MaKho });
                    table.ForeignKey(
                        name: "FK_CTHDs_HoaDons_SoHD",
                        column: x => x.SoHD,
                        principalTable: "HoaDons",
                        principalColumn: "SoHD",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTHDs_TonKho_MaKho_MaSP",
                        columns: x => new { x.MaKho, x.MaSP },
                        principalTable: "TonKho",
                        principalColumns: new[] { "MaKho", "MaSP" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikeTraLoi",
                columns: table => new
                {
                    MaTL = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    MaDG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKho = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    SoHD = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Sao = table.Column<int>(type: "int", nullable: false),
                    NgayDG = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TTHienThi = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.MaDG);
                    table.ForeignKey(
                        name: "FK_DanhGia_CTHDs_SoHD_MaSP_MaKho",
                        columns: x => new { x.SoHD, x.MaSP, x.MaKho },
                        principalTable: "CTHDs",
                        principalColumns: new[] { "SoHD", "MaSP", "MaKho" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnhDG",
                columns: table => new
                {
                    MaAnhDG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDG = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhDG", x => x.MaAnhDG);
                    table.ForeignKey(
                        name: "FK_AnhDG_DanhGia_MaDG",
                        column: x => x.MaDG,
                        principalTable: "DanhGia",
                        principalColumn: "MaDG",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikeDanhGia",
                columns: table => new
                {
                    MaDG = table.Column<int>(type: "int", nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    TGThich = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeDanhGia", x => new { x.MaKH, x.MaDG });
                    table.ForeignKey(
                        name: "FK_LikeDanhGia_DanhGia_MaDG",
                        column: x => x.MaDG,
                        principalTable: "DanhGia",
                        principalColumn: "MaDG",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikeDanhGia_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "PhanHoiDanhGia",
                columns: table => new
                {
                    MaPHDG = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaDG = table.Column<int>(type: "int", nullable: false),
                    NoiDungPHDG = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayPHDG = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoiDanhGia", x => x.MaPHDG);
                    table.ForeignKey(
                        name: "FK_PhanHoiDanhGia_DanhGia_MaDG",
                        column: x => x.MaDG,
                        principalTable: "DanhGia",
                        principalColumn: "MaDG",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanHoiDanhGia_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikePhanHoiDanhGia",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaPHDG = table.Column<int>(type: "int", nullable: false),
                    TGThich = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikePhanHoiDanhGia", x => new { x.MaKH, x.MaPHDG });
                    table.ForeignKey(
                        name: "FK_LikePhanHoiDanhGia_KhachHangs_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_LikePhanHoiDanhGia_PhanHoiDanhGia_MaPHDG",
                        column: x => x.MaPHDG,
                        principalTable: "PhanHoiDanhGia",
                        principalColumn: "MaPHDG",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnhDG_MaDG",
                table: "AnhDG",
                column: "MaDG");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CTGioHangs_MaSP",
                table: "CTGioHangs",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_CTGioHangs_SanPhamMaSP",
                table: "CTGioHangs",
                column: "SanPhamMaSP");

            migrationBuilder.CreateIndex(
                name: "IX_CTHDs_MaKho_MaSP",
                table: "CTHDs",
                columns: new[] { "MaKho", "MaSP" });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_SoHD_MaSP_MaKho",
                table: "DanhGia",
                columns: new[] { "SoHD", "MaSP", "MaKho" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DSAnhs_MaSP",
                table: "DSAnhs",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_MaKH",
                table: "GioHangs",
                column: "MaKH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_KhachHangMaKH",
                table: "HoaDons",
                column: "KhachHangMaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaKH",
                table: "HoaDons",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaPT",
                table: "HoaDons",
                column: "MaPT");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaTT",
                table: "HoaDons",
                column: "MaTT");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_UserId",
                table: "KhachHangs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeDanhGia_MaDG",
                table: "LikeDanhGia",
                column: "MaDG");

            migrationBuilder.CreateIndex(
                name: "IX_LikePhanHoi_MaPH",
                table: "LikePhanHoi",
                column: "MaPH");

            migrationBuilder.CreateIndex(
                name: "IX_LikePhanHoiDanhGia_MaPHDG",
                table: "LikePhanHoiDanhGia",
                column: "MaPHDG");

            migrationBuilder.CreateIndex(
                name: "IX_LikeTraLoi_MaTL",
                table: "LikeTraLoi",
                column: "MaTL");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_MaKH",
                table: "PhanHoi",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_MaSP",
                table: "PhanHoi",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoiDanhGia_MaDG",
                table: "PhanHoiDanhGia",
                column: "MaDG");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoiDanhGia_MaKH",
                table: "PhanHoiDanhGia",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_MaDM",
                table: "SanPhams",
                column: "MaDM");

            migrationBuilder.CreateIndex(
                name: "IX_TonKho_MaSP",
                table: "TonKho",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoi_MaKH",
                table: "TraLoi",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoi_MaPH",
                table: "TraLoi",
                column: "MaPH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhDG");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CTGioHangs");

            migrationBuilder.DropTable(
                name: "DSAnhs");

            migrationBuilder.DropTable(
                name: "LikeDanhGia");

            migrationBuilder.DropTable(
                name: "LikePhanHoi");

            migrationBuilder.DropTable(
                name: "LikePhanHoiDanhGia");

            migrationBuilder.DropTable(
                name: "LikeTraLoi");

            migrationBuilder.DropTable(
                name: "MaXacThucs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "GioHangs");

            migrationBuilder.DropTable(
                name: "PhanHoiDanhGia");

            migrationBuilder.DropTable(
                name: "TraLoi");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "PhanHoi");

            migrationBuilder.DropTable(
                name: "CTHDs");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "TonKho");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "PTTTs");

            migrationBuilder.DropTable(
                name: "TrangThais");

            migrationBuilder.DropTable(
                name: "Kho");

            migrationBuilder.DropTable(
                name: "SanPhams");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DanhMucs");
        }
    }
}
