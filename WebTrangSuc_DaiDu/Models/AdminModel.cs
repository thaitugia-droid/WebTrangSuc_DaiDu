using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace WebTrangSuc_DaiDu.Models
{
    public class AdminModel
    {
        // ==========================================
        // QUẢN LÝ SẢN PHẨM
        // ==========================================
        public static DataTable LayTatCaSanPham()
        {
            DataTable dt = new DataTable();
            using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SANPHAM ORDER BY STT DESC", ketNoi))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public static void ThemSanPham(string maSP, string tenSP, string maDM, string giaBan, string soLuong, string tenFileAnh, string moTa)
        {
            using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                string sql = "INSERT INTO SANPHAM (MaSP, TenSP, MaDM, GiaBan, SoLuongTon, HinhAnh, MoTa) VALUES (@Ma, @Ten, @DM, @Gia, @SL, @Anh, @MT)";
                using (SqlCommand cmd = new SqlCommand(sql, ketNoi))
                {
                    cmd.Parameters.AddWithValue("@Ma", maSP);
                    cmd.Parameters.AddWithValue("@Ten", tenSP);
                    cmd.Parameters.AddWithValue("@DM", maDM);
                    cmd.Parameters.AddWithValue("@Gia", giaBan);
                    cmd.Parameters.AddWithValue("@SL", soLuong);
                    cmd.Parameters.AddWithValue("@Anh", tenFileAnh);
                    cmd.Parameters.AddWithValue("@MT", moTa);
                    ketNoi.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void SuaSanPham(string maSP, string tenMoi, string giaMoi, string tenFileAnhFinal)
        {
            using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                string sql = "UPDATE SANPHAM SET TenSP = @Ten, GiaBan = @Gia, HinhAnh = @Anh WHERE MaSP = @Ma";
                using (SqlCommand cmd = new SqlCommand(sql, ketNoi))
                {
                    cmd.Parameters.AddWithValue("@Ten", tenMoi);
                    cmd.Parameters.AddWithValue("@Gia", giaMoi);
                    cmd.Parameters.AddWithValue("@Anh", tenFileAnhFinal);
                    cmd.Parameters.AddWithValue("@Ma", maSP);
                    ketNoi.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void XoaSanPham(string maSP)
        {
            using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM SANPHAM WHERE MaSP = @Ma", ketNoi))
                {
                    cmd.Parameters.AddWithValue("@Ma", maSP);
                    ketNoi.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ==========================================
        // QUẢN LÝ DANH MỤC
        // ==========================================
        public static DataTable LayBangDanhMuc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT MaDM, TenDM FROM DANHMUC", k))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public static void ThemDanhMuc(string ma, string ten)
        {
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand c = new SqlCommand("INSERT INTO DANHMUC VALUES (@Ma, @Ten)", k))
                {
                    c.Parameters.AddWithValue("@Ma", ma);
                    c.Parameters.AddWithValue("@Ten", ten);
                    k.Open();
                    c.ExecuteNonQuery();
                }
            }
        }

        public static void SuaDanhMuc(string maC, string maM, string tenM)
        {
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand c = new SqlCommand("UPDATE DANHMUC SET MaDM=@MaM, TenDM=@TenM WHERE MaDM=@MaC", k))
                {
                    c.Parameters.AddWithValue("@MaM", maM);
                    c.Parameters.AddWithValue("@TenM", tenM);
                    c.Parameters.AddWithValue("@MaC", maC);
                    k.Open();
                    c.ExecuteNonQuery();
                }
            }
        }

        public static void XoaDanhMuc(string ma)
        {
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand c = new SqlCommand("DELETE FROM DANHMUC WHERE MaDM=@Ma", k))
                {
                    c.Parameters.AddWithValue("@Ma", ma);
                    k.Open();
                    c.ExecuteNonQuery();
                }
            }
        }

        // ==========================================
        // QUẢN LÝ ĐƠN HÀNG
        // ==========================================
        public static DataTable LayDanhSachDonHang()
        {
            DataTable dt = new DataTable();
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM HOADON ORDER BY MaHD DESC", k))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public static void DuyetDonHang(string ma, string tt)
        {
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand c = new SqlCommand("UPDATE HOADON SET TrangThai=@TT WHERE MaHD=@Ma", k))
                {
                    c.Parameters.AddWithValue("@TT", tt);
                    c.Parameters.AddWithValue("@Ma", ma);
                    k.Open();
                    c.ExecuteNonQuery();
                }
            }
        }

        public static DataTable LayChiTietHoaDon(string ma)
        {
            DataTable dt = new DataTable();
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                string sql = "SELECT CT.MaSP, SP.TenSP, SP.HinhAnh, CT.SoLuongMua, CT.Dongia, (CT.SoLuongMua * CT.Dongia) AS ThanhTien " +
                             "FROM CHITIETHOADON CT INNER JOIN SANPHAM SP ON CT.MaSP = SP.MaSP WHERE CT.MaHD = @Ma";
                using (SqlCommand c = new SqlCommand(sql, k))
                {
                    c.Parameters.AddWithValue("@Ma", ma);
                    using (SqlDataAdapter da = new SqlDataAdapter(c))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // ==========================================
        // THỐNG KÊ
        // ==========================================
        public static void LayThongKeTong(out string tongDon, out double tongDoanhThu)
        {
            tongDon = "0";
            tongDoanhThu = 0;
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                k.Open();
                using (SqlCommand c = new SqlCommand("SELECT COUNT(MaHD) AS TongDon, SUM(TongTien) AS TongDoanhThu FROM HOADON", k))
                {
                    using (SqlDataReader d = c.ExecuteReader())
                    {
                        if (d.Read())
                        {
                            tongDon = d["TongDon"]?.ToString() ?? "0";
                            tongDoanhThu = d["TongDoanhThu"] != DBNull.Value ? Convert.ToDouble(d["TongDoanhThu"]) : 0;
                        }
                    }
                }
            }
        }

        public static DataTable LayThongKeThang()
        {
            DataTable dt = new DataTable();
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                string sql = "SELECT Month(NgayDat) AS Thang, Year(NgayDat) AS Nam, COUNT(MaHD) AS SoDonHang, SUM(TongTien) AS DoanhThuThang " +
                             "FROM HOADON GROUP BY Year(NgayDat), Month(NgayDat) ORDER BY Year(NgayDat) DESC, Month(NgayDat) DESC";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, k))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ==========================================
        // QUẢN LÝ TÀI KHOẢN (ĐÃ FIX TAIKHOAN -> MAKH)
        // ==========================================
        public static DataTable LayDanhSachTaiKhoan()
        {
            DataTable dt = new DataTable();
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                // 🔥 BÍ QUYẾT: Dùng "MaKH AS TaiKhoan" để giao diện không bị bỡ ngỡ
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT MaKH AS TaiKhoan, HoTen, Email, Quyen, HangThanhVien, TongChiTieu FROM KHACHHANG", k))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public static void CapNhatQuyen(string maKH, string quyen)
        {
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                // Đã sửa biến và câu truy vấn
                using (SqlCommand c = new SqlCommand("UPDATE KHACHHANG SET Quyen=@Q WHERE MaKH=@MaKH", k))
                {
                    c.Parameters.AddWithValue("@Q", quyen);
                    c.Parameters.AddWithValue("@MaKH", maKH);
                    k.Open();
                    c.ExecuteNonQuery();
                }
            }
        }

        public static void XoaTaiKhoan(string maKH)
        {
            using (SqlConnection k = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                // Đã sửa biến và câu truy vấn
                using (SqlCommand c = new SqlCommand("DELETE FROM KHACHHANG WHERE MaKH=@MaKH", k))
                {
                    c.Parameters.AddWithValue("@MaKH", maKH);
                    k.Open();
                    c.ExecuteNonQuery();
                }
            }
        }
    }

    // ==========================================
    // CORE WEB MANAGER (KHÔNG ĐỔI)
    // ==========================================
    public class CoreWebManager
    {
        public static void LuuLichSuXem(string maSP)
        {
            List<string> ds = HttpContext.Current.Session["LichSuXem"] as List<string> ?? new List<string>();
            ds.Remove(maSP);
            ds.Insert(0, maSP);
            if (ds.Count > 5) ds.RemoveAt(5);
            HttpContext.Current.Session["LichSuXem"] = ds;
        }

        public static DataTable LayDoPhuHop(string maDM)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 4 * FROM SANPHAM WHERE MaDM = @DM ORDER BY NEWID()", conn))
                {
                    cmd.Parameters.AddWithValue("@DM", maDM);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static DataTable LayDataSoSanh(List<string> dsMaSP)
        {
            DataTable dt = new DataTable();
            if (dsMaSP.Count == 0) return dt;
            string maSPs = "'" + string.Join("','", dsMaSP) + "'";
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SANPHAM WHERE MaSP IN (" + maSPs + ")", conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public static bool KichHoatGodMode(string pinNhap)
        {
            if (pinNhap == "5509" && HttpContext.Current.Session["Quyen"] != null && (HttpContext.Current.Session["Quyen"].ToString() == "1" || HttpContext.Current.Session["Quyen"].ToString().ToLower() == "admin"))
            {
                HttpContext.Current.Session["GodMode_Active"] = true;
                return true;
            }
            return false;
        }

        public static bool IsGodMode()
        {
            return HttpContext.Current.Session["Quyen"] != null && (HttpContext.Current.Session["Quyen"].ToString() == "1" || HttpContext.Current.Session["Quyen"].ToString().ToLower() == "admin") && HttpContext.Current.Session["GodMode_Active"] != null;
        }

        public static DataTable LayThongKeMatThan(string maSP)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SoLuongTon, LuotXem FROM SANPHAM WHERE MaSP = @Ma", conn))
                {
                    cmd.Parameters.AddWithValue("@Ma", maSP);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static bool ThaoTacNhanh(string maSP, string hanhDong)
        {
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE SANPHAM SET NhanMac = @HanhDong WHERE MaSP = @MaSP", conn))
                {
                    cmd.Parameters.AddWithValue("@HanhDong", hanhDong);
                    cmd.Parameters.AddWithValue("@MaSP", maSP);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static string AIPhanTichChienLuoc(int tonKho, int luotXem)
        {
            if (tonKho > 50 && luotXem < 100) return "CẢNH BÁO: Hàng tồn đọng! Đề xuất xả hàng Flash Sale 30%.";
            else if (tonKho < 10 && luotXem > 500) return "TÍN HIỆU TỐT: Hàng khan hiếm! Đề xuất tăng giá 10% hoặc gỡ mác Sale.";
            else return "ỔN ĐỊNH: Tiếp tục duy trì mức giá hiện tại.";
        }
    }
}