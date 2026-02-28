using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace SharpKnP321.Networking
{
    internal class EmailDemo
    {
        public void Run()
        {
            Console.WriteLine("Робота з електронною поштою. SMTP");
            String settingsFilename = "appsettings.json";
            if (!File.Exists(settingsFilename))
            {
                Console.WriteLine("Не знайдено файл конфігурації. Прочитайте Readme");
                return;
            }
            var settings = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(settingsFilename ) 
            );
            String server, email, password;
            int port;
            bool isSsl;
            try
            {
                var emailSection = settings.GetProperty("Emails");
                var gmailSection = emailSection.GetProperty("Gmail");
                server   = gmailSection.GetProperty("Server").GetString()!;
                email    = gmailSection.GetProperty("Username").GetString()!;
                password = gmailSection.GetProperty("Password").GetString()!;
                port     = gmailSection.GetProperty("Port").GetInt32();
                isSsl    = gmailSection.GetProperty("Ssl").GetBoolean();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Помилка визначення конфігурації: {ex.Message}");
                return;
            }

            using SmtpClient smtpClient = new()
            {
                Host = server,
                Port = port,
                EnableSsl = isSsl,
                Credentials = new NetworkCredential(email, password),
            };

            // базова форма - текстові повідомлення
            // smtpClient.Send(email, "azure.spd111.od.0@ukr.net", "Message from Sharp", "Hello, User!");

            // розширена форма 
            MailMessage mailMessage = new()
            {
                From = new MailAddress(email),
                IsBodyHtml = true,
                Subject = "Message from Sharp",
                Body = @"<html>
                    <h1>Шановний користувач!</h1>
                    <p>Шоби ви були здорові!<p>
                    <a style='background:maroon;color:snow;border-radius:10px;padding:7px 12px' 
                       href='https://itstep.org'>Вчитись</a>
                </html>",
            };
            mailMessage.To.Add(new MailAddress("azure.spd111.od.0@ukr.net"));
            smtpClient.Send(mailMessage);
        }
    }
}
/* Робота з електронною поштою. SMTP
 * 
 * Організація збереження парольних даних.
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
 *    (замінюється на *** чи шаблони)
 * - додаємо до репозиторію інструкцію зі встановлення (інсталяції) - README.MD
 * 
 * 
 * Program                   Gmail(Server)                   Ukr.Net(Client, box)
 *              SMTP
 * Send ---------------------->      -------------------------->
 *         to: ukr.net                      to: ukr.net   
 *         from: gmail
 *         
 * Д.З. Реалізувати метод, який візьме на себе надсилання E-mail повідомлення
 * (враховуючи запит конфігурації, у т.ч. кешування та інших налаштувань)
 * За наявності помилок метод має генерувати винятки.
 */