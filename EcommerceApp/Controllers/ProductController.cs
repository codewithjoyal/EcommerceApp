using EcommerceApp.Data;
using EcommerceApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }
        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId) 
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }
            var cartItem = _context.ShoppingCarts.FirstOrDefault
                (c => c.ApplicationUserId == userId && c.ProductId == productId);
            if (cartItem == null)
            {
                var shoppingCart = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    Count = 1
                };
                _context.ShoppingCarts.Add(shoppingCart);
            }
            else
            {
                cartItem.Count++;
            }
            _context.SaveChanges();
            TempData["Success"] = "Product added to cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}
