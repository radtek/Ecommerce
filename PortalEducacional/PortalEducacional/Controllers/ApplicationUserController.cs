using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalEducacional.Data;
using PortalEducacional.Extensions;
using PortalEducacional.Models;
using PortalEducacional.Services;
using static PortalEducacional.Enum.Enums;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Segurança", DescricaoTela = "Cadastro de Usuário", Controller = "ApplicationUserController", Acao = "Bloqueado;Editar")]
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ApplicationUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: ApplicationUser
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

            var applicationDbContext = _context.Usuario.Include(a => a.Escola).Include(p => p.Perfil);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ApplicationUser/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Usuario
                .Include(a => a.Escola)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUser/Create
        public IActionResult Create()
        {
            var user = new ApplicationUser();
            var list = new List<UsuarioPermissoes>();

            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewBag.Perfil = new SelectList(_context.Perfil, "PerfilID", "Nome");

            var dt = Seguranca.RetornaPermissoesTelas();

            foreach (var item in dt.Select().ToList())
            {
                var usuarioPermissoes = new UsuarioPermissoes
                {
                    Controller = item[0].ToString(),
                    Menu = item[2].ToString(),
                    DescricaoTela = item[3].ToString(),
                    Permissao = item[4].ToString(),
                    Status = "BLOQUEADO"
                };

                list.Add(usuarioPermissoes);
            }

            user.ListaPermissoes = list;

            return View(user);
        }

        // POST: ApplicationUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser usuario)
        {
            if (usuario.PerfilID == 0)
                return View(usuario);

            await CriaUsuarioAsync(usuario);

            return RedirectToAction(nameof(Index));
        }

        // GET: ApplicationUser/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var usuario = _context.Usuario.FirstOrDefault(m => m.Id == id);

            var list = new List<UsuarioPermissoes>();
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", usuario.EscolaID);
            ViewBag.Perfil = new SelectList(_context.Perfil, "PerfilID", "Nome", usuario.PerfilID);

            var permissoes = _context.UsuarioPermissoes.Where(x => x.ApplicationUserID == usuario.Id).ToList();

            var dt = Seguranca.RetornaPermissoesTelas();

            foreach (var item in dt.Select().ToList())
            {
                var usuarioPermissoes = new UsuarioPermissoes
                {
                    Controller = item[0].ToString(),
                    Menu = item[2].ToString(),
                    DescricaoTela = item[3].ToString(),
                    Permissao = item[4].ToString(),
                    Status = usuario.Master ? "EDITAR" : "BLOQUEADO"
                };

                foreach (var item2 in permissoes)
                {
                    if (item2.Controller == usuarioPermissoes.Controller)
                    {
                        usuarioPermissoes.Status = item2.Permissao;
                        usuarioPermissoes.ApplicationUserID = item2.ApplicationUserID;
                        break;
                    }
                }

                list.Add(usuarioPermissoes);
            }

            usuario.ListaPermissoes = list;

            return View(usuario);
        }

        // POST: ApplicationUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser usuario)
        {
            try
            {
                if (usuario.PerfilID == 0 || usuario.PerfilID == null)
                {
                    Alert("O campos Perfil é Obrigatório", NotificationType.error);
                    return View(usuario);
                }

                var perfil = _context.Perfil.FirstOrDefault(x => x.PerfilID == usuario.PerfilID);
                var escola = _context.Escola.FirstOrDefault(x => x.EscolaID == usuario.EscolaID);

                var user = await _userManager.FindByIdAsync(usuario.Id);

                user.AccessFailedCount = usuario.AccessFailedCount;
                user.Ativo = usuario.Ativo;
                user.ConcurrencyStamp = usuario.ConcurrencyStamp;
                user.ConfirmPassword = usuario.ConfirmPassword;
                user.Password = usuario.Password;
                user.Email = usuario.Email;
                user.EmailConfirmed = usuario.EmailConfirmed;
                user.Escola = _context.Escola.FirstOrDefault(x => x.EscolaID == usuario.EscolaID);
                user.EscolaID = usuario.EscolaID;
                user.LockoutEnabled = usuario.LockoutEnabled;
                user.LockoutEnd = usuario.LockoutEnd;
                user.Master = usuario.Master;
                user.Nome = usuario.Nome;
                user.NormalizedEmail = usuario.Email;
                user.NormalizedUserName = usuario.Email;
                user.PasswordHash = usuario.PasswordHash;
                user.Perfil = perfil;
                user.PerfilID = usuario.PerfilID;
                user.PhoneNumber = usuario.PhoneNumber;
                user.PhoneNumberConfirmed = usuario.PhoneNumberConfirmed;
                user.SecurityStamp = usuario.SecurityStamp;
                user.TwoFactorEnabled = usuario.TwoFactorEnabled;
                user.UserName = usuario.UserName;

                var result = await _userManager.UpdateAsync(user);

                await _context.SaveChangesAsync();

                //ATUALIZA PERMISSÕES USUÁRIO
                foreach (var item in usuario.ListaPermissoes)
                {
                    var permissoes = new UsuarioPermissoes
                    {
                        Controller = item.Controller,
                        ApplicationUser = usuario,
                        ApplicationUserID = usuario.Id,
                        UsuarioPermissoesID = item.UsuarioPermissoesID,
                        Permissao = item.Status
                    };

                    _context.Update(permissoes);
                    await _context.SaveChangesAsync();
                }

                Alert("Usuário Atualizado com Sucesso!", NotificationType.success);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ApplicationUserExists(usuario.Id))
                {
                    return NotFound();
                }
                else
                {
                    Alert(ex.Message, NotificationType.warning);
                    ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
                    ViewBag.Perfil = new SelectList(_context.Perfil, "PerfilID", "Nome");
                    return View(usuario);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ApplicationUser/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Usuario
                .Include(a => a.Escola)
                .SingleOrDefaultAsync(m => m.Id == id);

            var permissoes = _context.UsuarioPermissoes.Where(p => p.ApplicationUserID == id);

            applicationUser.ListaPermissoes = permissoes.ToList();

            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ApplicationUser usuario)
        {
            if (usuario.ListaPermissoes.Count() > 0)
            {
                foreach (var item in usuario.ListaPermissoes)
                {
                    var usuarioPermissoes = new UsuarioPermissoes
                    {
                        ApplicationUser = item.ApplicationUser,
                        ApplicationUserID = item.ApplicationUserID,
                        Controller = item.Controller,
                        DescricaoTela = item.DescricaoTela,
                        Menu = item.Menu,
                        Permissao = item.Permissao,
                        Status = item.Status,
                        UsuarioPermissoesID = item.UsuarioPermissoesID
                    };

                    _context.Remove(usuarioPermissoes);
                    await _context.SaveChangesAsync();
                }
            }

            var applicationUser = await _context.Usuario.SingleOrDefaultAsync(m => m.Id == usuario.Id);
            _context.Usuario.Remove(applicationUser);
            await _context.SaveChangesAsync();

            Alert("Usuário deletado com Sucesso!", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }

        private async Task<IActionResult> CriaUsuarioAsync(ApplicationUser usuario)
        {
            try
            {
                var perfil = _context.Perfil.FirstOrDefault(x => x.PerfilID == usuario.PerfilID);
                var escola = _context.Escola.FirstOrDefault(x => x.EscolaID == usuario.EscolaID);
                ApplicationUser user;
                if (!VerificaUsuarioCadastrado(usuario.Email))
                {
                    user = new ApplicationUser
                    {
                        UserName = usuario.Email,
                        Email = usuario.Email,
                        Nome = usuario.Nome,
                        Perfil = perfil,
                        PerfilID = usuario.PerfilID,
                        Escola = escola,
                        EscolaID = usuario.EscolaID,
                        Ativo = usuario.Ativo,
                        Master = usuario.Master
                    };
                    var result = await _userManager.CreateAsync(user, usuario.Password);
                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        await _emailSender.SendEmailAsync(usuario.Email, "Confirmação de E-mail", "Prezado, <br /><br /> Para acessar o sistema, confirme seu e-mail <a href=" + $"'{callbackUrl}'" + ">Clicando aqui!</a></p><p>Dados para acesso ao sistema: <br /><br />" + $"Login: {usuario.Email}" + $"<br /> Senha: {usuario.Password} " + "<br /><br /> Atenciosamente, <br /> My Bonus");
                    }
                    else
                    {
                        Alert("Erro ao salvar usuário. Entre em contato com o Suporte Técnico!", NotificationType.warning);
                        return View(usuario);
                    }
                }
                else
                {
                    Alert("E-mail já cadastro no sistema!", NotificationType.warning);
                    return View(usuario);
                }
                user.ListaPermissoes = usuario.ListaPermissoes;
                await GravaPermissoes(user);

                Alert("Usuário cadastrado com Sucesso!", NotificationType.success);
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
            foreach (var item in usuario.ListaPermissoes)
            {
                var permissoes = new UsuarioPermissoes
                {
                    Controller = item.Controller,
                    Permissao = item.Status,
                    ApplicationUser = usuario,
                    ApplicationUserID = usuario.Id
                };

                _context.UsuarioPermissoes.Add(permissoes);
                await _context.SaveChangesAsync();
            }
        }

        private bool VerificaUsuarioCadastrado(string email)
        {
            if (_context.Usuario.FirstOrDefault(x => x.Email == email) != null)
                return true;

            return false;
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        [HttpPost]
        public JsonResult RetornaPermissoes(string dado)
        {
            var id = int.Parse(dado);

            var settings = new JsonSerializerSettings();

            var PermissoesList = (from c in _context.PerfilPermissoes
                                  where c.PerfilID == id
                                  select new { c.Controller, c.Permissao });

            return Json(PermissoesList, settings);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "ApplicationUserController");
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
