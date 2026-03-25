<!-- Muốn chạy seed thì ae cho app.config vô file seed nha, đổi defaultdb thành QuanLyPhim -->


# Seed bình thường (bỏ qua nếu đã có data)
dotnet run --project SeedTool\SeedTool.csproj

# Xóa sạch DB rồi seed lại
dotnet run --project SeedTool\SeedTool.csproj -- --clear

# Seed + chạy test CRUD
dotnet run --project SeedTool\SeedTool.csproj -- --test
