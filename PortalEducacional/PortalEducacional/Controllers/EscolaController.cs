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
using PortalEducacional.Extensions;
using System.Web;
using static PortalEducacional.Enum.Enums;
using Newtonsoft.Json;
using System.Threading;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Cadastros", DescricaoTela = "Cadastro de Escola", Controller = "EscolaController", Acao = "Bloqueado;Editar")]
    public class EscolaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EscolaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Escola
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

            var applicationDbContext = _context.Escola.Include(e => e.Endereco.Cidade);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Escola/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escola = await _context.Escola
                .Include(e => e.Endereco)
                .SingleOrDefaultAsync(m => m.EscolaID == id);
            if (escola == null)
            {
                return NotFound();
            }

            return View(escola);
        }

        // GET: Escola/Create
        public IActionResult Create()
        {
            try
            {
                var escola = new Escola();

                CarregaEndereco(escola);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            ViewData["EscolaID"] = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewData["EnderecoId"] = new SelectList(_context.Set<Endereco>(), "EnderecoId", "CEP");
            return View();
        }

        // POST: Escola/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Escola escola)
        {
            try
            {
                var CNPJ = escola.CNPJ.ApenasNumero();
                escola.CNPJ = CNPJ.Trim();
                var telefone = escola.Telefone.ApenasNumero();
                escola.Telefone = telefone.Replace(" ", "");

                escola.Endereco.Cidade = RetornaCidade(escola.Endereco.Cidade.CidadeId);
                CarregaEndereco(escola);

                if (VerificaEscola(escola))
                {
                    Alert(string.Format("CNPJ {0} já cadastrada no sistema!", escola.CNPJ), NotificationType.info);
                    return View(escola);
                }

                _context.Add(escola);
                await _context.SaveChangesAsync();

                Alert("Escola Salva com sucesso!", NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                Alert("Erro ao gravar a Escola. Entre em contato com o Suporte Técnico através do CHAT!", NotificationType.error);
                return View(escola);
            }
        }

        // GET: Escola/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escola = await _context.Escola.SingleOrDefaultAsync(m => m.EscolaID == id);
            if (escola == null)
            {
                return NotFound();
            }
            escola.Endereco = _context.Endereco.FirstOrDefault(x => x.EnderecoId == escola.EnderecoId);

            escola.Endereco.Cidade = _context.Cidade.FirstOrDefault(x => x.CidadeId == escola.Endereco.CidadeId);

            escola.Endereco.Cidade.Estado = _context.Estado.FirstOrDefault(x => x.EstadoId == escola.Endereco.Cidade.EstadoId);

            CarregaEndereco(escola);

            return View(escola);
        }

        // POST: Escola/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int EscolaID, Escola escola)
        {
            if (EscolaID != escola.EscolaID)
            {
                return NotFound();
            }
            var iEstado = escola.Endereco.Cidade.Estado.EstadoId;
            escola.Endereco.Cidade = RetornaCidade(escola.Endereco.Cidade.CidadeId);
            escola.Endereco.Cidade.Estado = RetornaEstado(iEstado);

            CarregaEndereco(escola);

            try
            {
                var CNPJ = escola.CNPJ.ApenasNumero();
                escola.CNPJ = CNPJ.Trim();
                var telefone = escola.Telefone.ApenasNumero();
                escola.Telefone = telefone;

                _context.Update(escola);
                await _context.SaveChangesAsync();

                Alert("Escola Salva com sucesso!", NotificationType.success);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EscolaExists(escola.EscolaID))
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

        // GET: Escola/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var escola = await _context.Escola
                .Include(e => e.Endereco)
                .SingleOrDefaultAsync(m => m.EscolaID == id);
            if (escola == null)
            {
                return NotFound();
            }

            return View(escola);
        }

        // POST: Escola/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int EscolaID)
        {
            var escola = await _context.Escola.SingleOrDefaultAsync(m => m.EscolaID == EscolaID);

            var aluno = _context.Aluno.FirstOrDefault(a => a.EscolaID == EscolaID);

            if (aluno != null)
            {
                Alert("A Escola não pode ser excluida, pois há ALUNOS vinculado a ela.", NotificationType.error);
                return View(escola);
            }

            var produto = _context.Produto.FirstOrDefault(p => p.EscolaID == EscolaID);

            if (produto != null)
            {
                Alert("A Escola não pode ser excluida, pois há PRODUTOS vinculado a ela.", NotificationType.error);
                return View(escola);
            }

            var funcionario = _context.Funcionario.FirstOrDefault(f => f.EscolaId == EscolaID);

            if (funcionario != null)
            {
                Alert("A Escola não pode ser excluida, pois há FUNCIONÁRIO vinculado a ela.", NotificationType.error);
                return View(escola);
            }

            _context.Escola.Remove(escola);
            await _context.SaveChangesAsync();

            Alert("Escola removida com sucesso!", NotificationType.success);

            return RedirectToAction(nameof(Index));
        }

        private bool EscolaExists(int id)
        {
            return _context.Escola.Any(e => e.EscolaID == id);
        }

        private bool VerificaEscola(Escola escola)
        {
            if (escola == null) return false;

            var existe = _context.Escola.FirstOrDefault(x => x.CNPJ == escola.CNPJ.ApenasNumero());

            return existe == null ? false : true;
        }

        private void CarregaEndereco(Escola escola)
        {
            List<Endereco> enderecoList = new List<Endereco>();
            enderecoList = _context.Endereco.ToList();

            if (escola.Endereco == null || escola.Endereco.Cidade == null)
            {
                ViewBag.Estado = new SelectList(_context.Set<Estado>().OrderBy(x => x.Nome), "EstadoId", "Nome");
                ViewBag.Cidade = new SelectList(_context.Set<Cidade>().OrderBy(x => x.Nome), "CidadeId", "Nome");
            }
            else
            {
                var cidade = RetornaCidade(escola.Endereco.Cidade.CidadeId);
                var estado = RetornaEstado(escola.Endereco.Cidade.EstadoId);
                ViewBag.Estado = new SelectList(_context.Set<Estado>().OrderBy(x => x.Nome), "EstadoId", "Nome", estado.EstadoId);
                ViewBag.Cidade = new SelectList(_context.Set<Cidade>().OrderBy(x => x.Nome), "CidadeId", "Nome", cidade.CidadeId);
            }
        }

        private Cidade RetornaCidade(int id)
        {
            return _context.Cidade.FirstOrDefault(x => x.CidadeId == id);
        }

        private Estado RetornaEstado(int id)
        {
            return _context.Estado.FirstOrDefault(x => x.EstadoId == id);
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        [HttpPost]
        public JsonResult AutoComplete(string dado)
        {
            var settings = new JsonSerializerSettings();
            var ObjList = new List<Escola>();

            ObjList = _context.Escola.ToList();

            var EscolaList = (from N in ObjList
                              where N.CNPJ.ToLower().Contains(dado.ToLower()) || N.Nome.ToLower().Contains(dado.ToLower())
                              select new { N.EscolaID, N.CNPJ, N.Nome });

            return Json(EscolaList, settings);
        }

        public ActionResult ListaCidadeporEstado(string estadoId) => Json(_context.Cidade.Where(x => x.Estado.EstadoId == int.Parse(estadoId)));

        private bool VerificaPermissaoTela()
        {
            var liberado = false;

            var idUsuario = User.Claims.Select(u => u.Value).FirstOrDefault().ToString();

            var usuario = _context.Usuario.FirstOrDefault(x => x.Id == idUsuario);

            if (usuario != null)
            {
                if (!usuario.Master)
                {
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "EscolaController");
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