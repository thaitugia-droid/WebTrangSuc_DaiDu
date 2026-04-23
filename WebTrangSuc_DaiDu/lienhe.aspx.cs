using System;
using System.Web.UI;

namespace WebTrangSuc_DaiDu
{
    public partial class lienhe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // =========================================================
        // 🔥 HÀM XỬ LÝ NÚT GỬI TIN NHẮN (ĐÃ FIX LỖI CHUỖI) 🔥
        // =========================================================
        protected void btnGuiTinNhan_Click(object sender, EventArgs e)
        {
            try
            {
                string tenKhach = txtHoTen.Text.Trim();

                // 🔥 ĐÃ ĐỔI SANG CỘNG CHUỖI TRUYỀN THỐNG ĐỂ HỢP VS2013 🔥
                string script = "Swal.fire({ title: 'ĐÃ GỬI THÀNH CÔNG!', text: 'Cảm ơn " + tenKhach.Replace("'", "\\'") + ". Lời nhắn của bạn đã được gửi đến Đại Du. Chúng tôi sẽ phản hồi sớm nhất!', icon: 'success', background: '#010408', color: '#00FFD1', confirmButtonColor: '#00A8E8' });";
                ScriptManager.RegisterStartupScript(this, GetType(), "ThongBaoLienHe", script, true);

                // Xóa trắng các ô nhập liệu sau khi gửi
                txtHoTen.Text = "";
                txtEmail.Text = "";
                txtTieuDe.Text = "";
                txtNoiDung.Text = "";
            }
            catch (Exception ex)
            {
                // 🔥 ĐÃ FIX LỖI CÚ PHÁP CHUỖI 🔥
                string errScript = "Swal.fire('Lỗi', 'Không thể gửi tin nhắn: " + ex.Message.Replace("'", "\\'") + "', 'error');";
                ScriptManager.RegisterStartupScript(this, GetType(), "ThongBaoLoi", errScript, true);
            }
        }
    }
}