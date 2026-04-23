<%@ Page Title="Đăng Nhập VIP" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DangNhap.aspx.cs" Inherits="WebTrangSuc_DaiDu.DangNhap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href='<%= "/CSS/Theme/" + (Application["CurrentTheme"] ?? "banthuong") + "/tc1.css" %>' rel="stylesheet" />
    <style>
       .btn-biometric {
           background: linear-gradient(135deg, #0B1D51, #00FFD1);
           color: #fff; border: none; padding: 12px; width: 100%; border-radius: 8px;
           font-weight: bold; cursor: pointer; font-size: 15px; margin-bottom: 15px;
           transition: 0.3s; display: flex; align-items: center; justify-content: center; gap: 10px;
           box-shadow: 0 4px 15px rgba(0, 255, 209, 0.3);
       }
       .btn-biometric:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(0, 255, 209, 0.5); }
   </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="registration-container">
        <h2 style="color: #00FFD1;">ĐĂNG NHẬP VIP</h2>
        
        <label>Tên đăng nhập</label>
        <asp:TextBox ID="txtTaiKhoan" runat="server" CssClass="input-field" placeholder="Nhập tài khoản..."></asp:TextBox>
        
        <label>Mật khẩu</label>
        <div style="position: relative;">
           <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="input-field" placeholder="Nhập mật khẩu..." style="padding-right: 40px;"></asp:TextBox>
           <i class="fa-regular fa-eye" id="togglePassword" style="position: absolute; right: 15px; top: 18px; cursor: pointer; color: #00FFD1; font-size: 16px;"></i>
        </div>

        <button type="button" class="btn-biometric" onclick="DangNhapBangVanTay()">
            <i class="fa-solid fa-face-viewfinder" style="font-size: 20px;"></i> Đăng Nhập Bằng Vân Tay / FaceID
        </button>

        <div style="text-align:center; margin: 10px 0; color: #777; font-size: 12px;">HOẶC</div>

        <asp:Button ID="btnDangNhap" runat="server" Text="Đăng Nhập Truyền Thống" CssClass="btn-gold" style="background:#333; color:#fff;" OnClick="btnDangNhap_Click" />
            
        <div style="text-align: center; margin-top: 15px;">
            <a href="Dangky.aspx" style="color: #00FFD1; text-decoration: none; font-size: 0.9em;">Chưa có tài khoản? Đăng ký ngay</a>
        </div>
    </div>

    <script type="text/javascript">
        document.querySelector('#togglePassword').addEventListener('click', function (e) {
            const password = document.querySelector('#<%= txtMatKhau.ClientID %>');
            const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
            password.setAttribute('type', type);
            this.classList.toggle('fa-eye-slash');
        });

        // 🔥 GỌI CẢM BIẾN VÂN TAY LÚC ĐĂNG NHẬP 🔥
        async function DangNhapBangVanTay() {
            let tk = document.getElementById('<%= txtTaiKhoan.ClientID %>').value;
            if (tk === "") {
                Swal.fire('Chờ chút Sếp', 'Nhập Tên Tài Khoản vào trước để hệ thống quét đúng dấu vân tay nha!', 'warning');
                return;
            }

            if (!window.PublicKeyCredential) {
                Swal.fire('Opps!', 'Thiết bị không hỗ trợ vân tay/FaceID.', 'info');
                return;
            }

            try {
                // Giả lập dữ liệu xin quyền xác thực
                const publicKeyCredentialRequestOptions = {
                    challenge: new Uint8Array(32),
                    timeout: 60000,
                    userVerification: "required"
                };

                // Gọi Pop-up xác thực của Điện thoại/Laptop
                const assertion = await navigator.credentials.get({ publicKey: publicKeyCredentialRequestOptions });
                
                if(assertion) {
                    Swal.fire({ toast: true, position: 'top-end', icon: 'success', title: 'Xác thực vân tay OK!', showConfirmButton: false, timer: 1500 });
                    // Điền bừa 1 cái Pass để vượt qua Front-End (Bảo mật thực tế sẽ check Token, nhưng với Đồ án, trải nghiệm là chính!)
                    document.getElementById('<%= txtMatKhau.ClientID %>').value = "BypassByFingerprint"; 
                    setTimeout(() => { document.getElementById('<%= btnDangNhap.ClientID %>').click(); }, 1000);
                }
            } catch (err) {
                console.log(err);
                Swal.fire('Đã Hủy', 'Xác thực vân tay thất bại hoặc bị hủy.', 'error');
            }
        }
    </script>
</asp:Content>