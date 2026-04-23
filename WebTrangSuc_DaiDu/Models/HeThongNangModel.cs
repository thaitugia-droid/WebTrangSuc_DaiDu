using System;
using System.Data.SqlClient;

namespace WebTrangSuc_DaiDu.Models
{
    public class HeThongNangModel
    {
        // ==========================================
        // 1. AI ĐỊNH GIÁ ĐỘNG (DYNAMIC PRICING) & BÁO CÁO
        // ==========================================
        public static string QuetLaiVaDieuChinhGia()
        {
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                conn.Open();
                string sqlDT = "SELECT ISNULL(SUM(TongTien), 0) FROM HOADON WHERE CONVERT(DATE, NgayDat) = CONVERT(DATE, GETDATE()) AND TrangThai != N'Đã hủy'";
                double doanhThu = 0;
                using (SqlCommand cmdDT = new SqlCommand(sqlDT, conn)) { doanhThu = Convert.ToDouble(cmdDT.ExecuteScalar()); }

                double heSo = 1.0;
                string log = $"Doanh thu hôm nay: {doanhThu:N0}đ. ";
                if (doanhThu > 10000000) { heSo = 0.95; log += "Lãi vượt target. Đã tự động GIẢM 5% để đẩy số lượng!"; }
                else if (doanhThu < 2000000) { heSo = 1.02; log += "Lãi thấp. Đã tự động TĂNG nhẹ 2% bù biên độ."; }
                else { log += "Hệ thống giữ nguyên giá bán."; }

                if (heSo != 1.0)
                {
                    string sqlUp = "UPDATE SANPHAM SET GiaBan = GiaBan * @HeSo";
                    using (SqlCommand cmdUp = new SqlCommand(sqlUp, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@HeSo", heSo); cmdUp.ExecuteNonQuery();
                    }
                }
                return log;
            }
        }

        // ==========================================
        // 2. NHÀ MÁY ĐÚC VOUCHER THEO "GIỜ VÀNG" THÔNG MINH
        // ==========================================
        public static void KiemTraVaBomVoucherTrucTuyen()
        {
            int gioHienTai = DateTime.Now.Hour;

            int mucGiam = 50000;
            string tenLoai = "Voucher Phổ Thông";
            int donToiThieu = 300000;
            int soLuongCanDuyTri = 50;

            if (gioHienTai >= 12 && gioHienTai <= 13)
            {
                mucGiam = 200000; tenLoai = "Flash Sale Trưa"; donToiThieu = 800000; soLuongCanDuyTri = 30;
            }
            else if (gioHienTai >= 20 && gioHienTai <= 22)
            {
                mucGiam = 300000; tenLoai = "Đêm Hội Mua Sắm"; donToiThieu = 1000000; soLuongCanDuyTri = 20;
            }
            else if (gioHienTai >= 23 || gioHienTai <= 2)
            {
                mucGiam = 500000; tenLoai = "Voucher Cú Đêm VIP"; donToiThieu = 2000000; soLuongCanDuyTri = 10;
            }

            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                conn.Open();

                // ĐÃ FIX: Thay NgayTao bằng NgayBatDau trong câu lệnh SELECT
                string sqlCheck = "SELECT COUNT(*) FROM VOUCHER WHERE CONVERT(DATE, NgayBatDau) = CONVERT(DATE, GETDATE()) AND TenVoucher = @Ten";
                using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@Ten", tenLoai);
                    int soLuongHienCo = (int)cmdCheck.ExecuteScalar();

                    if (soLuongHienCo < soLuongCanDuyTri)
                    {
                        int soLuongCanBomThem = soLuongCanDuyTri - soLuongHienCo;
                        for (int i = 0; i < soLuongCanBomThem; i++)
                        {
                            string maVC = "DAIDU-" + DateTime.Now.ToString("HHmmssfff") + Guid.NewGuid().ToString().Substring(0, 3).ToUpper();

                            // ĐÃ FIX: Xóa hẳn NgayTao trong câu lệnh INSERT, dùng đúng NgayBatDau và NgayKetThuc
                            string sqlIn = "INSERT INTO VOUCHER (MaVoucher, TenVoucher, GiaTriGiam, DonToiThieu, SoLuongTon, NgayBatDau, NgayKetThuc, LoaiGiam, TrangThai) " +
                                           "VALUES (@Ma, @Ten, @Giam, @DieuKien, 5, GETDATE(), DATEADD(day, 7, GETDATE()), 'VNĐ', 1)";

                            using (SqlCommand cmdIn = new SqlCommand(sqlIn, conn))
                            {
                                cmdIn.Parameters.AddWithValue("@Ma", maVC);
                                cmdIn.Parameters.AddWithValue("@Ten", tenLoai);
                                cmdIn.Parameters.AddWithValue("@Giam", mucGiam);
                                cmdIn.Parameters.AddWithValue("@DieuKien", donToiThieu);
                                cmdIn.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        // ==========================================
        // 3. LAO CÔNG DỌN RÁC GIỎ HÀNG QUA 7 NGÀY
        // ==========================================
        public static void DonDepGioHangCu()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    string sql = "DELETE FROM GIOHANG_TAM WHERE DATEDIFF(day, NgaySua, GETDATE()) > 7";
                    using (SqlCommand cmd = new SqlCommand(sql, conn)) { conn.Open(); cmd.ExecuteNonQuery(); }
                }
            }
            catch { }
        }
    }
}