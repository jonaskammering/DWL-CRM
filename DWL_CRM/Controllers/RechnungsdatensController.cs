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
    public class RechnungsdatensController : Controller
    {
        private readonly AppDbContext _context;

        public RechnungsdatensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Rechnungsdatens
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Rechnungsdatens.Include(r => r.Firma);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Rechnungsdatens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rechnungsdaten = await _context.Rechnungsdatens
                .Include(r => r.Firma)
                .FirstOrDefaultAsync(m => m.RechnungsdatenId == id);
            if (rechnungsdaten == null)
            {
                return NotFound();
            }

            return View(rechnungsdaten);
        }

        // GET: Rechnungsdatens/Create
        public IActionResult Create()
        {
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId");
            return View();
        }

        // POST: Rechnungsdatens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RechnungsdatenId,FirmaId,RechnungenGesamt,RechnungenOffen,LetzterZahlungseingang")] Rechnungsdaten rechnungsdaten)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rechnungsdaten);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId", rechnungsdaten.FirmaId);
            return View(rechnungsdaten);
        }

        // GET: Rechnungsdatens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rechnungsdaten = await _context.Rechnungsdatens.FindAsync(id);
            if (rechnungsdaten == null)
            {
                return NotFound();
            }
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId", rechnungsdaten.FirmaId);
            return View(rechnungsdaten);
        }

        // POST: Rechnungsdatens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RechnungsdatenId,FirmaId,RechnungenGesamt,RechnungenOffen,LetzterZahlungseingang")] Rechnungsdaten rechnungsdaten)
        {
            if (id != rechnungsdaten.RechnungsdatenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rechnungsdaten);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RechnungsdatenExists(rechnungsdaten.RechnungsdatenId))
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
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId", rechnungsdaten.FirmaId);
            return View(rechnungsdaten);
        }

        // GET: Rechnungsdatens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rechnungsdaten = await _context.Rechnungsdatens
                .Include(r => r.Firma)
                .FirstOrDefaultAsync(m => m.RechnungsdatenId == id);
            if (rechnungsdaten == null)
            {
                return NotFound();
            }

            return View(rechnungsdaten);
        }

        // POST: Rechnungsdatens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rechnungsdaten = await _context.Rechnungsdatens.FindAsync(id);
            if (rechnungsdaten != null)
            {
                _context.Rechnungsdatens.Remove(rechnungsdaten);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RechnungsdatenExists(int id)
        {
            return _context.Rechnungsdatens.Any(e => e.RechnungsdatenId == id);
        }
    }
}
