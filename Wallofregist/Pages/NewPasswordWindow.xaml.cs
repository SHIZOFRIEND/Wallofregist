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
using Wallofregist.Models;
using Wallofregist.Pages;

namespace Wallofregist.Pages
{
    /// <summary>
    /// Логика взаимодействия для NewPasswordWindow.xaml
    /// код для смены пароля
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        private string userEmail;
        public NewPasswordWindow(string userEmail)
        {
            InitializeComponent();
            this.userEmail = userEmail;
        }

        private void SaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают. Пожалуйста, попробуйте снова.");
                return;
            }

            
            ChangePassword(userEmail, newPassword);
        }
        private void ChangePassword(string userEmail, string newPassword)//функция поиска сотрудника по почте, а далее пользователя по логину, чтобы в таблице пользователей сменть паролик
        {

            BazaDan bd = new BazaDan();

            try
            {
                Sotrydniki sotrydnik = bd.Sotrydniki.FirstOrDefault(s => s.Pochta == userEmail);

                if (sotrydnik != null)
                {
                    Polzovateli polzovatel = bd.Polzovateli.FirstOrDefault(p => p.IDPolzovateliaDlyaAvtorizacii == sotrydnik.IDPolzovateliaDlyaAvtorizacii);

                    if (polzovatel != null)
                    {
                        
                        string hashedPassword = HashLib.HashPassword.HashPasswort(newPassword);//хешируем и чуть ниже происходит сохранение
                        polzovatel.Paroli = hashedPassword;

                        bd.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с указанной почтой не найден в таблице Polzovateli.");
                    }
                }
                else
                {
                    MessageBox.Show("Сотрудник с указанной почтой не найден в таблице Sotrydniki.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при изменении пароля: " + ex.Message);
            }
        }
    }
}