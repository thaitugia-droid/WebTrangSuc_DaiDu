using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTrangSuc_DaiDu.Models; // Lấy cái BoNaoDuLieu để kết nối SQL

namespace WebTrangSuc_DaiDu
{
    public partial class sanpham : System.Web.UI.Page
    {
        // Nhớ bộ lọc hiện tại của khách
        private string CurrentSort
        {
            get { return ViewState["CurrentSort"] != null ? ViewState["CurrentSort"].ToString() : "MacDinh"; }
            set { ViewState["CurrentSort"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true; // Giữ nguyên vị trí cuộn khi load lại
            if (!IsPostBack)
            {
                CurrentSort = "MacDinh";
                TaiDuLieuVoiBoLoc();
            }
        }

        protected void btnSort_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            CurrentSort = btn.CommandArgument;
            ddlSortGia.SelectedIndex = 0; // Reset dropdown giá
            TaiDuLieuVoiBoLoc();
            CapNhatMauNut();
        }

        protected void ddlSortGia_Changed(object sender, EventArgs e)
        {
            if (ddlSortGia.SelectedValue != "none")
            {
                CurrentSort = ddlSortGia.SelectedValue;
                TaiDuLieuVoiBoLoc();
                CapNhatMauNut();
            }
        }

        protected void LocSanPham_Changed(object sender, EventArgs e)
        {
            TaiDuLieuVoiBoLoc();
        }

        // Đổi màu nút khi khách bấm
        private void CapNhatMauNut()
        {
            btnPhoBien.CssClass = "sort-btn"; btnMoiNhat.CssClass = "sort-btn"; btnBanChay.CssClass = "sort-btn";
            if (CurrentSort == "MacDinh") btnPhoBien.CssClass = "sort-btn active";
            else if (CurrentSort == "MoiNhat") btnMoiNhat.CssClass = "sort-btn active";
            else if (CurrentSort == "MuaNhieu") btnBanChay.CssClass = "sort-btn active";
        }

        // BỘ NÃO LỌC VÀ LẤY DỮ LIỆU TỪ SQL (Độc Lập 100%)
        private void TaiDuLieuVoiBoLoc()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    string sql = "SELECT * FROM SANPHAM WHERE 1=1 ";

                    // Xử lý bộ lọc khoảng giá
                    if (ddlKhoangGia.SelectedValue == "Duoi500") sql += "AND GiaBan < 500000 ";
                    else if (ddlKhoangGia.SelectedValue == "Tu500Den1Trieu") sql += "AND GiaBan >= 500000 AND GiaBan <= 1000000 ";
                    else if (ddlKhoangGia.SelectedValue == "Tu1TrieuDen5Trieu") sql += "AND GiaBan > 1000000 AND GiaBan <= 5000000 ";
                    else if (ddlKhoangGia.SelectedValue == "Tren5Trieu") sql += "AND GiaBan > 5000000 ";

                    // Xử lý Sắp xếp
                    if (CurrentSort == "MoiNhat") sql += "ORDER BY STT DESC";
                    else if (CurrentSort == "MuaNhieu") sql += "ORDER BY LuotBan DESC";
                    else if (CurrentSort == "GiaThapCao") sql += "ORDER BY GiaBan ASC";
                    else if (CurrentSort == "GiaCaoThap") sql += "ORDER BY GiaBan DESC";
                    else sql += "ORDER BY STT ASC"; // MacDinh

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                // Cấu hình Phân Trang (Mỗi trang 12 sản phẩm)
                PagedDataSource pds = new PagedDataSource();
                pds.DataSource = dt.DefaultView;
                pds.AllowPaging = true;
                pds.PageSize = 12;

                int trangHienTai = 1;
                if (Request.QueryString["trang"] != null) int.TryParse(Request.QueryString["trang"], out trangHienTai);

                // Chống lỗi nhập số trang tào lao trên URL
                if (trangHienTai < 1) trangHienTai = 1;
                if (pds.PageCount > 0 && trangHienTai > pds.PageCount) trangHienTai = pds.PageCount;

                pds.CurrentPageIndex = trangHienTai - 1;

                rptSanPham.DataSource = pds;
                rptSanPham.DataBind();

                // Tạo danh sách nút bấm trang 1, 2, 3...
                DataTable dtTrang = new DataTable();
                dtTrang.Columns.Add("TrangSo");
                dtTrang.Columns.Add("TrangHienTai", typeof(bool));
                for (int i = 1; i <= pds.PageCount; i++)
                {
                    DataRow dr = dtTrang.NewRow();
                    dr["TrangSo"] = i;
                    dr["TrangHienTai"] = (i == trangHienTai);
                    dtTrang.Rows.Add(dr);
                }
                rptPhanTrang.DataSource = dtTrang;
                rptPhanTrang.DataBind();
            }
            catch (Exception ex)
            {
                string errorScript = "ThongBaoTuCodeBehind('error', 'Lỗi Database', '" + ex.Message.Replace("'", "\\'") + "');";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "PopupError", errorScript, true);
            }
        }

        // BỘ NÃO THÊM VÀO GIỎ HÀNG CHUẨN XỊN (Không cần hàm ngoài)
        protected void rptSanPham_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChotDonNgam")
            {
                string maSP = e.CommandArgument.ToString();
                string tenSP = "";

                // Khởi tạo giỏ hàng nếu chưa có
                DataTable dtGioHang = Session["GioHang"] as DataTable;
                if (dtGioHang == null)
                {
                    dtGioHang = new DataTable();
                    dtGioHang.Columns.Add("MaSP");
                    dtGioHang.Columns.Add("TenSP");
                    dtGioHang.Columns.Add("HinhAnh");
                    dtGioHang.Columns.Add("GiaBan", typeof(double));
                    dtGioHang.Columns.Add("SoLuong", typeof(int));
                    dtGioHang.Columns.Add("ThanhTien", typeof(double));
                }

                // Lấy thông tin Sản Phẩm chọc thẳng từ Database
                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TenSP, HinhAnh, GiaBan FROM SANPHAM WHERE MaSP = @MaSP", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tenSP = reader["TenSP"].ToString();
                                string hinhAnh = reader["HinhAnh"].ToString();
                                double giaBan = Convert.ToDouble(reader["GiaBan"]);

                                // Kiểm tra xem sản phẩm đã có trong giỏ chưa
                                bool daCo = false;
                                foreach (DataRow row in dtGioHang.Rows)
                                {
                                    if (row["MaSP"].ToString() == maSP)
                                    {
                                        row["SoLuong"] = Convert.ToInt32(row["SoLuong"]) + 1;
                                        row["ThanhTien"] = Convert.ToInt32(row["SoLuong"]) * giaBan;
                                        daCo = true;
                                        break;
                                    }
                                }

                                // Nếu chưa có thì thêm dòng mới
                                if (!daCo)
                                {
                                    DataRow newRow = dtGioHang.NewRow();
                                    newRow["MaSP"] = maSP;
                                    newRow["TenSP"] = tenSP;
                                    newRow["HinhAnh"] = hinhAnh;
                                    newRow["GiaBan"] = giaBan;
                                    newRow["SoLuong"] = 1;
                                    newRow["ThanhTien"] = giaBan;
                                    dtGioHang.Rows.Add(newRow);
                                }
                            }
                        }
                    }
                }

                // Lưu lại giỏ hàng vào Session
                Session["GioHang"] = dtGioHang;

                // Hiện Popup Ting Ting
                string script = "ThongBaoTuCodeBehind('success', 'ĐÃ VÀO GIỎ MƯỢT MÀ', 'Sản phẩm [" + tenSP.Replace("'", "\\'") + "] đã nằm gọn trong giỏ hàng.');";
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "PopupTC", script, true);
            }
        }
    }
}