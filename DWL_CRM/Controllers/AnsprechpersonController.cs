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
    public class AnsprechpersonController : Controller
    {
        private readonly AppDbContext _context;

        public AnsprechpersonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ansprechpersons
        public async Task<IActionResult> Index(string? q, string? sort)
        {
            var query = _context.Ansprechpeople
                .Include(a => a.Person)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var search = q.Trim().ToLower();
                query = query.Where(a =>
                    (a.Person.Vorname != null && a.Person.Vorname.ToLower().Contains(search)) ||
                    (a.Person.Nachname != null && a.Person.Nachname.ToLower().Contains(search)) ||
                    (a.Person.Email != null && a.Person.Email.ToLower().Contains(search)));
            }

            query = sort switch
            {
                "name_desc" => query.OrderByDescending(a => a.Person.Nachname).ThenByDescending(a => a.Person.Vorname),
                _ => query.OrderBy(a => a.Person.Nachname).ThenBy(a => a.Person.Vorname)
            };

            ViewData["CurrentQuery"] = q;
            ViewData["CurrentSort"] = sort;
            ViewData["SortOptions"] = new List<SelectListItem>
            {
                new() { Value = "", Text = "Sortierung: Name A-Z", Selected = string.IsNullOrEmpty(sort) },
                new() { Value = "name_desc", Text = "Name Z-A", Selected = sort == "name_desc" }
            };

            return View(await query.ToListAsync());
        }

        // GET: Ansprechpersons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ansprechperson = await _context.Ansprechpeople
                .Include(a => a.Person)
                .FirstOrDefaultAsync(m => m.AnsprechpersonId == id);
            if (ansprechperson == null)
            {
                return NotFound();
            }

            return View(ansprechperson);
        }

        // GET: Ansprechpersons/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId");
            return View();
        }

        // POST: Ansprechpersons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnsprechpersonId,PersonId")] Ansprechperson ansprechperson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ansprechperson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", ansprechperson.PersonId);
            return View(ansprechperson);
        }

        // GET: Ansprechpersons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ansprechperson = await _context.Ansprechpeople.FindAsync(id);
            if (ansprechperson == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", ansprechperson.PersonId);
            return View(ansprechperson);
        }

        // POST: Ansprechpersons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnsprechpersonId,PersonId")] Ansprechperson ansprechperson)
        {
            if (id != ansprechperson.AnsprechpersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ansprechperson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnsprechpersonExists(ansprechperson.AnsprechpersonId))
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
            ViewData["PersonId"] = new SelectList(_context.People, "PersonId", "PersonId", ansprechperson.PersonId);
            return View(ansprechperson);
        }

        // GET: Ansprechpersons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ansprechperson = await _context.Ansprechpeople
                .Include(a => a.Person)
                .FirstOrDefaultAsync(m => m.AnsprechpersonId == id);
            if (ansprechperson == null)
            {
                return NotFound();
            }

            return View(ansprechperson);
        }

        // POST: Ansprechpersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ansprechperson = await _context.Ansprechpeople.FindAsync(id);
            if (ansprechperson != null)
            {
                _context.Ansprechpeople.Remove(ansprechperson);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnsprechpersonExists(int id)
        {
            return _context.Ansprechpeople.Any(e => e.AnsprechpersonId == id);
        }
    }
}
