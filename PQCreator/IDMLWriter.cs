using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;

namespace PQCreator
{
    class IDMLWriter
    {
        private Dictionary<string, string[]> dictIDMLBDecode;
        private Dictionary<string, Dictionary<string, string>> textTag;
        private Dictionary<int, string> indici = new Dictionary<int, string>();
        private string pqBaseIDMLFile;
        private string dirIDML;

        internal void LoadStyle(string nomefile)
        {
            StreamReader sr = File.OpenText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), nomefile));
            dictIDMLBDecode = new Dictionary<string, string[]>();
            while (!sr.EndOfStream)
            {
                string[] lineKeyVal = sr.ReadLine().Replace("\"\"", "'").Replace("\"", "").Split(';');
                dictIDMLBDecode.Add(lineKeyVal[0], lineKeyVal);
            }
            sr.Close();
        }

        internal void LoadBaseFile()
        {
            try
            {
                pqBaseIDMLFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "IDMLPQBase.idml");
                dirIDML = Path.Combine(Path.GetDirectoryName(pqBaseIDMLFile), Path.GetFileNameWithoutExtension(pqBaseIDMLFile));
                // unzippo file idml base
                utility.UnzipFile(pqBaseIDMLFile, dirIDML);

            }
            catch (Exception ex)
            {
                utility.writelog("LoadBaseFile: " + ex.Message);
                throw ex;
            }
        }


        internal void CreateIDML(string idmlfile, Dictionary<string, Dictionary<string, string>> dictionary)
        {
            textTag = dictionary;
            creaFileGiorni();
            //CreaIndice();
            utility.CreateZipFile(Path.GetDirectoryName(idmlfile), Path.GetFileName(idmlfile), new DirectoryInfo(dirIDML));
        }

        private void creaFileGiorni()
        {
            try
            {
                // apro file basegiorni.xhtml
                //string BaseGiorno = File.ReadAllText(Path.Combine(dirIDML, @"Stories\basegiorno.xhtml"));
                string nomesezione = "TESTO";


                Dictionary<string, string> sezioni = new Dictionary<string, string>();

                // se non esiste la sezione la creo
                if (!sezioni.ContainsKey(nomesezione)) sezioni.Add(nomesezione, "");

                //ciclo su tutti i giorni
                foreach (KeyValuePair<string, Dictionary<string, string>> giorno in textTag)
                {
                    //ciclo su tutti i tag

                    sezioni[nomesezione] += @"<ParagraphStyleRange AppliedParagraphStyle=""ParagraphStyle/$ID/[No paragraph style]"" HyphenationZone=""14.173228346456694"" Justification=""LeftJustified"">" + Environment.NewLine;

                    foreach (KeyValuePair<string, string> singleTag in giorno.Value)
                    {
                        //nomesezione = dictIDMLBDecode[singleTag.Key][6];



                        //aggiungo XML parte prima del testo
                        sezioni[nomesezione] += dictIDMLBDecode[singleTag.Key][1] + Environment.NewLine;
                        //aggiungo a capo prima del testo se devo
                        if (dictIDMLBDecode[singleTag.Key][3].Length > 0) sezioni[nomesezione] += "<Br />" + Environment.NewLine;

                        //trovo quante righe sono
                        string[] righe = singleTag.Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        //aggiungo tutte le righe
                        int ind = 0;
                        foreach (string riga in righe)
                        {
                            string ToPut = riga;
                            //solo se righe in mezzo e non vuote
                            if (ind > 0 && riga.Length > 0)
                            {
                                //ToPut = CheckNotXML(riga);
                                if (ToPut != riga)
                                    MessageBox.Show("riga non valida:" + riga);
                                sezioni[nomesezione] += "<Br />" + Environment.NewLine;
                            }
                            ind++;
                            sezioni[nomesezione] += "<Content>" + SetCaseRiga(ModifyNotXML(ToPut), singleTag.Key) + "</Content>" + Environment.NewLine;
                        }

                        //aggiungo a capo dopo il testo se devo
                        if (dictIDMLBDecode[singleTag.Key][4].Length > 0) sezioni[nomesezione] += "<Br />" + Environment.NewLine;

                        //aggiungo XML parte dopo del testo
                        sezioni[nomesezione] += dictIDMLBDecode[singleTag.Key][2] + Environment.NewLine;

                    }
                    sezioni[nomesezione] += @"<CharacterStyleRange AppliedCharacterStyle=""CharacterStyle/$ID/[No character style]"" PointSize=""10.5"" ParagraphBreakType=""NextPage""><Properties><Leading type=""unit"">11.5</Leading></Properties><Br /></CharacterStyleRange>" + Environment.NewLine + "</ParagraphStyleRange>" + Environment.NewLine;
                }

                FindAndWritePQStories(sezioni);


            }
            catch (Exception ex)
            {
                utility.writelog("creaFileGiorni: " + ex.Message);
                throw ex;
            }


        }

        private string ModifyNotXML(string riga)
        {
            return HttpUtility.HtmlEncode(riga);
           // return riga.Replace("<<", "«").Replace(">>", "»");

        }

        private string CheckNotXML( string riga)
        {
            Regex _invalidXMLChars = new Regex(@"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]", RegexOptions.Compiled);
            return _invalidXMLChars.Replace(riga, "");
            //return _invalidXMLChars.IsMatch(riga);
        }

        private bool FindAndWritePQStories(Dictionary<string, string> sezioni)
        {
            string temp = "";
            string StoryName = "";
            bool found = false;
            foreach (FileInfo f in new DirectoryInfo(Path.Combine(dirIDML, @"Stories")).GetFiles())
            {
                temp = File.ReadAllText(f.FullName);

                //sostituisco i segnaposto con i valori veri
                foreach (KeyValuePair<string, string> sez in sezioni)
                {
                    if (temp.IndexOf("<!--" + sez.Key + "-->") > 0)
                    {
                        found = true;
                        StoryName = f.FullName;
                        temp = temp.Replace("<!--" + sez.Key + "-->", sez.Value);
                    }
                }
                //trovato storia --> esco dal ciclo
                if (found) break;
            }
            if (found)
            {
                File.WriteAllText(StoryName, temp);
                utility.writelog("salvato file:" + StoryName);
                return true;
            }
            else
            {
                utility.writelog("file storia non trovata");
                return false;
            }

        }

        private string SetCaseRiga(string riga, string keytag)
        {
            if (dictIDMLBDecode[keytag][5] == "U")
                return riga.ToUpper();
            else if (dictIDMLBDecode[keytag][5] == "L")
                return riga.ToLower();
            else if (dictIDMLBDecode[keytag][5] == "S")
                return " " + riga;
            else if (dictIDMLBDecode[keytag][5] == "C")
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(riga);
            else return riga;
        }


    }
}

