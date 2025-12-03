using Microsoft.AspNetCore.Mvc;
using CasoPractico2.BLL.Servicios; 
using CasoPractico2.BLL.Dtos;

namespace CasoPractico2.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoServicio _productoServicio;

        public ProductosController(IProductoServicio productoServicio)
        {
            _productoServicio = productoServicio;
        }

        public async Task<IActionResult> Index()
        {
            var respuesta = await _productoServicio.ObtenerProductosAsync();
            if (respuesta.EsError) ViewBag.Error = respuesta.Mensaje;
            return View(respuesta.Data ?? new List<ProductoDto>());
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoDto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            var respuesta = await _productoServicio.CrearProductoAsync(producto);

            if (respuesta.EsError)
            {
                ViewBag.Error = respuesta.Mensaje;
                return View(producto);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var respuesta = await _productoServicio.ObtenerProductoPorIdAsync(id);
            if (respuesta.EsError) return RedirectToAction(nameof(Index));

            return View(respuesta.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProductoDto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            var respuesta = await _productoServicio.ActualizarProductoAsync(producto);

            if (respuesta.EsError)
            {
                ViewBag.Error = respuesta.Mensaje;
                return View(producto);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var respuesta = await _productoServicio.ObtenerProductoPorIdAsync(id);
            if (respuesta.EsError)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction(nameof(Index));
            }
            return View(respuesta.Data);
        }
    }
} 