using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.AudioBypassService.Extensions;
using Microsoft.Extensions.DependencyInjection;


namespace VKCrypto_reborn_win_
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            ServiceCollection servies = new ServiceCollection();
            servies.AddAudioBypass();
            Utils.Userapi = new VkApi(servies);
            try
            {
                string docPath = Environment.CurrentDirectory;
                string[] lines = File.ReadAllLines(Path.Combine(docPath, "LoginData.txt"));
                var login = lines[0];
                var password = lines[1];
                Auth(login, password);
                MainWindow main = new MainWindow();
                Close();
                main.Show();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                InitializeComponent();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Auth();
        }
        private void Password_holder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Auth(); }
        }
        public void Auth()
        {
            string login = Login_holder.Text;
            string password = Password_holder.Password;
            Utils.Userapi.Authorize(new ApiAuthParams
            {
                ApplicationId = 6723320,
                Login = login,
                Password = password,
                Settings = Settings.All
            });
            if ((bool)Remember_me.IsChecked)
            {
                Save_Data(login, password);
            }
        }
        
        public void Auth(string login, string password)
        {
            Utils.Userapi.Authorize(new ApiAuthParams
            {
                ApplicationId = 6723320,
                Login = login,
                Password = password,
                Settings = Settings.All
            });
        }
        private static void Save_Data(string login, string password)
        {
            string docPath = Environment.CurrentDirectory;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "LoginData.txt"), true))
            {
                outputFile.WriteLine(login);
                outputFile.WriteLine(password);
                outputFile.Close();
                outputFile.Dispose();
            }
        }
    }
}
