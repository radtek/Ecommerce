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
using PortalEducacional.Extensions;
using System.Threading;

namespace PortalEducacional.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [Permissao(Modulo = "Portal Educacional", Menu = "Produtos", DescricaoTela = "Fornecedor", Controller = "FornecedorController", Acao = "Bloqueado;Editar")]
    public class FornecedorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FornecedorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fornecedor
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

            var applicationDbContext = _context.Fornecedor.Include(f => f.Endereco.Cidade);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Fornecedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor
                .Include(f => f.Endereco)
                .SingleOrDefaultAsync(m => m.FornecedorID == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return View(fornecedor);
        }

        // GET: Fornecedor/Create
        public IActionResult Create()
        {
            try
            {
                var fornecedor = new Fornecedor();

                CarregaEndereco(fornecedor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            ViewData["EscolaID"] = new SelectList(_context.Escola, "EscolaID", "Nome");
            ViewData["EnderecoId"] = new SelectList(_context.Set<Endereco>(), "EnderecoId", "CEP");
            return View();
        }

        // POST: Fornecedor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fornecedor fornecedor)
        {
            
            var CNPJ = fornecedor.Cnpj.ApenasNumero();
            fornecedor.Cnpj = CNPJ.Trim();

            if (VerificarFornecedor(fornecedor.Cnpj))
            {
                Alert(string.Format("CNPJ {0} já cadastrada no sistema!", fornecedor.Cnpj), NotificationType.info);
                ViewData["EnderecoID"] = new SelectList(_context.Endereco, "EnderecoId", "Bairro", fornecedor.EnderecoID);
                return View(fornecedor);
            }

            var telefone = fornecedor.Telefone.ApenasNumero();
            fornecedor.Telefone = telefone;
            
            fornecedor.Endereco.CidadeId = RetornaCidade(fornecedor.Endereco.CidadeId);
        
            CarregaEndereco(fornecedor);

            //if (ModelState.IsValid)
            //{                
                _context.Add(fornecedor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            //}

            ViewData["EnderecoID"] = new SelectList(_context.Endereco, "EnderecoId", "Bairro", fornecedor.EnderecoID);
            return View(fornecedor);
        }

        // GET: Fornecedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                Alert(string.Format("Fornecedor Não Encontrado!"), NotificationType.info);
                return View();
            }

            var fornecedor = await _context.Fornecedor.SingleOrDefaultAsync(m => m.FornecedorID == id);
            fornecedor.Endereco = await _context.Endereco.SingleOrDefaultAsync(c => c.EnderecoId == fornecedor.EnderecoID);
            fornecedor.Endereco.Cidade = await _context.Cidade.SingleOrDefaultAsync(c => c.CidadeId == fornecedor.Endereco.CidadeId);

            CarregaEndereco(fornecedor);

            if (fornecedor == null)
            {
                Alert(string.Format("Fornecedor Não Encontrado!"), NotificationType.info);
                return View();
            }

            ViewData["EnderecoID"] = new SelectList(_context.Endereco, "EnderecoId", "Bairro", fornecedor.EnderecoID);
            return View(fornecedor);
        }

        // POST: Fornecedor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int FornecedorID, Fornecedor fornecedor)
        {
            if (FornecedorID != fornecedor.FornecedorID)
            {
                Alert(string.Format("Fornecedor Não Encontrado!"), NotificationType.info);
                return View(fornecedor);
            }

            var objFornecedor = await _context.Fornecedor.SingleOrDefaultAsync(m => m.FornecedorID == FornecedorID);
            objFornecedor.Endereco = await _context.Endereco.SingleOrDefaultAsync(c => c.EnderecoId == objFornecedor.EnderecoID);
            objFornecedor.Endereco.Cidade = await _context.Cidade.SingleOrDefaultAsync(c => c.CidadeId == objFornecedor.Endereco.CidadeId);
            objFornecedor.Endereco.Complemento = fornecedor.Endereco.Complemento;
            objFornecedor.Endereco.Cidade.Estado = await _context.Estado.SingleOrDefaultAsync(c => c.EstadoId == objFornecedor.Endereco.Cidade.EstadoId);
            objFornecedor.DataCadastro = fornecedor.DataCadastro;
            objFornecedor.Cnpj = fornecedor.Cnpj;
            objFornecedor.Descricao = fornecedor.Descricao;
            objFornecedor.Email = fornecedor.Email;
            objFornecedor.Nome = fornecedor.Nome;
            objFornecedor.Telefone = fornecedor.Telefone;
            objFornecedor.Ativo = fornecedor.Ativo;

            if (objFornecedor != null)
            {
                try
                {

                    _context.Update(objFornecedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FornecedorExists(objFornecedor.FornecedorID))
                    {
                        Alert(string.Format("Fornecedor {0} Alterado com Sucesso!", objFornecedor.Nome), NotificationType.info);
                        return View(objFornecedor);
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnderecoID"] = new SelectList(_context.Endereco, "EnderecoId", "Bairro", objFornecedor.EnderecoID);
            return View(objFornecedor);
        }

        // GET: Fornecedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor
                .Include(f => f.Endereco)
                .SingleOrDefaultAsync(m => m.FornecedorID == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return View(fornecedor);
        }

        // POST: Fornecedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int FornecedorID)
        {
            var fornecedor = await _context.Fornecedor.SingleOrDefaultAsync(m => m.FornecedorID == FornecedorID);
            _context.Fornecedor.Remove(fornecedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FornecedorExists(int id)
        {
            return _context.Fornecedor.Any(e => e.FornecedorID == id);
        }

        private bool VerificarFornecedor(string Cnpj)
        {
            if (Cnpj == null) return false;

            var existe = _context.Fornecedor.FirstOrDefault(x => x.Cnpj == Cnpj);

            return existe == null ? false : true;
        }

        private void CarregaEndereco(Fornecedor fornecedor)
        {
            var enderecoList = new List<Endereco>();
            enderecoList = _context.Endereco.ToList();

            if (fornecedor.Endereco == null || fornecedor.Endereco.Cidade == null)
            {
                ViewBag.Estado = new SelectList(_context.Set<Estado>().OrderBy(x => x.Nome), "EstadoId", "Nome");
                ViewBag.Cidade = new SelectList(_context.Set<Cidade>().OrderBy(x => x.Nome), "CidadeId", "Nome");
            }
            else
            {
                var cidade = RetornaCidade(fornecedor.Endereco.Cidade.CidadeId);
                var estado = RetornaEstado(fornecedor.Endereco.Cidade.EstadoId);
                ViewBag.Estado = new SelectList(_context.Set<Estado>().OrderBy(x => x.Nome), "EstadoId", "Nome", estado);
                ViewBag.Cidade = new SelectList(_context.Set<Cidade>().OrderBy(x => x.Nome), "CidadeId", "Nome", cidade);
            }
        }

        private int RetornaCidade(int id)
        {
            var cidadeID = _context.Cidade.FirstOrDefault(x => x.CidadeId == id);
            var retorno = cidadeID != null ? cidadeID.CidadeId : 0;
            return retorno;
        }

        private int RetornaEstado(int id)
        {
            var EstadoId = _context.Estado.FirstOrDefault(x => x.EstadoId == id);
            var retorno = EstadoId != null ? EstadoId.EstadoId : 0;
            return retorno;
        }

        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>swal({ title: 'ATENÇÃO!', text: '" + message + "', icon: '" + notificationType + "', timer: 4000, button: false});" + "</script>";
            TempData["notification"] = msg;
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
                    var acesso = _context.UsuarioPermissoes.FirstOrDefault(x => x.ApplicationUserID == usuario.Id && x.Controller == "FornecedorController");
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
