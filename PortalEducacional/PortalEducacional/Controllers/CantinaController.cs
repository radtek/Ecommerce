using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalEducacional.Data;
using PortalEducacional.Models.ViewModel;
using PortalEducacional.Models;
using static PortalEducacional.Enum.Enums;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalEducacional.Services;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Cantina Web", Menu = "Cantina", DescricaoTela = "Cantina Web - Vendas", Controller = "CantinaController", Acao = "Bloqueado;Editar")]
    public class CantinaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public CantinaController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Cantina
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

            return View();
        }

        // GET: Cantina/Create
        [HttpGet]
        public async Task<IActionResult> Create(string ID, string identificacaoTipo, CantinaViewModel CantinaViewModel)
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            if (ID == null && identificacaoTipo == null && CantinaViewModel.ClienteID == null)
            {
                Alert("Informe um cliente para realizar iniciar a venda.", NotificationType.warning);
                return RedirectToAction("Index", "Cantina");
            }

            if (ID == null && identificacaoTipo == null)
            {
                ID = CantinaViewModel.ClienteID;
                identificacaoTipo = CantinaViewModel.TipoCliente;
            }

            //VERIFICA SE O CAIXA DO DIA ANTERIOR FOI ENCERRADO
            var caixaAnterior = new CaixaFechamento();
            caixaAnterior = _context.CaixaFechamento.Include(x => x.Escola).FirstOrDefault(c => !c.Fechado && c.AspNetUsersID == usuario.Id && Convert.ToDateTime(c.DataCadastro).ToShortDateString() != DateTime.Now.ToShortDateString());

            if (caixaAnterior != null)
            {
                await RealizaFechamentoCaixa(CantinaViewModel, true, ID, identificacaoTipo);
            }

            //Verifica Caixa Aberto pro usuário/data em questão
            var caixaFechamento = _context.CaixaFechamento.FirstOrDefault(c => !c.Fechado && Convert.ToDateTime(c.DataCadastro).ToShortDateString() == DateTime.Now.ToShortDateString() && c.AspNetUsersID == usuario.Id);

            if (caixaFechamento == null)
            {
                Alert("Não há movimento ABERTO para o dia de hoje com o seu usuário. " +
                "Realize a abertura de Caixa para iniciar as vendas!", NotificationType.warning);

                return RedirectToAction("Index", "Cantina");
            }

            //VERIFICA CLIENTE
            var cantina = new CantinaViewModel();

            var ClienteID = ID == null ? 0 : int.Parse(ID);

            if (identificacaoTipo == "aluno")
            {
                cantina.Aluno = _context.Aluno.FirstOrDefault(x => x.AlunoID == ClienteID);

                if (cantina.Aluno != null)
                {
                    if (cantina.Aluno.Foto != null)
                    {
                        var img = Convert.ToBase64String(cantina.Aluno.Foto);
                        cantina.Imagem = img;
                    }

                    if (cantina.Aluno.SaldoDisponivel == null)
                        cantina.Aluno.SaldoDisponivel = 0;
                }
            }
            else
                cantina.Funcionario = _context.Funcionario.FirstOrDefault(x => x.FuncionarioID == ClienteID);

            return View(cantina);
        }

        // GET: Cantina/FechamentoCaixa
        public async Task<IActionResult> FechamentoCaixa(CantinaViewModel CantinaViewModel)
        {
            CantinaViewModel = AnalisaValoresMovimento(CantinaViewModel);

            if (CantinaViewModel == null)
                return View();

            return View(CantinaViewModel);
        }

        //GET: Cantina/VendasDia
        public async Task<IActionResult> VendasDia(CantinaViewModel CantinaViewModel)
        {
            CantinaViewModel = AnalisaValoresMovimento(CantinaViewModel);

            if (CantinaViewModel == null)
                return View();

            return View(CantinaViewModel);
        }

        // POST: Cantina/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CantinaViewModel cantina)
        {
            if (cantina.Aluno == null && cantina.Funcionario == null)
            {
                Alert("O cliente deve ser informado para a realização da venda.", NotificationType.info);
                return View(cantina);
            }

            //Verifica se o caixa do dia Anterior foi encerrado
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();

            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            if (cantina.PedidoItens == null)
            {
                Alert("Necessário informar pelo menos 1 item na venda.", NotificationType.info);
                return View(cantina);
            }

            if (usuario.Escola == null)
                cantina.Escola = _context.Escola.FirstOrDefault(x => x.EscolaID == usuario.EscolaID);
            else
                cantina.Escola = usuario.Escola;

            await GravaPedido(cantina);

            return RedirectToAction("Index", "Cantina");
        }

        //ABERTURA DE CAIXA
        public async Task<IActionResult> AberturaCaixa()
        {
            try
            {
                var user = User.Claims.Select(x => x.Value).FirstOrDefault();

                var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

                var caixaAnterior = new CaixaFechamento();

                var escola = _context.Escola.FirstOrDefault(e => e.EscolaID == usuario.EscolaID);

                caixaAnterior = _context.CaixaFechamento.Include(x => x.Escola).FirstOrDefault(c => !c.Fechado && c.AspNetUsersID == usuario.Id);

                if (caixaAnterior != null)
                {
                    Alert($"O movimento do dia { caixaAnterior.DataCadastro } ainda não foi encerrado. Realize o fechamento do caixa, logo após a abertura de um novo movimento.", NotificationType.warning);
                    return RedirectToAction("FechamentoCaixa", "Cantina");
                }

                //Verifica se já há um caixa aberto pro usuário logado
                var caixaAtual = _context.CaixaFechamento.Include(x => x.Escola).FirstOrDefault(c => Convert.ToDateTime(c.DataCadastro).ToShortDateString() == DateTime.Now.ToShortDateString() && c.AspNetUsersID == usuario.Id);

                if (caixaAtual != null)
                {
                    Alert($"Já contém um caixa ABERTO para o usuário { User.Identity.Name } no dia de hoje.", NotificationType.warning);
                    return RedirectToAction("Index", "Cantina");
                }

                //Verifica Ultima caixa Aberto
                var caixa = _context.CaixaFechamento.Include(x => x.Escola).OrderByDescending(x => x.CaixaFechamentoID).FirstOrDefault(c => c.AspNetUsersID == usuario.Id && c.Fechado);

                if (caixa == null)
                {
                    var caixaNew = new CaixaFechamento
                    {
                        AspNetUsersID = usuario.Id,
                        DataCadastro = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                        Escola = escola,
                        EscolaID = escola.EscolaID,
                        Fechado = false,
                        SaldoFinal = 0,
                        SaldoInicial = 0
                    };

                    caixa = caixaNew;
                }

                //Realiza a abertura de caixa
                await AberturaCaixa(caixa);

                Alert("Caixa aberto com Sucesso!", NotificationType.success);
                return RedirectToAction("Index", "Cantina");
            }
            catch (Exception)
            {
                Alert("Erro ao abrir o movimento do caixa, entre em contato com o Suporte técnico!", NotificationType.error);
                return View();
            }
        }

        //FECHAMENTO DE CAIXA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RealizaFechamentoCaixa(CantinaViewModel CantinaViewModel, bool boAutomativo = false, string ID = "", string identificacaoTipo = "")
        {
            CantinaViewModel = AnalisaValoresMovimento(CantinaViewModel);

            CantinaViewModel.CaixaFechamento.Fechado = true;
            CantinaViewModel.CaixaFechamento.SaldoFinal = double.Parse(CantinaViewModel.ValorTotalLiquido.Replace("R$", ""));

            _context.Update(CantinaViewModel.CaixaFechamento);

            await _context.SaveChangesAsync();

            if (boAutomativo)
                await Create(ID, identificacaoTipo, CantinaViewModel);

            Alert("Caixa encerrado com Sucesso!", NotificationType.success);

            return RedirectToAction("Index", "Cantina");
        }

        //GRAVA SANGRIA CAIXA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarSangria(CantinaViewModel cantina)
        {
            var usuario = User.Claims.Select(x => x.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(cantina.SangriaCaixa.Valor.ToString()) || string.IsNullOrEmpty(cantina.SangriaCaixa.Motivo.ToString()))
            {
                Alert("Os campo Valor e Motivo devem ser preenchidos!", NotificationType.error);
                return View(cantina);
            }
            else
            {
                var sangria = new SangriaCaixa
                {
                    AspNetUsersID = usuario,
                    DataCadastro = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(),
                    Motivo = cantina.SangriaCaixa.Motivo,
                    Valor = cantina.SangriaCaixa.Valor,
                };

                try
                {
                    _context.Add(sangria);

                    await _context.SaveChangesAsync();

                    // GRAVA MOVIMENTO CAIXA
                    cantina.SangriaCaixa = sangria;
                    var movimento = GeraMovimentoCaixa(cantina);

                    _context.Add(movimento);

                    await _context.SaveChangesAsync();

                    Alert("Sangria Salva com Sucesso!", NotificationType.success);
                    return RedirectToAction("Index", "Cantina");
                }
                catch (Exception)
                {
                    Alert("Erro ao gravar Sangria. Entre em contato com o Suporte Técnico!", NotificationType.error);
                    return View(cantina);
                }
            }
        }

        //FINALIZAR COMPRA
        public async Task<IActionResult> GravaPedido(CantinaViewModel cantina)
        {
            try
            {
                //VERIFICA SALDO DISPONÍVEL
                var mensagem = VerificaSaldoItens(cantina);

                if (mensagem == "")
                {
                    //GRAVA PEDIDO
                    var pedido = MontaPedidoVenda(cantina);
                    cantina.Pedido = pedido;

                    if (pedido == null)
                    {
                        Alert("O saldo disponível do cliente não é suficiente para a compra.", NotificationType.error);
                        return View(cantina);
                    }

                    _context.Add(pedido);

                    await _context.SaveChangesAsync();

                    //ATUALIZA PONTEIRO PEDIDO
                    await AtualizaPonteiroPedido(cantina.Escola);

                    // GRAVA MOVIMENTO CAIXA
                    var movimento = GeraMovimentoCaixa(cantina);
                    cantina.CaixaMovimento = movimento;

                    _context.Add(movimento);

                    await _context.SaveChangesAsync();

                    //GRAVA PEDIDO ITENS
                    await GravaPedidoItem(cantina);

                    //ATUALIZA SALDO ALUNO
                    if (cantina.Pedido.FormaRecebimento == "credito")
                    {
                        if (cantina.Aluno != null && cantina.Aluno.AlunoID != 0)
                            await AtualizaSaldoAluno(cantina);
                    }

                    Alert($"Pedido { cantina.Pedido.Numero } Finalizado com Sucesso!", NotificationType.success);
                    return RedirectToAction("Index", "Cantina");
                }
                else
                {
                    Alert($"Os produtos { mensagem } não contém saldo suficiente para essa venda!", NotificationType.error);
                    return View(cantina);
                }
            }
            catch (Exception ex)
            {
                Alert("Erro ao gravar Pedido de Venda. Entre em contato com o Suporte Técnico!", NotificationType.error);
                return View(cantina);
            }
        }

        //GET: Cantina/SaidaEstoque
        public async Task<IActionResult> SaidaEstoque()
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

        //POST: Cantina/SaidaEstoque
        [HttpPost]
        public async Task<IActionResult> SaidaEstoque(EstoqueEstoqueEmpresaViewModel EstoqueViewModel)
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

            return RedirectToAction("SaidaEstoque", "Cantina");
        }

        //POST: Cantina/ReabrirCaixa
        [HttpPost]
        public async Task<IActionResult> ReabrirCaixa(CantinaViewModel cantina)
        {
            try
            {
                var user = User.Claims.Select(x => x.Value).FirstOrDefault();

                var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

                var escola = _context.Escola.FirstOrDefault(e => e.EscolaID == usuario.EscolaID);

                if (cantina.CaixaFechamento != null)
                {
                    var caixaNew = new CaixaFechamento
                    {
                        CaixaFechamentoID = cantina.CaixaFechamento.CaixaFechamentoID,
                        AspNetUsersID = usuario.Id,
                        DataCadastro = cantina.CaixaFechamento.DataCadastro,
                        Escola = escola,
                        EscolaID = escola.EscolaID,
                        Fechado = false,
                        SaldoFinal = cantina.CaixaFechamento.SaldoFinal,
                        SaldoInicial = cantina.CaixaFechamento.SaldoInicial
                    };

                    _context.Update(caixaNew);
                    await _context.SaveChangesAsync();

                    Alert("Caixa reaberto com Sucesso!", NotificationType.success);
                    return RedirectToAction("Index", "Cantina");
                }
                else
                {
                    Alert("Erro ao reabrir o caixa, entre em contato com o Suporte Técnico!", NotificationType.error);
                    return RedirectToAction("Index", "Cantina");
                }

            }
            catch (Exception ex)
            {
                Alert("Erro ao reabrir o movimento do caixa, entre em contato com o Suporte técnico!", NotificationType.error);
                return View();
            }
        }

        #region Controle Estoque

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

        private Escola CarregaEscola(Estoque estoque)
        {
            var id = estoque.EscolaID == 0 ? estoque.Escola.EscolaID : estoque.EscolaID;

            return _context.Escola.FirstOrDefault(x => x.EscolaID == id);
        }

        private Produto CarregaProduto(Estoque estoque)
        {
            return _context.Produto.FirstOrDefault(x => x.ProdutoID == estoque.Produto.ProdutoID);
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

        private async Task EnviaEmail(string email, string assunto, string mensagem)
        {
            await _emailSender.SendEmailAsync(email, assunto, mensagem);
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

        #endregion

        private Pedido MontaPedidoVenda(CantinaViewModel cantina)
        {
            double valorBruto = 0;
            double valorTotal = 0;
            double desconto = 0;

            var numero = GeraNumeroPedido(cantina.Escola);

            var listaItens = cantina.PedidoItens.Where(x => !x.Deletado);

            foreach (var item in listaItens)
            {
                valorBruto = valorBruto + item.Valor;
                desconto = desconto + item.Desconto;
            }

            valorTotal = valorBruto - desconto;

            if (cantina.Pedido.FormaRecebimento == "credito")
            {
                if (cantina.Aluno.AlunoID != 0)
                {
                    //VERIFICA SALDO CLIENTE COM VALOR TOTAL PEDIDO
                    var saldo = VerificaSaldoCliente(cantina, valorTotal);

                    if (!saldo)
                        return null;
                }
            }

            var pedido = new Pedido
            {
                Aluno = cantina.Aluno != null ? _context.Aluno.FirstOrDefault(x => x.AlunoID == cantina.Aluno.AlunoID) : cantina.Aluno,
                AlunoID = cantina.Aluno == null ? 0 : cantina.Aluno.AlunoID,
                DataCadastro = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(),
                Desconto = desconto,
                Escola = cantina.Escola,
                EscolaID = cantina.Escola.EscolaID,
                Funcionario = cantina.Funcionario != null ? _context.Funcionario.FirstOrDefault(x => x.FuncionarioID == cantina.Funcionario.FuncionarioID) : cantina.Funcionario,
                FuncionarioID = cantina.Funcionario == null ? 0 : cantina.Funcionario.FuncionarioID,
                Numero = numero,
                ValorBruto = valorBruto,
                ValorTotal = valorTotal,
                AspNetUsersID = User.Claims.Select(x => x.Value).FirstOrDefault(),
                FormaRecebimento = cantina.Pedido.FormaRecebimento
            };

            if (pedido.Aluno == null || pedido.Aluno.AlunoID == 0)
            {
                pedido.Aluno = null;
                pedido.AlunoID = null;
            }
            if (pedido.Funcionario == null || pedido.Funcionario.FuncionarioID == 0)
            {
                pedido.Funcionario = null;
                pedido.FuncionarioID = null;
            }

            return pedido;
        }

        private bool VerificaSaldoCliente(CantinaViewModel cantina, double valorTotalPedido)
        {
            var result = true;

            var aluno = _context.Aluno.FirstOrDefault(x => x.AlunoID == cantina.Aluno.AlunoID);

            if (aluno == null)
                result = false;
            else
            {
                var saldo = aluno.SaldoDisponivel;

                if (valorTotalPedido > Convert.ToDouble(saldo))
                    result = false;
            }

            return result;
        }

        private string GeraNumeroPedido(Escola escola)
        {
            var ultimoPedido = _context.SetupEscola.FirstOrDefault(x => x.EscolaID == escola.EscolaID);

            var pedido = "";

            if (ultimoPedido == null)
                pedido = "PV-" + escola.EscolaID.ToString().PadLeft(4, '0') + "-" + ("1").PadLeft(5, '0');
            else
                pedido = "PV-" + escola.EscolaID.ToString().PadLeft(4, '0') + "-" + (ultimoPedido.PonteiroPedido + 1).ToString().PadLeft(5, '0');

            return pedido;
        }

        private async Task AtualizaPonteiroPedido(Escola escola)
        {
            try
            {
                var atualizaPont = _context.SetupEscola.FirstOrDefault(x => x.EscolaID == escola.EscolaID);

                if (atualizaPont == null)
                    atualizaPont = new SetupEscola();

                atualizaPont.SetupEscolaID = atualizaPont == null ? 0 : atualizaPont.SetupEscolaID;
                atualizaPont.PonteiroPedido = atualizaPont.PonteiroPedido == null ? 0 : atualizaPont.PonteiroPedido;

                atualizaPont.PonteiroPedido += 1;
                atualizaPont.Escola = escola;
                atualizaPont.EscolaID = escola.EscolaID;

                if (atualizaPont.SetupEscolaID == 0)
                    _context.Add(atualizaPont);
                else
                    _context.Update(atualizaPont);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                Alert("Erro ao atualizar o número de ponteiro!", NotificationType.error);
                throw;
            }
        }

        private async Task AtualizaSaldoAluno(CantinaViewModel cantina)
        {
            if (cantina.Aluno == null && cantina.Aluno.AlunoID == 0)
                return;
            else
            {
                var aluno = _context.Aluno.FirstOrDefault(x => x.AlunoID == cantina.Aluno.AlunoID);

                var saldo = aluno.SaldoDisponivel;

                aluno.SaldoDisponivel = saldo - Convert.ToDecimal(cantina.Pedido.ValorTotal);

                _context.Update(aluno);

                await _context.SaveChangesAsync();
            }
        }

        public async Task GravaPedidoItem(CantinaViewModel cantina)
        {
            try
            {
                var listaItens = cantina.PedidoItens.Where(x => !x.Deletado);

                foreach (var item in listaItens)
                {
                    var pedidoItem = new PedidoItem
                    {
                        Pedido = cantina.Pedido,
                        PedidoID = cantina.Pedido.PedidoID,
                        Desconto = item.Desconto,
                        Produto = _context.Produto.Include(e => e.Escola).Include(c => c.Categoria).FirstOrDefault(x => x.ProdutoID == item.Produto.ProdutoID),
                        ProdutoID = item.Produto.ProdutoID,
                        Quantidade = item.Quantidade,
                        Valor = item.Valor
                    };

                    _context.Add(pedidoItem);

                    await _context.SaveChangesAsync();

                    await ReduzSaldoItem(pedidoItem.Produto, pedidoItem.Quantidade);
                }
            }
            catch (Exception)
            {
                Alert("Erro ao gravar Itens do Pedido de Venda. Entre em contato com o Suporte Técnico!", NotificationType.error);
                throw;
            }
        }

        public CaixaMovimento GeraMovimentoCaixa(CantinaViewModel cantina)
        {
            if (cantina.CaixaFechamento == null)
            {
                var usuario = User.Claims.Select(x => x.Value).FirstOrDefault();

                cantina.CaixaFechamento = _context.CaixaFechamento.Include(e => e.Escola).FirstOrDefault(c => !c.Fechado && Convert.ToDateTime(c.DataCadastro).ToShortDateString() == DateTime.Now.ToShortDateString() && c.AspNetUsersID == usuario);
            }

            try
            {
                var caixamov = new CaixaMovimento
                {
                    AspNetUsersID = User.Claims.Select(x => x.Value).FirstOrDefault(),
                    DataCadastro = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(),
                    Pedido = cantina.Pedido,
                    PedidoID = cantina.Pedido == null ? 0 : cantina.Pedido.PedidoID,
                    Valor = cantina.Pedido == null ? cantina.SangriaCaixa.Valor : cantina.Pedido.ValorTotal,
                    SangriaCaixa = cantina.SangriaCaixa,
                    SangriaCaixaID = cantina.SangriaCaixa == null ? 0 : cantina.SangriaCaixa.SangriaCaixaID,
                    CaixaFechamento = cantina.CaixaFechamento,
                    CaixaFechamentoID = cantina.CaixaFechamento.CaixaFechamentoID
                };

                if (caixamov.SangriaCaixa == null)
                    caixamov.SangriaCaixaID = null;
                if (caixamov.Pedido == null)
                    caixamov.PedidoID = null;

                return caixamov;
            }
            catch (Exception)
            {
                Alert("Erro ao gravar Movimento do Caixa. Entre em contato com o Suporte Técnico!", NotificationType.error);
                throw;
            }
        }

        private async Task AberturaCaixa(CaixaFechamento caixaAnterior)
        {
            var caixa = PopularCaixaFechamento(caixaAnterior);

            _context.Add(caixa);

            await _context.SaveChangesAsync();
        }

        private CantinaViewModel AnalisaValoresMovimento(CantinaViewModel CantinaViewModel)
        {
            var lstMovimento = new List<CaixaMovimento>();
            CantinaViewModel.ListaCaixaMovimento = lstMovimento;

            var lstPedidoItem = new List<PedidoItem>();
            CantinaViewModel.PedidoItens = lstPedidoItem;

            //CONSULTA CAIXA ABERTO
            var usuario = User.Claims.Select(x => x.Value).FirstOrDefault();

            var caixaFechamento = _context.CaixaFechamento.FirstOrDefault(c => !c.Fechado && c.AspNetUsersID == usuario);

            if (caixaFechamento == null)
            {
                //PEGA O CAIXA JÁ ENCERRADO
                caixaFechamento = _context.CaixaFechamento.Where(c => c.Fechado && c.AspNetUsersID == usuario).OrderByDescending(x => x.DataCadastro).FirstOrDefault();
            }

            if (caixaFechamento == null)
                return null;

            //CONSULTA MOVIMENTO
            var caixaMovimento = _context.CaixaMovimento.Include(p => p.Pedido).Include(a => a.Pedido.Aluno).Include(f => f.Pedido.Funcionario).Include(s => s.SangriaCaixa).Where(x => x.CaixaFechamentoID == caixaFechamento.CaixaFechamentoID);

            if (caixaMovimento == null)
                return null;

            lstMovimento = caixaMovimento.ToList();

            foreach (var item in lstMovimento)
            {
                if (item.Pedido != null)
                {
                    lstPedidoItem = _context.PedidoItem.Include(p => p.Produto).Where(x => x.PedidoID == item.Pedido.PedidoID).ToList();

                    item.Pedido.ListaItens = lstPedidoItem;
                }
            }

            CantinaViewModel.ListaCaixaMovimento = lstMovimento;

            //Contabiliza Totais
            var Total = lstMovimento.Where(x => x.Pedido != null).Sum(x => x.Valor);
            double TotalDesconto = 0;

            CantinaViewModel.ValorTotalBruto = Total.ToString("C");
            CantinaViewModel.ValorTotalLiquido = (Total - TotalDesconto).ToString("C");
            CantinaViewModel.ValorTotalDesconto = (TotalDesconto).ToString("C");
            CantinaViewModel.QtdePedidos = lstMovimento.Count(x => x.Pedido != null);
            CantinaViewModel.ValorTotalSangria = lstMovimento.Where(x => x.SangriaCaixa != null).Sum(x => x.Valor).ToString("C");
            CantinaViewModel.QtdeSangrias = lstMovimento.Count(x => x.SangriaCaixa != null);
            CantinaViewModel.CaixaFechamento = caixaFechamento;
            CantinaViewModel.SaldoCaixaAnterior = caixaFechamento.SaldoInicial.ToString("C");

            return CantinaViewModel;
        }

        private string VerificaSaldoItens(CantinaViewModel CantinaViewModel)
        {
            var prod = "";

            var i = 0;
            foreach (var item in CantinaViewModel.PedidoItens)
            {
                double saldoAtual = 0;
                double qtdeVenda = 0;

                var saldo = _context.EstoqueEmpresa.FirstOrDefault(x => x.ProdutoID == item.Produto.ProdutoID);

                saldoAtual = saldo == null ? 0 : saldo.Saldo;
                qtdeVenda = item.Quantidade;

                if (qtdeVenda > saldoAtual)
                    if (prod == "")
                        prod = "'" + item.Produto.Codigo + " - " + item.Produto.Descricao + "'";
                    else
                        prod = prod + "'" + item.Produto.Codigo + " - " + item.Produto.Descricao + "'";
                i++;
            }

            return prod;
        }

        private async Task ReduzSaldoItem(Produto produto, double qtde)
        {
            try
            {
                var estoque = new EstoqueEmpresa();
                estoque = _context.EstoqueEmpresa.FirstOrDefault(x => x.ProdutoID == produto.ProdutoID);

                estoque.Saldo = estoque.Saldo - qtde;

                _context.Update(estoque);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 5000, button: false});" + "</script>";
            TempData["notification"] = msg;
        }

        [HttpPost]
        public JsonResult RetornaDadosProduto(string dado)
        {
            var settings = new JsonSerializerSettings();
            var id = int.Parse(dado);

            var ProdList = (from p in _context.Produto
                            where p.ProdutoID == id
                            select new { p.ProdutoID, p.Codigo, p.Descricao, p.Valor });

            return Json(ProdList, settings);
        }

        [HttpPost]
        public JsonResult RetornaDadosProdutoCategoria(string dado, string alunoID)
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();
            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var idAluno = 0;

            if (alunoID != null)
                idAluno = int.Parse(alunoID);

            var settings = new JsonSerializerSettings();

            if (idAluno != 0)
            {
                var produtosAluno = _context.AlunoProdutos.Count(x => x.AlunoID == idAluno);

                var id = int.Parse(dado);

                if (produtosAluno > 0)
                {
                    var ProdList = (from p in _context.Produto
                                    join c in _context.Categoria on p.CategoriaID equals c.CategoriaID
                                    join a in _context.AlunoProdutos on p.ProdutoID equals a.ProdutoID
                                    join e in _context.EstoqueEmpresa on p.ProdutoID equals e.ProdutoID
                                    where (0 == id || p.CategoriaID == id || p.Categoria.CategoriaID == id) && (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && e.Saldo > 0
                                    select new { p.ProdutoID, p.Codigo, p.Descricao, p.Valor, c.Imagem });

                    return Json(ProdList, settings);
                }
                else
                {
                    var ProdList = (from p in _context.Produto
                                    join c in _context.Categoria on p.CategoriaID equals c.CategoriaID
                                    join e in _context.EstoqueEmpresa on p.ProdutoID equals e.ProdutoID
                                    where (0 == id || p.CategoriaID == id || p.Categoria.CategoriaID == id) && (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && e.Saldo > 0
                                    select new { p.ProdutoID, p.Codigo, p.Descricao, p.Valor, c.Imagem });

                    return Json(ProdList, settings);
                }
            }
            else
            {
                var id = int.Parse(dado);

                var ProdList = (from p in _context.Produto
                                join c in _context.Categoria on p.CategoriaID equals c.CategoriaID
                                join e in _context.EstoqueEmpresa on p.ProdutoID equals e.ProdutoID
                                where (0 == id || p.CategoriaID == id || p.Categoria.CategoriaID == id) && (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && e.Saldo > 0
                                select new { p.ProdutoID, p.Codigo, p.Descricao, p.Valor, c.Imagem });

                return Json(ProdList, settings);
            }
        }

        [HttpPost]
        public JsonResult RetornaDadosProdutoCodigoBarras(string dado, string alunoID)
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();
            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);
            var id = 0;

            if (alunoID != null)
                id = int.Parse(alunoID);

            var settings = new JsonSerializerSettings();

            if (id != 0)
            {
                var ProdList = (from p in _context.Produto
                                join a in _context.AlunoProdutos on p.ProdutoID equals a.ProdutoID
                                where p.CodigoBarras == dado && (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && a.AlunoID == id
                                select new { p.ProdutoID });

                return Json(ProdList, settings);
            }
            else
            {
                var ProdList = (from p in _context.Produto
                                join a in _context.AlunoProdutos on p.ProdutoID equals a.ProdutoID
                                where p.CodigoBarras == dado && (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID)
                                select new { p.ProdutoID });

                return Json(ProdList, settings);
            }
        }

        [HttpPost]
        public JsonResult RetornaCategoria(string dado, string alunoID)
        {
            var id = 0;

            if (alunoID != null)
                id = int.Parse(alunoID);

            var user = User.Claims.Select(x => x.Value).FirstOrDefault();
            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var settings = new JsonSerializerSettings();

            if (id != 0)
            {
                var produtosAluno = _context.AlunoProdutos.Count(x => x.AlunoID == id);

                if (produtosAluno > 0)
                {
                    var CategoriaList = (from c in _context.Categoria
                                         join p in _context.Produto on c.CategoriaID equals p.CategoriaID
                                         join a in _context.AlunoProdutos on p.ProdutoID equals a.ProdutoID
                                         join e in _context.EstoqueEmpresa on p.ProdutoID equals e.ProdutoID
                                         where (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && e.Saldo > 0
                                         select new { c.CategoriaID, c.Descricao, c.Imagem }).Distinct();

                    return Json(CategoriaList, settings);
                }
                else
                {
                    var CategoriaList = (from c in _context.Categoria
                                         join p in _context.Produto on c.CategoriaID equals p.CategoriaID
                                         join e in _context.EstoqueEmpresa on p.ProdutoID equals e.ProdutoID
                                         where (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && e.Saldo > 0
                                         select new { c.CategoriaID, c.Descricao, c.Imagem }).Distinct();

                    return Json(CategoriaList, settings);
                }
            }
            else
            {
                var CategoriaList = (from c in _context.Categoria
                                     join p in _context.Produto on c.CategoriaID equals p.CategoriaID
                                     join e in _context.EstoqueEmpresa on p.ProdutoID equals e.ProdutoID
                                     where (p.Escola.EscolaID == usuario.EscolaID || p.EscolaID == usuario.EscolaID) && e.Saldo > 0
                                     select new { c.CategoriaID, c.Descricao, c.Imagem }).Distinct();

                return Json(CategoriaList, settings);
            }


        }

        [HttpPost]
        public JsonResult RetornaCliente(string dado)
        {
            var user = User.Claims.Select(x => x.Value).FirstOrDefault();
            var usuario = _context.Usuario.Include(e => e.Escola).FirstOrDefault(x => x.Id == user);

            var settings = new JsonSerializerSettings();

            if (dado == "aluno")
            {
                var AlunoList = (from a in _context.Aluno
                                 where (a.Escola.EscolaID == usuario.EscolaID || a.EscolaID == usuario.EscolaID)
                                 select new { ID = a.AlunoID, Nome = a.Nome.ToUpper() }).OrderBy(x => x.Nome);

                return Json(AlunoList, settings);
            }
            else
            {
                var FuncList = (from f in _context.Funcionario
                                where ((f.Escola.EscolaID == usuario.EscolaID || f.EscolaId == usuario.EscolaID) || f.Master)
                                select new { ID = f.FuncionarioID, Nome = f.Nome.ToUpper() }).OrderBy(x => x.Nome);

                return Json(FuncList, settings);
            }
        }

        #region Populando CantinaViewModel

        private static CaixaFechamento PopularCaixaFechamento(CaixaFechamento CaixaFechamento)
        {
            var caixaFechamento = new CaixaFechamento
            {
                AspNetUsersID = CaixaFechamento.AspNetUsersID,
                DataCadastro = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(),
                Escola = CaixaFechamento.Escola,
                EscolaID = CaixaFechamento.EscolaID,
                Fechado = false,
                SaldoFinal = 0,
                SaldoInicial = CaixaFechamento.SaldoFinal
            };

            return caixaFechamento;
        }

        private static SangriaCaixa PopularSangriaCaixa(SangriaCaixa SangriaCaixa)
        {
            var sangriaCaixa = new SangriaCaixa
            {
                AspNetUsersID = SangriaCaixa.AspNetUsersID,
                DataCadastro = SangriaCaixa.DataCadastro,
                Motivo = SangriaCaixa.Motivo,
                Valor = SangriaCaixa.Valor
            };

            return sangriaCaixa;
        }

        private static Aluno PopularAluno(Aluno Aluno)
        {
            var aluno = new Aluno
            {
                Ativo = Aluno.Ativo,
                DataCadastro = Aluno.DataCadastro,
                Escola = Aluno.Escola,
                EscolaID = Aluno.EscolaID,
                DataNascimento = Aluno.DataNascimento,
                Nome = Aluno.Nome,
                Nutricionais = Aluno.Nutricionais,
                Ra = Aluno.Ra,
                ResposavelFinanceiro = Aluno.ResposavelFinanceiro,
                ResposavelFinanceiroID = Aluno.ResposavelFinanceiroID,
                ResposavelLegal = Aluno.ResposavelLegal,
                ResposavelLegalID = Aluno.ResposavelLegalID,
                Serie = Aluno.Serie,
                SerieID = Aluno.SerieID,
                TipoGenero = Aluno.TipoGenero
            };

            return aluno;
        }

        private static Funcionario PoupularFuncionario(Funcionario Funcionario)
        {
            var funcionario = new Funcionario
            {
                Ativo = Funcionario.Ativo,
                Cargo = Funcionario.Cargo,
                CargoId = Funcionario.CargoId,
                DataCadastro = Funcionario.DataCadastro,
                Escola = Funcionario.Escola,
                EscolaId = Funcionario.EscolaId,
                Nome = Funcionario.Nome
            };

            return funcionario;
        }

        #endregion

        private bool VerificaPermissaoTela()
        {
            var liberado = false;

            var idUsuario = User.Claims.Select(u => u.Value).FirstOrDefault().ToString();

            var usuario = _context.Usuario.FirstOrDefault(x => x.Id == idUsuario);

            if (usuario != null)
            {
                if (!usuario.Master)
                {
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "CantinaController");
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
