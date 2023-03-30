using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CEPapi.Models;

namespace CEPapi.Controllers
{
    public class MainModelsController : Controller
    {
        private readonly Contexto _context;

        public MainModelsController(Contexto context)
        {
            _context = context;
        }

        // GET: MainModels
        public async Task<IActionResult> Index()
        {
              return _context.MainModels != null ? 
                          View(await _context.MainModels.ToListAsync()) :
                          Problem("Entity set 'Contexto.MainModels'  is null.");
        }

        // GET: MainModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MainModels == null)
            {
                return NotFound();
            }

            var mainModel = await _context.MainModels
                .FirstOrDefaultAsync(m => m.id == id);
            if (mainModel == null)
            {
                return NotFound();
            }

            return View(mainModel);
        }

        // GET: MainModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MainModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,cep,logradouro,complemento,bairro,localidade,uf,unidade,ibge,gia")] MainModel mainModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mainModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mainModel);
        }

        // GET: MainModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MainModels == null)
            {
                return NotFound();
            }

            var mainModel = await _context.MainModels.FindAsync(id);
            if (mainModel == null)
            {
                return NotFound();
            }
            return View(mainModel);
        }

        // POST: MainModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,cep,logradouro,complemento,bairro,localidade,uf,unidade,ibge,gia")] MainModel mainModel)
        {
            if (id != mainModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mainModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainModelExists(mainModel.id))
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
            return View(mainModel);
        }

        // GET: MainModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MainModels == null)
            {
                return NotFound();
            }

            var mainModel = await _context.MainModels
                .FirstOrDefaultAsync(m => m.id == id);
            if (mainModel == null)
            {
                return NotFound();
            }

            return View(mainModel);
        }

        // POST: MainModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MainModels == null)
            {
                return Problem("Entity set 'Contexto.MainModels'  is null.");
            }
            var mainModel = await _context.MainModels.FindAsync(id);
            if (mainModel != null)
            {
                _context.MainModels.Remove(mainModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MainModelExists(int id)
        {
          return (_context.MainModels?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
