using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BBC.Data;
using BBC.Models;
using Microsoft.AspNetCore.Identity;
using BBC.DataAccess.Interfaces;
using BBC.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;


namespace BBC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return View(products);
        }

        // GET: Product/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _unitOfWork.Products.GetProductWithReviewsAsync(id.Value);
            if (product == null) return NotFound();

            product.Reviews = (ICollection<Review>)await _unitOfWork.Reviews.GetReviewsByProductIdAsync(id.Value);


            return View(product);
        }


        // GET: Product/Create
        public IActionResult Create()
        {
            // Получаем все категории
            ViewData["CategoryId"] = new SelectList(_unitOfWork.Categories.GetAllAsync().Result, "CategoryId", "Name");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,CategoryId")] Product product)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Images");
            ModelState.Remove("Image");
            ModelState.Remove("Reviews");
            try
            {
                // Убираем обработку изображения
                // No need to handle the Image field here

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Products.AddAsync(product);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
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
                }

                ViewData["CategoryId"] = new SelectList(_unitOfWork.Categories.GetAllAsync().Result, "CategoryId", "Name", product.CategoryId);
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating product: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _unitOfWork.Products.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            // Получаем все категории для выпадающего списка
            ViewData["CategoryId"] = new SelectList(_unitOfWork.Categories.GetAllAsync().Result, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,CategoryId")] Product product)
        {
            ModelState.Remove("Category");
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _unitOfWork.Products.ExistsAsync(product.ProductId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_unitOfWork.Products.GetAllAsync().Result.Select(p => p.Category), "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _unitOfWork.Products.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}
