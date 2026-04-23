using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class DangNhap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Nếu đã đăng nhập rồi thì không cần đăng nhập lại nữa
            if (!IsPostBack && Session["TaiKhoan"] != null)
            {
                Response.Redirect("index.aspx");
            }
        }

        protected void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tkInput = txtTaiKhoan.Text.Trim();
            string mkInput = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(tkInput) || string.IsNullOrEmpty(mkInput))
            {
                ThongBaoModel.BanPopup(this, "warning", "Thiếu Thông Tin", "Vui lòng nhập tài khoản và mật khẩu!");
                return;
            }

            try
            {
                string[] thongTinUser = null;

                // 🚀 LUỒNG 1: HACK VÂN TAY (DEMO ĐỒ ÁN)
                if (mkInput == "BypassByFingerprint")
                {
                    using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                    {
                        // Truy vấn 4 cột chuẩn theo bảng SQL mới nhất của sếp
                        string sql = "SELECT MaKH, HoTen, Quyen, AnhDaiDien FROM KHACHHANG WHERE HoTen = @TK OR SDT = @TK OR MaKH = @TK";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@TK", tkInput);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongTinUser = new string[] {
                                    reader["MaKH"].ToString(),
                                    reader["HoTen"].ToString(),
                                    reader["Quyen"].ToString(),
                                    reader["AnhDaiDien"] != DBNull.Value ? reader["AnhDaiDien"].ToString() : "default-avatar.png"
                                };
                            }
                        }
                    }
                }
                else
                {
                    // 🚀 LUỒNG 2: ĐĂNG NHẬP BÌNH THƯỜNG (Gọi bộ não TaiKhoanModel)
                    thongTinUser = TaiKhoanModel.KiemTraDangNhap(tkInput, mkInput);
                }

                if (thongTinUser != null)
                {
                    // 🔥 NẠP DỮ LIỆU VÀO RAM SERVER (SESSION) 🔥
                    Session["TaiKhoan"] = thongTinUser[0];    // ID thực (MaKH)
                    Session["HoTen"] = thongTinUser[1];       // Tên hiển thị
                    Session["Quyen"] = thongTinUser[2].Trim(); // Quyền (1 là Admin)
                    Session["AnhDaiDien"] = thongTinUser[3];  // Tên file ảnh (Vd: Admin.jpg)

                    // KIỂM TRA QUYỀN ĐỂ PHÂN LUỒNG REDIRECT
                    if (Session["Quyen"].ToString() == "1" || Session["Quyen"].ToString().ToLower() == "admin")
                    {
                        // ADMIN: Bay thẳng vào trang Quản trị lớn (Admin.aspx)
                        ThongBaoModel.BanPopup(this, "success", "Chào mừng Thái Tử Gia!", "Đã nhận diện quyền SUPREME. Đang vào trang Quản trị...");
                        string scriptAdmin = "setTimeout(function() { window.location='Admin.aspx'; }, 1500);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "GoAdmin", scriptAdmin, true);
                    }
                    else
                    {
                        // KHÁCH: Về trang chủ lướt xem hàng
                        ThongBaoModel.BanPopup(this, "success", "Đăng Nhập Thành Công", "Rất hân hạnh được phục vụ bạn!");
                        string scriptUser = "setTimeout(function() { window.location='index.aspx'; }, 1500);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "GoHome", scriptUser, true);
                    }
                }
                else
                {
                    ThongBaoModel.BanPopup(this, "error", "Thất Bại", "Tài khoản hoặc mật khẩu không đúng!");
                }
            }
            catch (Exception ex)
            {
                ThongBaoModel.BanPopup(this, "error", "Lỗi Kết Nối", "Chi tiết: " + ex.Message.Replace("'", ""));
            }
        }
    }
}