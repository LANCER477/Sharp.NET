using SharpKnP321.Users.Dal;
using SharpKnP321.Users.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpKnP321.Users
{
    internal record MenuItem(char Key, String Title, Action? action)
    {
        public override string ToString()
        {
            return $"{Key} - {Title}";
        }
    };

    internal class UsersDemo
    {
        private DataAccessor _accessor = null!;
        private MenuItem[] menu => [
            new MenuItem('i', "Таблицы БД",() => _accessor.Install()),
            new MenuItem('h', "Переінсталювати Таблицы БД",() => _accessor.Install(isHard:true)),
            new MenuItem('1', "Реєстрація нового користувача", SignUp),
            new MenuItem('2', "Вхід до системи (автентифікація)", SignIn),
            new MenuItem('0', "Вихід", null),
        ];

        public void Run()
        {
            try
            {
                _accessor = new();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            MenuItem? selectedItem;
            do
            {
                foreach (var item in menu)
                {
                    Console.WriteLine(item);
                }
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.WriteLine();
                selectedItem = menu.FirstOrDefault(item => item.Key == keyInfo.KeyChar);
                if (selectedItem is null)
                {
                    Console.WriteLine("Нерозпізнаний вибір");
                }
                else
                {
                    selectedItem.action?.Invoke();
                }
            } while (selectedItem == null || selectedItem.action != null);
        }

        private void SignIn()
        {
            Console.WriteLine("SignIn");
        }

        private void SignUp()
        {
            UserData userData = new();
            bool isEntryCorrect;
            Console.WriteLine("Регистрация");
            
            do
            {
                Console.WriteLine("Полное имя");
                userData.UserName = Console.ReadLine()!;
                if (userData.UserName == String.Empty) return;
                isEntryCorrect = userData.UserName.Length > 2;
                if (userData.UserName.Length < 2)
                {
                    Console.WriteLine("Слишком короткий");
                }
            } while (!isEntryCorrect);
            
            do
            {
                Console.WriteLine("email");
                userData.UserEmail = Console.ReadLine()!.Trim()!;
                if (userData.UserEmail == String.Empty) return;
                isEntryCorrect = Regex.IsMatch(userData.UserEmail,@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
                if (!isEntryCorrect)
                {
                    Console.WriteLine("Не відповідає формату");
                }
            } while (!isEntryCorrect);

            // ДЗ: Валідація паролю
            string validPassword = ReadAndValidatePassword();
            Console.WriteLine("Пароль успішно прийнято!"); 
            
        }

        // ДЗ: Валідація паролю 
        private string ReadAndValidatePassword()
        {
            while (true)
            {
                Console.WriteLine("Введіть пароль для реєстрації: ");
                string password = Console.ReadLine() ?? "";

                List<string> errors = new List<string>();

                if (password.Length < 6)
                {
                    errors.Add("- довжина не менша 6 символів");
                }
                if (!password.Any(char.IsDigit))
                {
                    errors.Add("- містить щонайменше одну цифру");
                }
                if (!password.Any(c => !char.IsLetterOrDigit(c)))
                {
                    errors.Add("- містить щонайменше один спецсимвол (не літера, не цифра)");
                }
                if (!password.Any(char.IsLower))
                {
                    errors.Add("- містить щонайменше одну літеру нижнього реєстру (малу)");
                }
                if (!password.Any(char.IsUpper))
                {
                    errors.Add("- містить щонайменше одну літеру верхнього реєстру (велику)");
                }

                if (errors.Count == 0)
                {
                    return password; 
                }

                Console.WriteLine("\nПароль не відповідає вимогам безпеки. Порушені критерії:");
                foreach (string error in errors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine("Будь ласка, спробуйте ще раз.\n");
            }
        }
       
    }
}
/* Робота з користувачами: реєстрація, автентифікація, авторизація
 * * UserData        UserAccess        AccessTokens
 * UserId          AccessId          TokenId
 * UserName        AccessLogin       AccessId
 * UserEmail       AccessSalt        TokenIat
 * UserDelAt       AccessDk          TokenExp 
 * */
