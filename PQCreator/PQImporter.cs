using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// for ODT
using uno;
using uno.util;
using unoidl.com.sun.star.lang;
using unoidl.com.sun.star.uno;
using unoidl.com.sun.star.bridge;
using unoidl.com.sun.star.frame;
using unoidl.com.sun.star.text;
using unoidl.com.sun.star.beans;

using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections;
using SAE;

namespace PQCreator
{
    public class PQImporter
    {
        private string ODTText = "";
        private Story StoryText;
        private string sproblem = "";
        private string outPQTag;
        private Section sec;
        private int prog;
        private int lineNum = 0;
        private int NumGiorni = 0;
        private string giornoCorrente;
        //variabile contenente multiriga commenti e letture
        private Story sTemp;
        private ArrayList giorni;
        private ArrayList giorniTag;
        private string IncrementalPQTag = "";

        public Dictionary<string, Dictionary<string, Story>> dicPQTag;
        private Dictionary<string, string[]> dictPQTagIDMLData;

        //default da odt
        public static bool ODTorIDML = false;

        public PQImporter()
        {
            LoadIDMLData();
        }

        internal void LoadIDMLData()
        {
            StreamReader sr = File.OpenText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "style_idml_decode.csv"));
            dictPQTagIDMLData = new Dictionary<string, string[]>();
            while (!sr.EndOfStream)
            {
                string[] lineKeyVal = sr.ReadLine().Split(';');
                dictPQTagIDMLData.Add(lineKeyVal[0], lineKeyVal);
            }
            sr.Close();
        }

        public void OpenIDML(string inputFile)
        {
            DecodeIDML.Clean();
            DecodeIDML.UnzipIDMLFile(inputFile);
            DecodeIDML.LoadIDMLFile(SAE.DecodeIDML.IDMLTempPath);
            StoryText = DecodeIDML.Stories[0];
        }
        public void OpenODT(string inputFile)
        {

            //carico ODT

            StartOpenOffice();

            //Get a ComponentContext
            var xLocalContext =
                Bootstrap.bootstrap();
            //Get MultiServiceFactory
            var xRemoteFactory =
                (XMultiServiceFactory)
                xLocalContext.getServiceManager();
            //Get a CompontLoader
            var aLoader =
                (XComponentLoader)xRemoteFactory.createInstance("com.sun.star.frame.Desktop");
            //Load the sourcefile

            XComponent xComponent = null;

            xComponent = InitDocument(aLoader,
                                      PathConverter(inputFile), "_blank");
            //Wait for loading
            while (xComponent == null)
            {
                Thread.Sleep(1000);
            }
            ODTText = ((XTextDocument)xComponent).getText().getString();
            if (xComponent != null) xComponent.dispose();
        }

        public void OpenTXT(string IFile)
        {
            string[] text = File.ReadAllText(IFile).Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            StoryText = new Story();
            for (int i = 0; i < text.Length; i++)
            {
                StoryText.Lines.Add(new StoryLine(text[i], SystemFonts.DefaultFont));
            }
        }

        public string PutTag()
        {
            giorniTag = new ArrayList();
            giorni = SplitGiorni(StoryText);
            dicPQTag = new Dictionary<string, Dictionary<string, Story>>();

            foreach (Story giorno in giorni)
            {
                sec = Section.giorno;
                prog = 0;
                giornoCorrente = "bo!";
                sTemp = new Story();
                outPQTag = "";
                outPQTag = PutTagOnDay(giorno);
                giorniTag.Add(outPQTag);
                IncrementalPQTag += outPQTag;
            }

            foreach (KeyValuePair<string, Dictionary<string, Story>> giorno in dicPQTag)
            {

                string[] checkTag = { "#10", "#11", "#20", "#40", "#50", "#51" };
                for (int i = 0; i < checkTag.Length; i++)
                {
                    if (!giorno.Value.ContainsKey(checkTag[i]))
                        sproblem += giorno.Key + " manca tag:" + checkTag[i] + Environment.NewLine;
                }

                //DOMENICA --> deve esserci seconda lettura
                if (giorno.Value.ContainsKey("#09") && (!giorno.Value.ContainsKey("#30") || !giorno.Value.ContainsKey("#31"))) sproblem += giorno.Key + " manca tag:#30" + Environment.NewLine;

            }

            return sproblem;
        }

        private ArrayList SplitGiorni(Story s)
        {
            Story tempGiorno = new Story();
            string l;
            ArrayList SplitG = new ArrayList();

            foreach (StoryLine CurLine in StoryText.Lines)
            {
                l = CurLine.getText().Trim();
                if (èInizioGiornoSettimana(l) || èInizioDomenica(l))
                {
                    if (tempGiorno.Lines.Count > 0)
                    {
                        SplitG.Add(tempGiorno);
                    }
                    NumGiorni++;
                    tempGiorno = new Story();
                    tempGiorno.Lines.Add(CurLine);
                }
                else tempGiorno.Lines.Add(CurLine);
            }
            SplitG.Add(tempGiorno);
            NumGiorni++;
            return SplitG;
        }

        private static void StartOpenOffice()
        {
            var ps = Process.GetProcessesByName("soffice.exe");
            if (ps.Length != 0)
                throw new InvalidProgramException("OpenOffice not found.  Is OpenOffice installed?");
            if (ps.Length > 0)
                return;
            var p = new Process
            {
                StartInfo =
                {
                    Arguments = "-headless -nofirststartwizard",
                    FileName = "soffice.exe",
                    CreateNoWindow = true
                }
            };
            var result = p.Start();

            if (result == false)
                throw new InvalidProgramException("OpenOffice failed to start.");
        }

        private static XComponent InitDocument(XComponentLoader aLoader, string file, string target)
        {
            var openProps = new PropertyValue[1];
            openProps[0] = new PropertyValue { Name = "Hidden", Value = new Any(true) };

            var xComponent = aLoader.loadComponentFromURL(
                file, target, 0,
                openProps);

            return xComponent;
        }

        private static string PathConverter(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new NullReferenceException("Null or empty path passed to OpenOffice");

            return String.Format("file:///{0}", file.Replace(@"\", "/"));
        }

        //private ArrayList SplitGiorniODT(string s)
        //{
        //    string[] Lines = s.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
        //    string tempGiorno = "";
        //    string l;
        //    ArrayList SplitG = new ArrayList();

        //    foreach (string CurLine in Lines)
        //    {
        //        l = CurLine.Trim();
        //        if (èInizioGiornoSettimana(l) || èInizioDomenica(l))
        //        {
        //            if (tempGiorno.Length > 0)
        //            {
        //                SplitG.Add(tempGiorno);

        //            }
        //            NumGiorni++;
        //            tempGiorno = l + Environment.NewLine;
        //        }
        //        else tempGiorno += l + Environment.NewLine;
        //    }
        //    SplitG.Add(tempGiorno);
        //    NumGiorni++;
        //    return SplitG;
        //}

        private string PutTagOnDay(Story s)
        {
            //MessageBox.Show("linee:" + Convert.ToString(Lines.GetLength(0)));
            try
            {
                string l;

                foreach (StoryLine CurLine in s.Lines)
                {
                    lineNum++;
                    l = CurLine.getText().Trim();
                    switch (sec)
                    {
                        //sezione GIORNO
                        case (int)Section.giorno:
                            {
                                switch (prog)
                                {
                                    //inizio
                                    case 0:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;

                                            //GIORNO da lun a sab
                                            if (èInizioGiornoSettimana(l))
                                            {
                                                addInizioGiornoSettimana(new Story(CurLine));
                                            }
                                            //GIORNO domenica
                                            else if (èInizioDomenica(l))
                                            {
                                                addInizioDomenica(new Story(CurLine));
                                            }
                                            else
                                                addProblem("manca giorno");
                                            sTemp = new Story();
                                            break;
                                        }

                                    //GIORNO
                                    case 1:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            //aggiungo santi --> tutti sulla stessa riga
                                            addOutPQ("#02", new Story(CurLine));
                                            prog = 2;
                                            sTemp = new Story();
                                            break;
                                        }
                                    //NOME SANTI
                                    case 2:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(new Story(CurLine), Section.primaLettura); break; }
                                            //controllo se la stringa è lunga --> allora è la storia dei santi
                                            if (l.Length > 90 && (!ODTorIDML || CheckFontSize(new Story(CurLine), "#03")))
                                            {
                                                //l = AggiungiTagNomeSanti(l, dicPQTag[giornoCorrente]["#02"]);
                                                addOutPQ("#03", new Story(CurLine));
                                                prog = 3;
                                            }
                                            else
                                            {
                                                if (èGiornataMondiale(new Story(CurLine))) addGiornataMondiale(new Story(CurLine));
                                                else
                                                {
                                                    //suppongo sia festività
                                                    addOutPQ("#04", new Story(CurLine));
                                                    prog = 4;
                                                }
                                            }

                                            break;
                                        }
                                    //VITA SANTI
                                    case 3:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(new Story(CurLine), Section.primaLettura); break; }

                                            if (èGiornataMondiale(new Story(CurLine))) addGiornataMondiale(new Story(CurLine));
                                            else
                                            {
                                                //suppongo sia festività
                                                addOutPQ("#04", new Story(CurLine));
                                                prog = 4;
                                            }
                                            break;
                                        }
                                    // FESTA RELIGIOSA IMPORTANTE
                                    case 4:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(new Story(CurLine), Section.primaLettura); break; }

                                            if (èGiornataMondiale(new Story(CurLine))) addGiornataMondiale(new Story(CurLine));
                                            else
                                            {
                                                addProblem("parte non riconosciuta");
                                            }
                                            break;
                                        }
                                    // GIORNATA MONDIALE
                                    case 5:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(new Story(CurLine), Section.primaLettura); break; }

                                            //suppongo sia testo altra festa
                                            addOutPQ("#06", new Story(CurLine));
                                            //vado comunque avanti
                                            prog = 6;
                                            break;
                                        }
                                    // TESTO ALTRA FESTA
                                    case 6:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(new Story(CurLine), Section.primaLettura); break; }

                                            //se arrivo qui è un problema ... non ho trovato la prma lettura
                                            addProblem("manca prima lettura");
                                            sec = Section.primaLettura;
                                            prog = 1;
                                            break;
                                        }

                                    default:
                                        {
                                            addProblem("default .. non gestito!");
                                            break;
                                        }
                                }
                                break;
                            }
                        //PRIMA O seconda LETTURA o vangelo
                        case Section.vangelo:
                        case Section.primaLettura:
                        case Section.secondalettura:
                        case Section.vangeloOppure:
                        case Section.primaLetturaOppure:
                        case Section.secondaLetturaOppure:
                            {
                                switch (prog)
                                {
                                    case -1:
                                        {
                                            // quando so che deve iniziare una lettura o vangelo ma non ho ancora trovato niente
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (èInizioLettura(l) || èInizioVangelo(l))
                                            {
                                                addInizioLetturaOVangelo(new Story(CurLine), sec);
                                                break;
                                            }
                                            else if (èAntifonaVangelo(l))
                                            {
                                                addInizioLetturaOVangelo(new Story(CurLine), sec);
                                                break;
                                            }
                                            else
                                            {
                                                sTemp.Addstory(new Story(CurLine));
                                                addProblem("manca titolo e rif lettura");
                                            }
                                            break;
                                        }
                                    //riferimenti
                                    case 0:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            // NO--> COSì DA ERRORE MANCANZA RIFERIMENTO
                                            AppendInizioLetturaOVangelo(new Story(CurLine), sec);
                                            //AddRiferimento(new Story(CurLine), sec);
                                            break;
                                        }
                                    //titolo e riferimenti
                                    case 1:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            AddTestoLettura(new Story(CurLine));
                                            break;
                                        }
                                    case 3:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            AddCommentoLettura(new Story(CurLine));
                                            break;
                                        }

                                    default:
                                        {
                                            addProblem("default .. non gestito!");
                                            break;
                                        }
                                }
                                break;
                            }

                        //SALMO
                        case Section.salmo:
                            {
                                switch (prog)
                                {
                                    //titolo e riferimenti
                                    case 0:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (l.Length == 0)
                                            {
                                                Add1AntifonaSalmo(sTemp);
                                                sTemp = new Story();
                                                break;
                                            }
                                            if (èoppure(l)) Add1OppureSalmo(new Story(CurLine));
                                            else
                                            {
                                                sTemp.Addstory(new Story(CurLine));
                                            }
                                            break;
                                        }
                                    case 1:
                                        {
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (l.Length == 0)
                                            {
                                                AddStrofaSalmo(sTemp, 6);
                                                sTemp = new Story();
                                                break;
                                            }
                                            if (èoppure(l)) Add1OppureSalmo(new Story(CurLine));
                                            else
                                            {
                                                sTemp.Addstory(new Story(CurLine));
                                            }
                                            break;
                                        }
                                    case 2:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (l.Length == 0)
                                            {
                                                Add2AntifonaSalmo(sTemp);
                                                sTemp = new Story();
                                                break;
                                            }
                                            else
                                            {
                                                sTemp.Addstory(new Story(CurLine));
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (l.Length == 0)
                                            {
                                                AddStrofaSalmo(sTemp, 6);
                                                sTemp = new Story();
                                                break;
                                            }
                                            if (èoppure(l)) Add2OppureSalmo(new Story(CurLine));
                                            else
                                            {
                                                sTemp.Addstory(new Story(CurLine));
                                            }
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (l.Length == 0) break;
                                            Add3AntifonaSalmo(new Story(CurLine));
                                            break;
                                        }
                                    case 5:
                                    case 6:
                                    case 7:
                                    case 8:
                                    case 9:
                                    case 10: //'A' su rtf
                                    case 11: //'B'  su rtf
                                    case 12: //'C'  su rtf
                                    case 13: //'D'  su rtf
                                        {
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (èInizioLettura(l))
                                            {
                                                if (sTemp.Length > 0)
                                                {
                                                    AddStrofaSalmo(sTemp, prog + 1);
                                                    sTemp = new Story();
                                                }
                                                addInizioLetturaOVangelo(new Story(CurLine), Section.secondalettura);
                                                break;
                                            }

                                            if (èAntifonaVangelo(l))
                                            {
                                                if (sTemp.Length > 0)
                                                {
                                                    AddStrofaSalmo(sTemp, prog + 1);
                                                    sTemp = new Story();
                                                }
                                                AddInizioPrimaAntifonaVangelo(new Story(CurLine));
                                                break;
                                            }
                                            if (l.Length == 0)
                                            {
                                                AddStrofaSalmo(sTemp, prog + 1);
                                                sTemp = new Story();
                                                break;
                                            }
                                            else sTemp.Addstory(new Story(CurLine));

                                            break;
                                        }
                                    default:
                                        {
                                            addProblem("default .. non gestito!");
                                            break;
                                        }
                                }
                                break;
                            }
                        case Section.antifonaVangelo:
                            {
                                switch (prog)
                                {
                                    case 0:
                                        {
                                            if (l.Length == 0) break;
                                            if (èAntifonaVangelo(l))
                                            {
                                                addOutPQ("#41", sTemp);
                                                sTemp = new Story();
                                                addOutPQ("#42", new Story(CurLine));
                                                //sec = Section.vangelo;
                                                //prog = -1;
                                                prog = 2;
                                            }
                                            else sTemp.Addstory(new Story(CurLine));
                                            break;
                                        }

                                    case 2:
                                        {
                                            if (l.Length == 0) break;
                                            if (èoppure(l))
                                            {
                                                AddOppureAntifonaVangelo(new Story(CurLine));
                                            }
                                            else if (èInizioVangelo(l))
                                            {
                                                addInizioLetturaOVangelo(new Story(CurLine), Section.vangelo);
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (l.Length == 0) break;
                                            if (èAntifonaVangelo(l))
                                            {
                                                AddInizioSecondaAntifonaVangelo(new Story(CurLine));
                                            }
                                            else addProblem("manca seconda antifona vangelo");
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (l.Length == 0) break;
                                            if (èAntifonaVangelo(l))
                                            {
                                                addOutPQ("#45", sTemp);
                                                sTemp = new Story();
                                                addOutPQ("#46", new Story(CurLine));
                                                sec = Section.vangelo;
                                                prog = -1;
                                            }
                                            else sTemp.Addstory(new Story(CurLine));
                                            break;
                                        }
                                    default:
                                        {
                                            addProblem("default .. non gestito!");
                                            break;
                                        }
                                }
                                break;
                            }

                        default:
                            {
                                addProblem("default .. non gestito!");
                                break;
                            }
                    }
                }
                //aggiungo commento al vangelo
                if (sTemp.Length > 0)
                {
                    addOutPQ("#" + (int)sec + "4", sTemp);
                    sTemp = new Story();
                }

            }
            catch (System.Exception ex)
            {
                addProblem("eccezione: " + ex.Message);
            }

            return outPQTag;
        }

        private void AppendInizioLetturaOVangelo(Story s, Section newSec)
        {
            string l = s.getText();
            int inizioRif = l.IndexOf('(');
            //titolo prima lettura
            AppendOutPQ("#" + (int)newSec + "0", s.SubstoryLeft("("));
            prog = 0;
            sec = newSec;
            if (inizioRif >= 0) AddRiferimento(s, newSec);

        }
        private bool èGiornataMondiale(Story s)
        {
            string l = s.getText();
            return ((l.ToLower().IndexOf("giornata") >= 0 || l.ToLower().IndexOf("anniversario") >= 0) && l.Length < 130);
        }

        private bool CheckFontSize(Story s, string tag)
        {
            float f1 = Convert.ToSingle(dictPQTagIDMLData[tag][1]);
            float f2 = s.FontSize();
            return (f1 == f2);
        }

        private void addGiornataMondiale(Story l)
        {
            addOutPQ("#05", l);
            prog = 5;
        }

        //private string AggiungiTagNomeSanti(string Vita, string santi)
        //{
        //    string[] NomiSanti = santi.Split(new char[] { ' ', '.', ';' }, StringSplitOptions.RemoveEmptyEntries);
        //    int progSanti = 0;
        //    for (int i = 0; i < NomiSanti.Length; i++)
        //    {

        //        if (NomiSanti[i].Length > 3)
        //        {

        //            if (Vita.IndexOf(NomiSanti[i]) >= 0)
        //            {
        //                progSanti++;
        //                //al posto del nome del santo metto il tag
        //                Vita = Vita.Replace(NomiSanti[i], "#07_" + progSanti.ToString());
        //                //aggingo il tag alla lista dei tag del giorno
        //                addOutPQ("#07_" + progSanti.ToString(), NomiSanti[i]);
        //            }
        //        }
        //    }
        //    return Vita;
        //}

        private void AddInizioPrimaAntifonaVangelo(Story l)
        {
            addOutPQ("#40", l);
            sec = Section.antifonaVangelo;
            prog = 0;
        }

        private void AddInizioSecondaAntifonaVangelo(Story l)
        {
            addOutPQ("#44", l);
            sec = Section.antifonaVangelo;
            prog = 4;
        }

        private bool èAntifonaVangelo(string l)
        {
            return l.StartsWith("alleluia", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("gloria a te", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("lode ", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("gloria e lode", StringComparison.InvariantCultureIgnoreCase);
        }

        private void AddStrofaSalmo(Story l, int p)
        {
            //metto p in esadecimale per sezione salmo
            addOutPQ("#2" + p.ToString("X"), l);
            prog = p;
            if (l.Lines.Count < 2) addProblem("strofa salmo troppo corta");
            if (l.Lines.Count > 5) addProblem("strofa salmo troppo lunga");
        }

        private string hex(int p, string l)
        {
            throw new NotImplementedException();
        }

        private void Add1AntifonaSalmo(Story l)
        {
            addOutPQ("#21", l);
            prog = 1;
        }

        private void Add1OppureSalmo(Story l)
        {
            addOutPQ("#22", l);
            prog = 2;
        }

        private void Add2OppureSalmo(Story l)
        {
            addOutPQ("#24", l);
            prog = 4;
        }

        private void AddOppureAntifonaVangelo(Story l)
        {
            addOutPQ("#43", l);
            prog = 3;
        }

        private void Add2AntifonaSalmo(Story l)
        {
            addOutPQ("#23", l);
            prog = 3;
        }

        private void Add3AntifonaSalmo(Story l)
        {
            addOutPQ("#25", l);
            prog = 5;
        }

        private bool èoppure(string l)
        {
            return l.StartsWith("oppure", StringComparison.CurrentCultureIgnoreCase) && l.Length < 12;
        }

        private void addInizioDomenica(Story l)
        {
            giornoCorrente = l.getText();
            addOutPQ("#09", l);
            NumGiorni++;
            prog = 1;
        }

        private void addInizioGiornoSettimana(Story l)
        {
            giornoCorrente = l.getText();
            addOutPQ("#01", l);
            NumGiorni++;
            prog = 1;
        }

        private static bool èInizioGiornoSettimana(string l)
        {
            l = l.ToLower();
            return (l.StartsWith("luned", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("marted", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("mercoled", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("gioved", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("sabato", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("venerd", StringComparison.InvariantCultureIgnoreCase)) &&
                (l.Contains("gennaio") || l.Contains("febbraio") || l.Contains("marzo") || l.Contains("aprile") || l.Contains("maggio") || l.Contains("giugno") || l.Contains("luglio") || l.Contains("agosto") || l.Contains("settembre") || l.Contains("ottobre") || l.Contains("novembre") || l.Contains("dicembre"));
        }
        private static bool èInizioDomenica(string l)
        {
            //se festività fuori domenica
            //if (l.ToUpper() == l) return true;
            l = l.ToLower();
            return l.StartsWith("domenica", StringComparison.InvariantCultureIgnoreCase) &&
                (l.Contains("gennaio") || l.Contains("febbraio") || l.Contains("marzo") || l.Contains("aprile") || l.Contains("maggio") || l.Contains("giugno") || l.Contains("luglio") || l.Contains("agosto") || l.Contains("settembre") || l.Contains("ottobre") || l.Contains("novembre") || l.Contains("dicembre"));
        }

        private void AddCommentoLettura(Story l)
        {
            if (l.Lines.Count == 0) return;

            if (èoppure(l.getText()))
            {
                //c'è una seconda possibilità di lettura con opzione
                addOutPQ("#" + (int)sec + "4", sTemp);
                addOutPQ("#" + (int)sec + "5", l);
                //salto alla sezione della seconda possibilità
                if (sec == Section.primaLettura)
                    sec = Section.primaLetturaOppure;
                if (sec == Section.secondalettura)
                    sec = Section.secondaLetturaOppure;
                if (sec == Section.vangelo)
                    sec = Section.vangeloOppure;
                prog = -1;
                //V2
                sTemp = new Story();
                //
                return;
            }
            else if (èInizioSalmo(l))
            {
                addOutPQ("#" + (int)sec + "4", sTemp);
                sTemp = new Story();
                sec = Section.salmo;
                prog = 0;
                addOutPQ("#" + (int)sec + "0", l);
                return;
            }
            else if (èAntifonaVangelo(l.getText()))
            {
                addOutPQ("#" + (int)sec + "4", sTemp);
                sTemp = new Story();
                AddInizioPrimaAntifonaVangelo(l);
                return;
            }
            else
            {
                sTemp.Addstory(l);
                //problema --> chiudo comunque il commento
                //addOutPQ("#" + (int)sec + "4", sTemp);
                //sTemp = "";
                //addProblem("problema a fine lettura");
            }
        }

        private static bool èInizioSalmo(Story s)
        {
            string l = s.getText();
            return (l.StartsWith("dal salmo", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("1 sam", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("salmo", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("salmi", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("cant.", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("sal ", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("cant ", StringComparison.CurrentCultureIgnoreCase)) && l.Length < 40;
        }

        private void AddTestoLettura(Story l)
        {
            int indDio = l.getText().IndexOf("Parola di Dio");
            int indSignore = l.getText().IndexOf("Parola del Signore");

            if (indDio >= 0)
            {
                // trovato parola di Dio

                sTemp.Addstory(l.SubstoryLeft("Parola di Dio"));

                addOutPQ("#" + (int)sec + "2", sTemp);
                sTemp = new Story();
                addOutPQ("#" + (int)sec + "3", new Story("Parola di Dio.", SystemFonts.DefaultFont));
                prog = 3;
            }
            else if (indSignore >= 0)
            {
                //trovato parola del Signore
                sTemp.Addstory(l.SubstoryLeft("Parola del Signore"));
                addOutPQ("#" + (int)sec + "2", sTemp);
                sTemp = new Story();
                addOutPQ("#" + (int)sec + "3", new Story("Parola del Signore.", SystemFonts.DefaultFont));
                prog = 3;
            }
            else
            {
                //non trovato nulla.. fa parte del testo della lettura
                sTemp.Addstory(l);
            }


        }

        private bool èInizioLettura(string l)
        {
            // inizia lettura
            if (l.StartsWith("dal libro", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dal primo libro", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dal secondo libro", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla prima lettera", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla seconda lettera", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla terza lettera", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dagli atti", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dal cantico", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla lettera", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else return false;
        }

        private bool èInizioVangelo(string l)
        {
            // inizia vangelo
            if (l.StartsWith("dal vangelo", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("passione di nostro signore", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else return false;
        }

        private bool èRiferimento(string l)
        {
            // inizia vangelo
            if (l.IndexOf("(") >= 0 && l.IndexOf(")") >= 0)
                return true;
            else return false;
        }
        private void addInizioLetturaOVangelo(Story s, Section newSec)
        {
            string l = s.getText();
            int inizioRif = l.IndexOf('(');
            //titolo prima lettura
            addOutPQ("#" + (int)newSec + "0", s.SubstoryLeft("("));
            prog = 0;
            sec = newSec;
            if (inizioRif >= 0) AddRiferimento(s, newSec);
        }

        private void AddRiferimento(Story s, Section newSec)
        {
            string l = s.getText();
            int inizioRif = l.IndexOf('(');
            if (inizioRif >= 0)
            {
                //riferimenti lettura
                addOutPQ("#" + (int)newSec + "1", s.SubstoryRight("("));
                prog = 1;
                sec = newSec;
            }
            else addProblem("manca rif lettura/vangelo");
        }
        private void addOutPQ(string tag, Story l)
        {
            try
            {
                if (!dicPQTag.ContainsKey(giornoCorrente)) dicPQTag.Add(giornoCorrente, new Dictionary<string, Story>());
                dicPQTag[giornoCorrente].Add(tag, l);

                outPQTag += tag + l + Environment.NewLine;
            }
            catch (System.Exception ex)
            {
                addProblem(ex.Message);
            }
        }


        private void AppendOutPQ(string tag, Story l)
        {
            try
            {
                dicPQTag[giornoCorrente][tag].AppendText(l);

                outPQTag += tag + l + Environment.NewLine;
            }
            catch (System.Exception ex)
            {
                addProblem(ex.Message);
            }
        }
        private void addProblem(string p)
        {
            sproblem += giornoCorrente + ": " + p + " | linea:" + lineNum + " s:" + sec + " p:" + prog + " l:\"" + sTemp.getText() + "\"" + Environment.NewLine;
        }



    }
}
