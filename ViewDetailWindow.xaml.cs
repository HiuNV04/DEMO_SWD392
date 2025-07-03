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
    /// Interaction logic for ViewDetailWindow.xaml
    /// </summary>
    public partial class ViewDetailWindow : Window
    {
        private readonly MiniMartDbContext _context = new MiniMartDbContext();
        private Product _product;
        public bool IsSaved { get; private set; } = false;
        public ViewDetailWindow(int productId)
        {
            InitializeComponent();
            _product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            LoadCategory();
            LoadProductDetail();
        }
        private void LoadCategory()
        {
            var categories = _context.Categories.ToList();
            cbCategory.ItemsSource = categories;
            cbCategory.DisplayMemberPath = "CategoryName";
            cbCategory.SelectedValuePath = "CategoryId";
        }

        private void LoadProductDetail()
        {
            if (_product != null)
            {
                txtName.Text = _product.ProductName;
                txtBarcode.Text = _product.Barcode;
                txtQuantity.Text = _product.Quantity?.ToString();
                txtPrice.Text = _product.Price?.ToString();
                cbCategory.SelectedValue = _product.CategoryId;
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_product != null)
            {
                _product.ProductName = txtName.Text;
                _product.Barcode = txtBarcode.Text;
                _product.Quantity = int.TryParse(txtQuantity.Text, out int q) ? q : 0;
                _product.Price = decimal.TryParse(txtPrice.Text, out decimal p) ? p : 0;
                _product.CategoryId = (int?)cbCategory.SelectedValue;
                _context.SaveChanges();
                IsSaved = true;
                MessageBox.Show("Changes saved successfully!", "Success");
                this.Close();
            }
        }
    }
}
