using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PQCreator;
namespace PQCreatorTests
{
    [TestClass]
    public class PQCreatorTest
    {
        [TestMethod]
        public void TestGiornoTESTTXT()
        {
            string problems;
            ODTImporter odtI = new ODTImporter();
            odtI.OpenTXT(@"pqsingle.txt");
            problems = odtI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }

        [TestMethod]
        public void TestPQMAGGIU2013()
        {
            string problems;
            ODTImporter odtI = new ODTImporter();
            odtI.OpenTXT(@"pqmaggiu2013.txt");
            problems = odtI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }
        [TestMethod]
        public void TestPQGENFEB2013()
        {
            string problems;
            ODTImporter odtI = new ODTImporter();
            odtI.OpenTXT(@"pqgenfeb2013.txt");
            problems = odtI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }

        [TestMethod]
        public void TestPQMARAPR2013()
        {
            string problems;
            ODTImporter odtI = new ODTImporter();
            odtI.OpenTXT(@"pqmarapr2013.txt");
            problems = odtI.PutTag();
            Assert.IsTrue(problems.Length == 124);
        }

        [TestMethod]
        public void TestPQLUGAGO2013()
        {
            string problems;
            ODTImporter odtI = new ODTImporter();
            odtI.OpenTXT(@"pqlugago2013.txt");
            problems = odtI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }

        [TestMethod]
        public void TestPQSETOTT2013()
        {
            string problems;
            ODTImporter odtI = new ODTImporter();
            odtI.OpenTXT(@"pqsetott2013.txt");
            problems = odtI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }

        [TestMethod]
        public void TestPQSETOTT2013_IDML()
        {
            string problems;
            SAE.DecodeIDML.IDMLTempPath = @"c:\idml\";
            PQImporter PQI = new PQImporter();
            PQI.OpenIDML(@"pqsetott2013.idml");
            problems = PQI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }

        [TestMethod]
        public void NEWTestGiornoTESTIDML()
        {
            string problems;
            SAE.DecodeIDML.IDMLTempPath = @"c:\idml\";
            PQImporter PQI = new PQImporter();
            PQI.OpenIDML(@"pqsingle.idml");
            problems = PQI.PutTag();
            Assert.IsTrue(problems.Length == 0);
        }

    }
}
