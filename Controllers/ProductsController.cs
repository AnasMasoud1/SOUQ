using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Souq.Data;
using Souq.Models;

namespace Souq.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.category).Where(x => x.UserID == User.Identity.Name);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,CreatedDate,Image,CategoryId,ImageFile,UserID")] Product product)
        {
           
            if (ModelState.IsValid)
            {            
                if (product.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string extention = Path.GetExtension(product.ImageFile.FileName).ToLower();
                    string path = Path.Combine(wwwRootPath + "/Imgs/" + fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.Image = fileName;
                    product.UserID = User.Identity.Name;
                }
              
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CreatedDate,Image,CategoryId,ImageFile,UserID")] Product product)
        {
            //if (id != product.Id)
            //{
            //    return NotFound();
            //}

            //try
            //{
            //    if (ModelState.IsValid)
            //    {
            //        if (product.ImageFile != null)
            //        {
            //            string wwwRootPath = _webHostEnvironment.WebRootPath;
            //            string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
            //            string extention = Path.GetExtension(product.ImageFile.FileName).ToLower();
            //            string path = Path.Combine(wwwRootPath + "/Imgs/", fileName);

            //            using (var fileStream = new FileStream(path, FileMode.Create))
            //            {
            //                await product.ImageFile.CopyToAsync(fileStream);
            //            }
            //            product.Image = fileName;
            //        }

            //        _context.Update(product);
            //        await _context.SaveChangesAsync();
            //        return RedirectToAction(nameof(Index));
            //    }
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ProductExists(product.Id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            //return View(product);

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    // If a new image file is provided, save it
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath, "Imgs", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    // Delete the previous image file (if any)
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        string previousImagePath = Path.Combine(wwwRootPath, "Imgs", product.Image);
                        if (System.IO.File.Exists(previousImagePath))
                        {
                            System.IO.File.Delete(previousImagePath);
                        }
                    }
                    
                    product.Image = fileName;
                }
                else
                {
                    // If no new image file is provided, retain the existing image
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
                    product.Image = existingProduct?.Image;
                    product.UserID = User.Identity.Name;
                }

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
