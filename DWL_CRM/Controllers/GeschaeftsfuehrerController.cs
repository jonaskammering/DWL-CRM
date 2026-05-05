using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DWL_CRM.Models;

namespace DWL_CRM.Controllers
{
    public class GeschaeftsfuehrerController : Controller
    {
        private readonly AppDbContext _context;

        public GeschaeftsfuehrerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Geschaeftsfuehrers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Geschaeftsfuehrers.Include(g => g.Person);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Geschaeftsfuehrers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var geschaeftsfuehrer = await _context.Geschaeftsfuehrers
                .Include(g => g.Person)
                .FirstOrDefaultAsync(m => m.GeschaeftsfuehrerId == id);
            if (geschaeftsfuehrer == null)
            {
                return NotFound();
            }

            return View(geschaeftsfuehrer);
        }

        // GET: Geschaeftsfuehrers/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId");
            return View();
        }

        // POST: Geschaeftsfuehrers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GeschaeftsfuehrerId,PersonId")] Geschaeftsfuehrer geschaeftsfuehrer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(geschaeftsfuehrer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", geschaeftsfuehrer.PersonId);
            return View(geschaeftsfuehrer);
        }

        // GET: Geschaeftsfuehrers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var geschaeftsfuehrer = await _context.Geschaeftsfuehrers.FindAsync(id);
            if (geschaeftsfuehrer == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", geschaeftsfuehrer.PersonId);
            return View(geschaeftsfuehrer);
        }

        // POST: Geschaeftsfuehrers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GeschaeftsfuehrerId,PersonId")] Geschaeftsfuehrer geschaeftsfuehrer)
        {
            if (id != geschaeftsfuehrer.GeschaeftsfuehrerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(geschaeftsfuehrer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeschaeftsfuehrerExists(geschaeftsfuehrer.GeschaeftsfuehrerId))
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
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", geschaeftsfuehrer.PersonId);
            return View(geschaeftsfuehrer);
        }

        // GET: Geschaeftsfuehrers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var geschaeftsfuehrer = await _context.Geschaeftsfuehrers
                .Include(g => g.Person)
                .FirstOrDefaultAsync(m => m.GeschaeftsfuehrerId == id);
            if (geschaeftsfuehrer == null)
            {
                return NotFound();
            }

            return View(geschaeftsfuehrer);
        }

        // POST: Geschaeftsfuehrers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var geschaeftsfuehrer = await _context.Geschaeftsfuehrers.FindAsync(id);
            if (geschaeftsfuehrer != null)
            {
                _context.Geschaeftsfuehrers.Remove(geschaeftsfuehrer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeschaeftsfuehrerExists(int id)
        {
            return _context.Geschaeftsfuehrers.Any(e => e.GeschaeftsfuehrerId == id);
        }
    }
}
