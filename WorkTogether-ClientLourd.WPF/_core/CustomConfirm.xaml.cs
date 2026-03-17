using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace WorkTogether_ClientLourd.WPF.Core
{
    public partial class CustomConfirm : CustomDialog
    {
        private TaskCompletionSource<bool> _tcs;

        public CustomConfirm(string title, string message)
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
        }

        private async void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            var window = this.TryFindParent<MetroWindow>();
            if (window != null)
                await window.HideMetroDialogAsync(this);
            _tcs?.SetResult(true);
        }

        private async void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            var window = this.TryFindParent<MetroWindow>();
            if (window != null)
                await window.HideMetroDialogAsync(this);
            _tcs?.SetResult(false);
        }

        public static async Task<bool> Show(UIElement element, string title, string message)
        {
            var window = Window.GetWindow(element) as MetroWindow;
            if (window == null) return false;

            var dialog = new CustomConfirm(title, message);
            var tcs = new TaskCompletionSource<bool>();
            dialog._tcs = tcs;

            await window.ShowMetroDialogAsync(dialog);
            return await tcs.Task;
        }
    }
}