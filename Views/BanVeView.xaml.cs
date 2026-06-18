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
    /// Interaction logic for BanVeView.xaml
    /// </summary>
    public partial class BanVeView : Window
    {
        public BanVeView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<Viewmodels.BanVeViewmodel>();
            if (DataContext is Viewmodels.BanVeViewmodel vm)
            {
                vm.RequestClose += () =>
                {
                    Application.Current.Dispatcher.Invoke(() => this.Close());
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
