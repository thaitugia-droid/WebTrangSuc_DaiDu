using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class giohang : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Link tới hệ thống hạng nặng để tối ưu giá trước khi load giỏ
                HeThongNangModel.QuetLaiVaDieuChinhGia();

                LoadKhoVoucher();
                TinhTongHoaDon();
                DongBoGioHangXuongDB();
            }
        }

        private void LoadKhoVoucher()
        {
            ddlVoucher.Items.Clear();
            ddlVoucher.Items.Add(new System.Web.UI.WebControls.ListItem("-- Không dùng mã giảm giá --", "NONE"));

            if (Session["TaiKhoan"] == null) return;
            string maKH = Session["TaiKhoan"].ToString();

            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                string sql = "SELECT V.MaVoucher, V.TenVoucher, V.DonToiThieu, " +
                             "CASE WHEN V.LoaiGiam = '%' THEN CAST(V.GiaTriGiam AS VARCHAR) + '%' " +
                             "ELSE CAST(V.GiaTriGiam / 1000 AS VARCHAR) + 'K' END AS MucGiam " +
                             "FROM VOUCHER V JOIN VOUCHER_KHACHHANG VK ON V.MaVoucher = VK.MaVoucher " +
                             "WHERE VK.MaKH = @MaKH AND VK.DaSuDung = 0 AND V.TrangThai = 1 AND V.NgayKetThuc >= GETDATE()";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string mText = string.Format("[Giảm {0}] {1} (Đơn tối thiểu {2:N0}đ)", r["MucGiam"], r["TenVoucher"], r["DonToiThieu"]);
                            ddlVoucher.Items.Add(new System.Web.UI.WebControls.ListItem(mText, r["MaVoucher"].ToString()));
                        }
                    }
                }
            }
        }

        protected void btnApDungVoucher_Click(object sender, EventArgs e)
        {
            if (Session["TaiKhoan"] == null)
            {
                ThongBaoModel.BanPopup(this, "warning", "Cảnh báo", "Bạn cần đăng nhập để sử dụng mã giảm giá!"); return;
            }

            string maVC = ddlVoucher.SelectedValue;
            if (maVC == "NONE")
            {
                Session.Remove("VoucherDangDung"); Session.Remove("TienDuocGiamTuVoucher");
                lblLoiVoucher.Text = ""; TinhTongHoaDon(); return;
            }

            double tongTienBanDau = TinhTongTienTamTinh();
            if (tongTienBanDau == 0) { lblLoiVoucher.Text = "❌ Giỏ hàng đang trống!"; return; }

            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                string sql = "SELECT LoaiGiam, GiaTriGiam, GiamToiDa, DonToiThieu FROM VOUCHER WHERE MaVoucher = @MaVC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaVC", maVC);
                    conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            double donToiThieu = Convert.ToDouble(r["DonToiThieu"]);
                            if (tongTienBanDau < donToiThieu)
                            {
                                lblLoiVoucher.Text = "❌ Đơn hàng chưa đạt mức tối thiểu " + donToiThieu.ToString("N0") + "đ.";
                                lblLoiVoucher.ForeColor = System.Drawing.Color.Tomato;
                                Session.Remove("VoucherDangDung"); Session.Remove("TienDuocGiamTuVoucher");
                                TinhTongHoaDon(); return;
                            }

                            double tienDuocGiam = 0;
                            if (r["LoaiGiam"].ToString() == "%")
                            {
                                tienDuocGiam = tongTienBanDau * (Convert.ToDouble(r["GiaTriGiam"]) / 100);
                                double giamToiDa = Convert.ToDouble(r["GiamToiDa"]);
                                if (giamToiDa > 0 && tienDuocGiam > giamToiDa) tienDuocGiam = giamToiDa;
                            }
                            else { tienDuocGiam = Convert.ToDouble(r["GiaTriGiam"]); }

                            Session["VoucherDangDung"] = maVC;
                            Session["TienDuocGiamTuVoucher"] = tienDuocGiam;
                            lblLoiVoucher.Text = "✅ Đã áp dụng mã thành công!";
                            lblLoiVoucher.ForeColor = System.Drawing.Color.LimeGreen;

                            TinhTongHoaDon();
                        }
                    }
                }
            }
        }

        private double TinhTongTienTamTinh()
        {
            double tongTien = 0;
            if (Session["GioHang"] != null)
            {
                DataTable dt = (DataTable)Session["GioHang"];
                foreach (DataRow row in dt.Rows) tongTien += Convert.ToDouble(row["ThanhTien"]);
            }
            return tongTien;
        }

        private void TinhTongHoaDon()
        {
            if (Session["GioHang"] != null)
            {
                DataTable dtGioHang = (DataTable)Session["GioHang"];
                if (dtGioHang.Rows.Count > 0)
                {
                    gvGioHang.DataSource = dtGioHang; gvGioHang.DataBind();
                    double tongTienBanDau = TinhTongTienTamTinh();
                    lblTamTinh.Text = string.Format("{0:N0} đ", tongTienBanDau);

                    int tongSoLuong = 0;
                    foreach (DataRow r in dtGioHang.Rows) tongSoLuong += Convert.ToInt32(r["SoLuong"]);

                    double phanTramGiam = (tongSoLuong >= 3) ? Math.Min(tongSoLuong * 0.015, 0.25) : 0;
                    double tienGiamHeThong = tongTienBanDau * phanTramGiam;
                    double tienGiamVoucher = Session["TienDuocGiamTuVoucher"] != null ? Convert.ToDouble(Session["TienDuocGiamTuVoucher"]) : 0;

                    double tongTienPhaiTra = tongTienBanDau - tienGiamHeThong - tienGiamVoucher;
                    if (tongTienPhaiTra < 0) tongTienPhaiTra = 0;

                    lblTienGiam.Text = string.Format("- {0:N0} đ", tienGiamHeThong);
                    lblTienGiamVoucher.Text = string.Format("- {0:N0} đ", tienGiamVoucher);
                    lblTongTien.Text = string.Format("{0:N0} đ", tongTienPhaiTra);

                    Session["TienCanThanhToan"] = tongTienPhaiTra;
                }
                else XoaTrangGioHang();
            }
            else XoaTrangGioHang();
        }

        private void XoaTrangGioHang()
        {
            lblTamTinh.Text = "0 đ"; lblTienGiam.Text = "0 đ"; lblTienGiamVoucher.Text = "0 đ"; lblTongTien.Text = "0 đ";
            Session["TienCanThanhToan"] = 0;
            gvGioHang.DataSource = null; gvGioHang.DataBind();
        }

        protected void gvGioHang_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (Session["GioHang"] == null) return;
            DataTable dt = (DataTable)Session["GioHang"];
            string maSP = e.CommandArgument.ToString();

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["MaSP"].ToString() == maSP)
                {
                    int slCu = Convert.ToInt32(dt.Rows[i]["SoLuong"]);
                    double gia = Convert.ToDouble(dt.Rows[i]["GiaBan"]);

                    if (e.CommandName == "TangSL") { dt.Rows[i]["SoLuong"] = slCu + 1; dt.Rows[i]["ThanhTien"] = (slCu + 1) * gia; }
                    else if (e.CommandName == "GiamSL")
                    {
                        if (slCu > 1) { dt.Rows[i]["SoLuong"] = slCu - 1; dt.Rows[i]["ThanhTien"] = (slCu - 1) * gia; }
                        else dt.Rows.RemoveAt(i);
                    }
                    else if (e.CommandName == "XoaSP") { dt.Rows.RemoveAt(i); }
                    break;
                }
            }
            Session["GioHang"] = dt;
            Session.Remove("VoucherDangDung"); Session.Remove("TienDuocGiamTuVoucher"); lblLoiVoucher.Text = ""; ddlVoucher.SelectedIndex = 0;

            TinhTongHoaDon();
            DongBoGioHangXuongDB();
        }

        private void DongBoGioHangXuongDB()
        {
            if (Session["TaiKhoan"] == null) return;
            string maKH = Session["TaiKhoan"].ToString();

            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                conn.Open();
                new SqlCommand("DELETE FROM GIOHANG_TAM WHERE MaKH = '" + maKH + "'", conn).ExecuteNonQuery();

                if (Session["GioHang"] != null)
                {
                    DataTable dt = (DataTable)Session["GioHang"];
                    if (dt.Rows.Count > 0)
                    {
                        string maGH = "GH" + maKH + DateTime.Now.ToString("HHmmss");
                        new SqlCommand("INSERT INTO GIOHANG_TAM(MaGioHang, MaKH) VALUES('" + maGH + "', '" + maKH + "')", conn).ExecuteNonQuery();

                        foreach (DataRow r in dt.Rows)
                        {
                            string sqlCT = string.Format("INSERT INTO CHITIET_GIOHANG(MaGioHang, MaSP, SoLuong) VALUES('{0}', '{1}', {2})", maGH, r["MaSP"], r["SoLuong"]);
                            new SqlCommand(sqlCT, conn).ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        protected void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (Session["GioHang"] == null || ((DataTable)Session["GioHang"]).Rows.Count == 0)
            {
                ThongBaoModel.BanPopup(this, "error", "Lỗi", "Giỏ hàng trống!"); return;
            }

            string tenKhach = txtTenKhach.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            if (string.IsNullOrEmpty(tenKhach) || string.IsNullOrEmpty(sdt))
            {
                ThongBaoModel.BanPopup(this, "warning", "Thiếu thông tin", "Vui lòng nhập đầy đủ Tên và Số điện thoại nhận hàng."); return;
            }

            DataTable dtGioHang = (DataTable)Session["GioHang"];
            double tongTien = Convert.ToDouble(Session["TienCanThanhToan"]);
            string taiKhoanKh = Session["TaiKhoan"] != null ? Session["TaiKhoan"].ToString() : "KhachLe";
            string maVoucher = Session["VoucherDangDung"] != null ? Session["VoucherDangDung"].ToString() : "";

            try
            {
                string ketQuaThanhToan = GioHangModel.ThanhToanDonHang(tenKhach, sdt, dtGioHang, tongTien, taiKhoanKh, maVoucher);

                Session["GioHang"] = null;
                Session.Remove("VoucherDangDung"); Session.Remove("TienDuocGiamTuVoucher");

                // Bắt mạch Gacha: Tách mã HĐ và mã quà tặng
                string[] tachKetQua = ketQuaThanhToan.Split('|');
                string maHD_DaLuu = tachKetQua[0];
                string maQuaTang = tachKetQua.Length > 1 ? tachKetQua[1] : "";

                // Soạn lời chúc mừng
                string noiDungTC = "Mã đơn hàng: [" + maHD_DaLuu + "]. Hệ thống đã chốt và trừ tiền Ví Đại Du thành công!";
                if (!string.IsNullOrEmpty(maQuaTang))
                {
                    noiDungTC += " Sếp được tặng 1 Voucher [" + maQuaTang + "] vào ví. Hãy kiểm tra ngay!";
                }

                string scriptTC = "ThongBaoTuCodeBehind('success', 'CHỐT ĐƠN TING TING', '" + noiDungTC + "'); setTimeout(function() { window.location='TrangCaNhan.aspx'; }, 4000);";
                ScriptManager.RegisterStartupScript(this, GetType(), "PopupTC", scriptTC, true);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("VI_KHONG_DU_TIEN"))
                    ThongBaoModel.BanPopup(this, "error", "Số dư không đủ", "Ví Đại Du của bạn đã cạn tiền, vui lòng nạp thêm!");
                else
                    ThongBaoModel.BanPopup(this, "error", "Lỗi Thanh Toán", ex.Message);
            }
        }
    }
}