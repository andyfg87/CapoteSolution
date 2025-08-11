using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CapoteSolution.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class UsersController :Controller
    {
        private readonly IEntityRepository<User, Guid> _userRepo;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(
        IEntityRepository<User, Guid> userRepo,
        IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepo.GetAll().Result
                .Select(u => new UserDisplayVM
                {
                    Id = u.Id,
                    UserName = u.Username,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Role = u.Role.ToString()
                })
                .ToListAsync();
            

            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new UserInputVM
            {
                AvailableRoles = GetRoles()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserInputVM model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = GetRoles();
                return View(model);
            }

            var user = new User
            {
                Username = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserInputVM model)
        {
            var user = await _userRepo.GetAll().Result.FirstAsync(u => u.Username == model.UserName);
                                       

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Crear cookie de autenticación manual (ejemplo simplificado)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("Role", user.Role.ToString())  // Usa tu enum
    };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(Guid key)
        {
            var entity = await _userRepo.GetByIdAsync(key);
            if (entity == null)
                return NotFound();

            var viewModel = new UserInputVM();
            viewModel.AvailableRoles = GetRoles();
            viewModel.Import(entity);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserInputVM model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = GetRoles();
                return View(model);
            }

            var user = new User
            {
                Username = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private SelectList GetRoles()
        {
            return new SelectList(Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(r => new { Value = r, Text = r.ToString() }),
                "Value", "Text");
        }
    }
}
