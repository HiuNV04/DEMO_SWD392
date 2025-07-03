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
using System.ComponentModel;

namespace DEMO_SWD392
{
    /// <summary>
    /// Interaction logic for Cashier.xaml
    /// </summary>
    public class CartDisplayItem : INotifyPropertyChanged
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        private int quantity;
        public int Quantity
        {
            get => quantity;
            set { quantity = value; OnPropertyChanged("Quantity"); OnPropertyChanged("Total"); }
        }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class Cashier : Window
    {
        private MiniMartDbContext db = new MiniMartDbContext();
        private Cart currentCart;
        private List<CartDisplayItem> cartDisplayItems = new List<CartDisplayItem>();
        private List<CartItem> cartItems = new List<CartItem>();
        private int currentUserId = 1; // TODO: Lấy userId thực tế khi đăng nhập
        private decimal discountPercent = 0;
        private string lastInvoiceFile = null;
        private List<Product> allProducts;

        public Cashier()
        {
            InitializeComponent();
            allProducts = db.Products.ToList();
            StartNewCart();
            UpdateCartDisplay();
            cbProductInput.ItemsSource = allProducts.Select(p => p.ProductName + " (" + p.Barcode + ")").ToList();
        }

        private void StartNewCart()
        {
            currentCart = new Cart
            {
                UserId = currentUserId,
                CreatedDate = DateTime.Now,
                Status = "Active"
            };
            db.Carts.Add(currentCart);
            db.SaveChanges();
            cartItems.Clear();
            cartDisplayItems.Clear();
            discountPercent = 0;
            lastInvoiceFile = null;
            UpdateCartDisplay();
        }

        private void cbProductInput_KeyUp(object sender, KeyEventArgs e)
        {
            string text = cbProductInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                cbProductInput.ItemsSource = allProducts.Select(p => p.ProductName + " (" + p.Barcode + ")").ToList();
                cbProductInput.IsDropDownOpen = false;
                return;
            }
            var filtered = allProducts.Where(p => (!string.IsNullOrEmpty(p.ProductName) && p.ProductName.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0) || (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.Contains(text))).ToList();
            cbProductInput.ItemsSource = filtered.Select(p => p.ProductName + " (" + p.Barcode + ")").ToList();
            cbProductInput.IsDropDownOpen = filtered.Any();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            string input = cbProductInput.Text.Trim();
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ!");
                return;
            }
            // Tìm theo tên hoặc barcode
            Product product = null;
            if (input.Contains("(") && input.Contains(")"))
            {
                // Định dạng: Tên (Barcode)
                var barcode = input.Substring(input.LastIndexOf('(') + 1).TrimEnd(')');
                product = allProducts.FirstOrDefault(p => p.Barcode == barcode);
            }
            if (product == null)
            {
                product = allProducts.FirstOrDefault(p => (p.ProductName != null && p.ProductName.Equals(input, StringComparison.OrdinalIgnoreCase)) || (p.Barcode != null && p.Barcode == input));
            }
            if (product == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm!");
                return;
            }
            if (product.Quantity < quantity)
            {
                MessageBox.Show("Không đủ hàng tồn kho!");
                return;
            }
            var existing = cartDisplayItems.FirstOrDefault(ci => ci.ProductId == product.ProductId);
            if (existing != null)
            {
                if (product.Quantity < existing.Quantity + quantity)
                {
                    MessageBox.Show("Không đủ hàng tồn kho!");
                    return;
                }
                existing.Quantity += quantity;
            }
            else
            {
                cartDisplayItems.Add(new CartDisplayItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = product.Price ?? 0
                });
            }
            UpdateCartDisplay();
        }

        private void btnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null && int.TryParse(btn.Tag.ToString(), out int productId))
            {
                var item = cartDisplayItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (item != null)
                {
                    cartDisplayItems.Remove(item);
                    UpdateCartDisplay();
                }
            }
        }

        private void UpdateCartDisplay()
        {
            dgCart.ItemsSource = null;
            dgCart.ItemsSource = cartDisplayItems;
            decimal total = cartDisplayItems.Sum(i => i.Total);
            if (discountPercent > 0)
            {
                total = total * (1 - discountPercent / 100);
            }
            lblTotal.Text = total.ToString("N0");
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (!cartDisplayItems.Any())
            {
                MessageBox.Show("Giỏ hàng trống!");
                return;
            }
            // Áp dụng mã giảm giá nếu có
            string discountCode = txtDiscountCode.Text.Trim();
            DiscountCode discount = null;
            if (!string.IsNullOrEmpty(discountCode))
            {
                discount = db.DiscountCodes.FirstOrDefault(d => d.DiscountCode1 == discountCode && (d.ExpiryDate == null || d.ExpiryDate >= DateOnly.FromDateTime(DateTime.Now)));
                if (discount == null)
                {
                    MessageBox.Show("Mã giảm giá không hợp lệ hoặc đã hết hạn!");
                    return;
                }
                discountPercent = discount.Percentage ?? 0;
            }
            else
            {
                discountPercent = 0;
            }
            // Tạo hóa đơn
            var invoice = new Invoice
            {
                InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
                UserId = currentUserId,
                TotalAmount = cartDisplayItems.Sum(i => i.Total) * (1 - discountPercent / 100),
                DiscountCode = discount?.DiscountCode1
            };
            db.Invoices.Add(invoice);
            db.SaveChanges();
            // Thêm chi tiết hóa đơn và trừ tồn kho
            foreach (var item in cartDisplayItems)
            {
                db.InvoiceDetails.Add(new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
                var product = db.Products.Find(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
            }
            // Cập nhật trạng thái giỏ hàng
            var dbCart = db.Carts.Find(currentCart.CartId);
            if (dbCart != null)
            {
                dbCart.Status = "CheckedOut";
            }
            db.SaveChanges();
            MessageBox.Show("Thanh toán thành công!");
            PrintInvoice(invoice.InvoiceId);
            StartNewCart();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Hủy giỏ hàng
            var dbCart = db.Carts.Find(currentCart.CartId);
            if (dbCart != null && dbCart.Status == "Active")
            {
                dbCart.Status = "Cancelled";
                db.SaveChanges();
            }
            cartDisplayItems.Clear();
            UpdateCartDisplay();
            MessageBox.Show("Đã hủy giao dịch.");
            StartNewCart();
        }

        private void btnPrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (lastInvoiceFile == null)
            {
                MessageBox.Show("Chưa có hóa đơn nào để in!");
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = lastInvoiceFile,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Không thể mở file hóa đơn!");
            }
        }

        private void PrintInvoice(int invoiceId)
        {
            var invoice = db.Invoices.Find(invoiceId);
            var details = db.InvoiceDetails.Where(d => d.InvoiceId == invoiceId).ToList();

            // Lấy thư mục gốc project
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 2; i++)
            {
                projectRoot = System.IO.Directory.GetParent(projectRoot).FullName;
            }
            // Đường dẫn thư mục Invoice
            string invoiceDir = System.IO.Path.Combine(projectRoot, "Invoice");
            if (!System.IO.Directory.Exists(invoiceDir))
            {
                System.IO.Directory.CreateDirectory(invoiceDir);
            }
            string fileName = System.IO.Path.Combine(invoiceDir, $"Invoice_{invoiceId}_{DateTime.Now:yyyyMMddHHmmss}.txt");

            using (var sw = new System.IO.StreamWriter(fileName))
            {
                sw.WriteLine("HÓA ĐƠN BÁN HÀNG");
                sw.WriteLine($"Mã hóa đơn: {invoiceId}");
                sw.WriteLine($"Ngày: {invoice.InvoiceDate}");
                sw.WriteLine("Sản phẩm:");
                foreach (var d in details)
                {
                    var product = db.Products.Find(d.ProductId);
                    sw.WriteLine($"{product.ProductName} x{d.Quantity} - {d.UnitPrice} = {d.Quantity * d.UnitPrice}");
                }
                sw.WriteLine($"Mã giảm giá: {invoice.DiscountCode ?? "Không"}");
                sw.WriteLine("Tổng cộng: " + invoice.TotalAmount);
            }
            lastInvoiceFile = fileName;
            MessageBox.Show($"Đã in hóa đơn ra file: {fileName}");
        }
    }
}
