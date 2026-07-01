using Microsoft.Extensions.DependencyInjection;
using System;
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
    /// Interaction logic for ChinhSuaPhongChieuView.xaml
    /// </summary>
    public partial class ChinhSuaPhongChieuView : Window
    {
        public ChinhSuaPhongChieuView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<Viewmodels.ChinhSuaPhongChieuViewmodel>();
            if (DataContext is Viewmodels.ChinhSuaPhongChieuViewmodel vm)
            {
                vm.RequestClose += result =>
                {
                    DialogResult = result;
                    Close();
                };
            }
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
