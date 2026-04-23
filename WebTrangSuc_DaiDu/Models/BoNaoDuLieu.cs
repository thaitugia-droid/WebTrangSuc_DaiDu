using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WebTrangSuc_DaiDu.Models
{
    public class BoNaoDuLieu
    {
        // 1. ĐƯỜNG ỐNG KẾT NỐI DUY NHẤT LÊN SQL SERVER VIP
        public static string LayChuoiKetNoi()
        {
            return ConfigurationManager.ConnectionStrings["TrangSucDB"].ConnectionString;
        }

        // 2. THUẬT TOÁN LỌC & SẮP XẾP SẢN PHẨM TỔNG HỢP
        public static DataTable LayDanhSachSanPham(string khoangGia = "", string sapXep = "MacDinh", int top = 0)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT " + (top > 0 ? $"TOP {top} " : "") + "* FROM SANPHAM WHERE 1=1 ";

            if (khoangGia == "Duoi500") sql += " AND GiaBan < 500000";
            else if (khoangGia == "Tu500Den1Trieu") sql += " AND GiaBan >= 500000 AND GiaBan <= 1000000";
            else if (khoangGia == "Tu1TrieuDen5Trieu") sql += " AND GiaBan >= 1000000 AND GiaBan <= 5000000";
            else if (khoangGia == "Tren5Trieu") sql += " AND GiaBan > 5000000";

            if (sapXep == "MoiNhat") sql += " ORDER BY STT DESC";
            else if (sapXep == "GiaCaoThap") sql += " ORDER BY GiaBan DESC";
            else if (sapXep == "GiaThapCao") sql += " ORDER BY GiaBan ASC";
            else if (sapXep == "MuaNhieu") sql += " ORDER BY SoLuongTon ASC";
            else sql += " ORDER BY STT DESC";

            using (SqlConnection ketNoi = new SqlConnection(LayChuoiKetNoi()))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sql, ketNoi)) { da.Fill(dt); }
            }
            return dt;
        }

        // 3. THUẬT TOÁN THÊM VÀO GIỎ HÀNG 
        public static DataTable XuLyThemVaoGioHang(string maSP, DataTable gioHangHienTai, out string tenSanPhamDaThem)
        {
            string tenSP = ""; double giaBan = 0; string hinhAnh = "";
            tenSanPhamDaThem = "";

            using (SqlConnection ketNoi = new SqlConnection(LayChuoiKetNoi()))
            {
                string sql = "SELECT TenSP, GiaBan, HinhAnh FROM SANPHAM WHERE MaSP = @MaSP";
                using (SqlCommand cmd = new SqlCommand(sql, ketNoi))
                {
                    cmd.Parameters.AddWithValue("@MaSP", maSP);
                    ketNoi.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            tenSP = dr["TenSP"].ToString();
                            giaBan = Convert.ToDouble(dr["GiaBan"]);
                            hinhAnh = dr["HinhAnh"].ToString();
                            tenSanPhamDaThem = tenSP;
                        }
                    }
                }
            }

            if (gioHangHienTai == null)
            {
                gioHangHienTai = new DataTable();
                gioHangHienTai.Columns.Add("MaSP", typeof(string)); gioHangHienTai.Columns.Add("HinhAnh", typeof(string));
                gioHangHienTai.Columns.Add("TenSP", typeof(string)); gioHangHienTai.Columns.Add("GiaBan", typeof(double));
                gioHangHienTai.Columns.Add("SoLuong", typeof(int)); gioHangHienTai.Columns.Add("ThanhTien", typeof(double));
            }

            bool daCo = false;
            foreach (DataRow r in gioHangHienTai.Rows)
            {
                if (r["MaSP"].ToString() == maSP)
                {
                    r["SoLuong"] = (int)r["SoLuong"] + 1;
                    r["ThanhTien"] = (int)r["SoLuong"] * Convert.ToDouble(r["GiaBan"]);
                    daCo = true; break;
                }
            }

            if (!daCo && tenSP != "") gioHangHienTai.Rows.Add(maSP, hinhAnh, tenSP, giaBan, 1, giaBan);
            return gioHangHienTai;
        }
    }
}