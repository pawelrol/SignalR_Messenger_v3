using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignalApp.Data;
using SignalApp.Models;

namespace SignalApp.Controllers
{
    [Authorize]                                     //dodajemy sobie [Authorize] i to MVC nas przekierowywuje i każe się zalogować / zarejestrować
    public class HomeController : Controller
    {
        private UserManager<IdentityUser> _mgr;
        private ApplicationDbContext _db;


        public HomeController(UserManager<IdentityUser> mgr, ApplicationDbContext db)
        {
            _db = db;
            _mgr = mgr;

        }


        public IActionResult Search(string query)
        {
            var result = _db.Users.Where(x => x.UserName.StartsWith(query)).ToList();                  // mamy wjazd do Users bo nasz wybraliśmy sobie wceśniej jak stawialiśmy projekt na początku więc on sobie zrobiłsam te klasy odpowiednie, dlatego w bazie danych zrobily się same takie tabele, one są przed nami ukryte ale są normalnie jak zwykłę model i tabele w klasie kontekstowej
                                                                                                        // nasz result to lista wszystkich UserNamów zaczynających się na toco mu wpiszemy w wyszukiwarce w formularzu
            return View(result);
        }

        public JsonResult AddFriend(string id)                  //wywołujemy Ajaxem więc musi być JsonResult
        {
            FriendList list = new FriendList();                 //to jest nasza lista znajomych danego usera
            list.FriendId = id;                                 //dodajemy mu frined Id
            //zalogowany userId
            string userId = _mgr.GetUserId(HttpContext.User);   //wyciągamy od zalogowanego użytkownika jego Id - ono istniej w kontekscie

            if(                                                                                 //sprawdzamy czy użytkownicy już nie mają siew znajomych albo jeden drugiego, albo drugi pierwszego
                !_db.FriendLists.Any(x => x.UserId == x.FriendId && x.FriendId == id)
                && 
                !_db.FriendLists.Any(x => x.UserId == id && x.FriendId == userId)
               )
            {
                list.UserId = userId;                               //dodajemy jego Id
                _db.FriendLists.Add(list);                          //aktualizujemy bazę danych
                _db.SaveChanges();                                  //zapisujemy do bazy danych
            }

            

            return Json(true);
        }

        public IActionResult Index()
        {
            string userId = _mgr.GetUserId(HttpContext.User);   //wyciągamy od zalogowanego użytkownika jego Id - ono istniej w kontekscie
            //lista znajomych których zaprosiłem
            var friendList = _db.FriendLists.Where(x => x.UserId == userId).Select(x => x.FriendId).ToList();   //wyciągamy sobie z bazy danych listę znajomy w ktorej występuje nasz userId
            //lista znajomych którzy mnie zaprosili
            var requestList = _db.FriendLists.Where(x => x.FriendId == userId).Select(x => x.UserId).ToList();


            //ze wszystkich uzytkownikow poprosze tych ktorych id znajduje się w jednej albo w drugiej liscie
            var friends = _mgr.Users.Where(x => friendList.Contains(x.Id) || requestList.Contains(x.Id)).ToList();  // zdobyliśmy listę znajomych



            // string userId = _mgr.GetUserId(HttpContext.User);           //w ten sposób dostajemy się do zalogowanego usera do jego ID, wstrzyknęliśmy sobie do konstruktora Dependancy Injection klasą domyślnego usera (jest zdefiniowany w startup)
            return View(friends);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
