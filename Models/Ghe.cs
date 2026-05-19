using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Cinema_Management_App.Models
{
    public partial class Ghe : ObservableObject
    {
        public string MaGhe { get; set; } = string.Empty;
        public string MaSoGhe { get; set; } = string.Empty;
        public string MaPhong { get; set; } = string.Empty;
        [ObservableProperty]
        private LoaiGhe _loaiGheDuocChon;

        public string MaLoaiGhe => LoaiGheDuocChon?.MaLoaiGhe;

        partial void OnLoaiGheDuocChonChanged(LoaiGhe value)
        {
            OnPropertyChanged(nameof(MaLoaiGhe));
        }
    }
}