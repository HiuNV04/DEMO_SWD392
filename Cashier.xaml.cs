using DEMO_SWD392.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DEMO_SWD392
{
    /// <summary>
    /// Interaction logic for Cashier.xaml
    /// </summary>
    public partial class Cashier : Window
    {
        private MiniMartDbContext _context = new MiniMartDbContext();

        public Cashier()
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

            var detailWindow = new CashierViewDetailProduct(productId);
            detailWindow.ShowDialog();

             
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();

        }
    }
}
