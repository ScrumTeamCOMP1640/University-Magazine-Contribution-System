using AspNetCore.Unobtrusive.Ajax;
using COMP1640.Interfaces;
using COMP1640.Models;
using COMP1640.Repository;
using COMP1640.Services;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUnobtrusiveAjax();

#pragma warning disable CS0618 // Type or member is obsolete
builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
{
    ProgressBar = true,
    Timeout = 5000
});
#pragma warning restore CS0618 // Type or member is obsolete

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("UmcsContext");
builder.Services.AddDbContext<UmcsContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddTransient<IEmail, EmailService>();

builder.Services.AddTransient<IFile ,FileService>();

builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();

builder.Services.AddScoped<ITermRepository, TermRepository>();

builder.Services.AddSession();

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
app.UseUnobtrusiveAjax();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.UseNToastNotify();
app.Run();
