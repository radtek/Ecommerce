using PortalEducacional.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportarArquivosLote
{
    public partial class frmImportar : Form
    {
        public frmImportar()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtBrowse.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBrowse.Text))
            {
                MessageBox.Show("Necessário selecionar um diretório onde estão os PDFs", "Informação");
                return;
            }

            var listaArquivos = Directory.GetFiles(txtBrowse.Text);
            var repo = new Repositorio<RepositorioPDF>(BancoUtilizado.PortalEducacional);
            var listarAlunos = repo.returnListClass("SELECT * FROM RepositorioPDF", new Dapper.DynamicParameters());
            foreach (var item in listaArquivos)
            {
                var arquivo = new FileInfo(item);
                listarAlunos.ForEach(x =>
                {
                    if (x.RA == arquivo.Name.Replace(".pdf", ""))
                    {
                        repo.ExecuteNonQuery("UPDATE RepositorioPDF SET PDF = @PDF WHERE ID = @ID", new { ID = x.ID, PDF = File.ReadAllBytes(item) });
                    }
                });

            }
            MessageBox.Show("PDFs adicionados com sucesso", "Informação");

        }

        private void btnMigrarAlunos_Click(object sender, EventArgs e)
        {
            var repo = new Repositorio<RepositorioPDF>(BancoUtilizado.WebEscola);
            var query = $"SELECT  MAE.e_mail EmailMae,PAI.e_mail EmailPai,RF.e_mail EmailFinanceiro,RL.e_mail EmailLegal, b.cidade, b.NOME Aluno, " +
                $"b.RA,  " +
                $"(SELECT esc.nome from Escola esc where esc.cod_escola = d.cod_escola) Escola, " +
                $"(SELECT foto FROM ALUNO_FOTO AF WHERE AF.cod_escola = b.cod_escola AND AF.cod_aluno = b.cod_aluno) Imagem, " +
                $"(SELECT es.descricao from Escola_Serie es where es.cod_serie = d.cod_serie and es.cod_escola = d.cod_escola) serie, " +
                $"d.cod_serie, d.cod_turma turma " +
                $"FROM ALUNO b " +
                $"INNER JOIN(select C.cod_escola, C.ano, C.cod_aluno, C.cod_serie, C.cod_turma, MAX(C.data_matricula) data_matricula from Matricula_Turma C WHERE C.ano = 2018 " +
                $"and data_desligamento IS NULL " +
                $"GROUP BY C.cod_escola, C.ano, C.cod_aluno, C.cod_serie, C.cod_turma, C.data_matricula " +
                $"HAVING C.data_matricula = max(C.data_matricula)) D ON D.cod_aluno = b.cod_aluno and d.cod_escola = b.cod_escola " +
                $"LEFT JOIN Responsavel RL ON b.CPF_resp_legal = RL.CPF " +
                $"LEFT JOIN Responsavel PAI ON b.CPF_pai = PAI.CPF " +
                $"LEFT JOIN Responsavel RF ON b.CPF_resp_financeiro = RF.CPF " +
                $"LEFT JOIN Responsavel MAE ON b.CPF_mae = MAE.CPF " +
                $"WHERE b.RA IS NOT NULL AND b.RA <> '' AND b.RA NOT IN('000108753974-2', '000112690837-X') " +
                $"AND((RF.e_mail IS NOT NULL AND RF.e_mail <> '') OR(RL.e_mail IS NOT NULL AND RL.e_mail <> '') " +
                $"OR(MAE.e_mail IS NOT NULL AND MAE.e_mail <> '') OR(PAI.e_mail IS NOT NULL AND PAI.e_mail <> '')) " +
                $"order by escola,serie , turma ";
            var listaRepo = repo.returnListClass(query, new Dapper.DynamicParameters());
            var repoPortal = new Repositorio<RepositorioPDF>(BancoUtilizado.PortalEducacional);
            var executado = repoPortal.returnClass("SELECT TOP 1 * FROM RepositorioPDF", new Dapper.DynamicParameters());
            if (executado != null)
            {
                var resultado = MessageBox.Show("Banco de dados já foi migrado! Deseja atualizar os dados?", "Informação", MessageBoxButtons.YesNo);
                if (resultado == DialogResult.Yes)
                {
                    repoPortal.ExecuteNonQuery("DELETE FROM REPOSITORIOPDF", null);
                }
                else
                {
                    return;
                }
            }
            foreach (var item in listaRepo)
            {
                repoPortal.ExecuteNonQuery("INSERT INTO REPOSITORIOPDF(EmailLegal,EmailFinanceiro,EmailMae,EmailPai,Aluno,Escola,RA)" +
                    "VALUES(@EmailLegal,@EmailFinanceiro,@EmailMae,@EmailPai,@Aluno,@Escola,@RA)", item);
            }
            MessageBox.Show("Banco de dados migrado com sucesso!", "Informação");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var repo = new Repositorio<Aluno>(BancoUtilizado.WebEscola);
            var query = $"SELECT " +
                        $"REPLACE(b.Nome, '''', '''''') nome, " +
                        $"isnull(RA, '0000000') rA, " +
                        $"Convert(datetime, Convert(varchar, b.data_nascimento, 103), 103) dataNascimento, " +
                        $"CASE WHEN b.sexo = 'M' THEN 'Masculino' ELSE 'Feminino' END TipoGenero, " +
                        $"CASE WHEN data_desligamento is null THEN 'false' ELSE 'true' END Ativo, " +
                        $"(SELECT foto FROM ALUNO_FOTO AF WHERE AF.cod_escola = b.cod_escola AND AF.cod_aluno = b.cod_aluno) Foto, " +
                        $"'0.00' SaldoDisponivel, " +
                        $"ISNULL(REPLACE(REPLACE(RF.CPF, '.', ''), '-', ''), '') responsavelCpf, " +
                        $"ISNULL(REPLACE(REPLACE(RF.CPF, '.', ''), '-', ''), '') responsavelCpf, " +
                        $"(SELECT es.descricao from Escola_Serie es where es.cod_serie = d.cod_serie and es.cod_escola = d.cod_escola) SerieTurma, " +
                        $"isnull(REPLACE(REPLACE(REPLACE(e.CGC, '.', ''), '/', ''), '-', ''), '00000000000000') EscolaCnpj " +
                        $"FROM ALUNO b " +
                        $"INNER JOIN Escola e on e.cod_escola = b.cod_escola " +
                        $"LEFT OUTER JOIN Responsavel RL ON b.CPF_resp_legal = RL.CPF " +
                        $"LEFT OUTER JOIN Responsavel RF ON b.CPF_resp_financeiro = RF.CPF " +
                        $"INNER JOIN(select C.cod_escola, C.ano, C.cod_aluno, C.cod_serie, C.cod_turma, C.data_matricula from Matricula_Turma C WHERE C.ano = 2018 " +
                        $"GROUP BY C.cod_escola, C.ano, C.cod_aluno, C.cod_serie, C.cod_turma, C.data_matricula " +
                        $"HAVING C.data_matricula = max(C.data_matricula)) D " +
                        $"ON D.cod_aluno = b.cod_aluno and d.cod_escola = b.cod_escola ";

            var listaRepo = repo.returnListClass(query, new Dapper.DynamicParameters());
            var repoPortal = new Repositorio<Aluno>(BancoUtilizado.PortalEducacional);

            foreach (var item in listaRepo)
            {
                repoPortal.ExecuteNonQuery("IF NOT EXISTS (Select * from Aluno where Nome = @Nome and DataCadastro is null) BEGIN " +
                    "INSERT INTO ALUNO (NOME, RA, DataNascimento, TipoGenero, Ativo, ResposavelFinanceiroID, ResposavelLegalID, SerieID, EscolaID, Foto, SaldoDisponivel) " +
                    "VALUES(@Nome, @RA, @DataNascimento, @TipoGenero, @Ativo, (Select top 1 ResponsavelId from Responsavel where cpf = @responsavelCpf), (Select top 1 ResponsavelId from Responsavel where cpf = @responsavelCpf), (Select top 1 SerieId from Serie where Descricao = @SerieTurma), (Select top 1 EscolaId from Escola where CNPJ = @EscolaCnpj), @Foto, @SaldoDisponivel) END", item);
            }
            MessageBox.Show("Alunos inseridos com sucesso!", "Informação");
        }

        private void btnMigrafotoAluno_Click(object sender, EventArgs e)
        {
            try
            {
                var repoWebEscola = new Repositorio<Aluno>(BancoUtilizado.WebEscola);
                var repoPortalEducacional = new Repositorio<Aluno>(BancoUtilizado.PortalEducacional);

                var aluno = repoWebEscola.returnClass("SELECT Foto FROM ALUNO_FOTO where cod_aluno = 40765 and cod_escola = 901", new Dapper.DynamicParameters());
                repoPortalEducacional.ExecuteNonQuery("update aluno set foto = @foto where alunoid = 44267", new { foto = aluno.Foto});
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
