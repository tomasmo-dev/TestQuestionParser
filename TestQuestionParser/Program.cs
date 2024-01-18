using Microsoft.EntityFrameworkCore;
using TestQuestionParser.Data;
using TestQuestionParser.Services;

var builder = WebApplication.CreateBuilder(args);

//var conStr = builder.Configuration.GetConnectionString("DefaultConnection"); ----------------------------------- !!!!!!!!!!!! USE THIS FOR LOCAL TESTING AND EF SCAFFOLDING
var conStr = Environment.GetEnvironmentVariable("CONNECTION_STRING"); // PROD SAFE
if (conStr == null )
{
    throw new Exception("Connection string not found!");
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SystemContext>(
    options =>
        options.UseMySql(conStr, ServerVersion.AutoDetect(conStr)));

builder.Services.AddScoped<FileService>();

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