using MahApps.Metro.Controls;
using System.Windows;
namespace WorkTogether_ClientLourd.WPF.Dashboard;
using WorkTogether_ClientLourd.WPF.Users;


public partial class DashboardWindow : MetroWindow
{
    public DashboardWindow()
    {
        InitializeComponent();
        InitializeDashboard();
    }

    private void InitializeDashboard()
    {
        UserNameBlock.Text = $"{Session.CurrentUser.FirstName} {Session.CurrentUser.Name}";
        UserRoleBlock.Text = Session.Role;

        BtnUsers.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        BtnBays.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        BtnOffers.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        BtnReservations.Visibility = (Session.IsAdmin || Session.IsAccountant) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BtnUsers_Click(object sender, RoutedEventArgs e)
    {
        MainContent.Content = new UserBoard();
    }
    private void BtnBays_Click(object sender, RoutedEventArgs e)
    {

    }
    private void BtnOffers_Click(object sender, RoutedEventArgs e)
    {

    }
    private void BtnReservations_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.Clear();
        var login = new LoginWindow();
        login.Show();
        this.Close();
    }
}