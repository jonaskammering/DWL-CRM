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
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ansprechpeople.Include(a => a.Person);
            return View(await appDbContext.ToListAsync());
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
