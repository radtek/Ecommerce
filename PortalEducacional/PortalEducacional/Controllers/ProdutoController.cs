using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalEducacional.Data;
using PortalEducacional.Models;
using PortalEducacional.Services;
using static PortalEducacional.Enum.Enums;


namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Produtos", DescricaoTela = "Cadastro de Produto", Controller = "ProdutoController", Acao = "Bloqueado;Editar")]
    public class ProdutoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private bool isNutricionista = true;
        public ProdutoController(ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Produto
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

            return View(await _context.Produto.Include(e => e.Escola).ToListAsync());
        }

        // GET: Produto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .SingleOrDefaultAsync(m => m.ProdutoID == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produto/Create
        public IActionResult Create()
        {
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao");
            ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");

            var produto = new Produto();

            produto.DadosNutricionais = new List<DadoNutricional>();

            return View(produto);
        }

        // POST: Produto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {

            if (!string.IsNullOrEmpty(produto.CodigoBarras))
            {
                if (CodigoBarrasExists(produto.CodigoBarras))
                {
                    Alert($"Código de Barras {produto.CodigoBarras} já vinculado a outro produto no sistema!", NotificationType.error);
                    ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                    ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao", produto.CategoriaID);
                    ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", produto.EscolaID);
                    return View(produto);
                }
            }

            if (produto.DadosNutricionais == null)
            {
                Alert($"Dados Nutricionais vazios!", NotificationType.info);

                ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao", produto.CategoriaID);
                ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", produto.EscolaID);

                return View();
            }

            if (!ValidaProduto(produto))
            {
                Alert($"Código {produto.Codigo} já cadastrado no sistema!", NotificationType.info);

                ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao", produto.CategoriaID);
                ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", produto.EscolaID);

                return View(produto);
            }

            produto.UltimaAtualizacao = RetiraMiliseconds(DateTime.Now);
            UpdateDadosNutricionais(ref produto);
            produto.Escola = CarregaEscola(produto);

            if (produto.Replicar)
            {
                var escolas = _context.Escola.ToList();

                if (produto.PrecisaAprovacao)
                    await EnviaEmail("paulo.silva@epta.com.br", $"Produto {produto.Codigo} criado por não Nutricionista",
                        $"Usuário {Thread.CurrentPrincipal.Identity.Name} " +
                        $"criou um produto sem as informações nutricionais ");

                var listaProdutos = new List<Produto>();

                foreach (var item in escolas)
                {
                    var produtoEscola = new Produto
                    {
                        AtualizadoPor = produto.AtualizadoPor,
                        Categoria = produto.Categoria,
                        CategoriaID = produto.CategoriaID,
                        Codigo = produto.Codigo,
                        DadosNutricionais = produto.DadosNutricionais,
                        DataCadastro = produto.DataCadastro,
                        Descricao = produto.Descricao,
                        PrecisaAprovacao = produto.PrecisaAprovacao,
                        ProdutoID = produto.ProdutoID,
                        Replicar = produto.Replicar,
                        UltimaAtualizacao = produto.UltimaAtualizacao,
                        Valor = produto.Valor,
                        Ativo = produto.Ativo,
                        EscolaID = item.EscolaID,
                        Escola = item
                    };

                    try
                    {
                        listaProdutos.Add(produtoEscola);

                        _context.Add(produtoEscola);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        Alert("Erro ao cadastrar produto", NotificationType.error);
                        ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                        ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao", produto.CategoriaID);
                        ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", produto.EscolaID);
                        return RedirectToAction(nameof(Index));
                    }
                }

                //ADD DADOS NUTRICIONAIS
                foreach (var dados in listaProdutos)
                {
                    var nutri = new DadoNutricional
                    {
                        Porcao = dados.DadosNutricionais.Select(x => x.Porcao).FirstOrDefault(),
                        Produto = dados,
                        ProdutoID = dados.ProdutoID,
                        TipoNutricional = dados.DadosNutricionais.Select(x => x.TipoNutricional).FirstOrDefault(),
                        TipoNutricionalID = dados.DadosNutricionais.Select(x => x.TipoNutricionalID).FirstOrDefault(),
                        ValorDiario = dados.DadosNutricionais.Select(x => x.ValorDiario).FirstOrDefault()
                    };

                    try
                    {
                        nutri.Produto.DadosNutricionais = null;

                        _context.Add(nutri);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        Alert("Erro ao cadastrar produto", NotificationType.error);
                        ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                        ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao", produto.CategoriaID);
                        ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", produto.EscolaID);
                        return RedirectToAction(nameof(Index));
                    }
                }

                Alert("Produto cadastrado com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (produto.PrecisaAprovacao)
                    await EnviaEmail("paulo.silva@epta.com.br", $"Produto {produto.Codigo} criado por não Nutricionista",
                        $"Usuário {Thread.CurrentPrincipal.Identity.Name} " +
                        $"criou um produto sem as informações nutricionais ");

                _context.Add(produto);

                await _context.SaveChangesAsync();

                Alert("Produto cadastrado com Sucesso!", NotificationType.success);
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ValidaProduto(Produto produto, bool Edit = false)
        {
            if (Edit)
                return !_context.Produto.Any(x => x.Codigo == produto.Codigo && x.ProdutoID != produto.ProdutoID && x.EscolaID == produto.EscolaID);
            return !_context.Produto.Any(x => x.Codigo == produto.Codigo);
        }

        private void UpdateDadosNutricionais(ref Produto produto)
        {
            var lista = produto.DadosNutricionais;
            var i = 0;
            if (produto.DadosNutricionais != null)
                produto.DadosNutricionais.ToList().ForEach(x =>
                {
                    if (x.DadoNutricionalID == 0 && x.Deletado)
                        lista.RemoveAt(i);
                    i++;
                });
            produto.DadosNutricionais = lista;
        }

        // GET: Produto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Categoria = new SelectList(_context.Categoria, "CategoriaID", "Descricao");
            ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");

            var produto = await _context.Produto.Include("DadosNutricionais")
                                    .Include("Categoria")
                                    .Include("DadosNutricionais.TipoNutricional")
                                    .SingleOrDefaultAsync(m => m.ProdutoID == id);
            if (produto == null)
            {
                return NotFound();
            }

            produto.UltimaAtualizacao = RetiraMiliseconds(produto.UltimaAtualizacao);
            produto.DataCadastro = produto.DataCadastro;
            return View(produto);
        }

        private DateTime RetiraMiliseconds(DateTime data)
        {
            return data.AddTicks(-(data.Ticks % TimeSpan.TicksPerSecond));
        }

        // POST: Produto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProdutoID, Produto produto)
        {
            if (ProdutoID != produto.ProdutoID)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(produto.CodigoBarras))
            {
                if (!CodigoBarrasExists(produto.CodigoBarras))
                {
                    Alert($"Código de Barras {produto.CodigoBarras} já cadastrado no sistema!", NotificationType.info);
                    ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                    ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
                    return View(produto);
                }
            }

            if (!ValidaProduto(produto, true))
            {
                Alert($"Código {produto.Codigo} já cadastrado no sistema!", NotificationType.info);
                ViewBag.TipoNutricional = new SelectList(_context.TipoNutricional, "TipoNutricionalID", "Nome");
                ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome");
                return View(produto);
            }
            UpdateDadosNutricionais(ref produto);
            produto.UltimaAtualizacao = RetiraMiliseconds(DateTime.Now);
            try
            {
                // REMOVE ITENS DADOS NUTRICIONAIS DA TABELA
                _context.DadoNutricional.RemoveRange(produto.DadosNutricionais.Where(x => x.Deletado));
                //VERIFICA E INSERE OS DADOS ADICIONADOS
                var dadosNovos = produto.DadosNutricionais.Where(x => !x.Deletado && x.DadoNutricionalID == 0).ToList();
                dadosNovos.ForEach(x => x.ProdutoID = produto.ProdutoID);
                _context.DadoNutricional.AddRange(dadosNovos);
                //LIMPA LISTA PARA UPDATE
                produto.DadosNutricionais = null;
                produto.Categoria = _context.Categoria.FirstOrDefault(x => x.CategoriaID == produto.Categoria.CategoriaID);
                produto.CategoriaID = produto.Categoria.CategoriaID;
                _context.Update(produto);
                if (produto.PrecisaAprovacao)
                    await EnviaEmail("paulo.silva@epta.com.br", $"Produto {produto.Codigo} criado por usuário não Nutricionista", $"Usuário {"NOMEUSUARIO"} " +
                        $"editou um produto sem as informações nutricionais ");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(produto.ProdutoID))
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

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //ViewData["EscolaID"] = new SelectList(_context.Escola, "EscolaID", "Nome");
            var produto = await _context.Produto
                .SingleOrDefaultAsync(m => m.ProdutoID == id);
            if (produto == null)
            {
                return NotFound();
            }

            ViewBag.Escola = new SelectList(_context.Escola, "EscolaID", "Nome", produto.EscolaID);

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProdutoID)
        {
            var pedido = _context.PedidoItem.Include(x => x.Produto).FirstOrDefault(p => p.ProdutoID == ProdutoID);
            var produto = await _context.Produto.SingleOrDefaultAsync(m => m.ProdutoID == ProdutoID);

            if (pedido != null)
            {
                Alert("Contém uma venda vinculado a esse produto, o mesmo não pode ser deletado.", NotificationType.warning);
                return View(produto);
            }

            _context.Produto.Remove(produto);

            await _context.SaveChangesAsync();

            Alert("Produto deletado com Sucesso!.", NotificationType.success);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public JsonResult AutoComplete(string dado, int EscolaID)
        {
            var settings = new JsonSerializerSettings();
            var ObjList = new List<Produto>();

            if (EscolaID == 0)
            {
                var ProdutoList = (from N in _context.Produto
                                   where N.Codigo.ToLower().Contains(dado.ToLower()) || N.Descricao.ToLower().Contains(dado.ToLower())
                                   select new { N.ProdutoID, N.Codigo, N.Descricao });

                return Json(ProdutoList, settings);
            }
            else
            {
                var ProdutoList = (from N in _context.Produto
                                   where (N.Codigo.ToLower().Contains(dado.ToLower()) || N.Descricao.ToLower().Contains(dado.ToLower())) && N.EscolaID == EscolaID
                                   select new { N.ProdutoID, N.Codigo, N.Descricao });

                return Json(ProdutoList, settings);
            }
        }

        private Escola CarregaEscola(Produto Produto)
        {
            if (Produto.Escola != null)
                return _context.Escola.FirstOrDefault(x => x.EscolaID == Produto.Escola.EscolaID);
            else
                return _context.Escola.FirstOrDefault(x => x.EscolaID == Produto.EscolaID);
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.ProdutoID == id);
        }

        private bool CodigoBarrasExists(string codigo)
        {
            return _context.Produto.Any(e => e.CodigoBarras == codigo);
        }

        private async Task EnviaEmail(string email, string assunto, string mensagem)
        {
            await _emailSender.SendEmailAsync(email, assunto, mensagem);
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "ProdutoController");
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
