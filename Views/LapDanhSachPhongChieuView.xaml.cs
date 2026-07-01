using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cinema_Management_App.Views
{
    public partial class LapDanhSachPhongChieuView : Window
    {
        public LapDanhSachPhongChieuView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<Viewmodels.LapDanhSachPhongChieuViewmodel>();
            if (DataContext is Viewmodels.LapDanhSachPhongChieuViewmodel vm)
            {
                vm.RequestClose += result =>
                {
                    DialogResult = result;
                    Close();
                };
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}