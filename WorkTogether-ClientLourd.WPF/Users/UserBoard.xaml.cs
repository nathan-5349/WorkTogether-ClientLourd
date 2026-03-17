using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;
using WorkTogether_ClientLourd.WPF.Core;
using WorkTogether_ClientLourd.WPF.Dashboard;

namespace WorkTogether_ClientLourd.WPF.Users
{
    /// <summary>
    /// Logique d'interaction pour UserBoard.xaml
    /// </summary>
    public partial class UserBoard : UserControl
    {
        private WorkTogetherContext _ctx;
        private List<User> _staffList;
        private List<User> _clientList;
        private User _selectedStaff;
        private User _selectedClient;

        private List<string> _staffDiscr = new()
        {
            "admin", "accountant", "support", "technician"
        };
        private List<string> _clientDiscr = new()
        {
            "particular", "company"
        };
        public UserBoard()
        {
            InitializeComponent();
            _ctx = new WorkTogetherContext();
            LoadStaff();
            LoadClients();
            ApplyRolePermissions();
        }
        private void LoadStaff(string search = "")
        {
            _staffList = _ctx.Users
                .Where(u => _staffDiscr.Contains(u.Discr))
                .Where(u => string.IsNullOrEmpty(search) ||
                            u.FirstName.Contains(search) ||
                            u.Name.Contains(search) ||
                            u.Email.Contains(search))
                .ToList();
            StaffGrid.ItemsSource = _staffList;
        }

        private void LoadClients(string search = "")
        {
            _clientList = _ctx.Users
                .Where(u => _clientDiscr.Contains(u.Discr))
                .Where(u => string.IsNullOrEmpty(search) ||
                            u.FirstName.Contains(search) ||
                            u.Name.Contains(search) ||
                            u.Email.Contains(search))
                .ToList();
            ClientGrid.ItemsSource = _clientList;
        }
        private void ApplyRolePermissions()
        {
            if (!Session.IsAdmin)
            {
                BtnAddClient.Visibility = Visibility.Collapsed;
                BtnEditClient.Visibility = Visibility.Collapsed;
                BtnDeleteClient.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchStaffBox_TextChanged(object sender, TextChangedEventArgs e)
    => LoadStaff(SearchStaffBox.Text);

        private void SearchClientBox_TextChanged(object sender, TextChangedEventArgs e)
            => LoadClients(SearchClientBox.Text);

        private void StaffGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _selectedStaff = StaffGrid.SelectedItem as User;

        private void ClientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _selectedClient = ClientGrid.SelectedItem as User;

        private void BtnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            var form = new UserForm(isStaff: true);
            // Affiche le formulaire dans le ContentControl du Dashboard
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = form;
        }

        private async void BtnEditStaff_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStaff == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner un membre du staff.");
                return;
            }
            var form = new UserForm(isStaff: true, user: _selectedStaff);
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = form;
        }

        private async void BtnDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStaff == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner un membre du staff.");
                return;
            }

            bool confirm = await CustomConfirm.Show(this, "Confirmation",
                $"Voulez-vous vraiment supprimer {_selectedStaff.FirstName} {_selectedStaff.Name} ?");

            if (confirm)
            {
                try
                {
                    var user = _ctx.Users
                        .Include(u => u.Administrator)
                        .Include(u => u.Accountant)
                        .Include(u => u.Support)
                        .Include(u => u.Technician)
                        .FirstOrDefault(u => u.Id == _selectedStaff.Id);

                    if (user != null)
                    {
                        _ctx.Users.Remove(user);
                        _ctx.SaveChanges();
                        LoadStaff(SearchStaffBox.Text);
                        await CustomMessage.Show(this, "Succès", "Membre du staff supprimé avec succès !");
                    }
                }
                catch (Exception ex)
                {
                    await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
                }
            }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var form = new UserForm(isStaff: false);
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = form;
        }

        private async void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner un utilisateur.");
                return;
            }
            var form = new UserForm(isStaff: false, user: _selectedClient);
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = form;
        }

        private async void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner un client.");
                return;
            }

            bool confirm = await CustomConfirm.Show(this, "Confirmation",
                $"Voulez-vous vraiment supprimer {_selectedClient.FirstName} {_selectedClient.Name} ?");

            if (confirm)
            {
                try
                {
                    var user = _ctx.Users
                        .Include(u => u.Particular)
                        .Include(u => u.Company)
                        .FirstOrDefault(u => u.Id == _selectedClient.Id);

                    if (user != null)
                    {
                        _ctx.Users.Remove(user);
                        _ctx.SaveChanges();
                        LoadStaff(SearchClientBox.Text);
                        await CustomMessage.Show(this, "Succès", "Client supprimé avec succès !");
                    }
                }
                catch (Exception ex)
                {
                    await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
                }
            }
        }
        private void MainContent_ReturnToBoard()
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new UserBoard();
        }
    }
}
