<%@ Page Title="Liên Hệ - Trang Sức ĐẠI DU" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="lienhe.aspx.cs" Inherits="WebTrangSuc_DaiDu.lienhe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <section class="contact-section" style="padding: 50px 20px; max-width: 1200px; margin: 0 auto;">
        <h2 class="section-title" style="text-align:center; margin-bottom: 40px; color: #C19A6B;">LIÊN HỆ VỚI CHÚNG TÔI</h2>
        
        <div class="contact-container" style="display: flex; flex-wrap: wrap; gap: 30px; justify-content: space-between;">
            
            <div class="contact-info" style="flex: 1; min-width: 300px; background: #111; padding: 30px; border-radius: 10px; border: 1px solid #333;">
                <h3 style="color: #00FFD1; margin-top: 0;">Cửa Hàng Trang Sức ĐẠI DU</h3>
                <p style="color: #ccc; line-height: 2;"><i class="fa-solid fa-location-dot" style="color: #ff0055; width: 25px;"></i> Địa chỉ: Xã Đồng Bằng, tỉnh Hưng Yên</p>
                <p style="color: #ccc; line-height: 2;"><i class="fa-solid fa-phone" style="color: #00FFD1; width: 25px;"></i> Điện thoại: 037 972 9965</p>
                <p style="color: #ccc; line-height: 2;"><i class="fa-solid fa-envelope" style="color: #ffcc00; width: 25px;"></i> Email: thaitugia17@gmail.com</p>
                <p style="color: #ccc; line-height: 2;"><i class="fa-solid fa-clock" style="color: #b258ff; width: 25px;"></i> Giờ mở cửa: 8:00 - 21:00 (T2 - CN)</p>
                
                <div class="social-links-wrapper" style="margin-top: 30px; display: flex; gap: 15px;">
                    <a href="GẮN_LINK_FB_VÀO_ĐÂY" target="_blank" class="social-item fb" style="color: #fff; text-decoration: none; background: #1877f2; padding: 10px 20px; border-radius: 5px;">
                        <i class="fa-brands fa-facebook"></i> Facebook
                    </a>
                    <a href="GẮN_LINK_IG_VÀO_ĐÂY" target="_blank" class="social-item ins" style="color: #fff; text-decoration: none; background: #e4405f; padding: 10px 20px; border-radius: 5px;">
                        <i class="fa-brands fa-instagram"></i> Instagram
                    </a>
                </div>
            </div>

            <div class="contact-form" style="flex: 1; min-width: 300px; background: rgba(0,0,0,0.5); padding: 30px; border-radius: 10px; border: 1px solid #333;">
                
                <div style="margin-bottom: 15px;">
                    <asp:TextBox ID="txtHoTen" runat="server" placeholder="Họ và tên của bạn *" required="required" style="width: 100%; padding: 12px; box-sizing: border-box; background: #222; border: 1px solid #444; color: #fff; border-radius: 5px;"></asp:TextBox>
                </div>
                
                <div style="margin-bottom: 15px;">
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="Email liên hệ *" required="required" style="width: 100%; padding: 12px; box-sizing: border-box; background: #222; border: 1px solid #444; color: #fff; border-radius: 5px;"></asp:TextBox>
                </div>
                
                <div style="margin-bottom: 15px;">
                    <asp:TextBox ID="txtTieuDe" runat="server" placeholder="Tiêu đề *" required="required" style="width: 100%; padding: 12px; box-sizing: border-box; background: #222; border: 1px solid #444; color: #fff; border-radius: 5px;"></asp:TextBox>
                </div>
                
                <div style="margin-bottom: 20px;">
                    <asp:TextBox ID="txtNoiDung" runat="server" TextMode="MultiLine" Rows="5" placeholder="Bạn cần tư vấn trang sức gì?..." required="required" style="width: 100%; padding: 12px; box-sizing: border-box; background: #222; border: 1px solid #444; color: #fff; border-radius: 5px; resize: vertical;"></asp:TextBox>
                </div>
                
                <asp:Button ID="btnGuiTinNhan" runat="server" Text="GỬI TIN NHẮN" OnClick="btnGuiTinNhan_Click" style="width: 100%; padding: 15px; background: #00FFD1; color: #000; font-weight: bold; border: none; border-radius: 5px; cursor: pointer; font-size: 1.1rem; text-transform: uppercase;" />
                
            </div>
        </div>
    </section>

    <div class="map-container" style="margin-top: 40px;">
        <iframe src="https://maps.google.com/maps?q=Hưng%20Yên&t=&z=13&ie=UTF8&iwloc=&output=embed" 
            style="width: 100%; height: 400px; border:0;" allowfullscreen="" loading="lazy"></iframe>
    </div>
</asp:Content>