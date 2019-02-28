using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalEducacional.Data;
using PortalEducacional.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PortalEducacional.Services;
using static PortalEducacional.Enum.Enums;
using PortalEducacional.Extensions;
using System.Threading;
using Newtonsoft.Json;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Acesso Responsável", DescricaoTela = "Cadastro de Responsável", Controller = "ResponsavelController", Acao = "Bloqueado;Editar")]
    public class ResponsavelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResponsavelController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Responsavel
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

            //var respon = _context.Responsavel.FirstOrDefault(x => x.Email == User.Identity.Name);

            //if (respon == null)
            //{
            //    respon = new Responsavel();
            //    respon.ResponsavelFinanceiro = new List<ResponsavelFinanceiro>();
            //}

            //respon.ResponsavelFinanceiro = CarregaListaResponsavelFinanceiro(respon, User.Identity.Name);

            //return View(respon.ResponsavelFinanceiro);

            var respon = _context.Responsavel.FirstOrDefault(x => x.Email == User.Identity.Name);

            if (respon != null)
            {
                respon.ResponsavelFinanceiro = CarregaListaResponsavelFinanceiro(respon);

                return View(respon.ResponsavelFinanceiro);
            }
            else
            {
                Alert("Tela liberada apenas para usuário do tipo Responsável!", NotificationType.error);
                return Redirect(Url.Content("~/Home/Index"));
            }
        }

        // GET: Responsavel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var responsavel = await _context.Responsavel
                .SingleOrDefaultAsync(m => m.ResponsavelID == id);
            if (responsavel == null)
            {
                return NotFound();
            }

            return View(responsavel);
        }

        // GET: Responsavel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Responsavel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Responsavel responsavel)
        {
            var mensagem = string.Empty;

            responsavel = CarregaResponsavel(responsavel);

            UpdateListaResponsaveis(ref responsavel);

            if (responsavel != null)
            {
                mensagem = VerificaResponsavelCadastrado(responsavel.ResponsavelFinanceiro.ToList());

                if (!string.IsNullOrEmpty(mensagem))
                {
                    Alert(string.Format("Já existe um responsável cadastrado com o e-mail {0}.", mensagem), NotificationType.error);
                    return (View(responsavel));
                }

                await CriaUsuarioAsync(responsavel.ResponsavelFinanceiro.ToList());

                _context.Update(responsavel);
                await _context.SaveChangesAsync();

                Alert("Responsáveis cadastrados com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            else
                return (View(responsavel));
        }

        // GET: Responsavel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var responsavelFinanceiro = await _context.ResponsavelFinanceiro.SingleOrDefaultAsync(m => m.ResponsavelFinanceiroID == id);

            if (responsavelFinanceiro == null)
            {
                return NotFound();
            }

            return View(responsavelFinanceiro);
        }

        // POST: Responsavel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ResponsavelFinanceiroID, int ResponsavelID, ResponsavelFinanceiro responsavelFinanceiro)
        {
            if (ResponsavelFinanceiroID != responsavelFinanceiro.ResponsavelFinanceiroID)
            {
                return NotFound();
            }

            try
            {
                responsavelFinanceiro.ResponsavelID = ResponsavelID;

                _context.Update(responsavelFinanceiro);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResponsavelExists(responsavelFinanceiro.ResponsavelFinanceiroID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Alert("Responsável atualizado com Sucesso!", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        // GET: Responsavel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var responsavel = await _context.ResponsavelFinanceiro
                .SingleOrDefaultAsync(m => m.ResponsavelFinanceiroID == id);
            if (responsavel == null)
            {
                return NotFound();
            }

            return View(responsavel);
        }

        // POST: Responsavel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ResponsavelFinanceiroID)
        {
            var responsavelFinanceiro = await _context.ResponsavelFinanceiro.SingleOrDefaultAsync(m => m.ResponsavelFinanceiroID == ResponsavelFinanceiroID);

            _context.ResponsavelFinanceiro.Remove(responsavelFinanceiro);
            await _context.SaveChangesAsync();

            Alert("Responsável removido com Sucesso!", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        private bool ResponsavelExists(int id)
        {
            return _context.ResponsavelFinanceiro.Any(e => e.ResponsavelFinanceiroID == id);
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        private Responsavel CarregaResponsavel(Responsavel responsavel)
        {
            var email = User.Identity.Name;

            var respon = _context.Responsavel.FirstOrDefault(x => x.Email == email);

            respon.ResponsavelFinanceiro = responsavel.ResponsavelFinanceiro;

            foreach (var item in respon.ResponsavelFinanceiro)
            {
                item.Responsavel = respon;
                item.ResponsavelID = respon.ResponsavelID;
            }

            return respon;
        }

        private Responsavel CarregaResponsavelPorID(int responsavelID)
        {
            var respon = _context.Responsavel.FirstOrDefault(x => x.ResponsavelID == responsavelID);

            return respon;
        }

        private List<ResponsavelFinanceiro> CarregaListaResponsavelFinanceiro(Responsavel responsavel)
        {
             return _context.ResponsavelFinanceiro.Where(x => x.ResponsavelID == responsavel.ResponsavelID).ToList();
        }

        private async Task<IActionResult> CriaUsuarioAsync(List<ResponsavelFinanceiro> lstResponsaveis)
        {
            try
            {
                //Usuário Logado
                var idUsuario = User.Claims.Select(u => u.Value).FirstOrDefault().ToString();

                var usuario = _context.Usuario.FirstOrDefault(x => x.Id == idUsuario);
                var escola = _context.Escola.FirstOrDefault(e => e.EscolaID == usuario.EscolaID);

                foreach (var item in lstResponsaveis)
                {
                    var random = new Random();
                    var num = random.Next(1, 99999);
                    var senha = "@Epta" + num.ToString();

                    if (!VerificaUsuarioCadastrado(item.Email))
                    {
                        var user = new ApplicationUser { UserName = item.Email, Email = item.Email, EscolaID = escola.EscolaID, Escola = escola, Ativo = true, Nome = item.Nome };
                        if (user.Perfil == null)
                            user.PerfilID = null;
                        var result = await _userManager.CreateAsync(user, senha);
                        if (result.Succeeded)
                        {
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                            await _emailSender.SendEmailAsync(item.Email, "Confirmação de E-mail", "Prezado, <br /><br /> Para acessar o sistema, confirme seu e-mail <a href=" + $"'{callbackUrl}'" + ">Clicando aqui!</a></p><p>Dados para acesso ao sistema: <br /><br />" + $"Login: {item.Email}" + $"<br /> Senha: {senha} " + "<br /><br /> Atenciosamente, <br /> My Bonus");

                            await GravaPermissoes(user);
                        }
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

        private string VerificaResponsavelCadastrado(List<ResponsavelFinanceiro> lstResponsaveis)
        {
            var mensagem = string.Empty;

            foreach (var item in lstResponsaveis)
            {
                if (_context.ResponsavelFinanceiro.FirstOrDefault(x => x.Email == item.Email) != null)
                    return mensagem = item.Email;

                if (_context.Responsavel.FirstOrDefault(x => x.Email == item.Email) != null)
                    return mensagem = item.Email;
            }

            return mensagem;
        }

        private bool VerificaUsuarioCadastrado(string email)
        {
            if (_context.ResponsavelFinanceiro.FirstOrDefault(x => x.Email == email) != null)
                return true;

            if (_context.Responsavel.FirstOrDefault(x => x.Email == email) != null)
                return true;

            if (_context.Usuario.FirstOrDefault(x => x.Email == email) != null)
                return true;

            return false;
        }

        private void UpdateListaResponsaveis(ref Responsavel responsavel)
        {
            var lista = responsavel.ResponsavelFinanceiro;
            var i = 0;
            if (responsavel.ResponsavelFinanceiro != null)
                responsavel.ResponsavelFinanceiro.ToList().ForEach(x => {
                    if (x.ResponsavelFinanceiroID == 0 && x.Deletado)
                        lista.RemoveAt(i);
                    i++;
                });
            responsavel.ResponsavelFinanceiro = lista;
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "ResponsavelController");
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

        [HttpPost]
        public JsonResult RetornaResponsavel(string dado)
        {
            var settings = new JsonSerializerSettings();

            var RespList = (from r in _context.Responsavel
                            where r.CPF.ApenasNumero() == dado.ApenasNumero()
                            select new { r.ResponsavelID, r.Nome, r.Email, r.Telefone });

            if (RespList.Count() == 0)
            {
                RespList = (from r in _context.ResponsavelFinanceiro
                            where r.CPF.ApenasNumero() == dado.ApenasNumero()
                            select new { r.ResponsavelID, r.Nome, r.Email, r.Telefone });
            }

            return Json(RespList, settings);
        }
    }
}
