using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using TransferApp.Data;
using TransferApp.Repositories;
using TransferApp.Repositories.Interfaces;
using TransferApp.Security;
using TransferApp.Services;

var builder = WebApplication.CreateBuilder(args);

//autenticacion

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "TransferApp.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

//autenticacion



// Add services to the container.
//builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews(options =>
    {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    }).AddRazorRuntimeCompilation();
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddSingleton<ApplicationDbContext>();
builder.Services.AddScoped<CompanyRepository>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<CountryRepository>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<TrainingsLicensesRepository>();
builder.Services.AddScoped<TrainingsLicensesService>();
builder.Services.AddScoped<ReviewsRepository>();
builder.Services.AddScoped<ReviewsService>();
builder.Services.AddScoped<ClientsRepository>();
builder.Services.AddScoped<ClientsService>();
builder.Services.AddScoped<DocumentsTypesRepository>();
builder.Services.AddScoped<DocumentsTypesService>();

builder.Services.AddScoped<BeneficiariesRepository>();
builder.Services.AddScoped<BeneficiariesService>();
builder.Services.AddScoped<TransactionsTypesRepository>();
builder.Services.AddScoped<TransactionsTypesService>();
builder.Services.AddScoped<TransactionsRepository>();
builder.Services.AddScoped<TransactionsService>();
builder.Services.AddScoped<ParametersRepository>();
builder.Services.AddScoped<ParametersService>();
builder.Services.AddScoped<TransactionAttachmentRepository>();
builder.Services.AddScoped<TransactionAttachmentService>();
builder.Services.AddScoped<ReferenceNumberRepository>();
builder.Services.AddScoped<ReferenceNumberService>();
builder.Services.AddScoped<ClientCompaniesRepository>();
builder.Services.AddScoped<ClientCompaniesService>();
builder.Services.AddScoped<ReportsRepository>();
builder.Services.AddScoped<ReportsService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddSingleton<PasswordService>();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
