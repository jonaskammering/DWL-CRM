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
        public async Task<IActionResult> Index(string? q, string? status, string? sort)
        {
            var query = _context.Rechnungsdatens
                .Include(r => r.Firma)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var search = q.Trim().ToLower();
                query = query.Where(r => r.Firma.Firmenname.ToLower().Contains(search));
            }

            if (status == "offen")
            {
                query = query.Where(r => (r.RechnungenOffen ?? 0m) > 0m);
            }
            else if (status == "ausgeglichen")
            {
                query = query.Where(r => (r.RechnungenOffen ?? 0m) <= 0m);
            }

            query = sort switch
            {
                "offen_desc" => query.OrderByDescending(r => r.RechnungenOffen ?? 0m),
                "offen_asc" => query.OrderBy(r => r.RechnungenOffen ?? 0m),
                "gesamt_desc" => query.OrderByDescending(r => r.RechnungenGesamt ?? 0m),
                "gesamt_asc" => query.OrderBy(r => r.RechnungenGesamt ?? 0m),
                "datum_desc" => query.OrderByDescending(r => r.LetzterZahlungseingang),
                "datum_asc" => query.OrderBy(r => r.LetzterZahlungseingang),
                _ => query.OrderBy(r => r.Firma.Firmenname)
            };

            ViewData["CurrentQuery"] = q;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentSort"] = sort;
            ViewData["StatusOptions"] = new List<SelectListItem>
            {
                new() { Value = "", Text = "Status: Alle", Selected = string.IsNullOrEmpty(status) },
                new() { Value = "offen", Text = "Nur offene", Selected = status == "offen" },
                new() { Value = "ausgeglichen", Text = "Nur ausgeglichene", Selected = status == "ausgeglichen" }
            };
            ViewData["SortOptions"] = new List<SelectListItem>
            {
                new() { Value = "", Text = "Sortierung: Firma A-Z", Selected = string.IsNullOrEmpty(sort) },
                new() { Value = "offen_desc", Text = "Offen hoch-niedrig", Selected = sort == "offen_desc" },
                new() { Value = "offen_asc", Text = "Offen niedrig-hoch", Selected = sort == "offen_asc" },
                new() { Value = "gesamt_desc", Text = "Gesamt hoch-niedrig", Selected = sort == "gesamt_desc" },
                new() { Value = "gesamt_asc", Text = "Gesamt niedrig-hoch", Selected = sort == "gesamt_asc" },
                new() { Value = "datum_desc", Text = "Zahlung neu-alt", Selected = sort == "datum_desc" },
                new() { Value = "datum_asc", Text = "Zahlung alt-neu", Selected = sort == "datum_asc" }
            };

            return View(await query.ToListAsync());
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
