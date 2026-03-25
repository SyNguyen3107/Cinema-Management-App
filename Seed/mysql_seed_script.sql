-- Cinema Management App - MySQL Seed Script
-- Run this script in MySQL Workbench or Command Line to populate initial data

-- 1. CLEAR EXISTING DATA (Optional - Uncomment if you want a fresh start)
-- SET FOREIGN_KEY_CHECKS = 0;
-- TRUNCATE TABLE CHITIETTHELOAI;
-- TRUNCATE TABLE PHIM;
-- TRUNCATE TABLE NHANPHIM;
-- TRUNCATE TABLE THELOAI;
-- SET FOREIGN_KEY_CHECKS = 1;

-- 2. SEED NHANPHIM (Age Ratings)
INSERT INTO NHANPHIM (TenNhanPhim) VALUES 
('P (Mọi lứa tuổi)'),
('K (Dưới 13 tuổi với người giám hộ)'),
('T13 (Dưới 13 tuổi)'),
('T16 (Dưới 16 tuổi)'),
('T18 (Dưới 18 tuổi)');

-- 3. SEED THELOAI (Genres)
INSERT INTO THELOAI (TenTheLoai) VALUES 
('Hành động'),
('Hài kịch'),
('Kinh dị'),
('Tâm lý'),
('Viễn tưởng'),
('Hoạt hình'),
('Lãng mạn');

-- 4. SEED THAMSO (Parameters)
-- Assuming the table has a MaThamSo (AI) and SoLuongTheLoaiToiDa
-- INSERT INTO THAMSO (SoLuongTheLoaiToiDa) VALUES (5);

-- 5. SEED PHIM (Movies)
-- Note: Replace MaNhanPhim values with actual IDs if they differ
INSERT INTO PHIM (TenPhim, ThoiLuong, MaNhanPhim, TenDaoDien, TenDienVienChinh, NgayKhoiChieu) VALUES 
('Avengers: Endgame', 181, 1, 'Anthony Russo, Joe Russo', 'Robert Downey Jr., Chris Evans', '2019-04-26'),
('The Conjuring', 112, 5, 'James Wan', 'Vera Farmiga, Patrick Wilson', '2013-07-19'),
('Spider-Man: Across the Spider-Verse', 140, 1, 'Joaquim Dos Santos', 'Shameik Moore, Hailee Steinfeld', '2023-06-02'),
('The Dark Knight', 152, 3, 'Christopher Nolan', 'Christian Bale, Heath Ledger', '2008-07-18');

-- 6. SEED CHITIETTHELOAI (Movie-Genre Relationship)
-- Avengers: Endgame (Hành động, Viễn tưởng)
INSERT INTO CHITIETTHELOAI (MaPhim, MaTheLoai) VALUES (1, 1), (1, 5);
-- The Conjuring (Kinh dị)
INSERT INTO CHITIETTHELOAI (MaPhim, MaTheLoai) VALUES (2, 3);
-- Spider-Man (Hành động, Hoạt hình, Viễn tưởng)
INSERT INTO CHITIETTHELOAI (MaPhim, MaTheLoai) VALUES (3, 1), (3, 6), (3, 5);
-- The Dark Knight (Hành động, Tâm lý)
INSERT INTO CHITIETTHELOAI (MaPhim, MaTheLoai) VALUES (4, 1), (4, 4);

-- DONE
SELECT 'SEEDING COMPLETED' AS Message;
