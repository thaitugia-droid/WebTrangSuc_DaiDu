using System;
using System.Data;
using System.Data.SqlClient; // 🔥 NÂNG CẤP LÊN SQL SERVER
using System.Web.UI;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class chitiet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string maSP = Request.QueryString["ma"];
                if (string.IsNullOrEmpty(maSP)) maSP = Request.QueryString["id"];

                if (!string.IsNullOrEmpty(maSP))
                {
                    TaiThongTinChiTiet(maSP);

                    // 🔥 GỌI CHUẨN CoreWebManager (Đã nằm chung file với AdminModel)
                    CoreWebManager.LuuLichSuXem(maSP);
                }
                else
                {
                    Response.Write("<h2 style='text-align:center; margin-top:50px;'>Không tìm thấy sản phẩm!</h2>");
                }
            }
        }

        private void TaiThongTinChiTiet(string maSP)
        {
            try
            {
                // 🔥 ĐỔI SANG SQL SERVER VÀ SỬA THAM SỐ
                using (SqlConnection ketNoi = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    string lenhTruyVan = "SELECT * FROM SANPHAM WHERE MaSP = @MaSP";
                    using (SqlCommand cmd = new SqlCommand(lenhTruyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable(); da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                lblMaSP.Text = dt.Rows[0]["MaSP"].ToString();
                                lblTenSP.Text = dt.Rows[0]["TenSP"].ToString();
                                lblGiaBan.Text = Convert.ToDouble(dt.Rows[0]["GiaBan"]).ToString("N0") + " VNĐ";
                                imgSanPham.ImageUrl = "~/IMAGES/Sanpham/" + dt.Rows[0]["HinhAnh"].ToString();
                                lblMoTa.Text = dt.Rows[0]["MoTa"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Response.Write("<script>alert('Lỗi tải chi tiết: " + ex.Message + "');</script>"); }
        }

        protected void btnThemVaoGio_Click(object sender, EventArgs e)
        {
            string tenSP = "";
            Session["GioHang"] = BoNaoDuLieu.XuLyThemVaoGioHang(lblMaSP.Text, (DataTable)Session["GioHang"], out tenSP);

            // 🔥 ĐÃ VÁ CÚ PHÁP CHUỖI ĐỂ VS2013 KHÔNG BÁO LỖI ĐỎ
            string script = "ThongBaoTuCodeBehind('success', 'THÊM THÀNH CÔNG', 'Sản phẩm [" + tenSP.Replace("'", "\\'") + "] đã được thêm vào giỏ hàng.');";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "PopupTC", script, true);
        }
    }
}