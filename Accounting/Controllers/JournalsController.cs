using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL_ACCOUNTING.Data;
using DAL_ACCOUNTING.Models;

namespace Accounting.Controllers
{
    public class JournalsController : Controller
    {
        private readonly DatabaseContext _context;

        public JournalsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Journals
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Journals.Include(j => j.Account);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Journals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .Include(j => j.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }

        // GET: Journals/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts, "Id", "AccountNum");
            return View();
        }

        // POST: Journals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,JrnlType,TransNum,Dc,Posted,TransDate,PostDate,Amount")] Journal journal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(journal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts, "Id", "AccountNum", journal.AccountId);
            return View(journal);
        }

        // GET: Journals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals.FindAsync(id);
            if (journal == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts, "Id", "AccountNum", journal.AccountId);
            return View(journal);
        }

        // POST: Journals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,JrnlType,TransNum,Dc,Posted,TransDate,PostDate,Amount")] Journal journal)
        {
            if (id != journal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalExists(journal.Id))
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
            ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts, "Id", "AccountNum", journal.AccountId);
            return View(journal);
        }

        // GET: Journals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Journals == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .Include(j => j.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }

        // POST: Journals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Journals == null)
            {
                return Problem("Entity set 'DatabaseContext.Journals'  is null.");
            }
            var journal = await _context.Journals.FindAsync(id);
            if (journal != null)
            {
                _context.Journals.Remove(journal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalExists(int id)
        {
          return (_context.Journals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
