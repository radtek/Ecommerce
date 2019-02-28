using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalEducacional.Data;
using PortalEducacional.Models;
using static PortalEducacional.Enum.Enums;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Cadastros", DescricaoTela = "Cadastro de Série", Controller = "SerieController", Acao = "Bloqueado;Editar")]
    public class SerieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SerieController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Serie
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Request.Host.Host != "localhost")
            {
                if (!VerificaPermissaoTela())
                {
                    Alert("Você não possui acesso a essa Tela. Entre em contato com o seu Administrador!", NotificationType.error);
                    return Redirect(Url.Content("~/Home/Index"));
                }
            }

            return View(await _context.Serie.ToListAsync());
        }

        // GET: Serie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serie = await _context.Serie
                .SingleOrDefaultAsync(m => m.SerieID == id);
            if (serie == null)
            {
                return NotFound();
            }

            return View(serie);
        }

        // GET: Serie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Serie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SerieID,Descricao")] Serie serie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serie);
                await _context.SaveChangesAsync();

                Alert("Série cadastrada com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            return View(serie);
        }

        // GET: Serie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serie = await _context.Serie.SingleOrDefaultAsync(m => m.SerieID == id);
            if (serie == null)
            {
                return NotFound();
            }
            return View(serie);
        }

        // POST: Serie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int SerieID, [Bind("SerieID,Descricao")] Serie serie)
        {
            if (SerieID != serie.SerieID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SerieExists(serie.SerieID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Alert("Série atualizada com Sucesso!", NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            return View(serie);
        }

        // GET: Serie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serie = await _context.Serie
                .SingleOrDefaultAsync(m => m.SerieID == id);
            if (serie == null)
            {
                return NotFound();
            }

            return View(serie);
        }

        // POST: Serie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int SerieID)
        {
            var serie = await _context.Serie.SingleOrDefaultAsync(m => m.SerieID == SerieID);
            _context.Serie.Remove(serie);
            await _context.SaveChangesAsync();

            Alert("Série deletada com Sucesso!", NotificationType.success);

            return RedirectToAction(nameof(Index));
        }

        private bool SerieExists(int id)
        {
            return _context.Serie.Any(e => e.SerieID == id);
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        private bool VerificaPermissaoTela()
        {
            var liberado = false;

            var idUsuario = User.Claims.Select(u => u.Value).FirstOrDefault().ToString();

            var usuario = _context.Usuario.FirstOrDefault(x => x.Id == idUsuario);

            if (usuario != null)
            {
                if (!usuario.Master)
                {
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "SerieController");
                    if (acesso == null)
                        liberado = false;
                    else if (acesso.Permissao == "EDITAR")
                        liberado = true;
                }

                liberado = true;
            }

            return liberado;
        }

        public ActionResult UpdateResource()
        {
            Thread.Sleep(5000);
            return Content(string.Format("Current time is {0}", DateTime.Now.ToShortTimeString()));
        }
    }
}
