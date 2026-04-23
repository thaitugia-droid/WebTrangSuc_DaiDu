using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web.UI;

namespace WebTrangSuc_DaiDu.Models
{
    public class ThongBaoModel
    {
        // ==========================================
        // 1. GỌI THÔNG BÁO GIAO DIỆN HIỆN ĐẠI
        // ==========================================
        public static void BanPopup(Page trangHienTai, string loai, string tieuDe, string noiDung)
        {
            string anToanNoiDung = noiDung.Replace("'", "\\'").Replace("\r", "").Replace("\n", " ");
            string script = "ThongBaoTuCodeBehind('" + loai + "', '" + tieuDe + "', '" + anToanNoiDung + "');";
            ScriptManager.RegisterStartupScript(trangHienTai, trangHienTai.GetType(), "ThongBaoPopup", script, true);
        }

        // ==========================================
        // 2. KIỂM TRA QUYỀN TRUY CẬP QUẢN TRỊ VIÊN
        // ==========================================
        public static void KiemTraQuyenAdmin(Page trangHienTai)
        {
            if (trangHienTai.Session["Quyen"] == null || (trangHienTai.Session["Quyen"].ToString() != "1" && trangHienTai.Session["Quyen"].ToString().ToLower() != "admin"))
            {
                trangHienTai.Response.Write("<script>alert('Cảnh báo: Bạn chưa đăng nhập hoặc không có quyền truy cập khu vực này!'); window.location='DangNhap.aspx';</script>");
                trangHienTai.Response.End();
            }
        }

        // ==========================================
        // 3. HỆ THỐNG GỬI EMAIL TỰ ĐỘNG
        // ==========================================
        public static void GuiEmailToiKhach(string emailKhachNhan, string tieuDe, string noiDung)
        {
            if (string.IsNullOrEmpty(emailKhachNhan)) return;

            try
            {
                // Sử dụng thông tin tài khoản Gmail bạn đã cung cấp
                string emailHeThong = "buncamet55@gmail.com";
                string matKhauUngDung = "cpth ygfa hlxi hiku";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailHeThong, "HỆ THỐNG TRANG SỨC ĐẠI DU");
                mail.To.Add(emailKhachNhan);
                mail.Subject = tieuDe;
                mail.Body = noiDung;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(emailHeThong, matKhauUngDung);

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // Ghi lỗi hệ thống, tránh làm gián đoạn trải nghiệm của người dùng
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
            }
        }

        // ==========================================
        // 4. QUAY THƯỞNG MÃ ĐỘC QUYỀN THEO KHUNG GIỜ VÀNG
        // ==========================================
        public static string PhatMaDocQuyenKhungGioVang(string maKH)
        {
            // Khung giờ vàng: 20h đến 22h
            int gioHienTai = DateTime.Now.Hour;

            if (gioHienTai >= 20 && gioHienTai <= 22)
            {
                // Cơ hội nhận mã: 25%
                if (new Random().Next(1, 100) <= 25)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                        {
                            conn.Open();
                            // Chọn 1 mã Độc Quyền ngẫu nhiên
                            string sqlLayMa = "SELECT TOP 1 MaVoucher FROM VOUCHER WHERE TenVoucher LIKE N'%Độc Quyền%' AND SoLuongTon > 0 AND TrangThai = 1 ORDER BY NEWID()";
                            object kq = new SqlCommand(sqlLayMa, conn).ExecuteScalar();

                            if (kq != null)
                            {
                                string maVC = kq.ToString();

                                // Kiểm tra xem người dùng đã sở hữu mã này chưa
                                SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM VOUCHER_KHACHHANG WHERE MaKH = @MaKH AND MaVoucher = @MaVC", conn);
                                cmdCheck.Parameters.AddWithValue("@MaKH", maKH);
                                cmdCheck.Parameters.AddWithValue("@MaVC", maVC);

                                if ((int)cmdCheck.ExecuteScalar() == 0)
                                {
                                    SqlCommand cmdInsert = new SqlCommand("INSERT INTO VOUCHER_KHACHHANG (MaKH, MaVoucher) VALUES (@MaKH, @MaVC)", conn);
                                    cmdInsert.Parameters.AddWithValue("@MaKH", maKH);
                                    cmdInsert.Parameters.AddWithValue("@MaVC", maVC);
                                    cmdInsert.ExecuteNonQuery();

                                    SqlCommand cmdTruKho = new SqlCommand("UPDATE VOUCHER SET SoLuongTon = SoLuongTon - 1 WHERE MaVoucher = @MaVC", conn);
                                    cmdTruKho.Parameters.AddWithValue("@MaVC", maVC);
                                    cmdTruKho.ExecuteNonQuery();

                                    return maVC;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            return "";
        }
    }
}