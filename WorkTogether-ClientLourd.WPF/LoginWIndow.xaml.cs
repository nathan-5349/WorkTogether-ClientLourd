using BCrypt.Net;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.WPF.Core;
using WorkTogether_ClientLourd.WPF.Dashboard;

namespace WorkTogether_ClientLourd.WPF
{
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginClick(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text.Trim();
            var password = PasswordBox.Password;

            // Validation basique
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await this.ShowMessageAsync("Champs manquants", "Veuillez remplir tous les champs.");
                return;
            }

            using var ctx = new WorkTogetherContext();

            // Cherche l'utilisateur par email
            var user = ctx.Users.FirstOrDefault(u => u.Email == email);

            // Utilisateur introuvable mail/mdp faux
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                await CustomMessage.Show(this, "Erreur", "Email ou mot de passe incorrect."); return;
            }

            // Vérifie que le rôle est autorisé sur le client lourd
            if (user.Roles != "ROLE_ADMIN" && user.Roles != "ROLE_ACCOUNTANT" &&
                user.Roles != "ROLE_SUPPORT" && user.Roles != "ROLE_TECHNICIAN")
            {
                await CustomMessage.Show(this, "Accès refusé", "Vous n'avez pas les droits nécessaires.");
                return;
            }

            // Stocke la session
            Session.CurrentUser = user;
            Session.Role = user.Roles;

            // Ouvre la fenêtre principale et ferme le login
            var dashboard = new DashboardWindow();
            dashboard.Show();
            this.Close();
        }
    }
}