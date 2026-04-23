using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;
            if (!IsPostBack)
            {
                try
                {
                    // 🔥 KÍCH HOẠT HỆ THỐNG TỰ ĐỘNG CHẠY NGẦM CỦA SÀN LỚN 🔥
                    HeThongNangModel.KiemTraVaBomVoucherTrucTuyen(); // Bơm mã theo giờ vàng
                    HeThongNangModel.DonDepGioHangCu(); // Dọn giỏ hàng rác 7 ngày
                }
                catch { } // Bọc try-catch lỡ DB chưa Update xong cũng ko làm sập web

                TaiSanPham("MacDinh");
            }
        }

        protected void ddlBoLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaiSanPham(ddlBoLoc.SelectedValue);
        }

        private void TaiSanPham(string kieu)
        {
            try
            {
                DataTable dtFull = BoNaoDuLieu.LayDanhSachSanPham("", kieu, (kieu == "MacDinh") ? 0 : 4);
                DataTable dt = dtFull.DefaultView.ToTable();

                if (kieu == "MacDinh" && dt.Rows.Count > 0)
                {
                    DataTable dtRandom = dt.Clone();
                    Random rnd = new Random();
                    int soLuongCanLay = Math.Min(4, dt.Rows.Count);
                    List<int> daLay = new List<int>();

                    while (daLay.Count < soLuongCanLay)
                    {
                        int viTriNgauNhien = rnd.Next(0, dt.Rows.Count);
                        if (!daLay.Contains(viTriNgauNhien))
                        {
                            daLay.Add(viTriNgauNhien);
                            dtRandom.ImportRow(dt.Rows[viTriNgauNhien]);
                        }
                    }
                    rptSanPhamMoi.DataSource = dtRandom;
                }
                else { rptSanPhamMoi.DataSource = dt; }
                rptSanPhamMoi.DataBind();
            }
            catch (Exception ex)
            {
                // 🔥 ĐÃ FIX LỖI CÚ PHÁP CHUỖI CHO VS2013 🔥
                string errorScript = "ThongBaoTuCodeBehind('error', 'Lỗi', '" + ex.Message.Replace("'", "\\'") + "');";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "PopupError", errorScript, true);
            }
        }

        protected void rptSanPhamMoi_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChotDonNgam")
            {
                string tenSP = "";
                Session["GioHang"] = BoNaoDuLieu.XuLyThemVaoGioHang(e.CommandArgument.ToString(), (DataTable)Session["GioHang"], out tenSP);

                // 🔥 ĐÃ FIX LỖI CÚ PHÁP CHUỖI CHO VS2013 🔥
                string script = "ThongBaoTuCodeBehind('success', 'THÊM THÀNH CÔNG', 'Sản phẩm [" + tenSP.Replace("'", "\\'") + "] đã được thêm vào giỏ hàng.');";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "PopupTC", script, true);
            }
        }
    }
}