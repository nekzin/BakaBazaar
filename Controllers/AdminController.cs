using BBC.Data;
using BBC.DataAccess.Interfaces;
using BBC.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BBC.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        public AdminController(IUnitOfWork unitOfWork, ApplicationDbContext context, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Products()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return View(products);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ManageUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ManageProducts()
        {
            var products = await _productRepository.GetAllAsync(); // Возвращает IEnumerable<Product>
            return View(products);
        }


    }


}
