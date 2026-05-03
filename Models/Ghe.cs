using System;
using Cinema_Management_App.Viewmodels;

namespace Cinema_Management_App.Models
{
    public class Ghe : BaseViewModel
    {
        private int _stt;
        private string _maGhe = string.Empty;
        private string _tenLoaiGhe = string.Empty;
        private decimal _donGia;
        public int STT
        {
            get => _stt;
            set { _stt = value; OnPropertyChanged(); }
        }
        public string MaGhe
        {
            get => _maGhe;
            set { _maGhe = value; OnPropertyChanged(); }
        }

        public string TenLoaiGhe
        {
            get => _tenLoaiGhe;
            set
            {
                _tenLoaiGhe = value;
                OnPropertyChanged();
                CapNhatDonGia();
            }
        }
        public decimal DonGia
        {
            get => _donGia;
            set { _donGia = value; OnPropertyChanged(); }
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