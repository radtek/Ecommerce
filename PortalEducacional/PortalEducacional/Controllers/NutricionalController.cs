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
    [Permissao(Modulo = "Portal Educacional", Menu = "Nutricional", DescricaoTela = "Avaliação Nutricional", Controller = "NutricionalController", Acao = "Bloqueado;Editar")]
    public class NutricionalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NutricionalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Nutricional
        public async Task<IActionResult> Index(int? idEscola, int? idSerie, string Aluno, string Sexo, string Ativo)
        {
            if (HttpContext.Request.Host.Host != "localhost")
            {
                if (!VerificaPermissaoTela())
                {
                    Alert("Você não possui acesso a essa Tela. Entre em contato com o seu Administrador!", NotificationType.error);
                    return Redirect(Url.Content("~/Home/Index"));
                }
            }

            var avaliacao = new List<Nutricional>();

            var listaId = new SelectList(_context.Escola, "EscolaID", "Nome", idEscola);
            ViewBag.Escola = listaId;

            var listaSerieId = new SelectList(_context.Serie, "SerieID", "Descricao", idSerie);
            ViewBag.Serie = listaSerieId;

            if (listaId.FirstOrDefault() == null)
            {
                Alert("Nenhuma Escola cadastrada, por favor, realize o cadastro!", NotificationType.warning);
                var contexto = _context.Nutricional.Include(n => n.Aluno);

                return View(await contexto.ToListAsync());
            }

            var identificador = idEscola ?? Convert.ToInt32(listaId.FirstOrDefault().Value);

            if (idEscola == null)
                avaliacao = _context.Nutricional.Include(n => n.Aluno).Where(x => x.Aluno.Escola.EscolaID == identificador).OrderByDescending(x => x.DataCadastro).ToList();
            else
                avaliacao = RetornaPesquisa(idEscola, idSerie, Aluno, Sexo, Ativo);

            return View(avaliacao);
        }

        // GET: Nutricional/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutricional = await _context.Nutricional
                .Include(n => n.Aluno)
                .SingleOrDefaultAsync(m => m.NutricionalID == id);
            if (nutricional == null)
            {
                return NotFound();
            }

            return View(nutricional);
        }

        // GET: Nutricional/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nutricional/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Nutricional nutricional)
        {
            try
            {
                nutricional.Aluno = CarregaAluno(nutricional.Aluno.AlunoID);

                _context.Add(nutricional);

                await _context.SaveChangesAsync();

                Alert("Avaliação Salva com Sucesso!", NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                Alert("Erro ao gravar a Avaliação. Entre em contato com o Suporte Técnico através do CHAT!", NotificationType.error);
                return View(nutricional);
            }
        }

        // GET: Nutricional/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutricional = await _context.Nutricional.SingleOrDefaultAsync(m => m.NutricionalID == id);

            nutricional.Aluno = CarregaAluno(nutricional.AlunoID);
            nutricional.Aluno.Escola = CarregaEscola(nutricional.Aluno.EscolaID);

            if (nutricional == null)
            {
                return NotFound();
            }

            return View(nutricional);
        }

        // POST: Nutricional/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Nutricional nutricional)
        {
            if (id != nutricional.NutricionalID)
            {
                return NotFound();
            }

            try
            {
                _context.Update(nutricional);
                await _context.SaveChangesAsync();

                Alert("Avaliação Salva com Sucesso!", NotificationType.success);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NutricionalExists(nutricional.NutricionalID))
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

        // GET: Nutricional/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutricional = await _context.Nutricional
                .Include(n => n.Aluno)
                .SingleOrDefaultAsync(m => m.NutricionalID == id);
            if (nutricional == null)
            {
                return NotFound();
            }

            return View(nutricional);
        }

        // POST: Nutricional/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int NutricionalID)
        {
            var nutricional = await _context.Nutricional.SingleOrDefaultAsync(m => m.NutricionalID == NutricionalID);
            _context.Nutricional.Remove(nutricional);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NutricionalExists(int id)
        {
            return _context.Nutricional.Any(e => e.NutricionalID == id);
        }

        private Aluno CarregaAluno(int alunoID)
        {
            return _context.Aluno.FirstOrDefault(x => x.AlunoID == alunoID);
        }

        private Escola CarregaEscola(int escolaID)
        {
            return _context.Escola.FirstOrDefault(x => x.EscolaID == escolaID);
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        public List<Nutricional> RetornaPesquisa(int? idEscola, int? idSerie, string Aluno, string Sexo, string ativo)
        {
            var genero = Sexo == "M" ? Genero.Masculino : Genero.Feminino;
            var NutricionalList = new List<Nutricional>();

            var status = true;
            if (ativo == null)
                status = true;
            else if (ativo != "0")
                status = bool.Parse(ativo);

            if (Aluno != null)
            {
                NutricionalList = (from N in _context.Nutricional.Include(a => a.Aluno).Include(e => e.Aluno.Escola).Include(s => s.Aluno.Serie)
                                   where N.Aluno.Escola.EscolaID == idEscola &&
                                                             N.Aluno.Nome.ToLower().Contains(Aluno.ToLower()) &&
                                                             (idSerie == 0 || N.Aluno.Serie.SerieID == idSerie) &&
                                                             N.Aluno.TipoGenero == genero &&
                                                             (ativo == "0" || N.Aluno.Ativo == status)
                                   select N).OrderByDescending(x => x.DataCadastro).ToList();
            }
            else
            {
                NutricionalList = (from N in _context.Nutricional.Include(a => a.Aluno).Include(e => e.Aluno.Escola).Include(s => s.Aluno.Serie)
                                   where N.Aluno.Escola.EscolaID == idEscola &&
                                                             (idSerie == 0 || N.Aluno.Serie.SerieID == idSerie) &&
                                                             N.Aluno.TipoGenero == genero &&
                                                             (ativo == "0" || N.Aluno.Ativo == status)
                                   select N).OrderByDescending(x => x.DataCadastro).ToList();
            }

            return NutricionalList;
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "NutricionalController");
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
