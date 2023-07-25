using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections;
using SAE;
using System.Xml.Linq;
using unoidl.com.sun.star.frame;
using Microsoft.Office.Interop.Word;
using Application = System.Windows.Forms.Application;
using System.Diagnostics.Eventing.Reader;

namespace PQCreator
{

    public class Book
    {
        public List<Chapter> chapters = new List<Chapter>();
    }

    public class Chapter
    {
        public string title;
        public List<Paragraph> paragraphs = new List<Paragraph>();
    }
    public class Paragraph
    {
        public List<Tuple<string, string>> parts = new List<Tuple<string, string>>();

        static string startItalic = "<emphasis";
        static string startBold = "<emphasis role=\"strong\"";
        static string end = "</emphasis>";
        static string footnote = "<footnote>";

        public Paragraph(XElement parText, ref string problems)
        {
            parts = ImportParts(parts, parText, ref problems);
        }


        public Paragraph(string parText, ref string problems)
        {
            parts.Add(Tuple.Create(parText, "N"));
        }

        private List<Tuple<string, string>> ImportParts(List<Tuple<string, string>> parts, XElement partToAdd, ref string problems)
        {
            foreach (var n in partToAdd.Nodes())
            {
                string nText = n.ToString().Replace("  ", "").Replace("   ", "").Replace("\r\n", " ");
                if (nText.StartsWith(startBold))
                {
                    //aggiungo bold
                    string bold = nText.Substring(nText.IndexOf(">") + 1, nText.IndexOf(end) - nText.IndexOf(">") - 1);
                    if (bold.Contains(">") || bold.Contains("<"))
                        problems += "not valid char in :" + bold + Environment.NewLine;
                    else parts.Add(Tuple.Create(bold, "B"));
                }
                else if (nText.StartsWith(startItalic))
                {
                    //aggiungo italic
                    string italic = nText.Substring(nText.IndexOf(">") + 1, nText.IndexOf(end) - nText.IndexOf(">") - 1);
                    if (italic.Contains(">") || italic.Contains("<"))
                        problems += "not valid char in :" + italic + Environment.NewLine;
                    else parts.Add(Tuple.Create(italic, "C"));
                }
                else if (!nText.StartsWith(footnote))
                {
                    if (nText.Contains(">") || nText.Contains("<"))
                        problems += "not valid char in :" + nText + Environment.NewLine;
                    else parts.Add(Tuple.Create(nText, "N"));
                }
            }
            return parts;
        }
    }

    public static class BXMLImporter
    {
        public static Book book;

        public static string dirNITF;
        public static bool LoadBXMLFile(string FilePath)
        {

            book = new Book();
            string problems = "";
            XDocument bxml = XDocument.Load(FilePath);

            bool newChapter = true;

            Chapter currentChapter = new Chapter();

            foreach (XElement element in bxml.Root.Descendants("para"))
            {
                if (element.Name.LocalName == "para")
                {
                    string parText = element.Value;
                    if (newChapter && parText.Length > 0)
                    {
                        currentChapter = new Chapter();
                        currentChapter.title = element.Value.Trim();
                        newChapter = false;
                        continue;
                    }

                    if (parText.Contains("##CAPBRAKE##"))
                    {
                        book.chapters.Add(currentChapter);
                        newChapter = true;
                        continue;
                    }

                    Paragraph p = new Paragraph(element, ref problems);

                    if (p != null) currentChapter.paragraphs.Add(p);
                }
                else
                {
                    int i = 1;
                }


            }

            //aggiungo ultimo capitolo
            book.chapters.Add(currentChapter);


            if (problems.Length > 0)
            {
                Log.WriteLog(problems);
                return false;
            }


            return true;
        }


        public static bool LoadTXTSempreFile(string FilePath)
        {

            book = new Book();
            string problems = "";

            bool newChapter = true;

            Chapter currentChapter = new Chapter();

            foreach (string parText in File.ReadAllLines(FilePath))
            {
                if (newChapter && parText.Length > 0)
                {
                    currentChapter = new Chapter();
                    currentChapter.title = parText;
                    newChapter = false;
                    continue;
                }

                if (parText.Contains("##CAPBRAKE##"))
                {
                    book.chapters.Add(currentChapter);
                    newChapter = true;
                    continue;
                }

                Paragraph p = new Paragraph(parText, ref problems);

                if (p != null) currentChapter.paragraphs.Add(p);
            }

            //aggiungo ultimo capitolo
            book.chapters.Add(currentChapter);

            if (problems.Length > 0)
            {
                Log.WriteLog(problems);
                return false;
            }


            return true;
        }

        internal static void CreateNITF(string nitffile, string PQTitle)
        {
            string bookBaseNITFFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "NITFBookBase.zip");
            dirNITF = Path.Combine("c:\\temp\\PQ\\", PQTitle);
            if (Directory.Exists(dirNITF)) Directory.Delete(dirNITF, true);
            // unzippo file epub base
            utility.UnzipFile(bookBaseNITFFile, dirNITF);

            CreaFileCapitoli();
            utility.CreateZipFile(Path.GetDirectoryName(nitffile), Path.GetFileName(nitffile), new DirectoryInfo(dirNITF));
        }

        private static void CreaFileCapitoli()
        {
            try
            {
                string BaseCapitolo = File.ReadAllText(Path.Combine(dirNITF, @"articles\article.xml"));

                int index = 1;
                //ciclo su tutti i capitoli
                foreach (var chapter in book.chapters)
                {
                    string nuovoCap = BaseCapitolo;
                    string testo = "";

                    //ciclo su tutti i paragrafi
                    foreach (var par in chapter.paragraphs)
                    {
                        testo += "<p>";
                        foreach (var x in par.parts)
                        {
                            if (x.Item2 == "C")
                            {
                                testo += "<i>" + x.Item1 + "</i>";
                            }
                            else if (x.Item2 == "B")
                            {
                                testo += "<b>" + x.Item1 + "</b>";
                            }
                            else testo += x.Item1;
                        }
                        testo += "</p>" + Environment.NewLine;
                    }

                    nuovoCap = nuovoCap.Replace("%%CONTENT%%", testo);
                    nuovoCap = nuovoCap.Replace("%%HEADLINE%%", chapter.title);
                    nuovoCap = nuovoCap.Replace("%%TITLE%%", chapter.title);

                    //scrivo file xml del giorno
                    File.WriteAllText(Path.Combine(dirNITF, @"Articles\" + "article_" + index.ToString("00") + ".xml"), nuovoCap);
                    index++;
                }

                File.Delete(Path.Combine(dirNITF, @"articles\article.xml"));
            }
            catch (Exception ex)
            {
                utility.writelog("creaFileLibro: " + ex.Message);
                throw ex;
            }


        }
    }
}
