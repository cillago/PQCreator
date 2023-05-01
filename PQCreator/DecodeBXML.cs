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
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SAE
{
    public static class DecodeBXML
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


        public static void LoadBXMLFile(string FilePath)
        {

            problems = "";
            XDocument bxml = XDocument.Load(FilePath);

            foreach (XElement element in bxml.Descendants("para"))
            {

            }



            if (problems.Length > 0) Log.WriteLog(problems);


        }

    }
}
