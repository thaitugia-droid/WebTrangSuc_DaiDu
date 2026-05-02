<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="WebTrangSuc_DaiDu.Admin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trang Quản Trị Hệ Thống - Ollama AI Inside</title>
    <link href="admin.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <script type="text/javascript">
        function ThongBaoTuCodeBehind(loai, tieuDe, noiDung) {
            Swal.fire({
                title: tieuDe,
                text: noiDung,
                icon: loai,
                confirmButtonColor: loai === 'success' ? '#27ae60' : '#e74c3c'
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

        <div class="admin-wrapper">
            
            <div style="text-align: right; margin-bottom: 20px;">
                <asp:LinkButton ID="btnDangXuat" runat="server" OnClick="btnDangXuat_Click" 
                    style="background-color: #e74c3c; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; transition: 0.3s;">
                    <i class="fa-solid fa-right-from-bracket"></i> ĐĂNG XUẤT
                </asp:LinkButton>
            </div>
            <a href="index.aspx" class="btn-ve-trang-chu">
               <i class="fa-solid fa-house"></i> QUAY VỀ TRANG CHỦ
             </a>
            
            <asp:Label ID="lblThongBao" runat="server" ForeColor="Red" Font-Bold="true" Font-Size="Large"></asp:Label>
            <br /><br />
            <div style="background: #121212; padding: 25px; border-radius: 10px; border: 1px dashed #C19A6B; text-align: center; margin-bottom: 30px; box-shadow: 0 4px 15px rgba(193, 154, 107, 0.1);">
                <h3 style="color: #C19A6B; margin-top: 0; font-family: 'Playfair Display', serif; letter-spacing: 1px;">
                    <i class="fa-solid fa-wand-magic-sparkles"></i> QUẢN LÝ GIAO DIỆN WEBSITE
                </h3>
                <p style="color: #B0B0B0; margin-bottom: 20px;">Giao diện đang được bật: <strong style="color: #e74c3c; text-transform: uppercase; border-bottom: 1px solid #e74c3c; padding-bottom: 2px;"><%= Application["CurrentTheme"] ?? "banthuong" %></strong></p>
                
                <div style="display: flex; justify-content: center; gap: 20px; flex-wrap: wrap;">
                    <asp:LinkButton ID="btnThemeThuong" runat="server" OnClick="btnThemeThuong_Click" 
                        style="background-color: #1a1a1a; color: #C19A6B; padding: 12px 25px; border: 1px solid #C19A6B; border-radius: 5px; text-decoration: none; font-weight: bold; transition: all 0.3s; box-shadow: 0 4px 6px rgba(0,0,0,0.3);">
                        <i class="fa-solid fa-moon"></i> NGÀY THƯỜNG
                    </asp:LinkButton>
                    
                    <asp:LinkButton ID="btnThemeGiangSinh" runat="server" OnClick="btnThemeGiangSinh_Click" 
                        style="background-color: #c0392b; color: white; padding: 12px 25px; border: 1px solid #c0392b; border-radius: 5px; text-decoration: none; font-weight: bold; transition: all 0.3s; box-shadow: 0 4px 6px rgba(0,0,0,0.3);">
                        <i class="fa-solid fa-sleigh"></i> LỄ GIÁNG SINH
                    </asp:LinkButton>
                    
                    <asp:LinkButton ID="btnTheme304" runat="server" OnClick="btnTheme304_Click" 
                        style="background-color: #2980b9; color: white; padding: 12px 25px; border: 1px solid #2980b9; border-radius: 5px; text-decoration: none; font-weight: bold; transition: all 0.3s; box-shadow: 0 4px 6px rgba(0,0,0,0.3);">
                        <i class="fa-solid fa-star"></i> LỄ 30/4 & 1/5
                    </asp:LinkButton>
                </div>
            </div>

            <div class="thong-ke-container">
                <h3 class="thong-ke-title"><i class="fa-solid fa-chart-pie"></i> BÁO CÁO DOANH THU TỔNG QUAN</h3>
                <div class="thong-ke-cards">
                    <div class="tk-card">
                        <h4><i class="fa-solid fa-sack-dollar"></i> Tổng Doanh Thu Toàn Hệ Thống</h4>
                        <p class="tk-number tk-gold">
                            <asp:Label ID="lblTongDoanhThu" runat="server" Text="0 VNĐ"></asp:Label>
                        </p>
                    </div>
                    <div class="tk-card">
                        <h4><i class="fa-solid fa-file-invoice"></i> Tổng Số Đơn Hàng</h4>
                        <p class="tk-number">
                            <asp:Label ID="lblTongDon" runat="server" Text="0"></asp:Label> <span style="font-size: 14px; color:#aaa;">đơn</span>
                        </p>
                    </div>
                </div>

                <h4 style="color: #A86273; margin-bottom: 10px;">Chi tiết theo tháng:</h4>
                <asp:GridView ID="gvThongKeThang" runat="server" AutoGenerateColumns="False" CssClass ="myGridView" GridLines="None" EnableViewState="false">
                    <Columns>
                        <asp:BoundField DataField="Thang" HeaderText="Tháng" />
                        <asp:BoundField DataField="Nam" HeaderText="Năm" />
                        <asp:BoundField DataField="SoDonHang" HeaderText="Số Đơn Hàng" />
                       <asp:BoundField DataField="DoanhThuThang" HeaderText="Tổng Doanh Thu" DataFormatString="{0:N0} VNĐ" ItemStyle-ForeColor="#C19A6B" ItemStyle-Font-Bold="true" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-data-msg">Hệ thống chưa có dữ liệu doanh thu.</div>
                    </EmptyDataTemplate>
                </asp:GridView>

                <div style="background: #1a1a1a; padding: 20px; border-radius: 10px; border: 1px dashed #A86273; margin-top: 20px; text-align: center;">
                    <h3 style="color: #C19A6B; margin-top:0;"><i class="fa-solid fa-brain"></i> PHÒNG HỌP CHIẾN LƯỢC AI</h3>
                    <button type="button" onclick="HoiYKiengAI();" style="background-color: #A86273; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; font-size: 16px;">
                        📊 AI Phân Tích & Đề Xuất Khuyến Mãi
                    </button>
                    
                    <div id="vungKetQuaAI" style="display:none; margin-top:20px; background: #121212; padding: 15px; border-radius: 5px;">
                        <p style="font-style: italic; color: #E0E0E0; font-size: 16px;">" <span id="txtAINhanXet"></span> "</p>
                        <div style="margin-top:15px; border-top: 1px solid #333; padding-top: 15px;">
                            <span style="color: #aaa;">Mức giảm giá đề xuất: </span>
                            <b id="txtAIPercent" style="color: #e74c3c; font-size: 28px;">0</b> <b style="color: #e74c3c; font-size: 28px;">%</b>
                        </div>
                        <button type="button" onclick="KichHoatGiamGiaGiongAI();" style="margin-top:15px; background:#27ae60; color:white; border:none; padding:8px 20px; border-radius:3px; cursor:pointer; font-weight: bold;">
                            <i class="fa-solid fa-check"></i> Áp dụng cho Giỏ Hàng
                        </button>
                    </div>
                </div>
            </div>

            <div class="admin-top-section" style="margin-top: 30px;">
                <div class="admin-form-col">
                    <h3>💎 Quản Lý Sản Phẩm💎</h3>
                    <div class="form-group"><label>Mã SP:</label><asp:TextBox ID="txtMaSP" runat="server" CssClass="form-input" placeholder="Mã xịn do AI tạo..."></asp:TextBox></div>
                    <div class="form-group"><label>Tên SP:</label><asp:TextBox ID="txtTenSP" runat="server" CssClass="form-input" placeholder="Tên do AI dịch mượt..."></asp:TextBox></div>
                    <div class="form-group"><label>Mã DM:</label><asp:TextBox ID="txtMaDM" runat="server" CssClass="form-input" placeholder="AI điền đúng 'DC1', 'NV4'..."></asp:TextBox></div>
                    <div class="form-group"><label>Giá Bán:</label><asp:TextBox ID="txtGiaBan" runat="server" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-group"><label>Số Lượng:</label><asp:TextBox ID="txtSoLuong" runat="server" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-group"><label>Mô Tả:</label><asp:TextBox ID="txtMoTa" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-group"><label>Hình Ảnh:</label><asp:FileUpload ID="fileUpHinhAnh" runat="server" /></div>
                    
                    <div style="margin-bottom: 15px;">
                        <button type="button" id="btnAITuDien" onclick="GoiAIPhoTro();" style="background-color: #C19A6B; color: white; padding: 10px 15px; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; width:100%;">
                            ✨ Trợ Lý Ollama AI (No-Internet Inside)
                        </button>
                        <span id="aiLoading" style="display:none; color: #e74c3c; font-weight: bold; margin-top:5px; text-align:center;">
                            <i class="fa-solid fa-spinner fa-spin"></i> Trợ lý đang soi ảnh, sếp đợi xíu nhé...
                        </span>
                    </div>

                    <asp:Button ID="btnThemSP" runat="server" Text="Thêm Sản Phẩm" OnClick="btnThemSP_Click" CssClass="btn-submit" />
                </div>
                
                <div class="admin-category-col">
                    <h3>📋 QUẢN LÝ DANH MỤC</h3>
                    <p>(Xem mã ở đây để điền cho chuẩn)</p>
                    <asp:GridView ID="gvDanhMucAdmin" runat="server" AutoGenerateColumns="False" Width="100%" 
                        DataKeyNames="MaDM"
                        OnRowEditing="gvDanhMucAdmin_RowEditing"
                        OnRowCancelingEdit="gvDanhMucAdmin_RowCancelingEdit"
                        OnRowUpdating="gvDanhMucAdmin_RowUpdating"
                        OnRowDeleting="gvDanhMucAdmin_RowDeleting"
                        GridLines="Horizontal" CellPadding="10" style="background: white; border-radius: 5px; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.05); border-collapse: collapse; margin-bottom: 20px;">
                        <HeaderStyle BackColor="#A86273" ForeColor="White" Font-Bold="True" Height="40px" />
                        <RowStyle BorderColor="#eeeeee" BorderWidth="1px" Height="40px" />
                        <Columns>
                            <asp:BoundField DataField="MaDM" HeaderText="Mã DM">
                                <ItemStyle Font-Bold="True" ForeColor="#e74c3c" Width="30%" HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TenDM" HeaderText="Tên Danh Mục" />
                            <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" 
                                EditText="✏️ Sửa" UpdateText="💾 Lưu" CancelText="❌ Hủy" DeleteText="🗑️ Xóa">
                                <ItemStyle HorizontalAlign="Center" Width="30%" />
                            </asp:CommandField>
                        </Columns>
                    </asp:GridView>

                    <div class="khu-vuc-them-dm">
                        <h4 class="tieu-de-them-dm">➕ THÊM DANH MỤC MỚI</h4>
                        <div class="dm-group"><asp:TextBox ID="txtThemMaDM" runat="server" CssClass="dm-input" placeholder="Mã DM (VD: NH5)"></asp:TextBox></div>
                        <div class="dm-group"><asp:TextBox ID="txtThemTenDM" runat="server" CssClass="dm-input" placeholder="Tên Danh Mục (VD: Nhẫn Bạc)"></asp:TextBox></div>
                        <asp:Button ID="btnThemDM" runat="server" Text="Thêm Danh Mục Ngay" OnClick="btnThemDM_Click" CssClass="btn-submit-dm" />
                    </div>
                </div>
            </div>

            <div class="grid-container" style="padding: 20px; margin-top: 20px;">
                <h3 style="color: #A86273;">📦 DANH SÁCH SẢN PHẨM HIỆN CÓ</h3>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="MaSP"
                    OnRowDeleting="GridView1_RowDeleting"
                    OnRowEditing="GridView1_RowEditing"
                    OnRowUpdating="GridView1_RowUpdating"
                    OnRowCancelingEdit="GridView1_RowCancelingEdit"
                    CssClass="myGridView" GridLines="None" Width="100%">
                    <HeaderStyle BackColor="#A86273" ForeColor="White" />
                    <Columns>
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" HeaderText="Hành động" 
                             EditText="✏️ Sửa" UpdateText="💾 Lưu" CancelText="❌ Hủy" DeleteText="🗑️ Xóa"/>
                        <asp:BoundField DataField="MaSP" HeaderText="Mã SP" ReadOnly="True" />
                        <asp:BoundField DataField="TenSP" HeaderText="Tên Sản Phẩm" />
                        <asp:BoundField DataField="MaDM" HeaderText="Mã DM" />
                        <asp:BoundField DataField="GiaBan" HeaderText="Giá Bán" />
                        <asp:BoundField DataField="SoLuongTon" HeaderText="Số Lượng" />
                        <asp:TemplateField HeaderText="Hình Ảnh">
                            <ItemTemplate>
                            <img src='<%# ResolveUrl("~/IMAGES/Sanpham/" + Eval("HinhAnh")) %>' loading="lazy" class="img-sp-nho" style="width:50px; height:50px; object-fit:cover;" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblHinhAnhCu" runat="server" Text='<%# Eval("HinhAnh") %>' Visible="false"></asp:Label>
                                <asp:FileUpload ID="fileUpHinhAnhEdit" runat="server" CssClass="file-up-edit" />
                                <div class="ghi-chu-anh">(Để trống nếu muốn giữ ảnh cũ)</div>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="grid-container" style="padding: 20px; margin-top: 20px;">
                
                <div class="order-management-header">
                    <div class="order-title-group">
                        <i class="fa-solid fa-cart-shopping" style="font-size: 20px; color: #A86273;"></i>
                        <h3 class="grid-title-order" style="border:none; padding:0; margin:0;">DANH SÁCH ĐƠN HÀNG & DUYỆT ĐƠN</h3>
                        <span class="live-indicator"><span class="dot"></span> RADAR LIVE</span>
                    </div>

                    <div class="order-toolbar">
                        <div class="search-box-v5">
                            <i class="fa-solid fa-magnifying-glass"></i>
                            <asp:TextBox ID="txtTimDonHang" runat="server" placeholder="Nhập Mã ĐH hoặc SĐT..."></asp:TextBox>
                        </div>
                        <asp:Button ID="btnTimDonHang" runat="server" Text="TRA CỨU" OnClick="btnTimDonHang_Click" CssClass="btn-search-v5" />
                        <asp:LinkButton ID="btnLamMoiDon" runat="server" OnClick="btnLamMoiDon_Click" CssClass="btn-refresh-v5" title="Làm mới danh sách">
                            <i class="fa-solid fa-arrows-rotate"></i>
                        </asp:LinkButton>
                    </div>
                </div>
                
                <asp:GridView ID="gvDonHang" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="MaHD"
                    OnRowEditing="gvDonHang_RowEditing"
                    OnRowCancelingEdit="gvDonHang_RowCancelingEdit"
                    OnRowUpdating="gvDonHang_RowUpdating"
                    CssClass="myGridView" OnRowCommand="gvHoaDon_RowCommand" GridLines="None" Width="100%">
                    <HeaderStyle BackColor="#A86273" ForeColor="White" />
                    <Columns>
                        <asp:BoundField DataField="MaHD" HeaderText="Mã HĐ" ReadOnly="true" ItemStyle-Font-Bold="true" />
                        <asp:BoundField DataField="MaKH" HeaderText="Mã KH" ReadOnly="true" />
                        <asp:BoundField DataField="NgayDat" HeaderText="Ngày Đặt" ReadOnly="true" />
                        <asp:BoundField DataField="TongTien" HeaderText="Tổng Tiền" ReadOnly="true" DataFormatString="{0:N0} VNĐ" ItemStyle-ForeColor="#e74c3c" ItemStyle-Font-Bold="true" />
                        
                        <asp:TemplateField HeaderText="Trạng Thái">
                            <ItemTemplate>
                                <span style="font-weight:bold; color:#A86273;"><%# Eval("TrangThai") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlTrangThai" runat="server" style="padding: 5px; border-radius: 4px;">
                                    <asp:ListItem Value="Chờ duyệt">Chờ duyệt</asp:ListItem>
                                    <asp:ListItem Value="Đang giao">Đang giao</asp:ListItem>
                                    <asp:ListItem Value="Hoàn thành">Hoàn thành</asp:ListItem>
                                    <asp:ListItem Value="Đã hủy">Đã hủy</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="true" EditText="<i class='fa-solid fa-pen-to-square'></i> Duyệt" UpdateText="💾 Lưu" CancelText="❌ Hủy" HeaderText="Duyệt Đơn" />

                        <asp:TemplateField HeaderText="Xem Hàng">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnXemChiTiet" runat="server" CommandName="XemChiTiet" CommandArgument='<%# Eval("MaHD") %>' CssClass="btn-gold" style="background-color: #C19A6B; color: white; padding: 5px 10px; border-radius: 5px; text-decoration: none;">
                                    <i class="fa-solid fa-eye"></i> Xem
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <asp:Panel ID="pnlChiTiet" runat="server" CssClass="chi-tiet-don-hang-container" Visible="false" style="background:#121212; padding:20px; border-radius:8px; margin-top:20px;">
                <h3 style="color:#C19A6B;"><i class="fa-solid fa-box-open"></i> Chi Tiết Hóa Đơn</h3>
                <asp:GridView ID="gvChiTietHoaDon" runat="server" AutoGenerateColumns="False" CssClass="grid-chi-tiet" GridLines="None" Width="100%" style="color:white; text-align:center;" EnableViewState="false">
                    <HeaderStyle BackColor="#050505" ForeColor="#C19A6B" Height="40px" />
                    <Columns>
                        <asp:BoundField DataField="MaSP" HeaderText="Mã SP" />
                        <asp:BoundField DataField="TenSP" HeaderText="Tên Sản Phẩm" />
                        <asp:TemplateField HeaderText="Hình Ảnh">
                            <ItemTemplate>
                                <img src='<%# ResolveUrl("~/IMAGES/Sanpham/" + Eval("HinhAnh")) %>' loading="lazy" alt="Ảnh SP" class="img-chi-tiet" style="width:60px; height:60px; object-fit:cover; border-radius:5px;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="SoLuong" HeaderText="Số Lượng" />
                        <asp:BoundField DataField="Dongia" HeaderText="Đơn Giá" DataFormatString="{0:N0} VNĐ" />
                        <asp:BoundField DataField="ThanhTien" HeaderText="Thành Tiền" DataFormatString="{0:N0} VNĐ" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>

            <div class="grid-container" style="padding: 20px; margin-top: 40px; border-top: 3px dashed #A86273;">
                <h3 style="color: #A86273;"><i class="fa-solid fa-users-gear"></i> QUẢN LÝ TÀI KHOẢN & PHÂN QUYỀN</h3>
                <p style="margin-bottom: 15px; color: #666;">Chỉnh sửa quyền: Cấp quyền <b>1</b> để làm Admin, để <b>Trống</b> để làm Khách Hàng.</p>
                <asp:GridView ID="gvTaiKhoan" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="TaiKhoan"
                    OnRowEditing="gvTaiKhoan_RowEditing"
                    OnRowCancelingEdit="gvTaiKhoan_RowCancelingEdit"
                    OnRowUpdating="gvTaiKhoan_RowUpdating"
                    OnRowDeleting="gvTaiKhoan_RowDeleting"
                    CssClass="myGridView" GridLines="None" Width="100%">
                    <HeaderStyle BackColor="#2c3e50" ForeColor="White" />
                    <Columns>
                        <asp:BoundField DataField="TaiKhoan" HeaderText="Tên Tài Khoản" ReadOnly="True" ItemStyle-Font-Bold="true" />
                        <asp:BoundField DataField="HoTen" HeaderText="Họ Tên Khách" ReadOnly="True" />
                        <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="True" />
                        <asp:BoundField DataField="HangThanhVien" HeaderText="Hạng VIP" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#e74c3c" />
                        <asp:BoundField DataField="TongChiTieu" HeaderText="Tổng Chi Tiêu" DataFormatString="{0:N0} đ" ItemStyle-Font-Bold="true" ItemStyle-ForeColor="#C19A6B" />

                        <asp:TemplateField HeaderText="Quyền Truy Cập">
                            <ItemTemplate>
                                <%# Eval("Quyen").ToString() == "1" ? "<span style='color:red; font-weight:bold;'><i class='fa-solid fa-crown'></i> Admin</span>" : "<span style='color:green;'>Khách Hàng</span>" %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlQuyen" runat="server" style="padding: 5px; border-radius: 4px;">
                                    <asp:ListItem Value="">Khách Hàng</asp:ListItem>
                                    <asp:ListItem Value="1">Admin (Có quyền Q/Lý)</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" HeaderText="Cấp Quyền"
                             EditText="<i class='fa-solid fa-user-pen'></i> Đổi Quyền" UpdateText="💾 Lưu" CancelText="❌ Hủy" DeleteText="🗑️ Xóa TK"/>
                    </Columns>
                </asp:GridView>
            </div>

        </div>
    </form>

    <script type="text/javascript">
        // ==========================================
        // HÀM 1: AI MẮT THẦN SOI ẢNH ĐIỀN FORM 
        // ==========================================
        function GoiAIPhoTro() {
            const fileInput = document.getElementById('<%= fileUpHinhAnh.ClientID %>');
            if (!fileInput.files || fileInput.files.length === 0) {
                Swal.fire({
                    title: 'Khoan đã sếp!',
                    text: 'Tải ảnh trang sức lên trước thì AI mới có cái mà soi chứ!',
                    icon: 'warning',
                    confirmButtonColor: '#C19A6B'
                });
                return;
            }

            const file = fileInput.files[0];
            const reader = new FileReader();

            document.getElementById("btnAITuDien").disabled = true;
            document.getElementById("btnAITuDien").style.opacity = "0.5";
            document.getElementById("aiLoading").style.display = "block";
            reader.onloadend = function () {
                const base64String = reader.result.split(',')[1];
                PageMethods.AISoiAnh(base64String, function (response) {
                    document.getElementById("btnAITuDien").disabled = false;
                    document.getElementById("btnAITuDien").style.opacity = "1";
                    document.getElementById("aiLoading").style.display = "none";

                    try {
                        let cleanJson = response.replace(/```json/g, "").replace(/```/g, "").trim();
                        const data = JSON.parse(cleanJson);

                        if (data.Loi) {
                            Swal.fire({ title: 'AI Báo Lỗi Local!', text: data.Loi, icon: 'error', confirmButtonColor: '#e74c3c' });
                            return;
                        }

                        let maDanhMuc = data.MaDM || 'DAQUY';
                        document.getElementById('<%= txtMaDM.ClientID %>').value = maDanhMuc;

                        let prefix = maDanhMuc.replace(/[0-9]/g, '');
                        if (!prefix) prefix = "SP";
                        let randomCode = prefix + "_" + Math.floor(1000 + Math.random() * 9000);
                        document.getElementById('<%= txtMaSP.ClientID %>').value = randomCode;

                        document.getElementById('<%= txtTenSP.ClientID %>').value = data.TenSP || '';
                        let desc = data.MoTa || '';
                        let formatMoTa = desc.split('|').map(item => '- ' + item.trim()).join('\n');
                        document.getElementById('<%= txtMoTa.ClientID %>').value = formatMoTa;
                        
                        document.getElementById('<%= txtGiaBan.ClientID %>').value = "250000";
                        document.getElementById('<%= txtSoLuong.ClientID %>').value = "100";

                        Swal.fire({
                            title: 'Soi Ảnh Thành Công!',
                            text: 'Trợ lý Ollama đã điền form xong! Sếp thấy mã SP "' + randomCode + '" ngầu chưa?',
                            icon: 'success',
                            confirmButtonColor: '#27ae60'
                        });
                    } catch (e) {
                        console.error("Lỗi parse JSON:", response);
                        Swal.fire({
                            title: 'Oops... Hơi lệch tí!',
                            text: 'Dạ AI phân tích được rồi nhưng định dạng bị lệch, sếp bấm thử lại nha!',
                            icon: 'warning',
                            confirmButtonColor: '#f39c12'
                        });
                    }
                }, function(err) {
                    document.getElementById("btnAITuDien").disabled = false;
                    document.getElementById("btnAITuDien").style.opacity = "1";
                    document.getElementById("aiLoading").style.display = "none";
                    Swal.fire({ title: 'Lỗi Hệ Thống!', text: err.get_message(), icon: 'error', confirmButtonColor: '#e74c3c' });
                });
            };
            reader.readAsDataURL(file);
        }

        // ==========================================
        // 2. AI GIÁM ĐỐC CHIẾN LƯỢC
        // ==========================================
        function HoiYKiengAI() {
            let doanhThuThangNay = "0 VNĐ";
            try {
                const grid = document.getElementById('<%= gvThongKeThang.ClientID %>');
                if(grid && grid.rows.length > 1) {
                    doanhThuThangNay = grid.rows[1].cells[3].innerText;
                } else {
                    doanhThuThangNay = document.getElementById('<%= lblTongDoanhThu.ClientID %>').innerText;
                }
            } catch(e) {
                doanhThuThangNay = document.getElementById('<%= lblTongDoanhThu.ClientID %>').innerText;
            }

            document.getElementById("vungKetQuaAI").style.display = "none";

            const btnAI = event.target;
            const originalText = btnAI.innerText;
            btnAI.innerText = "⌛ Chờ em xíu sếp...";
            btnAI.disabled = true;
            PageMethods.AIPhanTichChienLuoc(doanhThuThangNay, function (res) {
                btnAI.innerText = originalText;
                btnAI.disabled = false;
                try {
                    let cleanJson = res.replace(/```json/g, "").replace(/```/g, "").trim();
                    const data = JSON.parse(cleanJson);
                    document.getElementById("vungKetQuaAI").style.display = "block";
                    document.getElementById("txtAINhanXet").innerText = data.NhanXet;
                    document.getElementById("txtAIPercent").innerText = data.PhanTramGiam;
                } catch (e) {
                    Swal.fire({ title: 'Lỗi Định Dạng', text: 'AI trả kết quả chưa chuẩn JSON chiến lược, sếp bấm lại nhé!', icon: 'warning', confirmButtonColor: '#f39c12' });
                }
            }, function (err) {
                btnAI.innerText = originalText;
                btnAI.disabled = false;
                Swal.fire({ title: 'Lỗi Local', text: err.get_message(), icon: 'error', confirmButtonColor: '#e74c3c' });
            });
        }

        function KichHoatGiamGiaGiongAI() {
            const phanTram = document.getElementById("txtAIPercent").innerText;
            PageMethods.ApDungKhuyenMaiToanHeThong(phanTram, function (res) {
                Swal.fire({ title: 'Đã Phát Lệnh!', text: res, icon: 'success', confirmButtonColor: '#27ae60' });
            });
        }

        // ==========================================
        // 🔥 HỆ THỐNG RADAR QUÉT ĐƠN HÀNG REAL-TIME 🔥
        // ==========================================
        setInterval(function() {
            PageMethods.KiemTraDonHangMoi(function(soDonMoi) {
                if (soDonMoi !== "0" && soDonMoi !== "") {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'info',
                        title: '🔔 TING TING! CÓ ĐƠN MỚI!',
                        text: 'Sếp vừa nhận thêm ' + soDonMoi + ' đơn hàng chốt thành công!',
                        showConfirmButton: false,
                        timer: 8000,
                        background: '#0B1D51',
                        color: '#00FFD1'
                    });
                    
                    var audio = new Audio('https://www.soundjay.com/buttons/sounds/button-09.mp3');
                    audio.play().catch(function(e) {});

                    document.getElementById('<%= btnLamMoiDon.ClientID %>').click();
                }
            });
        }, 5000);
    </script>
</body>
</html>