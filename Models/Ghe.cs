using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Cinema_Management_App.Models
{
    public partial class Ghe : ObservableObject
    {
        [ObservableProperty]
        private int _sTT;

        [ObservableProperty]
        private string _maGhe = string.Empty;

        [ObservableProperty]
        private string _tenLoaiGhe = string.Empty;

        [ObservableProperty]
        private decimal _donGia;
        partial void OnTenLoaiGheChanged(string value)
        {
            CapNhatDonGia();
        }

        private void CapNhatDonGia()
        {
            switch (TenLoaiGhe)
            {
                case "A": DonGia = 150000; break;
                case "B": DonGia = 170000; break;
                case "C": DonGia = 200000; break;
                case "D": DonGia = 250000; break;
                case "E": DonGia = 300000; break;
                default: DonGia = 0; break;
            }
        }
    }
}