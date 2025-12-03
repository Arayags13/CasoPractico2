using CasoPractico2.BLL.Servicios;
using CasoPractico2.DLL.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using CasoPractico2.BLL.Mapeos;  

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IProductoRepositorio, ProductoRepositorio>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7203/");
});

builder.Services.AddHttpClient<IOrdenRepositorio, OrdenRepositorio>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7211/");
});

builder.Services.AddScoped<IProductoServicio, ProductoServicio>();
builder.Services.AddScoped<IOrdenServicio, OrdenServicio>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MapeoClases));

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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