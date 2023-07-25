namespace PQCreator
{
    partial class FormPQ
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
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBoxTAG = new System.Windows.Forms.RichTextBox();
            this.bNitf = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tDirSanti = new System.Windows.Forms.TextBox();
            this.checkSanti = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TISBN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TPQTitle = new System.Windows.Forms.TextBox();
            this.buttomIDML = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.bimportRTF = new System.Windows.Forms.Button();
            this.BConvert = new System.Windows.Forms.Button();
            this.bcreaodf = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.bnitflibro = new System.Windows.Forms.Button();
            this.BImpBook = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenDialog
            // 
            this.OpenDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenDialog_FileOk);
            // 
            // SaveDialog
            // 
            this.SaveDialog.FileName = "PQ";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.richTextBoxTAG);
            this.panel1.Controls.Add(this.bNitf);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tDirSanti);
            this.panel1.Controls.Add(this.checkSanti);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.TISBN);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.TPQTitle);
            this.panel1.Controls.Add(this.buttomIDML);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.bimportRTF);
            this.panel1.Controls.Add(this.BConvert);
            this.panel1.Controls.Add(this.bcreaodf);
            this.panel1.Location = new System.Drawing.Point(8, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(353, 304);
            this.panel1.TabIndex = 16;
            // 
            // richTextBoxTAG
            // 
            this.richTextBoxTAG.Location = new System.Drawing.Point(197, 242);
            this.richTextBoxTAG.Name = "richTextBoxTAG";
            this.richTextBoxTAG.Size = new System.Drawing.Size(144, 28);
            this.richTextBoxTAG.TabIndex = 29;
            this.richTextBoxTAG.Text = "";
            this.richTextBoxTAG.Visible = false;
            // 
            // bNitf
            // 
            this.bNitf.Location = new System.Drawing.Point(197, 189);
            this.bNitf.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bNitf.Name = "bNitf";
            this.bNitf.Size = new System.Drawing.Size(144, 49);
            this.bNitf.TabIndex = 28;
            this.bNitf.Text = "Salva ZIP NITF";
            this.bNitf.UseVisualStyleBackColor = true;
            this.bNitf.Click += new System.EventHandler(this.bNitf_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 280);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 16);
            this.label3.TabIndex = 27;
            this.label3.Text = "dir Santi";
            // 
            // tDirSanti
            // 
            this.tDirSanti.Location = new System.Drawing.Point(73, 277);
            this.tDirSanti.Margin = new System.Windows.Forms.Padding(4);
            this.tDirSanti.Name = "tDirSanti";
            this.tDirSanti.Size = new System.Drawing.Size(268, 22);
            this.tDirSanti.TabIndex = 26;
            this.tDirSanti.Text = "c:\\Users\\AlyFabio\\Google Drive\\pq\\pqmaggiu2018\\santi\\";
            // 
            // checkSanti
            // 
            this.checkSanti.AutoSize = true;
            this.checkSanti.Checked = true;
            this.checkSanti.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSanti.Location = new System.Drawing.Point(10, 251);
            this.checkSanti.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkSanti.Name = "checkSanti";
            this.checkSanti.Size = new System.Drawing.Size(100, 20);
            this.checkSanti.TabIndex = 25;
            this.checkSanti.Text = "Includi Santi";
            this.checkSanti.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "ISBN";
            // 
            // TISBN
            // 
            this.TISBN.Location = new System.Drawing.Point(51, 32);
            this.TISBN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TISBN.Name = "TISBN";
            this.TISBN.Size = new System.Drawing.Size(291, 22);
            this.TISBN.TabIndex = 23;
            this.TISBN.Text = "9788889807415";
            this.TISBN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Titolo";
            // 
            // TPQTitle
            // 
            this.TPQTitle.Location = new System.Drawing.Point(51, 6);
            this.TPQTitle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TPQTitle.Name = "TPQTitle";
            this.TPQTitle.Size = new System.Drawing.Size(291, 22);
            this.TPQTitle.TabIndex = 21;
            this.TPQTitle.Text = "Pane Quotidiano Gennaio Febbraio 2023";
            // 
            // buttomIDML
            // 
            this.buttomIDML.Location = new System.Drawing.Point(7, 123);
            this.buttomIDML.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttomIDML.Name = "buttomIDML";
            this.buttomIDML.Size = new System.Drawing.Size(144, 49);
            this.buttomIDML.TabIndex = 20;
            this.buttomIDML.Text = "Salva IDML";
            this.buttomIDML.UseVisualStyleBackColor = true;
            this.buttomIDML.Click += new System.EventHandler(this.buttomIDML_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(198, 123);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(144, 49);
            this.button4.TabIndex = 19;
            this.button4.Text = "Salva EPUB/MOBI";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.creaEPUB_Click);
            // 
            // bimportRTF
            // 
            this.bimportRTF.Location = new System.Drawing.Point(198, 67);
            this.bimportRTF.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bimportRTF.Name = "bimportRTF";
            this.bimportRTF.Size = new System.Drawing.Size(144, 43);
            this.bimportRTF.TabIndex = 18;
            this.bimportRTF.Text = "Importa IDML definitivo";
            this.bimportRTF.UseVisualStyleBackColor = true;
            this.bimportRTF.Click += new System.EventHandler(this.bimportIDML_Click);
            // 
            // BConvert
            // 
            this.BConvert.Location = new System.Drawing.Point(7, 189);
            this.BConvert.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BConvert.Name = "BConvert";
            this.BConvert.Size = new System.Drawing.Size(144, 49);
            this.BConvert.TabIndex = 17;
            this.BConvert.Text = "Converti Santi";
            this.BConvert.UseVisualStyleBackColor = true;
            this.BConvert.Click += new System.EventHandler(this.bConvert_Click);
            // 
            // bcreaodf
            // 
            this.bcreaodf.Location = new System.Drawing.Point(7, 68);
            this.bcreaodf.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bcreaodf.Name = "bcreaodf";
            this.bcreaodf.Size = new System.Drawing.Size(144, 43);
            this.bcreaodf.TabIndex = 16;
            this.bcreaodf.Text = "Importa ODT ufficiale";
            this.bcreaodf.UseVisualStyleBackColor = true;
            this.bcreaodf.Click += new System.EventHandler(this.bcreaodf_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.bnitflibro);
            this.panel2.Controls.Add(this.BImpBook);
            this.panel2.Location = new System.Drawing.Point(367, 9);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(154, 304);
            this.panel2.TabIndex = 17;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(4, 113);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(147, 46);
            this.button1.TabIndex = 2;
            this.button1.Text = "Importa Sempre TXT";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bnitflibro
            // 
            this.bnitflibro.Location = new System.Drawing.Point(0, 61);
            this.bnitflibro.Name = "bnitflibro";
            this.bnitflibro.Size = new System.Drawing.Size(147, 46);
            this.bnitflibro.TabIndex = 1;
            this.bnitflibro.Text = "Salva NITF Libro";
            this.bnitflibro.UseVisualStyleBackColor = true;
            this.bnitflibro.Click += new System.EventHandler(this.bnitflibro_Click);
            // 
            // BImpBook
            // 
            this.BImpBook.Location = new System.Drawing.Point(4, 9);
            this.BImpBook.Name = "BImpBook";
            this.BImpBook.Size = new System.Drawing.Size(147, 46);
            this.BImpBook.TabIndex = 0;
            this.BImpBook.Text = "Importa Libro XML";
            this.BImpBook.UseVisualStyleBackColor = true;
            this.BImpBook.Click += new System.EventHandler(this.BImpBook_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 254);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(147, 46);
            this.button2.TabIndex = 3;
            this.button2.Text = "Unisci ODT PQ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormPQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 316);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPQ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pane Quotidiano";
            this.Load += new System.EventHandler(this.FormPQ_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private System.Windows.Forms.SaveFileDialog SaveDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bNitf;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tDirSanti;
        private System.Windows.Forms.CheckBox checkSanti;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TISBN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TPQTitle;
        private System.Windows.Forms.Button buttomIDML;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button bimportRTF;
        private System.Windows.Forms.Button BConvert;
        private System.Windows.Forms.Button bcreaodf;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button BImpBook;
        private System.Windows.Forms.Button bnitflibro;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBoxTAG;
        private System.Windows.Forms.Button button2;
    }
}

