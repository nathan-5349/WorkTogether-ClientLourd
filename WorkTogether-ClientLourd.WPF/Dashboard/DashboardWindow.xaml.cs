using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WorkTogether_ClientLourd.WPF.Bays;
using WorkTogether_ClientLourd.WPF.Offers;
using WorkTogether_ClientLourd.WPF.Reservations;
using WorkTogether_ClientLourd.WPF.Users;

namespace WorkTogether_ClientLourd.WPF.Dashboard
{
    public partial class DashboardWindow : MetroWindow
    {
        public DashboardWindow()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        public void NavigateTo(object content, string section)
        {
            Button target = section switch
            {
                "users" => BtnUsers,
                "bays" => BtnBays,
                "offers" => BtnOffers,
                "réservations" => BtnReservations,
                "" => null
            };
            SetActiveButton(target);
            MainContent.Content = content;
        }
        private void InitializeDashboard()
        {
            UserNameBlock.Text = $"{Session.CurrentUser.FirstName} {Session.CurrentUser.Name}";
            UserRoleBlock.Text = Session.Role;

            BtnUsers.Visibility = (Session.IsAdmin || Session.IsAccountant) ? Visibility.Visible : Visibility.Collapsed;
            BtnBays.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnOffers.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnReservations.Visibility = (Session.IsAdmin || Session.IsAccountant) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Met le Tag "Active" sur le bouton cliqué et réinitialise les autres
        private void SetActiveButton(Button active)
        {
            BtnUsers.Tag = null;
            BtnBays.Tag = null;
            BtnOffers.Tag = null;
            BtnReservations.Tag = null;
            active.Tag = "Active";
        } 
        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnUsers);
            MainContent.Content = new UserBoard();
        }

        private void BtnBays_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnBays);
            MainContent.Content = new BayBoard();
        }

        private void BtnOffers_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnOffers);
            MainContent.Content = new OfferBoard();
        }

        private void BtnReservations_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnReservations);
            MainContent.Content = new ReservationBoard();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Session.Clear();
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}