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
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Carts.Include(c => c.product).Where(x => x.UserID == User.Identity.Name);
            return View(await applicationDbContext.ToListAsync());
        }
        [HttpGet]
        public IActionResult GoFormOrder()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddOrder(Order order)
        {
            Order o = new Order { 
                Email = order.Email,
                Address = order.Address,
                Name = order.Name,
                PhoneNumber = order.PhoneNumber,
                UserID = User.Identity.Name
            };
            //عشان افضي السلة 
            var CartItem=_context.Carts.Where(x=> x.UserID == User.Identity.Name).ToList();
            _context.Carts.RemoveRange(CartItem);
            _context.Order.Add(o);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
   
        public IActionResult AddProductToCart(int id)
        {
            var price = _context.Products.Find(id).Price;
            var item =_context.Carts.FirstOrDefault(x => x.ProductId == id && x.UserID ==User.Identity.Name);
            if(item != null)
            {
                item.Quentity += 1;
                item.TotalPrice = price * item.Quentity;
                item.Price = price;
            }
            else
            {
                _context.Carts.Add(new Cart
                {
                    ProductId = id,
                    Price = price ,
                    UserID = User.Identity.Name,
                    Quentity=1,
                    TotalPrice = price,
                    DateTime = DateTime.Now,
                });
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult GetSingleProductByCategoryID(int id)
        {
            //ViewBag.cate = _context.Categories.FirstOrDefault(x => x.Id == id);
            var product = _context.Carts.Include(x=> x.product).ThenInclude(v=>v.category.Name).FirstOrDefault(c=> c.Id== id);
            return View(product);
        }
        // GET: Carts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,Price,Quentity,UserID,ProductId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,Price,Quentity,UserID,ProductId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
          return (_context.Carts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
