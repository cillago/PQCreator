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

namespace PQCreator
{
    class EPUBWriter
    {
        private Dictionary<string, string[]> dictEPUBDecode;
        private Dictionary<string, Dictionary<string, Story>> textTag;
        private Dictionary<string, string> indici = new Dictionary<string, string>();

        private string pqBaseEPUBFile;
        private string dirEPUB;
        private string dirSanti;

        private bool includeSanti = false;

        public EPUBWriter(bool _includeSanti)
        {
            includeSanti = _includeSanti;
        }

        internal void LoadStyle(string nomefile)
        {
            StreamReader sr = File.OpenText(nomefile);
            dictEPUBDecode = new Dictionary<string, string[]>();
            while (!sr.EndOfStream)
            {
                string[] lineKeyVal = sr.ReadLine().Replace("\"\"", "'").Replace("\"", "").Split(';');
                dictEPUBDecode.Add(lineKeyVal[0], lineKeyVal);
            }
            sr.Close();
        }

        internal void LoadBaseFile()
        {
            try
            {
                pqBaseEPUBFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "EPUBPQBase.epub");
                dirEPUB = Path.Combine(Path.GetDirectoryName(pqBaseEPUBFile), Path.GetFileNameWithoutExtension(pqBaseEPUBFile));
                dirSanti = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "santiNuovi");
                if (Directory.Exists(dirEPUB)) Directory.Delete(dirEPUB, true);
                // unzippo file epub base
                utility.UnzipFile(pqBaseEPUBFile, dirEPUB);

            }
            catch (Exception ex)
            {
                utility.writelog("LoadBaseFile: " + ex.Message);
                throw ex;
            }
        }


        internal void CreateEpub(string epubfile, Dictionary<string, Dictionary<string, Story>> dictionary, string PQTitle, string ISBN, string FileCopertina, string FileCalendario1, string FileCalendario2)
        {
            textTag = dictionary;
            creaFileGiorni();
            CreaIndice(PQTitle, ISBN);
            //COPIO FILE copertina e calendario
            if (FileCopertina != null) File.Copy(FileCopertina, Path.Combine(dirEPUB, @"OEBPS\Images\COP.jpg"), true);
            if (FileCalendario1 != null) File.Copy(FileCalendario1, Path.Combine(dirEPUB, @"OEBPS\Images\CAL1.jpg"), true);
            if (FileCalendario2 != null) File.Copy(FileCalendario2, Path.Combine(dirEPUB, @"OEBPS\Images\CAL2.jpg"), true);

            utility.CreateZipFile(Path.GetDirectoryName(epubfile), Path.GetFileName(epubfile), new DirectoryInfo(dirEPUB));
        }

        private void CreaIndice(string PQTitle, string ISBN)
        {
            try
            {
                // apro file basegiorni.xhtml
                string ContentOPF = File.ReadAllText(Path.Combine(dirEPUB, @"OEBPS\content.opf"));
                string tocNCX = File.ReadAllText(Path.Combine(dirEPUB, @"OEBPS\toc.ncx"));
                string tocHTML = File.ReadAllText(Path.Combine(dirEPUB, @"OEBPS\Text\toc.html"));
                string SpineGIORNI = "";
                string SpineSANTI = "";
                string NavPointGIORNI = "";
                string NavPointSANTI = "";
                string Manifest = "";
                string tocLinkGIORNI = "";
                string tocLinkSANTI = "";
                //ciclo su tutti i giorni/indici
                foreach (KeyValuePair<string, string> indice in indici)
                {
                    SpineGIORNI += "<itemref idref=\"DAY" + indice.Key + "\"/>" + Environment.NewLine;
                    NavPointGIORNI += "<navPoint id=\"navpointDAY" + indice.Key + "\">" + //playOrder=\"1"+ indice.Key +"\">" +
                            "<navLabel><text>" + indice.Value + "</text></navLabel><content src=\"Text/" + indice.Key + ".xhtml\"/></navPoint>" + Environment.NewLine;
                    Manifest += "<item href=\"Text/" + indice.Key + ".xhtml\" id=\"DAY" + indice.Key + "\" media-type=\"application/xhtml+xml\"/>" + Environment.NewLine;
                    //tocLink += "<p class=\"No-Paragraph-Style para-style-override-3\"><span class=\"char-style-override-6\"><a href=\"../Text/" + indice.Key + ".xhtml\">" + indice.Value + "</a></span></p>" + Environment.NewLine;
                    tocLinkGIORNI += "<p class=\"Normal para-style-override-5\"><span class=\"INDICE\"><a href=\"../Text/" + indice.Key + ".xhtml\">" + indice.Value + "</a></span></p>" + Environment.NewLine;
                }

                //ciclo su tutti i santi creo indice e copio file da comprimere
                int i = 0;
                foreach (string f in Directory.GetFiles(dirSanti, "*.*"))
                {
                    i++;
                    FileInfo file = new FileInfo(f);
                    //è testo e finisce con un giorno dei 2 mesi correnti
                    if (file.Extension.ToUpper() == ".XHTML" && indici.Keys.Contains(file.Name.Substring(6, 4)))
                    {
                        SpineSANTI += "<itemref idref=\"SANTITEXT" + i + "\"/>" + Environment.NewLine;
                        NavPointSANTI += "<navPoint id=\"navpointSANTI" + i + "\">" +
                           "<navLabel><text>" + ExtractNomeSanti(file.Name) + "</text></navLabel><content src=\"Text/" + file.Name + "\"/></navPoint>" + Environment.NewLine;
                        Manifest += "<item href=\"Text/" + file.Name + "\" id=\"SANTITEXT" + i + "\" media-type=\"application/xhtml+xml\"/>" + Environment.NewLine;
                        tocLinkSANTI += "<p class=\"Normal para-style-override-5\"><span class=\"INDICE\"><a href=\"../Text/" + file.Name + "\">" + ExtractNomeSanti(file.Name) + "</a></span></p>" + Environment.NewLine;
                        file.CopyTo(Path.Combine(dirEPUB, @"OEBPS\Text\" + file.Name));
                    }
                    //è immagine e inizia con un giorno dei 2 mesi correnti
                    if (file.Extension.ToUpper() == ".JPG" && indici.Keys.Contains(file.Name.Substring(0, 4)))
                    {
                        Manifest += "<item href=\"Images/" + file.Name + "\" id=\"SANTIIMG" + i + "\" media-type=\"image/jpeg\"/>" + Environment.NewLine;
                        file.CopyTo(Path.Combine(dirEPUB, @"OEBPS\Images\" + file.Name));
                    }
                }

                ContentOPF = ContentOPF.Replace("<!--SPINEGIORNI-->", SpineGIORNI);
                ContentOPF = ContentOPF.Replace("<!--SPINESANTI-->", SpineSANTI);
                ContentOPF = ContentOPF.Replace("<!--MANIFEST-->", Manifest);
                ContentOPF = ContentOPF.Replace("<!--COMOPEN-->", "<!--");
                ContentOPF = ContentOPF.Replace("<!--COMCLOSE-->", "-->");
                ContentOPF = ContentOPF.Replace("__PQTITLE__", PQTitle);
                ContentOPF = ContentOPF.Replace("__ISBN__", ISBN);
                tocNCX = tocNCX.Replace("<!--NAVPOINTGIORNI-->", NavPointGIORNI);
                tocNCX = tocNCX.Replace("<!--NAVPOINTSANTI-->", NavPointSANTI);
                tocNCX = tocNCX.Replace("<!--COMOPEN-->", "<!--");
                tocNCX = tocNCX.Replace("<!--COMCLOSE-->", "-->");
                tocNCX = tocNCX.Replace("__PQTITLE__", PQTitle);
                tocNCX = tocNCX.Replace("__ISBN__", ISBN);
                tocHTML = tocHTML.Replace("<!--TOCHTMLGIORNI-->", tocLinkGIORNI);
                tocHTML = tocHTML.Replace("<!--TOCHTMLSANTI-->", tocLinkSANTI);

                File.WriteAllText(Path.Combine(dirEPUB, @"OEBPS\content.opf"), ContentOPF);
                File.WriteAllText(Path.Combine(dirEPUB, @"OEBPS\toc.ncx"), tocNCX);
                File.WriteAllText(Path.Combine(dirEPUB, @"OEBPS\Text\toc.html"), tocHTML);

            }
            catch (Exception ex)
            {
                utility.writelog("creaIndice: " + ex.Message);
                throw ex;
            }

        }

        private string ExtractNomeSanti(string filename)
        {
            string start = "<p class=\"Normal para-style-override-4\" style=\"text-align: center;\"><b>";
            string end = "<";
            string ret = "";
            try
            {
                string[] textsanti = File.ReadAllText(Path.Combine(dirSanti, filename)).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string riga in textsanti)
                {
                    if (riga.IndexOf(start) >= 0)
                    {
                        int inizio = riga.IndexOf(start) + start.Length;
                        if (ret.Length > 0) ret += " - ";
                        ret += riga.Substring(inizio, riga.Substring(inizio).IndexOf(end)).Trim(); // CultureInfo.InvariantCulture.TextInfo.ToTitleCase(riga.Substring(inizio, riga.Substring(inizio).IndexOf(end)).Unidecode().ToLowerInvariant());
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                utility.writelog("ExtractNomeSanti: " + ex.Message);
                throw ex;
            }
        }

        private void creaFileGiorni()
        {
            try
            {
                // apro file basegiorni.xhtml
                string BaseGiorno = File.ReadAllText(Path.Combine(dirEPUB, @"OEBPS\Text\basegiorno.xhtml"));


                //ciclo su tutti i giorni
                foreach (KeyValuePair<string, Dictionary<string, Story>> giorno in textTag)
                {
                    string nuovoGiorno = BaseGiorno;
                    string nomefile = "";
                    Dictionary<string, string> sezioni = new Dictionary<string, string>();

                    //ciclo su tutti i tag
                    foreach (KeyValuePair<string, Story> singleTag in giorno.Value)
                    {
                        string nomesezione = dictEPUBDecode[singleTag.Key][1];

                        //aggiungo sezione ID uguale al giorno senza spazi e anche indice per nome file
                        if (singleTag.Key == "#01" || singleTag.Key == "#09")
                        {
                            sezioni.Add("ID", singleTag.Value.getText().Replace(" ", ""));
                            sezioni.Add("NOMEGIORNO", singleTag.Value.getText());
                            nomefile = ExtractNomeFileGiorno(singleTag.Value.getText());
                        }

                        // se non esiste la sezione la creo
                        if (!sezioni.ContainsKey(nomesezione)) sezioni.Add(nomesezione, "");

                        //aggiungo a capo all'inzio se devo
                        if (dictEPUBDecode[singleTag.Key][4].Length > 0) sezioni[nomesezione] += "<div>&nbsp;<br /></div>" + Environment.NewLine;
                        //if (dictEPUBDecode[singleTag.Key][4].Length > 0) sezioni[nomesezione] += "<div>&#160;<br /></div>" + Environment.NewLine;

                        //aggiungo tutte le righe
                        foreach (StoryLine riga in singleTag.Value.Lines)
                        {
                            //caso particolare --> link alla pagina dei santi se da fare e
                            if (includeSanti && singleTag.Key == "#02") sezioni[nomesezione] += AddLinkSanti(riga, nomefile, "#02", "#08");
                            //caso particolare --> i nomi dei santi devono avere un carattere diverso
                            else if (singleTag.Key == "#03")
                                sezioni[nomesezione] += ChangeTagForStyleIfBold(riga, "#03", "#07");
                            // se no tutto uguale
                            else sezioni[nomesezione] += dictEPUBDecode[singleTag.Key][2] + SetCaseRiga(riga.getText(), singleTag.Key) + dictEPUBDecode[singleTag.Key][3] + Environment.NewLine;
                        }

                    }

                    //sostituisco i segnaposto con i valori veri
                    foreach (KeyValuePair<string, string> sez in sezioni)
                    {
                        nuovoGiorno = nuovoGiorno.Replace("%%" + sez.Key + "%%", sez.Value);
                        nuovoGiorno = nuovoGiorno.Replace("<!--" + sez.Key + "-->", sez.Value);
                    }

                    //scrivo file xhtml del giorno
                    File.WriteAllText(Path.Combine(dirEPUB, @"OEBPS\Text\" + nomefile + ".xhtml"), nuovoGiorno);
                    //creo indice
                    indici.Add(nomefile, sezioni["NOMEGIORNO"]);
                }
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

        private string AddLinkSanti(StoryLine riga, string nomefile, string NormalTag, string LinkedTag)
        {
            var fileSanti = Directory.GetFiles(dirSanti).Where(x => x.Contains("santi_" + nomefile)).Select(x => x.Substring(x.LastIndexOf("\\")+1)).ToList();

            if (fileSanti.Count==0)
                return dictEPUBDecode[NormalTag][2] + SetCaseRiga(riga.getText(), NormalTag) + dictEPUBDecode[NormalTag][3] + Environment.NewLine;


            var santiText = riga.getText().Split(';').ToList();
            string ret = dictEPUBDecode[NormalTag][2];
            Dictionary<string, string> santi = BestFitDistance(santiText, fileSanti);
            foreach (var santo in santi)
            {
                if (santo.Value == null)
                    ret += SetCaseRiga(santo.Key, NormalTag) +"; ";
                else
                    ret += dictEPUBDecode[LinkedTag][2].Replace("!!!!", WebUtility.HtmlEncode(santo.Value)) + SetCaseRiga(santo.Key, LinkedTag) + dictEPUBDecode[LinkedTag][3]+"; ";
            }
            
            ret= ret.Substring(0,ret.Length-1) + dictEPUBDecode[NormalTag][3] + Environment.NewLine;

            return ret;

        }

        private Dictionary<string, string> BestFitDistance(List<string> s1, List<string> s2)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            foreach (string s in s2)
            {
                var t = s1.Select(x => new KeyValuePair<string, int>(x, ComputeDistance(x, s.Substring(6)))).OrderBy(x => x.Value).FirstOrDefault();

                if (!ret.ContainsKey(t.Key))
                    ret.Add(t.Key,s);
            }

            foreach (string s in s1)
            {
                if (!ret.ContainsKey(s))
                    ret.Add(s, null);
            }
            return ret;
        }

        private string ChangeTagForStyleIfBold(StoryLine riga, string NormalTag, string ChangedTag)
        {
            string ret = dictEPUBDecode[NormalTag][2];
            foreach (CTextpart tp in riga.TextParts)
            {
                if (tp.font.Bold)
                    ret += dictEPUBDecode[ChangedTag][2] + tp.text + dictEPUBDecode[ChangedTag][3];
                else ret += tp.text;
            }
            return ret += dictEPUBDecode[NormalTag][3] + Environment.NewLine;
        }

        private string SetCaseRiga(string riga, string keytag)
        {
            if (dictEPUBDecode[keytag][5] == "U")
                return riga.ToUpper();
            else if (dictEPUBDecode[keytag][5] == "L")
                return riga.ToLower();
            else if (dictEPUBDecode[keytag][5] == "C")
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(riga);
            else return riga;
        }


        public static int ComputeDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];

        }

    }
}
