using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;
using WorkTogether_ClientLourd.WPF.Core;
using WorkTogether_ClientLourd.WPF.Dashboard;
using WorkTogether_ClientLourd.WPF.Reservations;

namespace WorkTogether_ClientLourd.WPF.Offers
{
    public class OfferViewModel
    {
        public Offer Source { get; }

        public OfferViewModel(Offer offer) => Source = offer;

        public int Id => Source.Id;
        public string Name => Source.Name;
        public int NbUnit => Source.NbUnit;
        public int Price => Source.Price;
        public int Version => Source.Version;
        public bool IsActive => Source.IsActive;
        public int ReservationCount => Source.Reservations?.Count ?? 0;
        public string CreatedAtFormatted => Source.CreatedAt.ToString("dd/MM/yyyy");
    }

    public partial class OfferBoard : UserControl
    {
        private Offer _selectedOffer;

        public OfferBoard()
        {
            InitializeComponent();
        }

        private void OfferBoard_Loaded(object sender, RoutedEventArgs e)
            => LoadOffers();

        private void LoadOffers(string search = "")
        {
            using var ctx = new WorkTogetherContext();

            var status = (FilterStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "active";

            var query = ctx.Offers
                .AsNoTracking()
                .Include(o => o.Reservations)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(o => o.Name.Contains(search));

            if (status == "active")
                query = query.Where(o => o.IsActive);
            else if (status == "inactive")
                query = query.Where(o => !o.IsActive);

            OfferGrid.ItemsSource = query
                .OrderBy(o => o.Name)
                .ThenByDescending(o => o.Version)
                .ToList()
                .Select(o => new OfferViewModel(o))
                .ToList();
        }

        private void SearchOfferBox_TextChanged(object sender, TextChangedEventArgs e)
            => LoadOffers(SearchOfferBox.Text);

        private void FilterStatus_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            LoadOffers(SearchOfferBox.Text);
        }

        private void OfferGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _selectedOffer = (OfferGrid.SelectedItem as OfferViewModel)?.Source;

        private void BtnAddOffer_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new OfferForm();
        }

        private async void BtnEditOffer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOffer == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner une offre.");
                return;
            }
            if (!_selectedOffer.IsActive)
            {
                await CustomMessage.Show(this, "Offre archivée",
                    "Vous ne pouvez pas modifier une offre archivée. Sélectionnez la version active.");
                return;
            }
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new OfferForm(_selectedOffer);
        }

        private async void BtnToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int offerId) return;

            using var ctx = new WorkTogetherContext();
            var offer = ctx.Offers.FirstOrDefault(o => o.Id == offerId);
            if (offer == null) return;

            string action = offer.IsActive ? "désactiver" : "activer";
            bool confirm = await CustomConfirm.Show(this, "Confirmation",
                $"Voulez-vous vraiment {action} l'offre \"{offer.Name}\" (v{offer.Version}) ?");

            if (confirm)
            {
                try
                {
                    offer.IsActive = !offer.IsActive;
                    ctx.SaveChanges();
                    LoadOffers(SearchOfferBox.Text);
                    string etat = offer.IsActive ? "activée" : "désactivée";
                    await CustomMessage.Show(this, "Succès", $"Offre {etat} avec succès !");
                }
                catch (Exception ex)
                {
                    await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
                }
            }
        }

        private async void BtnDeleteOffer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOffer == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner une offre.");
                return;
            }
            if (_selectedOffer.Reservations.Any())
            {
                await CustomMessage.Show(this, "Suppression impossible",
                    "Cette offre est liée à des réservations et ne peut pas être supprimée.");
                return;
            }

            bool confirm = await CustomConfirm.Show(this, "Confirmation",
                $"Voulez-vous vraiment supprimer l'offre \"{_selectedOffer.Name}\" (v{_selectedOffer.Version}) ?");

            if (confirm)
            {
                try
                {
                    using var ctx = new WorkTogetherContext();
                    var offer = ctx.Offers.FirstOrDefault(o => o.Id == _selectedOffer.Id);
                    if (offer != null)
                    {
                        ctx.Offers.Remove(offer);
                        ctx.SaveChanges();
                        LoadOffers(SearchOfferBox.Text);
                        await CustomMessage.Show(this, "Succès", "Offre supprimée avec succès !");
                    }
                }
                catch (Exception ex)
                {
                    await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
                }
            }
        }
        private void BtnInfoOffer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int offerId) return;

            using var ctx = new WorkTogetherContext();
            var offer = ctx.Offers.FirstOrDefault(o => o.Id == offerId);
            if (offer == null) return;

            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new ReservationBoard(offerId, offer.Name);
        }
    }
}