using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;
using System.Net;
using System.Linq;
using System.Web.UI.WebControls;

enum Section
{
    giorno = 0,
    primaLettura = 1,
    salmo = 2,
    secondalettura = 3,
    antifonaVangelo = 4,
    vangelo = 5,
    primaLetturaOppure = 6,
    secondaLetturaOppure = 7,
    vangeloOppure = 8
}

namespace PQCreator
{
    public partial class FormPQ : Form
    {
        ODTImporter odtI;
        PQImporter PQI;
        RTFTXTWriter rtxS;
        EPUBWriter epubW;
        NITFWriter nitfW;
        IDMLWriter idmlW;
        public FormPQ()
        {
            InitializeComponent();
        }

        private void FormPQ_Load(object sender, EventArgs e)
        {


        }

        private void bcreaodf_Click(object sender, EventArgs e)
        {
            OpenDialog.Filter = "DOCX Files|*.docx|All Files|*.*";
            OpenDialog.Multiselect = false;
            OpenDialog.FileName = "";

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                //CARICO TXT
                string problems;
                odtI = new ODTImporter();
                //   odtI.OpenDOCX((object)OpenDialog.FileName);
                odtI.OpenODT(OpenDialog.FileName);
                //odtI.OpenTXT(OpenDialog.FileName);
                problems = odtI.PutTag();

                //Creo RTF
                //CreaRTF();

                File.WriteAllText("PQLOG.txt", problems);
                if (problems.Length > 0) MessageBox.Show(problems);
                else MessageBox.Show("OK");
            }
        }

        //private void CreaRTF()
        //{
        //    rtxS = new RTFTXTWriter();
        //    rtxS.LoadRTFStyle(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "style_rtf.csv"));
        //    rtxS.populateRTFBox(odtI.dicPQTag, ref richTextBoxTAG);
        //}

        //private void CreaRTFIDML()
        //{
        //    rtxS = new RTFTXTWriter();
        //    rtxS.LoadRTFStyle(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "style_rtf.csv"));
        //    rtxS.populateRTFBoxIDML(PQI.dicPQTag, ref richTextBoxTAG);
        //}

        //private void bCreaRtf_Click(object sender, EventArgs e)
        //{
        //    CreaRTF();
        //}



        private void bConvert_Click(object sender, EventArgs e)
        {
            //if (rtxS == null) 
            //{
            //    MessageBox.Show("File ODT non caricato"); 
            //    return;
            //}

            //SaveDialog.Filter="RTF File|*.rtf";
            //SaveDialog.FileName = "PQ";
            //if (SaveDialog.ShowDialog() == DialogResult.OK)
            //{
            //    rtxS.Save(SaveDialog.FileName, ref richTextBoxTAG, RichTextBoxStreamType.RichText);
            //    MessageBox.Show("File RTF salvato");
            //}

            //if (rtxS == null)
            //{
            //    MessageBox.Show("File ODT non caricato");
            //    return;
            //}

            //SaveDialog.Filter = "TXT File|*.txt";
            //if (SaveDialog.ShowDialog() == DialogResult.OK)
            //{
            //    rtxS.Save(SaveDialog.FileName, ref richTextBoxTAG, RichTextBoxStreamType.PlainText);
            //    MessageBox.Show("File TXT salvato");
            //}
            //else MessageBox.Show("File ODT non caricato");

            string[] filesSanti;

            //using (var fbd = new FolderBrowserDialog())
            //{

            //    DialogResult result = fbd.ShowDialog();

            //    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        filesSanti = Directory.GetFiles(fbd.SelectedPath, "*.doc*");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Nessun file");
            //        return;
            //    }
            //}
            string dirSanti = tDirSanti.Text;
            filesSanti = Directory.GetFiles(dirSanti, "*.doc*");
            Word.Application word = new Word.Application();
            string problems = "";
            for (int i = 0; i < filesSanti.Length; i++)
            {
                Word.Document doc = new Word.Document();
                try
                {
                    object fileName = filesSanti[i];
                    // Define an object to pass to the API for missing parameters
                    object missing = System.Type.Missing;
                    doc = word.Documents.Open(ref fileName,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing);

                    String read = string.Empty;
                    List<string> lines = new List<string>();
                    for (int j = 1; j <= doc.Paragraphs.Count; j++)
                    {
                        string temp = doc.Paragraphs[j].Range.Text.Trim();
                        lines.Add(temp);
                    }
                    doc.Close();
                    doc = null;
                    problems += ImportSanti(lines, filesSanti[i]);


                }
                catch (Exception ex)
                {
                    problems += ex.Message;
                    if (doc != null) doc.Close();
                }

            }
            word.Quit();
            File.WriteAllText("PQLOG.txt", problems);
            if (problems.Length > 0) MessageBox.Show(problems);
            else MessageBox.Show("OK");
        }

        private string ImportSanti(List<string> lines, string fileDirSanti)
        {
            string problems = "";
            string fileSanti = new FileInfo(fileDirSanti).Name;
            string dirSanti = new FileInfo(fileDirSanti).DirectoryName;
            int i = 0;
            try
            {
                string Santo = "";
                string Incipit = "";
                string Titolo1 = "";
                string Testo1 = "";
                string Titolo2 = "";
                string Testo2 = "";
                string Titolo3 = "";
                string Testo3 = "";

                List<string> LinesSection = new List<string>();
                int Sezione = 1;
                for (i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Trim().Length > 0) LinesSection.Add(lines[i]);
                    if ((lines[i].Trim().Length == 0 || i == lines.Count - 1) && LinesSection.Count > 0)
                    {
                        //fine sezione
                        switch (Sezione)
                        {
                            case 1:
                                Santo = LinesSection[0];
                                Incipit = LinesSection[1];
                                break;
                            case 2:
                                for (int j = 0; j < LinesSection.Count; j++)
                                {
                                    if (j == 0) Titolo1 = LinesSection[j];
                                    else Testo1 += LinesSection[j] + "!!!ACAPO!!!";
                                }
                                break;
                            case 3:
                                for (int j = 0; j < LinesSection.Count; j++)
                                {
                                    if (j == 0) Titolo2 = LinesSection[j];
                                    else Testo2 += LinesSection[j] + "!!!ACAPO!!!";
                                }
                                break;
                            case 4:
                                for (int j = 0; j < LinesSection.Count; j++)
                                {
                                    if (j == 0) Titolo3 = LinesSection[j];
                                    else Testo3 += LinesSection[j] + "!!!ACAPO!!!";
                                }
                                break;
                        }
                        LinesSection.Clear();
                        Sezione++;
                    }
                }
                string[] partifilename = fileSanti.Split(new char[] { '.', ' ' });
                string giorno = Convert.ToInt32(partifilename[1]) + " " + MeseDaNumero(partifilename[0]);
                string oldimgfile = fileSanti.Substring(0, fileSanti.LastIndexOf(".")) + ".jpg";
                string newimgfile = partifilename[0] + partifilename[1] + oldimgfile.Substring(5).Replace(" ", "_");
                string santixhtmlfile = "santi_" + partifilename[0] + partifilename[1] + fileSanti.Substring(5, fileSanti.LastIndexOf(".") - 5).Replace(" ", "_") + ".xhtml";
                string ritorno = partifilename[0] + partifilename[1] + ".xhtml";

                if (Santo == "")
                    problems += giorno + " missing Santo" + Environment.NewLine;
                if (Incipit == "")
                    problems += giorno + " missing Incipit" + Environment.NewLine;
                if (Titolo1 == "")
                    problems += giorno + " missing Titolo1" + Environment.NewLine;
                if (Testo1 == "")
                    problems += giorno + " missing Testo1" + Environment.NewLine;
                if (Titolo2 == "")
                    problems += giorno + " missing Titolo2" + Environment.NewLine;
                if (Testo2 == "")
                    problems += giorno + " missing Testo2" + Environment.NewLine;

                string baseText = File.ReadAllText("SANTIBase.xhtml");
                baseText = baseText.Replace("<!--SANTO-->", WebUtility.HtmlEncode(Santo));
                baseText = baseText.Replace("<!--GIORNO-->", WebUtility.HtmlEncode(giorno));
                baseText = baseText.Replace("<!--INCIPIT-->", WebUtility.HtmlEncode(Incipit));
                baseText = baseText.Replace("<!--IMGFILE-->", WebUtility.HtmlEncode(newimgfile));
                baseText = baseText.Replace("<!--NAMEPRIMOTESTO-->", WebUtility.HtmlEncode(Titolo1));
                baseText = baseText.Replace("<!--PRIMOTESTO-->", WebUtility.HtmlEncode(Testo1).Replace("!!!ACAPO!!!", @"<br/>"));
                baseText = baseText.Replace("<!--NAMESECONDOTESTO-->", WebUtility.HtmlEncode(Titolo2));
                baseText = baseText.Replace("<!--SECONDOTESTO-->", WebUtility.HtmlEncode(Testo2).Replace("!!!ACAPO!!!", @"<br/>"));
                baseText = baseText.Replace("<!--NAMETERZOTESTO-->", WebUtility.HtmlEncode(Titolo3));
                baseText = baseText.Replace("<!--TERZOTESTO-->", WebUtility.HtmlEncode(Testo3).Replace("!!!ACAPO!!!", @"<br/>"));
                baseText = baseText.Replace("<!--RITORNO-->", WebUtility.HtmlEncode(ritorno));

                Directory.CreateDirectory("santiNuovi");
                File.WriteAllText(@".\santiNuovi\" + santixhtmlfile, baseText);
                File.Copy(dirSanti + @"\" + oldimgfile, @".\santiNuovi\" + newimgfile, true);

            }
            catch (Exception ex)
            {
                problems += fileSanti + " " + lines[i] + " " + ex.Message;
            }

            return problems;
        }

        private string MeseDaNumero(string numerodelmese)
        {
            List<string> numeromese = new List<string>(new string[] { "Gennaio01", "Febbraio02", "Marzo03", "Aprile04", "Maggio05", "Giugno06", "Luglio07", "Agosto08", "Settembre09", "Ottobre10", "Novembre11", "Dicembre12" });
            try
            {
                string mese = numeromese.Where(x => x.Substring(x.Length - 2) == numerodelmese).Single();
                return mese.Substring(0, mese.Length - 2);
            }
            catch (Exception ex)
            {
                utility.writelog("Errore MeseDaNumero: '" + numerodelmese + "'" + ex.Message);
                throw ex;
            }
        }

        private void creaEPUB_Click(object sender, EventArgs e)
        {
            string FileCopertina = null;
            string FileCalendario1 = null;
            string FileCalendario2 = null;

            if (rtxS == null)
            {
                MessageBox.Show("File ODT non caricato");
                return;
            }

            OpenDialog.Filter = "JPEG File|*.jpg";
            OpenDialog.Title = "Selezionare immagine di copertina";
            OpenDialog.FileName = "";
            if (OpenDialog.ShowDialog() == DialogResult.OK) FileCopertina = OpenDialog.FileName;

            OpenDialog.Filter = "JPEG File|*.jpg";
            OpenDialog.Title = "Selezionare immagine del calendario1";
            OpenDialog.FileName = "";
            if (OpenDialog.ShowDialog() == DialogResult.OK) FileCalendario1 = OpenDialog.FileName;

            OpenDialog.Filter = "JPEG File|*.jpg";
            OpenDialog.Title = "Selezionare immagine del calendario2";
            OpenDialog.FileName = "";
            if (OpenDialog.ShowDialog() == DialogResult.OK) FileCalendario2 = OpenDialog.FileName;

            SaveDialog.Filter = "EPUB File|*.epub";
            SaveDialog.FileName = TPQTitle.Text + ".epub";
            SaveDialog.Title = "Selezionare percorso per salvataggio file EPUB";
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                epubW = new EPUBWriter(checkSanti.Checked);
                epubW.LoadStyle(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "style_epub.csv"));
                epubW.LoadBaseFile();
                epubW.CreateEpub(SaveDialog.FileName, PQI.dicPQTag, TPQTitle.Text, TISBN.Text, FileCopertina, FileCalendario1, FileCalendario2);

                CreateMOBI(SaveDialog.FileName);

                MessageBox.Show("OK");
            }
        }

        private void CreateMOBI(string filename)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\kindlegen.exe";
            ExternalProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            ExternalProcess.StartInfo.Arguments = " \"" + filename + "\"";
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();
        }

        private void buttomIDML_Click(object sender, EventArgs e)
        {
            if (rtxS == null)
            {
                MessageBox.Show("File ODT non caricato");
                return;
            }

            SaveDialog.Filter = "IDML File|*.idml";
            SaveDialog.FileName = "PQ";
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                idmlW = new IDMLWriter();
                idmlW.LoadStyle(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "style_idml.csv"));
                idmlW.LoadBaseFile();
                idmlW.CreateIDML(SaveDialog.FileName, odtI.dicPQTag);
                MessageBox.Show("OK");
            }
        }

        private void OpenDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void bimportIDML_Click(object sender, EventArgs e)
        {
            string idmlfile = @"c:\PQ\pqsetott2013\PQ SETT-OTT 2013 MODIFICATO.idml";

            OpenDialog.Filter = "IDML Files|*.idml";
            OpenDialog.Multiselect = false;
            OpenDialog.Title = "Seleziona il file IDML definitivo";
            OpenDialog.FileName = "";

            if (OpenDialog.ShowDialog() == DialogResult.OK) idmlfile = OpenDialog.FileName;

            string problems;
            PQI = new PQImporter();
            PQImporter.ODTorIDML = true;

            PQI.OpenIDML(idmlfile);
            problems = PQI.PutTag();
            //CreaRTFIDML();

            File.WriteAllText("PQLOG.txt", problems);
            if (problems.Length > 0) MessageBox.Show("problems");
            else MessageBox.Show("OK");

            //PQI = new PQImporter();
            //PQI.Open(@"c:\Users\CasaEster\Documents\prova.rtf", true);

            //if (OpenDialog.ShowDialog() == DialogResult.OK)
            //{
            //    //CARICO RTF
            //    PQI = new PQImporter();
            //    PQI.Open(OpenDialog.FileName,true);
            //    problems = odtI.PutTag();

            //    //Creo RTF
            //    CreaRTFIDML();

            //    File.WriteAllText("c:\\PQLOG.txt", problems);
            //    if (problems.Length > 0) MessageBox.Show(problems);
            //    else MessageBox.Show("OK");
            //}
        }

        private void bNitf_Click(object sender, EventArgs e)
        {
            if (rtxS == null)
            {
                MessageBox.Show("File non caricato");
                return;
            }


            SaveDialog.Filter = "ZIP File|*.zip";
            SaveDialog.FileName = TPQTitle.Text + ".zip";
            SaveDialog.Title = "Selezionare percorso per salvataggio file NITF";
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                nitfW = new NITFWriter();
                nitfW.LoadStyle(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "style_nitf.csv"));
                nitfW.LoadBaseFile();
                nitfW.CreateNITF(SaveDialog.FileName, PQI.dicPQTag, TPQTitle.Text);
                MessageBox.Show("OK");
            }
        }

        private void BImpBook_Click(object sender, EventArgs e)
        {
            string xmlfile = @"c:\Users\casae\Downloads\vertopal.com_2_Attirati da Gesù CAPBRAKE.xml";

            OpenDialog.Filter = "XML Files|*.xml";
            OpenDialog.Multiselect = false;
            OpenDialog.Title = "Seleziona il file XML del libro";
            OpenDialog.FileName = "";

            if (OpenDialog.ShowDialog() == DialogResult.OK) xmlfile = OpenDialog.FileName;

            string problems = "";
            bool ret = BXMLImporter.LoadBXMLFile(xmlfile);

            File.WriteAllText("PQLOG.txt", problems);
            if (problems.Length > 0) MessageBox.Show("problems");
            else MessageBox.Show("OK");

        }

        private void bnitflibro_Click(object sender, EventArgs e)
        {
            if (BXMLImporter.book  == null)
            {
                MessageBox.Show("Libro non caricato");
                return;
            }

            SaveDialog.Filter = "ZIP File|*.zip";
            SaveDialog.FileName = TPQTitle.Text + ".zip";
            SaveDialog.Title = "Selezionare percorso per salvataggio file NITF";
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                BXMLImporter.CreateNITF(SaveDialog.FileName, TPQTitle.Text);
                MessageBox.Show("OK");
            }
        }
    }
}
