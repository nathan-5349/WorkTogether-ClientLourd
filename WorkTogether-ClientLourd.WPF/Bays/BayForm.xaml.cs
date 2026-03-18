using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;
using WorkTogether_ClientLourd.WPF.Core;
using WorkTogether_ClientLourd.WPF.Dashboard;

namespace WorkTogether_ClientLourd.WPF.Bays
{
    public partial class BayForm : UserControl
    {
        private WorkTogetherContext _ctx;
        private Bay _bay;
        private bool _isEditMode;

        // Sans argument = création, avec Bay = modification
        public BayForm(Bay bay = null)
        {
            InitializeComponent();
            _ctx = new WorkTogetherContext();
            _isEditMode = bay != null;
            _bay = bay;
            SetupForm();
        }

        private void SetupForm()
        {
            FormTitle.Text = _isEditMode ? "Modifier la baie" : "Ajouter une baie";

            // Pré-remplit les champs si modification
            if (_isEditMode && _bay != null)
            {
                NameBox.Text = _bay.Name;
                CapacityBox.Value = _bay.CapacityUnit;
            }
        }

        private async Task<bool> ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                await CustomMessage.Show(this, "Champ manquant", "Le nom de la baie est obligatoire.");
                return false;
            }
            return true;
        }

        private async Task CreateBay()
        {
            try
            {
                var bay = new Bay
                {
                    Name = NameBox.Text.Trim(),
                    CapacityUnit = (int)CapacityBox.Value
                };

                _ctx.Bays.Add(bay);
                _ctx.SaveChanges();

                await CustomMessage.Show(this, "Succès", "Baie créée avec succès !");
                var dashboard = Window.GetWindow(this) as DashboardWindow;
                if (dashboard != null)
                    dashboard.MainContent.Content = new BayBoard();
            }
            catch (Exception ex)
            {
                await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
            }
        }

        private async Task UpdateBay()
        {
            try
            {
                var bay = _ctx.Bays.FirstOrDefault(b => b.Id == _bay.Id);
                if (bay == null) return;

                bay.Name = NameBox.Text.Trim();
                bay.CapacityUnit = (int)CapacityBox.Value;

                _ctx.SaveChanges();

                await CustomMessage.Show(this, "Succès", "Baie modifiée avec succès !");
                var dashboard = Window.GetWindow(this) as DashboardWindow;
                if (dashboard != null)
                    dashboard.MainContent.Content = new BayBoard();
            }
            catch (Exception ex)
            {
                await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new BayBoard();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!await ValidateForm()) return;

            if (_isEditMode)
                await UpdateBay();
            else
                await CreateBay();
        }
    }
}