using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Cielo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalEducacional.Data;
using PortalEducacional.Models;
using PortalEducacional.Models.ViewModel;
using PortalEducacional.Services;
using static PortalEducacional.Enum.Enums;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Cantina", DescricaoTela = "Compra de Créditos", Controller = "CompraCreditoController", Acao = "Bloqueado;Editar")]
    public class CompraCreditoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private CardBrand _bandeira;
        private ICieloApi api;

        public CompraCreditoController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Cantina/Index
        public async Task<IActionResult> Index()
        {
            if (!VerificaPermissaoTela())
            {
                Alert("Você não possui acesso a essa Tela. Entre em contato com o seu Administrador!", NotificationType.error);
                return Redirect(Url.Content("~/Home/Index"));
            }
            else
                return RedirectToAction("Create");
        }

        // GET: Cantina/Create
        [HttpGet]
        public async Task<IActionResult> Create(CompraCreditoViewModel compra, int alunoID)
        {
            CarregaAno();
            var AlunoID = 0;

            if (compra.Aluno == null)
                AlunoID = RetornaAlunoResponsavel();
            else
                AlunoID = compra.Aluno.AlunoID;

            compra.MaisAlunos = RetornaAlunosPorResponsavel();

            if (AlunoID == 0)
            {
                Alert("Não foi encontrado nenhum Aluno sobre sua reponsabilidade!", NotificationType.error);
                return Redirect(Url.Content("~/Home/Index"));
            }

            compra.Aluno = _context.Aluno.Include(x => x.Escola).Include(s => s.Serie).FirstOrDefault(x => x.AlunoID == AlunoID);

            compra.SaldoDisponivel = compra.Aluno.SaldoDisponivel;
            compra.TotalCredito = _context.PedidoVendaCredito.Where(x => x.AlunoID == compra.Aluno.AlunoID).Sum(x => x.Valor);

            if (compra.Aluno.Foto != null)
            {
                var img = Convert.ToBase64String(compra.Aluno.Foto);
                compra.Imagem = img;
            }

            return View(compra);
        }

        // POST: Cantina/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompraCreditoViewModel compra)
        {
            if (string.IsNullOrEmpty(compra.FaturamentoCartao.Valor.ToString()) || compra.FaturamentoCartao.Valor == 0)
            {
                Alert("Preencha o campo Valor de Crédito corretamente!", NotificationType.error);
                return View(compra);
            }

            try
            {
                var status = RealizaTransacao(compra);

                if (status != null && status.Equals("PaymentConfirmed"))
                {
                    await GravaTransacao(compra);

                    await GravaPedidoCredito(compra);

                    await AtualizaSaldoAluno(compra);

                    await DeletaProdutosAluno(compra);

                    if (compra.ListaProdutosAluno != null)
                        await GravaProdutosAluno(compra);

                    await EnvioEmail(compra);
                }
                else
                {
                    CarregaAno();
                    return View(compra);
                }
            }
            catch (Exception)
            {
                Alert("Erro ao gravar a Transação. Entre em contato com o Suporte Técnico!", NotificationType.error);
                CarregaAno();
                return View(compra);
            }

            Alert("Pedido Realizado com Sucesso!", NotificationType.success);
            return RedirectToAction("Index", "CompraCredito");
        }

        protected string RealizaTransacao(CompraCreditoViewModel compra)
        {
            var id = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);

            //API CIELO
            var Vencimento = compra.FaturamentoCartao.Mes + "/" + compra.FaturamentoCartao.Ano;
            var customer = new Customer(name: usuario.Nome);                                                // Nome do Comprador
            var valor = Convert.ToDecimal(Math.Round(compra.FaturamentoCartao.Valor * 100));                // Valor em centavos
            api = new CieloApi(CieloEnvironment.Sandbox, Merchant.Sandbox);
            _bandeira = new CardBrand();

            _bandeira = VerificaBandeira(compra);

            try
            {
                var creditCard = new CreditCard(
                    cardNumber: compra.FaturamentoCartao.NumeroCartao,
                    holder: compra.FaturamentoCartao.TitularCartao,
                    expirationDate: Convert.ToDateTime(Vencimento),
                    securityCode: compra.FaturamentoCartao.CVV,
                    brand: _bandeira);

                var payment = new Payment(
                    amount: valor,                                                                                  // Valor do pedido em centavos
                    currency: Currency.BRL,                                                                         // Tipo da moeda
                    installments: 1,                                                                                // Número de parcelas
                    capture: true,                                                                                  // Booleano que identifica que a autorização deve ser com captura automática
                    softDescriptor: "Educacional",                                                                  // Texto que será exibido na fatura do cartão. max. 13 letras (Exclusivo VISA e MASTER)
                    creditCard: creditCard);

                var merchantOrderId = new Random().Next();                                                          // Número do Pedido

                var transaction = new Transaction(
                    merchantOrderId: merchantOrderId.ToString(),
                    customer: customer,
                    payment: payment);

                var returnTransaction = api.CreateTransaction(Guid.NewGuid(), transaction);

                compra.FaturamentoCartao.PaymentID = returnTransaction.Payment.PaymentId.Value.ToString();

                if (returnTransaction.Payment.Status != Status.PaymentConfirmed)
                {
                    var mensagem = RetornaMensagemStatus(returnTransaction.Payment.Status.ToString());

                    Alert(string.Format("Transação não autorizada! Motivo: {0}! Tente novamente ou entre em contato com sua administradora!", mensagem), NotificationType.error);
                    return null;
                }

                return returnTransaction.Payment.Status.ToString();
            }
            catch (Exception ex)
            {
                Alert("Erro ao finalizar Transão no cartão de crédito. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return null;
            }
        }

        public async Task GravaTransacao(CompraCreditoViewModel compra)
        {
            try
            {
                var id = User.Claims.Select(x => x.Value).FirstOrDefault();

                var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);
                compra.ApplicationUser = usuario;

                var faturamento = new FaturamentoCartao
                {
                    Bandeira = _bandeira.ToString(),
                    DataTransacao = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                    DataVencimento = compra.FaturamentoCartao.Mes + "/" + compra.FaturamentoCartao.Ano,
                    NumeroCartao = compra.FaturamentoCartao.NumeroCartao,
                    ApplicationUser = usuario,
                    ApplicationUserID = usuario.Id,
                    TitularCartao = compra.FaturamentoCartao.TitularCartao,
                    Valor = compra.FaturamentoCartao.Valor,
                    PaymentID = compra.FaturamentoCartao.PaymentID
                };

                _context.Add(faturamento);

                await _context.SaveChangesAsync();
                compra.FaturamentoCartao = faturamento;
                compra.FaturamentoCartao.FaturamentoCartaoID = faturamento.FaturamentoCartaoID;
            }
            catch (Exception)
            {
                Alert("Erro ao gravar Faturamento Cartão. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return;
            }
        }

        public async Task GravaProdutosAluno(CompraCreditoViewModel compra)
        {
            try
            {
                var listaProds = compra.ListaProdutosAluno.Where(x => !x.Deletado);

                foreach (var item in listaProds)
                {
                    var alunoProdutos = new AlunoProdutos
                    {
                        Aluno = compra.Aluno,
                        AlunoID = compra.Aluno.AlunoID,
                        Produto = _context.Produto.FirstOrDefault(x => x.ProdutoID == item.ProdutoID),
                        ProdutoID = item.ProdutoID
                    };

                    _context.Add(alunoProdutos);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                Alert("Erro ao gravar os Produtos ao Aluno. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return;
            }
        }

        public async Task DeletaProdutosAluno(CompraCreditoViewModel compra)
        {
            try
            {
                if (compra.ListaProdutosAluno.Count > 0)
                {
                    foreach (var item in compra.ListaProdutosAluno)
                    {
                        var produtosAluno = new AlunoProdutos
                        {
                            Aluno = compra.Aluno,
                            AlunoID = compra.Aluno.AlunoID,
                            AlunoProdutosID = item.AlunoProdutosID,
                            Produto = _context.Produto.FirstOrDefault(x => x.ProdutoID == item.ProdutoID),
                            ProdutoID = item.ProdutoID
                        };

                        if (produtosAluno.AlunoProdutosID > 0)
                        {
                            _context.Remove(produtosAluno);

                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("Erro ao deletar os Produtos do Aluno. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return;
            }
        }

        public async Task GravaPedidoCredito(CompraCreditoViewModel compra)
        {
            try
            {
                var id = User.Claims.Select(x => x.Value).FirstOrDefault();

                var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);
                compra.ApplicationUser = usuario;

                var numero = GeraNumeroPedido(compra);

                var pedido = new PedidoVendaCredito
                {
                    FaturamentoCartao = compra.FaturamentoCartao,
                    FaturamentoCartaoID = compra.FaturamentoCartao.FaturamentoCartaoID,
                    Data = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                    Numero = numero,
                    ApplicationUser = usuario,
                    ApplicationUserID = usuario.Id,
                    Valor = compra.FaturamentoCartao.Valor,
                    Aluno = _context.Aluno.FirstOrDefault(x => x.AlunoID == compra.Aluno.AlunoID),
                    AlunoID = compra.Aluno.AlunoID
                };

                _context.Add(pedido);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                Alert("Erro ao gravar Pedido Crédito. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return;
            }
        }

        public async Task AtualizaSaldoAluno(CompraCreditoViewModel compra)
        {
            try
            {
                var Aluno = _context.Aluno.Include(e => e.Escola).Include(s => s.Serie).FirstOrDefault(x => x.AlunoID == compra.Aluno.AlunoID);
                Aluno.ResposavelFinanceiro = _context.Responsavel.FirstOrDefault(x => x.ResponsavelID == Aluno.ResposavelFinanceiroID);
                Aluno.ResposavelLegal = _context.Responsavel.FirstOrDefault(x => x.ResponsavelID == Aluno.ResposavelLegalID);
                Aluno.SaldoDisponivel = (Aluno.SaldoDisponivel == null ? 0 : Aluno.SaldoDisponivel) + compra.FaturamentoCartao.Valor;

                _context.Update(Aluno);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Alert("Erro ao atualizar Saldo Crédito ao Aluno. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return;
            }
        }

        public async Task EnvioEmail(CompraCreditoViewModel compra)
        {
            try
            {
                var id = User.Claims.Select(x => x.Value).FirstOrDefault();

                var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);

                await _emailSender.SendEmailAsync(usuario.Email, "Confirmação de E-mail", "Prezado, <br /><br /> Pedido" + $"'{usuario.Email}'" + "Realizado com Sucesso!<br /><br /> Atenciosamente, <br /> My Bonus");
            }
            catch (Exception)
            {
                Alert("Erro ao enviar E-mail. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return;
            }
        }

        private string GeraNumeroPedido(CompraCreditoViewModel compra)
        {
            var ultimoPedido = _context.PedidoVendaCredito.Count(x => x.ApplicationUserID == compra.ApplicationUser.Id);

            var pedido = "";

            if (ultimoPedido == 0)
                pedido = "PV-" + ("1").PadLeft(5, '0');
            else
                pedido = "PV-" + (ultimoPedido + 1).ToString().PadLeft(5, '0');

            return pedido;
        }

        private int RetornaAlunoResponsavel()
        {
            var alunoID = 0;

            var id = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);

            var responsavel = _context.Responsavel.Where(x => x.Email == usuario.Email);

            if (responsavel == null)
            {
                Alert("Ocorreu um erro ao localizar o responsável! Entre em contato com o Suporte Técnico!", NotificationType.error);
                return 0;
            }

            foreach (var item in responsavel)
            {
                var aluno = _context.Aluno.FirstOrDefault(x => x.ResposavelFinanceiroID == item.ResponsavelID || x.ResposavelLegalID == item.ResponsavelID);

                if (aluno != null)
                {
                    alunoID = aluno.AlunoID;
                    break;
                }
            }

            return alunoID;
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 5000, button: false});" + "</script>";
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "CompraCreditoController");
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

        private void CarregaAno()
        {
            var listAno = new List<ListItem>();
            listAno.Add(new ListItem("Ano", "0"));

            var ano = string.Empty;

            for (int i = 0; i <= 10; i++)
            {
                if (i == 0)
                    ano = DateTime.Now.Year.ToString();
                else
                    ano = DateTime.Now.AddYears(i).Year.ToString();

                listAno.Add(new ListItem(ano, ano));
            }

            var selectList = new List<SelectListItem>();

            foreach (var item in listAno)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text
                });
            }

            ViewBag.AnoVencimento = selectList;
        }

        protected CardBrand VerificaBandeira(CompraCreditoViewModel compra)
        {
            if (compra.BandeiraID.Equals("0"))
                _bandeira = CardBrand.Visa;
            else if (compra.BandeiraID.Equals("1"))
                _bandeira = CardBrand.Master;
            else if (compra.BandeiraID.Equals("2"))
                _bandeira = CardBrand.Amex;
            else if (compra.BandeiraID.Equals("3"))
                _bandeira = CardBrand.Elo;
            else if (compra.BandeiraID.Equals("4"))
                _bandeira = CardBrand.Aura;
            else if (compra.BandeiraID.Equals("5"))
                _bandeira = CardBrand.JCB;
            else if (compra.BandeiraID.Equals("6"))
                _bandeira = CardBrand.Diners;
            else if (compra.BandeiraID.Equals("7"))
                _bandeira = CardBrand.Discover;

            return _bandeira;
        }

        protected static string RetornaMensagemStatus(string status)
        {
            var mensagem = string.Empty;

            if (status.Equals("NotFinished"))
                mensagem = "Aguardando atualização de status";
            else if (status.Equals("Authorized"))
                mensagem = "Pagamento Autorizado.";
            else if (status.Equals("Denied"))
                mensagem = "Pagamento negado por Autorizador";
            else if (status.Equals("Voided"))
                mensagem = "Pagamento cancelado";
            else if (status.Equals("Refunded"))
                mensagem = "Pagamento cancelado após 23:59 do dia de autorização";
            else if (status.Equals("Pending"))
                mensagem = "Aguardando Status de instituição financeira";
            else if (status.Equals("Aborted"))
                mensagem = "Pagamento cancelado por falha no processamento";
            else if (status.Equals("Scheduled"))
                mensagem = "Recorrência agendada";
            else
                mensagem = "Erro ao processar pagamento";

            return mensagem;
        }

        private bool RetornaAlunosPorResponsavel()
        {
            var id = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);

            var responsavel = _context.Responsavel.FirstOrDefault(x => x.Email == usuario.Email);

            if (responsavel == null)
                return false;

            var aluno = _context.Aluno.Where(x => x.ResposavelFinanceiroID == responsavel.ResponsavelID || x.ResposavelLegalID == responsavel.ResponsavelID);

            ViewBag.Aluno = new SelectList(aluno, "AlunoID", "Nome");

            return aluno.Count() > 1 ? true : false;
        }

        [HttpPost]
        public JsonResult RetornaProdutos(string dado)
        {
            var id = int.Parse(dado);

            var settings = new JsonSerializerSettings();

            var ProdList = (from p in _context.Produto
                            where p.Escola.EscolaID == id
                            select new { p.ProdutoID, p.Codigo, p.Descricao, p.Valor }).Distinct();

            return Json(ProdList, settings);
        }

        [HttpPost]
        public JsonResult RetornaCategoria(string dado)
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();
            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var settings = new JsonSerializerSettings();

            var CategoriaList = (from c in _context.Categoria
                                 join p in _context.Produto on c.CategoriaID equals p.CategoriaID
                                 where (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID)
                                 select new { c.CategoriaID, c.Descricao }).Distinct();

            return Json(CategoriaList, settings);
        }

        [HttpPost]
        public JsonResult RetornaProdutosAluno(string dado)
        {
            var id = int.Parse(dado);

            var settings = new JsonSerializerSettings();

            var ProdList = (from a in _context.AlunoProdutos
                            join p in _context.Produto on a.ProdutoID equals p.ProdutoID
                            where a.AlunoID == id
                            select new { a.AlunoProdutosID, p.ProdutoID, p.Codigo, p.Descricao }).Distinct();

            return Json(ProdList, settings);
        }

        [HttpPost]
        public JsonResult RetornaDadosProdutoCategoria(string dado)
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();
            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var settings = new JsonSerializerSettings();
            var id = int.Parse(dado);

            var ProdList = (from p in _context.Produto
                            where (0 == id || p.CategoriaID == id || p.Categoria.CategoriaID == id) && (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID)
                            select new { p.ProdutoID, p.Codigo, p.Descricao, p.Valor });

            return Json(ProdList, settings);
        }
    }
}