using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SAE
{
    class FontID
    {
            public string charStyle;
            public string pointSize;
            public string fontStyle;
            public string basedOn="";
            const string DefPointSize="10";
            const string DefFontStyle = "Regular";

            public FontID(string charstyle, string pointsize, string fontstyle)
            {
                this.charStyle = charstyle;
                this.pointSize = pointsize;
                this.fontStyle = fontstyle;
            }
            public FontID(string charstyle, string pointsize, string fontstyle,string basedOn)
            {
                this.charStyle = charstyle;
                this.pointSize = pointsize;
                this.fontStyle = fontstyle;
                this.basedOn = basedOn;
            }

            public FontID()
            {
            }
            
        public FontID clone()
        {
            return new FontID(charStyle,pointSize,fontStyle,basedOn);
        }

        public string GetFont(Dictionary<string, FontID> dictFont, ref Font retFont)
        {
            FontStyle fs = FontStyle.Regular;
            string problems = "";
            //se non ho style prendo quello del font su cui è basato
            if (fontStyle.Length == 0 && basedOn.Length>0) fontStyle = dictFont[basedOn].fontStyle;

            string[] Strfontstyles = fontStyle.Split(' ');
            for (int i = 0; i < Strfontstyles.Length; i++)
            {
                switch (Strfontstyles[i].ToLower())
                {
                    case "heavy":
                    case "medium":
                    case "bold":
                    case "black": fs = fs | FontStyle.Bold; break;
                    case "italic":
                    case "oblique": fs = fs | FontStyle.Italic; break;
                }
            }

            //se non ho style prendo quello del font su cui è basato
            if (pointSize.Length == 0)
            {
                try
                {
                    pointSize = dictFont[basedOn].pointSize;
                }
                catch (Exception)
                {
                    problems += "manca point size";
                    pointSize = DefPointSize;
                }
            }

            retFont = new Font(FontFamily.GenericSansSerif, Convert.ToSingle(pointSize), fs);

            return problems; 
        }
    }
}
