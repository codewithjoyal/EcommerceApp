using EcommerceApp.Data;
using EcommerceApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]   
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Product product,IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(product);
            }
            if (image != null)
            {
                if (image.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("image", "Maximum file size is 5 MB.");
                    LoadCategories();
                    return View(product);
                }
                var extension = Path.GetExtension(image.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    ModelState.AddModelError("image", "Only image files are allowed.");
                    LoadCategories();
                    return View(product);
                }
                var filename = Guid.NewGuid().ToString();             
                var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", filename + extension);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                product.ImageUrl = filename + extension;
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            TempData["Success"] = "Product created Successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id,Product product,IFormFile? image)
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
            var productFromDb = _context.Products.Find(id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            if (image != null)
            {                
                if (image.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("image", "Maximum file size is 5 MB.");
                    LoadCategories();
                    return View(product);
                }             
                var extension = Path.GetExtension(image.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    ModelState.AddModelError("image", "Only image files are allowed.");
                    LoadCategories();
                    return View(product);
                }
                var filename = Guid.NewGuid().ToString();
                var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", filename + extension);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                if (!string.IsNullOrEmpty(productFromDb.ImageUrl))
                {
                    var oldImagePath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "products",
                        productFromDb.ImageUrl);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                productFromDb.ImageUrl = filename + extension;
            }

            productFromDb.Name = product.Name;
            productFromDb.Description = product.Description;
            productFromDb.Price = product.Price;
            productFromDb.Stock = product.Stock;
            productFromDb.CategoryId = product.CategoryId;
            _context.SaveChanges();
            TempData["Success"] = "Product updated succesfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeletePost(int id)
        {
            var productFromDb = _context.Products.Find(id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            _context.Products.Remove(productFromDb);
            _context.SaveChanges();
            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        private void LoadCategories()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
