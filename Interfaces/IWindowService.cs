using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cinema_Management_App.Interfaces
{
    public interface IWindowService
    {
        void Register<TViewModel, TView>()
        where TView : Window;

        bool? ShowDialog<TViewModel>()
        where TViewModel : class;
    }
}
