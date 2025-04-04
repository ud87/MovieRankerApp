﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Ranker.Models;

namespace Movie_Ranker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //constructor - Injects UserManager & SignInManager services
        public AccountController(UserManager<IdentityUser> userManager, //UserManager and SignInManager are the builtin class that provides APIs for managing user and user sign in respectively
                                 SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //Show Registration Form
        [HttpGet]
        public IActionResult Register() //when user navigates to /Account/Register this method is called
        {
            return View();
        }

        //Handle Registration Form Submission
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //create new user
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                //Creates new user account in ASP.NET identity
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded) //if registration is successful
                {
                    //logs user immediately after successful registration
                    await _signInManager.SignInAsync(user, isPersistent: false); //isPersistent:false for session cookie only lasts on window
                    TempData["success"] = "Sucessfully registered";
                    return RedirectToAction("index", "home");
                }

                //will show error if there is sign in error 
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //Show login form
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false); //lockoutonFailure: users wont be locked out after failed attempts

                if (result.Succeeded)
                {
                    TempData["success"] = "Sucessfully signed in";
                    return RedirectToAction("Index", "Movie");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Signed out";
            return RedirectToAction("Index", "Home");
        }


    }
}
