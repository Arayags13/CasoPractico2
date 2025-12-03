using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CasoPractico2.BLL.Dtos;
using CasoPractico2.BLL.Servicios;

namespace CasoPractico2.Controllers
{
    public class OrdenesController : Controller
    {
        private readonly IOrdenServicio _ordenServicio;
        private readonly IProductoServicio _productoServicio;

        public OrdenesController(IOrdenServicio ordenServicio, IProductoServicio productoServicio)
        {
            _ordenServicio = ordenServicio;
            _productoServicio = productoServicio;
        }

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

        public async Task<IActionResult> Crear()
        {
            await CargarListaProductos();
            return View();
        }

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
            }
            else
            {
                ViewBag.Productos = new SelectList(new List<ProductoDto>(), "Id", "Nombre");
            }
        }
    }
}