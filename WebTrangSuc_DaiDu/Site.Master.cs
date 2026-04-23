using System;
using System.Web.UI;

namespace WebTrangSuc_DaiDu
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentTheme = Application["CurrentTheme"] != null ? Application["CurrentTheme"].ToString() : "banthuong";
            themeCss.Href = "~/CSS/Theme/" + currentTheme + "/tc1.css?v=" + DateTime.Now.Ticks.ToString();

            if (!IsPostBack)
            {
                // 1. Kiểm tra khách hàng đăng nhập
                if (Session["TaiKhoan"] != null)
                {
                    phDaDangNhap.Visible = true;
                    phChuaDangNhap.Visible = false;

                    // Gắn hình Avatar
                    string anh = Session["AnhDaiDien"] != null && Session["AnhDaiDien"].ToString() != "" ? Session["AnhDaiDien"].ToString() : "default-avatar.png";
                    imgAvatar.ImageUrl = "~/IMAGES/Avatar/" + anh;
                }
                else
                {
                    phDaDangNhap.Visible = false;
                    phChuaDangNhap.Visible = true;
                }

                // 2. Kiểm tra quyền Admin
                if (Session["Quyen"] != null && (Session["Quyen"].ToString() == "1" || Session["Quyen"].ToString().ToLower() == "admin"))
                {
                    phMenuAdmin.Visible = true;
                    phGodMode.Visible = true;
                }
                else
                {
                    phMenuAdmin.Visible = false;
                    phGodMode.Visible = false;
                }
            }
        }

        protected void btnDangXuatKhach_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("index.aspx");
        }
    }
}