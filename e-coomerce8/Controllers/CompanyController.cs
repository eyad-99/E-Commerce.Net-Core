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
using Microsoft.AspNetCore.Authorization;

namespace e_coomerce7.Controllers


{
    [Authorize(Roles = Utility.SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        public CompanyController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
           
            List<Company> objCompanyList = _UnitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj = _UnitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }

        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {

                if (CompanyObj.Id == 0)
                {
                    _UnitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _UnitOfWork.Company.Update(CompanyObj);
                }

                _UnitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {

                return View(CompanyObj);
            }
        }





        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Company? categotyFromDb = _UnitOfWork.Company.Get(U=>U.Id== id);
            if (categotyFromDb == null) { return NotFound(); }

            return View(categotyFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Company? CompanyFromDb = _UnitOfWork.Company.Get(U => U.Id == id);
            if(CompanyFromDb == null)
                return NotFound();

            _UnitOfWork.Company.Remove(CompanyFromDb);
            _UnitOfWork.Save();
            TempData["Success"] = "Company Deleted Succefully";
            return RedirectToAction("Index", "Company");
        }
    }
}
