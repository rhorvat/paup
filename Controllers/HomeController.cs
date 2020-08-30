using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PAUP.Models;

namespace PAUP.Controllers
{

    public class HomeController : Controller
    {
        private IgreDbContext _context = new IgreDbContext();
        public IActionResult Games(int id)
        {
            var igre = new List<Game>();
            switch (id)
            {
                case 1:
                    igre = _context.Games.ToList();
                    break;
                case 2:
                    igre = _context.Games.Where(x => x.Category == (int)Category.Akcijska).ToList();
                    break;
                case 3:
                    igre = _context.Games.Where(x => x.Category == (int)Category.Avanturisticka).ToList();
                    break;
                case 4:
                    igre = _context.Games.Where(x => x.Category == (int)Category.Sportska).ToList();
                    break;
                default:
                    igre = _context.Games.ToList();
                    break;
            }

            return View(igre);
        }

        public IActionResult GameDetails(string id)
        {
            var game = _context.Games.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            return View(game);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email,Password")] LoginUser user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {

                if (!EmailExist(user.Email))
                {
                    ModelState.AddModelError("Email", "Nepostojeći email");
                    return View(user);
                }
                else
                {
                    User logUser = _context.Users.FirstOrDefault(x => x.Email == user.Email);

                    if (Enkripcija.Hash(user.Password) != logUser.Password)
                    {
                        ModelState.AddModelError("Password", "Kriva zaporka");
                        return View(user);
                    }
                    else
                    {
                        if (logUser.AccountType == 1)
                        {

                            HttpContext.Session.SetObjectAsJson("user", logUser);
                            return RedirectToAction("Games", "Admin");
                        }
                        else if (logUser.AccountType == 2)
                        {

                            ModelState.AddModelError("Password", "Korisnik nije administrator");
                            return View(user);
                        }
                    }
                    return View(user);
                }
            }
        }

        [NonAction]
        public bool EmailExist(string email)
        {

            var exists = _context.Users.FirstOrDefault(x => x.Email == email);
            return exists != null;

        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {

                return View(new User());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("Name,Surname,Adress,Email,Password")] User user)
        {

            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {

                if (!ModelState.IsValid)
                {
                    return View(user);
                }
                else
                {

                    if (EmailExist(user.Email))
                    {
                        ModelState.AddModelError("Email", "Korisnik s tom email adresom vec postoji!");
                        return View(user);
                    }
                    else
                    {
                        int id = _context.Users.LastOrDefault().Id + 1;
                        user.Id = id;
                        user.Password = Enkripcija.Hash(user.Password);
                        user.AccountType = (int)AccountType.Admin;

                        _context.Users.Add(user);
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Home");
                    }
                }
            }

        }

        [HttpGet]
        public IActionResult Basket(string games)
        {
            var igre = new List<GameViewModel>();
            string id = "";
            if (games == null)
            {
                return View(igre);
            }
            foreach (char c in games)
            {
                if (!c.Equals('[') && !c.Equals(',') && !c.Equals(']'))
                {
                    id += c;
                    try
                    {
                        var game = _context.Games.FirstOrDefault(x => x.Id == Int32.Parse(id));
                        var igra = igre.FirstOrDefault(x => x.Id == game.Id);
                        if (igra != null)
                        {
                            igra.Amount++;
                        }
                        else
                        {
                            igra = new GameViewModel();
                            igra.Id = game.Id;
                            igra.Name = game.Name;
                            igra.Amount = 1;
                            igra.AmountAvailable = game.Amount;
                            igra.Category = game.Category;
                            igra.ImgUrl = game.ImgUrl;
                            igra.Price = game.Price;
                            igra.Description = game.Description;
                            igre.Add(igra);
                        }

                    }
                    catch (System.Exception)
                    {
                    }
                }
                else
                {
                    id = "";
                }
            }
            foreach (var game in igre)
            {
                if (game.Amount > game.AmountAvailable)
                {
                    game.Amount = game.AmountAvailable;
                }
                game.TotalPrice = game.Price * game.Amount;
            }
            return View(igre);
        }

        [HttpGet]
        public IActionResult Order()
        {
            ViewBag.Success = false;
            return View(new UserOrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Order([Bind("Name,Surname,Adress,Email,GameIds")] UserOrderViewModel user)
        {

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
                var order = new Order();
                var igre = new List<GameViewModel>();
                string id = "";
                foreach (char c in user.GameIds)
                {
                    if (!c.Equals('[') && !c.Equals(',') && !c.Equals(']'))
                    {
                        id += c;
                        try
                        {
                            var game = _context.Games.FirstOrDefault(x => x.Id == Int32.Parse(id));
                            var igra = igre.FirstOrDefault(x => x.Id == game.Id);
                            game.Amount--;
                            _context.Games.Update(game);
                            _context.SaveChanges();
                            if (igra != null)
                            {
                                igra.Amount++;
                            }
                            else
                            {
                                igra = new GameViewModel();
                                igra.Id = game.Id;
                                igra.Name = game.Name;
                                igra.Amount = 1;
                                igra.AmountAvailable = game.Amount;
                                igra.Category = game.Category;
                                igra.ImgUrl = game.ImgUrl;
                                igra.Price = game.Price;
                                igra.Description = game.Description;
                                igre.Add(igra);
                            }

                        }
                        catch (System.Exception)
                        {
                        }
                    }
                    else
                    {
                        id = "";
                    }
                }
                Order lastOrder = _context.Orders.LastOrDefault();
                int orderId = lastOrder != null ? lastOrder.Id + 1 : 1;
                order.Id = orderId;
                order.Name = user.Name;
                order.Surname = user.Surname;
                order.Email = user.Email;
                order.Adress = user.Adress;
                order.Date = DateTime.Now;
                
                _context.Orders.Add(order);

                foreach(var game in igre) {
                    var gameOnOrder = new GameOnOrder();
                    GameOnOrder lastGameOnOrder = _context.GamesOnOrder.LastOrDefault();
                    int gameOnOrderId = lastGameOnOrder != null ? lastGameOnOrder.Id + 1 : 1;
                    gameOnOrder.Id =  gameOnOrderId;
                    gameOnOrder.GameId = game.Id;
                    gameOnOrder.OrderId = order.Id;
                    gameOnOrder.Amount = game.Amount;
                    _context.GamesOnOrder.Add(gameOnOrder);
                    _context.SaveChanges();
                }

                ViewBag.Success = true;
                return View(user);
            }


        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Games", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
