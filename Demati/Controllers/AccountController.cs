﻿using Demati.DataAccessLayer;
using Demati.Interfaces;
using Demati.Models;
using Demati.ViewModels.AccountVMs;
using Demati.ViewModels.BasketVMs;
using Demati.ViewModels.ProductVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace Demati.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> Verify(string email, string token)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser is null)
            {
                return NotFound();
            }
            await _userManager.ConfirmEmailAsync(appUser, token);
            await _signInManager.SignInAsync(appUser, true);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                Email = registerVM.Email,
                UserName = registerVM.Name+registerVM.Surname
            };
            //if (!await  _userManager.Users.AnyAsync(e=>e.NormalizedEmail == registerVM.Email.ToUpperInvariant().Trim()))
            //{
            //    ModelState.AddModelError("Email", $"{registerVM.Email} is already taken");
            //    return View(registerVM);
            //}

            //if (!await _userManager.Users.AnyAsync(e => e.NormalizedUserName == registerVM.Username.ToUpperInvariant().Trim()))
            //{
            //    ModelState.AddModelError("Username", $"{registerVM.Username} is already taken");
            //    return View(registerVM);
            //}


            await _userManager.AddToRoleAsync(appUser, "Member");
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> CreateRole()
        {
            await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));
            return Content("Successfully");

        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            AppUser appUser = new AppUser
            {
                Name = "Super",
                Surname = "Admin",
                Email = "superadmin@gmail.com",
                UserName = "SuperAdmin"
            };

            await _userManager.CreateAsync(appUser, "SuperAdmin17");
            await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

            return Content("Role added successfully");

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);
            AppUser appUser = await _userManager.Users
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .Include(u => u.Wishlists.Where(u=> u.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());


            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect ");
                return View(loginVM);

            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password,true, true);


            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", $"Your Account has been blocked. It will be active again after {appUser.LockoutEnd} ");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect ");
                return View(loginVM);
            }
            if ((await _userManager.IsInRoleAsync(appUser, "SuperAdmin")) || (await _userManager.IsInRoleAsync(appUser, "Admin")))
            {
                return RedirectToAction("index", "dashboard", new { area = "manage" });
            }

            string basket = HttpContext.Request.Cookies["basket"];
            HttpContext.Response.Cookies.Append("basket", "");
            if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
            {
                List<BasketVM> basketVMs = new List<BasketVM>();

                foreach (Basket basket1 in appUser.Baskets)
                {
                    BasketVM basketVM = new BasketVM
                    {
                        Id = (int)basket1.ProductId,
                        Count = basket1.Count,
                        SizeId = (int)basket1.SizeId,
                        ColorId = (int)basket1.ColorId,
                    };
                    basketVMs.Add(basketVM);
                }
                basket = JsonConvert.SerializeObject(basketVMs);
                HttpContext.Response.Cookies.Append("basket", basket);
            }

            string wishlist = HttpContext.Request.Cookies["wishlist"];
            HttpContext.Response.Cookies.Append("wishlist", "");
            if (appUser.Wishlists != null && appUser.Wishlists.Count() > 0)
            {
                List<WishlistVM> wishlistVMs = new List<WishlistVM>();

                foreach (Wishlist wishlist1 in appUser.Wishlists)
                {
                    WishlistVM wishlistVM = new WishlistVM
                    {
                        Id = (int)wishlist1.ProductId,
                    };
                    wishlistVMs.Add(wishlistVM);
                }
                wishlist = JsonConvert.SerializeObject(wishlistVMs);
                HttpContext.Response.Cookies.Append("wishlist", wishlist);
            }

            return RedirectToAction("Profile", "Account");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .Include(o => o.Orders.Where(o => o.IsDeleted == false))
                .ThenInclude(oi => oi.OrderItems.Where(i => i.IsDeleted == false)).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfilVM profileVM = new ProfilVM
            {
                Name = appUser.Name,
                Surname = appUser.Surname,
                Username = appUser.UserName,
                Email = appUser.Email,
                Addresses = appUser.Addresses,
                Orders = appUser.Orders
            };

            ViewBag.UserName = profileVM.Username;
            ViewBag.Email = profileVM.Email;
            ViewBag.Addresses = profileVM.Addresses;

            return View(profileVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(ProfilVM profileVM)
        {
            if (!ModelState.IsValid)
            {

                return View(profileVM);
            }
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (profileVM.Name != null)
            {
                appUser.Name = profileVM.Name;
            }
            if (profileVM.Surname != null)
            {
                appUser.Surname = profileVM.Surname;
            }


            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email = profileVM.Email;
            }
            if (appUser.NormalizedUserName != profileVM.Username.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.Username;
            }
            appUser = await _userManager.Users
                .Include(o => o.Orders.Where(o => o.IsDeleted == false))
                .ThenInclude(oi => oi.OrderItems.Where(i => i.IsDeleted == false)).ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {

                    ModelState.AddModelError("", identityError.Description);
                }
                return View(profileVM);
            }
            await _signInManager.SignInAsync(appUser, true);
            if (!string.IsNullOrWhiteSpace(profileVM.OldPassword))
            {
                if (!await _userManager.CheckPasswordAsync(appUser, profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old Password is incorrect");
                    return View(profileVM);
                }
                if (profileVM.OldPassword == profileVM.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "Old Password and New Password cannot be similar");
                    return View(profileVM);

                }
                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.NewPassword);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {

                        ModelState.AddModelError("", identityError.Description);
                    }
                    return View(profileVM);
                }
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfilVM profileVM = new ProfilVM
            {
                Name = appUser.Name,
                Surname = appUser.Surname,
                Username = appUser.UserName,
                Email = appUser.Email,
                Addresses = appUser.Addresses
            };

            if (!ModelState.IsValid) return View(nameof(Profile), profileVM);
            if (address.IsMain == true && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(a => a.IsMain))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain).IsMain = false;

            }
            address.UserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            address.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            TempData["Tab"] = "address";
            return RedirectToAction(nameof(Profile));
        }


        public async Task<IActionResult> DeleteAddress(int? addressId)
        {
            Address addressToDelete = await _context.Addresses.FindAsync(addressId);

            if (addressToDelete == null)
            {
                return BadRequest();
            }

            try
            {
                _context.Addresses.Remove(addressToDelete); 
                await _context.SaveChangesAsync(); 

                TempData["Tab"] = "address";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int? id)
        {
            Address address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return BadRequest();
            }

            return PartialView("_AddressModal",address);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateAddress(int? id,Address updatedAddress)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedAddress);
            }

            try
            {
                Address existingAddress = await _context.Addresses.FindAsync(id);

                if (existingAddress == null)
                {
                    return BadRequest();
                }

                AppUser appUser = await _userManager.Users
               .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
               .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (updatedAddress.IsMain == true && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(a => a.IsMain))
                {
                    appUser.Addresses.FirstOrDefault(a => a.IsMain).IsMain = false;

                }

                existingAddress.Country = updatedAddress.Country;
                existingAddress.Company = updatedAddress.Company;
                existingAddress.City = updatedAddress.City;
                existingAddress.PostalCode = updatedAddress.PostalCode;
                existingAddress.Phone = updatedAddress.Phone;
                existingAddress.AddressLine = updatedAddress.AddressLine;
                existingAddress.IsMain = updatedAddress.IsMain;

                _context.Entry(existingAddress).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["Tab"] = "address";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
