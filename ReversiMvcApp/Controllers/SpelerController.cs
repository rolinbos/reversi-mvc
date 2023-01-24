using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;

namespace ReversiMvcApp
{
    public class SpelerController : Controller
    {
        private readonly ReversiDbContext _context;

        public SpelerController(ReversiDbContext context)
        {
            _context = context;
        }

        // GET: Speler
        public async Task<IActionResult> Index()
        {
            return _context.Spelers != null ? 
                          View(await _context.Spelers.ToListAsync()) :
                          Problem("Entity set 'ReversiDbContext.Spelers'  is null.");
        }

        // GET: Speler/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Speler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Speler/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Spelers == null)
            {
                return Problem("Entity set 'ReversiDbContext.Spelers'  is null.");
            }
            var speler = await _context.Spelers.FindAsync(id);
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
