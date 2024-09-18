using System;
using System.Text;
using System.Security.Cryptography;
using Wallofregist.Models;
/// в данном классе представлен код добавления нового польхователя в базу данных, все моменты очевидны
/// также этот код ссылается на библиотеку классов HashLib,поскольку там лежит метод хеширования пароля
namespace HashPasswort
{
    public class Hashik
    {
        public static void Main(string[] args)
        {
            Console.Write("Введите Login: ");
            string login = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            Console.Write("Введите фамилию: ");
            string lastName = Console.ReadLine();

            Console.Write("Введите имя: ");
            string firstName = Console.ReadLine();

            Console.Write("Введите id роли: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Введите наличие двухфакторной аутефикации(1-включить,0- выключить: ");
            int twofactoravtor = Convert.ToInt32(Console.ReadLine());


            string hashedPassword = HashLib.HashPassword.HashPasswort(password);//ссылка на метод хеширования
            Console.WriteLine("Хешированный пароль: " + hashedPassword);

            using (var context = new BazaDan())
            {
                var newUser = new Polzovateli()
                {
                    Logini = login,
                    Paroli = hashedPassword,
                    LastName = lastName,
                    FirstName = firstName,
                    IDRoli = id,
                    TwoFactorAvtor = twofactoravtor

                };

                context.Polzovateli.Add(newUser);
                context.SaveChanges();

            }

            Console.WriteLine("Пользователь успешно добавлен в базу данных.");
        }
    }
}
