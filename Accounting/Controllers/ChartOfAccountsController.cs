using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL_ACCOUNTING.Data;
using DAL_ACCOUNTING.Models;

namespace Accounting.Controllers
{
    public class ChartOfAccountsController : Controller
    {
        private readonly DatabaseContext _context;

        public ChartOfAccountsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: ChartOfAccounts
        public async Task<IActionResult> Index()
        {
            return _context.ChartOfAccounts != null ?
                        View(await _context.ChartOfAccounts.ToListAsync()) :
                        Problem("Entity set 'DatabaseContext.ChartOfAccounts'  is null.");
        }

        // GET: ChartOfAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChartOfAccounts == null)
            {
                return NotFound();
            }

            var chartOfAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartOfAccount == null)
            {
                return NotFound();
            }

            return View(chartOfAccount);
        }

        // GET: ChartOfAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChartOfAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountNum,Descrip,AcctType,Balance")] ChartOfAccount chartOfAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chartOfAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chartOfAccount);
        }

        // GET: ChartOfAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChartOfAccounts == null)
            {
                return NotFound();
            }

            var chartOfAccount = await _context.ChartOfAccounts.FindAsync(id);
            if (chartOfAccount == null)
            {
                return NotFound();
            }
            return View(chartOfAccount);
        }

        // POST: ChartOfAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountNum,Descrip,AcctType,Balance")] ChartOfAccount chartOfAccount)
        {
            if (id != chartOfAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chartOfAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartOfAccountExists(chartOfAccount.Id))
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
            return View(chartOfAccount);
        }

        // GET: ChartOfAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChartOfAccounts == null)
            {
                return NotFound();
            }

            var chartOfAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartOfAccount == null)
            {
                return NotFound();
            }

            return View(chartOfAccount);
        }

        // POST: ChartOfAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChartOfAccounts == null)
            {
                return Problem("Entity set 'DatabaseContext.ChartOfAccounts'  is null.");
            }
            var chartOfAccount = await _context.ChartOfAccounts.FindAsync(id);
            if (chartOfAccount != null)
            {
                _context.ChartOfAccounts.Remove(chartOfAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartOfAccountExists(int id)
        {
            return (_context.ChartOfAccounts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
