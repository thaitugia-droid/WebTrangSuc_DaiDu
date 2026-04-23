<%@ Page Title="Đăng Ký Thành Viên" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dangky.aspx.cs" Inherits="WebTrangSuc_DaiDu.Dangky" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <link href='<%= "/CSS/Theme/" + (Application["CurrentTheme"] ?? "banthuong") + "/tc1.css" %>' rel="stylesheet" />
   <style>
       /* Nút Vân Tay chuẩn Sàn Lớn mang hơi hướng Đại Dương */
       .btn-biometric {
           background: linear-gradient(135deg, #0B1D51, #00FFD1);
           color: #fff; border: none; padding: 12px; width: 100%; border-radius: 8px;
           font-weight: bold; cursor: pointer; font-size: 15px; margin-bottom: 15px;
           transition: 0.3s; display: flex; align-items: center; justify-content: center; gap: 10px;
           box-shadow: 0 4px 15px rgba(0, 255, 209, 0.3);
       }
       .btn-biometric:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(0, 255, 209, 0.5); }
       
       .avatar-upload { margin-bottom: 15px; background: rgba(255,255,255,0.05); padding: 10px; border-radius: 8px; border: 1px dashed #00FFD1; }
   </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="registration-container">
        <h2 style="color: #00FFD1;">ĐĂNG KÝ THÀNH VIÊN VIP</h2>
        
        <label>Tên đăng nhập (*)</label>
        <asp:TextBox ID="txtTaiKhoan" runat="server" CssClass="input-field" placeholder="Ví dụ: thaitugia17"></asp:TextBox>
        
        <label>Mật khẩu (*)</label>
        <div style="position: relative;">
          <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="input-field" placeholder="Nhập mật khẩu..." style="padding-right: 40px;"></asp:TextBox>
          <i class="fa-regular fa-eye" id="togglePasswordDK" style="position: absolute; right: 15px; top: 18px; cursor: pointer; color: #00FFD1; font-size: 16px;"></i>
        </div>
        
        <label>Họ và tên (*)</label>
        <asp:TextBox ID="txtHoTen" runat="server" CssClass="input-field" placeholder="Tên để in lên hóa đơn"></asp:TextBox>
        
        <label>Số điện thoại (*)</label>
        <asp:TextBox ID="txtSDT" runat="server" CssClass="input-field"></asp:TextBox>
        
        <label>Email</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="input-field" placeholder="Tùy chọn"></asp:TextBox>
        
        <label>Địa chỉ</label>
        <asp:TextBox ID="txtDiaChi" runat="server" CssClass="input-field" placeholder="Tùy chọn"></asp:TextBox>

        <div class="avatar-upload">
            <label style="color: #00FFD1; font-size: 12px;"><i class="fa-solid fa-image"></i> Chọn Ảnh Đại Diện (Avatar)</label>
            <asp:FileUpload ID="fuAvatar" runat="server" style="width: 100%; color: #fff; margin-top: 5px;" />
        </div>

        <button type="button" class="btn-biometric" onclick="DangKyBangVanTay()">
            <i class="fa-solid fa-fingerprint" style="font-size: 20px;"></i> Đăng ký siêu tốc bằng Vân Tay / FaceID
        </button>

        <div style="text-align:center; margin: 10px 0; color: #777; font-size: 12px;">HOẶC</div>

        <asp:Button ID="btnDangKy" runat="server" Text="Đăng Ký Truyền Thống" CssClass="btn-gold" style="background:#333; color:#fff;" OnClick="btnDangKy_Click" />
            
        <div style="text-align: center; margin-top: 15px;">
            <a href="DangNhap.aspx" style="color: #00FFD1; text-decoration: none; font-size: 13px;">Đã có tài khoản? Đăng nhập ngay</a>
        </div>
    </div>

    <script type="text/javascript">
        // Chức năng ẩn hiện mật khẩu
        document.querySelector('#togglePasswordDK').addEventListener('click', function (e) {
            const pwd = document.querySelector('#<%= txtMatKhau.ClientID %>');
            pwd.setAttribute('type', pwd.getAttribute('type') === 'password' ? 'text' : 'password');
            this.classList.toggle('fa-eye-slash');
        });

        // 🔥 THUẬT TOÁN GỌI CẢM BIẾN VÂN TAY (WEBAUTHN) 🔥
        async function DangKyBangVanTay() {
            // Kiểm tra khách đã nhập Tài Khoản và Mật Khẩu chưa
            let tk = document.getElementById('<%= txtTaiKhoan.ClientID %>').value;
            let mk = document.getElementById('<%= txtMatKhau.ClientID %>').value;
            let ten = document.getElementById('<%= txtHoTen.ClientID %>').value;

            if (tk === "" || mk === "" || ten === "") {
                Swal.fire('Khoan đã Sếp', 'Nhập đủ Tài Khoản, Mật Khẩu và Họ Tên trước khi quét vân tay nhé!', 'warning');
                return;
            }

            if (!window.PublicKeyCredential) {
                Swal.fire('Opps!', 'Trình duyệt hoặc thiết bị của sếp không hỗ trợ quét vân tay/khuôn mặt.', 'info');
                return;
            }

            try {
                // Tạo Data giả lập FIDO2 để lừa trình duyệt bật Popup Quét Vân Tay của thiết bị
                const publicKeyCredentialCreationOptions = {
                    challenge: new Uint8Array(32), 
                    rp: { name: "Trang Sức Đại Du", id: window.location.hostname },
                    user: {
                        id: new Uint8Array(16),
                        name: tk,
                        displayName: ten
                    },
                    pubKeyCredParams: [{alg: -7, type: "public-key"}],
                    authenticatorSelection: { authenticatorAttachment: "platform", userVerification: "required" },
                    timeout: 60000
                };

                // Lệnh này sẽ gọi màn hình "Chạm vào nút nguồn / Quét FaceID" của điện thoại/laptop
                const credential = await navigator.credentials.create({ publicKey: publicKeyCredentialCreationOptions });
                
                if(credential) {
                    // Nếu khách quét ngón tay đúng -> Tự động bấm nút Đăng Ký truyền thống để gửi dữ liệu về C#
                    Swal.fire({ toast: true, position: 'top-end', icon: 'success', title: 'Quét vân tay thành công!', showConfirmButton: false, timer: 1500 });
                    setTimeout(() => { document.getElementById('<%= btnDangKy.ClientID %>').click(); }, 1000);
                }
            } catch (err) {
                console.log(err);
                Swal.fire('Đã Hủy', ' Đã hủy quét vân tay hoặc thiết bị không nhận diện được.', 'error');
            }
        }
    </script>
</asp:Content>