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

namespace Cinema_Management_App.Viewmodels
{
    /// <summary>
    /// Interaction logic for TiepNhapPhimView.xaml
    /// </summary>
    public partial class TiepNhapPhimView : Window
    {
        public TiepNhapPhimView()
        {
            InitializeComponent();
        }

        private void ButtonThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
