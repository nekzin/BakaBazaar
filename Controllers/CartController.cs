using BBC.DataAccess.Repositories;
using BBC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BBC.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BBC.Controllers;
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<ProductController> _logger;
    public CartController(IUnitOfWork unitOfWork, UserManager<User> userManager, ICartRepository cartRepository, ILogger<ProductController> logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _cartRepository = cartRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(user.Id);
        if (cart == null)
        {
            cart = new Cart { UserId = user.Id };
            await _unitOfWork.Carts.AddCartAsync(cart);
            await _unitOfWork.CompleteAsync();
        }

        return View(cart.Items);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(user.Id);
        if (cart == null) return NotFound();

        var cartItem = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
        if (cartItem != null)
        {
            try
            {
                // Удаляем CartItem через UnitOfWork (через Cart репозиторий)
                _unitOfWork.Carts.RemoveItemFromCart(cartItem);  // Используйте репозиторий Carts или добавьте метод в UnitOfWork

                // Сохраняем изменения в базе данных
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new
                {
                    success = false,
                    message = "Ошибка при удалении товара из корзины. Товар был изменен или удален."
                });
            }
        }

        return Json(new
        {
            success = true,
            message = "Item removed from cart",
            totalItems = cart.Items.Count,
            totalPrice = cart.Items.Sum(i => i.Price * i.Quantity).ToString("C")
        });
    }







    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Console.WriteLine("User not found");
            return Unauthorized();
        }

        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            Console.WriteLine($"Product not found with ID: {productId}");
            return NotFound();
        }

        Console.WriteLine($"Found product: {product.Name} with ID: {product.ProductId}");

        var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(user.Id);
        if (cart == null)
        {
            cart = new Cart { UserId = user.Id };
            await _unitOfWork.Carts.AddCartAsync(cart);
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price
            });
        }

        await _unitOfWork.CompleteAsync();

        // После добавления товара в корзину, переадресуем на страницу корзины
        return RedirectToAction("Index", "Cart");
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(user.Id);
        if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");

        var checkoutViewModel = new CheckoutViewModel
        {
            FullName = user.FullName, // Добавьте это поле в модель User, если нужно
            Email = user.Email,
            CartItems = cart.Items.Select(i => new CartItemViewModel
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList(),
            TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity)
        };

        return View(checkoutViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(user.Id);
        if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");

        if (ModelState.IsValid)
        {
            var order = new Order
            {
                UserId = user.Id,
                Status = OrderStatus.Processing,
                TotalPrice = model.CartItems.Sum(i => i.Price * i.Quantity),
                CreatedAt = DateTime.Now,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);

            // Очищаем корзину
            _unitOfWork.Carts.ClearCart(cart);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Details", "Order", new { id = order.OrderId });
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

        model.CartItems = cart.Items.Select(i => new CartItemViewModel
        {
            ProductName = i.Product.Name,
            Quantity = i.Quantity,
            Price = i.Price
        }).ToList();
        model.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

        return View(model);
    }


    // Методы CRUD
    public IActionResult Create()
    {
        var users = _userManager.Users.ToList();
        ViewBag.UserId = new SelectList(users, "Id", "UserName");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Cart cart)
    {
        if (ModelState.IsValid)
        {
            await _cartRepository.AddCartAsync(cart);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("Index");
        }
        var users = _userManager.Users.ToList();
        ViewBag.UserId = new SelectList(users, "Id", "UserName", cart.UserId);
        return View(cart);
    }

    // Метод для редактирования корзины
    public async Task<IActionResult> Edit(int id)
    {
        var cart = await _unitOfWork.Carts.GetByIdAsync(id);
        if (cart == null) return NotFound();

        var users = _userManager.Users.ToList();
        ViewBag.UserId = new SelectList(users, "Id", "UserName", cart.UserId);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Cart cart)
    {
        if (ModelState.IsValid)
        {
            _cartRepository.Update(cart);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("Index");
        }
        var users = _userManager.Users.ToList();
        ViewBag.UserId = new SelectList(users, "Id", "UserName", cart.UserId);
        return View(cart);
    }

    // Метод для удаления корзины
    public async Task<IActionResult> Delete(int id)
    {
        var cart = await _unitOfWork.Carts.GetByIdAsync(id);
        if (cart == null) return NotFound();
        return View(cart);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _cartRepository.DeleteCart(id);
        await _unitOfWork.CompleteAsync();
        return RedirectToAction("Index");
    }

    // Метод для просмотра деталей корзины
    public async Task<IActionResult> Details(int id)
    {
        var cart = await _unitOfWork.Carts.GetByIdAsync(id);
        if (cart == null) return NotFound();
        return View(cart);
    }
}
