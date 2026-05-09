using TransferApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
