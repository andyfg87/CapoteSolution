using CapoteSolution.Models.EF;
using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Controllers;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Configuración de repositorios
builder.Services.AddScoped<IEntityRepository<Toner, Guid>, EntityRepository<Toner, Guid>>();
builder.Services.AddScoped<IEntityRepository<Brand, Guid>, EntityRepository<Brand, Guid>>();
builder.Services.AddScoped<IEntityRepository<MachineModel, Guid>, EntityRepository<MachineModel, Guid>>();
builder.Services.AddScoped<IEntityRepository<Copier, string>, EntityRepository<Copier, string>>();
builder.Services.AddScoped<IEntityRepository<Contract, Guid>, EntityRepository<Contract, Guid>>();
builder.Services.AddScoped<IEntityRepository<Service, Guid>, EntityRepository<Service, Guid>>();
builder.Services.AddScoped<IEntityRepository<ServiceReason, byte>, EntityRepository<ServiceReason, byte>>();
builder.Services.AddScoped<IEntityRepository<User, Guid>, EntityRepository<User, Guid>>();
builder.Services.AddScoped<IEntityRepository<Customer, Guid>, EntityRepository<Customer, Guid>>();
builder.Services.AddScoped<ContractRepository>();

// Configuración de controladores
builder.Services.AddScoped<TonersController>();
builder.Services.AddScoped<BrandsController>();
builder.Services.AddScoped<MachineModelsController>();
builder.Services.AddScoped<CopiersController>();
builder.Services.AddScoped<ContractsController>();
builder.Services.AddScoped<ServicesController>();
builder.Services.AddScoped<ServiceReasonsController>();
builder.Services.AddScoped<UsersController>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("Role", UserRole.Admin.ToString()));

    options.AddPolicy("Technician", policy =>
        policy.RequireClaim("Role", UserRole.Technician.ToString()));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configuración de ruta por defecto con redirección condicional
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Middleware para redirigir usuarios no autenticados
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.StartsWithSegments("/Account") &&
        !context.Request.Path.StartsWithSegments("/Home") &&
        context.Request.Path == "/")
    {
        context.Response.Redirect("/Account/Login");
        return;
    }

    await next();
});

app.Run();