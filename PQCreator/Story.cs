using System;
using System.Collections.Generic;
using System.Text;
//using System.Web.UI;
using System.IO;
using System.Drawing;

namespace SAE
{

    public class CTextpart
    {
        public string text;
        public Font font;

        public CTextpart(string stext, Font sfont)
        {
            this.text = stext;
            this.font = sfont;
        }
    }

    public class StoryLine
    {
        private List<CTextpart> textParts;
        private string text = "";

        public StoryLine()
        {
            textParts = new List<CTextpart>();
        }

        public StoryLine(CTextpart one)
        {
            textParts = new List<CTextpart>();
            textParts.Add(one);
            text=one.text;
        }

        public StoryLine(string tex, Font font)
        {
            textParts = new List<CTextpart>();
            textParts.Add(new CTextpart(tex,font));
            text = tex;
        }

        public void AddTextParts(CTextpart tp)
        {
            textParts.Add(tp);
            text += tp.text;
        }

        public void AppendTextParts(int index, string s)
        {
            textParts[index].text += s;
            text += s;
        }

        public CTextpart GetTextPart(int i)
        {
            return textParts[i];
        }

        public List<CTextpart> TextParts
        {
            get
            {
              return textParts;
            }
            set 
            {
                textParts=value;
            }
        }

        internal string getText()
        {
            return text;
        }
    }

    public class Story
    {
        public string ID;

        public List<StoryLine> Lines;
        private StoryLine TempLine;

        public Story()
        {
            this.ID = "";
            this.Lines = new List<StoryLine>();
            TempLine = new StoryLine();
        }
        public Story(string sID)
        {
            this.ID = sID;
            this.Lines = new List<StoryLine>();
            TempLine = new StoryLine();
        }

        public Story(StoryLine CurLine)
        {
            this.Lines = new List<StoryLine>();
            this.Lines.Add(CurLine);
            TempLine = new StoryLine();
        }

        public Story(string text, Font font)
        {
            this.Lines = new List<StoryLine>();
            TempLine = new StoryLine();
            TempLine.AddTextParts(new CTextpart(text, font));
            this.Lines.Add(TempLine);
            TempLine = new StoryLine();
        }
        public string getText()
        {
            string ret = "";
            for (int i = 0; i < Lines.Count; i++)
            {
                if (i == 0) ret = Lines[i].getText();
                else ret += Environment.NewLine + Lines[i].getText();
            }
            return ret;
        }

        public void AddText(string stext, Font sfont)
        {
            TempLine.AddTextParts(new CTextpart(stext, sfont));
        }

        public List<StoryLine> GetLines()
        {
            return Lines;
        }

        public StoryLine GetLine(int i)
        {
            return Lines[i];
        }

        internal void AddLineEnd()
        {
            Lines.Add(TempLine);
            TempLine = new StoryLine();
        }

        public int Length
        {
            get
            {
                return this.getText().Length;
            }
        }

        internal Story SubstoryLeft(string p)
        {
            Story ret = new Story();
            foreach (StoryLine l in Lines)
            {
                StoryLine TempLine = new StoryLine();
                foreach (CTextpart tp in l.TextParts)
                {
                    int index = tp.text.IndexOf(p);
                    if (index>=0)
                    {
                        // aggiungo parte prima del separtore
                        if (index > 0) TempLine.AddTextParts(new CTextpart(tp.text.Substring(0,index), tp.font));

                        if (TempLine.TextParts.Count > 0) ret.AddLine(TempLine);
                        return ret;
                    }
                    else TempLine.AddTextParts(tp);
                }
                ret.AddLine(TempLine);
            }
            return ret;
        }

        private void AddLine(StoryLine TempLine)
        {
            Lines.Add(TempLine);
        }

        internal void Addstory(Story story)
        {
            foreach (StoryLine l in story.Lines)
            {
                Lines.Add(l);
            }
        }

        internal Story SubstoryRight(string p)
        {
            bool Found=false;
            Story ret = new Story();
            foreach (StoryLine l in Lines)
            {
                if (!Found)
                {
                    StoryLine TempLine = new StoryLine();
                    foreach (CTextpart tp in l.TextParts)
                    {
                        if (!Found)
                        {

                            int index = tp.text.IndexOf(p);
                            if (index >= 0)
                            {
                                Found = true;
                                TempLine.AddTextParts(new CTextpart(tp.text.Substring(index), tp.font));
                            }
                        }
                        else TempLine.AddTextParts(tp);
                    }
                    ret.AddLine(TempLine);
                }
                else ret.AddLine(l);
            }
            return ret;
        }

        internal float FontSize()
        {
            float maxFS = 0;
            foreach (CTextpart tp in this.Lines[0].TextParts)
            {
                if (tp.font.Size > maxFS) maxFS = tp.font.Size;
            }
            return maxFS;
        }

        internal void AppendText(Story l)
        {
            this.GetLine(this.Lines.Count-1).AppendTextParts(
                                                                this.GetLine(this.Lines.Count-1).TextParts.Count-1,
                                                                l.getText());
        }
    }
}
