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
using Wallofregist.MailBox;

namespace Wallofregist.Pages
{
    /// <summary>
    /// Логика взаимодействия для PasswordRecoveryWindow.xaml
    /// </summary>
    public partial class PasswordRecoveryWindow : Window
    {
        private string generatedCode;
        private string userEmail;
        public PasswordRecoveryWindow()
        {
            InitializeComponent();

        }

        private void SendConfirmationCode_Click(object sender, RoutedEventArgs e)  //следует засунуть код в отдельный метод, поскольку точно такой же код лежит в другом окне
        {
            userEmail = txtUsernameOrEmail.Text;

            if (userEmail.Contains("@yandex.ru"))
            {
                generatedCode = YandexMailSender.SendMailYandex(userEmail);
                MessageBox.Show("Код подтверждения отправлен на вашу почту.");
            }
            else if (userEmail.Contains("@mail.ru"))
            {
                generatedCode = MailRuMailSender.SendMailRu(userEmail);
                MessageBox.Show("Код подтверждения отправлен на вашу почту.");
            }
            else if (userEmail.Contains("@gmail.com"))
            {
                generatedCode = GmailSender.SendGMail(userEmail);
                MessageBox.Show("Код подтверждения отправлен на вашу почту.");
            }
            else
            {
                MessageBox.Show("Указанный почтовый сервис не поддерживается");
            }
        }
        

        //тем самым лишние строки кода, считай 2 раза одно и тоже..
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = txtConfirmationCode.Text;

            
            if (ValidateConfirmationCode(enteredCode, generatedCode))
            {
                
                OpenNewPasswordWindow();
            }
            else
            {
                MessageBox.Show("Введенный код неверный. Пожалуйста, попробуйте снова.");
            }
        }
        private bool ValidateConfirmationCode(string enteredCode, string generatedCode)//сравнение кодов
        {
            return enteredCode == generatedCode;
        }
        private void OpenNewPasswordWindow()
        {
            
            NewPasswordWindow newPasswordWindow = new NewPasswordWindow(userEmail);
            newPasswordWindow.Show();

            
            this.Close();
        }
    }
}
