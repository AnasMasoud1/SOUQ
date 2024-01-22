using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Souq.Data;
using Souq.Models;
using System.Diagnostics;

namespace Souq.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //Dependency injection  
        private readonly ApplicationDbContext _context; 

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // At first add new model identify tables u need 
            // then identify object ftom action then identify each table fron this object like:
            IndexVM result = new IndexVM();
            result.Reviews=_context.Reviews.ToList();
            result.Categories=_context.Categories.ToList();
            result.Products=_context.Products.ToList();
            return View(result);
        }
        public IActionResult Category()
        {
            // If U want to use ViewBag to get more tables from database..
            // because u can identify only one variable in action like => (var  cat=_context......... )
            // & at actions view @model List<Category>
            // second table u can use ViewBag.rev on foreach.
            ViewBag.rev= _context.Reviews.ToList();
            var  cat = _context.Categories.ToList();
            return View(cat);
        }
        public IActionResult GetProductByCategoryID(int id)
        {
            var product = _context.Products.Include(c=>c.category).Where(x => x.CategoryId == id).ToList();
            return View(product);
        }
        //public IActionResult GetSingleProductByCategoryID(int id)
        //{
        //    ViewBag.cate = _context.Categories.FirstOrDefault(x => x.Id == id);
        //    var product = _context.Products.Where(x => x.CategoryId == id).FirstOrDefault();
        //    return View(product);
        //}
        public IActionResult ProductSearch(string xname)
        {
            var pro = _context.Products.Include(c => c.category).Where(x=>x.Name.Contains(xname)).ToList();
            return View(pro);
        }

        [HttpGet]
        public IActionResult SendReview()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendReview(Review review)
        {
            _context.Reviews.Add(new Review
            {
                Name=review.Name,
                Subject=review.Subject,
                message=review.message,
                Email=review.Email,
            });
            _context.SaveChanges();
            // because u need to save this data not to represens use : 
          return RedirectToAction("Index") ;
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult CurrentProduct(int id)
        {
            ViewBag.pro = _context.Products.ToList();
            var product = _context.Products.Include(x=>x.category).FirstOrDefault(x => x.Id == id);
            return View(product);
        }

        public IActionResult FilterByPrice()
        {
            //if want to present 3 cheapest items..if it's descending will present 3 most expensive items:
            //var product = _context.Products.OrderBy(x => x.Price).Take(3).ToList();
            var product = _context.Products.Include(c => c.category).OrderBy(x => x.Price).ToList();
            return View(product);
        }
        public IActionResult FilterByDate()
        {
            var product = _context.Products.Include(c => c.category).OrderByDescending(x => x.CreatedDate).ToList();
            return View(product);
        }
       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
