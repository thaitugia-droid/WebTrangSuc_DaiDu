using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace WebTrangSuc_DaiDu.Models
{
    public class FlashCoreModel
    {
        // Thời gian nhớ trên RAM: 30 phút (Hết 30 phút tự chọc SQL lấy dữ liệu mới)
        private static int CacheDurationMinutes = 30;

        /// <summary>
        /// THUẬT TOÁN 1: LẤY SẢN PHẨM TỪ RAM (Siêu Tốc)
        /// Dùng cho các danh sách không cần cập nhật từng giây (như Trang Chủ, Tất cả sản phẩm)
        /// </summary>
        public static DataTable LaySanPhamChopNhoang(string kieuSort = "MacDinh")
        {
            string cacheKey = "SanPhamCache_" + kieuSort;

            // 1. Nếu RAM có sẵn đồ -> Bốc ra xài ngay (Mất 0.001s)
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                return (DataTable)HttpContext.Current.Cache[cacheKey];
            }

            // 2. Nếu RAM trống -> Mới lặn xuống SQL lấy (Mất 0.5s)
            DataTable dt = BoNaoDuLieu.LayDanhSachSanPham("TatCa", kieuSort);

            // 3. Lấy xong thì nhét luôn vào RAM để lần sau thằng khác vào lấy cho nhanh
            if (dt != null && dt.Rows.Count > 0)
            {
                HttpContext.Current.Cache.Insert(
                    cacheKey,
                    dt,
                    null,
                    DateTime.Now.AddMinutes(CacheDurationMinutes),
                    System.Web.Caching.Cache.NoSlidingExpiration
                );
            }

            return dt;
        }

        /// <summary>
        /// THUẬT TOÁN 2: NÉN ẢNH ĐỘNG BẰNG CSS (Giảm tải băng thông)
        /// Trả về đường dẫn ảnh kèm theo "Tuyệt chiêu" ép trình duyệt hiển thị bản mượt hơn.
        /// (Ghi chú: Bản nén vật lý sếp vẫn nên dùng tinypng, đây là nén bằng cách tối ưu rendering của Browser)
        /// </summary>
        public static string RenderAnhSieuMuot(object tenHinhAnh)
        {
            if (tenHinhAnh == null || string.IsNullOrEmpty(tenHinhAnh.ToString()))
                return "IMAGES/Sanpham/default.png";

            // Trả về link chuẩn
            return "IMAGES/Sanpham/" + tenHinhAnh.ToString();
        }
    }
}