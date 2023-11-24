using e_coomerce7.DataAccess.Data;
using e_coomerce7.DataAccess.Repository.IRepository;
using e_coomerce7.Models;
using e_coomerce7.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_coomerce7.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _UnitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name != null && obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name and Display order cannot be the same");
            }
            if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is invalid value");
            }
            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Add(obj);
                _UnitOfWork.Save();
                TempData["Success"] = "Category Created Succefully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }



        public IActionResult Edit(int? id)
        {
            if (id == 0 || id==null)
            {
                return NotFound();
            }
            Category? categotyFromDb = _UnitOfWork.Category.Get(u=>u.Id==id);
            if(categotyFromDb == null) { return NotFound(); }

            return View(categotyFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {

       
            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Update(obj);
                _UnitOfWork.Save();
                TempData["Success"] = "Category Updated Succefully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }





        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category? categotyFromDb = _UnitOfWork.Category.Get(U=>U.Id== id);
            if (categotyFromDb == null) { return NotFound(); }

            return View(categotyFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? categotyFromDb = _UnitOfWork.Category.Get(U => U.Id == id);
            if(categotyFromDb == null)
                return NotFound();

            _UnitOfWork.Category.Remove(categotyFromDb);
            _UnitOfWork.Save();
            TempData["Success"] = "Category Deleted Succefully";
            return RedirectToAction("Index", "Category");
        }
    }
}
