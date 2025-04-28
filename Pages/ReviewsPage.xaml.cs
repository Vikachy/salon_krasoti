using salon_krasoti.PagesEdit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace salon_krasoti.Pages
{
    public partial class ReviewsPage : Page
    {
        private List<Reviews> _allReviews;

        public ReviewsPage()
        {
            InitializeComponent();
            LoadReviews();
        }

        private void LoadReviews()
        {
            _allReviews = Entities.GetContext().Reviews
                .Include(r => r.Clients)
                .Include(r => r.Services)
                .ToList();
            UpdateReviews();
        }

        private void UpdateReviews()
        {
            var currentReviews = _allReviews.AsEnumerable();

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(SearchReview.Text))
            {
                currentReviews = currentReviews.Where(r =>
                    (r.Clients.FirstName + " " + r.Clients.LastName).ToLower().Contains(SearchReview.Text.ToLower()) ||
                    r.Services.ServiceName.ToLower().Contains(SearchReview.Text.ToLower()) ||
                    r.Rating.ToString().Contains(SearchReview.Text) ||
                    (r.Comment != null && r.Comment.ToLower().Contains(SearchReview.Text.ToLower())));
            }

            // Сортировка
            if (SortReviewBy.SelectedItem != null)
            {
                switch (((ComboBoxItem)SortReviewBy.SelectedItem).Content.ToString())
                {
                    case "По рейтингу (↑)":
                        currentReviews = currentReviews.OrderBy(r => r.Rating);
                        break;
                    case "По рейтингу (↓)":
                        currentReviews = currentReviews.OrderByDescending(r => r.Rating);
                        break;
                }
            }

            DataGridReviews.ItemsSource = currentReviews.Select(r => new
            {
                ReviewID = r.ReviewID,
                ClientName = r.Clients.FirstName + " " + r.Clients.LastName,
                ServiceName = r.Services.ServiceName,
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty
            }).ToList();
        }

        private void SearchReview_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateReviews();
        }

        private void SortReviewBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateReviews();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchReview.Text = "";
            SortReviewBy.SelectedIndex = -1;
            UpdateReviews();
        }

        private void AddReview_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditReviewPage(null));
        }

        private void EditReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedReview = DataGridReviews.SelectedItem as dynamic;
            if (selectedReview == null)
            {
                MessageBox.Show("Выберите отзыв для редактирования.");
                return;
            }

            var review = _allReviews.FirstOrDefault(r => r.ReviewID == selectedReview.ReviewID);
            if (review == null)
            {
                MessageBox.Show("Отзыв не найден.");
                return;
            }

            NavigationService.Navigate(new AddEditReviewPage(review));
        }

        private void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedReview = DataGridReviews.SelectedItem as dynamic;
            if (selectedReview == null)
            {
                MessageBox.Show("Выберите отзыв для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот отзыв?",
                "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var review = _allReviews.FirstOrDefault(r => r.ReviewID == selectedReview.ReviewID);
                    if (review != null)
                    {
                        Entities.GetContext().Reviews.Remove(review);
                        Entities.GetContext().SaveChanges();
                        LoadReviews();
                        MessageBox.Show("Отзыв удален.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadReviews();
            }
        }
    }
}