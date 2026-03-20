Ý Nghĩa Các Thư Mục
1. Models (Dữ liệu thuần túy)
Chứa gì: Các class C# đại diện cho các bảng trong Database (Ví dụ: Phim.cs, TheLoai.cs, NhanPhim.cs).

Quy tắc: Chỉ khai báo các thuộc tính (Properties: get, set). KHÔNG chứa bất kỳ đoạn code tính toán hay kết nối Database nào ở đây.

2. Views (Giao diện người dùng)
Chứa gì: Các file thiết kế màn hình .xaml (Ví dụ: ThemPhimView.xaml).

Quy tắc: Tuyệt đối không viết code logic vào file code-behind (.xaml.cs). File này phải trống rỗng. View chỉ làm nhiệm vụ hiển thị, mọi tương tác của người dùng sẽ được "Binding" qua ViewModel.

3. ViewModels (Logic)
Chứa gì: Các class quản lý trạng thái và hành vi cho View (Ví dụ: ThemPhimViewModel.cs).

Chức năng: Khi người dùng bấm nút trên View, ViewModel sẽ nhận lệnh, kiểm tra tính hợp lệ của dữ liệu (Validation). Sau đó, ViewModel sẽ gọi đến các Repositories để yêu cầu thêm/sửa/xóa/đọc dữ liệu.

4. Repositories (Quản lý nghiệp vụ dữ liệu)
Chứa gì: Các class chuyên đảm nhận thao tác CRUD (Create - Read - Update - Delete) cho từng đối tượng cụ thể (Ví dụ: PhimRepository.cs, TheLoaiRepository.cs).

Chức năng: Nằm giữa ViewModel và Services. Repository chứa các câu lệnh truy vấn (Query). Nó tổng hợp dữ liệu, tạo thành câu lệnh hoàn chỉnh rồi gửi xuống cho MySQLService để chạy.

5. Services (Giao tiếp hạ tầng & Hệ thống ngoài)
Chứa gì: Nơi giao tiếp trực tiếp với phần cứng hoặc hệ thống bên ngoài. Hiện tại cốt lõi là MySQLService.cs.

Chức năng: MySQLService là nơi duy nhất trực tiếp kết nối tới DB và thực thi các câu lệnh do Repository gửi xuống. Sau này có thể có thêm các Service khác (VD AuthenticationService,...)

6. Helpers (Công cụ tiện ích)
Chứa gì: Các hàm và đối tượng dùng chung, có thể gọi ở bất cứ đâu trong dự án.

Ví dụ: Hàm kiểm tra định dạng email, hàm format ngày tháng năm, hàm hiển thị hộp thoại thông báo (MessageBox), hoặc hàm mã hóa mật khẩu.

VIEW ↔️ VIEWMODEL ↔️ REPOSITORY ↔️ SERVICE (MySQLService) ↔️ DATABASE CLOUD

Mọi người đọc kỹ, ai được phân công làm ở tầng nào thì chỉ code đúng file ở thư mục đó.