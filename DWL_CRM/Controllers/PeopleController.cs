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
    public class PeopleController : Controller
    {
        private readonly AppDbContext _context;

        public PeopleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: People
        public async Task<IActionResult> Index(string? q, int? firmaId, string? sort)
        {
            var query = _context.People.Include(p => p.Firma).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var search = q.Trim().ToLower();
                query = query.Where(p =>
                    (p.Vorname != null && p.Vorname.ToLower().Contains(search)) ||
                    (p.Nachname != null && p.Nachname.ToLower().Contains(search)) ||
                    (p.Email != null && p.Email.ToLower().Contains(search)) ||
                    (p.Telefon != null && p.Telefon.ToLower().Contains(search)));
            }

            if (firmaId.HasValue)
            {
                query = query.Where(p => p.FirmaId == firmaId.Value);
            }

            query = sort switch
            {
                "name_desc" => query.OrderByDescending(p => p.Nachname).ThenByDescending(p => p.Vorname),
                "firma_asc" => query.OrderBy(p => p.Firma.Firmenname).ThenBy(p => p.Nachname),
                "firma_desc" => query.OrderByDescending(p => p.Firma.Firmenname).ThenBy(p => p.Nachname),
                _ => query.OrderBy(p => p.Nachname).ThenBy(p => p.Vorname)
            };

            ViewData["CurrentQuery"] = q;
            ViewData["CurrentFirmaId"] = firmaId;
            ViewData["CurrentSort"] = sort;
            ViewData["SortOptions"] = new List<SelectListItem>
            {
                new() { Value = "", Text = "Sortierung: Name A-Z", Selected = string.IsNullOrEmpty(sort) },
                new() { Value = "name_desc", Text = "Name Z-A", Selected = sort == "name_desc" },
                new() { Value = "firma_asc", Text = "Firma A-Z", Selected = sort == "firma_asc" },
                new() { Value = "firma_desc", Text = "Firma Z-A", Selected = sort == "firma_desc" }
            };
            ViewData["FirmaFilter"] = new SelectList(
                await _context.Firmas.OrderBy(f => f.Firmenname).ToListAsync(),
                "FirmaId",
                "Firmenname",
                firmaId
            );

            return View(await query.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .Include(p => p.Firma)
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FirmaId,Titel,Vorname,Nachname,Geburtsdatum,Telefon,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId", person.FirmaId);
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId", person.FirmaId);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,FirmaId,Titel,Vorname,Nachname,Geburtsdatum,Telefon,Email")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
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
            ViewData["FirmaId"] = new SelectList(_context.Firmas, "FirmaId", "FirmaId", person.FirmaId);
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .Include(p => p.Firma)
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}
