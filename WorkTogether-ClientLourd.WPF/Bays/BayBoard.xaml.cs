using Microsoft.EntityFrameworkCore;
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
    public partial class BayBoard : UserControl
    {
        private WorkTogetherContext _ctx;
        private List<Bay> _bayList;
        private Bay _selectedBay;

        public BayBoard()
        {
            InitializeComponent();
            _ctx = new WorkTogetherContext();
            LoadBays();
        }

        private void LoadBays(string search = "")
        {
            // On inclut les unités pour avoir Units.Count dans le DataGrid
            _bayList = _ctx.Bays
                .Include(b => b.Units)
                .Where(b => string.IsNullOrEmpty(search) ||
                            b.Name.Contains(search))
                .ToList();
            BayGrid.ItemsSource = _bayList;
        }

        private void SearchBayBox_TextChanged(object sender, TextChangedEventArgs e)
            => LoadBays(SearchBayBox.Text);

        private void BayGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => _selectedBay = BayGrid.SelectedItem as Bay;

        private async void BtnAddBay_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new BayForm();
        }

        private async void BtnEditBay_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBay == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner une baie.");
                return;
            }
            var dashboard = Window.GetWindow(this) as DashboardWindow;
            if (dashboard != null)
                dashboard.MainContent.Content = new BayForm(_selectedBay);
        }

        private async void BtnDeleteBay_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBay == null)
            {
                await CustomMessage.Show(this, "Sélection manquante", "Veuillez sélectionner une baie.");
                return;
            }

            bool confirm = await CustomConfirm.Show(this, "Confirmation",
                $"Voulez-vous vraiment supprimer la baie {_selectedBay.Name} ?");

            if (confirm)
            {
                try
                {
                    var bay = _ctx.Bays
                        .Include(b => b.Units)
                        .FirstOrDefault(b => b.Id == _selectedBay.Id);

                    if (bay != null)
                    {
                        _ctx.Bays.Remove(bay);
                        _ctx.SaveChanges();
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