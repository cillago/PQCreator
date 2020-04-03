using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PQCreator2
{
    public class CTextpart
    {
        public string text;
        public Font font;

        public CTextpart(string stext,  Font sfont)
        {
            this.text = stext;
            this.font = sfont;
        }
    }

    class RTFLine
    {
        private List<CTextpart> TextParts;
        public int length = 0;

        public RTFLine()
        { 
            TextParts = new List<CTextpart>();
        }

        public void AddTextPart(CTextpart Text)
        {
            TextParts.Add(Text);
            length += Text.text.Length;
        }

        public List<CTextpart> GetTextParts()
        {
            return TextParts;
        }

        public CTextpart GetTextPart(int i)
        {
            return TextParts[i];
        }

        public string GetAllText()
        {
            return GetAllText(0);
        }

        public string GetAllText(int firstchars)
        {
            string ret = "";
            foreach (CTextpart TP in TextParts)
            {
                ret += TP.text;
                if (ret.Length > firstchars && firstchars > 0) return ret;
            }
            return ret;
        }
    }


    class RTFText
    {
        private List<RTFLine> Lines;
        public int length = 0;
        public RTFText()
        {
            this.Lines = new List<RTFLine>();
        }

        public RTFText(RichTextBox RTFB)
        {
            Lines = new List<RTFLine>();
            Font PrevFont=null;
            string TempS="";
            RTFLine RTFLineTemp = new RTFLine();
            for (int i = 0; i<RTFB.Text.Length; i++)
            {
             RTFB.Select(i,1);

             if (i == 0) PrevFont = RTFB.SelectionFont;

             if (RTFB.Text[i]=='\n')
             {
                RTFLineTemp.AddTextPart(new CTextpart(TempS, PrevFont));
                //fine riga
                Lines.Add(RTFLineTemp);
                RTFLineTemp = new RTFLine();
                TempS = "";
             }
             else if (RTFB.SelectionFont.Equals(PrevFont))
             {
                 //sempre stesso font
                 TempS+=RTFB.Text[i];
             }
             else
             {
                 //fine stesso font
                 if (TempS.Length > 0) RTFLineTemp.AddTextPart(new CTextpart(TempS, PrevFont));
                 TempS = RTFB.Text[i].ToString();
                 PrevFont=RTFB.SelectionFont;
             }
            }
            Lines.Add(RTFLineTemp);
        }
    }
}
