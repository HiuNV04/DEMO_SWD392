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
    /// Interaction logic for CashierViewDetailProduct.xaml
    /// </summary>
    public partial class CashierViewDetailProduct : Window
    {
        private readonly MiniMartDbContext _context = new MiniMartDbContext();
        private Product _product;
        public CashierViewDetailProduct(int productId)
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
    }
}
