using System;
using System.Data;
using System.Data.SqlClient;

namespace WebTrangSuc_DaiDu.Models
{
    public class GioHangModel
    {
        private static string TaoMaHoaDon() { return "DH" + DateTime.Now.ToString("yyMMddHHmmssffff") + new Random().Next(10, 99).ToString(); }
        private static string TaoMaGD() { return "GD" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999); }

        public static string ThanhToanDonHang(string tenKhach, string sdt, DataTable dtGioHang, double tongTien, string taiKhoanKh, string maVoucher = "")
        {
            string maHD = TaoMaHoaDon();
            string thongBao = maHD;

            // [LINK ĐẾN HỆ THỐNG HẠNG NẶNG] Dọn rác và bơm mã tự động trước khi mở Transaction
            HeThongNangModel.DonDepGioHangCu();
            HeThongNangModel.KiemTraVaBomVoucherTrucTuyen();

            using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                ketNoi.Open();
                using (SqlTransaction tran = ketNoi.BeginTransaction())
                {
                    try
                    {
                        if (taiKhoanKh != "KhachLe")
                        {
                            string sqlCheckVi = "SELECT ISNULL(ViTien, 0) FROM KHACHHANG WHERE MaKH = @TK";
                            using (SqlCommand cmdCheck = new SqlCommand(sqlCheckVi, ketNoi, tran))
                            {
                                cmdCheck.Parameters.AddWithValue("@TK", taiKhoanKh);
                                if (Convert.ToDouble(cmdCheck.ExecuteScalar()) < tongTien) throw new Exception("VI_KHONG_DU_TIEN");
                            }

                            using (SqlCommand cmdTru = new SqlCommand("UPDATE KHACHHANG SET ViTien = ViTien - @Tong, TongChiTieu = ISNULL(TongChiTieu, 0) + @Tong WHERE MaKH = @TK", ketNoi, tran))
                            {
                                cmdTru.Parameters.AddWithValue("@Tong", tongTien); cmdTru.Parameters.AddWithValue("@TK", taiKhoanKh); cmdTru.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdSK = new SqlCommand("INSERT INTO LICHSU_GIAODICH (MaGD, MaKH, LoaiGD, SoTien, NoiDung) VALUES (@MaGD, @TK, N'THANH TOÁN', @Tong, @NoiDung)", ketNoi, tran))
                            {
                                cmdSK.Parameters.AddWithValue("@MaGD", TaoMaGD()); cmdSK.Parameters.AddWithValue("@TK", taiKhoanKh);
                                cmdSK.Parameters.AddWithValue("@Tong", tongTien); cmdSK.Parameters.AddWithValue("@NoiDung", "Thanh toán thành công đơn hàng " + maHD);
                                cmdSK.ExecuteNonQuery();
                            }
                        }

                        using (SqlCommand cmdHD = new SqlCommand("INSERT INTO HOADON (MaHD, MaKH, NgayDat, TongTien, TrangThai, MaVoucher) VALUES (@MaHD, @TK, GETDATE(), @Tong, N'Chờ duyệt', @MaVC)", ketNoi, tran))
                        {
                            cmdHD.Parameters.AddWithValue("@MaHD", maHD); cmdHD.Parameters.AddWithValue("@Tong", tongTien);
                            cmdHD.Parameters.AddWithValue("@TK", taiKhoanKh == "KhachLe" ? (object)DBNull.Value : taiKhoanKh);
                            cmdHD.Parameters.AddWithValue("@MaVC", string.IsNullOrEmpty(maVoucher) ? (object)DBNull.Value : maVoucher);
                            cmdHD.ExecuteNonQuery();
                        }

                        foreach (DataRow row in dtGioHang.Rows)
                        {
                            using (SqlCommand cmdCT = new SqlCommand("INSERT INTO CHITIETHOADON (MaHD, MaSP, SoLuong, DonGia) VALUES (@MaHD, @MaSP, @SL, @Gia)", ketNoi, tran))
                            {
                                cmdCT.Parameters.AddWithValue("@MaHD", maHD); cmdCT.Parameters.AddWithValue("@MaSP", row["MaSP"]);
                                cmdCT.Parameters.AddWithValue("@SL", row["SoLuong"]); cmdCT.Parameters.AddWithValue("@Gia", row["GiaBan"]);
                                cmdCT.ExecuteNonQuery();
                            }
                        }

                        if (!string.IsNullOrEmpty(maVoucher) && taiKhoanKh != "KhachLe")
                        {
                            using (SqlCommand cmdDot = new SqlCommand("UPDATE VOUCHER_KHACHHANG SET DaSuDung = 1 WHERE MaKH = @TK AND MaVoucher = @VC", ketNoi, tran))
                            {
                                cmdDot.Parameters.AddWithValue("@TK", taiKhoanKh); cmdDot.Parameters.AddWithValue("@VC", maVoucher); cmdDot.ExecuteNonQuery();
                            }
                        }

                        // [GACHA] MUA XONG TẶNG QUÀ CHO KHÁCH
                        if (taiKhoanKh != "KhachLe")
                        {
                            string maTang = "";
                            using (SqlCommand cmdBocTham = new SqlCommand("SELECT TOP 1 MaVoucher FROM VOUCHER WHERE SoLuongTon > 0 AND TrangThai = 1 AND NgayKetThuc > GETDATE() AND TenVoucher NOT LIKE N'%Độc Quyền%' ORDER BY NEWID()", ketNoi, tran))
                            {
                                object kq = cmdBocTham.ExecuteScalar();
                                if (kq != null) maTang = kq.ToString();
                            }

                            if (!string.IsNullOrEmpty(maTang))
                            {
                                using (SqlCommand cmdTang = new SqlCommand("INSERT INTO VOUCHER_KHACHHANG (MaKH, MaVoucher) VALUES (@TK, @VC)", ketNoi, tran))
                                {
                                    cmdTang.Parameters.AddWithValue("@TK", taiKhoanKh); cmdTang.Parameters.AddWithValue("@VC", maTang);
                                    try { cmdTang.ExecuteNonQuery(); thongBao += "|" + maTang; } catch { }
                                }
                                using (SqlCommand cmdTruKho = new SqlCommand("UPDATE VOUCHER SET SoLuongTon = SoLuongTon - 1 WHERE MaVoucher = @VC", ketNoi, tran))
                                {
                                    cmdTruKho.Parameters.AddWithValue("@VC", maTang); cmdTruKho.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();
                        return thongBao;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        if (ex.Message == "VI_KHONG_DU_TIEN") throw;
                        throw new Exception("Lỗi giao dịch: " + ex.Message);
                    }
                }
            }
        }
    }
}