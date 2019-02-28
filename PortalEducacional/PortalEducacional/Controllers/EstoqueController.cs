using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalEducacional.Data;
using PortalEducacional.Models;
using PortalEducacional.Models.ViewModel;
using static PortalEducacional.Enum.Enums;
using PortalEducacional.Services;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Produtos", DescricaoTela = "Entrada/Saída Estoque", Controller = "EstoqueController", Acao = "Bloqueado;Editar")]
    public class EstoqueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public EstoqueController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Estoque
        public async Task<IActionResult> Index(int? idEscola)
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

            var estoque = new List<Estoque>();

            var listaEscolaId = new SelectList(_context.Escola, "EscolaID", "Nome", idEscola == null ? usuario.EscolaID : idEscola);
            ViewBag.Escola = listaEscolaId;

            if (!usuario.Master)
            {
                if (usuario.EscolaID > 0)
                {
                    listaEscolaId = new SelectList(_context.Escola.Where(x => x.EscolaID == usuario.EscolaID), "EscolaID", "Nome", idEscola);
                    ViewBag.Escola = listaEscolaId;
                }
            }

            var id = idEscola == null ? usuario.EscolaID : idEscola;

            if (idEscola == 0)
                estoque = _context.Estoque.Include(e => e.Escola).Include(x => x.Produto).ToList();
            else if (idEscola == null && !usuario.Master)
                estoque = _context.Estoque.Include(e => e.Escola).Include(x => x.Produto).Where(x => x.EscolaID == usuario.EscolaID).ToList();
            else
                estoque = RetornaPesquisa(idEscola);

            return View(estoque);
        }

        // GET: Estoque/Details/5
        public async Task<IActionResult> Details()
        {
            var estoque = _context.EstoqueEmpresa.Include(e => e.Escola).Include(p => p.Produto).OrderBy(e => e.Produto.Codigo);

            return View(estoque);
        }

        // GET: Estoque/Create
        public IActionResult Create()
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var listaEscolaId = new SelectList(_context.Escola, "EscolaID", "Nome");

            if (!usuario.Master)
            {
                if (usuario.EscolaID > 0)
                    listaEscolaId = new SelectList(_context.Escola.Where(x => x.EscolaID == usuario.EscolaID), "EscolaID", "Nome", usuario.EscolaID);
            }

            ViewBag.Escola = listaEscolaId;

            return View();
        }

        // POST: Estoque/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstoqueEstoqueEmpresaViewModel EstoqueViewModel)
        {
            var estoque = PopularEstoque(EstoqueViewModel.Estoque);

            var estoqueEmpresa = PopularEstoqueEmpresa(EstoqueViewModel.EstoqueEmpresa);

            estoque.AspNetUsersID = User.Claims.Select(x => x.Value).FirstOrDefault();
            estoque.Escola = CarregaEscola(EstoqueViewModel.Estoque);
            estoque.Produto = CarregaProduto(EstoqueViewModel.Estoque);
            estoqueEmpresa.Escola = estoque.Escola;
            estoqueEmpresa.EscolaID = estoque.Escola.EscolaID;
            estoqueEmpresa.Produto = estoque.Produto;
            estoqueEmpresa.ProdutoID = estoque.Produto.ProdutoID;
            estoque.DataCadastro = estoque.DataCadastro + " " + DateTime.Now.ToLongTimeString();

            if (!estoque.Saida)
            {
                if (string.IsNullOrEmpty(estoqueEmpresa.QuantidadeMaxima.ToString()) || string.IsNullOrEmpty(estoque.ValorCompra.ToString()))
                {
                    Alert("Para operação de ENTRADA os campos Valor Compra e Quantidade Mínima deverão ser preenchidos.", NotificationType.info);
                    ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", estoque.EscolaID);
                    return View(EstoqueViewModel);
                }
            }

            if (estoque.Saida)
            {
                if (!VerificaSaldoSuficiente(estoque))
                {
                    Alert(string.Format("O produto {0} - {1} não possui saldo suficiente para essa ação.", estoque.Produto.Codigo, estoque.Produto.Descricao), NotificationType.warning);
                    ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", estoque.EscolaID);
                    return View(EstoqueViewModel);
                }

            }

            estoqueEmpresa = RetornaSaldoFinal(estoque, estoqueEmpresa);

            if (estoqueEmpresa == null)
                return View(EstoqueViewModel);

            if (estoqueEmpresa.Escola == null)
                estoqueEmpresa.Escola = CarregaEscola(estoque);

            _context.Add(estoque);

            if (estoqueEmpresa.EstoqueEmpresaID == 0)
                _context.Add(estoqueEmpresa);
            else
                _context.Update(estoqueEmpresa);

            await AvisaEstoqueMinino(estoqueEmpresa);

            await _context.SaveChangesAsync();

            Alert("Movimento registrado com Sucesso!", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        // GET: Estoque/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoque = await _context.Estoque.SingleOrDefaultAsync(m => m.EstoqueID == id);
            if (estoque == null)
            {
                return NotFound();
            }

            return View(estoque);
        }

        // POST: Estoque/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstoqueID,EscolaID,ProdutoID,AspNetUsersID,Data,Saida,Quantidade,Historico")] Estoque estoque)
        {
            if (id != estoque.EstoqueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estoque);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstoqueExists(estoque.EstoqueID))
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
            ViewData["EscolaID"] = new SelectList(_context.Escola, "EscolaID", "CNPJ", estoque.EscolaID);
            ViewData["ProdutoID"] = new SelectList(_context.Produto, "ProdutoID", "Codigo", estoque.ProdutoID);
            return View(estoque);
        }

        // GET: Estoque/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoque = await _context.Estoque
                .Include(e => e.Escola)
                .Include(e => e.Produto)
                .SingleOrDefaultAsync(m => m.EstoqueID == id);
            if (estoque == null)
            {
                return NotFound();
            }

            return View(estoque);
        }

        // POST: Estoque/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estoque = await _context.Estoque.SingleOrDefaultAsync(m => m.EstoqueID == id);
            _context.Estoque.Remove(estoque);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstoqueExists(int id)
        {
            return _context.Estoque.Any(e => e.EstoqueID == id);
        }

        private Escola CarregaEscola(Estoque estoque)
        {
            var id = estoque.EscolaID == 0 ? estoque.Escola.EscolaID : estoque.EscolaID;

            return _context.Escola.FirstOrDefault(x => x.EscolaID == id);
        }

        private Produto CarregaProduto(Estoque estoque)
        {
            return _context.Produto.FirstOrDefault(x => x.ProdutoID == estoque.Produto.ProdutoID);
        }

        private static Estoque PopularEstoque(Estoque Estoque)
        {
            var estoque = new Estoque
            {
                EscolaID = Estoque.EstoqueID,
                ProdutoID = Estoque.ProdutoID,
                AspNetUsersID = Estoque.AspNetUsersID,
                DataCadastro = Estoque.DataCadastro,
                Saida = Estoque.Saida,
                ValorCompra = Estoque.ValorCompra,
                Quantidade = Estoque.Quantidade,
                Historico = Estoque.Historico
            };

            return estoque;
        }

        private static EstoqueEmpresa PopularEstoqueEmpresa(EstoqueEmpresa EstoqueEmpresa)
        {
            var estoqueempresa = new EstoqueEmpresa
            {
                EscolaID = EstoqueEmpresa.EscolaID,
                ProdutoID = EstoqueEmpresa.ProdutoID,
                Saldo = 0,
                QuantidadeMinima = EstoqueEmpresa.QuantidadeMinima,
                QuantidadeMaxima = EstoqueEmpresa.QuantidadeMaxima,
                Validade = EstoqueEmpresa.Validade
            };

            return estoqueempresa;
        }

        private bool VerificaSaldoSuficiente(Estoque estoque)
        {
            double dcSaldoAtual = 0;

            var estEmpresa = _context.EstoqueEmpresa.FirstOrDefault(x => x.ProdutoID == estoque.Produto.ProdutoID);

            if (estEmpresa != null)
                dcSaldoAtual = estEmpresa.Saldo;

            if (estoque.Quantidade > dcSaldoAtual)
                return false;

            return true;
        }

        private async Task AvisaEstoqueMinino(EstoqueEmpresa estoqueEmpresa)
        {
            try
            {
                var saldo = estoqueEmpresa.Saldo;
                var qtdeMinima = estoqueEmpresa.QuantidadeMinima;

                var result = Convert.ToDouble(saldo) - qtdeMinima;

                if (result < 3)
                {
                    await EnviaEmail("lucas.paulista@epta.com.br", $"Estoque Minimo {estoqueEmpresa.Produto.Codigo} - {estoqueEmpresa.Produto.Descricao}",
                                                                    $"O Produto {estoqueEmpresa.Produto.Codigo} - {estoqueEmpresa.Produto.Descricao} está chegando perto da sua quantidade mínima. Saldo Atual {estoqueEmpresa.Saldo}");
                }

            }
            catch (Exception)
            {
                Alert("Erro ao Consulta Estoque minimo. Entre em contato com o Suporte Técnico através do CHAT!", NotificationType.error);
                return;
            }
        }

        private EstoqueEmpresa RetornaSaldoFinal(Estoque estoque, EstoqueEmpresa estoqueEmpresa)
        {
            try
            {
                double dcSaldoAtual = 0;

                var estoqueEmpresaAtual = _context.EstoqueEmpresa.FirstOrDefault(x => x.ProdutoID == estoque.Produto.ProdutoID);

                if (estoqueEmpresaAtual != null)
                    dcSaldoAtual = estoqueEmpresaAtual.Saldo;
                else
                    estoqueEmpresaAtual = estoqueEmpresa;

                if (estoque.Saida)
                    dcSaldoAtual = dcSaldoAtual - estoque.Quantidade;
                else
                    dcSaldoAtual = dcSaldoAtual + estoque.Quantidade;

                estoqueEmpresaAtual.Escola = CarregaEscola(estoque);
                estoqueEmpresaAtual.Produto = CarregaProduto(estoque);
                estoqueEmpresaAtual.Saldo = dcSaldoAtual;

                return estoqueEmpresaAtual;
            }
            catch (Exception)
            {
                Alert("Erro ao Atualizar Saldo. Entre em contato com o Suporte Técnico através do CHAT!", NotificationType.error);
                return null;
            }
        }

        private async Task EnviaEmail(string email, string assunto, string mensagem)
        {
            await _emailSender.SendEmailAsync(email, assunto, mensagem);
        }

        public List<Estoque> RetornaPesquisa(int? idEscola)
        {
            var estoqueList = (from N in _context.Estoque.Include(e => e.Escola).Include(s => s.Produto)
                               where N.Escola.EscolaID == idEscola || N.EstoqueID == idEscola
                               select N).OrderBy(x => x.DataCadastro).ToList();

            return estoqueList;
        }

        [HttpPost]
        public JsonResult RetornaControleProduto(string dado)
        {
            var settings = new JsonSerializerSettings();
            var idProduto = int.Parse(dado);

            var ControleList = (from E in _context.EstoqueEmpresa
                                where E.ProdutoID == idProduto || E.Produto.ProdutoID == idProduto
                                select new { E.QuantidadeMinima, E.QuantidadeMaxima, E.Validade }).FirstOrDefault();

            return Json(ControleList, settings);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "EstoqueController");
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
