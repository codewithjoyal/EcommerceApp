using EcommerceApp.Data;
using EcommerceApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(product);
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            TempData["Success"] = "Product created Successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            LoadCategories();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(product);
            }
            _context.Products.Update(product);
            _context.SaveChanges();
            TempData["Success"] = "Product updated succesfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            return View();
        }
        private void LoadCategories()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
