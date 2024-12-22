using BBC.DataAccess.Interfaces;
using BBC.DataAccess.Repositories;
using BBC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BBC.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public WishlistController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var wishlistItems = await _unitOfWork.Wishlist.GetWishlistItemsByUserIdAsync(user.Id);
            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingItem = await _unitOfWork.Wishlist.GetWishlistItemByUserAndProductAsync(user.Id, productId);

            if (existingItem == null)  // Добавляем товар в список, если его нет
            {
                var wishlistItem = new Wishlist
                {
                    UserId = user.Id,
                    ProductId = productId
                };

                await _unitOfWork.Wishlist.AddAsync(wishlistItem);
                await _unitOfWork.CompleteAsync();
            }

            return Redirect("/Identity/Account/Manage");

        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var wishlistItem = await _unitOfWork.Wishlist.GetWishlistItemByUserAndProductAsync(user.Id, productId);

            if (wishlistItem != null)
            {
                await _unitOfWork.Wishlist.DeleteAsync(wishlistItem);
                await _unitOfWork.CompleteAsync();
            }

            return Redirect("/Identity/Account/Manage");
        }
    }
}
