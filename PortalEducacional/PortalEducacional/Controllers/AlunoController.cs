using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalEducacional.Data;
using PortalEducacional.Models;
using Microsoft.AspNetCore.Authorization;
using PortalEducacional.Models.ViewModel;
using static PortalEducacional.Enum.Enums;
using PortalEducacional.Extensions;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Identity;
using PortalEducacional.Services;

namespace PortalEducacional.Controllers
{
    [Permissao(Modulo = "Portal Educacional", Menu = "Cadastros", DescricaoTela = "Cadastro de Aluno", Controller = "AlunoController", Acao = "Bloqueado;Editar")]
    [Authorize]
    [Route("[controller]/[action]")]
    public class AlunoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AlunoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Aluno
        public async Task<IActionResult> Index(int? idEscola, int? idSerie, string Aluno, string Sexo, string Ativo, bool boPesquisar)
        {
            if (HttpContext.Request.Host.Host != "localhost")
            {
                if (!VerificaPermissaoTela())
                {
                    Alert("Você não possui acesso a essa Tela. Entre em contato com o seu Administrador!", NotificationType.error);
                    return Redirect(Url.Content("~/Home/Index"));
                }
            }

            var user = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var alunos = new List<Aluno>();

            var listaEscolaId = new SelectList(_context.Escola, "EscolaID", "Nome", idEscola == null ? usuario.EscolaID : idEscola);
            ViewBag.Escola = listaEscolaId;

            var listaSerieId = new SelectList(_context.Serie, "SerieID", "Descricao", idSerie);
            ViewBag.Serie = listaSerieId;

            if (!usuario.Master)
            {
                if (usuario.EscolaID > 0)
                {
                    listaEscolaId = new SelectList(_context.Escola.Where(x => x.EscolaID == usuario.EscolaID), "EscolaID", "Nome", idEscola);
                    ViewBag.Escola = listaEscolaId;
                }

                idEscola = usuario.EscolaID;
            }

            if (listaEscolaId.FirstOrDefault() == null)
            {
                Alert("Nenhuma Escola cadastrada, por favor, realize o cadastro!", NotificationType.warning);
                var contexto = _context.Aluno.Include(e => e.Escola).Include(s => s.Serie);

                return View(await contexto.ToListAsync());
            }

            var identificador = idEscola ?? Convert.ToInt32(listaEscolaId.FirstOrDefault().Value);

            if (boPesquisar)
            {
                if (idEscola == null)
                    alunos = _context.Aluno.Include(a => a.Escola).Where(a => a.Escola.EscolaID == identificador).Include(s => s.Serie).ToList();
                else
                    alunos = RetornaPesquisa(idEscola, idSerie, Aluno, Sexo, Ativo);
            }

            ViewBag.Contador = alunos.Count();
            return View(alunos);
        }

        // GET: Aluno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno
                .Include(a => a.Escola)
                .SingleOrDefaultAsync(m => m.AlunoID == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // GET: Aluno/Create
        public IActionResult Create()
        {
            ViewData["SerieID"] = new SelectList(_context.Serie, "SerieID", "Descricao");
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlunoResponsavelViewModel AlunoResponsavelViewModel, IFormFile img)
        {
            if (!RaExists(AlunoResponsavelViewModel.Aluno.Ra, false, 0))
            {
                AlunoResponsavelViewModel.Aluno.Serie = _context.Serie.FirstOrDefault(x => x.SerieID == AlunoResponsavelViewModel.Aluno.SerieID);
                var aluno = PopularAluno(AlunoResponsavelViewModel.Aluno);

                var responsavelLegal = PopularResponsavel(AlunoResponsavelViewModel.ResponsavelLegal);
                responsavelLegal.DataCadastro = aluno.DataCadastro;

                var responsavelFinanceiro = PopularResponsavel(AlunoResponsavelViewModel.ResponsavelFinanceiro);
                responsavelFinanceiro.DataCadastro = aluno.DataCadastro;

                aluno.Escola = CarregaEscola(aluno);

                #region VERIFICA RESPONSÁVEL

                if (AlunoResponsavelViewModel.ResponsavelLegal.ResponsavelID > 0)
                {
                    if (responsavelLegal.Email != null)
                        _context.Update(responsavelLegal);
                }
                else if (AlunoResponsavelViewModel.ResponsavelFinanceiro.ResponsavelID > 0)
                {
                    if (responsavelFinanceiro.Email != null)
                        _context.Update(responsavelFinanceiro);
                }
                else if (AlunoResponsavelViewModel.ResponsavelLegal.ResponsavelID == 0)
                {
                    if (responsavelLegal.Email != null)
                        _context.Add(responsavelLegal);
                }
                else if (AlunoResponsavelViewModel.ResponsavelFinanceiro.ResponsavelID == 0)
                {
                    if (responsavelFinanceiro.Email != null)
                        _context.Add(responsavelFinanceiro);
                }

                if (responsavelLegal.Email != null || responsavelFinanceiro.Email != null)
                    await _context.SaveChangesAsync();

                #endregion

                #region CRIA USUÁRIO

                if (responsavelLegal.Email != null)
                    await CriaUsuarioAsync(responsavelLegal, aluno.EscolaID);

                if (responsavelFinanceiro.Email != null)
                    await CriaUsuarioAsync(responsavelFinanceiro, aluno.EscolaID);

                #endregion

                aluno.ResposavelFinanceiroID = responsavelFinanceiro.ResponsavelID;
                aluno.ResposavelLegalID = responsavelLegal.ResponsavelID;

                if (img != null)
                {
                    if (img.Length > 0)
                    {

                        byte[] p1 = null;
                        using (var fs1 = img.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }

                        aluno.Foto = p1;
                    }
                }

                _context.Add(aluno);

                await _context.SaveChangesAsync();
                Alert("Aluno Salvo com sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Alert("Já contém um aluno com esse RA!", NotificationType.error);
                ViewData["SerieID"] = new SelectList(_context.Serie, "SerieID", "Descricao", AlunoResponsavelViewModel.Aluno.SerieID);
                ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
                return View(AlunoResponsavelViewModel);
            }
        }

        // GET: Aluno/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                Alert("Aluno não encontrado.", NotificationType.warning);
                return View();
            }

            var aluno = await _context.Aluno.SingleOrDefaultAsync(m => m.AlunoID == id);

            var responsavelLegal = await _context.Responsavel.SingleOrDefaultAsync(r => r.ResponsavelID == aluno.ResposavelLegalID);
            var responsavelFinanceiro = await _context.Responsavel.SingleOrDefaultAsync(r => r.ResponsavelID == aluno.ResposavelFinanceiroID);
            aluno.ResposavelLegal = responsavelLegal;
            aluno.ResposavelFinanceiro = responsavelFinanceiro;

            if (aluno == null)
            {
                Alert("Aluno não encontrado.", NotificationType.warning);
                return View();
            }

            ViewData["SerieID"] = new SelectList(_context.Serie, "SerieID", "Descricao", aluno.SerieID);
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");

            var alunoResponsavelViewModel = new AlunoResponsavelViewModel
            {
                Aluno = aluno,
                ResponsavelLegal = aluno.ResposavelLegal,
                ResponsavelFinanceiro = aluno.ResposavelFinanceiro
            };

            alunoResponsavelViewModel.Aluno.Escola = CarregaEscola(aluno);

            if (aluno.Foto != null)
            {
                var img = Convert.ToBase64String(aluno.Foto);
                alunoResponsavelViewModel.Imagem = img;
            }

            return View(alunoResponsavelViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AlunoResponsavelViewModel alunoView, IFormFile img)
        {
            if (!RaExists(alunoView.Aluno.Ra, true, alunoView.Aluno.AlunoID))
            {
                try
                {
                    var responsavelLegal = PopularResponsavel(alunoView.ResponsavelLegal);
                    responsavelLegal.DataCadastro = alunoView.Aluno.DataCadastro;

                    var responsavelFinanceiro = PopularResponsavel(alunoView.ResponsavelFinanceiro);
                    responsavelFinanceiro.DataCadastro = alunoView.Aluno.DataCadastro;

                    alunoView.Aluno.Escola = CarregaEscola(alunoView.Aluno);

                    #region VERIFICA RESPONSÁVEL

                    if (alunoView.ResponsavelLegal.ResponsavelID > 0)
                    {
                        if (responsavelLegal.Email != null)
                            _context.Update(responsavelLegal);
                    }

                    if (alunoView.ResponsavelFinanceiro.ResponsavelID > 0)
                    {
                        if (responsavelFinanceiro.Email != null)
                            _context.Update(responsavelFinanceiro);
                    }

                    if (alunoView.ResponsavelLegal.ResponsavelID == 0)
                    {
                        if (responsavelLegal.Email != null)
                            _context.Add(responsavelLegal);
                    }

                    if (alunoView.ResponsavelFinanceiro.ResponsavelID == 0)
                    {
                        if (responsavelFinanceiro.Email != null)
                            _context.Add(responsavelFinanceiro);
                    }

                    if (responsavelLegal.Email != null || responsavelFinanceiro.Email != null)
                        await _context.SaveChangesAsync();

                    #endregion

                    #region CRIA USUÁRIO

                    if (responsavelLegal.Email != null)
                        await CriaUsuarioAsync(responsavelLegal, alunoView.Aluno.EscolaID);

                    if (responsavelFinanceiro.Email != null)
                        await CriaUsuarioAsync(responsavelFinanceiro, alunoView.Aluno.EscolaID);

                    #endregion

                    alunoView.Aluno.ResposavelFinanceiroID = responsavelFinanceiro.ResponsavelID;
                    alunoView.Aluno.ResposavelLegalID = responsavelLegal.ResponsavelID;

                    if (img != null)
                    {
                        if (img.Length > 0)
                        {

                            byte[] p1 = null;
                            using (var fs1 = img.OpenReadStream())
                            using (var ms1 = new MemoryStream())
                            {
                                fs1.CopyTo(ms1);
                                p1 = ms1.ToArray();
                            }

                            alunoView.Aluno.Foto = p1;
                        }
                    }

                    _context.Update(alunoView.Aluno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoExists(alunoView.Aluno.AlunoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Alert("Aluno Atualizado com Sucesso!", NotificationType.success);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                Alert("Já contém um aluno com esse RA!", NotificationType.error);
                ViewData["SerieID"] = new SelectList(_context.Serie, "SerieID", "Descricao", alunoView.Aluno.SerieID);
                ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
                return View(alunoView);
            }
        }

        // GET: Aluno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Aluno
                .Include(a => a.Escola)
                .SingleOrDefaultAsync(m => m.AlunoID == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // POST: Aluno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int AlunoID)
        {
            var aluno = await _context.Aluno.SingleOrDefaultAsync(m => m.AlunoID == AlunoID);
            _context.Aluno.Remove(aluno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private static Aluno PopularAluno(Aluno Aluno)
        {
            var aluno = new Aluno
            {
                Ativo = Aluno.Ativo,
                DataCadastro = Aluno.DataCadastro,
                DataNascimento = Aluno.DataNascimento,
                Nome = Aluno.Nome,
                Ra = Aluno.Ra,
                Serie = Aluno.Serie,
                TipoGenero = Aluno.TipoGenero,
                EscolaID = Aluno.EscolaID,
                SerieID = Aluno.SerieID
            };

            return aluno;
        }

        private static Responsavel PopularResponsavel(Responsavel Responsavel)
        {
            var responsavel = new Responsavel
            {
                ResponsavelID = Responsavel.ResponsavelID,
                DataCadastro = Responsavel.DataCadastro,
                Nome = Responsavel.Nome,
                Telefone = Responsavel.Telefone,
                Email = Responsavel.Email,
                CPF = Responsavel.CPF
            };

            return responsavel;
        }

        private bool AlunoExists(int id)
        {
            return _context.Aluno.Any(e => e.AlunoID == id);
        }

        private bool RaExists(string ra, bool boEdit, int AlunoID)
        {
            if (!boEdit)
                return _context.Aluno.Any(e => e.Ra == ra);
            else
                return _context.Aluno.Any(e => e.Ra == ra && e.AlunoID != AlunoID);
        }

        private Escola CarregaEscola(Aluno aluno)
        {
            if (aluno.EscolaID == 0)
                Alert("Erro carregar escola!", NotificationType.error);

            return _context.Escola.FirstOrDefault(x => x.EscolaID == aluno.EscolaID);
        }

        [HttpPost]
        public JsonResult AutoComplete(string dado, string IDescola)
        {
            var settings = new JsonSerializerSettings();
            var ObjList = new List<Aluno>();
            var escolaId = Convert.ToInt32(IDescola);

            //ObjList = _context.Aluno.Include(e => e.Escola).ToList();

            ObjList = _context.Aluno.Include(e => e.Escola).Where(x => x.EscolaID == escolaId).Include(s => s.Serie).ToList();

            var AlunoList = (from N in ObjList
                             where N.Ra.ToLower().Contains(dado.ToLower()) || N.Nome.ToLower().Contains(dado.ToLower())
                             select new
                             {
                                 IdAluno = N.AlunoID,
                                 RA = N.Ra,
                                 NomeAluno = N.Nome,
                                 IdEscola = N.Escola.EscolaID,
                                 NomeEscola = N.Escola.Nome,
                                 Sexo = N.TipoGenero == 0 ? "Feminino" : "Masculino",
                                 Serie = N.Serie.Descricao,
                                 IdSerie = N.SerieID,
                                 Nascimento = N.DataNascimento.ToShortDateString()
                             });

            return Json(AlunoList, settings);
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        public List<Aluno> RetornaPesquisa(int? idEscola, int? idSerie, string Aluno, string Sexo, string ativo)
        {
            var genero = Sexo == "M" ? Genero.Masculino : Genero.Feminino;
            var AlunoList = new List<Aluno>();

            var status = true;
            if (ativo == null)
                status = true;
            else if (ativo != "0")
                status = bool.Parse(ativo);

            if (Aluno != null)
            {
                AlunoList = (from N in _context.Aluno.Include(e => e.Escola).Include(s => s.Serie)
                             where N.Escola.EscolaID == idEscola &&
                                        N.Nome.ToLower().Contains(Aluno.ToLower()) &&
                                        (idSerie == 0 || N.Serie.SerieID == idSerie) &&
                                        N.TipoGenero == genero &&
                                        (ativo == "0" || N.Ativo == status)
                             select N).OrderBy(x => x.Nome).ToList();
            }
            else
            {
                AlunoList = (from N in _context.Aluno.Include(e => e.Escola).Include(s => s.Serie)
                             where N.Escola.EscolaID == idEscola &&
                                    (idSerie == 0 || N.Serie.SerieID == idSerie) &&
                                    N.TipoGenero == genero &&
                                    (ativo == "0" || N.Ativo == status)
                             select N).OrderBy(x => x.Nome).ToList();
            }

            return AlunoList;
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "AlunoController");
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

        private async Task<IActionResult> CriaUsuarioAsync(Responsavel responsavel, int EscolaID)
        {
            try
            {
                var escola = _context.Escola.FirstOrDefault(e => e.EscolaID == EscolaID);

                var random = new Random();
                var num = random.Next(1, 99999);
                var senha = "@Epta" + num.ToString();

                if (!VerificaUsuarioCadastrado(responsavel.Email))
                {
                    var user = new ApplicationUser { UserName = responsavel.Email, Email = responsavel.Email, EscolaID = escola.EscolaID, Escola = escola, Ativo = true, Nome = responsavel.Nome };
                    if (user.Perfil == null)
                        user.PerfilID = null;
                    var result = await _userManager.CreateAsync(user, senha);
                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        await _emailSender.SendEmailAsync(responsavel.Email, "Confirmação de E-mail", "Prezado, <br /><br /> Para acessar o sistema, confirme seu e-mail <a href=" + $"'{callbackUrl}'" + ">Clicando aqui!</a></p><p>Dados para acesso ao sistema: <br /><br />" + $"Login: {responsavel.Email}" + $"<br /> Senha: {senha} " + "<br /><br /> Atenciosamente, <br /> My Bonus");

                        await GravaPermissoes(user);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Alert(ex.Message, NotificationType.error);
                throw;
            }
        }

        private bool VerificaUsuarioCadastrado(string email)
        {
            if (_context.Usuario.FirstOrDefault(x => x.Email == email) != null)
                return true;

            return false;
        }

        public async Task GravaPermissoes(ApplicationUser usuario)
        {
            var dt = Seguranca.RetornaPermissoesTelas();

            foreach (var item in dt.Select().ToList())
            {
                var permissoes = new UsuarioPermissoes
                {
                    Controller = item[0].ToString(),
                    Permissao = item[0].ToString() == "ResponsavelController" ? "EDITAR" : "BLOQUEADO",
                    ApplicationUser = usuario,
                    ApplicationUserID = usuario.Id
                };

                _context.UsuarioPermissoes.Add(permissoes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
