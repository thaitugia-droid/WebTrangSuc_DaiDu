using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Threading.Tasks;
using System.Web;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;

            // Kiểm tra quyền Admin, nếu là khách lậu thì đá văng ra ngay
            ThongBaoModel.KiemTraQuyenAdmin(this);

            if (!IsPostBack)
            {
                TaiDuLieuLenBang();
                TaiBangDanhMucChoAdmin();
                TaiDanhSachDonHang();
                TaiThongKeDoanhThu();
                TaiDanhSachTaiKhoan();
            }
        }

        #region ======================= TRUY VẤN ĐƠN HÀNG THẦN TỐC =======================
        protected void btnTimDonHang_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimDonHang.Text.Trim().ToUpper();
          
            DataTable dt = AdminModel.LayDanhSachDonHang();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                DataView dv = dt.DefaultView;
                // Lọc theo Mã ĐH hoặc Số Điện Thoại
                dv.RowFilter = string.Format("MaHD LIKE '%{0}%' OR SDT LIKE '%{0}%'", tuKhoa);

                // BỘ NÃO LỌC TOP: Tạo cột ảo "UuTien"
                DataTable dtSorted = dv.ToTable();
                if (!dtSorted.Columns.Contains("UuTien"))
                {
                    dtSorted.Columns.Add("UuTien", typeof(int));
                }

                foreach (DataRow row in dtSorted.Rows)
                {
                    // Nếu gõ trúng đầu mã (VD: gõ DH23, mã là DH2304) -> Ưu tiên đưa lên top 1 (Gán giá trị = 0)
                    if (row["MaHD"].ToString().ToUpper().StartsWith(tuKhoa) || row["SDT"].ToString().StartsWith(tuKhoa))
                        row["UuTien"] = 0;
                    else
                        row["UuTien"] = 1;
                }

                // Sắp xếp: Ưu tiên số 0 lên trước, sau đó mới tính đến Ngày đặt mới nhất
                DataView dvFinal = dtSorted.DefaultView;
                dvFinal.Sort = "UuTien ASC, NgayDat DESC";

                gvDonHang.DataSource = dvFinal.ToTable();
            }
            else
            {
                gvDonHang.DataSource = dt;
            }

            gvDonHang.DataBind();
        }

        protected void btnLamMoiDon_Click(object sender, EventArgs e)
        {
            txtTimDonHang.Text = "";
            TaiDanhSachDonHang();
        }
        #endregion

        #region ======================= RADAR QUÉT ĐƠN HÀNG REAL-TIME =======================
        [WebMethod(EnableSession = true)]
        public static string KiemTraDonHangMoi()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM HOADON WHERE TrangThai = N'Chờ duyệt'";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        int currentCount = (int)cmd.ExecuteScalar();
                        int lastCount = HttpContext.Current.Session["LastOrderCount"] != null ? (int)HttpContext.Current.Session["LastOrderCount"] : 0;

                        if (currentCount > lastCount)
                        {
                            HttpContext.Current.Session["LastOrderCount"] = currentCount;
                            return (currentCount - lastCount).ToString();
                        }
                        else if (currentCount < lastCount)
                        {
                            HttpContext.Current.Session["LastOrderCount"] = currentCount;
                        }
                    }
                }
                return "0";
            }
            catch
            {
                return "0";
            }
        }
        #endregion

        #region ======================= QUẢN LÝ THEME LỄ HỘI =======================
        protected void btnThemeThuong_Click(object sender, EventArgs e)
        {
            Application["CurrentTheme"] = "banthuong";
            ThongBaoModel.BanPopup(this, "success", "Giao Diện", "NGÀY THƯỜNG");
        }

        protected void btnThemeGiangSinh_Click(object sender, EventArgs e)
        {
            Application["CurrentTheme"] = "giangsinh";
            ThongBaoModel.BanPopup(this, "success", "Giao Diện", "LỄ GIÁNG SINH");
        }

        protected void btnTheme304_Click(object sender, EventArgs e)
        {
            Application["CurrentTheme"] = "le304";
            ThongBaoModel.BanPopup(this, "success", "Giao Diện", "LỄ 30/4");
        }
        #endregion

        #region ======================= QUẢN LÝ SẢN PHẨM =======================
        private void TaiDuLieuLenBang()
        {
            try
            {
                GridView1.DataSource = AdminModel.LayTatCaSanPham();
                GridView1.DataBind();
            }
            catch { }
        }

        protected void btnThemSP_Click(object sender, EventArgs e)
        {
            string tenFileAnh = "chua-co-anh.jpg";

            if (fileUpHinhAnh.HasFile)
            {
                tenFileAnh = fileUpHinhAnh.FileName;
                fileUpHinhAnh.SaveAs(Server.MapPath("~/IMAGES/Sanpham/") + tenFileAnh);
            }

            try
            {
                AdminModel.ThemSanPham(txtMaSP.Text.Trim(), txtTenSP.Text.Trim(), txtMaDM.Text.Trim(), txtGiaBan.Text.Trim(), txtSoLuong.Text.Trim(), tenFileAnh, txtMoTa.Text.Trim());
                TaiDuLieuLenBang();
                ThongBaoModel.BanPopup(this, "success", "Xong!", "Thêm SP thành công");
            }
            catch
            {
                ThongBaoModel.BanPopup(this, "error", "Lỗi", "Trùng Mã SP");
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            TaiDuLieuLenBang();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            TaiDuLieuLenBang();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string maSP = GridView1.DataKeys[e.RowIndex].Value.ToString();
            string tenMoi = ((TextBox)GridView1.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
            string giaMoi = ((TextBox)GridView1.Rows[e.RowIndex].Cells[4].Controls[0]).Text;

            FileUpload fileUpMoi = (FileUpload)GridView1.Rows[e.RowIndex].Cells[6].FindControl("fileUpHinhAnhEdit");
            string tenFileAnhFinal = ((Label)GridView1.Rows[e.RowIndex].Cells[6].FindControl("lblHinhAnhCu")).Text;

            if (fileUpMoi.HasFile)
            {
                tenFileAnhFinal = fileUpMoi.FileName;
                fileUpMoi.SaveAs(Server.MapPath("~/IMAGES/Sanpham/") + tenFileAnhFinal);
            }

            AdminModel.SuaSanPham(maSP, tenMoi, giaMoi, tenFileAnhFinal);
            GridView1.EditIndex = -1;
            TaiDuLieuLenBang();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            AdminModel.XoaSanPham(GridView1.DataKeys[e.RowIndex].Value.ToString());
            TaiDuLieuLenBang();
        }
        #endregion

        #region ======================= QUẢN LÝ DANH MỤC =======================
        private void TaiBangDanhMucChoAdmin()
        {
            gvDanhMucAdmin.DataSource = AdminModel.LayBangDanhMuc();
            gvDanhMucAdmin.DataBind();
        }

        protected void btnThemDM_Click(object sender, EventArgs e)
        {
            AdminModel.ThemDanhMuc(txtThemMaDM.Text.Trim(), txtThemTenDM.Text.Trim());
            TaiBangDanhMucChoAdmin();
        }

        protected void gvDanhMucAdmin_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDanhMucAdmin.EditIndex = e.NewEditIndex;
            TaiBangDanhMucChoAdmin();
        }

        protected void gvDanhMucAdmin_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDanhMucAdmin.EditIndex = -1;
            TaiBangDanhMucChoAdmin();
        }

        protected void gvDanhMucAdmin_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string maDM = gvDanhMucAdmin.DataKeys[e.RowIndex].Value.ToString();
            string maMoi = ((TextBox)gvDanhMucAdmin.Rows[e.RowIndex].Cells[0].Controls[0]).Text;
            string tenMoi = ((TextBox)gvDanhMucAdmin.Rows[e.RowIndex].Cells[1].Controls[0]).Text;

            AdminModel.SuaDanhMuc(maDM, maMoi, tenMoi);
            gvDanhMucAdmin.EditIndex = -1;
            TaiBangDanhMucChoAdmin();
        }

        protected void gvDanhMucAdmin_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            AdminModel.XoaDanhMuc(gvDanhMucAdmin.DataKeys[e.RowIndex].Value.ToString());
            TaiBangDanhMucChoAdmin();
        }
        #endregion

        #region ======================= QUẢN LÝ ĐƠN HÀNG =======================
        private void TaiDanhSachDonHang()
        {
            gvDonHang.DataSource = AdminModel.LayDanhSachDonHang();
            gvDonHang.DataBind();
        }

        protected void gvDonHang_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDonHang.EditIndex = e.NewEditIndex;
            TaiDanhSachDonHang();
        }

        protected void gvDonHang_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDonHang.EditIndex = -1;
            TaiDanhSachDonHang();
        }

        protected void gvDonHang_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string maHD = gvDonHang.DataKeys[e.RowIndex].Value.ToString();
            string trangThai = ((DropDownList)gvDonHang.Rows[e.RowIndex].FindControl("ddlTrangThai")).SelectedValue;

            AdminModel.DuyetDonHang(maHD, trangThai);
            gvDonHang.EditIndex = -1;
            TaiDanhSachDonHang();
        }

        protected void gvHoaDon_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "XemChiTiet")
            {
                gvChiTietHoaDon.DataSource = AdminModel.LayChiTietHoaDon(e.CommandArgument.ToString());
                gvChiTietHoaDon.DataBind();
                pnlChiTiet.Visible = true;
            }
        }
        #endregion

        #region ======================= THỐNG KÊ & TÀI KHOẢN =======================
        private void TaiThongKeDoanhThu()
        {
            string td;
            double tdt;

            AdminModel.LayThongKeTong(out td, out tdt);
            lblTongDon.Text = td;
            lblTongDoanhThu.Text = tdt.ToString("N0") + " VNĐ";

            gvThongKeThang.DataSource = AdminModel.LayThongKeThang();
            gvThongKeThang.DataBind();
        }

        private void TaiDanhSachTaiKhoan()
        {
            gvTaiKhoan.DataSource = AdminModel.LayDanhSachTaiKhoan();
            gvTaiKhoan.DataBind();
        }

        protected void gvTaiKhoan_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvTaiKhoan.EditIndex = e.NewEditIndex;
            TaiDanhSachTaiKhoan();
        }

        protected void gvTaiKhoan_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvTaiKhoan.EditIndex = -1;
            TaiDanhSachTaiKhoan();
        }

        protected void gvTaiKhoan_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string maKH = gvTaiKhoan.DataKeys[e.RowIndex].Value.ToString();
            string quyen = ((DropDownList)gvTaiKhoan.Rows[e.RowIndex].FindControl("ddlQuyen")).SelectedValue;

            AdminModel.CapNhatQuyen(maKH, quyen);
            gvTaiKhoan.EditIndex = -1;
            TaiDanhSachTaiKhoan();
        }

        protected void gvTaiKhoan_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            AdminModel.XoaTaiKhoan(gvTaiKhoan.DataKeys[e.RowIndex].Value.ToString());
            TaiDanhSachTaiKhoan();
        }

        protected void btnDangXuat_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("DangNhap.aspx");
        }
        #endregion

        #region ======================= TRÍ TUỆ NHÂN TẠO (AI) =======================
        [WebMethod]
        public static string AISoiAnh(string base64Image)
        {
            return Task.Run(() => TroLyAI.SoiAnhTrangSuc(base64Image)).GetAwaiter().GetResult();
        }

        [WebMethod]
        public static string AIPhanTichChienLuoc(string duLieuDoanhThu)
        {
            return TroLyAI.PhanTichDoanhThu(duLieuDoanhThu);
        }

        [WebMethod]
        public static string ApDungKhuyenMaiToanHeThong(int phanTram)
        {
            System.Web.HttpContext.Current.Application["MucGiamGiaAI"] = phanTram;
            return "✅ Lệnh AI đã ban hành!";
        }
        #endregion
    }
}