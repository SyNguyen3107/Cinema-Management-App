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
    /// <summary>
    /// Interaction logic for LapDanhSachPhongChieuView.xaml
    /// </summary>
    public partial class LapDanhSachPhongChieuView : Window
    {
        public LapDanhSachPhongChieuView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<Viewmodels.LapDanhSachPhongChieuViewmodel>();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
