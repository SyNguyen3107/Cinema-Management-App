using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cinema_Management_App.Viewmodels
{
    // Lớp này thực thi interface INotifyPropertyChanged để hỗ trợ Binding dữ liệu
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Hàm này sẽ thông báo cho UI biết thuộc tính nào vừa thay đổi
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}