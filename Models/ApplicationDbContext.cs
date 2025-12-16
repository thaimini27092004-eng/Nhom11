using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebsiteBanHang.Models.NguoiDung;
using WebsiteBanHang.Models.GioHang;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using WebsiteBanHang.Models.QLTonKho;
using WebsiteBanHang.Models.QLDanhGia_PhanHoi;
using WebsiteBanHang.Models.QuanLyBanner;
using WebsiteBanHang.Models.QuanLyTrangThai;

public class ApplicationDbContext : IdentityDbContext<ThongTinNguoiDung>
{
    public
   ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
   base(options)
    {
    }
    public DbSet<HoaDon> HoaDon { get; set; }
    public DbSet<KhachHang> KhachHang { get; set; }
    public DbSet<PTTT> PTTT { get; set; }
    public DbSet<TrangThai> TrangThai { get; set; }
    public DbSet<CTHD> CTHD { get; set; }
    public DbSet<GioHang> GioHang { get; set; } 
    public DbSet<CTGioHang> CTGioHang { get; set; }
    public DbSet<SanPham> SanPham { get; set; }
    public DbSet<DanhMuc> DanhMuc { get; set; }
    public DbSet<DSAnh> DSAnh { get; set; }
    public DbSet<Kho> Kho { get; set; }

    public DbSet<TonKho> TonKho { get; set; }
    public DbSet<DanhGia> DanhGia { get; set; }
    public DbSet<AnhDG> AnhDG { get; set; }
    public DbSet<PhanHoiDanhGia> PhanHoiDanhGia { get; set; }
    public DbSet<BinhLuan> BinhLuan { get; set; }

    public DbSet<PhanHoiBinhLuan> PhanHoiBinhLuan { get; set; }
    public DbSet<LikeDanhGia> LikeDanhGia { get; set; }
    public DbSet<LikePhanHoiDanhGia> LikePhanHoiDanhGia { get; set; }

    public DbSet<LikeBinhLuan> LikeBinhLuan { get; set; }
    public DbSet<LikePhanHoiBinhLuan> LikePhanHoiBinhLuan { get; set; }

    public DbSet<MaXacThuc> MaXacThuc { get; set; } // Thêm dòng này
    public DbSet<NhanVien> NhanVien { get; set; }

    public DbSet<VaiTroNhanVien> VaiTroNhanVien { get; set; } // Thêm dòng này
    public DbSet<BannerQuangCao> BannerQuangCao { get; set; }

    public DbSet<LichSuTTHD> LichSuTTHD { get; set; }
    public DbSet<GmailThongBao> GmailThongBao { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Cấu hình quan hệ giữa LichSuTTHD và HoaDon
        modelBuilder.Entity<LichSuTTHD>()
            .HasOne(ls => ls.HoaDon)
            .WithMany(hd => hd.LichSuTTHD)
            .HasForeignKey(ls => ls.SoHD)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình quan hệ giữa LichSuTTHD và TrangThai
        modelBuilder.Entity<LichSuTTHD>()
            .HasOne(ls => ls.TrangThai)
            .WithMany(tt => tt.LichSuTTHD)
            .HasForeignKey(ls => ls.MaTT)
            .OnDelete(DeleteBehavior.NoAction);

        // Đảm bảo (SoHD, MaSP, MaKho) trong DanhGia là duy nhất
        modelBuilder.Entity<DanhGia>()
            .HasIndex(dg => new { dg.SoHD, dg.MaSP, dg.MaKho })
            .IsUnique();

        
        // Cấu hình quan hệ nhiều-1 giữa HoaDon và KhachHang, sử dụng ON DELETE SET NULL.
        //Ngăn khách hàng bị xoá khiến hoá đơn bị xoá
        modelBuilder.Entity<HoaDon>()
            .HasOne(ph => ph.KhachHang)
            .WithMany()
            .HasForeignKey(ph => ph.MaKH)
            .OnDelete(DeleteBehavior.NoAction);


        // Cấu hình quan hệ nhiều-1 giữa TraLoi và KhachHang
        //ngăn xoá trùng, và việc xoá sẽ do PhanHoi đảm nhận
        modelBuilder.Entity<PhanHoiBinhLuan>()
             .HasOne(ct => ct.KhachHang)
             .WithMany()
             .HasForeignKey(ct => ct.MaKH)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ nhiều-1 giữa CTGioHang và SanPham
        //ngăn việc khi xoá khách hàng thông qua giỏ hàng khiến sản phẩm bị xoá theo
        modelBuilder.Entity<CTGioHang>()
             .HasOne(ct => ct.SanPham)
             .WithMany()
             .HasForeignKey(ct => ct.MaSP)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ nhiều-1 giữa PhanHoi và SanPham
        //ngăn việc khi xoá sản phâm thông qua giỏ hàng khiến sản phẩm bị xoá theo
        modelBuilder.Entity<BinhLuan>()
             .HasOne(ct => ct.SanPham)
             .WithMany()
             .HasForeignKey(ct => ct.MaSP)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ nhiều-1 giữa LikePhanHoi và KhachHang
        modelBuilder.Entity<LikeBinhLuan>()
             .HasOne(ct => ct.KhachHang)
             .WithMany()
             .HasForeignKey(ct => ct.MaKH)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ nhiều-1 giữa LikeDanhGia và KhachHang
        modelBuilder.Entity<LikeDanhGia>()
             .HasOne(ct => ct.KhachHang)
             .WithMany()
             .HasForeignKey(ct => ct.MaKH)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ nhiều-1 giữa LikeTraLoi và KhachHang
        modelBuilder.Entity<LikePhanHoiBinhLuan>()
             .HasOne(ct => ct.KhachHang)
             .WithMany()
             .HasForeignKey(ct => ct.MaKH)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ nhiều-1 giữa LikePhanHoiDanhGia và KhachHang
        modelBuilder.Entity<LikePhanHoiDanhGia>()
             .HasOne(ct => ct.KhachHang)
             .WithMany()
             .HasForeignKey(ct => ct.MaKH)
             .OnDelete(DeleteBehavior.NoAction);

        // Cấu hình quan hệ cho NhanVien
        modelBuilder.Entity<NhanVien>()
            .HasOne(nv => nv.User)
            .WithMany()
            .HasForeignKey(nv => nv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình quan hệ cho VaiTroNhanVien
        modelBuilder.Entity<VaiTroNhanVien>()
            .HasOne(vt => vt.NhanVien)
            .WithMany()
            .HasForeignKey(vt => vt.MaNV)
            .OnDelete(DeleteBehavior.Cascade);




    }

}