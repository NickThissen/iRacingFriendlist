using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iRacingFriendlist.Http;
using iRacingPmToolPluginLibrary;

namespace iRacingFriendlist.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool _shown;
        private bool _autoLogin;

        public LoginWindow()
        {
            InitializeComponent();

            var username = Properties.Settings.Default.Username;
            var password = Properties.Settings.Default.Password;

            if (!string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password))
            {
                txtUsername.Text = username;
                txtPassword.Password = password;
                _autoLogin = true;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
            _shown = true;

            if (_autoLogin) this.Login();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Login();
        }

        private void Login()
        {
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            var result = Global.Login(username, password);

            if (result == HttpPost.LoginResults.IncorrectCredentials)
            {
                MessageBox.Show("Incorrect credentials specified, please try again.", "Login failed");
            }
            else if (result == HttpPost.LoginResults.NoResponse)
            {
                MessageBox.Show("No response from iRacing membersite, please try again.", "Login failed");
            }
            else
            {
                if (chkRemember.IsChecked.GetValueOrDefault())
                {
                    Properties.Settings.Default.Username = username;
                    Properties.Settings.Default.Password = password;
                    Properties.Settings.Default.Save();
                }

                var window = new MainWindow();
                ((App) Application.Current).MainWindow = window;
                window.Show();

                this.Close();
            }
        }
    }
}
