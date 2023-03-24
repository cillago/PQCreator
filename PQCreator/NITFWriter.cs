using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using SAE;
using System.Linq;
using UnidecodeSharpFork;
using System.Net;
using Microsoft.Office.Interop.Word;
using Application = System.Windows.Forms.Application;
using System.Configuration;

namespace PQCreator
{
    class NITFWriter
    {
        private Dictionary<string, string[]> dictNITFDecode;
        private Dictionary<string, Dictionary<string, Story>> textTag;
        private Dictionary<string, string> indici = new Dictionary<string, string>();

        private string pqBaseNITFFile;
        private string dirNITF;

        private bool includeSanti = false;

        internal void LoadStyle(string nomefile)
        {
            StreamReader sr = File.OpenText(nomefile);
            dictNITFDecode = new Dictionary<string, string[]>();
            while (!sr.EndOfStream)
            {
                string[] lineKeyVal = sr.ReadLine().Replace("\"\"", "'").Replace("\"", "").Split(';');
                dictNITFDecode.Add(lineKeyVal[0], lineKeyVal);
            }
            sr.Close();
        }

        internal void LoadBaseFile()
        {
            try
            {
                pqBaseNITFFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "NITFPQBase.zip");
                dirNITF = Path.Combine("c:\\temp\\PQ\\", Path.GetFileNameWithoutExtension(pqBaseNITFFile));
                if (Directory.Exists(dirNITF)) Directory.Delete(dirNITF, true);
                // unzippo file epub base
                utility.UnzipFile(pqBaseNITFFile, dirNITF);
            }
            catch (Exception ex)
            {
                utility.writelog("LoadBaseFile: " + ex.Message);
                throw ex;
            }
        }


        internal void CreateNITF(string nitffile, Dictionary<string, Dictionary<string, Story>> dictionary, string PQTitle)
        {
            textTag = dictionary;
            creaFileGiorni();
            utility.CreateZipFile(Path.GetDirectoryName(nitffile), Path.GetFileName(nitffile), new DirectoryInfo(dirNITF));
        }

        private void creaFileGiorni()
        {
            try
            {
                // apro file basegiorni.xhtml
                string BaseGiorno = File.ReadAllText(Path.Combine(dirNITF, @"articles\article.xml"));

                int index = 6;
                //ciclo su tutti i giorni
                foreach (KeyValuePair<string, Dictionary<string, Story>> giorno in textTag)
                {
                    string nuovoGiorno = BaseGiorno;
                    string nomefile = "";
                    string nomeGiorno = "";
                    //Dictionary<string, string> sezioni = new Dictionary<string, string>();
                    string testo = "";

                    //ciclo su tutti i tag
                    foreach (KeyValuePair<string, Story> singleTag in giorno.Value)
                    {
                        //string nomesezione = dictNITFDecode[singleTag.Key][1];

                        //aggiungo sezione ID uguale al giorno senza spazi e anche indice per nome file
                        //if (singleTag.Key == "#01" || singleTag.Key == "#09")
                        //{
                        //    sezioni.Add("ID", singleTag.Value.getText().Replace(" ", ""));
                        //    sezioni.Add("NOMEGIORNO", singleTag.Value.getText());
                        //    nomefile = ExtractNomeFileGiorno(singleTag.Value.getText());
                        //}

                        // se non esiste la sezione la creo
                        //if (!sezioni.ContainsKey(nomesezione)) sezioni.Add(nomesezione, "");
                        if (singleTag.Key == "#01" || singleTag.Key == "#09")
                        {
                            nomeGiorno = singleTag.Value.getText();
                        }
                        else
                        {
                            //aggiungo a capo all'inzio se devo
                            if (dictNITFDecode[singleTag.Key][4].Length > 0) testo += "<div><br/></div>" + Environment.NewLine;

                            //aggiungo tutte le righe
                            foreach (StoryLine riga in singleTag.Value.Lines)
                            {
                                //caso particolare --> i nomi dei santi devono avere un carattere diverso
                                if (singleTag.Key == "#03")
                                    testo += ChangeTagForStyleIfBold(riga, "#03", "#07");
                                // se no tutto uguale
                                else testo += dictNITFDecode[singleTag.Key][2] + SetCaseRiga(riga.getText(), singleTag.Key) + dictNITFDecode[singleTag.Key][3] + Environment.NewLine;
                            }
                        }
                    }

                    nuovoGiorno = nuovoGiorno.Replace("%%CONTENT%%", testo);
                    nuovoGiorno = nuovoGiorno.Replace("%%HEADLINE%%", nomeGiorno);
                    nuovoGiorno = nuovoGiorno.Replace("%%TITLE%%", nomeGiorno);

                    //scrivo file xml del giorno
                    File.WriteAllText(Path.Combine(dirNITF, @"Articles\" + "article_" + index.ToString("00") + ".xml"), nuovoGiorno);
                    index++;
                }

                File.Delete(Path.Combine(dirNITF, @"articles\article.xml"));
            }
            catch (Exception ex)
            {
                utility.writelog("creaFileGiorni: " + ex.Message);
                throw ex;
            }


        }

        private string ExtractNomeFileGiorno(string nomegiorno)
        {
            List<string> numeromese = new List<string>(new string[] { "gennaio01", "febbraio02", "marzo03", "aprile04", "maggio05", "giugno06", "luglio07", "agosto08", "settembre09", "ottobre10", "novembre11", "dicembre12" });
            try
            {
                string[] giornomese = nomegiorno.Split(' ');
                string mese = numeromese.Where(x => x.Substring(0, x.Length - 2).ToUpper() == giornomese[2].ToUpper()).Single();
                return mese.Substring(mese.Length - 2) + Convert.ToInt32(giornomese[1]).ToString("00");
            }
            catch (Exception ex)
            {
                utility.writelog("Errore ExtractNomeFileGiorno: '" + nomegiorno + "'" + ex.Message);
                throw ex;
            }
        }

        private string ChangeTagForStyleIfBold(StoryLine riga, string NormalTag, string ChangedTag)
        {
            string ret = dictNITFDecode[NormalTag][2];
            foreach (CTextpart tp in riga.TextParts)
            {
                if (tp.font.Bold)
                    ret += dictNITFDecode[ChangedTag][2] + tp.text + dictNITFDecode[ChangedTag][3];
                else ret += tp.text;
            }
            return ret += dictNITFDecode[NormalTag][3] + Environment.NewLine;
        }

        private string SetCaseRiga(string riga, string keytag)
        {
            if (dictNITFDecode[keytag][5] == "U")
                return riga.ToUpper();
            else if (dictNITFDecode[keytag][5] == "L")
                return riga.ToLower();
            else if (dictNITFDecode[keytag][5] == "C")
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(riga);
            else return riga;
        }
    }
}
