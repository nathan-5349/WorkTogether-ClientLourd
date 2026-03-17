using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace WorkTogether_ClientLourd.WPF.Core
{
    public partial class CustomMessage : CustomDialog
    {
        public CustomMessage(string title, string message)
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
        }

        private async void BtnOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = this.TryFindParent<MetroWindow>();
            if (window != null)
                await window.HideMetroDialogAsync(this);
        }

        public static async Task Show(UIElement element, string title, string message)
        {
            var window = Window.GetWindow(element) as MetroWindow;
            if (window != null)
                await window.ShowMetroDialogAsync(new CustomMessage(title, message));
        }
    }
}