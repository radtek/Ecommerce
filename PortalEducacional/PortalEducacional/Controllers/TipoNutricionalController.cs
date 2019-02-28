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
    [Permissao(Modulo = "Portal Educacional", Menu = "Nutricional", DescricaoTela = "Cadastro de Tipo Nutrição", Controller = "TipoNutricionalController", Acao = "Bloqueado;Editar")]
    public class TipoNutricionalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoNutricionalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoNutricional
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

            return View(await _context.TipoNutricional.ToListAsync());
        }

        // GET: TipoNutricional/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoNutricional = await _context.TipoNutricional
                .SingleOrDefaultAsync(m => m.TipoNutricionalID == id);
            if (tipoNutricional == null)
            {
                return NotFound();
            }

            return View(tipoNutricional);
        }

        // GET: TipoNutricional/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoNutricional/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoNutricionalID,Nome,Porcao,ValorDiario,UnidadeMedida")] TipoNutricional tipoNutricional)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoNutricional);
                await _context.SaveChangesAsync();

                Alert("Tipo Nutrição cadastrada com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoNutricional);
        }

        // GET: TipoNutricional/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoNutricional = await _context.TipoNutricional.SingleOrDefaultAsync(m => m.TipoNutricionalID == id);
            if (tipoNutricional == null)
            {
                return NotFound();
            }
            return View(tipoNutricional);
        }

        // POST: TipoNutricional/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int TipoNutricionalID, [Bind("TipoNutricionalID,Nome,Porcao,ValorDiario,UnidadeMedida")] TipoNutricional tipoNutricional)
        {
            if (TipoNutricionalID != tipoNutricional.TipoNutricionalID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoNutricional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoNutricionalExists(tipoNutricional.TipoNutricionalID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Alert("Tipo Nutrição atualizada com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoNutricional);
        }

        // GET: TipoNutricional/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoNutricional = await _context.TipoNutricional
                .SingleOrDefaultAsync(m => m.TipoNutricionalID == id);
            if (tipoNutricional == null)
            {
                return NotFound();
            }

            return View(tipoNutricional);
        }

        // POST: TipoNutricional/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int TipoNutricionalID)
        {
            var tipoNutricional = await _context.TipoNutricional.SingleOrDefaultAsync(m => m.TipoNutricionalID == TipoNutricionalID);
            _context.TipoNutricional.Remove(tipoNutricional);
            await _context.SaveChangesAsync();

            Alert("Tipo Nutrição deletada com Sucesso!", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        private bool TipoNutricionalExists(int id)
        {
            return _context.TipoNutricional.Any(e => e.TipoNutricionalID == id);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "TipoNutricionalController");
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
