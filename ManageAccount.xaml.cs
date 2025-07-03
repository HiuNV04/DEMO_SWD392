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
    /// Interaction logic for ManageAccount.xaml
    /// </summary>
    public partial class ManageAccount : Window
    {
        private MiniMartDbContext _context = new MiniMartDbContext();
        public ManageAccount()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.AccountFullName,
                    u.Password,
                    u.RoleId
                })
                .ToList();

            dgUsers.ItemsSource = users;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim();
            LoadUsers(searchTerm); // Call LoadUsers with search term
        }

        private void LoadUsers(string searchTerm = "")
        {
            var usersQuery = _context.Users
                .Where(u => u.Username.Contains(searchTerm)) // Filter by username
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.AccountFullName,
                    u.Password,
                    u.RoleId
                })
                .ToList();

            dgUsers.ItemsSource = usersQuery;
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            string usernameInput = username.Text.Trim();
            string passwordInput = pass.Text.Trim();

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == usernameInput);

            if (existingUser != null)
            {
                // Nếu tài khoản đã tồn tại
                MessageBox.Show("Account already exists!", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var cashierRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Cashier");
                if (cashierRole == null)
                {
                    MessageBox.Show("Cashier role does not exist in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newUser = new User
                {
                    Username = usernameInput,
                    Password = passwordInput,
                    AccountFullName = "New User",
                    RoleId = cashierRole.RoleId
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                MessageBox.Show("Account successfully added!", "Registration Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Sau khi thêm tài khoản, bạn có thể làm mới danh sách người dùng hoặc đóng cửa sổ
                LoadUsers();
            }
        }

        private void ViewUserById_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int userId)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    var detailWindow = new UserDetailWindow(user);
                    detailWindow.ShowDialog();

                    if (detailWindow.IsSaved)
                    {
                        LoadUsers();
                    }
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DeleteUserById_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int userId)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete user '{user.Username}'?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Users.Remove(user);
                        _context.SaveChanges();
                        MessageBox.Show("User deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ManageProduct_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
