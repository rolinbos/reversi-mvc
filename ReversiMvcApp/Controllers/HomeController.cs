using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using ReversiMvcApp.Services;

namespace ReversiMvcApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ReversiDbContext _context;
    private readonly ApiServices _apiServices;
    
    public HomeController(ILogger<HomeController> logger, ReversiDbContext context)
    {
        _logger = logger;
        _context = context;
        _apiServices = new ApiServices();
    }

    public IActionResult Index()
    {
        ClaimsPrincipal currentUser = this.User;
        if (currentUser.Identity.IsAuthenticated)
        {
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var spelers = _context.Spelers.Where(predicate: s => s.Guuid.ToLower() == currentUserId.ToLower());

            if (spelers.Count() == 0)
            {
                Speler speler = new Speler()
                {
                    Naam = currentUser.Identity.Name,
                    Guuid = new Guid(currentUserId).ToString(),
                    AantalGelijk = 0,
                    AantalGewonnen = 0,
                    AantalVerloren = 0,
                };

                _context.Spelers.Add(speler);
                _context.SaveChanges();
            }
            
            // Krijg alle spellen van de user
            List<Spel> spellen = _apiServices.KrijgAlleSpellenVanGebruiker(spelers.First().Guuid);

            if (spellen.Count() > 0)
            {
                return RedirectToAction("Play", "Spel", new {spelToken = spellen.First().token});
            }
            
            return RedirectToAction("Index", "Spel");
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}