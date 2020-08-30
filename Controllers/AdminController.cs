using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using PAUP.Models;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using IronPdf;

namespace PAUP.Controllers
{

    public class AdminController : Controller
    {
        private IgreDbContext _context = new IgreDbContext();

        private readonly IHostingEnvironment he;


        public AdminController(IHostingEnvironment e)
        {
            he = e;
        }
        public IActionResult Games()
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {
                var igre = _context.Games.ToList();
                return View(igre);
            }
        }

        public IActionResult Users()
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {
                var user = HttpContext.Session.GetObjectFromJson<User>("user");
                var korisnici = _context.Users.ToList();
                var users = new List<User>();
                users.AddRange(korisnici);
                foreach (var korisnik in korisnici)
                {
                    if (korisnik.Email == user.Email)
                    {
                        users.Remove(korisnik);
                    }
                    else if (korisnik.Email == "admin@admin.admin")
                    {
                        users.Remove(korisnik);
                    }
                }
                return View(users);
            }
        }

        public IActionResult Offers()
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {
                var ponude = _context.Offers.ToList();
                var offers = new List<OfferViewModel>();
                foreach (var item in ponude)
                {
                    OfferViewModel ponuda = new OfferViewModel();
                    ponuda.Date = item.Date;
                    ponuda.Email = item.Email;
                    ponuda.OrderId = item.OrderId;
                    ponuda.Price = item.Price;
                    offers.Add(ponuda);
                }
                return View(offers);
            }
        }

        public IActionResult Orders()
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {
                List<OrderViewModel> nar = new List<OrderViewModel>();
                var narudzbe = _context.Orders.ToList();

                foreach (var item in narudzbe)
                {
                    var igre = _context.GamesOnOrder.Where(x => x.OrderId == item.Id).ToList();
                    var narudzba = new OrderViewModel();
                    narudzba.Id = item.Id;
                    narudzba.Date = item.Date;
                    narudzba.Name = item.Name;
                    narudzba.Surname = item.Surname;
                    narudzba.Adress = item.Adress;
                    narudzba.Email = item.Email;
                    narudzba.HasOffer = item.HasOffer;
                    narudzba.Price = 0;
                    foreach (var game in igre)
                    {
                        var igra = _context.Games.FirstOrDefault(x => x.Id == game.GameId);
                        narudzba.Price += igra.Price * game.Amount;
                    }
                    nar.Add(narudzba);
                }
                return View(nar);
            }
        }

        [HttpPost]
        public JsonResult DeleteUser([FromBody] User _user)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return Json(null);
            }
            else
            {
                User user = _context.Users.FirstOrDefault(x => x.Email == _user.Email);
                try
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return Json(true);
                }
                catch
                {

                    return Json(false);
                }

            }
        }

        [HttpPost]
        public JsonResult DeleteOffer([FromBody] Offer _offer)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return Json(null);
            }
            else
            {
                Offer offer = _context.Offers.FirstOrDefault(x => x.OrderId == _offer.OrderId);
                try
                {
                    _context.Offers.Remove(offer);
                    var order = _context.Orders.FirstOrDefault(x => x.Id == offer.OrderId);
                    order.HasOffer = false;
                    _context.Orders.Update(order);
                    string fileName = he.WebRootPath + "/ponude/" + _offer.OrderId + ".pdf";
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }

                    _context.SaveChanges();
                    return Json(true);
                }
                catch
                {

                    return Json(false);
                }

            }
        }

        [HttpPost]
        public JsonResult DeleteOrder([FromBody] Order _order)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return Json(null);
            }
            else
            {
                Order order = _context.Orders.FirstOrDefault(x => x.Id == _order.Id);
                try
                {
                    var games = _context.GamesOnOrder.Where(x => x.OrderId == order.Id);
                    foreach (var igra in games)
                    {
                        var game = _context.Games.FirstOrDefault(x => x.Id == igra.GameId);
                        game.Amount += igra.Amount;
                        _context.SaveChanges();
                    }
                    _context.GamesOnOrder.RemoveRange(games);
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                    return Json(true);
                }
                catch
                {

                    return Json(false);
                }

            }
        }

        [HttpPost]
        public JsonResult DeleteGame([FromBody] Game _game)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return Json(null);
            }
            else
            {
                Game game = _context.Games.FirstOrDefault(x => x.Name == _game.Name);
                try
                {
                    _context.Games.Remove(game);
                    _context.SaveChanges();
                    return Json(true);
                }
                catch
                {

                    return Json(false);
                }

            }
        }

        [HttpGet]
        public IActionResult EditGame(string id)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {

                Game game = _context.Games.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                return View(game);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGame([Bind("Id,Name,Description,Category,ImgUrl,Price,Amount")] Game game, IFormFile Img)
        {
            if (!ModelState.IsValid)
            {
                return View(game);
            }
            else
            {
                if (Img != null)
                {

                    try
                    {
                        string fileName = he.WebRootPath + "/images/Games/" + Path.GetFileName(Img.FileName);
                        using (var fs = new FileStream(fileName, FileMode.Create))
                        {
                            Img.CopyTo(fs);
                        }
                        game.ImgUrl = "/images/Games/" + Path.GetFileName(Img.FileName);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    game.ImgUrl = "/images/Games/default.png";
                }
                _context.Games.Update(game);
                _context.SaveChanges();
                return RedirectToAction("Games", "Admin");

            }
        }


        //PROVJERA!!
        [HttpGet]
        public IActionResult CreateOffer(string id)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {

                var offer = new OfferViewModel();
                var order = _context.Orders.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
                var games = _context.GamesOnOrder.Where(x => x.OrderId == order.Id);

                offer.OrderId = order.Id;
                offer.Name = order.Name;
                offer.Surname = order.Surname;
                offer.Email = order.Email;
                offer.OrderId = order.Id;
                offer.Adress = order.Adress;
                offer.Games = new List<GameViewModel>();
                offer.Price = 0;
                foreach (var item in games)
                {
                    var game = _context.Games.FirstOrDefault(x => x.Id == item.GameId);
                    var gameVM = new GameViewModel();
                    gameVM.Id = game.Id;
                    gameVM.Name = game.Name;
                    gameVM.Price = game.Price;
                    gameVM.Category = game.Category;
                    gameVM.Amount = item.Amount;
                    offer.Price += item.Amount * game.Price;
                    offer.Games.Add(gameVM);
                }
                return View(offer);


            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOffer([Bind("Name,Surname,Adress,Email,Rabat,Price,Games,OrderId,Guarantee")] OfferViewModel _offer)
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {
                try
                {
                    var order = _context.Orders.FirstOrDefault(x => x.Id == _offer.OrderId);
                    var games = _context.GamesOnOrder.Where(x => x.OrderId == order.Id);
                    var offer = new Offer();
                    Offer lastOffer = _context.Offers.LastOrDefault();
                    offer.Id = lastOffer != null ? lastOffer.Id + 1 : 1;
                    offer.Name = _offer.Name;
                    offer.Surname = _offer.Surname;
                    offer.OrderId = order.Id;
                    offer.Adress = _offer.Adress;
                    offer.Email = _offer.Email;
                    offer.Date = DateTime.Now;
                    offer.Guarantee = _offer.Guarantee;
                    offer.UserId = HttpContext.Session.GetObjectFromJson<User>("user").Id;
                    _offer.Games = new List<GameViewModel>();
                    offer.Price = 0;
                    foreach (var item in games)
                    {
                        var game = _context.Games.FirstOrDefault(x => x.Id == item.GameId);
                        var gameVM = new GameViewModel();
                        gameVM.Id = game.Id;
                        gameVM.Name = game.Name;
                        gameVM.Price = game.Price;
                        gameVM.Category = game.Category;
                        gameVM.Amount = item.Amount;
                        offer.Price += item.Amount * game.Price;
                        _offer.Price += item.Amount * game.Price;
                        _offer.Games.Add(gameVM);
                    }
                    offer.Price = offer.Price - (offer.Price * (_offer.Rabat / 100));
                    _offer.Price = offer.Price;
                    SendOffer(_offer);
                    _context.Offers.Add(offer);
                    order.HasOffer = true;
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                    return Redirect("/Admin/generatePDF/" + order.Id);


                }
                catch (System.Exception)
                {
                    return View(_offer);
                }

            }
        }

        [HttpGet]
        public IActionResult generatePDF(string id)
        {
            var offerV = new OfferViewModel();
            var order = _context.Orders.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
            var games = _context.GamesOnOrder.Where(x => x.OrderId == order.Id);
            var offer = _context.Offers.FirstOrDefault(x => x.OrderId == order.Id);
            offerV.Games = new List<GameViewModel>();
            foreach (var item in games)
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == item.GameId);
                var gameVM = new GameViewModel();
                gameVM.Id = game.Id;
                gameVM.Name = game.Name;
                gameVM.Price = game.Price;
                gameVM.Category = game.Category;
                gameVM.Amount = item.Amount;
                offerV.Games.Add(gameVM);
            }
            offerV.Price = offer.Price;
            offerV.Name = offer.Name;
            offerV.Surname = offer.Surname;
            offerV.Email = offer.Email;
            offerV.Guarantee = offer.Guarantee;
            offerV.Adress = offer.Adress;
            offerV.Date = offer.Date;
            offerV.OrderId = offer.OrderId;
            HtmlToPdf renderer = new HtmlToPdf();
            var file = renderer.RenderHtmlAsPdf(HtmlOffer(offerV));
            return File(file.BinaryData, "application/pdf", "Ponuda_br." + offer.OrderId + ".pdf");
        }
        [NonAction]
        public void SendOffer(OfferViewModel offer)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Question", "bangpeklenica@gmail.com"));
            message.To.Add(new MailboxAddress(offer.Email));
            message.Subject = "Ponuda";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = HtmlOffer(offer);

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("bangpeklenica@gmail.com", "258156peklenica");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        [NonAction]
        public string HtmlOffer(OfferViewModel offer)
        {
            string html = "";
            string header = "<html><head><meta charset='utf-8'/><link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css'/></head><body>";
            string footer = "</body></html>";
            string body = "";
            body += "<div class='container body-content'>";
            body += "<div class='row'><div class='col-md-12 text-center'><h2>Ponuda za narudžbu br." + offer.OrderId + "</h2><br></div></div><div class='row'><div class='col-md-6 text-center'><table class='table table-striped'><thead><tr><th scope='col'>Naziv</th><th scope='col'>Cijena</th><th scope='col'>Kategorija</th><th scope='col'>Komada</th></tr></thead><tbody>";
            foreach (var game in offer.Games)
            {
                body += "<tr><th scope='row'>" + game.Name + "</th><td>" + game.Price + "</td><td>";
                switch (game.Category)
                {
                    case 0:
                        body += "<text>Avanturistička</text>";
                        break;
                    case 1:
                        body += "<text>Akcijska</text>";
                        break;
                    case 2:
                        body += "<text>Sport</text>";
                        break;
                    case 3:
                        body += "<text>Ostalo</text>";
                        break;
                }

                body += "</td><td>" + game.Amount + "</td></tr>";
            }
            body += "</tbody></table></div><div class='col-md-6 text-center'><label class='sr-only-focusable'>Ime</label><input type='text' value='" + offer.Name + "' name='Name' class='form-control' readonly><label  class='sr-only-focusable'>Prezime</label>";
            body += "<input type='text' value='" + offer.Surname + "' class='form-control' readonly><label class='sr-only-focusable'>Adresa</label><input type='text'  value='" + offer.Adress + "' class='form-control' readonly><label class='sr-only-focusable'>Email</label>";
            body += "<input type='email' value='" + offer.Email + "' class='form-control' readonly><label  class='sr-only-focusable'>Garancija</label>";
            body += "<input class='form-control' type='text' value='" + offer.Guarantee + "' readonly><label>Popust (%):</label>";
            body += "<input class='form-control' type='number' value='" + offer.Rabat + "' readonly></div></div><div class='row'><h2>Ukupno za platiti: <span>" + offer.Price + "</span></h2></div>";
            html += header + body + footer;
            return html;
        }

        //END PROVJERA!
        [HttpGet]
        public IActionResult AddGame()
        {
            if (HttpContext.Session.GetObjectFromJson<User>("user") == null || HttpContext.Session.GetObjectFromJson<User>("user").AccountType != 1)
            {
                return RedirectToAction("Games", "Home");
            }
            else
            {

                return View(new Game());
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGame([Bind("Id,Name,Description,Category,ImgUrl,Price,Amount")] Game game, IFormFile Img)
        {
            if (!ModelState.IsValid)
            {
                return View(game);
            }
            else
            {
                if (Img != null)
                {

                    try
                    {
                        string fileName = he.WebRootPath + "/images/Games/" + Path.GetFileName(Img.FileName);
                        using (var fs = new FileStream(fileName, FileMode.Create))
                        {
                            Img.CopyTo(fs);
                        }
                        game.ImgUrl = "/images/Games/" + Path.GetFileName(Img.FileName);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    game.ImgUrl = "/images/Games/default.png";
                }


                int id = _context.Games.LastOrDefault().Id;
                game.Id = id + 1;
                _context.Games.Add(game);
                _context.SaveChanges();
                return RedirectToAction("Games", "Admin");

            }
        }

    }
}