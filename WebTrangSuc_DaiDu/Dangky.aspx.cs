using System;
using System.Data.SqlClient;
using System.Web.UI;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class Dangky : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnDangKy_Click(object sender, EventArgs e)
        {
            // 1. KIỂM TRA ĐẦU VÀO CƠ BẢN
            if (string.IsNullOrEmpty(txtMatKhau.Text) || string.IsNullOrEmpty(txtHoTen.Text))
            {
                ThongBaoModel.BanPopup(this, "warning", "Thiếu Thông Tin", "Vui lòng nhập mật khẩu và họ tên đầy đủ.");
                return;
            }

            try
            {
                // 2. NHÀ MÁY ĐẺ MÃ KHÁCH HÀNG TỰ ĐỘNG (THEO THỎA THUẬN)
                // Công thức: KH + yyyyMMddHHmmss + 2 số ngẫu nhiên
                string maKH_TuDong = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10, 99);

                // 3. THUẬT TOÁN XỬ LÝ AVATAR
                string tenAnhAvatar = "default-avatar.png";
                if (fuAvatar.HasFile)
                {
                    string ext = System.IO.Path.GetExtension(fuAvatar.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                    {
                        // Dùng mã KH vừa tạo để đặt tên ảnh cho đồng bộ
                        tenAnhAvatar = maKH_TuDong + "_" + DateTime.Now.Ticks.ToString() + ext;
                        string duongDanLuu = Server.MapPath("~/IMAGES/Avatar/" + tenAnhAvatar);
                        fuAvatar.SaveAs(duongDanLuu);
                    }
                    else
                    {
                        ThongBaoModel.BanPopup(this, "warning", "Sai Định Dạng", "Chỉ chấp nhận ảnh JPG, PNG, GIF.");
                        return;
                    }
                }

                // 4. LỆNH SQL SERVER (DÙNG CỘT MaKH)
                string sql = "INSERT INTO KHACHHANG (MaKH, MatKhau, HoTen, SDT, Email, DiaChi, quyen, HangThanhVien, AnhDaiDien) " +
                             "VALUES (@MaKH, @MatKhau, @HoTen, @SDT, @Email, @DiaChi, '0', N'Thành Viên Mới', @Anh)";

                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Nạp tham số vào lệnh SQL
                        cmd.Parameters.AddWithValue("@MaKH", maKH_TuDong);
                        cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text.Trim());
                        cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());
                        cmd.Parameters.AddWithValue("@Anh", tenAnhAvatar);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // 5. THÔNG BÁO THÀNH CÔNG VÀ KHOE MÃ TÀI KHOẢN
                        string noiDungThongBao = "Đăng ký thành công! Mã tài khoản để đăng nhập của bạn là: " + maKH_TuDong;
                        ThongBaoModel.BanPopup(this, "success", "Chào Mừng bạn đến với shop của chúng tôi!", noiDungThongBao);

                        // Cài đồng hồ 3 giây sau tự động nhảy sang trang Đăng Nhập
                        string scriptChuyenTrang = "setTimeout(function() { window.location='DangNhap.aspx'; }, 3500);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "ChuyenTrang", scriptChuyenTrang, true);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu trùng mã hoặc lỗi kết nối
                ThongBaoModel.BanPopup(this, "error", "Lỗi Hệ Thống", "Chi tiết: " + ex.Message.Replace("'", ""));
            }
        }
    }
}