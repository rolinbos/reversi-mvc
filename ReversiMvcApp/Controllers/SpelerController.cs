using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using ReversiMvcApp.Services;

namespace ReversiMvcApp
{
    public class SpelerController : Controller
    {
        private readonly ApiServices _apiServices;
        private readonly ReversiDbContext _context;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        
        public SpelerController(ReversiDbContext context, ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _applicationDbContext = applicationDbContext;
            _apiServices = new ApiServices();
            _userManager = userManager;
        }

        // GET: Speler
        [Authorize(Roles = "Beheerder,Mediator")]
        public async Task<IActionResult> Index()
        {
            return _context.Spelers != null ? 
                          View(await _context.Spelers.ToListAsync()) :
                          Problem("Entity set 'ReversiDbContext.Spelers'  is null.");
        }

        // GET: Speler/Details/5
        [Authorize(Roles = "Beheerder,Mediator")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Spelers == null)
            {
                return NotFound();
            }

            var speler = await _context.Spelers
                .FirstOrDefaultAsync(m => m.Guuid == id);
            if (speler == null)
            {
                return NotFound();
            }

            return View(speler);
        }

        // GET: Speler/Create
        [Authorize(Roles = "Beheerder,Mediator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Speler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mediator")]
        public async Task<IActionResult> Create([Bind("Guuid,Naam,AantalGewonnen,AantalVerloren,AantalGelijk")] Speler speler)
        {
            if (ModelState.IsValid)
            {
                _context.Add(speler);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(speler);
        }

        // GET: Speler/Edit/5
        [Authorize(Roles = "Beheerder,Mediator")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Spelers == null)
            {
                return NotFound();
            }

            var speler = await _context.Spelers.FindAsync(id);
            if (speler == null)
            {
                return NotFound();
            }
            return View(speler);
        }

        // POST: Speler/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Beheerder,Mediator")]
        public async Task<IActionResult> Edit(string id, [Bind("Guuid,Naam,AantalGewonnen,AantalVerloren,AantalGelijk")] Speler speler)
        {
            if (id != speler.Guuid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(speler);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpelerExists(speler.Guuid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(speler);
        }
        
        // GET: Speler/Role/5
        [Authorize(Roles = "Beheerder")]
        public async Task<IActionResult> Role(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var dbUser = await _userManager.FindByIdAsync(id);
            if (dbUser == null)
            {
                return NotFound();
            }
            
            var user = new User()
            {
                Naam = dbUser.UserName,
                Guuid = dbUser.Id,
                Rollen = (List<string>) await _userManager.GetRolesAsync(dbUser)
            };
            
            return View(user);
        }

        // POST: Speler/Role/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Beheerder")]
        public async Task<IActionResult> Role(string id, List<String> roles)
        {
            if (id == null)
            {
                return NotFound();
            }

            Speler speler = _context.Spelers.First(speler => speler.Guuid == id);

            if (speler == null)
            {
                return NotFound();
            }
            
            var currentUser = await _userManager.FindByIdAsync(id);

            if (currentUser == null)
            {
                return NotFound();
            }
            
            var currentRolesFromUser = await _userManager.GetRolesAsync(currentUser);
            await _userManager.RemoveFromRolesAsync(currentUser, currentRolesFromUser.ToArray());
            
            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(currentUser, role);
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Speler/Delete/5
        [Authorize(Roles = "Beheerder,Mediator")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Spelers == null)
            {
                return NotFound();
            }

            var speler = await _context.Spelers
                .FirstOrDefaultAsync(m => m.Guuid == id);
            if (speler == null)
            {
                return NotFound();
            }

            return View(speler);
        }

        // POST: Speler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Beheerder,Mediator")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Spelers == null)
            {
                return Problem("Entity set 'ReversiDbContext.Spelers'  is null.");
            }
            
            
            var speler = await _context.Spelers.FindAsync(id);
            if (!_apiServices.VerwijderAlleSpellenVanGebruiker(speler.Guuid))
            {
                return RedirectToAction(nameof(Index));    
            }
            
            if (speler != null)
            {
                _context.Spelers.Remove(speler);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpelerExists(string id)
        {
          return (_context.Spelers?.Any(e => e.Guuid == id)).GetValueOrDefault();
        }
    }
}
