using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using WebTrangSuc_DaiDu.Models;

namespace WebTrangSuc_DaiDu
{
    public partial class TrangCaNhan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TaiKhoan"] == null)
            {
                Response.Redirect("DangNhap.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTatCaDuLieu();
            }
        }

        private void LoadTatCaDuLieu()
        {
            string maKH = Session["TaiKhoan"].ToString();
            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                conn.Open();

                // 1. Hồ sơ & Ví
                string sqlHoSo = "SELECT HoTen, SDT, Email, HangThanhVien, ViTien, TongChiTieu, AnhDaiDien, BioID, BioKey FROM KHACHHANG WHERE MaKH = @MaKH";
                using (SqlCommand cmd = new SqlCommand(sqlHoSo, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            string ten = r["HoTen"].ToString();
                            string email = r["Email"].ToString();
                            string bioKey = r["BioKey"].ToString();

                            lblTen.Text = ten;
                            lblSDT.Text = r["SDT"].ToString();
                            lblEmail.Text = string.IsNullOrEmpty(email) ? "Chưa cập nhật" : email;
                            lblHang.Text = r["HangThanhVien"].ToString();

                            lblSoDu.Text = string.Format("{0:N0} đ", r["ViTien"]);
                            double tongChiTieu = r["TongChiTieu"] != DBNull.Value ? Convert.ToDouble(r["TongChiTieu"]) : 0;
                            lblTongChiTieu.Text = string.Format("{0:N0} đ", tongChiTieu);
                            imgAvatar.ImageUrl = "~/IMAGES/Avatar/" + (r["AnhDaiDien"].ToString() != "" ? r["AnhDaiDien"].ToString() : "default-avatar.png");

                            lblVanTay.Text = string.IsNullOrEmpty(bioKey) ? "Chưa thiết lập" : "<span style='color:#2ecc71;'><i class='fa-solid fa-fingerprint'></i> Đã mã hóa bảo mật</span>";

                            txtEditTen.Text = ten;
                            txtEditEmail.Text = email;
                        }
                    }
                }

                // 2. Đơn Mua (Đã sửa thành Repeater Lồng nhau)
                string sqlDonHang = "SELECT TOP 20 MaHD, NgayDat, TongTien, TrangThai FROM HOADON WHERE MaKH = @MaKH ORDER BY NgayDat DESC";
                using (SqlDataAdapter daHD = new SqlDataAdapter(sqlDonHang, conn))
                {
                    daHD.SelectCommand.Parameters.AddWithValue("@MaKH", maKH);
                    DataTable dtHD = new DataTable();
                    daHD.Fill(dtHD);

                    if (dtHD.Rows.Count > 0)
                    {
                        rptDonHang.DataSource = dtHD;
                        rptDonHang.DataBind();
                    }
                    else
                    {
                        lblKhongCoDon.Visible = true;
                    }
                }

                // 3. Sao Kê
                string sqlSaoKe = "SELECT TOP 20 NgayGD, MaGD, NoiDung, LoaiGD, SoTien FROM LICHSU_GIAODICH WHERE MaKH = @MaKH ORDER BY NgayGD DESC";
                using (SqlDataAdapter daSK = new SqlDataAdapter(sqlSaoKe, conn))
                {
                    daSK.SelectCommand.Parameters.AddWithValue("@MaKH", maKH);
                    DataTable dtSK = new DataTable(); daSK.Fill(dtSK);
                    gvSaoKe.DataSource = dtSK; gvSaoKe.DataBind();
                }

                // 4. Kho Voucher
                string sqlVoucher = @"SELECT V.MaVoucher, V.TenVoucher, V.LoaiGiam, V.GiaTriGiam, V.DonToiThieu, V.NgayKetThuc 
                                      FROM VOUCHER V 
                                      INNER JOIN VOUCHER_KHACHHANG VK ON V.MaVoucher = VK.MaVoucher 
                                      WHERE VK.MaKH = @MaKH AND VK.DaSuDung = 0 AND V.NgayKetThuc >= GETDATE()";
                using (SqlDataAdapter daVC = new SqlDataAdapter(sqlVoucher, conn))
                {
                    daVC.SelectCommand.Parameters.AddWithValue("@MaKH", maKH);
                    DataTable dtVC = new DataTable(); daVC.Fill(dtVC);
                    if (dtVC.Rows.Count > 0)
                    {
                        rptVoucher.DataSource = dtVC; rptVoucher.DataBind();
                        lblChuaCoVoucher.Visible = false;
                    }
                    else lblChuaCoVoucher.Visible = true;
                }
            }
        }

        // HÀM MỚI: Bắt sự kiện khi Repeater Mẹ (Đơn hàng) render từng dòng, thì gọi DB để lấy Chi Tiết đắp vào Repeater Con
        protected void rptDonHang_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                string maHD = drv["MaHD"].ToString();

                Repeater rptChiTiet = (Repeater)e.Item.FindControl("rptChiTiet");

                using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
                {
                    string sqlCT = "SELECT SP.TenSP, SP.HinhAnh, CT.SoLuong, CT.DonGia " +
                                   "FROM CHITIETHOADON CT INNER JOIN SANPHAM SP ON CT.MaSP = SP.MaSP " +
                                   "WHERE CT.MaHD = @MaHD";
                    using (SqlDataAdapter daCT = new SqlDataAdapter(sqlCT, conn))
                    {
                        daCT.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);
                        DataTable dtCT = new DataTable();
                        daCT.Fill(dtCT);
                        rptChiTiet.DataSource = dtCT;
                        rptChiTiet.DataBind();
                    }
                }
            }
        }

        // Hàm tiện ích đổi màu Badge trạng thái
        protected string GetTrangThaiClass(string trangThai)
        {
            if (trangThai == "Chờ xác nhận") return "bg-cho";
            if (trangThai == "Đã Hủy") return "bg-huy";
            if (trangThai == "Hoàn Thành") return "bg-xong";
            return "bg-giao";
        }

        protected void btnLuuHoSo_Click(object sender, EventArgs e)
        {
            string maKH = Session["TaiKhoan"].ToString();
            string tenMoi = txtEditTen.Text.Trim();
            string emailMoi = txtEditEmail.Text.Trim();
            string mkMoi = txtEditMatKhau.Text.Trim();
            string bioIdMoi = hdfBioID.Value;
            string bioKeyMoi = hdfBioKey.Value;

            using (SqlConnection conn = new SqlConnection(BoNaoDuLieu.LayChuoiKetNoi()))
            {
                conn.Open();
                string sql = "UPDATE KHACHHANG SET HoTen = @Ten, Email = @Email ";

                if (!string.IsNullOrEmpty(mkMoi)) sql += ", MatKhau = @MK ";
                if (!string.IsNullOrEmpty(bioIdMoi) && !string.IsNullOrEmpty(bioKeyMoi))
                {
                    sql += ", BioID = @BioID, BioKey = @BioKey ";
                }
                sql += "WHERE MaKH = @MaKH";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Ten", tenMoi);
                    cmd.Parameters.AddWithValue("@Email", emailMoi);
                    if (!string.IsNullOrEmpty(mkMoi)) cmd.Parameters.AddWithValue("@MK", mkMoi);
                    if (!string.IsNullOrEmpty(bioIdMoi) && !string.IsNullOrEmpty(bioKeyMoi))
                    {
                        cmd.Parameters.AddWithValue("@BioID", bioIdMoi);
                        cmd.Parameters.AddWithValue("@BioKey", bioKeyMoi);
                    }
                    cmd.Parameters.AddWithValue("@MaKH", maKH);
                    cmd.ExecuteNonQuery();
                }
            }
            Session["HoTen"] = tenMoi;
            Response.Redirect("TrangCaNhan.aspx");
        }

        protected void btnDangXuat_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("index.aspx");
        }
    }
}