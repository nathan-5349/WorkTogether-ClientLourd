using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WorkTogether_ClientLourd.EF;
using WorkTogether_ClientLourd.EF.Entities;
using WorkTogether_ClientLourd.WPF.Core;
using WorkTogether_ClientLourd.WPF.Dashboard;

namespace WorkTogether_ClientLourd.WPF.Bays
{
    public class BayViewModel
    {
        public int Id { get; }
        public string Name { get; }
        public int CapacityUnit { get; }
        public int TotalUnits { get; }
        public int FreeUnits { get; }
        public int BusyUnits { get; }
        public int MaintenanceUnits { get; }

        // Garde l'Id pour recharger l'entité depuis un nouveau ctx
        public BayViewModel(Bay bay)
        {
            Id = bay.Id;
            Name = bay.Name;
            CapacityUnit = bay.CapacityUnit;
            TotalUnits = bay.Units?.Count ?? 0;
            FreeUnits = bay.Units?.Count(u => u.Status == "Libre") ?? 0;
            BusyUnits = bay.Units?.Count(u => u.Status == "Occupé") ?? 0;
            MaintenanceUnits = bay.Units?.Count(u => u.Status == "Maintenance") ?? 0;
        }
    }

    public partial class BayBoard : UserControl
    {
        private int? _selectedBayId;

        public BayBoard()
        {
            InitializeComponent();
            LoadBays();
        }

        private void LoadBays(string search = "")
        {
            List<BayViewModel> items;
            using (var ctx = new WorkTogetherContext())
            {
                items = ctx.Bays
                    .Include(b => b.Units)
                    .Where(b => string.IsNullOrEmpty(search) || b.Name.Contains(search))
                    .ToList()
                    .Select(b => new BayViewModel(b))
                    .ToList();
            }
            BayGrid.ItemsSource = items;
        }

        private void SearchBayBox_TextChanged(object sender, TextChangedEventArgs e)
            => LoadBays(SearchBayBox.Text);

        private void BayGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BayGrid.SelectedItem is BayViewModel vm)
                _selectedBayId = vm.Id;
            else
                _selectedBayId = null;
        }

        private void BtnAddBay_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new BayForm();
        }

        private async void BtnEditBay_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBayId == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner une baie.");
                return;
            }
            using var ctx = new WorkTogetherContext();
            var bay = ctx.Bays.FirstOrDefault(b => b.Id == _selectedBayId);
            if (bay == null) return;

            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new BayForm(bay);
        }

        private async void BtnDeleteBay_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBayId == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner une baie.");
                return;
            }

            bool confirm = await CustomConfirm.Show(this, "Confirmation",
                "Voulez-vous vraiment supprimer cette baie ?");

            if (confirm)
            {
                try
                {
                    using var ctx = new WorkTogetherContext();
                    var bay = ctx.Bays
                        .Include(b => b.Units)
                        .FirstOrDefault(b => b.Id == _selectedBayId);
                    if (bay != null)
                    {
                        ctx.Bays.Remove(bay);
                        ctx.SaveChanges();
                        LoadBays(SearchBayBox.Text);
                        await CustomMessage.Show(this, "Succès", "Baie supprimée avec succès !");
                    }
                }
                catch (Exception ex)
                {
                    await CustomMessage.Show(this, "Erreur", $"Erreur : {ex.Message}");
                }
            }
        }
    }
}