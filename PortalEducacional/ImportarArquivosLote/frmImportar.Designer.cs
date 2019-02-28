namespace ImportarArquivosLote
{
    partial class frmImportar
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
            this.txtBrowse = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnImportar = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnMigrarAlunos = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnMigrafotoAluno = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtBrowse
            // 
            this.txtBrowse.Location = new System.Drawing.Point(16, 67);
            this.txtBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtBrowse.Name = "txtBrowse";
            this.txtBrowse.Size = new System.Drawing.Size(282, 20);
            this.txtBrowse.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(310, 67);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(56, 19);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pesquisar arquivos";
            // 
            // btnImportar
            // 
            this.btnImportar.Location = new System.Drawing.Point(258, 100);
            this.btnImportar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(109, 24);
            this.btnImportar.TabIndex = 5;
            this.btnImportar.Text = "Importar arquivos";
            this.btnImportar.UseVisualStyleBackColor = true;
            this.btnImportar.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnMigrarAlunos
            // 
            this.btnMigrarAlunos.Location = new System.Drawing.Point(16, 10);
            this.btnMigrarAlunos.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnMigrarAlunos.Name = "btnMigrarAlunos";
            this.btnMigrarAlunos.Size = new System.Drawing.Size(95, 24);
            this.btnMigrarAlunos.TabIndex = 6;
            this.btnMigrarAlunos.Text = "Migrar Alunos";
            this.btnMigrarAlunos.UseVisualStyleBackColor = true;
            this.btnMigrarAlunos.Click += new System.EventHandler(this.btnMigrarAlunos_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(184, 15);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 20);
            this.button1.TabIndex = 7;
            this.button1.Text = "INSERT ALUNOS";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnMigrafotoAluno
            // 
            this.btnMigrafotoAluno.Location = new System.Drawing.Point(242, 149);
            this.btnMigrafotoAluno.Name = "btnMigrafotoAluno";
            this.btnMigrafotoAluno.Size = new System.Drawing.Size(75, 23);
            this.btnMigrafotoAluno.TabIndex = 8;
            this.btnMigrafotoAluno.Text = "migrar foto aluno";
            this.btnMigrafotoAluno.UseVisualStyleBackColor = true;
            this.btnMigrafotoAluno.Click += new System.EventHandler(this.btnMigrafotoAluno_Click);
            // 
            // frmImportar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 203);
            this.Controls.Add(this.btnMigrafotoAluno);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnMigrarAlunos);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtBrowse);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmImportar";
            this.Text = "Importar arquivos";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBrowse;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnMigrarAlunos;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnMigrafotoAluno;
    }
}

