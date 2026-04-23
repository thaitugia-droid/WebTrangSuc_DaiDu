<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminHub.aspx.cs" Inherits="WebTrangSuc_DaiDu.AdminHub" %>
<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <title>Trợ Lý Bỏ Túi Admin</title>
    <style>
        body { background: #f4f6f9; color: #333; font-family: 'Segoe UI', sans-serif; margin: 0; padding: 0; overflow-x: hidden; }
        .header { background: linear-gradient(90deg, #111, #222); color: #FFD700; padding: 15px; text-align: center; font-weight: bold; letter-spacing: 1px; }
        .menu-item { display: block; padding: 15px; color: #fff; background: #222; text-decoration: none; border-bottom: 1px solid #333; transition: all 0.3s ease; font-size: 14px; }
        .menu-item:hover { background: #444; padding-left: 25px; border-left: 4px solid #FFD700; }
        .btn-ai { background: #e8f0fe; color: #1a73e8; font-weight: bold; text-align:center; border-radius:8px; margin:10px; border: 1px solid #1a73e8; }
        .ai-header-info { padding: 15px; border-bottom: 1px solid #ddd; font-size: 13px; text-transform: uppercase; color: #555; }
        .ai-header-info div { display: flex; justify-content: space-between; margin-bottom: 8px; }
        .ai-list { padding: 10px; height: 300px; overflow-y: auto; }
        .ai-card { background: #fff; border: 1px solid #ccc; border-radius: 10px; padding: 12px; margin-bottom: 12px; position: relative; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
        .ai-card h4 { margin: 0 0 5px 0; font-size: 15px; color: #222; }
        .ai-stats { font-size: 11px; color: #777; margin-bottom: 10px; }
        .badge { padding: 3px 8px; border-radius: 12px; font-size: 10px; font-weight: bold; border: 1px solid; display: inline-block; margin-bottom: 5px; }
        .b-ton { color: #c0392b; border-color: #c0392b; background: #fadbd8; }
        .b-hot { color: #27ae60; border-color: #27ae60; background: #eaeded; }
        .b-tiemnang { color: #2980b9; border-color: #2980b9; background: #d4e6f1; }
        .btn-apply { background: #1a73e8; color: #fff; border: none; padding: 6px 12px; border-radius: 5px; font-size: 11px; font-weight: bold; cursor: pointer; width: 100%; }
        .pin-container { background:#1a1a1a; color:#fff; height:100vh; text-align: center; padding: 30px 20px; }
        .pin-dots { display: flex; justify-content: center; gap: 15px; margin: 20px 0 30px 0; }
        .dot { width: 15px; height: 15px; border-radius: 50%; border: 2px solid #FFD700; transition: all 0.2s ease; }
        .dot.active { background: #FFD700; box-shadow: 0 0 10px #FFD700; }
        .numpad { display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px; max-width: 220px; margin: 0 auto; }
        .num-btn { background: rgba(255,255,255,0.1); color: #fff; border: 1px solid #555; width: 60px; height: 60px; border-radius: 50%; font-size: 24px; cursor: pointer; display: flex; align-items: center; justify-content: center; }
        .quick-form { padding: 15px; }
        .q-input { width: 100%; padding: 8px; margin-bottom: 10px; border: 1px solid #ccc; border-radius: 5px; box-sizing: border-box; }
        .search-box { display: flex; gap: 5px; margin-bottom: 15px; }
        .search-box input { flex: 1; }
        .btn-search { background: #2c3e50; color: white; border: none; padding: 8px 15px; border-radius: 5px; cursor: pointer; font-weight:bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="upAdmin" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnLuuSPNhanh" />
            </Triggers>
            <ContentTemplate>
                <asp:MultiView ID="mvAdmin" runat="server" ActiveViewIndex="0">
                    
                    <%-- MÀN 1: MÃ PIN --%>
                    <asp:View ID="vPinPad" runat="server">
                        <div class="pin-container">
                            <h3 style="color:#FFD700; margin-top:0;">XÁC THỰC GOD MODE</h3>
                            <div class="pin-dots"><div class="dot" id="dot1"></div><div class="dot" id="dot2"></div><div class="dot" id="dot3"></div><div class="dot" id="dot4"></div></div>
                            <asp:Label ID="lblPinError" runat="server" ForeColor="#ff4757" Font-Size="12px"></asp:Label>
                            <div class="numpad">
                                <div class="num-btn" onclick="nhapSo(1)">1</div><div class="num-btn" onclick="nhapSo(2)">2</div><div class="num-btn" onclick="nhapSo(3)">3</div>
                                <div class="num-btn" onclick="nhapSo(4)">4</div><div class="num-btn" onclick="nhapSo(5)">5</div><div class="num-btn" onclick="nhapSo(6)">6</div>
                                <div class="num-btn" onclick="nhapSo(7)">7</div><div class="num-btn" onclick="nhapSo(8)">8</div><div class="num-btn" onclick="nhapSo(9)">9</div>
                                <div class="num-btn" onclick="xoaSo()" style="color:#ff4757;">C</div><div class="num-btn" onclick="nhapSo(0)">0</div>
                                <div class="num-btn" onclick="window.parent.document.getElementById('khungAdminV5').classList.remove('show-panel');" style="font-size:16px;">Đóng</div>
                            </div>
                        </div>
                        <asp:HiddenField ID="hfPinCode" runat="server" />
                        <asp:Button ID="btnSubmitPin" runat="server" OnClick="btnSubmitPin_Click" style="display:none;" />
                        <script>
                            let currentPin = "";
                            function nhapSo(num) {
                                if (currentPin.length < 4) {
                                    currentPin += num; capNhatHienThi();
                                    if (currentPin.length === 4) {
                                        document.getElementById('<%= hfPinCode.ClientID %>').value = currentPin;
                                        document.getElementById('<%= btnSubmitPin.ClientID %>').click();
                                    }
                                }
                            }
                            function xoaSo() { currentPin = ""; capNhatHienThi(); }
                            function capNhatHienThi() {
                                for (let i = 1; i <= 4; i++) {
                                    let dot = document.getElementById('dot' + i);
                                    if (i <= currentPin.length) dot.classList.add('active'); else dot.classList.remove('active');
                                }
                            }
                        </script>
                    </asp:View>

                    <%-- MÀN 2: MENU CHÍNH GỌN GÀNG QUYỀN LỰC --%>
                    <asp:View ID="vMenu" runat="server">
                        <div class="header">👑 ADMIN SẾP ĐẠI</div>
                        <div style="background:#222; height:100vh; overflow-y:auto; padding-bottom:50px;">
                            <asp:LinkButton ID="btnMoFormSua" runat="server" CssClass="menu-item" OnClick="btnMoFormSua_Click">✏️ Tìm STT & Sửa Giá / Áp Mác</asp:LinkButton>
                            <asp:LinkButton ID="btnMoFormThem" runat="server" CssClass="menu-item" OnClick="btnMoFormThem_Click">➕ Thêm Mới SP Tại Chỗ</asp:LinkButton>
                            <asp:LinkButton ID="btnChayAI" runat="server" CssClass="menu-item btn-ai" OnClick="btnChayAI_Click">🤖 Kích hoạt AI Phân Tích</asp:LinkButton>
                            <a href="Admin.aspx" target="_parent" class="menu-item">📊 Vào Trang Quản Trị Lớn</a>

                            <div style="padding:15px; border-top:1px solid #333; margin-top:10px;">
                                <label style="color:#00FFD1; font-size:11px; font-weight:bold;">🎭 ĐỔI GIAO DIỆN LIVE:</label>
                                <asp:DropDownList ID="ddlTheme" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTheme_Changed" CssClass="q-input" style="background:#111; color:#FFD700; border:1px solid #555; margin-top:5px;">
                                    <asp:ListItem Value="banthuong">Bản Thường (Đen Vàng)</asp:ListItem>
                                    <asp:ListItem Value="giangsinh">Sự Kiện Giáng Sinh</asp:ListItem>
                                    <asp:ListItem Value="le304">Sự Kiện Lễ 30/4</asp:ListItem>
                                </asp:DropDownList>
                                </div>
                        </div>
                    </asp:View>

                    <%-- MÀN 3: BẢNG QUÉT AI TỪ ACCESS THẬT --%>
                    <asp:View ID="vAI_List" runat="server">
                        <div style="display:flex; align-items:center; padding:15px; background:#fff; border-bottom:1px solid #ddd;">
                            <asp:LinkButton ID="btnBackFromAI" runat="server" OnClick="btnBackToMenu_Click" style="text-decoration:none; font-size:20px; margin-right:10px; color:#333;">←</asp:LinkButton>
                            <h3 style="margin:0; font-size:16px;">AI PHÂN TÍCH GIÁ</h3>
                        </div>
                        <div class="ai-header-info">
                            <div><span>TỔNG SẢN PHẨM</span> <b style="color:#1a73e8; font-size:16px;"><asp:Label ID="lblTongSPAI" runat="server" Text="0"></asp:Label></b></div>
                            <div><span>TRẠNG THÁI HỆ THỐNG</span> <b style="color:#27ae60;">SẴN SÀNG</b></div>
                            <div><span>CƠ SỞ DỮ LIỆU</span> <b style="color:#c0392b;">MS Access</b></div>
                        </div>
                        <div class="ai-list" id="divDanhSachAI" runat="server"></div>
                    </asp:View>

                    <%-- MÀN 4: THÊM SẢN PHẨM TẠI CHỖ --%>
                    <asp:View ID="vThemSP" runat="server">
                        <div style="display:flex; align-items:center; padding:15px; background:#fff; border-bottom:1px solid #ddd;">
                            <asp:LinkButton ID="btnBackFromAdd" runat="server" OnClick="btnBackToMenu_Click" style="text-decoration:none; font-size:20px; margin-right:10px; color:#333;">←</asp:LinkButton>
                            <h3 style="margin:0; font-size:16px;">THÊM SP MỚI</h3>
                        </div>
                        <div class="quick-form" style="height: 320px; overflow-y: auto;">
                            <label style="font-size:11px; color:#1a73e8; font-weight:bold;">Tải ảnh sản phẩm lên:</label>
                            <asp:FileUpload ID="fileAnhSP" runat="server" CssClass="q-input" style="background:#e8f0fe; padding:5px; margin-bottom:10px;" />
                            <label style="font-size:11px; color:#666; font-weight:bold;">Mã SP:</label>
                            <asp:TextBox ID="txtMaSP" runat="server" CssClass="q-input" placeholder="Mã xịn do AI tạo..."></asp:TextBox>
                            <label style="font-size:11px; color:#666; font-weight:bold;">Tên SP:</label>
                            <asp:TextBox ID="txtTenSP" runat="server" CssClass="q-input" placeholder="Tên do AI dịch mượt..."></asp:TextBox>
                            <label style="font-size:11px; color:#666; font-weight:bold;">Mã DM:</label>
                            <asp:TextBox ID="txtMaDM" runat="server" CssClass="q-input" placeholder="Vd: DC1, NV4..."></asp:TextBox>
                            <div style="display:flex; gap:10px;">
                                <div style="flex:1;">
                                    <label style="font-size:11px; color:#666; font-weight:bold;">Giá bán:</label>
                                    <asp:TextBox ID="txtGia" runat="server" CssClass="q-input"></asp:TextBox>
                                </div>
                                <div style="flex:1;">
                                    <label style="font-size:11px; color:#666; font-weight:bold;">Số lượng:</label>
                                    <asp:TextBox ID="txtTonKho" runat="server" CssClass="q-input"></asp:TextBox>
                                </div>
                            </div>
                            <label style="font-size:11px; color:#666; font-weight:bold;">Mô tả:</label>
                            <asp:TextBox ID="txtMoTa" runat="server" CssClass="q-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            <asp:Button ID="btnLuuSPNhanh" runat="server" Text="LƯU VÀO DATABASE" CssClass="btn-apply" OnClick="btnLuuSPNhanh_Click" style="margin-top:10px; background:#1a73e8; padding:10px;" />
                            <asp:Label ID="lblThemTC" runat="server" ForeColor="#27ae60" Font-Size="12px" style="display:block; margin-top:10px; text-align:center;"></asp:Label>
                        </div>
                    </asp:View>

                    <%-- MÀN 5: TRA CỨU THEO STT VÀ HIỆN ẢNH ĐỂ SỬA --%>
                    <asp:View ID="vSuaSP" runat="server">
                        <div style="display:flex; align-items:center; padding:15px; background:#fff; border-bottom:1px solid #ddd;">
                            <asp:LinkButton ID="btnBackFromEdit" runat="server" OnClick="btnBackToMenu_Click" style="text-decoration:none; font-size:20px; margin-right:10px; color:#333;">←</asp:LinkButton>
                            <h3 style="margin:0; font-size:16px;">TRA CỨU & ÁP MÃ SALE</h3>
                        </div>
                        <div class="quick-form" style="height: 320px; overflow-y: auto;">
                            <div class="search-box">
                                <asp:TextBox ID="txtTimSTT" runat="server" CssClass="q-input" style="margin:0;" placeholder="Nhập STT (VD: 31)..."></asp:TextBox>
                                <asp:Button ID="btnTimSP" runat="server" Text="TÌM" CssClass="btn-search" OnClick="btnTimSP_Click" />
                            </div>
                            <asp:Label ID="lblTimKiemErr" runat="server" ForeColor="#e74c3c" Font-Size="12px" style="display:block; margin-bottom:10px;"></asp:Label>

                            <asp:Panel ID="pnEditForm" runat="server" Visible="false">
                                <div style="text-align:center; margin-bottom:15px;">
                                    <asp:Image ID="imgSuaPreview" runat="server" style="width:100px; height:100px; object-fit:cover; border-radius:8px; border:2px solid #FFD700; box-shadow: 0 4px 8px rgba(0,0,0,0.1);" />
                                </div>
                                <asp:HiddenField ID="hfSuaSTT" runat="server" />
                                <label style="font-size:11px; font-weight:bold;">Tên Sản Phẩm:</label>
                                <asp:TextBox ID="txtSuaTen" runat="server" CssClass="q-input" ReadOnly="true" style="background:#f9f9f9; color:#777;"></asp:TextBox>
                                <div style="display:flex; gap:10px;">
                                    <div style="flex:1;">
                                        <label style="font-size:11px; font-weight:bold;">Giá bán mới:</label>
                                        <asp:TextBox ID="txtSuaGia" runat="server" CssClass="q-input"></asp:TextBox>
                                    </div>
                                    <div style="flex:1;">
                                        <label style="font-size:11px; font-weight:bold;">Tồn kho:</label>
                                        <asp:TextBox ID="txtSuaTon" runat="server" CssClass="q-input"></asp:TextBox>
                                    </div>
                                </div>
                                <label style="font-size:11px; font-weight:bold; color:#c0392b;">Áp mã Sale / Gắn mác:</label>
                                <asp:DropDownList ID="ddlSuaMac" runat="server" CssClass="q-input" style="background:#fadbd8; font-weight:bold;">
                                    <asp:ListItem Value="">-- Bỏ mác, không có mác --</asp:ListItem>
                                    <asp:ListItem Value="HOT">🔥 SẢN PHẨM HOT</asp:ListItem>
                                    <asp:ListItem Value="-30%">⚡ GIẢM GIÁ 30%</asp:ListItem>
                                    <asp:ListItem Value="-50%">💥 XẢ KHO 50%</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="btnLuuSuaSP" runat="server" Text="CẬP NHẬT LÊN WEB" CssClass="btn-apply" OnClick="btnLuuSuaSP_Click" style="margin-top:10px; background:#27ae60; padding:10px;" />
                            </asp:Panel>
                        </div>
                    </asp:View>

                </asp:MultiView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>