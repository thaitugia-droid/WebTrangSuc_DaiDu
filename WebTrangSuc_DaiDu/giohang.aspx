<%@ Page Title="Giỏ Hàng của bạn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="giohang.aspx.cs" Inherits="WebTrangSuc_DaiDu.giohang" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Tinh chỉnh cho Dropdown Voucher ăn theo theme tối */
        select.input-field option { background-color: var(--deep-blue); color: var(--text-light); }
        .cart-glass-box {
            background-color: var(--glass-bg);
            backdrop-filter: blur(15px);
            -webkit-backdrop-filter: blur(15px);
            padding: 30px;
            border-radius: 15px;
            border: 1px solid rgba(0, 255, 209, 0.2);
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.4);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-area" style="padding: 50px 20px;">
        <div class="cart-glass-box" style="max-width: 900px; margin: auto;">
            <h2 class="cart-title" style="text-align: center; color: var(--neon-cyan); font-family: 'Playfair Display', serif; margin-bottom: 30px; text-shadow: 0 0 10px rgba(0,255,209,0.3);">🛒 GIỎ HÀNG CỦA BẠN</h2>

            <asp:GridView ID="gvGioHang" runat="server" AutoGenerateColumns="False" Width="100%" GridLines="None" 
                style="text-align: center; color: var(--text-light);" OnRowCommand="gvGioHang_RowCommand" EmptyDataText="Giỏ hàng của bạn đang trống!">
                <HeaderStyle CssClass="gv-ocean-header" Font-Bold="True" Height="50px" />
                <RowStyle CssClass="gv-ocean-row" Height="70px" />
                <Columns>
                    <asp:BoundField DataField="MaSP" HeaderText="Mã SP" />
                    <asp:ImageField DataImageUrlField="HinhAnh" DataImageUrlFormatString="~/IMAGES/Sanpham/{0}" 
                        HeaderText="Hình Ảnh" ControlStyle-Width="65px" ControlStyle-CssClass="img-san-pham" />
                    <asp:BoundField DataField="TenSP" HeaderText="Tên Sản Phẩm" />
                    <asp:BoundField DataField="GiaBan" HeaderText="Đơn Giá" DataFormatString="{0:N0} đ" />
                    
                    <asp:TemplateField HeaderText="Số Lượng">
                        <ItemTemplate>
                            <div style="display:flex; justify-content:center; align-items:center; gap:10px;">
                                <asp:LinkButton ID="btnTru" runat="server" CommandName="GiamSL" CommandArgument='<%# Eval("MaSP") %>' style="text-decoration:none; background: rgba(255, 74, 74, 0.2); color:#ff4a4a; border: 1px solid #ff4a4a; padding:2px 10px; border-radius:5px; font-weight:bold; transition: 0.3s;">-</asp:LinkButton>
                                <span style="font-weight:bold; font-size:16px; color: var(--neon-cyan);"><%# Eval("SoLuong") %></span>
                                <asp:LinkButton ID="btnCong" runat="server" CommandName="TangSL" CommandArgument='<%# Eval("MaSP") %>' style="text-decoration:none; background: rgba(0, 255, 209, 0.2); color:var(--neon-cyan); border: 1px solid var(--neon-cyan); padding:2px 10px; border-radius:5px; font-weight:bold; transition: 0.3s;">+</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="ThanhTien" HeaderText="Thành Tiền" DataFormatString="{0:N0} đ" ItemStyle-CssClass="txt-neon-cyan" ItemStyle-Font-Bold="true" />
                    
                    <asp:TemplateField HeaderText="Xóa">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnXoa" runat="server" CommandName="XoaSP" CommandArgument='<%# Eval("MaSP") %>' style="color:#ff4a4a; text-decoration:none; font-size:18px;" ToolTip="Xóa khỏi giỏ hàng"><i class="fa-solid fa-trash"></i></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div style="margin-top: 30px; padding: 25px; background: rgba(0, 0, 0, 0.3); border-radius: 12px; border: 1px solid rgba(255, 255, 255, 0.1);">
                <h3 style="color: var(--neon-cyan); margin-bottom: 20px; border-left: 4px solid var(--neon-cyan); padding-left: 10px;">📝 THÔNG TIN GIAO HÀNG</h3>
                <div style="margin-bottom: 15px;">
                    <label style="display: block; margin-bottom: 5px; color: var(--text-gray);">Họ và Tên Khách Hàng (*):</label>
                    <asp:TextBox ID="txtTenKhach" runat="server" CssClass="input-field" placeholder="Nhập tên người nhận..."></asp:TextBox>
                </div>
                <div style="margin-bottom: 15px;">
                    <label style="display: block; margin-bottom: 5px; color: var(--text-gray);">Số Điện Thoại Liên Hệ (*):</label>
                    <asp:TextBox ID="txtSDT" runat="server" CssClass="input-field" placeholder="Nhập số điện thoại..."></asp:TextBox>
                </div>

                <div style="margin-top: 25px; padding: 20px; background: rgba(0, 255, 209, 0.05); border: 1px dashed var(--neon-cyan); border-radius: 8px;">
                    <h4 style="color: var(--neon-cyan); margin-bottom: 15px;"><i class="fa-solid fa-ticket"></i> Kho Voucher Của Bạn</h4>
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <asp:DropDownList ID="ddlVoucher" runat="server" CssClass="input-field" style="flex: 1; margin-bottom: 0; padding: 12px;"></asp:DropDownList>
                        <asp:Button ID="btnApDungVoucher" runat="server" Text="Áp Mã" OnClick="btnApDungVoucher_Click" CssClass="btn-gold" style="width: auto; margin-top: 0; padding: 12px 25px;" />
                    </div>
                    <asp:Label ID="lblLoiVoucher" runat="server" Font-Bold="true" style="display:block; margin-top:10px;"></asp:Label>
                </div>
            </div>

            <div class="cart-summary" style="margin-top: 30px; border-top: 2px dashed rgba(0, 255, 209, 0.3); padding-top: 20px; color: var(--text-light); line-height: 2;">
                <p>Tạm tính: <asp:Label ID="lblTamTinh" runat="server" Text="0 đ" Font-Bold="true"></asp:Label></p>
                <asp:Label ID="lblThongBaoGiamGia" runat="server" ForeColor="#00FFD1" Font-Bold="true" Font-Italic="true" style="display:block; margin-bottom:10px;"></asp:Label>
                
                <p style="color: var(--neon-cyan);">Giảm hệ thống: <asp:Label ID="lblTienGiam" runat="server" Text="0 đ" Font-Bold="true"></asp:Label></p>
                <p style="color: #ff9f43;">Giảm Voucher: <asp:Label ID="lblTienGiamVoucher" runat="server" Text="0 đ" Font-Bold="true"></asp:Label></p>
                
                <div style="margin-top: 15px; font-size: 20px;">
                    <strong>Cần thanh toán: </strong>
                    <asp:Label ID="lblTongTien" runat="server" CssClass="total-price-neon" Text="0 đ"></asp:Label>
                </div>
            </div>

            <div style="margin-top: 40px; display: flex; justify-content: space-between; align-items: center;">
               <a href="sanpham.aspx" class="btn-back-sea">⬅ Tiếp tục mua sắm</a>
               <asp:Button ID="btnThanhToan" runat="server" Text="Thanh Toán Ngay ➡" OnClick="btnThanhToan_Click" CssClass="btn-checkout-neon" />
            </div>
        </div>
    </div>
</asp:Content>