<%@ Page Title="Sản Phẩm - Trang Sức ĐẠI DU" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="sanpham.aspx.cs" Inherits="WebTrangSuc_DaiDu.sanpham" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* NÚT LÊN ĐẦU TRANG BAY LƠ LỬNG */
        #btnBackToTop {
            display: none; position: fixed; bottom: 30px; right: 30px; z-index: 99;
            font-size: 20px; border: none; outline: none; background: linear-gradient(135deg, #0B1D51, #00FFD1);
            color: white; cursor: pointer; padding: 15px 18px; border-radius: 50%;
            box-shadow: 0 4px 15px rgba(0, 255, 209, 0.4); transition: transform 0.3s, background 0.3s;
        }
        #btnBackToTop:hover { transform: translateY(-5px); background: #00FFD1; color: #0B1D51; }

        /* NÉN ẢNH: TỐI ƯU HIỂN THỊ TRÌNH DUYỆT */
        .img-san-pham-muot {
            width: 100%; height: auto; object-fit: cover;
            image-rendering: -webkit-optimize-contrast; /* Ép Chrome render nét hơn */
            transform: translateZ(0); /* Kích hoạt tăng tốc phần cứng (GPU) */
            transition: transform 0.4s ease;
        }
        .product-card:hover .img-san-pham-muot { transform: scale(1.05) translateZ(0); }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <section class="product-section" style="padding: 50px 20px; max-width: 1200px; margin: auto;">
        <h2 class="section-title" style="text-align:center; margin-bottom: 40px; color: #2c3e50;">TẤT CẢ SẢN PHẨM</h2>
        
        <div class="boloc-filter-bar">
            <span class="sort-label">💰 Khoảng giá:</span>
            <asp:DropDownList ID="ddlKhoangGia" runat="server" AutoPostBack="true" OnSelectedIndexChanged="LocSanPham_Changed" CssClass="bo-loc-dropdown">
                <asp:ListItem Value="TatCa" Text="Tất cả mức giá"></asp:ListItem>
                <asp:ListItem Value="Duoi500" Text="Dưới 500.000đ"></asp:ListItem>
                <asp:ListItem Value="Tu500Den1Trieu" Text="500.000đ - 1.000.000đ"></asp:ListItem>
                <asp:ListItem Value="Tu1TrieuDen5Trieu" Text="1.000.000đ - 5.000.000đ"></asp:ListItem>
                <asp:ListItem Value="Tren5Trieu" Text="Trên 5.000.000đ"></asp:ListItem>
            </asp:DropDownList>
            <span class="sort-label" style="margin-left: 20px;">Sắp xếp theo</span>
            <asp:Button ID="btnPhoBien" runat="server" Text="Phổ Biến" CssClass="sort-btn active" OnClick="btnSort_Click" CommandArgument="MacDinh" />
            <asp:Button ID="btnMoiNhat" runat="server" Text="Mới Nhất" CssClass="sort-btn" OnClick="btnSort_Click" CommandArgument="MoiNhat" />
            <asp:Button ID="btnBanChay" runat="server" Text="Bán Chạy" CssClass="sort-btn" OnClick="btnSort_Click" CommandArgument="MuaNhieu" />
            <asp:DropDownList ID="ddlSortGia" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortGia_Changed" CssClass="bo-loc-dropdown">
                <asp:ListItem Value="none" Text="Giá"></asp:ListItem>
                <asp:ListItem Value="GiaThapCao" Text="Giá: Thấp đến Cao"></asp:ListItem>
                <asp:ListItem Value="GiaCaoThap" Text="Giá: Cao đến Thấp"></asp:ListItem>
            </asp:DropDownList>
        </div>
        
        <div class="grid-san-pham">
            <asp:Repeater ID="rptSanPham" runat="server" OnItemCommand="rptSanPham_ItemCommand">
                <ItemTemplate>
                    <div class="khung-san-pham product-card" style="position: relative;">
                        <span class="nhan-mac-vip" style='<%# (Eval("NhanMac") == DBNull.Value || string.IsNullOrEmpty(Eval("NhanMac").ToString())) ? "display:none;" : "" %>'>
                            <%# Eval("NhanMac") %>
                        </span>

                        <a href='chitiet.aspx?ma=<%# Eval("MaSP") %>'>
                            <img src='<%# WebTrangSuc_DaiDu.Models.FlashCoreModel.RenderAnhSieuMuot(Eval("HinhAnh")) %>' 
                                 alt='<%# Eval("TenSP") %>' 
                                 loading="lazy" 
                                 class="img-san-pham-muot" />
                        </a>
                        <div class="candy-fix"></div>
                        <h3 class="ten-san-pham"><%# Eval("TenSP") %></h3>
                        <p class="gia-san-pham"><%# Eval("GiaBan", "{0:N0} VNĐ") %></p>  
                        <asp:LinkButton ID="btnThemGio" runat="server" CommandName="ChotDonNgam" CommandArgument='<%# Eval("MaSP") %>' CssClass="btn-them-gio" Text="Thêm vào giỏ"></asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="phan-trang-container">
            <asp:Repeater ID="rptPhanTrang" runat="server">
                <ItemTemplate>
                    <a href='sanpham.aspx?trang=<%# Eval("TrangSo") %>' class='<%# Convert.ToBoolean(Eval("TrangHienTai")) ? "btn-trang active" : "btn-trang" %>'><%# Eval("TrangSo") %></a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
    <!-- Nút bấm giờ sẽ kiêm chức năng 'Về Trang 1' -->
<button type="button" onclick="quayVeTrang1()" id="btnBackToTop" title="Quay về trang đầu tiên">
    <i class="fa-solid fa-arrow-up"></i>
</button>

<script>
    // JS điều khiển hiện/ẩn nút khi cuộn chuột
    window.onscroll = function () { cuonTrang() };
    function cuonTrang() {
        if (document.body.scrollTop > 300 || document.documentElement.scrollTop > 300) {
            document.getElementById("btnBackToTop").style.display = "block";
        } else {
            document.getElementById("btnBackToTop").style.display = "none";
        }
    }

    // Hàm 'Method B' của sếp đây: Bấm là bay về trang 1
    function quayVeTrang1() {
        // Chuyển hướng trình duyệt về trang sanpham.aspx không kèm tham số ?trang
        // Code Behind của sếp sẽ tự động hiểu đây là Trang 1
        window.location.href = 'sanpham.aspx';
    }
</script>
</asp:Content>