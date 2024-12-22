using BBC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BBC.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace BBC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return View(orders);
        }

        // GET: Order/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id.Value);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync(); // Получаем всех пользователей
            ViewData["UserId"] = new SelectList(users, "Id", "UserName");
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>());
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,Status,TotalPrice,CreatedAt")] Order order)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(order.UserId);
                if (user == null)
                    return NotFound("User not found");

                order.User = user; // Привязываем пользователя к заказу
                await _unitOfWork.Orders.AddAsync(order);
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
            var users = await _userManager.Users.ToListAsync();
            ViewData["UserId"] = new SelectList(users, "Id", "UserName", order.UserId);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id.Value);
            if (order == null)
                return NotFound();

            var users = await _userManager.Users.ToListAsync();
            ViewData["UserId"] = new SelectList(users, "Id", "UserName", order.UserId);
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,Status,TotalPrice,CreatedAt")] Order order)
        {
            if (id != order.OrderId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    if (user == null)
                        return NotFound("User not found");

                    order.User = user;
                    await _unitOfWork.Orders.UpdateAsync(order);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _unitOfWork.Orders.OrderExistsAsync(order.OrderId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var users = await _userManager.Users.ToListAsync();
            ViewData["UserId"] = new SelectList(users, "Id", "UserName", order.UserId);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id.Value);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
            if (order != null)
            {
                await _unitOfWork.Orders.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
