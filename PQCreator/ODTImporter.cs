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

namespace PQCreator
{
    public class ODTImporter
    {
        private string ODTText = "";
        private string sproblem = "";
        private string outPQTag;
        private Section sec;
        private int prog;
        private int lineNum = 0;
        private int NumGiorni = 0;
        private string giornoCorrente;
        //variabile contenente multiriga commenti e letture
        private string sTemp;
        private ArrayList giorni;
        private ArrayList giorniTag;
        private string IncrementalPQTag = "";

        public Dictionary<string, Dictionary<string, string>> dicPQTag;

        public void OpenODT(string inputFile)
        {

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
            ODTText = File.ReadAllText(IFile,Encoding.Default );
        }

        public string PutTag()
        {
            giorniTag = new ArrayList();
            giorni = SplitGiorni(ODTText);
            dicPQTag = new Dictionary<string, Dictionary<string, string>>();

            foreach (string giorno in giorni)
            {
                sec = Section.giorno;
                prog = 0;
                giornoCorrente = "bo!";
                sTemp = "";
                outPQTag = "";
                outPQTag = PutTagOnDay(giorno);
                giorniTag.Add(outPQTag);
                IncrementalPQTag += outPQTag;
            }

            foreach (KeyValuePair<string, Dictionary<string, string>> giorno in dicPQTag)
            {

                string [] checkTag= {"#10","#20","#40","#50"};
                for (int i = 0; i < checkTag.Length; i++)
			    {
			        if (!giorno.Value.ContainsKey(checkTag[i])) 
                        sproblem += giorno.Key + " manca tag:" + checkTag[i] + Environment.NewLine;
                }

                //DOMENICA --> deve esserci seconda lettura
                if (giorno.Value.ContainsKey("#09") && !giorno.Value.ContainsKey("#30")) sproblem+=giorno.Key + " manca tag:#30" + Environment.NewLine;

			}

            return sproblem;
        }

        private ArrayList SplitGiorni(string s)
        {
            string[] Lines = s.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            string tempGiorno = "";
            string l;
            ArrayList SplitG = new ArrayList();

            foreach (string CurLine in Lines)
            {
                l = CurLine.Trim();
                if (èInizioGiornoSettimana(l) || èInizioDomenica(l))
                {
                    if (tempGiorno.Length > 0)
                    {
                        SplitG.Add(tempGiorno);
                    }
                    NumGiorni++;
                    tempGiorno = l + Environment.NewLine;
                }
                else tempGiorno += l + Environment.NewLine;
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

        private ArrayList SplitGiorniODT(string s)
        {
            string[] Lines = s.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            string tempGiorno = "";
            string l;
            ArrayList SplitG = new ArrayList();

            foreach (string CurLine in Lines)
            {
                l = CurLine.Trim();
                if (èInizioGiornoSettimana(l) || èInizioDomenica(l))
                {
                    if (tempGiorno.Length > 0)
                    {
                        SplitG.Add(tempGiorno);

                    }
                    NumGiorni++;
                    tempGiorno = l + Environment.NewLine;
                }
                else tempGiorno += l + Environment.NewLine;
            }
            SplitG.Add(tempGiorno);
            NumGiorni++;
            return SplitG;
        }

        private string PutTagOnDay(string s)
        {

            string[] Lines = s.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            //MessageBox.Show("linee:" + Convert.ToString(Lines.GetLength(0)));
            try
            {
                string l;

                foreach (string CurLine in Lines)
                {
                    lineNum++;
                    l = CurLine.Trim();
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
                                                addInizioGiornoSettimana(l);
                                            }
                                            //GIORNO domenica
                                            else if (èInizioDomenica(l))
                                            {
                                                addInizioDomenica(l);
                                            }
                                            else
                                                addProblem("manca giorno");
                                            sTemp = "";
                                            break;
                                        }

                                    //GIORNO
                                    case 1:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            //aggiungo santi --> tutti sulla stessa riga
                                            addOutPQ("#02", l);
                                            prog = 2;
                                            sTemp = "";
                                            break;
                                        }
                                    //NOME SANTI
                                    case 2:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(l, Section.primaLettura); break; }
                                            //controllo se la stringa è lunga --> allora è la storia dei santi
                                            if (l.Length > 90)
                                            {
                                                //l = AggiungiTagNomeSanti(l, dicPQTag[giornoCorrente]["#02"]);
                                                addOutPQ("#03", l);
                                                prog = 3;
                                            }
                                            else
                                            {
                                                if (èGiornataMondiale(l)) addGiornataMondiale(l);
                                                else
                                                {
                                                 //suppongo sia festività
                                                 addOutPQ("#04", l);
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
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(l, Section.primaLettura); break; }

                                            if (èGiornataMondiale(l)) addGiornataMondiale(l);
                                            else
                                            {
                                                //suppongo sia festività
                                                addOutPQ("#04", l);
                                                prog = 4;
                                            }
                                            break;
                                        }
                                    // FESTA RELIGIOSA IMPORTANTE
                                    case 4:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(l, Section.primaLettura); break; }

                                            if (èGiornataMondiale(l)) addGiornataMondiale(l);
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
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(l, Section.primaLettura); break; }

                                            //suppongo sia testo altra festa
                                            addOutPQ("#06", l);
                                            //vado comunque avanti
                                            prog = 6;
                                            break;
                                        }
                                    // TESTO ALTRA FESTA
                                    case 6:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            if (èInizioLettura(l)) { addInizioLetturaOVangelo(l, Section.primaLettura); break; }

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
                                                addInizioLetturaOVangelo(l, sec);
                                                break;
                                            }
                                            else if (èAntifonaVangelo(l))
                                            {
                                                addInizioLetturaOVangelo(l, sec);
                                                break;
                                            }
                                            else
                                            {
                                                sTemp += l;
                                                addProblem("manca titolo e rif lettura");
                                            }
                                            break;
                                        }

                                    //titolo e riferimenti
                                    case 1:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            AddTestoLettura(l);
                                            break;
                                        }
                                    case 3:
                                        {
                                            //riga vuota salto
                                            if (l.Length == 0) break;
                                            AddCommentoLettura(l);
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
                                            if (l.Length == 0) break;
                                            Add1AntifonaSalmo(l);
                                            break;
                                        }
                                    case 1:
                                        {
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (l.Length == 0)
                                            {
                                                AddStrofaSalmo(sTemp, 6);
                                                sTemp = "";
                                                break;
                                            }
                                            if (èoppure(l)) Add1OppureSalmo(l);
                                            else
                                            {
                                                if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                                                else sTemp += l;
                                            }
                                            break;
                                        }
                                    case 2:
                                        {
                                            if (l.Length == 0) break;
                                            Add2AntifonaSalmo(l);
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (l.Length == 0 && sTemp.Length == 0) break;
                                            if (l.Length == 0)
                                            {
                                                AddStrofaSalmo(sTemp, 6);
                                                sTemp = "";
                                                break;
                                            }
                                            if (èoppure(l)) Add2OppureSalmo(l);
                                            else
                                            {
                                                if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                                                else sTemp += l;
                                            }
                                            break;
                                        }
                                    case 4:
                                        {
                                            if (l.Length == 0) break;
                                            Add3AntifonaSalmo(l);
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
                                                    sTemp = "";
                                                }
                                                addInizioLetturaOVangelo(l, Section.secondalettura);
                                                break;
                                            }

                                            if (èAntifonaVangelo(l))
                                            {
                                                if (sTemp.Length > 0)
                                                {
                                                    AddStrofaSalmo(sTemp, prog + 1);
                                                    sTemp = "";
                                                }
                                                AddInizioPrimaAntifonaVangelo(l);
                                                break;
                                            }
                                            if (l.Length == 0)
                                            {
                                                AddStrofaSalmo(sTemp, prog + 1);
                                                sTemp = "";
                                                break;
                                            }
                                            else if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                                            else sTemp += l;
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
                                                sTemp = "";
                                                addOutPQ("#42", l);
                                                //sec = Section.vangelo;
                                                //prog = -1;
                                                prog = 2;
                                            }
                                            else if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                                            else sTemp += l;
                                            break;
                                        }

                                    case 2:
                                        {
                                            if (l.Length == 0) break;
                                            if (èoppure(l))
                                            {
                                                AddOppureAntifonaVangelo(l);
                                            }
                                            else if (èInizioVangelo(l))
                                            {
                                                addInizioLetturaOVangelo(l, Section.vangelo);
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            if (l.Length == 0) break;
                                            if (èAntifonaVangelo(l))
                                            {
                                                AddInizioSecondaAntifonaVangelo(l);
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
                                                sTemp = "";
                                                addOutPQ("#46", l);
                                                sec = Section.vangelo;
                                                prog = -1;
                                            }
                                            else if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                                            else sTemp += l;
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
                    sTemp = "";
                }

            }
            catch (System.Exception ex)
            {
                addProblem("eccezione: " + ex.Message);
            }

            return outPQTag;
        }

        private bool èGiornataMondiale(string l)
        {
            return (l.ToLower().IndexOf("giornata") >= 0 && l.Length < 130);
        }

        private void addGiornataMondiale(string l)
        {
            addOutPQ("#05", l);
            prog = 5;
        }

        private string AggiungiTagNomeSanti(string Vita, string santi)
        {
            string[] NomiSanti= santi.Split(new char[]{' ','.',';'},StringSplitOptions.RemoveEmptyEntries);
            int progSanti=0;
            for (int i = 0; i < NomiSanti.Length; i++)
            {

                if (NomiSanti[i].Length > 3)
                {

                    if (Vita.IndexOf(NomiSanti[i]) >= 0)
                    {
                        progSanti++;
                        //al posto del nome del santo metto il tag
                        Vita=Vita.Replace(NomiSanti[i], "#07_" + progSanti.ToString());
                        //aggingo il tag alla lista dei tag del giorno
                        addOutPQ("#07_" + progSanti.ToString(), NomiSanti[i]);
                    }
                }
            }
            return Vita;
        }

        private void AddInizioPrimaAntifonaVangelo(string l)
        {
            addOutPQ("#40", l);
            sec = Section.antifonaVangelo;
            prog = 0;
        }

        private void AddInizioSecondaAntifonaVangelo(string l)
        {
            addOutPQ("#44", l);
            sec = Section.antifonaVangelo;
            prog = 4;
        }

        private bool èAntifonaVangelo(string l)
        {
            return l.StartsWith("alleluia", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("lode ", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("gloria a te", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("gloria e lode", StringComparison.InvariantCultureIgnoreCase);
        }

        private void AddStrofaSalmo(string l, int p)
        {
            //metto p in esadecimale per sezione salmo
            addOutPQ("#2" + p.ToString("X"), l);
            prog = p;
        }

        private string hex(int p, string l)
        {
            throw new NotImplementedException();
        }

        private void Add1AntifonaSalmo(string l)
        {
            addOutPQ("#21", l);
            prog = 1;
        }

        private void Add1OppureSalmo(string l)
        {
            addOutPQ("#22", l);
            prog = 2;
        }

        private void Add2OppureSalmo(string l)
        {
            addOutPQ("#24", l);
            prog = 4;
        }
        
        private void AddOppureAntifonaVangelo(string l)
        {
            addOutPQ("#43", l);
            prog = 3;
        }

        private void Add2AntifonaSalmo(string l)
        {
            addOutPQ("#23", l);
            prog = 3;
        }

        private void Add3AntifonaSalmo(string l)
        {
            addOutPQ("#25", l);
            prog = 5;
        }

        private bool èoppure(string l)
        {
            return l.StartsWith("oppure", StringComparison.CurrentCultureIgnoreCase) && l.Length < 12;
        }

        private void addInizioDomenica(string l)
        {
            giornoCorrente = l;
            addOutPQ("#09", l);
            NumGiorni++;
            prog = 1;
        }

        private void addInizioGiornoSettimana(string l)
        {
            giornoCorrente = l;
            addOutPQ("#01", l);
            NumGiorni++;
            prog = 1;
        }

        private static bool èInizioGiornoSettimana(string l)
        {
            l = l.ToLower();
            return (l.StartsWith("luned", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("marted", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("mercoled", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("gioved", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("sabato", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("venerd", StringComparison.InvariantCultureIgnoreCase)) &&
                (l.Contains("gennaio")|| l.Contains("febbraio")|| l.Contains("marzo")|| l.Contains("aprile")|| l.Contains("maggio")|| l.Contains("giugno")|| l.Contains("luglio")|| l.Contains("agosto")|| l.Contains("settembre")|| l.Contains("ottobre")|| l.Contains("novembre")|| l.Contains("dicembre"));
        }
        private static bool èInizioDomenica(string l)
        {
            //se festività fuori domenica
            //if (l.ToUpper() == l) return true;

            l = l.ToLower();
            return l.StartsWith("domenica", StringComparison.InvariantCultureIgnoreCase) &&
                (l.Contains("gennaio") || l.Contains("febbraio") || l.Contains("marzo") || l.Contains("aprile") || l.Contains("maggio") || l.Contains("giugno") || l.Contains("luglio") || l.Contains("agosto") || l.Contains("settembre") || l.Contains("ottobre") || l.Contains("novembre") || l.Contains("dicembre"));
        }

        private void AddCommentoLettura(string l)
        {
            if (l.Length == 0) return;

            if (èoppure(l))
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
                sTemp = "";
                //
                return;
            }
            else if (èInizioSalmo(l))
            {
                addOutPQ("#" + (int)sec + "4", sTemp);
                sTemp = "";
                sec = Section.salmo;
                prog = 0;
                addOutPQ("#" + (int)sec + "0", l);
                return;
            }
            else if (èAntifonaVangelo(l))
            {
                addOutPQ("#" + (int)sec + "4", sTemp);
                sTemp = "";
                AddInizioPrimaAntifonaVangelo(l);
                return;
            }
            else
            {
                if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                else sTemp += l;
                //problema --> chiudo comunque il commento
                //addOutPQ("#" + (int)sec + "4", sTemp);
                //sTemp = "";
                //addProblem("problema a fine lettura");
            }
        }

        private static bool èInizioSalmo(string l)
        {
            return (l.StartsWith("dal salmo", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("salmo", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("salmi", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("cant.", StringComparison.CurrentCultureIgnoreCase) || l.StartsWith("1 sam", StringComparison.CurrentCultureIgnoreCase)) && l.Length < 40;
        }

        private void AddTestoLettura(string l)
        {
            int indDio = l.IndexOf("Parola di Dio");
            int indSignore = l.IndexOf("Parola del Signore");

            if (indDio >= 0)
            {
                // trovato parola di Dio

                if (sTemp.Length > 0) sTemp += Environment.NewLine + l.Substring(0, indDio);
                else sTemp += l.Substring(0, indDio);

                addOutPQ("#" + (int)sec + "2", sTemp);
                sTemp = "";
                addOutPQ("#" + (int)sec + "3", "Parola di Dio.");
                prog = 3;
            }
            else if (indSignore >= 0)
            {
                //trovato parola del Signore
                if (sTemp.Length > 0) sTemp += Environment.NewLine + l.Substring(0, indSignore);
                else sTemp +=  l.Substring(0, indSignore);
                addOutPQ("#" + (int)sec + "2", sTemp);
                sTemp = "";
                addOutPQ("#" + (int)sec + "3", "Parola del Signore.");
                prog = 3;
            }
            else
            {
                //non trovato nulla.. fa parte del testo della lettura
                if (sTemp.Length > 0) sTemp += Environment.NewLine + l;
                else sTemp += l;
            }


        }

        private bool èInizioLettura(string l)
        {
            // inizia lettura
            if ((l.StartsWith("dal libro", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dal primo libro", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dal secondo libro", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla prima lettera", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla seconda lettera", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla terza lettera", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dagli atti", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dal cantico", StringComparison.InvariantCultureIgnoreCase) ||
                l.StartsWith("dalla lettera", StringComparison.InvariantCultureIgnoreCase)) && l.IndexOf("(") > 0 && l.IndexOf(")") > 0)
                return true;
            else return false;
        }

        private bool èInizioVangelo(string l)
        {
            // inizia vangelo
            if ((l.StartsWith("dal vangelo", StringComparison.InvariantCultureIgnoreCase) || l.StartsWith("passione di nostro signore", StringComparison.InvariantCultureIgnoreCase) )&& l.IndexOf("(") > 0 && l.IndexOf(")") > 0)
                return true;
            else return false;
        }

        private void addInizioLetturaOVangelo(string l, Section newSec)
        {
            int inizioRif = l.IndexOf('(');
            if (inizioRif > 0)
            {
                //titolo prima lettura
                addOutPQ("#" + (int)newSec + "0", l.Substring(0, inizioRif - 1));
                //riferimenti prima lettura
                addOutPQ("#" + (int)newSec + "1", l.Substring(inizioRif));
                prog = 1;
                sec = newSec;
            }
            else addProblem("manca rif lettura/vangelo");
        }

        private void addOutPQ(string tag, string l)
        {
            try
            {
                if (!dicPQTag.ContainsKey(giornoCorrente)) dicPQTag.Add(giornoCorrente, new Dictionary<string, string>());
                dicPQTag[giornoCorrente].Add(tag, l);

                outPQTag += tag + l + Environment.NewLine;
            }
            catch (System.Exception ex)
            {
                addProblem(ex.Message);
            }
        }

        private void addProblem(string p)
        {
            sproblem += giornoCorrente + ": " + p   +" | linea:" + lineNum + " s:" + sec + " p:" + prog + " l:" + sTemp + Environment.NewLine;
        }



    }
}
