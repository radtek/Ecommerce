//using FlexCodeSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using ZK.ConfigManager;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;
using zkonline;
using System.Data.SqlClient;
using PortalEducacional.Models;
using Fingerprint;

namespace FingerprintClass
{
    public partial class frmBiometria : Form
    {
        private int _escolaID;
        private Dictionary<int, List<string>> tempList = new Dictionary<int, List<string>>();
        private string m_oldfingerSign;
        private Dictionary<int, List<byte[]>> templates = new Dictionary<int, List<byte[]>>();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public frmBiometria()
        {
            InitializeComponent();
            _escolaID = int.Parse(ConfigurationManager.AppSettings["escolaID"]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HabilitarUC(true);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {

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
                    if (aFXOnlineMainClass.Register())
                    {
                        var i = 0;
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
                        for (int m = 0; m < 10; m++)
                        {

                            if (this.tempList.ContainsKey(m))
                            {
                                var dedo = this.tempList[m][0];
                                repo.ExecuteNonQuery("INSERT INTO Biometria(BiometriaID, AlunoID, FuncionarioID,ResponsavelID,Dedo,HashDedo,EscolaID)" +
                                    "VALUES(@BiometriaID, @AlunoID, @FuncionarioID,@ResponsavelID,@Dedo,@HashDedo,@EscolaID)",
                                    new { BiometriaID = Guid.NewGuid(), AlunoID = 3, FuncionarioID = 0, ResponsavelID = 0, Dedo = m, HashDedo = dedo, EscolaID = _escolaID });
                            }
                        }
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

        private void btnBuscarAluno_Click(object sender, EventArgs e)
        {            
            HabilitarUC(true);
        }

        private void HabilitarUC(bool habilitar)
        {
            ucBuscaManual1.Visible = habilitar;
            btnBuscarAluno.Visible = !habilitar;
            btnBuscarBiometria.Visible = !habilitar;
            pboxAluno.Visible = !habilitar;
            pboxBiometria.Visible = !habilitar;
            btnVoltar.Visible = habilitar;
            if (habilitar)
            {
                ucBuscaManual1.Inicializar();
            }
        }

        private void btnBuscarBiometria_Click(object sender, EventArgs e)
        {
            AFXOnlineMainClass aFXOnlineMainClass = null;
            object aFXOnlineMain = this.GetAFXOnlineMain();
            string text = "0000000000";
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
                var dedoA = aFXOnlineMainClass.GetVerTemplate();
                var i = 0;
                while (aFXOnlineMainClass.VerifyTemplate == null)
                {
                    aFXOnlineMainClass.GetVerTemplate();
                    i++;
                    if (i > 2)
                        break;
                }
                if (aFXOnlineMainClass.VerifyTemplate == null)
                {
                    MessageBox.Show($"Usuário Não Encontrado!");
                    return;
                }
                var repo = new Repositorio<Biometria>();
                var parameter = new Dapper.DynamicParameters();
                parameter.Add("@EscolaID", _escolaID);
                var resultados = repo.returnListClass("SELECT BiometriaID,HashDedo,AlunoID,EscolaID,FuncionarioID FROM Biometria WHERE EscolaID = @EscolaID", parameter);
                var resposta = new Biometria();
                var resultado = false;
                foreach (var item in resultados)
                {
                    resultado = aFXOnlineMainClass.MatchFinger(item.HashDedo, aFXOnlineMainClass.VerifyTemplate);
                    if (resultado)
                    {
                        resposta = item;
                        break;
                    }
                }
                if (resultado)
                {
                    HabilitarUC(true);
                    ucBuscaManual1.Inicializar(resposta.AlunoID);
                }

                else
                    MessageBox.Show($"Usuário Não Encontrado!");

            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBiometria));
            this.pboxAluno = new System.Windows.Forms.PictureBox();
            this.btnBuscarAluno = new System.Windows.Forms.Button();
            this.pboxBiometria = new System.Windows.Forms.PictureBox();
            this.btnBuscarBiometria = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnVoltar = new System.Windows.Forms.Button();
            this.ucBuscaManual1 = new FingerprintClass.UCBuscaManual();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pboxAluno)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxBiometria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // pboxAluno
            // 
            this.pboxAluno.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pboxAluno.BackgroundImage")));
            this.pboxAluno.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pboxAluno.Location = new System.Drawing.Point(187, 90);
            this.pboxAluno.Name = "pboxAluno";
            this.pboxAluno.Size = new System.Drawing.Size(430, 176);
            this.pboxAluno.TabIndex = 2;
            this.pboxAluno.TabStop = false;
            // 
            // btnBuscarAluno
            // 
            this.btnBuscarAluno.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBuscarAluno.BackgroundImage")));
            this.btnBuscarAluno.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBuscarAluno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarAluno.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnBuscarAluno.Location = new System.Drawing.Point(384, 125);
            this.btnBuscarAluno.Name = "btnBuscarAluno";
            this.btnBuscarAluno.Size = new System.Drawing.Size(187, 72);
            this.btnBuscarAluno.TabIndex = 3;
            this.btnBuscarAluno.UseVisualStyleBackColor = true;
            this.btnBuscarAluno.Click += new System.EventHandler(this.btnBuscarAluno_Click);
            // 
            // pboxBiometria
            // 
            this.pboxBiometria.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pboxBiometria.BackgroundImage")));
            this.pboxBiometria.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pboxBiometria.Location = new System.Drawing.Point(187, 272);
            this.pboxBiometria.Name = "pboxBiometria";
            this.pboxBiometria.Size = new System.Drawing.Size(430, 215);
            this.pboxBiometria.TabIndex = 4;
            this.pboxBiometria.TabStop = false;
            // 
            // btnBuscarBiometria
            // 
            this.btnBuscarBiometria.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBuscarBiometria.BackgroundImage")));
            this.btnBuscarBiometria.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBuscarBiometria.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarBiometria.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnBuscarBiometria.Location = new System.Drawing.Point(380, 327);
            this.btnBuscarBiometria.Name = "btnBuscarBiometria";
            this.btnBuscarBiometria.Size = new System.Drawing.Size(191, 71);
            this.btnBuscarBiometria.TabIndex = 5;
            this.btnBuscarBiometria.UseVisualStyleBackColor = true;
            this.btnBuscarBiometria.Click += new System.EventHandler(this.btnBuscarBiometria_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox3.Location = new System.Drawing.Point(0, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(691, 60);
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox3_MouseDown);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox4.Location = new System.Drawing.Point(0, 59);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(82, 473);
            this.pictureBox4.TabIndex = 7;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox5.BackgroundImage")));
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.Location = new System.Drawing.Point(0, 0);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(164, 160);
            this.pictureBox5.TabIndex = 8;
            this.pictureBox5.TabStop = false;
            // 
            // btnMinimizar
            // 
            this.btnMinimizar.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnMinimizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimizar.Location = new System.Drawing.Point(594, 13);
            this.btnMinimizar.Name = "btnMinimizar";
            this.btnMinimizar.Size = new System.Drawing.Size(39, 31);
            this.btnMinimizar.TabIndex = 9;
            this.btnMinimizar.Text = "-";
            this.btnMinimizar.UseVisualStyleBackColor = false;
            this.btnMinimizar.Click += new System.EventHandler(this.btnMinimizar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Font = new System.Drawing.Font("Verdana", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(275, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 29);
            this.label1.TabIndex = 10;
            this.label1.Text = "My Bonus";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            // 
            // btnVoltar
            // 
            this.btnVoltar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnVoltar.BackgroundImage")));
            this.btnVoltar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVoltar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVoltar.ForeColor = System.Drawing.SystemColors.Control;
            this.btnVoltar.Location = new System.Drawing.Point(556, 90);
            this.btnVoltar.Name = "btnVoltar";
            this.btnVoltar.Size = new System.Drawing.Size(92, 32);
            this.btnVoltar.TabIndex = 12;
            this.btnVoltar.UseVisualStyleBackColor = true;
            this.btnVoltar.Visible = false;
            this.btnVoltar.Click += new System.EventHandler(this.btnVoltar_Click);
            // 
            // ucBuscaManual1
            // 
            this.ucBuscaManual1.Location = new System.Drawing.Point(84, 166);
            this.ucBuscaManual1.Name = "ucBuscaManual1";
            this.ucBuscaManual1.Size = new System.Drawing.Size(594, 366);
            this.ucBuscaManual1.TabIndex = 13;
            this.ucBuscaManual1.Visible = false;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click_1);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(639, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 31);
            this.button1.TabIndex = 14;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // frmBiometria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(690, 533);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ucBuscaManual1);
            this.Controls.Add(this.btnVoltar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMinimizar);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.btnBuscarBiometria);
            this.Controls.Add(this.pboxBiometria);
            this.Controls.Add(this.btnBuscarAluno);
            this.Controls.Add(this.pboxAluno);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmBiometria";
            this.Text = "Sistema Cantina";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBiometria_FormClosing);
            this.SizeChanged += new System.EventHandler(this.frmBiometria_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pboxAluno)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxBiometria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            HabilitarUC(false);
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #region Minimizar Tela

        private void frmBiometria_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (FormWindowState.Minimized == this.WindowState)
                {
                    notifyIcon1.Icon = this.Icon; //SystemIcons.Application;
                    notifyIcon1.BalloonTipText = @"My Bonus foi minimizado!";
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(500);
                    this.Hide();
                }
                else if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon1.Visible = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void frmBiometria_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void notifyIcon1_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon1.Icon = this.Icon; //SystemIcons.Application;
                    notifyIcon1.BalloonTipText = @"My Bonus foi minimizado!";
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(500);
                    this.Hide();
                }
                else if (FormWindowState.Normal == this.WindowState)
                {
                    notifyIcon1.Visible = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }




        #endregion

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
