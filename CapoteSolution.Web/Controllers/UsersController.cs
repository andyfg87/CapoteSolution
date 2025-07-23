using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CapoteSolution.Web.Controllers
{
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
                    Username = u.Username,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Role = u.Role.ToString()
                })
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
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
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            await _userRepo.AddAsync(user);
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
