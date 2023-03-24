using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SAE
{
    public static class DecodeIDML
    {
        public static string IDMLTempPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"IDML\");
        public static string LinkPath = @"Links\";
        public static string ConvertedPath = @"converted\";
        public static string IDMLFile = "";
        public static string IDMLConvertedPath = "";
        public static string IDMLLinkPath = "";
        public static List<Story> Stories = new List<Story>();
        public static string TextSempre;
        public static int numPagina = 0;
        public static string problems = "";
        private static Dictionary<string, FontID> dictStyles;
        private static RichTextBox rTB = new RichTextBox();


        public static void LoadIDMLFile(string FilePath)
        {
            string[] Spreads;
            TextSempre = "";
            Stories.Clear();
            rTB.Clear();
            Spreads = loadInDesignMAP();
            loadStyles();

            for (int i = 0; i < Spreads.Length; i++)
            {
                string[] ImagesName = null;
                string[] storiesID = LoadSpread(Spreads[i], ref rTB, ref ImagesName);

                for (int j = 0; j < storiesID.Length; j++)
                {
                    LoadStory(FilePath, storiesID[j], ref rTB);
                }

            }

            if (problems.Length > 0) Log.WriteLog(problems);
        }

        public static void LoadStory(string FilePath, string sID, ref RichTextBox rtfB)
        {
            //if (LoadedStories.Exists(element => element==sID)) return;

            //LoadedStories.Add(sID);

            if (Stories.Exists(element => element.ID == sID)) return;
            

            FileInfo f = new FileInfo(FilePath + "Stories\\Story_" + sID + ".xml");
            // voglio solo la story di PQ con il testo dei giorni
            if (f.Length<200000) return;

            Story story = new Story(sID);
            StreamReader sr = f.OpenText();
            string ret = "";
            Font CurrentFont = rtfB.Font;

            RichTextBox RTFTemp = new RichTextBox();

            while (!sr.EndOfStream)
            {
                string tempL = sr.ReadLine();

                // trovato nuovo font
                if (tempL.Contains("AppliedCharacterStyle=\"")) CurrentFont = extractFontFromLine(tempL);

                // leggo content
                if (tempL.Contains("<Content>"))
                {
                    rTB.SelectionFont = CurrentFont;
                    string sT = findStringWithDelimiter(tempL, "<Content>", "</Content>").Replace("&apos;", "'").Replace("&quot;", "\"");
                    if (sT.Length > 0)
                    {
                        rTB.AppendText(sT);
                        ret += sT;
                        story.AddText(sT, CurrentFont);
                    }


                    //ret += tempL.Substring(tempL.IndexOf("<Content>") + 9, tempL.IndexOf("</Content>") - tempL.IndexOf("<Content>") - 9).Replace("&apos;","'");
                }
                // leggo se c'è a capo riga
                if (tempL.Contains("<Br/>") || tempL.Contains("<Br />"))
                {
                    story.AddLineEnd();
                    rTB.AppendText(Environment.NewLine);
                }
                // dictRTF.Add(lineKeyVal[0], lineKeyVal);
            }

            //agiungo anche ultima riga
            story.AddLineEnd();

            //richTextBox1.Text = richTextBox1.Text.Trim();
            if (ret.Length > 0)
            {
                rTB.SelectionFont = new Font(rtfB.Font.FontFamily,
                                              10,
                                              FontStyle.Regular);
                rTB.AppendText(Environment.NewLine + "------------------------------------------------------------- " + Environment.NewLine);

                //CopyRTFText(ref richTextBox1, RTFTemp);
                //richTextBox1.Rtf = richTextBox1.Rtf + RTFTemp.Rtf;
            }

            Stories.Add(story);
            //richTextBox1.Refresh();
            sr.Close();
            f = null;
            //StreamReader strS = new StreamReader(IDMLPath +  "stories\\Story_" + s + ".xml");

            //XmlSerializer xSerializerS = new XmlSerializer(typeof(NMSTORY.Story));

            //NMSTORY.Story bcResponseS = (NMSTORY.Story)xSerializerS.Deserialize(strS);
        }


        public static Font extractFontFromLine(string tempL)
        {
            string CharStyle = findStringWithDelimiter(tempL, "AppliedCharacterStyle=\"", "\"");

            //carico il default dello stile del carattere
            FontID F = dictStyles[CharStyle].clone();

            // leggo PointSize se c'è
            if (tempL.Contains("PointSize=\""))
            {
                F.pointSize = findStringWithDelimiter(tempL, "PointSize=\"", "\"");
                if (F.pointSize.Contains("."))
                    F.pointSize = F.pointSize.Substring(0, F.pointSize.IndexOf("."));
            }

            // leggo // leggo FontStyle se c'è
            if (tempL.Contains("FontStyle=\""))
            {
                F.fontStyle = findStringWithDelimiter(tempL, "FontStyle=\"", "\"");

            }
            Font curFont = null;
            problems += F.GetFont(dictStyles, ref curFont);
            return curFont;
        }

        public static string[] LoadSpread(string s, ref RichTextBox rtfB, ref string[] Images)
        {
            string TextnumPagina = "";
            StreamReader sr = File.OpenText(IDMLTempPath + "Spreads\\Spread_" + s + ".xml");

            if (numPagina == 0)
            {
                numPagina++;
                TextnumPagina = "1";
            }
            else
            {
                numPagina = numPagina + 2;
                TextnumPagina = (numPagina - 1).ToString() + "-" + numPagina.ToString();
            }

            List<string> StoriesName = new List<string>();
            List<string> ImagesName = new List<string>();
            while (!sr.EndOfStream)
            {
                string tempL = sr.ReadLine();
                // leggo ParentStory="u16fc3"
                if (tempL.Contains("ParentStory=\""))
                {
                    StoriesName.Add(findStringWithDelimiter(tempL, "ParentStory=\"", "\""));
                    //StoriesName.Add(tempL.Substring(tempL.IndexOf("ParentStory=\"") + 13, tempL.IndexOf('\"', tempL.IndexOf("ParentStory=\"") + 13) - tempL.IndexOf("ParentStory=\"") - 13));
                }

                // estraggo link a immagine
                string[] Imgextensions = { "eps", "gif", "jpg", "jpeg", "png", "tif" };
                for (int i = 0; i < Imgextensions.Length; i++)
                {
                    if (tempL.Contains("LinkResourceURI=") && tempL.Contains("." + Imgextensions[i] + "\""))
                    {
                        string ImgPath = tempL.Substring(tempL.IndexOf("LinkResourceURI=") + 17, tempL.IndexOf("." + Imgextensions[i] + "\"") - tempL.IndexOf("LinkResourceURI=") - 16 + Imgextensions[i].Length);
                        ImagesName.Add(ImgPath.Substring(ImgPath.LastIndexOf("/") + 1));
                    }
                }

            }
            sr.Close();

            if (StoriesName.Count > 0)
            {
                rtfB.SelectionFont = new Font(rtfB.Font.FontFamily,
                                             10,
                                             FontStyle.Regular);
                rtfB.AppendText("--------------------------------------------- PAGINE: " + TextnumPagina + " -----------------------------------------" + Environment.NewLine + Environment.NewLine);

            }
            Images = ImagesName.ToArray();
            return StoriesName.ToArray();
        }

        public static string[] loadInDesignMAP()
        {
            StreamReader sr = File.OpenText(IDMLTempPath + "designmap.xml");
            List<string> SpreadName = new List<string>();
            while (!sr.EndOfStream)
            {
                string tempL = sr.ReadLine();
                // leggo ParentStory="u16fc3"
                if (tempL.Contains("<idPkg:Spread src=\"Spreads/Spread_"))
                {
                    SpreadName.Add(findStringWithDelimiter(tempL, "<idPkg:Spread src=\"Spreads/Spread_", "."));
                    //SpreadName.Add(tempL.Substring(tempL.IndexOf("<idPkg:Spread src=\"Spreads/Spread_") + 34, tempL.IndexOf('.', tempL.IndexOf("<idPkg:Spread src=\"Spreads/Spread_") + 34) - tempL.IndexOf("<idPkg:Spread src=\"Spreads/Spread_") - 34));
                }
            }
            sr.Close();
            return SpreadName.ToArray();
        }

        public static string findStringWithDelimiter(string StringBase, string before, string after)
        {
            return StringBase.Substring(StringBase.IndexOf(before) + before.Length, StringBase.IndexOf(after, StringBase.IndexOf(before) + before.Length) - StringBase.IndexOf(before) - before.Length);

        }

        public static void loadStyles()
        {
            StreamReader sr = File.OpenText(IDMLTempPath + @"\resources\styles.xml");
            dictStyles = new Dictionary<string, FontID>();
            FontID LastFont = new FontID();
            while (!sr.EndOfStream)
            {
                string tempL = sr.ReadLine();
                if (tempL.Contains("CharacterStyle Self=\""))
                {
                    //int PointSize=-1;
                    //bool FoundPointSize = false;
                    //bool FoundStyle = false;
                    //FontStyle Fs = new FontStyle();
                    //string charStyle = extractFontFromLine(tempL,"CharacterStyle Self=\"",ref PointSize,ref  Fs, ref FoundPointSize, ref FoundStyle);
                    //if ((FoundPointSize & FoundStyle) == false) extractFontFromStyleFile(charStyle, ref PointSize, ref Fs, ref FoundPointSize, ref FoundStyle);

                    string charstyle = findStringWithDelimiter(tempL, "CharacterStyle Self=\"", "\"");
                    string pointsize = "";
                    string fontstyle = "";
                    if (tempL.Contains("PointSize=\""))
                    {
                        string Strpointsize = findStringWithDelimiter(tempL, "PointSize=\"", "\"");
                        if (Strpointsize.Contains("."))
                            pointsize = Strpointsize.Substring(0, Strpointsize.IndexOf("."));
                        else pointsize = Strpointsize;
                    }

                    if (tempL.Contains("FontStyle=\""))
                    {
                        fontstyle = findStringWithDelimiter(tempL, "FontStyle=\"", "\"");
                    }

                    LastFont = new FontID(charstyle, pointsize, fontstyle);
                    //aggiungo Font
                    dictStyles.Add(charstyle, LastFont);
                }

                // <BasedOn type="object">CharacterStyle/DIN</BasedOn>
                if (tempL.Contains("<BasedOn type=\"object\">"))
                {
                    LastFont.basedOn = findStringWithDelimiter(tempL, "<BasedOn type=\"object\">", "</BasedOn>");
                }
            }
            sr.Close();
        }

        internal static void Clean()
        {
            Stories = new List<Story>();
            numPagina = 0;
            problems = "";
            dictStyles = null;
            rTB = new RichTextBox();
        }

        internal static void UnzipIDMLFile(string IDMLFile)
        {
            try
            {
                if (Directory.Exists(IDMLTempPath)) Directory.Delete(IDMLTempPath, true);
            }
            catch (Exception ex)
            {
                Log.WriteLog("LoadBaseFile: " + ex.Message);
            }

            // unzippo file epub base
            PQCreator.utility.UnzipFile(IDMLFile, IDMLTempPath);

        }
    }
}
