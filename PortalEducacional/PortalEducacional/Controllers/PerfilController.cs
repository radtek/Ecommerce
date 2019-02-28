using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalEducacional.Data;
using PortalEducacional.Models;
using PortalEducacional.Extensions;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.WebControls;
using static PortalEducacional.Enum.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Segurança", DescricaoTela = "Cadastro de Perfil", Controller = "PerfilController", Acao = "Bloqueado;Editar")]
    public class PerfilController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PerfilController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Perfil
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

            var applicationDbContext = _context.Perfil.Include(p => p.Escola);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Perfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfil
                .Include(p => p.Escola)
                .SingleOrDefaultAsync(m => m.PerfilID == id);
            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }

        // GET: Perfil/Create
        public IActionResult Create()
        {
            var perfil = new Perfil();
            var list = new List<PerfilPermissoes>();

            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");

            var dt = Seguranca.RetornaPermissoesTelas();

            foreach (var item in dt.Select().ToList())
            {
                var perfilPermissoes = new PerfilPermissoes
                {
                    Controller = item[0].ToString(),
                    Menu = item[2].ToString(),
                    DescricaoTela = item[3].ToString(),
                    Permissao = item[4].ToString(),
                    Status = "BLOQUEADO"
                };

                list.Add(perfilPermissoes);
            }

            perfil.ListaTelas = list;

            return View(perfil);
        }

        // POST: Perfil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Perfil perfil)
        {
            try
            {
                perfil.Escola = _context.Escola.FirstOrDefault(e => e.EscolaID == perfil.EscolaID);

                _context.Add(perfil);
                await _context.SaveChangesAsync();

                foreach (var item in perfil.ListaTelas)
                {
                    var permissoes = new PerfilPermissoes
                    {
                        Controller = item.Controller,
                        Permissao = item.Status,
                        Perfil = perfil,
                        PerfilID = perfil.PerfilID
                    };

                    _context.Add(permissoes);
                    await _context.SaveChangesAsync();
                }

                Alert("Perfil cadastro com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                Alert("Erro ao cadastrar perfil, entre em contato com o Suporte Técnico!", NotificationType.error);
                ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
                return View(perfil);
            }
        }

        // GET: Perfil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var list = new List<PerfilPermissoes>();
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");

            var perfil = _context.Perfil.FirstOrDefault(m => m.PerfilID == id);
            var permissoes = _context.PerfilPermissoes.Where(x => x.PerfilID == perfil.PerfilID).ToList();

            var dt = Seguranca.RetornaPermissoesTelas();

            foreach (var item in dt.Select().ToList())
            {
                var perfilPermissoes = new PerfilPermissoes
                {
                    Controller = item[0].ToString(),
                    Menu = item[2].ToString(),
                    DescricaoTela = item[3].ToString(),
                    Permissao = item[4].ToString(),
                    Status = "BLOQUEADO"
                };

                foreach (var item2 in permissoes)
                {
                    if (item2.Controller == perfilPermissoes.Controller)
                    {
                        perfilPermissoes.Status = item2.Permissao;
                        perfilPermissoes.PerfilPermissoesID = item2.PerfilPermissoesID;
                        break;
                    }
                }

                list.Add(perfilPermissoes);
            }

            perfil.ListaTelas = list;

            return View(perfil);
        }

        // POST: Perfil/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int PerfilID, Perfil perfil)
        {
            if (PerfilID != perfil.PerfilID)
            {
                return NotFound();
            }

            try
            {
                _context.Update(perfil);
                await _context.SaveChangesAsync();

                foreach (var item in perfil.ListaTelas)
                {
                    var permissoes = new PerfilPermissoes
                    {
                        Controller = item.Controller,
                        Perfil = perfil,
                        PerfilID = perfil.PerfilID,
                        PerfilPermissoesID = item.PerfilPermissoesID,
                        Permissao = item.Status
                    };

                    _context.Update(permissoes);
                    await _context.SaveChangesAsync();
                }

                Alert("Perfil atualizado com Sucesso!", NotificationType.success);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerfilExists(perfil.PerfilID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Perfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfil
                .Include(p => p.Escola)
                .SingleOrDefaultAsync(m => m.PerfilID == id);
            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }

        // POST: Perfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PerfilID)
        {
            var perfil = await _context.Perfil.SingleOrDefaultAsync(m => m.PerfilID == PerfilID);

            var usuario = _context.Usuario.FirstOrDefault(x => x.PerfilID == PerfilID);

            if (usuario != null)
            {
                Alert("Existe Usuários vinculados com esse Perfil, não possível deleta lo.", NotificationType.error);
                return View(perfil);
            }

            _context.Perfil.Remove(perfil);
            await _context.SaveChangesAsync();

            Alert("Perfil deletado com Sucesso!", NotificationType.success);

            return RedirectToAction(nameof(Index));
        }

        private bool PerfilExists(int id)
        {
            return _context.Perfil.Any(e => e.PerfilID == id);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "PerfilController");
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
