using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DWL_CRM.Models;
using DWL_CRM.ViewModels;

namespace DWL_CRM.Controllers
{
    public class FirmaController : Controller
    {
        private readonly AppDbContext _context;

        public FirmaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Firmas
        public async Task<IActionResult> Index(string? q, string? abc, string? sort)
        {
            var firmenQuery = _context.Firmas
                .Include(f => f.Ort)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var search = q.Trim().ToLower();
                firmenQuery = firmenQuery.Where(f =>
                    f.Firmenname.ToLower().Contains(search) ||
                    (f.Branche != null && f.Branche.ToLower().Contains(search)) ||
                    (f.Ort.Ortsname != null && f.Ort.Ortsname.ToLower().Contains(search)));
            }

            var firmen = await firmenQuery.ToListAsync();

            var abcRanking = firmen
                .OrderByDescending(f => f.Jahresumsatz ?? 0m)
                .ThenBy(f => f.Firmenname)
                .ToList();

            var totalUmsatz = abcRanking.Sum(f => f.Jahresumsatz ?? 0m);
            decimal kumulierterUmsatz = 0m;

            var model = new List<FirmaIndexItemViewModel>(abcRanking.Count);
            foreach (var firma in abcRanking)
            {
                var umsatz = firma.Jahresumsatz ?? 0m;
                kumulierterUmsatz += umsatz;

                var kategorie = "C";
                if (totalUmsatz > 0)
                {
                    var anteil = kumulierterUmsatz / totalUmsatz;
                    if (anteil <= 0.80m)
                    {
                        kategorie = "A";
                    }
                    else if (anteil <= 0.95m)
                    {
                        kategorie = "B";
                    }
                }

                model.Add(new FirmaIndexItemViewModel
                {
                    Firma = firma,
                    AbcKategorie = kategorie
                });
            }

            if (!string.IsNullOrWhiteSpace(abc) && abc is "A" or "B" or "C")
            {
                model = model.Where(x => x.AbcKategorie == abc).ToList();
            }

            model = sort switch
            {
                "name_desc" => model.OrderByDescending(x => x.Firma.Firmenname).ToList(),
                "umsatz_asc" => model.OrderBy(x => x.Firma.Jahresumsatz ?? 0m).ToList(),
                "umsatz_desc" => model.OrderByDescending(x => x.Firma.Jahresumsatz ?? 0m).ToList(),
                "abc" => model.OrderBy(x => x.AbcKategorie).ThenByDescending(x => x.Firma.Jahresumsatz ?? 0m).ToList(),
                _ => model.OrderBy(x => x.Firma.Firmenname).ToList()
            };

            ViewData["CurrentQuery"] = q;
            ViewData["CurrentAbc"] = abc;
            ViewData["CurrentSort"] = sort;
            ViewData["AbcOptions"] = new List<SelectListItem>
            {
                new() { Value = "", Text = "ABC: Alle", Selected = string.IsNullOrEmpty(abc) },
                new() { Value = "A", Text = "A", Selected = abc == "A" },
                new() { Value = "B", Text = "B", Selected = abc == "B" },
                new() { Value = "C", Text = "C", Selected = abc == "C" }
            };
            ViewData["SortOptions"] = new List<SelectListItem>
            {
                new() { Value = "", Text = "Sortierung: Name A-Z", Selected = string.IsNullOrEmpty(sort) },
                new() { Value = "name_desc", Text = "Name Z-A", Selected = sort == "name_desc" },
                new() { Value = "umsatz_desc", Text = "Umsatz hoch-niedrig", Selected = sort == "umsatz_desc" },
                new() { Value = "umsatz_asc", Text = "Umsatz niedrig-hoch", Selected = sort == "umsatz_asc" },
                new() { Value = "abc", Text = "ABC-Kategorie", Selected = sort == "abc" }
            };

            return View(model);
        }

        // GET: Firmas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firma = await _context.Firmas
                .Include(f => f.Ort)
                .Include(f => f.Rechnungsdaten)
                .Include(f => f.People)
                    .ThenInclude(p => p.Ansprechpeople)
                .Include(f => f.People)
                    .ThenInclude(p => p.Geschaeftsfuehrers)
                .FirstOrDefaultAsync(m => m.FirmaId == id);
            if (firma == null)
            {
                return NotFound();
            }

            return View(firma);
        }

        // GET: Firmas/Create
        public IActionResult Create()
        {
            ViewData["OrtId"] = new SelectList(_context.Orts, "OrtId", "OrtId");
            return View();
        }

        // POST: Firmas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirmaId,Firmenname,Strasse,OrtId,Branche,Gruendungsdatum,Jahresumsatz,Bemerkungen")] Firma firma)
        {
            if (ModelState.IsValid)
            {
                _context.Add(firma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrtId"] = new SelectList(_context.Orts, "OrtId", "OrtId", firma.OrtId);
            return View(firma);
        }

        // GET: Firmas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firma = await _context.Firmas.FindAsync(id);
            if (firma == null)
            {
                return NotFound();
            }
            ViewData["OrtId"] = new SelectList(_context.Orts, "OrtId", "OrtId", firma.OrtId);
            return View(firma);
        }

        // POST: Firmas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirmaId,Firmenname,Strasse,OrtId,Branche,Gruendungsdatum,Jahresumsatz,Bemerkungen")] Firma firma)
        {
            if (id != firma.FirmaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(firma);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FirmaExists(firma.FirmaId))
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
            ViewData["OrtId"] = new SelectList(_context.Orts, "OrtId", "OrtId", firma.OrtId);
            return View(firma);
        }

        // GET: Firmas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firma = await _context.Firmas
                .Include(f => f.Ort)
                .FirstOrDefaultAsync(m => m.FirmaId == id);
            if (firma == null)
            {
                return NotFound();
            }

            return View(firma);
        }

        // POST: Firmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var firma = await _context.Firmas.FindAsync(id);
            if (firma != null)
            {
                _context.Firmas.Remove(firma);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FirmaExists(int id)
        {
            return _context.Firmas.Any(e => e.FirmaId == id);
        }
    }
}
