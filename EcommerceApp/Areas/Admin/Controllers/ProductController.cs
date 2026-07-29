using EcommerceApp.Data;
using EcommerceApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]   
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int MaxImageSize = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

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
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product,IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(product);
            }
            if (image != null)
            {
                if (!IsImageValid(image))
                {
                    LoadCategories();
                    return View(product);
                }
                product.ImageUrl = UploadImage(image);
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
                if (!IsImageValid(image))
                {
                    LoadCategories();
                    return View(product);
                }
                DeleteImage(productFromDb.ImageUrl);
                productFromDb.ImageUrl = UploadImage(image);
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
        public IActionResult DeletePost(int id)
        {
            var productFromDb = _context.Products.Find(id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            DeleteImage(productFromDb.ImageUrl);
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
        private void DeleteImage(string? imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return;
            var oldImagePath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "images",
                "products",
                imageName);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            
        }
        private bool IsImageSizeValid(IFormFile image) 
        {
            if(image.Length > MaxImageSize)
            {
                ModelState.AddModelError("image", "Maximum file size is 5 MB.");
                return false;
            }
            return true;
        }
        private bool IsImageExtensionValid(string extension)
        {                       
            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("image", "Only image files are allowed.");
                return false;
            }
            return true;
        }
        private bool IsImageValid(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            return IsImageSizeValid(image) && IsImageExtensionValid(extension);
        }
        private string UploadImage(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var filename = Guid.NewGuid().ToString() + extension;
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", filename);
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                image.CopyTo(stream);
            }
            return filename;
        }
    }
}