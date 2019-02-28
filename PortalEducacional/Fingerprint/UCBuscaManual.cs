using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using zkonline;
using ZK.Utils;
using ZK.ConfigManager;
using System.IO;
using PortalEducacional.Models;
using SourceAFIS.Templates;
using Dapper;
using System.Configuration;
using System.Diagnostics;

namespace FingerprintClass
{
    public partial class UCBuscaManual : UserControl
    {

        bool _biometria = false;
        int _escolaID;
        private int _alunoID;
        private Aluno _alunoCompleto;
        private Dictionary<int, List<string>> tempList = new Dictionary<int, List<string>>();
        private string m_oldfingerSign;
        private Dictionary<int, List<byte[]>> templates = new Dictionary<int, List<byte[]>>();

        public UCBuscaManual()
        {
            InitializeComponent();            
        }

        public void Inicializar(int alunoID = 0)
        {
            _alunoID = alunoID;
            _escolaID = int.Parse(ConfigurationManager.AppSettings["escolaID"]);
            _biometria = alunoID != 0;
            var dic = new Dictionary<int, string>();
            dic.Add(1, "Masculino");
            dic.Add(2, "Feminino");
            ddlSexo.DisplayMember = "value";
            ddlSexo.ValueMember = "key";
            ddlSexo.DataSource = new BindingSource(dic, null);
            ListarDDLEscola();
            
        }

        private void ListarDDLEscola()
        {
            var repoEscola = new Repositorio<Escola>();
            var listaEscolas = repoEscola.returnListClass("SELECT EscolaID, (CNPJ + ' - ' + NOME) NOME FROM Escola", new Dapper.DynamicParameters());
            ddlEscola.DisplayMember = "NOME";
            ddlEscola.ValueMember = "EscolaID";
            ddlEscola.DataSource = listaEscolas;
            ddlEscola.SelectedValue = _escolaID;
            ddlEscola.Enabled = false;
        }
        private List<Aluno> ListaAlunos(int escolaID)
        {
            var repo = new Repositorio<Aluno>();
            var parameter = new DynamicParameters();
            parameter.Add("@EscolaID", escolaID);
            return repo.returnListClass("SELECT AlunoID,(Nome + '[' + RA +']') Nome FROM Aluno WHERE EscolaID = @EscolaID ORDER BY Nome", parameter);
        }

        private void ddlEscola_SelectedIndexChanged(object sender, EventArgs e)
        {

            ddlAluno.DataSource = null;
            ddlAluno.Items.Clear();
            txtRA.Text = "";
            ddlSexo.SelectedIndex = -1;
            ddlAluno.DisplayMember = "Nome";
            ddlAluno.ValueMember = "AlunoID";
            ddlAluno.AutoCompleteSource = AutoCompleteSource.ListItems;
            ddlAluno.DataSource = ListaAlunos(_escolaID);
            if (_alunoID != 0 && _biometria)
            {
                ddlAluno.SelectedValue = _alunoID;
                ddlAluno.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAluno.SelectedValue == null)
                return;
            _alunoID = (!_biometria) ? (int)ddlAluno.SelectedValue : _alunoID;
            var repo = new Repositorio<Aluno>();
            var parameter = new DynamicParameters();
            parameter.Add("@AlunoID", _alunoID);
            parameter.Add("@EscolaID", _escolaID);
            _alunoCompleto = repo.returnClass("SELECT * FROM ALUNO WHERE AlunoID = @AlunoID AND EscolaID = @EscolaID", parameter);
            txtRA.Text = _alunoCompleto.Ra;
            ddlSexo.SelectedValue = _alunoCompleto.TipoGenero == Genero.Masculino ? 1 : 2;
            if (_alunoCompleto?.Foto != null)
                pnlFoto.BackgroundImage = Image.FromStream(new MemoryStream(_alunoCompleto.Foto));
            btnIniciarCompra.Visible = true;
            btnCadastrar.Visible = true;
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_alunoID == 0)
                    _alunoID = (int)ddlAluno.SelectedValue;
                Dictionary<int, Template> dictionary = new Dictionary<int, Template>();
                string text = "0000000000";

                AFXOnlineMainClass aFXOnlineMainClass = null;
                object aFXOnlineMain = this.GetAFXOnlineMain();
                if (aFXOnlineMain != null)
                {
                    aFXOnlineMainClass = (aFXOnlineMain as AFXOnlineMainClass);
                }
                if (aFXOnlineMainClass != null)
                {
                    try
                    {
                        this.InitSensor(text, aFXOnlineMainClass);
                    }
                    catch
                    {
                        SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SInitSensorFailed", "Falha ao inicializar o leitor de impressão digital"));
                        return;
                    }
                    if (DialogResult.Cancel == MessageBox.Show("A operação a seguir apagará todas as digitais já cadastradas para esse aluno, deseja continuar mesmo assim?", "Informação", MessageBoxButtons.OKCancel))
                        return;
                    if (aFXOnlineMainClass.Register())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            string regFingerTemplateEx = aFXOnlineMainClass.GetRegFingerTemplateEx("10", j + 1);
                            string regFingerTemplateEx2 = aFXOnlineMainClass.GetRegFingerTemplateEx("9", j + 1);

                            if (!string.IsNullOrEmpty(regFingerTemplateEx) || !string.IsNullOrEmpty(regFingerTemplateEx2))
                            {

                                if (regFingerTemplateEx.Length < 800 || regFingerTemplateEx.Length == regFingerTemplateEx2.Length)
                                {
                                    SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SFingerprintBad", "O modelo de impressão digital está incorreto. Entre em contato com o desenvolvedor imediatamente!"));
                                    return;
                                }
                                if ((regFingerTemplateEx2.Length < 100 && regFingerTemplateEx.Length > 100) || (regFingerTemplateEx2.Length > 100 && regFingerTemplateEx.Length < 100))
                                {
                                    SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFPBadRegister", "O modelo da impressão digital está errado. Por favor, registre-se novamente!"));
                                    text = "0000000000";
                                    break;
                                }
                                if (this.tempList.ContainsKey(j))
                                {
                                    this.tempList.Remove(j);
                                }
                                List<string> list = new List<string>();
                                if (!string.IsNullOrEmpty(regFingerTemplateEx))
                                {
                                    list.Add(regFingerTemplateEx);
                                }
                                else
                                {
                                    list.Add("");
                                }
                                if (!string.IsNullOrEmpty(regFingerTemplateEx2))
                                {
                                    list.Add(regFingerTemplateEx2);
                                }
                                else
                                {
                                    list.Add("");
                                }
                                this.tempList.Add(j, list);
                            }
                        }

                    }

                    if (this.tempList.Count > 0)
                    {
                        var repo = new Repositorio<Biometria>();
                        this.templates.Clear();
                        repo.ExecuteNonQuery("DELETE FROM Biometria WHERE AlunoID = @AlunoID AND EscolaID = @EscolaID " +
                            "AND FuncionarioID = @FuncionarioID AND ResponsavelID = @ResponsavelID",
                            new { AlunoID = _alunoID, EscolaID = _escolaID, FuncionarioID = 0, ResponsavelID = 0 });
                        for (int m = 0; m < 10; m++)
                        {

                            if (this.tempList.ContainsKey(m))
                            {
                                var dedo = this.tempList[m][0];
                                repo.ExecuteNonQuery("INSERT INTO Biometria(BiometriaID, AlunoID, FuncionarioID,ResponsavelID,Dedo,HashDedo,EscolaID)" +
                                    "VALUES(@BiometriaID, @AlunoID, @FuncionarioID,@ResponsavelID,@Dedo,@HashDedo,@EscolaID)",
                                    new { BiometriaID = Guid.NewGuid(), AlunoID = _alunoID, FuncionarioID = 0, ResponsavelID = 0, Dedo = m, HashDedo = dedo, EscolaID = _escolaID });
                            }
                        }
                        MessageBox.Show("Biometrias adicionadas com sucesso!", "Informações");
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                SysDialogs.ShowWarningMessage(ex.Message);
            }
            catch (Exception ex2)
            {
                SysDialogs.ShowWarningMessage(ex2.Message);
            }
        }

        private object GetAFXOnlineMain()
        {
            try
            {
                AFXOnlineMainClass aFXOnlineMainClass = null;
                return new AFXOnlineMainClass();
            }
            catch
            {
                SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInstallSensorDriver", "Por favor, instale o driver do leitor de impressão digital"));
                return null;
            }
        }

        private void InitSensor(string FingerSign, object obj)
        {
            AFXOnlineMainClass aFXOnlineMainClass = null;
            if (obj != null)
            {
                aFXOnlineMainClass = (obj as AFXOnlineMainClass);
            }
            if (aFXOnlineMainClass != null)
            {
                aFXOnlineMainClass.FPEngineVersion = "10";
                aFXOnlineMainClass.EnrollCount = 3;
                try
                {
                    aFXOnlineMainClass.DisableSound = false;
                }
                catch
                {
                }
                aFXOnlineMainClass.SetLanguageFile("online." + "pt");
                aFXOnlineMainClass.SetVerHint = "Register fingerprint";
                aFXOnlineMainClass.IsSupportDuress = true;
                aFXOnlineMainClass.CheckFinger = FingerSign;
            }
        }

        private void btnIniciarCompra_Click(object sender, EventArgs e)
        {
            if ((int)ddlAluno.SelectedValue != 0)
            {
                Process.Start($"{ConfigurationManager.AppSettings["hostRetorno"]}?ID={_alunoID}&identificacaotipo=aluno");
            }
            else
            {
                MessageBox.Show("Selecione um aluno antes de abrir o Ponto de Venda!", "Informações");
            }
        }
    }
}
