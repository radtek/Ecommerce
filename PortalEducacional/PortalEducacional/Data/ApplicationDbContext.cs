using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalEducacional.Models;
using PortalEducacional.Models.LojaEA;

namespace PortalEducacional.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


           
            builder.Entity<EscolaFornecedor>()       // THIS IS FIRST
      .HasOne(u => u.Escola).WithMany(u => u.EscolaFornecedor).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.Entity<EscolaFornecedor>()
    .HasKey(ef => new { ef.EscolaID, ef.FornecedorID });

            builder.Entity<EscolaFornecedor>()
                .HasOne(bc => bc.Escola)
                .WithMany(b => b.EscolaFornecedor)
                .HasForeignKey(bc => bc.EscolaID);

            builder.Entity<EscolaFornecedor>()
                .HasOne(bc => bc.Fornecedor)
                .WithMany(c => c.EscolaFornecedor)
                .HasForeignKey(bc => bc.FornecedorID);
        }

        public DbSet<Aluno> Aluno { get; set; }

        public DbSet<Cidade> Cidade { get; set; }

        public DbSet<Estado> Estado { get; set; }

        public DbSet<Endereco> Endereco { get; set; }

        public DbSet<Escola> Escola { get; set; }

        public DbSet<Funcionario> Funcionario { get; set; }

        public DbSet<Cargo> Cargo { get; set; }

        public DbSet<Responsavel> Responsavel { get; set; }

        public DbSet<ResponsavelFinanceiro> ResponsavelFinanceiro { get; set; }

        public DbSet<Nutricional> Nutricional { get; set; }

        public DbSet<Produto> Produto { get; set; }

        public DbSet<Fornecedor> Fornecedor { get; set; }

        public DbSet<DadoNutricional> DadoNutricional { get; set; }

        public DbSet<Serie> Serie { get; set; }

        public DbSet<TipoNutricional> TipoNutricional { get; set; }

        public DbSet<Estoque> Estoque { get; set; }

        public DbSet<EstoqueEmpresa> EstoqueEmpresa { get; set; }

        public DbSet<Pedido> Pedido { get; set; }

        public DbSet<PedidoItem> PedidoItem { get; set; }

        public DbSet<CaixaFechamento> CaixaFechamento { get; set; }

        public DbSet<CaixaMovimento> CaixaMovimento { get; set; }

        public DbSet<SangriaCaixa> SangriaCaixa { get; set; }

        public DbSet<SetupEscola> SetupEscola { get; set; }

        public DbSet<Categoria> Categoria { get; set; }

        public DbSet<ApplicationUser> Usuario { get; set; }

        public DbSet<Perfil> Perfil { get; set; }

        public DbSet<PerfilPermissoes> PerfilPermissoes { get; set; }

        public DbSet<UsuarioPermissoes> UsuarioPermissoes { get; set; }

        public DbSet<Biometria> Biometria { get; set; }

        public DbSet<RepositorioPDF> RepositorioPDF { get; set; }

        public DbSet<FaturamentoCartao> FaturamentoCartao { get; set; }

        public DbSet<AlunoProdutos> AlunoProdutos { get; set; }

        public DbSet<PedidoVendaCredito> PedidoVendaCredito { get; set; }

        public DbSet<PortalEducacional.Models.LojaEA.LojaProduto> LojaProduto { get; set; }
    }
}
