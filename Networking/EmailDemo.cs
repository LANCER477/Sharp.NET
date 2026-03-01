using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace SharpKnP321.Networking
{
    internal class EmailDemo
    {
        private class EmailConfig
        {
            public string Server { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public int Port { get; set; }
            public bool IsSsl { get; set; }
        }

        private static EmailConfig? _cachedConfig;

        private static EmailConfig GetConfiguration()
        {
            if (_cachedConfig != null)
            {
                return _cachedConfig;
            }

            string settingsFilename = "appsettings.json";
            if (!File.Exists(settingsFilename))
            {
                throw new FileNotFoundException("Не знайдено файл конфігурації. Прочитайте Readme");
            }

            try
            {
                var settings = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(settingsFilename));
                var emailSection = settings.GetProperty("Emails");
                var gmailSection = emailSection.GetProperty("Gmail");

                _cachedConfig = new EmailConfig
                {
                    Server = gmailSection.GetProperty("Server").GetString()!,
                    Email = gmailSection.GetProperty("Username").GetString()!,
                    Password = gmailSection.GetProperty("Password").GetString()!,
                    Port = gmailSection.GetProperty("Port").GetInt32(),
                    IsSsl = gmailSection.GetProperty("Ssl").GetBoolean()
                };

                return _cachedConfig;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка визначення конфігурації: {ex.Message}", ex);
            }
        }

        public void SendEmail(string toAddress, string subject, string htmlBody)
        {
            var config = GetConfiguration();

            try
            {
                using SmtpClient smtpClient = new()
                {
                    Host = config.Server,
                    Port = config.Port,
                    EnableSsl = config.IsSsl,
                    Credentials = new NetworkCredential(config.Email, config.Password),
                };

                MailMessage mailMessage = new()
                {
                    From = new MailAddress(config.Email),
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = htmlBody,
                };

                mailMessage.To.Add(new MailAddress(toAddress));
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new SmtpException($"Помилка під час надсилання листа на адресу {toAddress}", ex);
            }
        }

        public void Run()
        {
            Console.WriteLine("Робота з електронною поштою. SMTP");

            try
            {
                SendEmail(
                    "azure.spd111.od.0@ukr.net",
                    "Message from Sharp",
                    @"<html>
                        <h1>Шановний користувач!</h1>
                        <p>Шоби ви були здорові!<p>
                        <a style='background:maroon;color:snow;border-radius:10px;padding:7px 12px' 
                           href='https://itstep.org'>Вчитись</a>
                    </html>"
                );

                Console.WriteLine("Лист успішно надіслано.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виняток: {ex.Message}");
            }
        }
    }
}
/* Робота з електронною поштою. SMTP
 * * Організація збереження парольних даних.
 * При роботі з мережними сервісами доволі часто потрібні паролі, ключі тощо.
 * Основні проблеми виникають при публікації проєкту у репозиторії, особливо,
 * відкритого типу (public).
 * Одне з рішень - змінні оточення, проте, це ускладнює поширення проєкту.
 * Інше рішення - файли конфігурації: один з паролями та вилучений з репозиторію,
 * другий зразковий з правильними ключами, але видаленими паролями.
 * - визначаємось з назвою файлу: appsettings.json
 * - вносимо до .gitignore відповідний запис (до створення файлу), зберігаємо зміни
 * - створюємо сам файл, переконуємось, що він не фіксується у змінах репозиторію
 * - заповнюємо файл даними
 * - створюємо копію appsettings_sample.json, у якій видаляємо парольну інформацію
 * (замінюється на *** чи шаблони)
 * - додаємо до репозиторію інструкцію зі встановлення (інсталяції) - README.MD
 * * * Program                   Gmail(Server)                   Ukr.Net(Client, box)
 * SMTP
 * Send ---------------------->      -------------------------->
 * to: ukr.net                      to: ukr.net   
 * from: gmail
 * * Д.З. Реалізувати метод, який візьме на себе надсилання E-mail повідомлення
 * (враховуючи запит конфігурації, у т.ч. кешування та інших налаштувань)
 * За наявності помилок метод має генерувати винятки.
 */