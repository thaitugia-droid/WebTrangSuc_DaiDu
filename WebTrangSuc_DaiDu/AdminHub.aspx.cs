using System;
using System.Data;
using System.Data.SqlClient; // ĐÃ NÂNG CẤP SQL SERVER
using System.Web.UI;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class AdminHub : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Quyen"] == null || (Session["Quyen"].ToString() != "1" && Session["Quyen"].ToString().ToLower() != "admin"))
            {
                Response.Write("<body style='background:#1a1a1a;color:#ff4757;display:flex;align-items:center;justify-content:center;height:100vh;margin:0;'>");
                Response.Write("<div style='text-align:center;'><h2>⚠️ TỪ CHỐI</h2></div></body>");
                Response.End();
            }

            if (!IsPostBack)
            {
                if (Session["DaNhapPinGodMode"] != null && (bool)Session["DaNhapPinGodMode"] == true)
                    mvAdmin.ActiveViewIndex = 1;
                else
                    mvAdmin.ActiveViewIndex = 0;
            }
        }

        protected void btnSubmitPin_Click(object sender, EventArgs e)
        {
            if (hfPinCode.Value == "5509")
            {
                lblPinError.Text = "";
                Session["DaNhapPinGodMode"] = true;
                mvAdmin.ActiveViewIndex = 1;
            }
            else
            {
                lblPinError.Text = "❌ Mã sai! Thử lại."; hfPinCode.Value = "";
                ScriptManager.RegisterStartupScript(this, GetType(), "Reset", "currentPin = ''; capNhatHienThi();", true);
            }
        }

        protected void btnChayAI_Click(object sender, EventArgs e)
        {
            mvAdmin.ActiveViewIndex = 2;
            string htmlList = "";

            try
            {
                DataTable dt = AdminModel.LayTatCaSanPham();
                lblTongSPAI.Text = dt.Rows.Count.ToString();

                int count = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (count >= 10) break;

                    string tenSP = row["TenSP"].ToString();
                    int tonKho = Convert.ToInt32(row["SoLuongTon"]);
                    int luotXem = new Random(tenSP.GetHashCode()).Next(10, 1000);

                    // GỌI ĐÚNG COREWEBMANAGER ĐÃ TÁCH
                    string deXuat = CoreWebManager.AIPhanTichChienLuoc(tonKho, luotXem);

                    string tenBadge = "TIỀM NĂNG"; string classBadge = "b-tiemnang"; string nutBam = "Áp dụng";
                    if (deXuat.Contains("TỒN ĐỌNG")) { tenBadge = "HÀNG TỒN ĐỌNG"; classBadge = "b-ton"; nutBam = "Mở Flash Sale"; }
                    else if (deXuat.Contains("TÍN HIỆU TỐT")) { tenBadge = "HÀNG CỰC HOT"; classBadge = "b-hot"; nutBam = "Tăng giá bán"; }

                    // DÙNG CỘNG CHUỖI TRUYỀN THỐNG THAY VÌ $ ĐỂ VS2013 KHÔNG BÁO LỖI
                    string jsCommand = "window.parent.Swal.fire({ toast: true, position: 'top-end', icon: 'success', title: 'ÁP DỤNG THÀNH CÔNG', text: 'Đã kích hoạt chiến lược cho " + tenSP.Replace("\"", "'") + "', showConfirmButton: false, timer: 2500 });";

                    htmlList += "<div class='ai-card'>" +
                                "<span class='badge " + classBadge + "'>" + tenBadge + "</span>" +
                                "<h4 style='text-transform: capitalize;'>" + tenSP + "</h4>" +
                                "<div class='ai-stats'>Tồn kho: <b>" + tonKho + "</b> | Lượt xem: <b>" + luotXem + "</b></div>" +
                                "<p style='font-size:11px; color:#555; font-style:italic;'>" + deXuat + "</p>" +
                                "<button type='button' class='btn-apply' onclick=\"" + jsCommand + "\">⚡ " + nutBam + "</button>" +
                                "</div>";
                    count++;
                }
            }
            catch (Exception ex) { htmlList = "<p style='color:red; padding:15px;'>Lỗi kết nối: " + ex.Message + "</p>"; }

            divDanhSachAI.InnerHtml = htmlList;
        }

        protected void btnMoFormThem_Click(object sender, EventArgs e) { mvAdmin.ActiveViewIndex = 3; lblThemTC.Text = ""; }

        protected void btnLuuSPNhanh_Click(object sender, EventArgs e)
        {
            string tenFileAnh = "default.png";
            if (fileAnhSP.HasFile)
            {
                tenFileAnh = DateTime.Now.Ticks.ToString() + "_" + fileAnhSP.FileName;
                fileAnhSP.SaveAs(Server.MapPath("~/IMAGES/Sanpham/" + tenFileAnh));
            }

            AdminModel.ThemSanPham(txtMaSP.Text, txtTenSP.Text, txtMaDM.Text, txtGia.Text, txtTonKho.Text, tenFileAnh, txtMoTa.Text);
            lblThemTC.Text = "✅ Đã lưu thành công vào CSDL!";
            txtMaSP.Text = ""; txtTenSP.Text = ""; txtMaDM.Text = ""; txtGia.Text = ""; txtTonKho.Text = ""; txtMoTa.Text = "";
            ClientScript.RegisterStartupScript(this.GetType(), "Refresh", "setTimeout(function(){ window.parent.location.reload(); }, 1500);", true);
        }

        protected void btnBackToMenu_Click(object sender, EventArgs e) { mvAdmin.ActiveViewIndex = 1; }

        protected void btnMoFormSua_Click(object sender, EventArgs e)
        {
            mvAdmin.ActiveViewIndex = 4; pnEditForm.Visible = false; lblTimKiemErr.Text = ""; txtTimSTT.Text = "";
        }

        // TÌM SẢN PHẨM BẰNG SQL SERVER
        protected void btnTimSP_Click(object sender, EventArgs e)
        {
            string sttStr = txtTimSTT.Text.Trim();
            if (string.IsNullOrEmpty(sttStr)) { lblTimKiemErr.Text = "Nhập STT vào đi sếp!"; return; }

            string sql = "SELECT * FROM SANPHAM WHERE STT = @STT";
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@STT", Convert.ToInt32(sttStr));
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pnEditForm.Visible = true; lblTimKiemErr.Text = ""; hfSuaSTT.Value = reader["STT"].ToString();
                            txtSuaTen.Text = reader["TenSP"].ToString(); txtSuaGia.Text = reader["GiaBan"].ToString();
                            txtSuaTon.Text = reader["SoLuongTon"].ToString(); imgSuaPreview.ImageUrl = "~/IMAGES/Sanpham/" + reader["HinhAnh"].ToString();
                            try { ddlSuaMac.SelectedValue = reader["NhanMac"].ToString(); } catch { ddlSuaMac.SelectedIndex = 0; }
                        }
                        else { pnEditForm.Visible = false; lblTimKiemErr.Text = "❌ Không tìm thấy STT: " + sttStr; }
                    }
                }
            }
        }

        // LƯU CHỈNH SỬA BẰNG SQL SERVER
        protected void btnLuuSuaSP_Click(object sender, EventArgs e)
        {
            string sql = "UPDATE SANPHAM SET GiaBan = @Gia, SoLuongTon = @Ton, NhanMac = @Mac WHERE STT = @STT";
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Gia", txtSuaGia.Text); cmd.Parameters.AddWithValue("@Ton", txtSuaTon.Text);
                    cmd.Parameters.AddWithValue("@Mac", ddlSuaMac.SelectedValue); cmd.Parameters.AddWithValue("@STT", Convert.ToInt32(hfSuaSTT.Value));
                    conn.Open(); cmd.ExecuteNonQuery();
                }
            }

            lblTimKiemErr.Text = "<span style='color:#27ae60;'>✅ Cập nhật & Áp mã thành công!</span>";
            pnEditForm.Visible = false;
            ClientScript.RegisterStartupScript(this.GetType(), "Refresh", "setTimeout(function(){ window.parent.location.reload(); }, 1000);", true);
        }

        protected void ddlTheme_Changed(object sender, EventArgs e)
        {
            Application["CurrentTheme"] = ddlTheme.SelectedValue;
            ScriptManager.RegisterStartupScript(this, GetType(), "DoiThemeLocal", "window.parent.location.reload();", true);
        }
    }
}