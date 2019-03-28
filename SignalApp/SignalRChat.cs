using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalApp.Data;
using SignalApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalApp
{
    public class SignalRChat : Hub      // jeśli chcemy korzystać zSignalR to nasz klasa musi diedziyczyćz Hub (MVCma wbudowany SignalR)
    {

        private static List<AppUser> connectedUsers = new List<AppUser>();

        private UserManager<IdentityUser> _mgr;
        private IHttpContextAccessor _httpContext;
        private ApplicationDbContext _appDb;

        public SignalRChat(UserManager<IdentityUser> mgr, IHttpContextAccessor httpContext, ApplicationDbContext appDb)
        {
            _mgr = mgr;
            _httpContext = httpContext;
            _appDb = appDb;
            
        }


        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReciveMessage", user, message);        // to jest moment kiedy server robi broadcast - w tym stringu jest nazwa metody jaka ma zostać utruchomiona w kliencie

            //Clients.Client("").SendAsync();                                        // jeśli damy nie all tylko client to możemy wpisać w stringu id danego uzytkownika i do niego wysyłąć tylko
        }

        public override Task OnConnectedAsync()                                 //HUB posiada taką metodą virtualną, możemy ją nadpisać - to jest metoda wywołąna dla każdego połączenia i mamy w niej dostęp do cennectionId każdego połączenia - clienta
        {

            string userId = _mgr.GetUserId(_httpContext.HttpContext.User);                        //to jest taki mechanizm, który nam sprawdzi czy istnieje usero danym UserName i jeśli istnieje (istniejący user przeładował stronę)
            var checkUser = connectedUsers.Where(x => x.Id == userId).FirstOrDefault();         // to usuwamy takiego usera z naszej listy userów żeby jak póżniej go dodamy do listy przez Add to żeby każdy user miał tylko jedno connectionId
            if(checkUser != null)
            {
                connectedUsers.Remove(checkUser);
            }



            AppUser user = new AppUser();
       
            string conId = Context.ConnectionId;                                //tutaj amy ID każeego clienta :) to nas ustawia elegancko


            user.ConnectionId = conId;
            user.Id = _mgr.GetUserId(_httpContext.HttpContext.User);            //controllety dziedziczą HttpContext, tutaj musieliśmy go wstrzyknąć do konstruktora żeby go mieć
            userId = user.Id;
            connectedUsers.Add(user);         
            //dodajemy naszego usera do listy userów podłączonych - będziemy mogli ich szukać po UserId, bo to nasze connectionId się zmienia

            //Clients.All.SendAsync("ReciveMessage2", userId, conId);

            var json = JsonConvert.SerializeObject(connectedUsers);

            Clients.All.SendAsync("ReciveMessage3", json);

            return base.OnConnectedAsync();
        }

        public void SendToUse(string userId, string msg)
        {
            var connectionId = connectedUsers.Where(x => x.Id == userId).Select(x => x.ConnectionId).FirstOrDefault();  //zapytanie linqu któreszuka w liście takiego obiektu x gdzie x.Id == userId(nasz argument) i bierzemy jego Id
            string myName = _httpContext.HttpContext.User.Identity.Name;                                                //bierzemy sobie userName
            Clients.Client(connectionId).SendAsync("ReciveMessage", myName, msg);                                       //i uruchamiamy na tym userze tę funkcję javascriptowąprzesyłąjącodpowiednie argumenty
        }

        public async Task SendMessageToUser(string conId, string msg)               //zjavascriptu uruchomilismy te motedą przez connection.invoke
        {
            string userName = _httpContext.HttpContext.User.Identity.Name;
            await Clients.Client(conId).SendAsync("ReciveMessage4", userName, msg, Context.ConnectionId);         //jako 3 parametr dodajemy nasze id                    //metodąclient mozemy wyslac do jednego ona przyjmuje connectionId

            string userId = _mgr.GetUserId(_httpContext.HttpContext.User);
            var Friend = connectedUsers.Where(x => x.ConnectionId == conId).FirstOrDefault();

            SignalMessage newMessage = new SignalMessage();
            newMessage.UserId = userId;
            newMessage.Friend = Friend.Id;
            newMessage.DateCreated = DateTime.Now;
            newMessage.Message = msg;

            _appDb.Add(newMessage);
            _appDb.SaveChanges();


        }
    }
}
