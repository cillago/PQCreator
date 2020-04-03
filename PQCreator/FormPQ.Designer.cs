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
            this.bcreaodf = new System.Windows.Forms.Button();
            this.richTextBoxTAG = new System.Windows.Forms.RichTextBox();
            this.bCreaRtf = new System.Windows.Forms.Button();
            this.BConvert = new System.Windows.Forms.Button();
            this.bimportRTF = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.buttomIDML = new System.Windows.Forms.Button();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.TPQTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TISBN = new System.Windows.Forms.TextBox();
            this.checkSanti = new System.Windows.Forms.CheckBox();
            this.tDirSanti = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bcreaodf
            // 
            this.bcreaodf.Location = new System.Drawing.Point(16, 58);
            this.bcreaodf.Margin = new System.Windows.Forms.Padding(2);
            this.bcreaodf.Name = "bcreaodf";
            this.bcreaodf.Size = new System.Drawing.Size(108, 35);
            this.bcreaodf.TabIndex = 0;
            this.bcreaodf.Text = "Importa ODT ufficiale";
            this.bcreaodf.UseVisualStyleBackColor = true;
            this.bcreaodf.Click += new System.EventHandler(this.bcreaodf_Click);
            // 
            // richTextBoxTAG
            // 
            this.richTextBoxTAG.Location = new System.Drawing.Point(345, 83);
            this.richTextBoxTAG.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBoxTAG.Name = "richTextBoxTAG";
            this.richTextBoxTAG.Size = new System.Drawing.Size(117, 31);
            this.richTextBoxTAG.TabIndex = 1;
            this.richTextBoxTAG.Text = "";
            this.richTextBoxTAG.Visible = false;
            // 
            // bCreaRtf
            // 
            this.bCreaRtf.Location = new System.Drawing.Point(347, 18);
            this.bCreaRtf.Margin = new System.Windows.Forms.Padding(2);
            this.bCreaRtf.Name = "bCreaRtf";
            this.bCreaRtf.Size = new System.Drawing.Size(108, 40);
            this.bCreaRtf.TabIndex = 2;
            this.bCreaRtf.Text = "Crea RTF";
            this.bCreaRtf.UseVisualStyleBackColor = true;
            this.bCreaRtf.Click += new System.EventHandler(this.bCreaRtf_Click);
            // 
            // BConvert
            // 
            this.BConvert.Location = new System.Drawing.Point(16, 162);
            this.BConvert.Margin = new System.Windows.Forms.Padding(2);
            this.BConvert.Name = "BConvert";
            this.BConvert.Size = new System.Drawing.Size(108, 40);
            this.BConvert.TabIndex = 4;
            this.BConvert.Text = "Converti Santi";
            this.BConvert.UseVisualStyleBackColor = true;
            this.BConvert.Click += new System.EventHandler(this.bConvert_Click);
            // 
            // bimportRTF
            // 
            this.bimportRTF.Location = new System.Drawing.Point(159, 58);
            this.bimportRTF.Margin = new System.Windows.Forms.Padding(2);
            this.bimportRTF.Name = "bimportRTF";
            this.bimportRTF.Size = new System.Drawing.Size(108, 35);
            this.bimportRTF.TabIndex = 5;
            this.bimportRTF.Text = "Importa IDML definitivo";
            this.bimportRTF.UseVisualStyleBackColor = true;
            this.bimportRTF.Click += new System.EventHandler(this.bimportIDML_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(159, 103);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(108, 40);
            this.button4.TabIndex = 6;
            this.button4.Text = "Salva EPUB/MOBI";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.creaEPUB_Click);
            // 
            // buttomIDML
            // 
            this.buttomIDML.Location = new System.Drawing.Point(16, 103);
            this.buttomIDML.Margin = new System.Windows.Forms.Padding(2);
            this.buttomIDML.Name = "buttomIDML";
            this.buttomIDML.Size = new System.Drawing.Size(108, 40);
            this.buttomIDML.TabIndex = 7;
            this.buttomIDML.Text = "Salva IDML";
            this.buttomIDML.UseVisualStyleBackColor = true;
            this.buttomIDML.Click += new System.EventHandler(this.buttomIDML_Click);
            // 
            // OpenDialog
            // 
            this.OpenDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenDialog_FileOk);
            // 
            // SaveDialog
            // 
            this.SaveDialog.FileName = "PQ";
            // 
            // TPQTitle
            // 
            this.TPQTitle.Location = new System.Drawing.Point(49, 8);
            this.TPQTitle.Margin = new System.Windows.Forms.Padding(2);
            this.TPQTitle.Name = "TPQTitle";
            this.TPQTitle.Size = new System.Drawing.Size(219, 20);
            this.TPQTitle.TabIndex = 8;
            this.TPQTitle.Text = "Pane Quotidiano Gennaio Febbraio 2018";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Titolo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "ISBN";
            // 
            // TISBN
            // 
            this.TISBN.Location = new System.Drawing.Point(49, 29);
            this.TISBN.Margin = new System.Windows.Forms.Padding(2);
            this.TISBN.Name = "TISBN";
            this.TISBN.Size = new System.Drawing.Size(219, 20);
            this.TISBN.TabIndex = 10;
            this.TISBN.Text = "9788889807415";
            this.TISBN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkSanti
            // 
            this.checkSanti.AutoSize = true;
            this.checkSanti.Checked = true;
            this.checkSanti.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSanti.Location = new System.Drawing.Point(159, 162);
            this.checkSanti.Margin = new System.Windows.Forms.Padding(2);
            this.checkSanti.Name = "checkSanti";
            this.checkSanti.Size = new System.Drawing.Size(84, 17);
            this.checkSanti.TabIndex = 12;
            this.checkSanti.Text = "Includi Santi";
            this.checkSanti.UseVisualStyleBackColor = true;
            // 
            // tDirSanti
            // 
            this.tDirSanti.Location = new System.Drawing.Point(65, 207);
            this.tDirSanti.Name = "tDirSanti";
            this.tDirSanti.Size = new System.Drawing.Size(202, 20);
            this.tDirSanti.TabIndex = 13;
            this.tDirSanti.Text = "c:\\Users\\AlyFabio\\Google Drive\\pq\\pqmaggiu2018\\santi\\";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "dir Santi";
            // 
            // FormPQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 234);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tDirSanti);
            this.Controls.Add(this.checkSanti);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TISBN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TPQTitle);
            this.Controls.Add(this.buttomIDML);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.bimportRTF);
            this.Controls.Add(this.BConvert);
            this.Controls.Add(this.bCreaRtf);
            this.Controls.Add(this.richTextBoxTAG);
            this.Controls.Add(this.bcreaodf);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPQ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pane Quotidiano";
            this.Load += new System.EventHandler(this.FormPQ_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bcreaodf;
        private System.Windows.Forms.RichTextBox richTextBoxTAG;
        private System.Windows.Forms.Button bCreaRtf;
        private System.Windows.Forms.Button BConvert;
        private System.Windows.Forms.Button bimportRTF;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button buttomIDML;
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private System.Windows.Forms.SaveFileDialog SaveDialog;
        private System.Windows.Forms.TextBox TPQTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TISBN;
        private System.Windows.Forms.CheckBox checkSanti;
        private System.Windows.Forms.TextBox tDirSanti;
        private System.Windows.Forms.Label label3;
    }
}

