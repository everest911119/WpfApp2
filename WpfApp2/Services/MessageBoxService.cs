using System.Windows;

namespace WpfApp2.Services
{
    public sealed class MessageBoxService : IMessageBoxService
    {
        public void Show(string message, string caption, MessageBoxButton button)
        {
            MessageBox.Show(message, caption, button);
        }
    }
}