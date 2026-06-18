using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Cinema_Management_App.Models
{
    public partial class Ghe : ObservableObject
    {
        public string MaGhe { get; set; } = string.Empty;
        public string MaSoGhe { get; set; } = string.Empty;
        public string MaPhong { get; set; } = string.Empty;
        public string MaLoaiGhe { get; set; } = string.Empty;

        private LoaiGhe? _loaiGheDuocChon;

        public LoaiGhe? LoaiGheDuocChon
        {
            get => _loaiGheDuocChon;
            set
            {
                if (SetProperty(ref _loaiGheDuocChon, value))
                {
                    MaLoaiGhe = value?.MaLoaiGhe ?? string.Empty;
                }
            }
        }
    }
}