using DEMO_SWD392.Models;
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
    }
}