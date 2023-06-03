using Domain.Entity;
using Infrastructure.Data;
using Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebBook.Resource;

namespace WebBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountsController : Controller
    {
        #region Decalaration
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly FreeBookDbContext _context;
        #endregion

        #region Constructor
        public AccountsController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, FreeBookDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        #endregion

        #region Method
        [Authorize(Roles = "SuperAdmin,Admin,User")]
        public IActionResult Roles()
        {
            var model = new RolesViewModel
            {
                NewRole = new NewRole(),
                Roles = _roleManager.Roles.OrderBy(x => x.Name).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= "Admin,SuperAdmin")]
        public async Task<IActionResult> Roles(RolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var role = new IdentityRole {
                //    Id = model.NewRole.RoleId,
                //    Name = model.NewRole.RoleName
                //};
                if (model.NewRole.RoleId == null) // Create
                {
                    //role.Id = Guid.NewGuid().ToString();
                    var result = await _roleManager.CreateAsync(new IdentityRole(model.NewRole.RoleName));
                    if (result.Succeeded)// Succeeded
                        SessionMsg(Helper.Success, ResourceWeb.lbSave, ResourceWeb.lbSaveMsgRole);
                    else // Not Successeded
                        SessionMsg(Helper.Error, ResourceWeb.lbNotSaved, ResourceWeb.lbNotSavedMsgRole);
                }//Update
                else
                {
                    var roleUpdate = await _roleManager.FindByIdAsync(model.NewRole.RoleId);
                    roleUpdate.Id = model.NewRole.RoleId;
                    roleUpdate.Name = model.NewRole.RoleName;
                    var result = await _roleManager.UpdateAsync(roleUpdate);
                    if(result.Succeeded)
                        SessionMsg(Helper.Success, ResourceWeb.lbUpdate, ResourceWeb.lbUpdatedMsgRole);
                    else  // Not Successeded
                        SessionMsg(Helper.Error, ResourceWeb.lbNotUpdated,ResourceWeb.lbNotUpdatedMsgRole);
                } 
            }
            return RedirectToAction("Roles");
        }

        private void SessionMsg(string MsgType, string Title, string Msg)
        {
            HttpContext.Session.SetString(Helper.MsgType, MsgType);
            HttpContext.Session.SetString(Helper.Title, Title);
            HttpContext.Session.SetString(Helper.Msg, Msg);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == Id);
            if ((await _roleManager.DeleteAsync(role)).Succeeded)
                return RedirectToAction(nameof(Roles));

            return RedirectToAction("Roles");
        }

        /// <summary>
        /// Register
        /// </summary>
        [Authorize(Roles = "SuperAdmin,Admin,User")]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                NewRegister = new NewRegister(),
                Roles = _roleManager.Roles.OrderBy(x => x.Name).ToList(),
                Users = _context.VwUsers.OrderBy(x => x.Role).ToList() //_userManager.Users.OrderBy(x=>x.Name).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files;
                if(file.Count > 0)
                {
                    string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(file[0].FileName);
                    var fileStream = new FileStream(Path.Combine(@"wwwroot/", Helper.PathSaveImageUser, ImageName), FileMode.Create);
                    file[0].CopyTo(fileStream);
                    model.NewRegister.ImageUser = ImageName;

                }
                else
                {
                    model.NewRegister.ImageUser = model.NewRegister.ImageUser;
                }
                var user = new ApplicationUser
                {
                    Id = model.NewRegister.Id,
                    Name = model.NewRegister.Name,
                    UserName = model.NewRegister.Email,
                    Email = model.NewRegister.Email,
                    ActiveUser = model.NewRegister.ActiveUser,
                    ImageUser = model.NewRegister.ImageUser
                };
                if (user.Id == null) 
                {
                    //Create
                    user.Id = Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(user,model.NewRegister.Password);
                    if (result.Succeeded)
                    {                        
                        var Role = await _userManager.AddToRoleAsync(user, model.NewRegister.RoleName);
                        if (Role.Succeeded)
                            SessionMsg(Helper.Success, ResourceWeb.lbSave, ResourceWeb.lbNotSavedMsgUserRole);
                        else
                            SessionMsg(Helper.Error, ResourceWeb.lbNotSaved, ResourceWeb.lbNotSavedMsgUser);
                    }
                    else
                        SessionMsg(Helper.Error, ResourceWeb.lbNotSaved, ResourceWeb.lbNotSavedMsgUser);
                }
                else
                {
                    //Update
                    var userUpdate = await _userManager.FindByIdAsync(user.Id);
                    userUpdate.Id = model.NewRegister.Id;
                    userUpdate.Name = model.NewRegister.Name;
                    userUpdate.UserName = model.NewRegister.Email;
                    userUpdate.Email = model.NewRegister.Email;
                    userUpdate.ActiveUser = model.NewRegister.ActiveUser;
                    userUpdate.ImageUser = model.NewRegister.ImageUser;
                    var result = await _userManager.UpdateAsync(userUpdate);
                    if (result.Succeeded)
                    {
                        var OldRole = await _userManager.GetRolesAsync(userUpdate);
                        await _userManager.RemoveFromRolesAsync(userUpdate, OldRole);
                        var addRole = await _userManager.AddToRoleAsync(userUpdate, model.NewRegister.RoleName);
                        if(addRole.Succeeded)
                            SessionMsg(Helper.Error, ResourceWeb.lbUpdate, ResourceWeb.lbUpdatedMsgRole);
                        else
                            SessionMsg(Helper.Error, ResourceWeb.lbNotUpdated, ResourceWeb.lbNotUpdateMsgUserRole);
                    }
                    else
                        SessionMsg(Helper.Error, ResourceWeb.lbNotUpdated, ResourceWeb.lbNotUpdateMsgUser);
                }
                return RedirectToAction("Register", "Accounts");
            }
            return RedirectToAction("Register","Accounts");
        }

        public async Task<IActionResult> DeleteUser(string userId)
        {
            var User = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            if (User.ImageUser != null && User.ImageUser != Guid.Empty.ToString()) {
                var PathImage = Path.Combine(@"wwwroot/",Helper.PathImageUser,User.ImageUser);
                if(System.IO.File.Exists(PathImage))
                {
                    System.IO.File.Delete(PathImage);
                }
            }
            if((await _userManager.DeleteAsync(User)).Succeeded)
            {
                return RedirectToAction("Register", "Accounts");
            }
            return RedirectToAction("Register", "Accounts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(RegisterViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.ChangePassword.Id);
            if( user != null)
            {
                await _userManager.RemovePasswordAsync(user);
                var AddNewPassword = await _userManager.AddPasswordAsync(user, model.ChangePassword.NewPassword);
                if (AddNewPassword.Succeeded)
                    SessionMsg(Helper.Error, ResourceWeb.lbSave, ResourceWeb.lbMsgSavedChangePassword);
                else
                    SessionMsg(Helper.Error, ResourceWeb.lbNotSaved, ResourceWeb.lbMsgNotSavedChangePassword);
                return RedirectToAction(nameof(Register));
            }
            return RedirectToAction(nameof(Register));
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                    ViewBag.ErrorLogin = false;
                
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LoginViewModel model)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


        #endregion
    }
}

