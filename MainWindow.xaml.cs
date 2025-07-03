using DEMO_SWD392.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DEMO_SWD392
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MiniMartDbContext _context = new MiniMartDbContext();
        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();


        }
        private void LoadProducts()
        {
            var products = _context.Products
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Barcode,
                    p.Quantity,
                    p.Price,
                    Category = p.Category != null ? p.Category.CategoryName : ""
                })
                .ToList();

            dgProduct.ItemsSource = products;
        }
        private void ManageProduct_Click(object sender, RoutedEventArgs e)
        {
            dgAccountList.Visibility = Visibility.Collapsed;

            dgProduct.Visibility = Visibility.Visible;

            LoadProducts();
        }
        private void ViewProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = (button.DataContext as dynamic);
            int productId = selectedProduct.ProductId;

            var detailWindow = new ViewDetailWindow( productId);
            detailWindow.ShowDialog();

            // Nếu có thay đổi, load lại data
            if (detailWindow.IsSaved)
            {
                LoadProducts();
            }
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // Lấy ProductId từ CommandParameter
            var button = sender as Button;
            if (button?.CommandParameter is int productId)
            {
                // Tìm sản phẩm trong database
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    // Xác nhận xóa (tùy chọn)
                    var result = MessageBox.Show($"Are you sure you want to delete '{product.ProductName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Products.Remove(product);
                        _context.SaveChanges();

                        // Reload lại DataGrid
                        LoadProducts();
                    }
                }
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddProductWindow() { Owner = this };
            bool? result = addWindow.ShowDialog();
            if (result == true)
            {
                LoadProducts(); // Hàm này đã có, dùng để reload lại danh sách
            }
        }

        private void SearchProduct_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var products = _context.Products
                .Where(p =>
                    (p.ProductName ?? "").ToLower().Contains(keyword) ||
                    (p.Barcode ?? "").ToLower().Contains(keyword) ||
                    (p.Category != null && (p.Category.CategoryName ?? "").ToLower().Contains(keyword))
                )
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Barcode,
                    p.Quantity,
                    p.Price,
                    Category = p.Category != null ? p.Category.CategoryName : ""
                })
                .ToList();

            if (products.Count == 0)
            {
                MessageBox.Show("No product matched your search.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            dgProduct.ItemsSource = products;
        }

        private void ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            ManageAccount manageAccountWindow = new ManageAccount();
            manageAccountWindow.Show();
            this.Close();
        }
    }
}