using System;
using System.Net;
using System.Net.Mail;
///библиотека классов. которая отвечает за расслку сообщений в зависимости от введенной почты
namespace Wallofregist.MailBox
{
    internal class YandexMailSender
    {
        public static string SendMailYandex(string userEmail)
        {
            string yandexAppPassword = "eqnzyuimfailzhjd";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("egor18425@yandex.ru");
            mail.To.Add(userEmail); 
            mail.Subject = "Код подтверждения";
            string confirmationCode = GenerateFourDigitCode(); 
            mail.Body = $"Ваш код подтверждения: {confirmationCode}"/*отправка в теле письма сгенеррированный кода*/;
            SmtpClient smtpClient = new SmtpClient("smtp.yandex.ru");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("egor18425@yandex.ru"/* почта с которой отправляется письмо при сбросе пароля*/, yandexAppPassword)/*пароль приложения для автоматической отправки писем*/;
            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Письмо успешно отправлено");
                return confirmationCode; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                return null; 
            }
        }

        private static string GenerateFourDigitCode()//генерация 4-значного случайного кода
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString("D4");
        }
    }

    internal class MailRuMailSender
    {

        public static string SendMailRu(string userEmail)
        {
            string mailRuAppPassword = "4hsdjXTmzb9tKBKPTQ9F";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("egorantonov1804@bk.ru");
            mail.To.Add(userEmail); 
            mail.Subject = "Код подтверждения";
            string confirmationCode = GenerateFourDigitCode();
            mail.Body = $"Ваш код подтверждения: {confirmationCode}";
            SmtpClient smtpClient = new SmtpClient("smtp.mail.ru");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("egorantonov1804@bk.ru"/* почта с которой отправляется письмо при сбросе пароля*/, mailRuAppPassword/*пароль приложения для автоматической отправки писем*/);
            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Письмо успешно отправлено");
                return confirmationCode; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                return null; 
            }
        }
        private static string GenerateFourDigitCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString("D4");
        }
    }


    internal class GmailSender
    {
        public static string SendGMail(string userEmail)
        {
            string gmailAppPassword = "navp wxkd uluc osja";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("egorantonov1804@gmail.com");
            mail.To.Add(userEmail); 
            mail.Subject = "Код подтверждения";
            string confirmationCode = GenerateFourDigitCode(); 
            mail.Body = $"Ваш код подтверждения: {confirmationCode}";
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("egorantonov1804@gmail.com"/* почта с которой отправляется письмо при сбросе пароля*/, gmailAppPassword/*пароль приложения для автоматической отправки писем*/);
            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Письмо успешно отправлено");
                return confirmationCode; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                return null; 
            }
        }
        private static string GenerateFourDigitCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString("D4");
        }
    }
}

