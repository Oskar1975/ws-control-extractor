namespace ExtraxtorWS
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.btnConectar = new System.Windows.Forms.Button();
            this.grdEmpresa = new System.Windows.Forms.DataGridView();
            this.btnConsultarEmp = new System.Windows.Forms.Button();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.btnFusionCompanies = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdEmpresa)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(602, 163);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 52);
            this.button1.TabIndex = 2;
            this.button1.Text = "Mostrar empresas";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnConectar
            // 
            this.btnConectar.Location = new System.Drawing.Point(351, 47);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(75, 23);
            this.btnConectar.TabIndex = 3;
            this.btnConectar.Text = "Conexion";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // grdEmpresa
            // 
            this.grdEmpresa.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdEmpresa.Location = new System.Drawing.Point(44, 348);
            this.grdEmpresa.Name = "grdEmpresa";
            this.grdEmpresa.Size = new System.Drawing.Size(717, 312);
            this.grdEmpresa.TabIndex = 4;
            // 
            // btnConsultarEmp
            // 
            this.btnConsultarEmp.Location = new System.Drawing.Point(389, 163);
            this.btnConsultarEmp.Name = "btnConsultarEmp";
            this.btnConsultarEmp.Size = new System.Drawing.Size(177, 52);
            this.btnConsultarEmp.TabIndex = 5;
            this.btnConsultarEmp.Text = "Consulta empresa";
            this.btnConsultarEmp.UseVisualStyleBackColor = true;
            this.btnConsultarEmp.Click += new System.EventHandler(this.btnConsultarEmp_Click);
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(143, 194);
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(207, 20);
            this.txtClave.TabIndex = 6;
            // 
            // btnFusionCompanies
            // 
            this.btnFusionCompanies.Location = new System.Drawing.Point(389, 276);
            this.btnFusionCompanies.Name = "btnFusionCompanies";
            this.btnFusionCompanies.Size = new System.Drawing.Size(177, 34);
            this.btnFusionCompanies.TabIndex = 7;
            this.btnFusionCompanies.Text = "FusionCompanies";
            this.btnFusionCompanies.UseVisualStyleBackColor = true;
            this.btnFusionCompanies.Click += new System.EventHandler(this.btnFusionCompanies_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(583, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(159, 86);
            this.button3.TabIndex = 9;
            this.button3.Text = "Ciclo";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 672);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnFusionCompanies);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.btnConsultarEmp);
            this.Controls.Add(this.grdEmpresa);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.grdEmpresa)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.DataGridView grdEmpresa;
        private System.Windows.Forms.Button btnConsultarEmp;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Button btnFusionCompanies;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

