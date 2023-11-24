using e_coomerce7.DataAccess.Data;
using e_coomerce7.DataAccess.Repository.IRepository;
using e_coomerce7.Models;
using e_coomerce7.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_coomerce8.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var users = _db.Users.ToList();



            foreach (var user in objUserList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                user.lockstatus = "false";
               var lockdate = users.FirstOrDefault(u => u.Id == user.Id).LockoutEnd;
          //      DateTimeOffset? lockdate = (DateTimeOffset)users.FirstOrDefault(u => u.Id == user.Id).LockoutEnd;

                if ( lockdate==null)
                    user.lockstatus = "unLocked";
                else
                   user.lockstatus = "Locked";
                


                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return View(objUserList);
        }



        public IActionResult LockUnlock(string Id)
        {

            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
            if (objFromDb == null)
            {
                return RedirectToAction("Index", "User");
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = null;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return RedirectToAction("Index", "User");
        }



        #region API CALLS




        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
