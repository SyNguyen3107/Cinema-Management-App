using System.Threading.Tasks;

namespace Cinema_Management_App.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title);
        void ShowWarning(string message, string title);
        void ShowError(string message, string title);

        // Useful for Yes/No confirmations later
        Task<bool> ShowConfirmationAsync(string message, string title);
    }
}