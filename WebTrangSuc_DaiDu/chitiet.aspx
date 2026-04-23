<%@ Page Title="Chi Tiết - Trang Sức ĐẠI DU" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="chitiet.aspx.cs" Inherits="WebTrangSuc_DaiDu.chitiet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-area" style="padding: 40px 20px;">
        
        <div style="max-width: 1000px; margin: 0 auto 20px auto;">
            <a href="sanpham.aspx" style="text-decoration: none; color: var(--gold-primary, #00FFD1); font-weight: bold; transition: 0.3s;">
                <i class="fa-solid fa-arrow-left"></i> Quay lại Cửa hàng
            </a>
        </div>
        
        <div style="max-width: 1000px; margin: 0 auto; display: flex; gap: 40px; background: var(--dark-card, rgba(255,255,255,0.05)); backdrop-filter: blur(12px); padding: 40px; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.5); border: 1px solid rgba(255, 255, 255, 0.1); flex-wrap: wrap;">
            
            <div style="flex: 1; min-width: 300px;">
                <asp:Image ID="imgSanPham" runat="server" style="width: 100%; border-radius: 12px; object-fit: cover; box-shadow: 0 5px 20px rgba(0,0,0,0.6); border: 2px solid var(--gold-primary, #00FFD1);" />
            </div>
            
            <div style="flex: 1.2; min-width: 300px;">
                <h1 style="color: var(--gold-primary, #00FFD1); margin-top: 0; font-family: 'Playfair Display', serif; font-size: 2.5em; text-shadow: 0 0 10px rgba(0,0,0,0.3);">
                    <asp:Label ID="lblTenSP" runat="server"></asp:Label>
                </h1>
                
                <p style="color: var(--text-gray, #ccc); margin-bottom: 10px;">Mã SP: <b><asp:Label ID="lblMaSP" runat="server"></asp:Label></b></p>
                
                <h2 style="color: var(--text-light, #fff); font-size: 2em; font-weight: bold; margin-bottom: 25px;">
                    <asp:Label ID="lblGiaBan" runat="server"></asp:Label>
                </h2>
                
                <div style="background: rgba(0, 0, 0, 0.4); padding: 20px; border-radius: 12px; border-left: 4px solid var(--gold-primary, #00FFD1); margin-bottom: 30px;">
                    <h4 style="color: var(--gold-primary, #00FFD1); margin-top: 0; margin-bottom: 10px; display: flex; align-items: center; gap: 10px;">
                        <i class="fa-solid fa-gem"></i> Thông tin & Thiết kế:
                    </h4>
                    <asp:Label ID="lblMoTa" runat="server" style="line-height: 1.8; color: var(--text-light, #fff); white-space: pre-line;"></asp:Label>
                </div>

                <asp:Button ID="btnThemVaoGio" runat="server" Text="🛒 Thêm Vào Giỏ Hàng" OnClick="btnThemVaoGio_Click" CssClass="btn-gold" style="font-size: 16px; padding: 15px; width: 100%; border-radius: 50px;" />
            </div>
            
        </div>
    </div>
</asp:Content>