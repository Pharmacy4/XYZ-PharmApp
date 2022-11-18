using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using xyzpharmacy.Data;
using Microsoft.AspNetCore.Authorization;
using xyzpharmacy.Data.Services;
using xyzpharmacy.Models;
using xyzpharmacy.Data.Static;
using System.Security.Cryptography;
using XYZPharmacy.Data.ViewModels;

namespace xyzpharmacy.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ProductsController : Controller
    {
        

        
        private readonly IProductsService _service;

        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(IProductsService Service, IWebHostEnvironment hostEnvironment)
        {
            _service = Service;
            webHostEnvironment = hostEnvironment;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {

                var data = await _service.GetAllAsync();
                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> CreateAsync()
        {

            try
            {
                var movieDropdownsData = await _service.GetNewProductsDropdownsValues();

                ViewBag.Suppliers = new SelectList(movieDropdownsData.Suppliers, "SupplierId", "SupplierName");
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = "~/Image/" + UploadedFile(model);

                    Product prod = new Product()
                    {
                        ProductName = model.ProductName,
                        ProductPrice = model.ProductPrice,
                        CategoryName = model.CategoryName,
                        SupplierId = model.SupplierId,
                        Stock = model.Stock,
                        ExpiryDate = model.ExpiryDate,
                        MedicinalUse = model.MedicinalUse,
                        MedicineDesc = model.MedicineDesc,
                        ProductImage = uniqueFileName,
                    };

                    await _service.AddAsync(prod);
                    return RedirectToAction(nameof(Index));
                }
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int Id)
        {try
            {
                var SupplierDetails = await _service.GetByIdAsync(Id);
                if (SupplierDetails == null)
                {
                    return View("Not Found");
                }
                return View(SupplierDetails);
            }
            catch (Exception) { throw; }
        }


        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                var movieDropdownsData = await _service.GetNewProductsDropdownsValues();

                ViewBag.Suppliers = new SelectList(movieDropdownsData.Suppliers, "SupplierId", "SupplierName");
                var model = await _service.GetByIdAsync(Id);
                if (model == null)
                {
                    return View("Not Found");
                }
                ProductsViewModel supplier = new ProductsViewModel()
                {
                    ProductName = model.ProductName,
                    ProductPrice = model.ProductPrice,
                    CategoryName = model.CategoryName,
                    SupplierId = model.SupplierId,
                    Stock = model.Stock,
                    ExpiryDate = model.ExpiryDate,
                    MedicinalUse = model.MedicinalUse,
                    MedicineDesc = model.MedicineDesc,
                };

                return View(supplier);

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = "~/Image/" + UploadedFile(model);

                    Product prod = new Product()
                    {
                        ProductName = model.ProductName,
                        ProductPrice = model.ProductPrice,
                        CategoryName = model.CategoryName,
                        SupplierId = model.SupplierId,
                        Stock = model.Stock,
                        ExpiryDate = model.ExpiryDate,
                        MedicinalUse = model.MedicinalUse,
                        MedicineDesc = model.MedicineDesc,
                        ProductImage = uniqueFileName,
                    };

                    await _service.UpdateAsync(id, prod);
                    return RedirectToAction(nameof(Index));
                }
                return View();

            }
            catch (Exception)
            {

                throw;
            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            try
            {
                var allProd = await _service.GetAllAsync();

                if (!string.IsNullOrEmpty(searchString))
                {

                    var filteredResult = allProd.Where(n => n.ProductName.Contains(searchString) || n.MedicineDesc.Contains(searchString) || n.MedicinalUse.ToString().Contains(searchString) || n.CategoryName.ToString().Contains(searchString)).ToList();
                    //var filteredResultNew = allProd.Where(n => string.Equals(n.ProductName, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.MedicineDesc, searchString,
                    //    StringComparison.CurrentCultureIgnoreCase)||(string.Equals(n.CategoryName.ToString(), searchString, StringComparison.CurrentCultureIgnoreCase))).ToList();
                    return View("Index", filteredResult);

                }

                return View("Index", allProd);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var supplier = await _service.GetByIdAsync(Id);
                if (supplier == null)
                {
                    return View("NotFound");
                }
                return View(supplier);

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            try
            {
                var supplier = await _service.GetByIdAsync(Id);
                if (supplier == null)
                {
                    return View("NotFound");
                }
                await _service.DeleteAsync(Id);

                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {

                throw;
            }

        }

        private string UploadedFile(ProductsViewModel model)
        {
            try
            {
                string uniqueFileName = null;

                if (model.ProductImage != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Image");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProductImage.CopyTo(fileStream);
                    }
                }
                return uniqueFileName;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
