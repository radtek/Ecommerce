using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PortalEducacional.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Biometria",
                columns: table => new
                {
                    BiometriaID = table.Column<Guid>(nullable: false),
                    AlunoID = table.Column<int>(nullable: false),
                    Dedo = table.Column<int>(nullable: false),
                    EscolaID = table.Column<int>(nullable: false),
                    FuncionarioID = table.Column<int>(nullable: false),
                    HashDedo = table.Column<string>(nullable: true),
                    ResponsavelID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biometria", x => x.BiometriaID);
                });

            migrationBuilder.CreateTable(
                name: "Cargo",
                columns: table => new
                {
                    CargoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargo", x => x.CargoID);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    CategoriaID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: false),
                    Imagem = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.CategoriaID);
                });

            migrationBuilder.CreateTable(
                name: "Estado",
                columns: table => new
                {
                    EstadoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodIbge = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    UF = table.Column<string>(maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado", x => x.EstadoId);
                });

            migrationBuilder.CreateTable(
                name: "RepositorioPDF",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Aluno = table.Column<string>(nullable: true),
                    EmailFinanceiro = table.Column<string>(nullable: true),
                    EmailLegal = table.Column<string>(nullable: true),
                    EmailMae = table.Column<string>(nullable: true),
                    EmailPai = table.Column<string>(nullable: true),
                    Escola = table.Column<string>(nullable: true),
                    PDF = table.Column<byte[]>(nullable: true),
                    RA = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositorioPDF", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Responsavel",
                columns: table => new
                {
                    ResponsavelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CPF = table.Column<string>(maxLength: 14, nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    Nome = table.Column<string>(maxLength: 100, nullable: true),
                    Telefone = table.Column<string>(maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsavel", x => x.ResponsavelID);
                });

            migrationBuilder.CreateTable(
                name: "SangriaCaixa",
                columns: table => new
                {
                    SangriaCaixaID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AspNetUsersID = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    Motivo = table.Column<string>(nullable: false),
                    Valor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SangriaCaixa", x => x.SangriaCaixaID);
                });

            migrationBuilder.CreateTable(
                name: "Serie",
                columns: table => new
                {
                    SerieID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Serie", x => x.SerieID);
                });

            migrationBuilder.CreateTable(
                name: "TipoNutricional",
                columns: table => new
                {
                    TipoNutricionalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    UnidadeMedida = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoNutricional", x => x.TipoNutricionalID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cidade",
                columns: table => new
                {
                    CidadeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodIbge = table.Column<int>(nullable: false),
                    EstadoId = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidade", x => x.CidadeId);
                    table.ForeignKey(
                        name: "FK_Cidade_Estado_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estado",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponsavelFinanceiro",
                columns: table => new
                {
                    ResponsavelFinanceiroID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CPF = table.Column<string>(maxLength: 14, nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    Nome = table.Column<string>(maxLength: 100, nullable: true),
                    ResponsavelID = table.Column<int>(nullable: false),
                    Telefone = table.Column<string>(maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsavelFinanceiro", x => x.ResponsavelFinanceiroID);
                    table.ForeignKey(
                        name: "FK_ResponsavelFinanceiro_Responsavel_ResponsavelID",
                        column: x => x.ResponsavelID,
                        principalTable: "Responsavel",
                        principalColumn: "ResponsavelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    EnderecoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bairro = table.Column<string>(maxLength: 60, nullable: false),
                    CEP = table.Column<string>(maxLength: 10, nullable: false),
                    CidadeId = table.Column<int>(nullable: false),
                    Complemento = table.Column<string>(maxLength: 100, nullable: true),
                    Logradouro = table.Column<string>(maxLength: 100, nullable: false),
                    Numero = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.EnderecoId);
                    table.ForeignKey(
                        name: "FK_Endereco_Cidade_CidadeId",
                        column: x => x.CidadeId,
                        principalTable: "Cidade",
                        principalColumn: "CidadeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Escola",
                columns: table => new
                {
                    EscolaID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    CNPJ = table.Column<string>(maxLength: 19, nullable: false),
                    DataCadastro = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    EnderecoId = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escola", x => x.EscolaID);
                    table.ForeignKey(
                        name: "FK_Escola_Endereco_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Endereco",
                        principalColumn: "EnderecoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedor",
                columns: table => new
                {
                    FornecedorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    Cnpj = table.Column<string>(maxLength: 18, nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EnderecoID = table.Column<int>(nullable: false),
                    EscolaFornecedorID = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedor", x => x.FornecedorID);
                    table.ForeignKey(
                        name: "FK_Fornecedor_Endereco_EnderecoID",
                        column: x => x.EnderecoID,
                        principalTable: "Endereco",
                        principalColumn: "EnderecoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aluno",
                columns: table => new
                {
                    AlunoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    DataCadastro = table.Column<string>(nullable: true),
                    DataNascimento = table.Column<DateTime>(nullable: false),
                    EscolaID = table.Column<int>(nullable: false),
                    Foto = table.Column<byte[]>(nullable: true),
                    Nome = table.Column<string>(maxLength: 100, nullable: false),
                    Ra = table.Column<string>(maxLength: 20, nullable: false),
                    ResposavelFinanceiroID = table.Column<int>(nullable: true),
                    ResposavelLegalID = table.Column<int>(nullable: true),
                    SaldoDisponivel = table.Column<decimal>(nullable: true),
                    SerieID = table.Column<int>(nullable: false),
                    TipoGenero = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aluno", x => x.AlunoID);
                    table.ForeignKey(
                        name: "FK_Aluno_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Aluno_Serie_SerieID",
                        column: x => x.SerieID,
                        principalTable: "Serie",
                        principalColumn: "SerieID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaixaFechamento",
                columns: table => new
                {
                    CaixaFechamentoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AspNetUsersID = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    EscolaID = table.Column<int>(nullable: false),
                    Fechado = table.Column<bool>(nullable: false),
                    SaldoFinal = table.Column<double>(nullable: false),
                    SaldoInicial = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaFechamento", x => x.CaixaFechamentoID);
                    table.ForeignKey(
                        name: "FK_CaixaFechamento_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    FuncionarioID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    CargoId = table.Column<int>(nullable: false),
                    DataCadastro = table.Column<string>(nullable: true),
                    EscolaId = table.Column<int>(nullable: false),
                    Master = table.Column<bool>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.FuncionarioID);
                    table.ForeignKey(
                        name: "FK_Funcionario_Cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargo",
                        principalColumn: "CargoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Funcionario_Escola_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Perfil",
                columns: table => new
                {
                    PerfilID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    EscolaID = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.PerfilID);
                    table.ForeignKey(
                        name: "FK_Perfil_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    ProdutoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ativo = table.Column<bool>(nullable: false),
                    AtualizadoPor = table.Column<string>(nullable: true),
                    CategoriaID = table.Column<int>(nullable: false),
                    Codigo = table.Column<string>(nullable: false),
                    CodigoBarras = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(nullable: false),
                    EscolaID = table.Column<int>(nullable: false),
                    Observacao = table.Column<string>(nullable: true),
                    PrecisaAprovacao = table.Column<bool>(nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(nullable: false),
                    UnidadeMedida = table.Column<string>(nullable: true),
                    Valor = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.ProdutoID);
                    table.ForeignKey(
                        name: "FK_Produto_Categoria_CategoriaID",
                        column: x => x.CategoriaID,
                        principalTable: "Categoria",
                        principalColumn: "CategoriaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produto_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetupEscola",
                columns: table => new
                {
                    SetupEscolaID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EscolaID = table.Column<int>(nullable: false),
                    PonteiroPedido = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupEscola", x => x.SetupEscolaID);
                    table.ForeignKey(
                        name: "FK_SetupEscola_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscolaFornecedor",
                columns: table => new
                {
                    EscolaID = table.Column<int>(nullable: false),
                    FornecedorID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscolaFornecedor", x => new { x.EscolaID, x.FornecedorID });
                    table.ForeignKey(
                        name: "FK_EscolaFornecedor_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EscolaFornecedor_Fornecedor_FornecedorID",
                        column: x => x.FornecedorID,
                        principalTable: "Fornecedor",
                        principalColumn: "FornecedorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nutricional",
                columns: table => new
                {
                    NutricionalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Altura = table.Column<string>(nullable: false),
                    AlunoID = table.Column<int>(nullable: false),
                    DataCadastro = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(maxLength: 200, nullable: true),
                    Peso = table.Column<string>(nullable: false),
                    Resultado = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutricional", x => x.NutricionalID);
                    table.ForeignKey(
                        name: "FK_Nutricional_Aluno_AlunoID",
                        column: x => x.AlunoID,
                        principalTable: "Aluno",
                        principalColumn: "AlunoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    PedidoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlunoID = table.Column<int>(nullable: true),
                    AspNetUsersID = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    Desconto = table.Column<double>(nullable: false),
                    EscolaID = table.Column<int>(nullable: false),
                    FuncionarioID = table.Column<int>(nullable: true),
                    Numero = table.Column<string>(nullable: false),
                    Observacao = table.Column<string>(nullable: true),
                    ValorBruto = table.Column<double>(nullable: false),
                    ValorTotal = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.PedidoID);
                    table.ForeignKey(
                        name: "FK_Pedido_Aluno_AlunoID",
                        column: x => x.AlunoID,
                        principalTable: "Aluno",
                        principalColumn: "AlunoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedido_Funcionario_FuncionarioID",
                        column: x => x.FuncionarioID,
                        principalTable: "Funcionario",
                        principalColumn: "FuncionarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Ativo = table.Column<bool>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    EscolaID = table.Column<int>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    Master = table.Column<bool>(nullable: false),
                    Nome = table.Column<string>(nullable: false),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PerfilID = table.Column<int>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPermissoes",
                columns: table => new
                {
                    PerfilPermissoesID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Controller = table.Column<string>(nullable: true),
                    PerfilID = table.Column<int>(nullable: false),
                    Permissao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPermissoes", x => x.PerfilPermissoesID);
                    table.ForeignKey(
                        name: "FK_PerfilPermissoes_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlunoProdutos",
                columns: table => new
                {
                    AlunoProdutosID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlunoID = table.Column<int>(nullable: false),
                    ProdutoID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlunoProdutos", x => x.AlunoProdutosID);
                    table.ForeignKey(
                        name: "FK_AlunoProdutos_Aluno_AlunoID",
                        column: x => x.AlunoID,
                        principalTable: "Aluno",
                        principalColumn: "AlunoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlunoProdutos_Produto_ProdutoID",
                        column: x => x.ProdutoID,
                        principalTable: "Produto",
                        principalColumn: "ProdutoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadoNutricional",
                columns: table => new
                {
                    DadoNutricionalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Porcao = table.Column<decimal>(nullable: false),
                    ProdutoID = table.Column<int>(nullable: false),
                    TipoNutricionalID = table.Column<int>(nullable: false),
                    ValorDiario = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadoNutricional", x => x.DadoNutricionalID);
                    table.ForeignKey(
                        name: "FK_DadoNutricional_Produto_ProdutoID",
                        column: x => x.ProdutoID,
                        principalTable: "Produto",
                        principalColumn: "ProdutoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DadoNutricional_TipoNutricional_TipoNutricionalID",
                        column: x => x.TipoNutricionalID,
                        principalTable: "TipoNutricional",
                        principalColumn: "TipoNutricionalID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estoque",
                columns: table => new
                {
                    EstoqueID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AspNetUsersID = table.Column<string>(nullable: true),
                    DataCadastro = table.Column<string>(nullable: true),
                    EscolaID = table.Column<int>(nullable: false),
                    Historico = table.Column<string>(nullable: false),
                    ProdutoID = table.Column<int>(nullable: false),
                    Quantidade = table.Column<double>(nullable: false),
                    Saida = table.Column<bool>(nullable: false),
                    ValorCompra = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoque", x => x.EstoqueID);
                    table.ForeignKey(
                        name: "FK_Estoque_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estoque_Produto_ProdutoID",
                        column: x => x.ProdutoID,
                        principalTable: "Produto",
                        principalColumn: "ProdutoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstoqueEmpresa",
                columns: table => new
                {
                    EstoqueEmpresaID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EscolaID = table.Column<int>(nullable: false),
                    ProdutoID = table.Column<int>(nullable: false),
                    QuantidadeMaxima = table.Column<double>(nullable: true),
                    QuantidadeMinima = table.Column<double>(nullable: true),
                    Saldo = table.Column<double>(nullable: false),
                    Validade = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstoqueEmpresa", x => x.EstoqueEmpresaID);
                    table.ForeignKey(
                        name: "FK_EstoqueEmpresa_Escola_EscolaID",
                        column: x => x.EscolaID,
                        principalTable: "Escola",
                        principalColumn: "EscolaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstoqueEmpresa_Produto_ProdutoID",
                        column: x => x.ProdutoID,
                        principalTable: "Produto",
                        principalColumn: "ProdutoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaixaMovimento",
                columns: table => new
                {
                    CaixaMovimentoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AspNetUsersID = table.Column<string>(nullable: true),
                    CaixaFechamentoID = table.Column<int>(nullable: false),
                    DataCadastro = table.Column<string>(nullable: true),
                    PedidoID = table.Column<int>(nullable: true),
                    SangriaCaixaID = table.Column<int>(nullable: true),
                    Valor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaMovimento", x => x.CaixaMovimentoID);
                    table.ForeignKey(
                        name: "FK_CaixaMovimento_CaixaFechamento_CaixaFechamentoID",
                        column: x => x.CaixaFechamentoID,
                        principalTable: "CaixaFechamento",
                        principalColumn: "CaixaFechamentoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaixaMovimento_Pedido_PedidoID",
                        column: x => x.PedidoID,
                        principalTable: "Pedido",
                        principalColumn: "PedidoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaixaMovimento_SangriaCaixa_SangriaCaixaID",
                        column: x => x.SangriaCaixaID,
                        principalTable: "SangriaCaixa",
                        principalColumn: "SangriaCaixaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    PedidoItemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Desconto = table.Column<double>(nullable: false),
                    PedidoID = table.Column<int>(nullable: false),
                    ProdutoID = table.Column<int>(nullable: false),
                    Quantidade = table.Column<double>(nullable: false),
                    Valor = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => x.PedidoItemID);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Pedido_PedidoID",
                        column: x => x.PedidoID,
                        principalTable: "Pedido",
                        principalColumn: "PedidoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Produto_ProdutoID",
                        column: x => x.ProdutoID,
                        principalTable: "Produto",
                        principalColumn: "ProdutoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaturamentoCartao",
                columns: table => new
                {
                    FaturamentoCartaoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserID = table.Column<string>(nullable: true),
                    Bandeira = table.Column<string>(nullable: true),
                    DataTransacao = table.Column<string>(nullable: true),
                    DataVencimento = table.Column<string>(nullable: false),
                    NumeroCartao = table.Column<string>(nullable: false),
                    PaymentID = table.Column<string>(nullable: true),
                    TitularCartao = table.Column<string>(nullable: false),
                    Valor = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturamentoCartao", x => x.FaturamentoCartaoID);
                    table.ForeignKey(
                        name: "FK_FaturamentoCartao_AspNetUsers_ApplicationUserID",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioPermissoes",
                columns: table => new
                {
                    UsuarioPermissoesID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserID = table.Column<string>(nullable: true),
                    Controller = table.Column<string>(nullable: true),
                    Permissao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPermissoes", x => x.UsuarioPermissoesID);
                    table.ForeignKey(
                        name: "FK_UsuarioPermissoes_AspNetUsers_ApplicationUserID",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoVendaCredito",
                columns: table => new
                {
                    PedidoVendaCreditoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlunoID = table.Column<int>(nullable: false),
                    ApplicationUserID = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    FaturamentoCartaoID = table.Column<int>(nullable: false),
                    Numero = table.Column<string>(nullable: true),
                    Valor = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoVendaCredito", x => x.PedidoVendaCreditoID);
                    table.ForeignKey(
                        name: "FK_PedidoVendaCredito_Aluno_AlunoID",
                        column: x => x.AlunoID,
                        principalTable: "Aluno",
                        principalColumn: "AlunoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoVendaCredito_AspNetUsers_ApplicationUserID",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoVendaCredito_FaturamentoCartao_FaturamentoCartaoID",
                        column: x => x.FaturamentoCartaoID,
                        principalTable: "FaturamentoCartao",
                        principalColumn: "FaturamentoCartaoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_EscolaID",
                table: "Aluno",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_SerieID",
                table: "Aluno",
                column: "SerieID");

            migrationBuilder.CreateIndex(
                name: "IX_AlunoProdutos_AlunoID",
                table: "AlunoProdutos",
                column: "AlunoID");

            migrationBuilder.CreateIndex(
                name: "IX_AlunoProdutos_ProdutoID",
                table: "AlunoProdutos",
                column: "ProdutoID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EscolaID",
                table: "AspNetUsers",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PerfilID",
                table: "AspNetUsers",
                column: "PerfilID");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaFechamento_EscolaID",
                table: "CaixaFechamento",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMovimento_CaixaFechamentoID",
                table: "CaixaMovimento",
                column: "CaixaFechamentoID");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMovimento_PedidoID",
                table: "CaixaMovimento",
                column: "PedidoID");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMovimento_SangriaCaixaID",
                table: "CaixaMovimento",
                column: "SangriaCaixaID");

            migrationBuilder.CreateIndex(
                name: "IX_Cidade_EstadoId",
                table: "Cidade",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DadoNutricional_ProdutoID",
                table: "DadoNutricional",
                column: "ProdutoID");

            migrationBuilder.CreateIndex(
                name: "IX_DadoNutricional_TipoNutricionalID",
                table: "DadoNutricional",
                column: "TipoNutricionalID");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_CidadeId",
                table: "Endereco",
                column: "CidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Escola_EnderecoId",
                table: "Escola",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_EscolaFornecedor_FornecedorID",
                table: "EscolaFornecedor",
                column: "FornecedorID");

            migrationBuilder.CreateIndex(
                name: "IX_Estoque_EscolaID",
                table: "Estoque",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_Estoque_ProdutoID",
                table: "Estoque",
                column: "ProdutoID");

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueEmpresa_EscolaID",
                table: "EstoqueEmpresa",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueEmpresa_ProdutoID",
                table: "EstoqueEmpresa",
                column: "ProdutoID");

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentoCartao_ApplicationUserID",
                table: "FaturamentoCartao",
                column: "ApplicationUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedor_EnderecoID",
                table: "Fornecedor",
                column: "EnderecoID");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_CargoId",
                table: "Funcionario",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_EscolaId",
                table: "Funcionario",
                column: "EscolaId");

            migrationBuilder.CreateIndex(
                name: "IX_Nutricional_AlunoID",
                table: "Nutricional",
                column: "AlunoID");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_AlunoID",
                table: "Pedido",
                column: "AlunoID");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_EscolaID",
                table: "Pedido",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_FuncionarioID",
                table: "Pedido",
                column: "FuncionarioID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_PedidoID",
                table: "PedidoItem",
                column: "PedidoID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_ProdutoID",
                table: "PedidoItem",
                column: "ProdutoID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoVendaCredito_AlunoID",
                table: "PedidoVendaCredito",
                column: "AlunoID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoVendaCredito_ApplicationUserID",
                table: "PedidoVendaCredito",
                column: "ApplicationUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoVendaCredito_FaturamentoCartaoID",
                table: "PedidoVendaCredito",
                column: "FaturamentoCartaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Perfil_EscolaID",
                table: "Perfil",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPermissoes_PerfilID",
                table: "PerfilPermissoes",
                column: "PerfilID");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaID",
                table: "Produto",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_EscolaID",
                table: "Produto",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsavelFinanceiro_ResponsavelID",
                table: "ResponsavelFinanceiro",
                column: "ResponsavelID");

            migrationBuilder.CreateIndex(
                name: "IX_SetupEscola_EscolaID",
                table: "SetupEscola",
                column: "EscolaID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPermissoes_ApplicationUserID",
                table: "UsuarioPermissoes",
                column: "ApplicationUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlunoProdutos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Biometria");

            migrationBuilder.DropTable(
                name: "CaixaMovimento");

            migrationBuilder.DropTable(
                name: "DadoNutricional");

            migrationBuilder.DropTable(
                name: "EscolaFornecedor");

            migrationBuilder.DropTable(
                name: "Estoque");

            migrationBuilder.DropTable(
                name: "EstoqueEmpresa");

            migrationBuilder.DropTable(
                name: "Nutricional");

            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "PedidoVendaCredito");

            migrationBuilder.DropTable(
                name: "PerfilPermissoes");

            migrationBuilder.DropTable(
                name: "RepositorioPDF");

            migrationBuilder.DropTable(
                name: "ResponsavelFinanceiro");

            migrationBuilder.DropTable(
                name: "SetupEscola");

            migrationBuilder.DropTable(
                name: "UsuarioPermissoes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CaixaFechamento");

            migrationBuilder.DropTable(
                name: "SangriaCaixa");

            migrationBuilder.DropTable(
                name: "TipoNutricional");

            migrationBuilder.DropTable(
                name: "Fornecedor");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "FaturamentoCartao");

            migrationBuilder.DropTable(
                name: "Responsavel");

            migrationBuilder.DropTable(
                name: "Aluno");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Serie");

            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "Perfil");

            migrationBuilder.DropTable(
                name: "Escola");

            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "Cidade");

            migrationBuilder.DropTable(
                name: "Estado");
        }
    }
}
