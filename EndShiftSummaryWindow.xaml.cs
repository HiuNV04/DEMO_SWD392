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
    /// Interaction logic for EndShiftSummaryWindow.xaml
    /// </summary>
    public partial class EndShiftSummaryWindow : Window
    {
        private MiniMartDbContext db = new MiniMartDbContext();
        private int cashierUserId;
        private List<Invoice> invoices;
        public EndShiftSummaryWindow(int userId)
        {
            InitializeComponent();
            cashierUserId = userId;
            dpStart.SelectedDate = DateTime.Today;
            dpEnd.SelectedDate = DateTime.Today;
            LoadInvoices();
        }
        private void LoadInvoices()
        {
            DateTime startDate = dpStart.SelectedDate ?? DateTime.Today;
            DateTime endDate = dpEnd.SelectedDate ?? DateTime.Today;

            // Lấy hóa đơn của cashier này
            invoices = db.Invoices
                .Where(inv => inv.UserId == cashierUserId
                    && inv.InvoiceDate >= DateOnly.FromDateTime(startDate)
                    && inv.InvoiceDate <= DateOnly.FromDateTime(endDate))
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string q = txtSearch.Text.Trim().ToLower();

            var result = invoices.Where(inv =>
                (string.IsNullOrEmpty(q) ||
                inv.InvoiceId.ToString().Contains(q) ||
                (inv.InvoiceDate != null && inv.InvoiceDate.ToString().Contains(q)) ||
                (inv.DiscountCode != null && inv.DiscountCode.ToLower().Contains(q)) ||
                (inv.TotalAmount != null && inv.TotalAmount.ToString().Contains(q)))
            ).Select(inv => new
            {
                inv.InvoiceId,
                InvoiceDate = inv.InvoiceDate.ToString(),
                inv.DiscountCode,
                inv.TotalAmount
            }).ToList();

            dgInvoice.ItemsSource = result;
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadInvoices();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnEndShift_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Kết thúc ca thành công. Đăng xuất!", "End Shift", MessageBoxButton.OK, MessageBoxImage.Information);
            // Quay về màn hình Login
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
          
        }

    }
}
