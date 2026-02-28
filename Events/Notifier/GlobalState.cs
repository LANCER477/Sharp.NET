using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Events.Notifier
{
    internal class GlobalState
    {
        public double Price { get; set; }
        public DateTime LastSyncMoment { get; set; }
        public String? UserName { get; set; }
        public String ActivePage { get; set; } = "Home";
    }
}
/* Формалізм ChangeNotifier передбачає запуск підписників з передачею 
 * до них імені поля/властивості, що зазнало зміни.
 * Наприклад, на сторінці Price потрібна інформація про ціну,
 * на сторінці Router - про ActivePage
 * на сторінці Profile - про UserName
 * на всіх сторінках - LastSyncMoment
 * 
 * Д.З. Створити систему ChangeNotifier - три "сторінки" - об'єкти 
 * PricePage, ProfilePage, RouterModule
 * В рамках них підписатись на зміни стану, але реагувати тільки на 
 * ті поля, які становлять "інтерес".
 * Зробити демонстраційний проєкт, в якому забезпечується зміна 
 * полів один за одним з виведення реакцій об'єктів-"сторінок" на них
 * 
 * GlobalState.Price = 200
 *  PricePage: new price 200
 * GlobalState.UserName = "User"
 *  ProfilePage: new user - User
 * GlobalState.ActivePage = "Profile"
 *  RouterModule: new page - Profile
 * GlobalState.LastSyncMoment = DateTime.Now
 *  PricePage: new sync at ....
 *  ProfilePage: new sync at ....
 *  RouterModule: new sync at ....
 */