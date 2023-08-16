using Demati.DataAccessLayer;
using Demati.Models;
using Demati.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace Demati.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;


        public ProductController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index(int pageIndex = 1)
        {
            IEnumerable<Product> products = _context.Products
                .Include(p => p.ProductColors.Where(pc =>pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes.Where(ps => ps.IsDeleted == false)).ThenInclude(pt => pt.Size)
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            return View(PagenatedList<Product>.Create(products, pageIndex, 4));
        }

        
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductCategories.Where(pc => pc.IsDeleted ==false)).ThenInclude(pc => pc.Category)
                .Include(p => p.ProductColors.Where(pc => pc.IsDeleted == false)).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes.Where(pc => pc.IsDeleted== false)).ThenInclude(pt => pt.Size)
                .Include(p => p.ProductImages.Where(pc => pc.IsDeleted == false))
                .Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return PartialView("_error-404");

            return View(product);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(b => b.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ViewBag.Categories = await _context.Categories.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(b => b.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(product);

            if (product.Price < product.DisCountPrice)
            {
                ModelState.AddModelError("Price", "The Price Cannot Be Lower Than The Discount Price");
                ModelState.AddModelError("DisCountPrice", "Discount Price Cannot Be More Than The Price");
                return View(product);
            }

            if (product.Count < 0)
            {
                ModelState.AddModelError("Count", "Count Cannot Be Less Than Zero");
                return View(product);
            }

            if (product.Images != null && product.Images.Count() > 5)
            {
                ModelState.AddModelError("Images", "Maximum 5 Images Must Be Selected");
                return View(product);
            }

            if (product.MainFile != null && !product.MainFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainFile", "File Type Is InCorrect");
                return View(product);
            }

            if (product.MainFile != null && (product.MainFile.Length / 1024) > 500)
            {
                ModelState.AddModelError("MainFile", "File Size Can Be Max 500kb");
                return View(product);
            }

            if (product.HoverFile != null && !product.HoverFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("HoverFile", "File Type Is InCorrect");
                return View(product);
            }

            if (product.HoverFile != null && (product.HoverFile.Length / 1024) > 500)
            {
                ModelState.AddModelError("HoverFile", "File Size Can Be Max 500kb");
                return View(product);
            }

            if (product.Images != null)
            {
                foreach (IFormFile file in product.Images)
                {
                    if (!file.ContentType.Contains("image/"))
                    {
                        ModelState.AddModelError("Images", "File Type Is InCorrect");
                        return View(product);
                    }

                    if ((file.Length / 1024) > 300)
                    {
                        ModelState.AddModelError("Images", "File Size Can Be Max 300kb");
                        return View(product);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Images", "Minumum 1 Image Must Be Selected");
                return View(product);
            }

            if (product.MainFile != null)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + product.MainFile.FileName.Substring(product.MainFile.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets","images", "Product", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await product.MainFile.CopyToAsync(fileStream);
                }

                product.MainImage = fileName;
            }
            else
            {
                ModelState.AddModelError("MainFile", "Image Must be Selected");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + product.HoverFile.FileName.Substring(product.HoverFile.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await product.HoverFile.CopyToAsync(fileStream);
                }

                product.HoverImage = fileName;
            }
            else
            {
                ModelState.AddModelError("HoverFile", "Image Must be Selected");
                return View(product);
            }

            List<ProductImage> productImages = new List<ProductImage>();

            if (product.Images != null)
            {
                foreach (IFormFile file in product.Images)
                {
                    if (!file.ContentType.Contains("image/"))
                    {
                        ModelState.AddModelError("Images", "File Type Is InCorrect");
                        return View(product);
                    }

                    if ((file.Length / 1024) > 300)
                    {
                        ModelState.AddModelError("Images", "File Size Can Be Max 300kb");
                        return View(product);
                    }

                    string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + file.FileName.Substring(file.FileName.LastIndexOf('-'));

                    string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", "Detail", fileName);

                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = fileName,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }
            }
            else
            {
                ModelState.AddModelError("Images", "Minumum 1 Image Must Be Selected");
                return View(product);
            }

            if (product.CategoryIds == null)
            {
                ModelState.AddModelError("CategoryIds", $"Must Be Selected");
                return View(product);
            }

            List<ProductCategory> productCategories = new List<ProductCategory>();

            foreach (int categoryId in product.CategoryIds)
            {
                if (!await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Id == categoryId))
                {
                    ModelState.AddModelError("CategoryIds", $"CategoryIds: {categoryId} - InCorrect");
                    return View(product);
                }

                ProductCategory productCategory = new ProductCategory
                {
                    CategoryId = categoryId,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow.AddHours(4)
                };

                productCategories.Add(productCategory);
            }

            if (product.ColorIds == null)
            {
                ModelState.AddModelError("ColorIds", $"Must Be Selected");
                return View(product);
            }

            List<ProductColor> productColors = new List<ProductColor>();

            foreach (int colorId in product.ColorIds)
            {
                if (!await _context.Colors.AnyAsync(b => b.IsDeleted == false && b.Id == colorId))
                {
                    ModelState.AddModelError("ColorIds", $"CategoryIds: {colorId} - InCorrect");
                    return View(product);
                }

                ProductColor productColor = new ProductColor
                {
                    ColorId = colorId,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow.AddHours(4)
                };

                productColors.Add(productColor);
            }

            if (product.SizeIds == null)
            {
                ModelState.AddModelError("SizeIds", $"Must Be Selected");
                return View(product);
            }

            List<ProductSize> productSizes = new List<ProductSize>();

            foreach (int sizeId in product.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(b => b.IsDeleted == false && b.Id == sizeId))
                {
                    ModelState.AddModelError("SizeIds", $"CategoryIds: {sizeId} - InCorrect");
                    return View(product);
                }

                ProductSize productSize = new ProductSize
                {
                    SizeId = sizeId,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow.AddHours(4)
                };

                productSizes.Add(productSize);
            }

            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            product.ProductColors = productColors;
            product.ProductSizes = productSizes;
            product.ProductCategories = productCategories;
            product.ProductImages = productImages;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductCategories.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductColors.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductSizes.Where(pt => pt.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return BadRequest();

            ViewBag.Categories = await _context.Categories.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(b => b.IsDeleted == false).ToListAsync();

            product.CategoryIds = product.ProductCategories?.Select(c => (int)c.CategoryId).ToList();
            product.ColorIds = product.ProductColors?.Select(c => (int)c.ColorId).ToList();
            product.SizeIds = product.ProductSizes?.Select(c => (int)c.SizeId).ToList();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ViewBag.Categories = await _context.Categories.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Colors = await _context.Colors.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Sizes = await _context.Sizes.Where(b => b.IsDeleted == false).ToListAsync();

            if (id == null) return BadRequest();

            if (product.Id != id) return BadRequest(product);

            Product? dbProduct = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductCategories.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductColors.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductSizes.Where(pt => pt.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            int imageCount = int.Parse(_context.Settings.FirstOrDefault(s => s.Key == "ImageCount").Value);

            int canUpload = imageCount - dbProduct.ProductImages.Count();

            if(product.Images != null && product.Images.Count() > canUpload)
            {
                ModelState.AddModelError("Images", $"Maximum {canUpload} Images Must Be Selected");
                return View(dbProduct);
            }

            if (dbProduct == null) return PartialView("_error-404");

            if (!ModelState.IsValid) return View(product);

            if (product.Price < product.DisCountPrice)
            {
                ModelState.AddModelError("Price", "The Price Cannot Be Lower Than The Discount Price");
                ModelState.AddModelError("DisCountPrice", "Discount Price Cannot Be More Than The Price");
                return View(product);
            }

            if (product.Count < 0)
            {
                ModelState.AddModelError("Count", "Count Cannot Be Less Than Zero");
                return View(product);
            }

            if (product.MainFile != null && !product.MainFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainFile", "File Type Is InCorrect");
                return View(product);
            }

            if (product.MainFile != null && (product.MainFile.Length / 1024) > 500)
            {
                ModelState.AddModelError("MainFile", "File Size Can Be Max 500kb");
                return View(product);
            }

            if (product.HoverFile != null && !product.HoverFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("HoverFile", "File Type Is InCorrect");
                return View(product);
            }

            if (product.HoverFile != null && (product.HoverFile.Length / 1024) > 500)
            {
                ModelState.AddModelError("HoverFile", "File Size Can Be Max 500kb");
                return View(product);
            }

            if (product.Images != null)
            {
                foreach (IFormFile file in product.Images)
                {
                    if (!file.ContentType.Contains("image/"))
                    {
                        ModelState.AddModelError("Images", "File Type Is InCorrect");
                        return View(product);
                    }

                    if ((file.Length / 1024) > 300)
                    {
                        ModelState.AddModelError("Images", "File Size Can Be Max 300kb");
                        return View(product);
                    }
                }
            }

            if (product.MainFile != null)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + product.MainFile.FileName.Substring(product.MainFile.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", dbProduct.MainImage);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await product.MainFile.CopyToAsync(fileStream);
                }

                dbProduct.MainImage = fileName;
            }

            if (product.HoverFile != null)
            {
                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + product.HoverFile.FileName.Substring(product.HoverFile.FileName.LastIndexOf('-'));

                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", dbProduct.HoverImage);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", fileName);

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await product.HoverFile.CopyToAsync(fileStream);
                }

                dbProduct.HoverImage = fileName;
            }

            List<ProductImage> productImages = new List<ProductImage>();

            if (product.Images != null)
            {
                foreach (IFormFile file in product.Images)
                {
                    if (!file.ContentType.Contains("image/"))
                    {
                        ModelState.AddModelError("Images", "File Type Is InCorrect");
                        return View(product);
                    }

                    if ((file.Length / 1024) > 300)
                    {
                        ModelState.AddModelError("Images", "File Size Can Be Max 300kb");
                        return View(product);
                    }

                    string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMHHmmssfff") + file.FileName.Substring(file.FileName.LastIndexOf('-'));

                    string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", "Detail", fileName);

                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = fileName,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }
            }

            dbProduct.ProductImages.AddRange(productImages);

            if (product.CategoryIds != null)
            {
                foreach (ProductCategory productCategory in dbProduct.ProductCategories)
                {
                    productCategory.IsDeleted = true;
                }

                List<ProductCategory> productCategories = new List<ProductCategory>();

                foreach (int categoryId in product.CategoryIds)
                {
                    if (!await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Id == categoryId))
                    {
                        ModelState.AddModelError("CategoryIds", $"CategoryIds: {categoryId} - InCorrect");
                        return View(product);
                    }

                    ProductCategory productCategory = new ProductCategory
                    {
                        CategoryId = categoryId,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow.AddHours(4)
                    };

                    productCategories.Add(productCategory);
                }

                dbProduct.ProductCategories.AddRange(productCategories);
            }

            if (product.ColorIds != null)
            {
                foreach (ProductColor productColor in dbProduct.ProductColors)
                {
                    productColor.IsDeleted = true;
                }

                List<ProductColor> productColors = new List<ProductColor>();

                foreach (int colorId in product.ColorIds)
                {
                    if (!await _context.Colors.AnyAsync(b => b.IsDeleted == false && b.Id == colorId))
                    {
                        ModelState.AddModelError("ColorIds", $"CategoryIds: {colorId} - InCorrect");
                        return View(product);
                    }

                    ProductColor productColor = new ProductColor
                    {
                        ColorId = colorId,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow.AddHours(4)
                    };

                    productColors.Add(productColor);
                }

                dbProduct.ProductColors.AddRange(productColors);
            }

            if (product.SizeIds != null)
            {
                foreach (ProductSize productSize in dbProduct.ProductSizes)
                {
                    productSize.IsDeleted = true;
                }

                List<ProductSize> productSizes = new List<ProductSize>();

                foreach (int sizeId in product.SizeIds)
                {
                    if (!await _context.Sizes.AnyAsync(b => b.IsDeleted == false && b.Id == sizeId))
                    {
                        ModelState.AddModelError("SizeIds", $"CategoryIds: {sizeId} - InCorrect");
                        return View(product);
                    }

                    ProductSize productSize = new ProductSize
                    {
                        SizeId = sizeId,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow.AddHours(4)
                    };

                    productSizes.Add(productSize);
                }

                dbProduct.ProductSizes.AddRange(productSizes);
            }

            dbProduct.Title = product.Title.Trim();
            dbProduct.Info = product.Info.Trim();
            dbProduct.Description = product.Description;
            dbProduct.Count = product.Count;
            dbProduct.Price = product.Price;
            dbProduct.DisCountPrice = product.DisCountPrice;
            dbProduct.IsNewArrivals = product.IsNewArrivals;
            dbProduct.IsFeatured = product.IsFeatured;
            dbProduct.IsBestseller = product.IsBestseller;
            dbProduct.isNewIn = product.isNewIn;
            dbProduct.isHot = product.isHot;
            dbProduct.UpdatedBy = $"{appUser.Name} {appUser.Surname}";
            dbProduct.UpdatedAt= DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteImage(int? id, int? productId)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (id == null) return BadRequest();

            if (productId == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductCategories.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductColors.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductSizes.Where(pt => pt.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == productId);

            if (product == null) return PartialView("_error-404");

            if (product.ProductImages == null) return BadRequest();

            if (product.ProductImages.Count() < 2)
            {
                return BadRequest();
            }

            if (product.ProductImages != null && !product.ProductImages.Any(pi => pi.Id == id && pi.IsDeleted == false))
            {
                return PartialView("_error-404");
            }

            string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", "Detail", product.ProductImages.FirstOrDefault(p => p.Id == id).Image);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            product.ProductImages.FirstOrDefault(p => p.Id == id).IsDeleted = true;
            product.ProductImages.FirstOrDefault(p => p.Id == id).DeletedBy = $"{appUser.Name} {appUser.Surname}";
            product.ProductImages.FirstOrDefault(p => p.Id == id).DeletedAt = DateTime.UtcNow.AddHours(4);


            await _context.SaveChangesAsync();

            return PartialView("_ProductImagePartial", product.ProductImages.Where(pi => pi.IsDeleted == false).ToList());
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (id == null) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductCategories.Where(pi => pi.IsDeleted == false))
                .Include(p => p.ProductColors.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductSizes.Where(pt => pt.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return PartialView("_error-404");

            foreach (ProductCategory productCategory in product.ProductCategories)
            {
                productCategory.IsDeleted = true;
                productCategory.DeletedBy = "System";
                productCategory.DeletedAt= DateTime.UtcNow.AddHours(4);
            }

            foreach (ProductColor productColor in product.ProductColors)
            {
                productColor.IsDeleted = true;
                productColor.DeletedBy = "System";
                productColor.DeletedAt = DateTime.UtcNow.AddHours(4);
            }

            foreach (ProductSize productSize in product.ProductSizes)
            {
                productSize.IsDeleted = true;
                productSize.DeletedBy = "System";
                productSize.DeletedAt = DateTime.UtcNow.AddHours(4);
            }

            if (product.ProductImages != null)
            {
                foreach (ProductImage productImage in product.ProductImages)
                {
                    string filePath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", "Detail", productImage.Image);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    productImage.IsDeleted = true;
                    productImage.DeletedBy = "System";
                    productImage.DeletedAt = DateTime.UtcNow.AddHours(4);
                }
            }

            if (product.MainImage != null)
            {
                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", product.MainImage);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            if (product.HoverImage != null)
            {
                string fullPath = Path.Combine(_env.WebRootPath, "assets", "images", "Product", product.HoverImage);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            product.IsDeleted = true;
            product.DeletedBy = $"{appUser.Name} {appUser.Surname}"; ;
            product.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
