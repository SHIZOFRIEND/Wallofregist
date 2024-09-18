using System;
using System.Text;
using System.Security.Cryptography;
using Wallofregist.Models;


namespace HashPasswort
{
    class Program
    {
        static void Main(string[] args)
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


            string hashedPassword = HashPasswort(password);
            Console.WriteLine("Хешированный пароль: " + hashedPassword);

            using (var context = new BDRegist())
            {
                var newUser = new Polzovateli()
                {
                    Logini = login,
                    Paroli = hashedPassword,
                    LastName = lastName,
                    FirstName = firstName,
                    IDRoli = id

                };

                context.Polzovateli.Add(newUser);
                context.SaveChanges();

            }

            Console.WriteLine("Пользователь успешно добавлен в базу данных.");

          
        }

        
        
    }
}
