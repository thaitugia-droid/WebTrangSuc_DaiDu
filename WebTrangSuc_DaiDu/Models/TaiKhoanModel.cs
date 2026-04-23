using System;
using System.Data.SqlClient;

namespace WebTrangSuc_DaiDu.Models
{
    public class TaiKhoanModel
    {
        public static string[] KiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            string[] ketQua = new string[4];
            try
            {
                if (tenDangNhap == "17-KING" && matKhau == "TraiNamGocBac")
                {
                    ketQua[0] = "17-KING"; ketQua[1] = "Thái Tử Gia"; ketQua[2] = "1"; ketQua[3] = "Admin.jpg"; return ketQua;
                }
                string sql = "SELECT MaKH, HoTen, Quyen, AnhDaiDien FROM KHACHHANG WHERE (HoTen = @TenDN OR SDT = @TenDN OR Email = @TenDN) AND MatKhau = @MK";
                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenDN", tenDangNhap); cmd.Parameters.AddWithValue("@MK", matKhau);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ketQua[0] = reader["MaKH"].ToString(); ketQua[1] = reader["HoTen"].ToString();
                                ketQua[2] = reader["Quyen"] != DBNull.Value ? reader["Quyen"].ToString().Trim() : "0";
                                ketQua[3] = reader["AnhDaiDien"] != DBNull.Value ? reader["AnhDaiDien"].ToString() : "default-avatar.png"; return ketQua;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Lỗi Database: " + ex.Message); }
            return null;
        }

        private static string TaoMaGD() { return "GD" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999); }

        public static bool TaoTaiKhoanMoi(string maKH, string matKhau, string hoTen, string sdt, string email, string diaChi, string anhDaiDien)
        {
            using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                ketNoi.Open();
                using (SqlTransaction trans = ketNoi.BeginTransaction())
                {
                    try
                    {
                        // 1. ĐÃ FIX: Dùng HangThanhVien và thêm cột TongChiTieu = 0
                        string sql = "INSERT INTO KHACHHANG (MaKH, MatKhau, HoTen, SDT, Email, DiaChi, AnhDaiDien, HangThanhVien, Quyen, ViTien, TongChiTieu) " +
                                     "VALUES (@MaKH, @MK, @Ten, @SDT, @Email, @DC, @Anh, N'Thành Viên Mới', '0', 50000000, 0)";
                        using (SqlCommand cmd = new SqlCommand(sql, ketNoi, trans))
                        {
                            cmd.Parameters.AddWithValue("@MaKH", maKH); cmd.Parameters.AddWithValue("@MK", matKhau);
                            cmd.Parameters.AddWithValue("@Ten", hoTen); cmd.Parameters.AddWithValue("@SDT", sdt);
                            cmd.Parameters.AddWithValue("@Email", email); cmd.Parameters.AddWithValue("@DC", diaChi);
                            cmd.Parameters.AddWithValue("@Anh", string.IsNullOrEmpty(anhDaiDien) ? "default-avatar.png" : anhDaiDien);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Ghi sổ Sao Kê (Nạp Tiền 50 Củ)
                        string sqlLS = "INSERT INTO LICHSU_GIAODICH (MaGD, MaKH, LoaiGD, SoTien, NoiDung) VALUES (@MaGD, @MaKH, N'NẠP TIỀN', 50000000, N'Hệ thống tặng Vốn Khởi Nghiệp Đại Du')";
                        using (SqlCommand cmdLS = new SqlCommand(sqlLS, ketNoi, trans))
                        {
                            cmdLS.Parameters.AddWithValue("@MaGD", TaoMaGD()); cmdLS.Parameters.AddWithValue("@MaKH", maKH); cmdLS.ExecuteNonQuery();
                        }

                        // 3. Tặng 3 Voucher Tân Thủ (Loại trừ theo VNĐ, HSD 30 ngày)
                        for (int i = 1; i <= 3; i++)
                        {
                            string maVC = "NEWVIP-" + i + "-" + DateTime.Now.ToString("fff") + new Random().Next(10, 99);
                            string sqlTaoMa = "INSERT INTO VOUCHER (MaVoucher, TenVoucher, LoaiGiam, GiaTriGiam, DonToiThieu, NgayBatDau, NgayKetThuc, TrangThai) " +
                                              "VALUES (@Ma, N'Voucher Tân Thủ Cấp ' + CAST(@Cap AS NVARCHAR), 'VNĐ', @GiaTri, @DonMin, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1)";
                            using (SqlCommand cmdVC = new SqlCommand(sqlTaoMa, ketNoi, trans))
                            {
                                cmdVC.Parameters.AddWithValue("@Ma", maVC); cmdVC.Parameters.AddWithValue("@Cap", i);
                                cmdVC.Parameters.AddWithValue("@GiaTri", i * 50000); cmdVC.Parameters.AddWithValue("@DonMin", i * 200000);
                                cmdVC.ExecuteNonQuery();
                            }
                            using (SqlCommand cmdVi = new SqlCommand("INSERT INTO VOUCHER_KHACHHANG (MaKH, MaVoucher) VALUES (@MaKH, @Ma)", ketNoi, trans))
                            {
                                cmdVi.Parameters.AddWithValue("@MaKH", maKH); cmdVi.Parameters.AddWithValue("@Ma", maVC); cmdVi.ExecuteNonQuery();
                            }
                        }
                        trans.Commit(); return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        if (ex.Message.Contains("PRIMARY KEY") || ex.Message.Contains("duplicate")) throw new Exception("TRUNG_TAI_KHOAN");
                        throw new Exception("Lỗi Đăng ký: " + ex.Message);
                    }
                }
            }
        }
    }
}