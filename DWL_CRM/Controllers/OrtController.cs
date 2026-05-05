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
    public class OrtController : Controller
    {
        private readonly AppDbContext _context;

        public OrtController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ort
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orts.ToListAsync());
        }

        // GET: Ort/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ort = await _context.Orts
                .FirstOrDefaultAsync(m => m.OrtId == id);
            if (ort == null)
            {
                return NotFound();
            }

            return View(ort);
        }

        // GET: Ort/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ort/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrtId,Plz,Ortsname")] Ort ort)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ort);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ort);
        }

        // GET: Ort/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ort = await _context.Orts.FindAsync(id);
            if (ort == null)
            {
                return NotFound();
            }
            return View(ort);
        }

        // POST: Ort/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrtId,Plz,Ortsname")] Ort ort)
        {
            if (id != ort.OrtId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ort);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrtExists(ort.OrtId))
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
            return View(ort);
        }

        // GET: Ort/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ort = await _context.Orts
                .FirstOrDefaultAsync(m => m.OrtId == id);
            if (ort == null)
            {
                return NotFound();
            }

            return View(ort);
        }

        // POST: Ort/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ort = await _context.Orts.FindAsync(id);
            if (ort != null)
            {
                _context.Orts.Remove(ort);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrtExists(int id)
        {
            return _context.Orts.Any(e => e.OrtId == id);
        }
    }
}
