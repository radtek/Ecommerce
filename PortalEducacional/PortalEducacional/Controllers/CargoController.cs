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
    [Permissao(Modulo = "Portal Educacional", Menu = "Cadastros", DescricaoTela = "Cadastro de Cargo", Controller = "CargoController", Acao = "Bloqueado;Editar")]
    public class CargoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CargoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cargo
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

            return View(await _context.Cargo.ToListAsync());
        }

        // GET: Cargo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargo
                .SingleOrDefaultAsync(m => m.CargoID == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // GET: Cargo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cargo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CargoID,Nome,Ativo")] Cargo cargo)
        {
            if (!DescricaoCargoExists(cargo.Nome))
            {
                if (ModelState.IsValid)
                {
                    _context.Add(cargo);
                    await _context.SaveChangesAsync();

                    Alert("Cargo salvo com sucesso!", NotificationType.success);

                    return RedirectToAction(nameof(Index));
                }
            }

            Alert("Já existe um cargo com esse nome!", NotificationType.error);
            return View(cargo);
        }

        // GET: Cargo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargo.SingleOrDefaultAsync(m => m.CargoID == id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        // POST: Cargo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int CargoID, [Bind("CargoID,Nome,Ativo")] Cargo cargo)
        {
            if (CargoID != cargo.CargoID)
            {
                return NotFound();
            }

            if (!DescricaoCargoExists(cargo.Nome))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(cargo);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CargoExists(cargo.CargoID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    Alert("Cargo salvo com sucesso!", NotificationType.success);

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(cargo);
        }

        // GET: Cargo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargo
                .SingleOrDefaultAsync(m => m.CargoID == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: Cargo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CargoID)
        {
            var funcionario = _context.Funcionario.FirstOrDefault(f => f.CargoId == CargoID);

            var cargo = await _context.Cargo.SingleOrDefaultAsync(m => m.CargoID == CargoID);

            if (funcionario != null)
            {
                Alert("Já existe um funcionário vinculado a esse cargo!", NotificationType.error);
                return View(cargo);
            }

            _context.Cargo.Remove(cargo);
            await _context.SaveChangesAsync();

            Alert("Cargo removido com sucesso!", NotificationType.success);

            return RedirectToAction(nameof(Index));
        }

        private bool CargoExists(int id)
        {
            return _context.Cargo.Any(e => e.CargoID == id);
        }

        private bool DescricaoCargoExists(string nome)
        {
            return _context.Cargo.Any(e => e.Nome == nome);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "CargoController");
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
