using System.Windows;

namespace WpfApp2.Services
{
    public interface IMessageBoxService
    {
        void Show(string message, string caption, MessageBoxButton button);
    }
}