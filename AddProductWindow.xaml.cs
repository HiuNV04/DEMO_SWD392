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
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private readonly MiniMartDbContext _context = new MiniMartDbContext();
        public AddProductWindow()
        {
            InitializeComponent();
            LoadCategories();
        }
        private void LoadCategories()
        {
            var categories = _context.Categories.ToList();
            cbCategory.ItemsSource = categories;
            cbCategory.DisplayMemberPath = "CategoryName";
            cbCategory.SelectedValuePath = "CategoryId";
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = txtName.Text.Trim();
                var barcode = txtBarcode.Text.Trim();
                int.TryParse(txtQuantity.Text, out int quantity);
                decimal.TryParse(txtPrice.Text, out decimal price);
                int? categoryId = cbCategory.SelectedValue as int?;

                if (string.IsNullOrEmpty(name) || categoryId == null)
                {
                    MessageBox.Show("Please enter all required fields!", "Warning");
                    return;
                }

                var product = new Product
                {
                    ProductName = name,
                    Barcode = barcode,
                    Quantity = quantity,
                    Price = price,
                    CategoryId = categoryId
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                MessageBox.Show("Thêm sản phẩm thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Để MainWindow biết reload
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }

        }
    }
}
