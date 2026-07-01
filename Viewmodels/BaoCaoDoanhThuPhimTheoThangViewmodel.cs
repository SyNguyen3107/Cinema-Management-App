using Cinema_Management_App.DTOs;
using Cinema_Management_App.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_Management_App.Viewmodels
{
    public partial class BaoCaoDoanhThuPhimTheoThangViewmodel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly IPhimRepository _phimRepo;
        public List<int> DanhSachThang { get; } = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public List<int> DanhSachNam { get; } = Enumerable.Range(1900, DateTime.Now.Year - 1900 + 1).Reverse().ToList();

        [ObservableProperty]
        private int? _thangDuocChon;

        [ObservableProperty]
        private int? _namDuocChon;

        public Action? RequestClose;

        [ObservableProperty]
        private ObservableCollection<BaoCaoDoanhThuPhimDTO> _danhSachBaoCao = new();

        public BaoCaoDoanhThuPhimTheoThangViewmodel(IPhimRepository phimRepo, IDialogService dialogService)
        {
            _phimRepo = phimRepo;
            _dialogService = dialogService;

            ThangDuocChon = DateTime.Now.Month;
            NamDuocChon = DateTime.Now.Year;
        }

        [RelayCommand]
        private async Task LapBaoCaoAsync()
        {
            if (!KiemTraBaoCaoHopLe())
                return;

            try
            {
                var baocao = await _phimRepo.GetMoviesRevenueReportsByMonthYear(ThangDuocChon!.Value, NamDuocChon!.Value);

                DanhSachBaoCao = new ObservableCollection<BaoCaoDoanhThuPhimDTO>(baocao);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi lập báo cáo.\nChi tiết lỗi: {ex.Message}", "Lỗi");
            }
        }

        private bool KiemTraBaoCaoHopLe()
        {
            if (ThangDuocChon == null)
            {
                _dialogService.ShowError("Vui lòng chọn tháng để lập báo cáo.", "Lỗi");
                return false;
            }
            if (NamDuocChon == null)
            {
                _dialogService.ShowError("Vui lòng chọn năm để lập báo cáo.", "Lỗi");
                return false;
            }
            return true;
        }

        [RelayCommand]
        private void Thoat()
        {
            RequestClose?.Invoke();
        }
    }
}