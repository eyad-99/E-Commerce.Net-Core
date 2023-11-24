using e_coomerce7.DataAccess.Data;
using e_coomerce7.DataAccess.Repository;
using e_coomerce7.DataAccess.Repository.IRepository;
using e_coomerce7.Models;
using e_coomerce7.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace e_coomerce7.Controllers


{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = UnitOfWork;
            _WebHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
           
            List<Product> objProductList = _UnitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)  //work for insert and update
        {
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category.GetAll().
               Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString(),
               });
           // ViewBag.CategoryList = CategoryList;
           ProductVM productVM = new ProductVM()
           {
               CategoryList =CategoryList,
               Product = new Product()
           };
            if(id==null || id==0)
            {
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _UnitOfWork.Product.Get(u => u.Id==id);
                return View(productVM);


            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _WebHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _UnitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _UnitOfWork.Product.Update(productVM.Product);
                }

                _UnitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList =_UnitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }






        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Product? categotyFromDb = _UnitOfWork.Product.Get(U=>U.Id== id);
            if (categotyFromDb == null) { return NotFound(); }

            return View(categotyFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? productFromDb = _UnitOfWork.Product.Get(U => U.Id == id);
            if(productFromDb == null)
                return NotFound();

            _UnitOfWork.Product.Remove(productFromDb);
            _UnitOfWork.Save();
            TempData["Success"] = "Product Deleted Succefully";
            return RedirectToAction("Index", "Product");
        }
    }
}
