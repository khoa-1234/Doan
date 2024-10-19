using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLNH.Data.Models;

public partial class QlnhContext : DbContext
{
    public QlnhContext()
    {
    }

    public QlnhContext(DbContextOptions<QlnhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ban> Bans { get; set; }

    public virtual DbSet<CaLamViec> CaLamViecs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; }

    public virtual DbSet<ChiTietNhapKho> ChiTietNhapKhoes { get; set; }

    public virtual DbSet<DangKyLichLamViec> DangKyLichLamViecs { get; set; }

    public virtual DbSet<DatBan> DatBans { get; set; }

    public virtual DbSet<DiemDanh> DiemDanhs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhuVuc> KhuVucs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichLamViec> LichLamViecs { get; set; }

    public virtual DbSet<MoMoNotification> MoMoNotifications { get; set; }

    public virtual DbSet<MonAn> MonAns { get; set; }

    public virtual DbSet<MonAnHinhAnh> MonAnHinhAnhs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<NhomMonAn> NhomMonAns { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Puser> Pusers { get; set; }

    public virtual DbSet<QuyTrinhNauAn> QuyTrinhNauAns { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<VBanTrangThai> VBanTrangThais { get; set; }

    public virtual DbSet<VBanTrangThaiHomNay> VBanTrangThaiHomNays { get; set; }

    public virtual DbSet<VanTay> VanTays { get; set; }

    public virtual DbSet<VatTu> VatTus { get; set; }

    public virtual DbSet<VwXemDonHang> VwXemDonHangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=QLNHDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ban>(entity =>
        {
            entity.HasKey(e => e.BanId).HasName("PK__Ban__991CE765C3AFD115");

            entity.ToTable("Ban");

            entity.HasIndex(e => e.SoBan, "UQ__Ban__21B4EECB458B5E0E")
                .IsUnique()
                .HasFilter("([SoBan] IS NOT NULL)");

            entity.Property(e => e.BanId).HasColumnName("BanID");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");

            entity.HasOne(d => d.KhuVuc).WithMany(p => p.Bans)
                .HasForeignKey(d => d.KhuVucId)
                .HasConstraintName("FK__Ban__KhuVucId__0C85DE4D");
        });

        modelBuilder.Entity<CaLamViec>(entity =>
        {
            entity.HasKey(e => e.CaLamViecId).HasName("PK__CaLamVie__D7E8733E3B9FF2AF");

            entity.ToTable("CaLamViec");

            entity.Property(e => e.CaLamViecId).HasColumnName("CaLamViecID");
            entity.Property(e => e.TenCa).HasMaxLength(50);
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietDonHangId).HasName("PK__ChiTietD__45B33F830C844BE4");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.ChiTietDonHangId).HasColumnName("ChiTietDonHangID");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.DonHang).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK_ChiTietDonHang_DonHang");

            entity.HasOne(d => d.MonAn).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MonAnId)
                .HasConstraintName("FK_ChiTietDonHang_MonAn");
        });

        modelBuilder.Entity<ChiTietKhuyenMai>(entity =>
        {
            entity.HasKey(e => e.ChiTietKhuyenMaiId).HasName("PK__ChiTietK__FDC5C4B20EDF991A");

            entity.ToTable("ChiTietKhuyenMai");

            entity.HasIndex(e => e.KhuyenMaiId, "IX_ChiTietKhuyenMai_KhuyenMaiID");

            entity.HasIndex(e => e.SanPhamId, "IX_ChiTietKhuyenMai_SanPhamID");

            entity.Property(e => e.ChiTietKhuyenMaiId).HasColumnName("ChiTietKhuyenMaiID");
            entity.Property(e => e.KhuyenMaiId).HasColumnName("KhuyenMaiID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");

            entity.HasOne(d => d.KhuyenMai).WithMany(p => p.ChiTietKhuyenMais)
                .HasForeignKey(d => d.KhuyenMaiId)
                .HasConstraintName("FK__ChiTietKh__Khuye__5CD6CB2B");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietKhuyenMais)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK__ChiTietKh__SanPh__5DCAEF64");
        });

        modelBuilder.Entity<ChiTietNhapKho>(entity =>
        {
            entity.HasKey(e => e.ChiTietNhapKhoId).HasName("PK__ChiTietN__0F86D7D9D29AB350");

            entity.ToTable("ChiTietNhapKho");

            entity.HasIndex(e => e.VatTuId, "IX_ChiTietNhapKho_VatTuID");

            entity.Property(e => e.ChiTietNhapKhoId).HasColumnName("ChiTietNhapKhoID");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VatTuId).HasColumnName("VatTuID");

            entity.HasOne(d => d.VatTu).WithMany(p => p.ChiTietNhapKhoes)
                .HasForeignKey(d => d.VatTuId)
                .HasConstraintName("FK__ChiTietNh__VatTu__5EBF139D");
        });

        modelBuilder.Entity<DangKyLichLamViec>(entity =>
        {
            entity.HasKey(e => e.DangKyLichLamViecId).HasName("PK__DangKyLi__6CD2E20A2330B327");

            entity.ToTable("DangKyLichLamViec");

            entity.HasIndex(e => e.NhanVienId, "IX_DangKyLichLamViec_NhanVienId");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("ChuaDuyet");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.DangKyLichLamViecs)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKyLichLamViec_NhanVien");
        });

        modelBuilder.Entity<DatBan>(entity =>
        {
            entity.HasKey(e => e.DatBanId).HasName("PK__DatBan__6A75F719F00BB5BE");

            entity.ToTable("DatBan");

            entity.HasIndex(e => e.BanId, "IX_DatBan_BanID");

            entity.HasIndex(e => e.KhachHangId, "IX_DatBan_KhachHangID");

            entity.HasIndex(e => e.NhanVienMoBanId, "IX_DatBan_NhanVienMoBanID");

            entity.Property(e => e.DatBanId).HasColumnName("DatBanID");
            entity.Property(e => e.BanId).HasColumnName("BanID");
            entity.Property(e => e.CoDatMon).HasDefaultValue(false);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NhanVienMoBanId).HasColumnName("NhanVienMoBanID");
            entity.Property(e => e.PhuongThucDat).HasMaxLength(50);
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianDat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianDongBan).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTaoBan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ duyệt");
            entity.Property(e => e.TrangThaiXacNhan).HasMaxLength(50);

            entity.HasOne(d => d.Ban).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.BanId)
                .HasConstraintName("FK__DatBan__BanID__0A9D95DB");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("FK__DatBan__KhachHan__0B91BA14");

            entity.HasOne(d => d.KhuVuc).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.KhuVucId)
                .HasConstraintName("FK__DatBan__KhuVucId__0D7A0286");

            entity.HasOne(d => d.NhanVienMoBan).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.NhanVienMoBanId)
                .HasConstraintName("FK__DatBan__NhanVien__0C85DE4D");
        });

        modelBuilder.Entity<DiemDanh>(entity =>
        {
            entity.HasKey(e => e.DiemDanhId).HasName("PK__DiemDanh__972C4F0380EBC590");

            entity.ToTable("DiemDanh");

            entity.HasIndex(e => e.NhanVienId, "IX_DiemDanh_NhanVienID");

            entity.HasIndex(e => e.VanTayId, "IX_DiemDanh_VanTayID");

            entity.Property(e => e.DiemDanhId).HasColumnName("DiemDanhID");
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.VanTayId).HasColumnName("VanTayID");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__DiemDanh__NhanVi__628FA481");

            entity.HasOne(d => d.VanTay).WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => d.VanTayId)
                .HasConstraintName("FK_DiemDanh_VanTay");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.DonHangId).HasName("PK__DonHang__D159F4DEC420024C");

            entity.ToTable("DonHang");

            entity.HasIndex(e => e.NhanVienId, "IX_DonHang_NhanVienID");

            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.DatBanId).HasColumnName("DatBanID");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.TongTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(100);

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("FK_DonHang_KhachHang");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__DonHang__NhanVie__6383C8BA");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.HoaDonId).HasName("PK__HoaDon__6956CE699F35DB35");

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.DonHangId, "IX_HoaDon_DonHangID");

            entity.Property(e => e.HoaDonId).HasColumnName("HoaDonID");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TongTienSauVat)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("TongTienSauVAT");
            entity.Property(e => e.Vat)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("VAT");

            entity.HasOne(d => d.DonHang).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK__HoaDon__DonHangI__6477ECF3");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211B3461BF6E");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.UserId, "IX_KhachHang_UserID");

            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User_Khachhang");
        });

        modelBuilder.Entity<KhuVuc>(entity =>
        {
            entity.HasKey(e => e.KhuVucId).HasName("PK__KhuVuc__201CA7F9DD57A64E");

            entity.ToTable("KhuVuc");

            entity.Property(e => e.Mota).HasMaxLength(500);
            entity.Property(e => e.TenKhuVuc).HasMaxLength(100);
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.KhuyenMaiId).HasName("PK__KhuyenMa__820D7477883D5679");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.KhuyenMaiId).HasColumnName("KhuyenMaiID");
            entity.Property(e => e.PhanTramGiamGia).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenKhuyenMai).HasMaxLength(100);
        });

        modelBuilder.Entity<LichLamViec>(entity =>
        {
            entity.HasKey(e => e.LichLamViecId).HasName("PK__LichLamV__0371487D248C7812");

            entity.ToTable("LichLamViec");

            entity.HasIndex(e => e.DangKyLichLamViecId, "IX_LichLamViec_DangKyLichLamViecId");

            entity.HasIndex(e => e.NhanVienId, "IX_LichLamViec_NhanVienId");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");

            entity.HasOne(d => d.DangKyLichLamViec).WithMany(p => p.LichLamViecs)
                .HasForeignKey(d => d.DangKyLichLamViecId)
                .HasConstraintName("FK_LichLamViec_DangKy");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.LichLamViecs)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LichLamViec_NhanVien");
        });

        modelBuilder.Entity<MoMoNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__MoMoNoti__20CF2E12BFAF53A6");

            entity.HasIndex(e => e.TransactionId, "IX_MoMoNotifications_TransactionId");

            entity.Property(e => e.ReceivedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Transaction).WithMany(p => p.MoMoNotifications)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MoMoNotifications_Transactions");
        });

        modelBuilder.Entity<MonAn>(entity =>
        {
            entity.HasKey(e => e.MonAnId).HasName("PK__SanPham__05180FF4FAAB0AC5");

            entity.ToTable("MonAn");

            entity.HasIndex(e => e.NhomMonAnId, "IX_SanPham_NhomMonAnID");

            entity.Property(e => e.MonAnId).HasColumnName("MonAnID");
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.NhomMonAnId).HasColumnName("NhomMonAnID");
            entity.Property(e => e.TenMonAn)
                .HasMaxLength(100)
                .HasColumnName("TenMonAN");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");

            entity.HasOne(d => d.NhomMonAn).WithMany(p => p.MonAns)
                .HasForeignKey(d => d.NhomMonAnId)
                .HasConstraintName("FK__SanPham__NhomMon__68487DD7");
        });

        modelBuilder.Entity<MonAnHinhAnh>(entity =>
        {
            entity.HasKey(e => e.HinhAnhId).HasName("PK__SanPhamH__8EF32B7B0F048935");

            entity.ToTable("MonAnHinhAnh");

            entity.HasIndex(e => e.MonAnId, "IX_SanPhamHinhAnh_SanPhamID");

            entity.Property(e => e.HinhAnhId).HasColumnName("HinhAnhID");
            entity.Property(e => e.MonAnId).HasColumnName("MonAnID");

            entity.HasOne(d => d.MonAn).WithMany(p => p.MonAnHinhAnhs)
                .HasForeignKey(d => d.MonAnId)
                .HasConstraintName("FK_SanPhamHinhAnh_SanPham");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EA5757DEC0");

            entity.ToTable("NhanVien");

            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.BoPhan).HasMaxLength(50);
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_NhanVien_UserID");
        });

        modelBuilder.Entity<NhomMonAn>(entity =>
        {
            entity.HasKey(e => e.NhomMonAnId).HasName("PK__NhomMonA__9E6041A95A3DA90B");

            entity.ToTable("NhomMonAn");

            entity.Property(e => e.NhomMonAnId).HasColumnName("NhomMonAnID");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.TenNhom).HasMaxLength(100);
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_NhomMonAn_NhomMonAn");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OTP__3214EC07E986BDDA");

            entity.ToTable("OTP");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Otpcode)
                .HasMaxLength(10)
                .HasColumnName("OTPCode");
        });

        modelBuilder.Entity<Puser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC52E6C7C4");

            entity.ToTable("PUser");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<QuyTrinhNauAn>(entity =>
        {
            entity.HasKey(e => e.QuyTrinhNauAnId).HasName("PK__QuyTrinh__ABFB4024B605620B");

            entity.ToTable("QuyTrinhNauAn");

            entity.HasIndex(e => e.SanPhamId, "IX_QuyTrinhNauAn_SanPhamID");

            entity.Property(e => e.QuyTrinhNauAnId).HasColumnName("QuyTrinhNauAnID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

            entity.HasOne(d => d.SanPham).WithMany(p => p.QuyTrinhNauAns)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK__QuyTrinhN__SanPh__6754599E");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.ThanhToanId).HasName("PK__ThanhToa__24A8D6843F43BB67");

            entity.ToTable("ThanhToan");

            entity.HasIndex(e => e.DonHangId, "IX_ThanhToan_DonHangID");

            entity.Property(e => e.ThanhToanId).HasColumnName("ThanhToanID");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.MaQr)
                .HasMaxLength(255)
                .HasColumnName("MaQR");
            entity.Property(e => e.SoTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.DonHang).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK__ThanhToan__DonHa__693CA210");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6B5BE037A0");

            entity.HasIndex(e => e.DonHangId, "IX_Transactions_DonHangID");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.MoMoOrderInfo).HasMaxLength(255);
            entity.Property(e => e.MoMoRequestId).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.DonHang).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DonHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Orders");
        });

        modelBuilder.Entity<VBanTrangThai>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_BanTrangThai");

            entity.Property(e => e.DatBanId).HasMaxLength(30);
            entity.Property(e => e.DonHangId).HasMaxLength(30);
            entity.Property(e => e.KhachHangId).HasMaxLength(30);
            entity.Property(e => e.NgayGioDat)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(6);
        });

        modelBuilder.Entity<VBanTrangThaiHomNay>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_BanTrangThaiHomNay");

            entity.Property(e => e.DatBanId).HasMaxLength(30);
            entity.Property(e => e.DonHangId).HasMaxLength(30);
            entity.Property(e => e.KhachHangId).HasMaxLength(30);
            entity.Property(e => e.NgayGioDat)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(6);
        });

        modelBuilder.Entity<VanTay>(entity =>
        {
            entity.HasKey(e => e.VanTayId).HasName("PK__VanTay__9C0D52B9CE65EAF6");

            entity.ToTable("VanTay");

            entity.HasIndex(e => e.NhanVienId, "IX_VanTay_NhanVienID");

            entity.Property(e => e.VanTayId).HasColumnName("VanTayID");
            entity.Property(e => e.MaVanTayHex).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasMaxLength(100);
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.ThoiGianCapNhat).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianTao).HasColumnType("datetime");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.VanTays)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK__VanTay__NhanVien__6A30C649");
        });

        modelBuilder.Entity<VatTu>(entity =>
        {
            entity.HasKey(e => e.VatTuId).HasName("PK__VatTu__4BE70C76E617E6B0");

            entity.ToTable("VatTu");

            entity.Property(e => e.VatTuId).HasColumnName("VatTuID");
            entity.Property(e => e.DonViTinh).HasMaxLength(50);
            entity.Property(e => e.GiaMua).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TenVatTu).HasMaxLength(100);
        });

        modelBuilder.Entity<VwXemDonHang>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_XemDonHang");

            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
