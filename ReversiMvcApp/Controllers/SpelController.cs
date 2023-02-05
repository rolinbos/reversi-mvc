using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using ReversiMvcApp.Responses;
using ReversiMvcApp.Services;

namespace ReversiMvcApp
{
    [Authorize]
    public class SpelController : Controller
    {
        private readonly ApiServices _apiServices;
        private readonly ReversiDbContext _context;

        public SpelController(ReversiDbContext context)
        {
            _context = context;
            _apiServices = new ApiServices();
        }
        // GET: Spel
        public ActionResult Index()
        {
            dynamic myModel = new ExpandoObject();
            myModel.alleOpenstaandeSpellen = _apiServices.KrijgAlleOPenSpellen(); 
            myModel.spellenVanGebruiker = _apiServices.KrijgAlleSpellenVanGebruiker(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            return View(myModel);
        }

        // GET: Spel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Spel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToActionResult> Create([Bind("omschrijving")] Spel spel)
        {
            Spel nieuwSpel = _apiServices.NieuwSpel(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, spel.omschrijving);

            return RedirectToAction(nameof(Play), new {spelToken = nieuwSpel.token});
        }

        public IActionResult Done(string token, string spelerToken)
        {
            string message = "Nog geen uitslag.";
            if (token == null)
            {
                return NotFound();
            }
            
            string response = _apiServices.Done(token, spelerToken);
            Speler speler =  _context.Spelers.First(speler => speler.Guuid == spelerToken);
            
            if (response == "gelijk-spel")
            {
                speler.AantalGelijk += 1;
                message = "Het is gelijkspel geworden";
            } else if (response == "gewonnen")
            {
                speler.AantalGewonnen += 1;
                message = "Je hebt gewonnen";
            } else if (response == "verloren")
            {
                speler.AantalVerloren += 1;
                message = "Je hebt helaas verloren";
            }
            else
            {
                return NotFound();
            }
            
            _context.SaveChanges();
            
            return View("Done", message);
        }

        public ActionResult Join(string token)
        {
            Spel spel = _apiServices.JoinSpel(token, this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            if (spel == null)
            {
                return NotFound();
            }
            
            return RedirectToAction(nameof(Play), new {spelToken = spel.token});
        }
        
        public IActionResult Play(string spelToken)
        {
            if (spelToken == null)
            {
                return NotFound();
            }

            Spel spel = _apiServices.KrijgSpel(spelToken);
            
            if (spel == null) {
                return NotFound();
            }
            
            return View(spel);
        }
    }
}