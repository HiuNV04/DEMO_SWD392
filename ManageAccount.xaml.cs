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

            // Kiểm tra nếu tài khoản đã tồn tại trong cơ sở dữ liệu
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == usernameInput);

            if (existingUser != null)
            {
                // Nếu tài khoản đã tồn tại
                MessageBox.Show("Account already exists!", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Nếu tài khoản chưa tồn tại, thêm tài khoản mới vào cơ sở dữ liệu
                var newUser = new User
                {
                    Username = usernameInput,
                    Password = passwordInput,  // Bạn có thể mã hóa mật khẩu trước khi lưu nếu cần
                    AccountFullName = "New User",  // Bạn có thể yêu cầu người dùng nhập tên đầy đủ nếu cần
                    RoleId = 2  // Ví dụ, gán RoleId = 2 cho "Cashier"
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                MessageBox.Show("Account successfully added!", "Registration Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Sau khi thêm tài khoản, bạn có thể làm mới danh sách người dùng hoặc đóng cửa sổ
                LoadUsers();
            }
        }

        private void dgUsers_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Get the selected user from DataGrid
            var selectedUser = dgUsers.SelectedItem as dynamic;

            if (selectedUser != null)
            {
                // Populate the TextBoxes with the user's information
                username.Text = selectedUser.Username;
                pass.Text = selectedUser.Password;
            }
        }
    }
}
