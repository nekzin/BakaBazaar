using BBC.DataAccess.Repositories;
using BBC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BBC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return View(products);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Search(string query)
        {
            // �������� ��� �������� ����� UnitOfWork
            var products = await _unitOfWork.Products.GetAllAsync();

            // ���� ������ �� ������, ��������� �������� �� �������� ��� ���������
            if (!string.IsNullOrEmpty(query))
            {
                products = products.Where(p => p.Name.ToLower().Contains(query.ToLower()) || p.Category.Name.ToLower().Contains(query.ToLower())).ToList();
            }


            ViewData["SearchQuery"] = query;  // �������� ������ � �������������
            return View(products);
        }
        public IActionResult About()
        {
            return View();
        }
        public async Task<IActionResult> Catalog()
        {
            var products = await _unitOfWork.Products.GetAllAsync(); // �������� ��� ������
            return View(products);
        }

    }
}
