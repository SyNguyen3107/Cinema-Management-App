using Cinema_Management_App.Viewmodels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cinema_Management_App.Views;

/// <summary>
/// Interaction logic for TiepNhanPhimView.xaml
/// </summary>
public partial class TiepNhanPhimView : Window
{
    public TiepNhanPhimView()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<TiepNhanPhimViewmodel>();
    }
    private void ComboBoxTenTheLoai_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxTenTheLoai.IsDropDownOpen = true;
    }
    private void ChuyenDuyetNhapSo(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");

        e.Handled = regex.IsMatch(e.Text);
    }
}
