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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wallofregist.Models;

namespace Wallofregist.Pages
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Page
    {
       

        public Client(Polzovateli polzov)
        {
            InitializeComponent();
            AuthenticateUser(polzov);
        }

        private void AuthenticateUser(Polzovateli polzov)//вывод данных о сотруднике переданный из окна авторизации, передача осуществилась в методе LoadForm
        {
            if (UserCanAccessSystem())
            {
                string greeting = GetTimeOfDayGreeting();
                txtfamilia.Text = $"{polzov.LastName} {polzov.FirstName} {polzov.Logini}";
                txttime.Text = greeting;
            }
            else
            {
                txtfamilia.Text = "Доступ заблокирован. Рабочее время: 10:00 - 19:00";
                txttime.Text = string.Empty;
            }
        }

        private bool UserCanAccessSystem()//доступ в временных рамках
        {
            DateTime now = DateTime.Now;
            return now.TimeOfDay >= new TimeSpan(10, 0, 0) && now.TimeOfDay <= new TimeSpan(19, 0, 0);
        }

        private string GetTimeOfDayGreeting()//диапозоны времени
        {
            DateTime now = DateTime.Now;
            if (now.TimeOfDay >= new TimeSpan(10, 0, 0) && now.TimeOfDay <= new TimeSpan(12, 0, 0))
                return "Доброе утро";
            else if (now.TimeOfDay > new TimeSpan(12, 0, 0) && now.TimeOfDay <= new TimeSpan(17, 0, 0))
                return "Добрый день";
            else if (now.TimeOfDay > new TimeSpan(17, 0, 0) && now.TimeOfDay <= new TimeSpan(19, 0, 0))
                return "Добрый вечер";
            else
                return "Пора байки";
        }
    }
}
