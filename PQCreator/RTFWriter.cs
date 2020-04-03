using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using SAE;


namespace PQCreator
{
    class RTFTXTWriter
    {
        private Dictionary<string, string[]> dictRTF;

        public void LoadRTFStyle(string nomefile)
        {
            StreamReader sr = File.OpenText(nomefile);
            dictRTF = new Dictionary<string, string[]>();
            while (!sr.EndOfStream)
            {
                string[] lineKeyVal = sr.ReadLine().Split(';');
                dictRTF.Add(lineKeyVal[0], lineKeyVal);
            }
            sr.Close();
        }

        public void populateRTFBox(Dictionary<string, Dictionary<string, string>> dicPQTag, ref RichTextBox rtfB)
        {
            rtfB.Clear();
            foreach (KeyValuePair<string, Dictionary<string, string>> giorno in dicPQTag)
            {
                foreach (KeyValuePair<string, string> singleTag in giorno.Value)
                {
                    //prima del testo
                    if (dictRTF[singleTag.Key][2].Length > 0) rtfB.AppendText(Environment.NewLine);


                    //aggiungo il tag in rosso
                    rtfB.SelectionFont = new Font(rtfB.SelectionFont.FontFamily,
                                                11,
                                                FontStyle.Bold);
                    rtfB.SelectionColor = Color.FromName("Red");
                    rtfB.AppendText(singleTag.Key);

                    //aggiungo il testo in nero
                    rtfB.SelectionFont = new Font(rtfB.SelectionFont.FontFamily,
                                                Convert.ToInt32(dictRTF[singleTag.Key][4]),
                                                FSfromText(dictRTF[singleTag.Key][1]));
                    rtfB.SelectionColor = Color.FromName("Black");
                    rtfB.AppendText(singleTag.Value);

                    //dopo il testo
                    if (dictRTF[singleTag.Key][3].Length > 0) rtfB.AppendText(Environment.NewLine);
                }
            }
        }
        private FontStyle FSfromText(string stylefont)
        {
            switch (stylefont)
            {
                case "bold": return FontStyle.Bold;
                case "italic": return FontStyle.Italic;
                case "regular": return FontStyle.Regular;
                default: return FontStyle.Regular;
            }
        }


        public void Save(string nomefile, ref RichTextBox richTextBoxTAG, RichTextBoxStreamType RTFforTXT)
        {
            richTextBoxTAG.SaveFile(nomefile, RTFforTXT);
        }

        internal void populateRTFBoxIDML(Dictionary<string, Dictionary<string, Story>> dicPQTag, ref RichTextBox rtfB)
        {
            rtfB.Clear();
            foreach (KeyValuePair<string, Dictionary<string, Story>> giorno in dicPQTag)
            {
                foreach (KeyValuePair<string, Story> singleTag in giorno.Value)
                {
                    //prima del testo
                    if (dictRTF[singleTag.Key][2].Length > 0) rtfB.AppendText(Environment.NewLine);


                    //aggiungo il tag in rosso
                    rtfB.SelectionFont = new Font(rtfB.SelectionFont.FontFamily,
                                                11,
                                                FontStyle.Bold);
                    rtfB.SelectionColor = Color.FromName("Red");
                    rtfB.AppendText(singleTag.Key);

                    //aggiungo il testo in nero
                    rtfB.SelectionFont = new Font(rtfB.SelectionFont.FontFamily,
                                                Convert.ToInt32(dictRTF[singleTag.Key][4]),
                                                FSfromText(dictRTF[singleTag.Key][1]));
                    rtfB.SelectionColor = Color.FromName("Black");
                    rtfB.AppendText(singleTag.Value.getText());

                    //dopo il testo
                    if (dictRTF[singleTag.Key][3].Length > 0) rtfB.AppendText(Environment.NewLine);
                }
            }
        }
    }
}
