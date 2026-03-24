using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;
using WorkTogether_ClientLourd.WPF.Core;
using WorkTogether_ClientLourd.WPF.Dashboard;

namespace WorkTogether_ClientLourd.WPF.Offers
{
    public partial class OfferForm : UserControl
    {
        private WorkTogetherContext _ctx;
        private Offer _offer;
        private bool _isEditMode;

        public OfferForm(Offer offer = null)
        {
            InitializeComponent();
            _ctx = new WorkTogetherContext();
            _isEditMode = offer != null;
            _offer = offer;
            SetupForm();
        }

        private void SetupForm()
        {
            if (_isEditMode && _offer != null)
            {
                FormTitle.Text = "Modifier l'offre";
                NameBox.Text = _offer.Name;
                NbUnitBox.Value = _offer.NbUnit;
                PriceBox.Value = _offer.Price;
            }
            else
            {
                FormTitle.Text = "Ajouter une offre";
            }
        }

        private async Task<bool> ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                await CustomMessage.Show(this, "Champ manquant", "Le nom de l'offre est obligatoire.");
                return false;
            }
            if (NbUnitBox.Value == null || NbUnitBox.Value < 1)
            {
                await CustomMessage.Show(this, "Champ invalide", "Le nombre d'unités doit être supérieur à 0.");
                return false;
            }
            if (PriceBox.Value == null || PriceBox.Value < 0)
            {
                await CustomMessage.Show(this, "Champ invalide", "Le prix doit être positif ou nul.");
                return false;
            }
            return true;
        }

        private async Task CreateOffer()
        {
            try
            {
                var offer = new Offer
                {
                    Name = NameBox.Text.Trim(),
                    NbUnit = (int)NbUnitBox.Value,
                    Price = (int)PriceBox.Value,
                    Version = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _ctx.Offers.Add(offer);
                _ctx.SaveChanges();
                await CustomMessage.Show(this, "Succès", "Offre créée avec succès en version 1 !");
                NavigateToBoard();
            }
            catch (Exception ex)
            {
                await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
            }
        }

        private async Task UpdateOffer()
        {
            try
            {
                var oldOffer = _ctx.Offers.FirstOrDefault(o => o.Id == _offer.Id);
                if (oldOffer == null) return;
                oldOffer.IsActive = false;

                // 2. Créer une nouvelle offre avec version + 1
                var newOffer = new Offer
                {
                    Name = NameBox.Text.Trim(),
                    NbUnit = (int)NbUnitBox.Value,
                    Price = (int)PriceBox.Value,
                    Version = oldOffer.Version + 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                _ctx.Offers.Add(newOffer);
                _ctx.SaveChanges();

                await CustomMessage.Show(this, "Succès",
                    $"Offre mise à jour ! Version {newOffer.Version} active, version {oldOffer.Version} archivée.");
                NavigateToBoard();
            }
            catch (Exception ex)
            {
                await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
            }
        }

        private void NavigateToBoard()
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            dashboard?.NavigateTo(new OfferBoard(), "offers");
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => NavigateToBoard();

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!await ValidateForm()) return;

            if (_isEditMode)
                await UpdateOffer();
            else
                await CreateOffer();
        }
    }
}