using BCrypt.Net;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;
using WorkTogether_ClientLourd.WPF.Dashboard;
using WorkTogether_ClientLourd.WPF.Core;

namespace WorkTogether_ClientLourd.WPF.Users
{
    public partial class UserForm : UserControl
    {
        private WorkTogetherContext _ctx;
        private User _user;
        private bool _isStaff;
        private bool _isEditMode;

        // Appelé depuis UserBoard
        // isStaff = true → formulaire staff, false → formulaire client
        // user = null → création, user != null → modification
        public UserForm(bool isStaff, User user = null)
        {
            InitializeComponent();
            _ctx = new WorkTogetherContext();
            _isStaff = isStaff;
            _isEditMode = user != null;
            _user = user;

            SetupForm();
        }

        private void SetupForm()
        {
            // Titre dynamique
            FormTitle.Text = _isEditMode
                ? $"Modifier {(_isStaff ? "le membre du staff" : "le client")}"
                : $"Ajouter {(_isStaff ? "un membre du staff" : "un client")}";

            // Affiche les bons champs
            if (_isStaff)
            {
                StaffFields.Visibility = Visibility.Visible;
                ClientFields.Visibility = Visibility.Collapsed;
            }
            else
            {
                StaffFields.Visibility = Visibility.Collapsed;
                ClientFields.Visibility = Visibility.Visible;
            }

            // En mode modification, affiche le hint mot de passe
            if (_isEditMode)
                PasswordHint.Visibility = Visibility.Visible;

            // Pré-remplit les champs si modification
            if (_isEditMode && _user != null)
                LoadUserData();
        }

        private void LoadUserData()
        {
            FirstNameBox.Text = _user.FirstName;
            NameBox.Text = _user.Name;
            EmailBox.Text = _user.Email;

            if (_isStaff)
            {
                // Sélectionne le bon rôle dans le ComboBox
                foreach (ComboBoxItem item in RoleBox.Items)
                    if (item.Content.ToString() == _user.Roles)
                        RoleBox.SelectedItem = item;
            }
            else
            {
                // Charge les données client avec ses relations
                var customer = _ctx.Customers
                    .Include(c => c.IdNavigation)
                    .Include(c => c.IdNavigation.Particular)
                    .Include(c => c.IdNavigation.Company)
                    .FirstOrDefault(c => c.Id == _user.Id);

                if (customer != null)
                {
                    PhoneBox.Text = customer.Phone;
                    AdressBox.Text = customer.Adress;
                    InvoiceAdressBox.Text = customer.InvoiceAdress;

                    if (_user.Discr == "particular")
                    {
                        ClientTypeBox.SelectedIndex = 0;
                        ParticularFields.Visibility = Visibility.Visible;
                        CompanyFields.Visibility = Visibility.Collapsed;

                        var particular = _ctx.Particulars.FirstOrDefault(p => p.Id == _user.Id);
                        if (particular != null)
                            foreach (ComboBoxItem item in GenderBox.Items)
                                if (item.Content.ToString() == particular.Gender)
                                    GenderBox.SelectedItem = item;
                    }
                    else if (_user.Discr == "company")
                    {
                        ClientTypeBox.SelectedIndex = 1;
                        ParticularFields.Visibility = Visibility.Collapsed;
                        CompanyFields.Visibility = Visibility.Visible;

                        var company = _ctx.Companies.FirstOrDefault(c => c.Id == _user.Id);
                        if (company != null)
                        {
                            SiretBox.Text = company.Siret;
                            CompanyNameBox.Text = company.CompanyName;
                        }
                    }
                }
            }
        }

        private void ClientTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientTypeBox.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Tag?.ToString() == "particular")
                {
                    ParticularFields.Visibility = Visibility.Visible;
                    CompanyFields.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ParticularFields.Visibility = Visibility.Collapsed;
                    CompanyFields.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new UserBoard();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!await ValidateForm()) return;

            if (_isEditMode)
                await UpdateUser();
            else
                await CreateUser();
        }

        private async Task<bool> ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                await ShowMessage("Champs manquants", "Prénom, nom et email sont obligatoires.");
                return false;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                await ShowMessage("Mot de passe manquant", "Le mot de passe est obligatoire.");
                return false;
            }

            if (_isStaff && RoleBox.SelectedItem == null)
            {
                await ShowMessage("Rôle manquant", "Veuillez sélectionner un rôle.");
                return false;
            }

            return true;
        }

        private async Task CreateUser()
        {
            try
            {
                var user = new User
                {
                    FirstName = FirstNameBox.Text.Trim(),
                    Name = NameBox.Text.Trim(),
                    Email = EmailBox.Text.Trim(),
                    Password = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password),
                };

                if (_isStaff)
                {
                    var role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    user.Roles = role;
                    user.Discr = role switch
                    {
                        "ROLE_ADMIN" => "admin",
                        "ROLE_ACCOUNTANT" => "accountant",
                        "ROLE_SUPPORT" => "support",
                        "ROLE_TECHNICIAN" => "technician",
                        _ => "user"
                    };

                    _ctx.Users.Add(user);
                    _ctx.SaveChanges();

                    // Crée l'entrée dans la table spécifique
                    switch (user.Discr)
                    {
                        case "admin":
                            _ctx.Administrators.Add(new Administrator { Id = user.Id });
                            break;
                        case "accountant":
                            _ctx.Accountants.Add(new Accountant { Id = user.Id });
                            break;
                        case "support":
                            _ctx.Supports.Add(new Support { Id = user.Id });
                            break;
                        case "technician":
                            _ctx.Technicians.Add(new Technician
                            {
                                Id = user.Id,
                                Phone = "",
                                Level = 1
                            });
                            break;
                    }
                    _ctx.SaveChanges();
                }
                else
                {
                    var clientType = (ClientTypeBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
                    user.Discr = clientType;
                    user.Roles = "ROLE_CUSTOMER";

                    _ctx.Users.Add(user);
                    _ctx.SaveChanges();

                    var customer = new Customer
                    {
                        Id = user.Id,
                        Phone = PhoneBox.Text.Trim(),
                        Adress = AdressBox.Text.Trim(),
                        InvoiceAdress = InvoiceAdressBox.Text.Trim()
                    };
                    _ctx.Customers.Add(customer);
                    _ctx.SaveChanges();

                    if (clientType == "particular")
                    {
                        var gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                        _ctx.Particulars.Add(new Particular { Id = user.Id, Gender = gender });
                    }
                    else
                    {
                        _ctx.Companies.Add(new Company
                        {
                            Id = user.Id,
                            Siret = SiretBox.Text.Trim(),
                            CompanyName = CompanyNameBox.Text.Trim()
                        });
                    }
                    _ctx.SaveChanges();
                }

                await ShowMessage("Succès", "Utilisateur créé avec succès !");
                var dashboard = Window.GetWindow(this) as DashboardWindow;
                if (dashboard != null)
                    dashboard.MainContent.Content = new UserBoard();
            }
            catch (Exception ex)
            {
                await ShowMessage("Erreur", $"Erreur : {ex.Message}");
            }
        }

        private async Task UpdateUser()
        {
            try
            {
                var user = _ctx.Users.FirstOrDefault(u => u.Id == _user.Id);
                if (user == null) return;

                user.FirstName = FirstNameBox.Text.Trim();
                user.Name = NameBox.Text.Trim();
                user.Email = EmailBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                    user.Password = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password);

                if (_isStaff)
                {
                    var role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    user.Roles = role;
                }
                else
                {
                    var customer = _ctx.Customers.FirstOrDefault(c => c.Id == _user.Id);
                    if (customer != null)
                    {
                        customer.Phone = PhoneBox.Text.Trim();
                        customer.Adress = AdressBox.Text.Trim();
                        customer.InvoiceAdress = InvoiceAdressBox.Text.Trim();
                    }

                    if (_user.Discr == "particular")
                    {
                        var particular = _ctx.Particulars.FirstOrDefault(p => p.Id == _user.Id);
                        if (particular != null)
                            particular.Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    }
                    else if (_user.Discr == "company")
                    {
                        var company = _ctx.Companies.FirstOrDefault(c => c.Id == _user.Id);
                        if (company != null)
                        {
                            company.Siret = SiretBox.Text.Trim();
                            company.CompanyName = CompanyNameBox.Text.Trim();
                        }
                    }
                }

                _ctx.SaveChanges();
                await ShowMessage("Succès", "Utilisateur modifié avec succès !");
                var dashboard = Window.GetWindow(this) as DashboardWindow;
                if (dashboard != null)
                    dashboard.MainContent.Content = new UserBoard();
            }
            catch (Exception ex)
            {
                await ShowMessage("Erreur", $"Erreur : {ex.Message}");
            }
        }

        private async Task ShowMessage(string title, string message)
        {
            var window = Window.GetWindow(this) as MetroWindow;
            if (window != null)
                await window.ShowMetroDialogAsync(new CustomMessage(title, message));
        }
    }
}