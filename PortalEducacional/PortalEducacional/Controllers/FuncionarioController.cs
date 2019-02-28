using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalEducacional.Data;
using PortalEducacional.Models;
using Microsoft.AspNetCore.Authorization;
using static PortalEducacional.Enum.Enums;
using System.Threading;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Cadastros", DescricaoTela = "Cadastro de Funcionário", Controller = "FuncionarioController", Acao = "Bloqueado;Editar")]
    public class FuncionarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FuncionarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Funcionario
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

            var applicationDbContext = _context.Funcionario.Include(f => f.Cargo).Include(f => f.Escola);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Funcionario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionario
                .Include(f => f.Cargo)
                .Include(f => f.Escola)
                .SingleOrDefaultAsync(m => m.FuncionarioID == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // GET: Funcionario/Create
        public IActionResult Create()
        {
            ViewData["CargoId"] = new SelectList(_context.Cargo, "CargoID", "Nome");
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            return View();
        }

        // POST: Funcionario/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Funcionario funcionario)
        {
            if (!NomeFuncionarioExists(funcionario.Nome, funcionario.CargoId, false))
            {
                funcionario.Escola = CarregaEscola(funcionario);

                _context.Add(funcionario);
                await _context.SaveChangesAsync();

                Alert("Funcionário cadastrado com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }

            Alert("Já existe um funcionário com esse nome", NotificationType.error);

            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewData["CargoId"] = new SelectList(_context.Cargo, "CargoID", "Nome", funcionario.CargoId);

            return View(funcionario);
        }

        // GET: Funcionario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionario.SingleOrDefaultAsync(m => m.FuncionarioID == id);

            funcionario.Escola = CarregaEscola(funcionario);

            if (funcionario == null)
            {
                return NotFound();
            }

            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewData["CargoId"] = new SelectList(_context.Cargo, "CargoID", "Nome", funcionario.CargoId);

            return View(funcionario);
        }

        // POST: Funcionario/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int FuncionarioID, Funcionario funcionario)
        {
            if (FuncionarioID != funcionario.FuncionarioID)
            {
                return NotFound();
            }
            if (!NomeFuncionarioExists(funcionario.Nome, funcionario.CargoId, true, FuncionarioID))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(funcionario);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FuncionarioExists(funcionario.FuncionarioID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    Alert("Funcionário atualizado com Sucesso!", NotificationType.success);
                    return RedirectToAction(nameof(Index));
                }
            }

            Alert("Já existe um funcionário com esse nome", NotificationType.error);

            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewData["CargoId"] = new SelectList(_context.Cargo, "CargoID", "Nome", funcionario.CargoId);

            return View(funcionario);
        }

        // GET: Funcionario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionario
                .Include(f => f.Cargo)
                .Include(f => f.Escola)
                .SingleOrDefaultAsync(m => m.FuncionarioID == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // POST: Funcionario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int FuncionarioID)
        {
            var funcionario = await _context.Funcionario.SingleOrDefaultAsync(m => m.FuncionarioID == FuncionarioID);
            _context.Funcionario.Remove(funcionario);
            await _context.SaveChangesAsync();

            Alert("Funcionário deletado com Sucesso!", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionario.Any(e => e.FuncionarioID == id);
        }

        private bool NomeFuncionarioExists(string nome, int cargo, bool boEditar, int idFunc = 0)
        {
            if (!boEditar)
                return _context.Funcionario.Any(e => e.Nome == nome && e.CargoId == cargo);
            else
                return _context.Funcionario.Any(e => e.Nome == nome && e.CargoId == cargo && e.FuncionarioID != idFunc);
        }

        private Escola CarregaEscola(Funcionario func)
        {
            return _context.Escola.FirstOrDefault(x => x.EscolaID == func.EscolaId);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "FuncionarioController");
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
