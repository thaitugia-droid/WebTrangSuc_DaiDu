<%@ Page Title="Trang Chủ - Trang Sức ĐẠI DU" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="WebTrangSuc_DaiDu.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .lord-icon-wrapper { display: none; }
        .btn-explore { display: inline-block; transition: all 0.3s; }
        .theme-daiduong .lord-icon-wrapper { display: block; }
        .theme-daiduong .btn-explore { display: none; }
        .theme-daiduong::before { display: none !important; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% string themeName = Application["CurrentTheme"] != null ? Application["CurrentTheme"].ToString() : "banthuong"; %>

    <section class="hero-banner <%= "theme-" + themeName %>">
        <script src="https://cdn.lordicon.com/lordicon.js"></script>
        <div class="lord-icon-wrapper" style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: -1; opacity: 0.15; filter: drop-shadow(0 0 30px #00FFD1); pointer-events: none;">
            <lord-icon src="https://cdn.lordicon.com/ginorzey.json" trigger="loop" colors="primary:#00FFD1,secondary:#00A8E8" style="width:550px;height:550px"></lord-icon>
        </div>

        <div class="banner-container">
            <div class="banner-content">
                <h1>Tôn vinh vẻ đẹp đích thực</h1>
                <p>Bộ sưu tập trang sức tinh tế dành riêng cho giới trẻ</p>
                <br />
                <a href="SanPham.aspx" class="btn-explore">Khám phá ngay</a>
                <% if (themeName != "banthuong" && themeName != "daiduong") { %>
                    <br /><img class="wreath-fix" src='<%= ResolveUrl("~/IMAGES/Theme/" + themeName + "/icon-trangtri.png") %>' />
                <% } %>
            </div>
        </div>
    </section>

    <section class="product-section" style="padding: 50px 20px; max-width: 1200px; margin: auto;">
        <h2 class="section-title" style="text-align:center; margin-bottom: 20px; color: #2c3e50;">💎 GỢI Ý DÀNH RIÊNG CHO BẠN 💎</h2>
        <div class="bo-loc-container" style="justify-content: flex-end; margin-bottom: 30px; background: transparent; border: none; box-shadow: none;">
            <span class="bo-loc-label">Hiển thị: </span>
            <asp:DropDownList ID="ddlBoLoc" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBoLoc_SelectedIndexChanged" CssClass="bo-loc-dropdown">
                <asp:ListItem Value="MacDinh" Text="Mặc định"></asp:ListItem>
                <asp:ListItem Value="MuaNhieu" Text="Mua nhiều nhất"></asp:ListItem>
                <asp:ListItem Value="MoiNhat" Text="Hàng mới về"></asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="grid-san-pham">
            <asp:Repeater ID="rptSanPhamMoi" runat="server" OnItemCommand="rptSanPhamMoi_ItemCommand">
                <ItemTemplate>
                    <div class="khung-san-pham product-card" style="position: relative;">
                        
                        <span class="nhan-mac-vip" style='<%# (Eval("NhanMac") == DBNull.Value || string.IsNullOrEmpty(Eval("NhanMac").ToString())) ? "display:none;" : "" %>'>
                            <%# Eval("NhanMac") %>
                        </span>

                        <a href='chitiet.aspx?ma=<%# Eval("MaSP") %>'><img src='IMAGES/Sanpham/<%# Eval("HinhAnh") %>' alt='<%# Eval("TenSP") %>' class="img-san-pham" /></a>
                        <div class="candy-fix"></div>
                        <h3 class="ten-san-pham"><%# Eval("TenSP") %></h3>
                        <p class="gia-san-pham"><%# Eval("GiaBan", "{0:N0} VNĐ") %></p>  
                        <asp:LinkButton ID="btnThemGio" runat="server" CommandName="ChotDonNgam" CommandArgument='<%# Eval("MaSP") %>' CssClass="btn-them-gio" Text="Thêm vào giỏ"></asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>
</asp:Content>