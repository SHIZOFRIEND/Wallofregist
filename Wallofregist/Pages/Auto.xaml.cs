using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wallofregist.Models;

using System.Net.Http;
using System.Net.Http.Json;

using HashLib;
using Wallofregist.MailBox;
 

namespace Wallofregist.Pages
{
    public partial class Auto : Page
    {
        private int countUnsuccessful = 0;
        private DateTime unlockTime;

        public Auto()
        {
            InitializeComponent();
            txtBlockCaptcha.Visibility = Visibility.Hidden;
            txtboxCaptcha.Visibility = Visibility.Hidden;
        }
        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Gost(null));
            
        }
        private async void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtbLogin.Text.Trim();
            string password = pswbPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Логин и пароль не должен быть пустыми");
                return;
            }

            var loginRequest = new LoginRequest { Login = login, Password = password };

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync("https://localhost:7235/api/auth/login", loginRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                        if (loginResponse.TwoFactorEnabled)
                        {
                            string confirmationCode = null;
                            string userEmail = loginResponse.Email;

                            if (userEmail.Contains("@yandex.ru"))
                            {
                                confirmationCode = YandexMailSender.SendMailYandex(userEmail);
                            }
                            else if (userEmail.Contains("@mail.ru"))
                            {
                                confirmationCode = MailRuMailSender.SendMailRu(userEmail);
                            }
                            else if (userEmail.Contains("@gmail.com"))
                            {
                                confirmationCode = GmailSender.SendGMail(userEmail);
                            }

                            ConfirmWindow confirmWindow = new ConfirmWindow(confirmationCode);
                            bool? result = confirmWindow.ShowDialog();

                            if (result == true)
                            {
                                MessageBox.Show("Вы вошли под: " + loginResponse.Role);
                                LoadForm(loginResponse.Role);
                            }
                            else
                            {
                                MessageBox.Show("Введенный код неверный. Пожалуйста, попробуйте еще раз.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Вы вошли под: " + loginResponse.Role);
                            LoadForm(loginResponse.Role);
                        }
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Произошла ошибка: {response.StatusCode}, {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void UnsuccessfulLogin()
        {
            countUnsuccessful++;
            GeneratCaptcha();
            MessageBox.Show("Вы ввели неверный логин или пароль");
        }

        private void SuccessfulLogin(string roleName)
        {
            MessageBox.Show("Вы вошли под: " + roleName);
            LoadForm(roleName);
            ClearFields();
        }

        private void ValidateAndUnlockAccount(int polzovCount)
        {
            BazaDan bd = new BazaDan();
            string enteredCaptcha = txtboxCaptcha.Text.Trim();

            if (polzovCount > 0 && ValidateCaptcha(enteredCaptcha))
            {
                SuccessfulLogin(bd.Polzovateli.First(_ => _.Logini == txtbLogin.Text.Trim()).Roli.NazvanieRoli.ToString());
                txtBlockCaptcha.Visibility = Visibility.Hidden;
                txtboxCaptcha.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Вы ввели неверный логин или пароль или неверно ввели капчу, вам следует ждать");
                GeneratCaptcha();
              
                if (DateTime.Now < unlockTime)
                {
                    btnEnter.IsEnabled = false;
                    txtbLogin.IsEnabled = false;
                    pswbPassword.IsEnabled = false;
                    int secondsRemaining = (int)(unlockTime - DateTime.Now).TotalSeconds;
                    UnlockAccount();
                }
                else
                {
                    unlockTime = DateTime.Now.AddSeconds(10);
                    countUnsuccessful = 0;
                    btnEnter.IsEnabled = false;
                    txtbLogin.IsEnabled = false;
                    pswbPassword.IsEnabled = false;
                    UnlockAccount();

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += (s, e) =>
                    {
                        btnEnter.IsEnabled = true;
                        txtbLogin.IsEnabled = true;
                        pswbPassword.IsEnabled = true;
                        timer.Stop();
                    };

                    timer.Interval = TimeSpan.FromSeconds(10);
                    timer.Start();
                }
            }
        }

        private void GeneratCaptcha()//генерация капчика 
        {
            txtBlockCaptcha.Visibility = Visibility.Visible;
            txtboxCaptcha.Visibility = Visibility.Visible;
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int captchaLength = 6;
            string captcha = new string(Enumerable.Repeat(chars, captchaLength).Select(s => s[random.Next(s.Length)]).ToArray());

            txtBlockCaptcha.Text = captcha;
            txtBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void ClearFields()
        {
            txtbLogin.Clear();
            pswbPassword.Clear();
            txtboxCaptcha.Clear();
        }

        private bool ValidateCaptcha(string enteredCaptcha)
        {
            string actualCaptcha = txtBlockCaptcha.Text.Trim();
            return string.Equals(enteredCaptcha, actualCaptcha, StringComparison.OrdinalIgnoreCase);
        }

        private void LoadForm(string roleName)//распредление ролей 
        {
            string login = txtbLogin.Text.Trim();
            string password = pswbPassword.Password;
            string passwort = HashPassword.HashPasswort(password.Trim().Trim());

            BazaDan bd = new BazaDan();
            Polzovateli polzov = bd.Polzovateli.FirstOrDefault(_ => _.Logini == login && _.Paroli == passwort);
            switch (roleName)
            {
                case "Клиент":
                    NavigationService.Navigate(new Client(polzov));
                    break;
                case "Врач":
                    NavigationService.Navigate(new Vrach(polzov));
                    break;
                case "Директор":
                    NavigationService.Navigate(new Director(polzov));
                    break;
                case "Оператор":
                    NavigationService.Navigate(new Operator(polzov));
                    break;
                case "Юрист":
                    NavigationService.Navigate(new Jurist(polzov));
                    break;
                case "Администратор":
                    NavigationService.Navigate(new Admin());
                    break;
            }
        }
      
    
 
        private async void UnlockAccount()
        {
            int secondsRemaining = (int)(unlockTime - DateTime.Now).TotalSeconds;

            txtBlockRemainingTime.Text = $"Remaining Time: {secondsRemaining} seconds";
            txtBlockRemainingTime.Visibility = Visibility.Visible;

            while (secondsRemaining >= 0)
            {
                await Task.Delay(1000); 

                secondsRemaining--;

                Dispatcher.Invoke(() =>
                {
                    txtBlockRemainingTime.Text = $"Remaining Time: {secondsRemaining} seconds";
                });
            }

            txtBlockRemainingTime.Visibility = Visibility.Hidden;
            btnEnter.IsEnabled = true;
            txtbLogin.IsEnabled = true;
            pswbPassword.IsEnabled = true;
        }

        private void btnforget_Click(object sender, RoutedEventArgs e)
        {
            PasswordRecoveryWindow passwordRecoveryWindow = new PasswordRecoveryWindow();
            passwordRecoveryWindow.Show();
            
        }
    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string Role { get; set; }
    }
}

