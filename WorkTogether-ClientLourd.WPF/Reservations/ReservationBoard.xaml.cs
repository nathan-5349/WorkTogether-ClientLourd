using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;

namespace WorkTogether_ClientLourd.WPF.Reservations
{
    public class ReservationViewModel
    {
        public int Id { get; }
        public string CustomerFullName { get; }
        public string CustomerType { get; }
        public string OfferName { get; }
        public string BeginDateFormatted { get; }
        public string FinishDateFormatted { get; }
        public int UnitCount { get; }
        public string Status { get; }
        public Brush StatusBackground { get; }
        public Brush StatusForeground { get; }
        public Brush OfferBackground { get; }
        public Brush OfferForeground { get; }

        public ReservationViewModel(Reservation r)
        {
            Id = r.Id;
            CustomerFullName = $"{r.Customer?.IdNavigation?.FirstName} {r.Customer?.IdNavigation?.Name}".Trim();
            OfferName = r.ReservationOfferNavigation?.Name ?? "—";
            BeginDateFormatted = r.BeginDate.ToString("dd/MM/yyyy");
            FinishDateFormatted = r.FinishDate.ToString("dd/MM/yyyy");
            UnitCount = r.Units?.Count ?? 0;
            Status = r.Status;

            // Type client : Particulier ou Entreprise
            CustomerType = r.Customer?.IdNavigation?.Discr switch
            {
                "particular" => "Particulier",
                "company" => "Entreprise",
                _ => "—"
            };

            // Couleurs pastel statut
            (StatusBackground, StatusForeground) = r.Status switch
            {
                "Confirmée" => (new SolidColorBrush(Color.FromRgb(200, 230, 201)),
                                 new SolidColorBrush(Color.FromRgb(27, 94, 32))),
                "Annulée" => (new SolidColorBrush(Color.FromRgb(255, 205, 210)),
                                 new SolidColorBrush(Color.FromRgb(183, 28, 28))),
                "En attente" => (new SolidColorBrush(Color.FromRgb(255, 236, 179)),
                                 new SolidColorBrush(Color.FromRgb(230, 81, 0))),
                _ => (new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                                 new SolidColorBrush(Color.FromRgb(66, 66, 66))),
            };

            // Couleurs pastel offre
            var offerColors = new[]
            {
                (Color.FromRgb(207, 226, 255), Color.FromRgb(13,  71, 161)),
                (Color.FromRgb(225, 190, 231), Color.FromRgb(106, 27, 154)),
                (Color.FromRgb(178, 235, 242), Color.FromRgb(0,   96, 100)),
                (Color.FromRgb(255, 224, 178), Color.FromRgb(230, 81,   0)),
            };
            int idx = Math.Abs((OfferName?.GetHashCode() ?? 0) % offerColors.Length);
            OfferBackground = new SolidColorBrush(offerColors[idx].Item1);
            OfferForeground = new SolidColorBrush(offerColors[idx].Item2);
        }
    }

    // Item simple pour le ComboBox des offres
    public class OfferFilterItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class ReservationBoard : UserControl
    {
        private int? _filterOfferId;   // pré-filtre depuis OfferBoard

        // Constructeur normal (depuis la sidebar)
        public ReservationBoard()
        {
            InitializeComponent();
        }

        // Constructeur avec filtre offre (depuis OfferBoard → bouton Info)
        public ReservationBoard(int offerId, string offerName)
        {
            InitializeComponent();
            _filterOfferId = offerId;
        }

        private void ReservationBoard_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOfferFilter();
            LoadReservations();
        }

        // Charge la liste des offres dans le ComboBox filtre
        private void LoadOfferFilter()
        {
            using var ctx = new WorkTogetherContext();
            var offers = ctx.Offers
                .AsNoTracking()
                .OrderBy(o => o.Name)
                .Select(o => new OfferFilterItem { Id = o.Id, Name = o.Name })
                .ToList();

            // Item "Toutes les offres" en tête
            offers.Insert(0, new OfferFilterItem { Id = 0, Name = "Toutes les offres" });
            FilterOffer.ItemsSource = offers;

            // Si on vient de OfferBoard, présélectionne l'offre
            FilterOffer.SelectedValue = _filterOfferId.HasValue ? _filterOfferId.Value : 0;
        }

        private void LoadReservations(string search = "")
        {
            string status = (FilterStatus.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "all";
            string customerType = (FilterCustomerType.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "all";
            int offerId = (FilterOffer.SelectedValue is int id) ? id : 0;

            List<ReservationViewModel> items;
            using (var ctx = new WorkTogetherContext())
            {
                var query = ctx.Reservations
                    .AsNoTracking()
                    .Include(r => r.Customer)
                        .ThenInclude(c => c.IdNavigation)
                    .Include(r => r.ReservationOfferNavigation)
                    .Include(r => r.Units)
                    .AsQueryable();

                // Filtre offre
                if (offerId > 0)
                    query = query.Where(r => r.ReservationOffer == offerId);

                // Filtre statut
                if (status != "all")
                    query = query.Where(r => r.Status == status);

                // Filtre type client via le discriminateur Symfony
                if (customerType == "particular")
                    query = query.Where(r => r.Customer.IdNavigation.Discr == "particular");
                else if (customerType == "company")
                    query = query.Where(r => r.Customer.IdNavigation.Discr == "company");

                items = query
                    .OrderByDescending(r => r.BeginDate)
                    .ToList()
                    .Where(r => string.IsNullOrEmpty(search) ||
                                (r.Customer?.IdNavigation?.FirstName + " " + r.Customer?.IdNavigation?.Name)
                                .Contains(search, StringComparison.OrdinalIgnoreCase))
                    .Select(r => new ReservationViewModel(r))
                    .ToList();
            }

            ReservationGrid.ItemsSource = items;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => LoadReservations(SearchBox.Text);

        private void FilterStatus_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            LoadReservations(SearchBox.Text);
        }

        private void FilterOffer_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            LoadReservations(SearchBox.Text);
        }

        private void FilterCustomerType_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            LoadReservations(SearchBox.Text);
        }
    }
}