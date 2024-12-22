using BBC.DataAccess.Interfaces;
using BBC.DataAccess.Repositories;
using BBC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BBC.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public ReviewController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByProductIdAsync(productId);
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingReview = await _unitOfWork.Reviews.GetReviewByProductAndUserIdAsync(productId, user.Id);

            if (existingReview != null)  // Если отзыв уже существует, обновляем его
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedAt = DateTime.Now;

                _unitOfWork.Reviews.Update(existingReview);
            }
            else  // Создаем новый отзыв
            {
                if (rating < 1 || rating > 5)
                    ModelState.AddModelError("Rating", "Rating must be between 1 and 5");
                if (string.IsNullOrEmpty(comment))
                    ModelState.AddModelError("Comment", "Comment is required");

                if (!ModelState.IsValid) // Если есть ошибки, возвращаем форму
                {
                    return View(); // Здесь предполагается, что форма отзыва отображается на странице товара
                }

                var review = new Review
                {
                    UserId = user.Id,
                    ProductId = productId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Reviews.AddAsync(review);
            }

            await _unitOfWork.CompleteAsync();

            // Перенаправляем на страницу товара
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpGet]
        public async Task<IActionResult> Create(int productId)
        {
            if (ModelState.IsValid)
            {


                var user = await _userManager.GetUserAsync(User);

                // Получаем список пользователей для выборки
                ViewBag.UserId = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");

                // Получаем список продуктов для выборки (если необходимо)
                var products = await _unitOfWork.Products.GetAllAsync();
                ViewBag.ProductId = new SelectList(products, "ProductId", "Name", productId); // Здесь передаем выбранный productId
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Validation Error for {key}: {error.ErrorMessage}");
                    }
                }
                return RedirectToAction("Index", new { productId = productId });
            }
            return View(new Review { ProductId = productId }); 
        }
    }
}
