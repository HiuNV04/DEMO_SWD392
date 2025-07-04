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
    /// Interaction logic for UserDetailWindow.xaml
    /// </summary>
    public partial class UserDetailWindow : Window
    {
        private readonly MiniMartDbContext _context = new();
        private readonly User _user;

        public bool IsSaved { get; private set; } = false;

        public UserDetailWindow(User user)
        {
            InitializeComponent();
            _user = _context.Users.Find(user.UserId)!;

            // Load dữ liệu ban đầu
            txtUsername.Text = _user.Username;
            txtPassword.Text = _user.Password;
            txtFullName.Text = _user.AccountFullName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _user.Username = username;
            _user.Password = password;
            _user.AccountFullName = fullName;

            _context.SaveChanges();
            IsSaved = true;

            MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
