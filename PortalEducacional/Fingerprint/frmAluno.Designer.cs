namespace Fingerprint
{
    partial class frmAluno
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ddlEscola = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ddlAluno = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.txtRA = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ddlSexo = new System.Windows.Forms.ComboBox();
            this.btnCadastrar = new System.Windows.Forms.Button();
            this.pnlFoto = new System.Windows.Forms.Panel();
            this.btnIniciarCompra = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ddlEscola
            // 
            this.ddlEscola.FormattingEnabled = true;
            this.ddlEscola.Items.AddRange(new object[] {
            "-- Selecione --"});
            this.ddlEscola.Location = new System.Drawing.Point(31, 35);
            this.ddlEscola.Name = "ddlEscola";
            this.ddlEscola.Size = new System.Drawing.Size(464, 24);
            this.ddlEscola.TabIndex = 0;
            this.ddlEscola.SelectedIndexChanged += new System.EventHandler(this.ddlEscola_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Escola:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Aluno:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // ddlAluno
            // 
            this.ddlAluno.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.ddlAluno.FormattingEnabled = true;
            this.ddlAluno.Location = new System.Drawing.Point(31, 86);
            this.ddlAluno.Name = "ddlAluno";
            this.ddlAluno.Size = new System.Drawing.Size(464, 24);
            this.ddlAluno.TabIndex = 4;
            this.ddlAluno.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(31, 86);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(464, 25);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // txtRA
            // 
            this.txtRA.Enabled = false;
            this.txtRA.Location = new System.Drawing.Point(31, 149);
            this.txtRA.Name = "txtRA";
            this.txtRA.Size = new System.Drawing.Size(166, 22);
            this.txtRA.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "RA";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Sexo";
            // 
            // ddlSexo
            // 
            this.ddlSexo.Enabled = false;
            this.ddlSexo.FormattingEnabled = true;
            this.ddlSexo.Location = new System.Drawing.Point(31, 207);
            this.ddlSexo.Name = "ddlSexo";
            this.ddlSexo.Size = new System.Drawing.Size(166, 24);
            this.ddlSexo.TabIndex = 8;
            // 
            // btnCadastrar
            // 
            this.btnCadastrar.Location = new System.Drawing.Point(31, 262);
            this.btnCadastrar.Name = "btnCadastrar";
            this.btnCadastrar.Size = new System.Drawing.Size(161, 39);
            this.btnCadastrar.TabIndex = 9;
            this.btnCadastrar.Text = "Cadastrar Digitais";
            this.btnCadastrar.UseVisualStyleBackColor = true;
            this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
            // 
            // pnlFoto
            // 
            this.pnlFoto.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pnlFoto.Location = new System.Drawing.Point(541, 35);
            this.pnlFoto.Name = "pnlFoto";
            this.pnlFoto.Size = new System.Drawing.Size(162, 211);
            this.pnlFoto.TabIndex = 10;
            // 
            // btnIniciarCompra
            // 
            this.btnIniciarCompra.Location = new System.Drawing.Point(541, 262);
            this.btnIniciarCompra.Name = "btnIniciarCompra";
            this.btnIniciarCompra.Size = new System.Drawing.Size(162, 39);
            this.btnIniciarCompra.TabIndex = 11;
            this.btnIniciarCompra.Text = "Iniciar Compra";
            this.btnIniciarCompra.UseVisualStyleBackColor = true;
            this.btnIniciarCompra.Click += new System.EventHandler(this.btnIniciarCompra_Click);
            // 
            // frmAluno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 313);
            this.Controls.Add(this.btnIniciarCompra);
            this.Controls.Add(this.pnlFoto);
            this.Controls.Add(this.btnCadastrar);
            this.Controls.Add(this.ddlSexo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtRA);
            this.Controls.Add(this.ddlAluno);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlEscola);
            this.Name = "frmAluno";
            this.Text = "Aluno";
            this.Load += new System.EventHandler(this.frmAluno_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlEscola;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox ddlAluno;
        private System.Windows.Forms.TextBox txtRA;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlSexo;
        private System.Windows.Forms.Button btnCadastrar;
        private System.Windows.Forms.Panel pnlFoto;
        private System.Windows.Forms.Button btnIniciarCompra;
    }
}