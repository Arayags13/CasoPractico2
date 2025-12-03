using CasoPractico2.BLL.Dtos;
using CasoPractico2.BLL.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcInventarios.Controllers
{
    public class OrdenesController : Controller
    {
        private readonly IOrdenServicio _ordenServicio;
        private readonly IProductoServicio _productoServicio; // Necesario para el dropdown de productos

        public OrdenesController(IOrdenServicio ordenServicio, IProductoServicio productoServicio)
        {
            _ordenServicio = ordenServicio;
            _productoServicio = productoServicio;
        }

        // GET: Ordenes/Index
        public async Task<IActionResult> Index()
        {
            var respuesta = await _ordenServicio.ObtenerOrdenesAsync();

            if (respuesta.EsError)
            {
                ViewBag.Error = respuesta.Mensaje;
                return View(new List<OrdenDto>());
            }

            return View(respuesta.Data);
        }

        // GET: Ordenes/Detalles/5
        public async Task<IActionResult> Detalles(int id)
        {
            var respuesta = await _ordenServicio.ObtenerOrdenPorIdAsync(id);

            if (respuesta.EsError)
            {
                TempData["Error"] = respuesta.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            return View(respuesta.Data);
        }

        // GET: Ordenes/Crear
        public async Task<IActionResult> Crear()
        {
            await CargarListaProductos();
            return View();
        }

        // POST: Ordenes/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(OrdenDto orden)
        {

            if (orden.Detalles == null || !orden.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la orden.");
            }

            if (ModelState.IsValid)
            {
                var respuesta = await _ordenServicio.CrearOrdenAsync(orden);

                if (!respuesta.EsError)
                {
                    TempData["Mensaje"] = "Orden creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = respuesta.Mensaje;
                }
            }

            await CargarListaProductos();
            return View(orden);
        }

        // POST: Ordenes/Cancelar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var respuesta = await _ordenServicio.CancelarOrdenAsync(id);

            if (respuesta.EsError)
            {
                TempData["Error"] = respuesta.Mensaje;
            }
            else
            {
                TempData["Mensaje"] = "Orden cancelada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListaProductos()
        {
            var respuesta = await _productoServicio.ObtenerProductosAsync();
            if (!respuesta.EsError)
            {
                ViewBag.Productos = new SelectList(respuesta.Data, "Id", "Nombre");
                ViewBag.ProductosData = respuesta.Data;
            }
            else
            {
                ViewBag.Productos = new SelectList(new List<ProductoDto>(), "Id", "Nombre");
            }
        }
    }
}