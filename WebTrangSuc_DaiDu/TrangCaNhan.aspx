<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrangCaNhan.aspx.cs" Inherits="WebTrangSuc_DaiDu.TrangCaNhan" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="UTF-8">
    <title>Hồ Sơ | Trang Sức Đại Du</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <style>
        * { box-sizing: border-box; margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
        body { background-color: #f5f5f5; color: #333; }
        
        /* HEADER TỐI GIẢN */
        .profile-header { background: #fff; padding: 15px 50px; display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #1E90FF; box-shadow: 0 2px 10px rgba(0,0,0,0.05); }
        .logo-text { font-size: 22px; font-weight: bold; color: #1E90FF; text-decoration: none; letter-spacing: 1px; }
        
        .container { max-width: 1200px; margin: 30px auto; display: flex; gap: 20px; align-items: flex-start; }
        
        /* SIDEBAR */
        .sidebar { width: 260px; background: transparent; }
        .user-info { display: flex; align-items: center; gap: 15px; margin-bottom: 30px; padding-bottom: 20px; border-bottom: 1px solid #ddd; }
        .user-info img { width: 55px; height: 55px; border-radius: 50%; border: 2px solid #1E90FF; object-fit: cover; }
        .btn-edit-profile { font-size: 12px; color: #777; cursor: pointer; border: none; background: none; transition: 0.2s; }
        .btn-edit-profile:hover { color: #1E90FF; font-weight: bold; }
        
        .menu-list { list-style: none; }
        .menu-list li { margin-bottom: 15px; cursor: pointer; color: #555; font-weight: 500; transition: 0.2s; display: flex; align-items: center; gap: 10px; }
        .menu-list li:hover, .menu-list li.active { color: #1E90FF; font-weight: bold; }
        .menu-list li i { width: 20px; text-align: center; color: #1E90FF; }
        
        /* NỘI DUNG */
        .main-content { flex: 1; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); min-height: 500px; padding: 30px; }
        .tab-content { display: none; }
        .tab-content.active { display: block; animation: fadeIn 0.3s ease; }
        @keyframes fadeIn { from { opacity: 0; transform: translateY(5px); } to { opacity: 1; transform: translateY(0); } }
        
        .tab-title { font-size: 20px; color: #333; margin-bottom: 5px; border-left: 4px solid #1E90FF; padding-left: 10px; }
        .tab-desc { font-size: 14px; color: #777; margin-bottom: 25px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
        
        /* BẢNG GRID CHUẨN THƯƠNG MẠI ĐIỆN TỬ */
        .shopee-grid { width: 100%; border-collapse: collapse; text-align: center; font-size: 14px; margin-top: 15px; }
        .shopee-grid th { background: #f8f9fa; padding: 12px; color: #555; border-bottom: 2px solid #1E90FF; font-weight: 600; }
        .shopee-grid td { padding: 15px 12px; border-bottom: 1px solid #eee; color: #333; }
        
        .badge { padding: 5px 10px; border-radius: 4px; font-size: 12px; font-weight: bold; color: #fff; }
        .bg-cho { background: #f39c12; } .bg-giao { background: #3498db; } .bg-xong { background: #2ecc71; } .bg-huy { background: #e74c3c; }
        .text-blue { color: #1E90FF; font-weight: bold; } .text-red { color: #e74c3c; font-weight: bold; }
        .link-don-hang { color: #1E90FF; text-decoration: underline; font-weight: bold; transition: 0.2s; }
        .link-don-hang:hover { color: #ff4757; }
        
        /* VOUCHER CARD */
        .vc-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 15px; }
        .vc-card { border: 1px solid #e0e0e0; border-radius: 8px; display: flex; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.02); }
        .vc-left { background: #1E90FF; color: white; width: 100px; display: flex; align-items: center; justify-content: center; flex-direction: column; font-weight: bold; font-size: 18px; border-right: 2px dashed #fff; }
        .vc-right { padding: 15px; flex: 1; background: #fff; position: relative; }
        .vc-code { font-family: monospace; background: #f0f8ff; color: #1E90FF; padding: 3px 6px; border-radius: 3px; border: 1px solid #cce5ff; margin-top: 5px; display: inline-block; font-weight: bold; }
        
        .btn-logout { background: none; border: none; color: #555; font-size: 15px; font-weight: 500; cursor: pointer; display: flex; align-items: center; gap: 10px; padding: 0; font-family: inherit; }
        .btn-logout:hover { color: #e74c3c; }

        /* MODAL SỬA HỒ SƠ */
        .modal-overlay { display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.6); z-index: 999; justify-content: center; align-items: center; }
        .modal-box { background: #fff; padding: 30px; border-radius: 10px; width: 400px; box-shadow: 0 10px 30px rgba(0,0,0,0.3); position: relative; animation: slideDown 0.3s ease; }
        @keyframes slideDown { from { transform: translateY(-20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
        .form-group { margin-bottom: 15px; }
        .form-group label { display: block; margin-bottom: 5px; font-weight: 500; color: #555; font-size: 14px; }
        .form-control { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 5px; outline: none; }
        .form-control:focus { border-color: #1E90FF; }
        .btn-save { background: #1E90FF; color: white; padding: 10px 20px; border: none; border-radius: 5px; width: 100%; font-weight: bold; cursor: pointer; margin-top: 10px; }
        .btn-save:hover { background: #0073e6; }
        .close-btn { position: absolute; top: 15px; right: 15px; cursor: pointer; font-size: 20px; color: #aaa; }
        .close-btn:hover { color: #e74c3c; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <header class="profile-header">
            <a href="index.aspx" class="logo-text"><i class="fa-solid fa-gem"></i> TRANG SỨC ĐẠI DU</a>
            <a href="sanpham.aspx" style="color: #1E90FF; text-decoration: none; font-weight: bold;"><i class="fa-solid fa-house"></i> Tiếp tục mua sắm</a>
        </header>

        <div class="container">
            <div class="sidebar">
                <div class="user-info">
                    <asp:Image ID="imgAvatar" runat="server" />
                    <div>
                        <div style="font-weight: bold; font-size: 16px; color: #333;"><asp:Label ID="lblTen" runat="server"></asp:Label></div>
                        <button type="button" class="btn-edit-profile" onclick="MoFormSuaHoSo()"><i class="fa-solid fa-pen"></i> Sửa hồ sơ</button>
                    </div>
                </div>
                <ul class="menu-list">
                    <li class="active" onclick="switchTab('hoso')"><i class="fa-regular fa-user"></i> Hồ Sơ Của Tôi</li>
                    <li onclick="switchTab('donmua')"><i class="fa-solid fa-clipboard-list"></i> Đơn Mua</li>
                    <li onclick="switchTab('vitien')"><i class="fa-solid fa-wallet"></i> Ví Tiền & Lịch Sử GD</li>
                    <li onclick="switchTab('voucher')"><i class="fa-solid fa-ticket"></i> Kho Voucher</li>
                    <li style="margin-top: 30px;">
                        <asp:LinkButton ID="btnDangXuat" runat="server" CssClass="btn-logout" OnClick="btnDangXuat_Click">
                            <i class="fa-solid fa-right-from-bracket" style="color:#e74c3c;"></i> Đăng Xuất
                        </asp:LinkButton>
                    </li>
                </ul>
            </div>

            <div class="main-content">
                
                <div id="tab-hoso" class="tab-content active">
                    <h2 class="tab-title">Hồ Sơ Của Tôi</h2>
                    <div class="tab-desc">Quản lý thông tin hồ sơ tài khoản</div>
                    <table style="width: 100%; max-width: 600px; line-height: 2.5; font-size: 15px;">
                        <tr><td style="color:#777; width: 180px;">Tên đăng nhập / SĐT:</td><td><asp:Label ID="lblSDT" runat="server" Font-Bold="true"></asp:Label></td></tr>
                        <tr><td style="color:#777;">Email liên hệ:</td><td><asp:Label ID="lblEmail" runat="server" Font-Bold="true"></asp:Label></td></tr>
                        <tr><td style="color:#777;">Hạng Thành Viên:</td><td><span style="color:#1E90FF; font-weight:bold;"><i class="fa-solid fa-medal"></i> <asp:Label ID="lblHang" runat="server"></asp:Label></span></td></tr>
                        <tr><td style="color:#777;">Mã Vân Tay (Sinh trắc):</td><td><asp:Label ID="lblVanTay" runat="server" Text="Chưa thiết lập" Font-Italic="true"></asp:Label></td></tr>
                    </table>
                </div>

                <div id="tab-donmua" class="tab-content">
                    <h2 class="tab-title">Đơn Mua</h2>
                    <div class="tab-desc">Nhấn vào từng đơn hàng để xem chi tiết sản phẩm</div>
                    
                    <div class="order-list-container">
                        <asp:Repeater ID="rptDonHang" runat="server" OnItemDataBound="rptDonHang_ItemDataBound">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin-bottom: 15px; border-radius: 8px; background: #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.05); overflow: hidden;">
                                    
                                    <div onclick="toggleOrderDetails('detail_<%# Eval("MaHD") %>', this)" 
                                         style="padding: 15px 20px; cursor: pointer; display: flex; justify-content: space-between; align-items: center; background: #f8f9fa; transition: background 0.3s;">
                                        <div>
                                            <strong style="color: #1E90FF; font-size: 15px;"><i class="fa-solid fa-box"></i> MÃ ĐƠN: <%# Eval("MaHD") %></strong>
                                            <span style="margin-left: 15px; color: #777; font-size: 13px;"><i class="fa-regular fa-clock"></i> <%# Eval("NgayDat", "{0:dd/MM/yyyy HH:mm}") %></span>
                                        </div>
                                        <div style="display: flex; align-items: center;">
                                            <span class='badge <%# GetTrangThaiClass(Eval("TrangThai").ToString()) %>'><%# Eval("TrangThai") %></span>
                                            <strong style="margin-left: 20px; color: #ff4757; font-size: 16px;"><%# Eval("TongTien", "{0:N0} đ") %></strong>
                                            <i class="fa-solid fa-chevron-down icon-arrow" style="margin-left: 15px; color: #777; transition: transform 0.3s;"></i>
                                        </div>
                                    </div>

                                    <div id='detail_<%# Eval("MaHD") %>' style="display: none; padding: 0 20px 20px 20px; border-top: 1px dashed #ddd; background: #fff;">
                                        <table class="shopee-grid" style="margin-top: 15px; box-shadow: none;">
                                            <thead>
                                                <tr>
                                                    <th style="text-align: left; padding-left: 10px;">Sản Phẩm</th>
                                                    <th>Đơn Giá</th>
                                                    <th>Số Lượng</th>
                                                    <th>Thành Tiền</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="rptChiTiet" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="text-align: left; display: flex; align-items: center; gap: 15px;">
                                                                <img src='IMAGES/Sanpham/<%# Eval("HinhAnh") %>' width="50" style="border-radius: 5px; border: 1px solid #eee;" />
                                                                <span style="font-weight: 500; color: #333;"><%# Eval("TenSP") %></span>
                                                            </td>
                                                            <td style="color: #777;"><%# Eval("DonGia", "{0:N0} đ") %></td>
                                                            <td>x<%# Eval("SoLuong") %></td>
                                                            <td style="color: #1E90FF; font-weight: bold;"><%# (Convert.ToDouble(Eval("DonGia")) * Convert.ToInt32(Eval("SoLuong"))).ToString("N0") %> đ</td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblKhongCoDon" runat="server" Text="Bạn chưa có đơn hàng nào." Visible="false" Font-Italic="true" ForeColor="#999"></asp:Label>
                    </div>
                </div>

                <div id="tab-vitien" class="tab-content">
                    <h2 class="tab-title">Ví Tiền & Lịch Sử Giao Dịch</h2>
                    <div class="tab-desc">Quản lý dòng tiền và các biến động số dư</div>
                    
                    <div style="display: flex; gap: 20px; margin-bottom: 30px; flex-wrap: wrap;">
                        <div style="flex: 1; min-width: 250px; background: linear-gradient(135deg, #1E90FF, #00BFFF); padding: 25px; border-radius: 10px; color: white; text-align: center; box-shadow: 0 5px 15px rgba(30,144,255,0.3);">
                            <div style="font-size: 15px; margin-bottom: 10px;"><i class="fa-solid fa-wallet"></i> SỐ DƯ KHẢ DỤNG</div>
                            <div style="font-size: 35px; font-weight: bold;"><asp:Label ID="lblSoDu" runat="server" Text="0 đ"></asp:Label></div>
                        </div>
                        <div style="flex: 1; min-width: 250px; background: linear-gradient(135deg, #ff9f43, #ff6b6b); padding: 25px; border-radius: 10px; color: white; text-align: center; box-shadow: 0 5px 15px rgba(255,107,107,0.3);">
                            <div style="font-size: 15px; margin-bottom: 10px;"><i class="fa-solid fa-chart-pie"></i> TỔNG CHI TIÊU</div>
                            <div style="font-size: 35px; font-weight: bold;"><asp:Label ID="lblTongChiTieu" runat="server" Text="0 đ"></asp:Label></div>
                        </div>
                    </div>

                    <h3 style="font-size: 16px; margin-bottom: 15px; color: #555; border-bottom: 1px solid #ddd; padding-bottom: 5px;">Giao Dịch Gần Đây</h3>
                    <asp:GridView ID="gvSaoKe" runat="server" AutoGenerateColumns="False" CssClass="shopee-grid" EmptyDataText="Chưa có lịch sử giao dịch.">
                        <Columns>
                            <asp:BoundField DataField="NgayGD" HeaderText="Thời Gian" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="MaGD" HeaderText="Mã Giao Dịch" />
                            <asp:BoundField DataField="NoiDung" HeaderText="Nội Dung" ItemStyle-HorizontalAlign="Left" />
                            <asp:TemplateField HeaderText="Số Tiền">
                                <ItemTemplate>
                                    <span class='<%# Eval("LoaiGD").ToString() == "NẠP TIỀN" ? "text-blue" : "text-red" %>'>
                                        <%# Eval("LoaiGD").ToString() == "NẠP TIỀN" ? "+" : "-" %> <%# Eval("SoTien", "{0:N0}") %> đ
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <div id="tab-voucher" class="tab-content">
                    <h2 class="tab-title">Kho Voucher</h2>
                    <div class="tab-desc">Các mã giảm giá đang có hiệu lực của bạn</div>
                    <asp:Label ID="lblChuaCoVoucher" runat="server" Text="Kho voucher của bạn đang trống." ForeColor="#999" Visible="false" Font-Italic="true"></asp:Label>
                    
                    <asp:Repeater ID="rptVoucher" runat="server">
                        <HeaderTemplate><div class="vc-grid"></HeaderTemplate>
                        <ItemTemplate>
                            <div class="vc-card">
                                <div class="vc-left">
                                    <i class="fa-solid fa-ticket" style="font-size: 24px; margin-bottom: 5px;"></i>
                                    <%# Eval("LoaiGiam").ToString() == "%" ? Eval("GiaTriGiam") + "%" : (Convert.ToDouble(Eval("GiaTriGiam")) / 1000) + "K" %>
                                </div>
                                <div class="vc-right">
                                    <div style="font-weight: bold; color: #333; font-size: 15px;"><%# Eval("TenVoucher") %></div>
                                    <div style="font-size: 12px; color: #777; margin-top: 5px;">Đơn tối thiểu <%# Eval("DonToiThieu", "{0:N0}") %>đ</div>
                                    <div class="vc-code"><%# Eval("MaVoucher") %></div>
                                    <div style="font-size: 11px; color: #e74c3c; position: absolute; bottom: 10px; right: 10px;">HSD: <%# Eval("NgayKetThuc", "{0:dd/MM/yyyy}") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <div id="modalSuaHoSo" class="modal-overlay">
            <div class="modal-box">
                <i class="fa-solid fa-xmark close-btn" onclick="DongFormSuaHoSo()"></i>
                <h3 style="margin-bottom: 20px; color: #1E90FF; text-align: center;">Cập Nhật Hồ Sơ</h3>
                
                <div class="form-group">
                    <label>Họ và Tên hiển thị:</label>
                    <asp:TextBox ID="txtEditTen" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Email liên hệ:</label>
                    <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Mật khẩu mới (Để trống nếu không đổi):</label>
                    <asp:TextBox ID="txtEditMatKhau" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                </div>
                
                <div class="form-group" style="background: #f0f8ff; padding: 15px; border-radius: 8px; border: 1px dashed #1E90FF; text-align: center;">
                    <label style="color: #1E90FF; margin-bottom: 10px; font-weight: bold;"><i class="fa-solid fa-fingerprint"></i> Thiết lập Sinh Trắc Học</label>
                    <br />
                    <button type="button" id="btnScan" onclick="giaLapQuetVanTay()" style="background: #0B1D51; color: #00FFD1; border: 1px solid #00FFD1; padding: 10px 20px; border-radius: 20px; cursor: pointer; transition: 0.3s; font-weight: bold;">
                        <i class="fa-solid fa-fingerprint"></i> Chạm để quét vân tay
                    </button>
                    <p id="txtScanStatus" style="font-size: 12px; color: #777; margin-top: 5px;">* Sử dụng cảm biến trên thiết bị</p>

                    <asp:HiddenField ID="hdfBioID" runat="server" />
                    <asp:HiddenField ID="hdfBioKey" runat="server" />
                </div>
                
                <asp:Button ID="btnLuuHoSo" runat="server" Text="LƯU THAY ĐỔI" CssClass="btn-save" OnClick="btnLuuHoSo_Click" />
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <script>
            function switchTab(tabId) {
                document.querySelectorAll('.tab-content').forEach(t => t.classList.remove('active'));
                document.querySelectorAll('.menu-list li').forEach(l => l.classList.remove('active'));
                document.getElementById('tab-' + tabId).classList.add('active');
                event.currentTarget.classList.add('active');
            }

            function MoFormSuaHoSo() {
                document.getElementById('modalSuaHoSo').style.display = 'flex';
            }

            function DongFormSuaHoSo() {
                document.getElementById('modalSuaHoSo').style.display = 'none';
            }

            // JavaScript cho thao tác bấm xổ xuống ở Tab Đơn Mua
            function toggleOrderDetails(id, headerElement) {
                var el = document.getElementById(id);
                var arrow = headerElement.querySelector('.icon-arrow');
                
                if (el.style.display === "none" || el.style.display === "") {
                    el.style.display = "block";
                    headerElement.style.background = "#eef5ff";
                    arrow.style.transform = "rotate(180deg)";
                } else {
                    el.style.display = "none";
                    headerElement.style.background = "#f8f9fa";
                    arrow.style.transform = "rotate(0deg)";
                }
            }

            // JS giả lập quét vân tay
            function giaLapQuetVanTay() {
                Swal.fire({
                    title: 'Xác thực sinh trắc học',
                    text: 'Vui lòng chạm vào cảm biến vân tay trên thiết bị...',
                    icon: 'info',
                    showConfirmButton: false,
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                        setTimeout(() => {
                            let fakeBioID = 'CRED-' + Math.random().toString(36).substring(2, 15).toUpperCase();
                            let fakeBioKey = 'PUBKEY-' + btoa(Math.random().toString()).substring(0, 20);

                            document.getElementById('<%= hdfBioID.ClientID %>').value = fakeBioID;
                            document.getElementById('<%= hdfBioKey.ClientID %>').value = fakeBioKey;

                            let btn = document.getElementById('btnScan');
                            btn.innerHTML = '<i class="fa-solid fa-check"></i> Đã lấy mẫu vân tay';
                            btn.style.background = '#2ecc71';
                            btn.style.borderColor = '#2ecc71';
                            btn.style.color = '#fff';
                            document.getElementById('txtScanStatus').innerText = 'Dữ liệu đã mã hóa an toàn.';

                            Swal.fire({
                                title: 'Thành công!',
                                text: 'Đã nhận diện sinh trắc học thành công. Vui lòng bấm LƯU THAY ĐỔI.',
                                icon: 'success',
                                background: '#0B1D51', color: '#fff', confirmButtonColor: '#00FFD1'
                            });
                        }, 2000);
                    }
                });
            }
        </script>
    </form>
</body>
</html>