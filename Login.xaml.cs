using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Đọc cấu hình từ appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // Đặt thư mục hiện tại làm cơ sở
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);  // Đảm bảo appsettings.json tồn tại

            IConfiguration configuration = builder.Build();

            // Lấy thông tin tài khoản admin từ appsettings.json
            string adminUsername = configuration["AdminAccount:Username"];
            string adminPassword = configuration["AdminAccount:Password"];

            // Kiểm tra nếu thông tin người dùng nhập khớp với tài khoản admin
            if (UsernameTextBox.Text == adminUsername && PasswordBox.Password == adminPassword)
            {
                // Nếu là tài khoản admin và mật khẩu đúng, mở MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Đóng cửa sổ đăng nhập
                return; // Dừng lại ở đây, không kiểm tra tiếp
            }

            // Nếu không phải admin, kiểm tra tài khoản người dùng trong cơ sở dữ liệu
            string connectionString = configuration.GetConnectionString("MyCnn");
            string query = "SELECT RoleId FROM Users WHERE Username = @Username AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", UsernameTextBox.Text);
                command.Parameters.AddWithValue("@Password", PasswordBox.Password); // Kiểm tra mật khẩu người dùng

                connection.Open();
                var roleId = command.ExecuteScalar();

                if (roleId != null)
                {
                    // Nếu đăng nhập thành công và RoleId là Cashier (2), chuyển đến Cashier.xaml
                    if ((int)roleId == 2)
                    {
                        Cashier cashierWindow = new Cashier();
                        cashierWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        // Đối với role khác, mở MainWindow
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        
    }
}
