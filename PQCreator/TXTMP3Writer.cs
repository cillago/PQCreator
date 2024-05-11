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
using System;
using System.Diagnostics.Eventing.Reader;

namespace PQCreator
{
    class TXTMP3Writer
    {
        private Dictionary<string, string[]> dictNITFDecode;
        private Dictionary<string, Dictionary<string, Story>> textTag;
        private Dictionary<string, string> indici = new Dictionary<string, string>();

        private string pqBaseNITFFile;
        private string dirNITF;

        internal void CreateTXTMP3(Dictionary<string, Dictionary<string, Story>> dictionary)
        {
            textTag = dictionary;
            creaFileGiorni();
        }

        private void creaFileGiorni()
        {
            {
                try
                {
                    string txtPath = @"C:\temp\TXTMP3\";
                    //ciclo su tutti i giorni
                    foreach (KeyValuePair<string, Dictionary<string, Story>> giorno in textTag)
                    {
                        int maxIndexGiorno = 0;
                        string nuovoGiorno = "";
                        string nomeGiorno = "";
                        string testo = "";
                        string[] partiGiorno = new string[8];
                        string ritornelloSalmo = null;

                        //ciclo su tutti i tag
                        foreach (KeyValuePair<string, Story> singleTag in giorno.Value)
                        {
                            if (singleTag.Key == "#01" || singleTag.Key == "#09")
                            {
                                nomeGiorno = singleTag.Value.getText();
                            }
                            else
                            {
                                int indexParteGiorno = Convert.ToInt32(singleTag.Key.Substring(1, 1));
                                if (indexParteGiorno > maxIndexGiorno) maxIndexGiorno = indexParteGiorno;


                                if (indexParteGiorno == 1 && partiGiorno[indexParteGiorno] == null)
                                    partiGiorno[indexParteGiorno] = nomeGiorno + "..." + Environment.NewLine;

                                if (indexParteGiorno == 2)
                                {

                                    string strofa = singleTag.Value.getText();

                                    if (partiGiorno[indexParteGiorno] == null)
                                    {
                                        //titolo salmo
                                        int parAp = strofa.IndexOf("(");

                                        if (parAp > 0)
                                            strofa = strofa.Substring(0, parAp);

                                        partiGiorno[indexParteGiorno] += strofa + "..." + Environment.NewLine;
                                    }
                                    else if (ritornelloSalmo == null)
                                    {
                                        //ritornello
                                        ritornelloSalmo = strofa;
                                        partiGiorno[indexParteGiorno] += strofa + "..." + Environment.NewLine;
                                    }
                                    else
                                    {
                                        //strofa + ritornello 
                                        partiGiorno[indexParteGiorno] += strofa + "..." + Environment.NewLine;
                                        partiGiorno[indexParteGiorno] += ritornelloSalmo + "..." + Environment.NewLine;
                                    }
                                }
                                else if (indexParteGiorno != 0)
                                {
                                    //tolgo le 
                                    if (singleTag.Key != "#11" && singleTag.Key != "#21" && singleTag.Key != "#51" && singleTag.Key != "#61" && singleTag.Key != "#71" && singleTag.Key != "#81")
                                    {
                                        foreach (StoryLine riga in singleTag.Value.Lines)
                                        {
                                            string r = riga.getText();
                                            if (r == "Oppure:")
                                                continue;

                                            int parAp = r.IndexOf("(");
                                            int parCh = r.IndexOf(")");

                                            if (parAp > 0 && parCh > 0)
                                                r = r.Substring(0, parAp) + r.Substring(parCh+1);
                                            partiGiorno[indexParteGiorno] += r + "..." + Environment.NewLine;
                                        }
                                    }

                                }
                            }
                        }

                        string alleluia = null;
                        for (int i = 1; i <= maxIndexGiorno; i++)
                        {
                            int j = 0;
                            switch (i)
                            {
                                case 1: j = 10; break;
                                case 2: j = 20; break;
                                case 3: j = 30; break;
                                case 4: j = 40; break;
                                case 5: j = 50; break;
                                case 6: j = 11; break;
                                case 7: j = 31; break;
                                case 8: j = 51; break;
                            }

                            if (i == 4)
                            {
                                alleluia = partiGiorno[i];
                            }
                            else if (i == 5)
                            {
                                File.WriteAllText(Path.Combine(txtPath, ExtractNomeFileGiorno(nomeGiorno) + "_" + j + ".txt"), alleluia + Environment.NewLine + partiGiorno[i] + "... ... ...");
                            }
                            else if (partiGiorno[i] != null)
                                File.WriteAllText(Path.Combine(txtPath, ExtractNomeFileGiorno(nomeGiorno) + "_" + j + ".txt"), partiGiorno[i] + Environment.NewLine + "... ... ...");
                        }
                    }
                }
                catch (Exception ex)
                {
                    utility.writelog("creaFileGiorni: " + ex.Message);
                    throw ex;
                }

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
    }
}
