// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BBC.DataAccess.Repositories;
using BBC.DataAccess.UnitOfWork;
using BBC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BBC.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public List<CartItem> CartItems { get; set; }
        public List<Wishlist> WishlistItems { get; set; }
        public string Email { get; set; }
        public string Name  { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("Identity/Account/Login");
            }

            await LoadAsync(user);

            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(user.Id);
            CartItems = cart?.Items.ToList() ?? new List<CartItem>();
            var wishlist = await _unitOfWork.Wishlist.GetWishlistItemsByUserIdAsync(user.Id);
            WishlistItems = wishlist.ToList();
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(user.Id);

            Orders = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString()
            }).ToList();

            return Page();
        }



        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [Display(Name = "Имя пользователя")]
            public string UserName { get; set; } // Новое поле для изменения имени

            [Phone]
            [Display(Name = "Номер телефона")]
            public string PhoneNumber { get; set; }
        }


        private async Task LoadAsync(User user)
        {
            Username = user.UserName; // Текущее имя пользователя
            Email = user.Email;

            Input = new InputModel
            {
                UserName = user.UserName, // Заполнение поля для редактирования
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
            };
        }




        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("Identity/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            if (Input.UserName != user.UserName)
            {
                var setNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!setNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set username.";
                    return RedirectToPage();
                }
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    foreach (var error in setPhoneResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadAsync(user);
                    return Page();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Ваш профиль обновлён";
            return RedirectToPage();
        }

    }
}
