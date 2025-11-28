using Alertify.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Alertify.Controllers
{
    public class ConnectionController : Controller
    {
        private readonly AlertifyDbContext _context;

        public ConnectionController(AlertifyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.ContextStatus = _context != null ? "? Contexto inicializado" : "? Contexto es nulo";
                var canConnect = await _context.Database.CanConnectAsync();
                ViewBag.ConnectionStatus = canConnect ? "? Conexión exitosa" : "? No se puede conectar";
                ViewBag.DatabaseName = _context.Database.GetDbConnection().Database;
                ViewBag.EdificiosCount = await _context.Users.CountAsync();
                ViewBag.MateriasCount = await _context.Stations.CountAsync();
                ViewBag.PersonasCount = await _context.Units.CountAsync();
                ViewBag.UsuariosCount = await _context.Emergencies.CountAsync();
                ViewBag.AulasCount = await _context.Notifications.CountAsync();

                ViewBag.TestStatus = "? TODAS LAS PRUEBAS PASARON";
            }
            catch (Exception ex)
            {
                ViewBag.TestStatus = "? ERROR EN LA CONEXIÓN";
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.ErrorDetails = ex.InnerException?.Message ?? "No hay detalles adicionales";
            }

            return View();
        }

        public async Task<IActionResult> TestConnection()
        {
            var result = new
            {
                success = false,
                message = "",
                details = new Dictionary<string, object>()
            };

            try
            {
                // Verificar conexión
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    result = new
                    {
                        success = true,
                        message = "Conexión exitosa a la base de datos",
                        details = new Dictionary<string, object>
                        {
                            { "database", _context.Database.GetDbConnection().Database },
                            { "Users", await _context.Users.CountAsync() },
                            { "stations", await _context.Stations.CountAsync() },
                            { "units", await _context.Units.CountAsync() },
                            { "emergencia", await _context.Emergencies.CountAsync() },
                            { "notifications", await _context.Notifications.CountAsync() }
                        }
                    };
                }
                else
                {
                    result = new
                    {
                        success = false,
                        message = "No se pudo conectar a la base de datos",
                        details = new Dictionary<string, object>()
                    };
                }
            }
            catch (Exception ex)
            {
                result = new
                {
                    success = false,
                    message = ex.Message,
                    details = new Dictionary<string, object>
                    {
                        { "innerException", ex.InnerException?.Message ?? "No hay detalles" },
                        { "stackTrace", ex.StackTrace ?? "" }
                    }
                };
            }

            return Json(result);
        }

    }
}
