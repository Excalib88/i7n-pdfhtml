using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.Jsoup;
using Org.Jsoup.Integration;
using Org.Jsoup.Nodes;
using iText.IO.Util;

namespace Org.Jsoup.Parser {
    /// <summary>Tests XmlTreeBuilder.</summary>
    /// <author>Jonathan Hedley</author>
    public class XmlTreeBuilderTest {
        [NUnit.Framework.Test]
        public virtual void TestSimpleXmlParse() {
            String xml = "<doc id=2 href='/bar'>Foo <br /><link>One</link><link>Two</link></doc>";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<doc id=\"2\" href=\"/bar\">Foo <br /><link>One</link><link>Two</link></doc>"
                , TextUtil.StripNewlines(doc.Html()));
            NUnit.Framework.Assert.AreEqual(doc.GetElementById("2").AbsUrl("href"), "http://foo.com/bar");
        }

        [NUnit.Framework.Test]
        public virtual void TestPopToClose() {
            // test: </val> closes Two, </bar> ignored
            String xml = "<doc><val>One<val>Two</val></bar>Three</doc>";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<doc><val>One<val>Two</val>Three</val></doc>", TextUtil.StripNewlines(doc
                .Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestCommentAndDocType() {
            String xml = "<!DOCTYPE html><!-- a comment -->One <qux />Two";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html><!-- a comment -->One <qux />Two", TextUtil.StripNewlines(
                doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestSupplyParserToJsoupClass() {
            String xml = "<doc><val>One<val>Two</val></bar>Three</doc>";
            Document doc = Org.Jsoup.Jsoup.Parse(xml, "http://foo.com/", Org.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.AreEqual("<doc><val>One<val>Two</val>Three</val></doc>", TextUtil.StripNewlines(doc
                .Html()));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Java.Net.URISyntaxException"/>
        [NUnit.Framework.Test]
        public virtual void TestSupplyParserToDataStream() {
            FileInfo xmlFile = PortTestUtil.GetFile("/htmltests/xml-test.xml");
            Stream inStream = new FileStream(xmlFile.FullName, FileMode.Open, FileAccess.Read);
            Document doc = Org.Jsoup.Jsoup.Parse(inStream, null, "http://foo.com", Org.Jsoup.Parser.Parser.XmlParser()
                );
            NUnit.Framework.Assert.AreEqual("<doc><val>One<val>Two</val>Three</val></doc>", TextUtil.StripNewlines(doc
                .Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestDoesNotForceSelfClosingKnownTags() {
            // html will force "<br>one</br>" to logically "<br />One<br />". XML should be stay "<br>one</br> -- don't recognise tag.
            Document htmlDoc = Org.Jsoup.Jsoup.Parse("<br>one</br>");
            NUnit.Framework.Assert.AreEqual("<br>one\n<br>", htmlDoc.Body().Html());
            Document xmlDoc = Org.Jsoup.Jsoup.Parse("<br>one</br>", "", Org.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.AreEqual("<br>one</br>", xmlDoc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesXmlDeclarationAsDeclaration() {
            String html = "<?xml encoding='UTF-8' ?><body>One</body><!-- comment -->";
            Document doc = Org.Jsoup.Jsoup.Parse(html, "", Org.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.AreEqual("<?xml encoding=\"UTF-8\"?> <body> One </body> <!-- comment -->", Org.Jsoup.Helper.StringUtil
                .NormaliseWhitespace(doc.OuterHtml()));
            NUnit.Framework.Assert.AreEqual("#declaration", doc.ChildNode(0).NodeName());
            NUnit.Framework.Assert.AreEqual("#comment", doc.ChildNode(2).NodeName());
        }

        [NUnit.Framework.Test]
        public virtual void XmlFragment() {
            String xml = "<one src='/foo/' />Two<three><four /></three>";
            IList<Node> nodes = Org.Jsoup.Parser.Parser.ParseXmlFragment(xml, "http://example.com/");
            NUnit.Framework.Assert.AreEqual(3, nodes.Count);
            NUnit.Framework.Assert.AreEqual("http://example.com/foo/", nodes[0].AbsUrl("src"));
            NUnit.Framework.Assert.AreEqual("one", nodes[0].NodeName());
            NUnit.Framework.Assert.AreEqual("Two", ((TextNode)nodes[1]).Text());
        }

        [NUnit.Framework.Test]
        public virtual void XmlParseDefaultsToHtmlOutputSyntax() {
            Document doc = Org.Jsoup.Jsoup.Parse("x", "", Org.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.AreEqual(Syntax.xml, doc.OutputSettings().Syntax());
        }

        [NUnit.Framework.Test]
        public virtual void TestDoesHandleEOFInTag() {
            String html = "<img src=asdf onerror=\"alert(1)\" x=";
            Document xmlDoc = Org.Jsoup.Jsoup.Parse(html, "", Org.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.AreEqual("<img src=\"asdf\" onerror=\"alert(1)\" x=\"\" />", xmlDoc.Html());
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Java.Net.URISyntaxException"/>
        [NUnit.Framework.Test]
        public virtual void TestDetectCharsetEncodingDeclaration() {
            FileInfo xmlFile = PortTestUtil.GetFile("/htmltests/xml-charset.xml");
            Stream inStream = new FileStream(xmlFile.FullName, FileMode.Open, FileAccess.Read);
            Document doc = Org.Jsoup.Jsoup.Parse(inStream, null, "http://example.com/", Org.Jsoup.Parser.Parser.XmlParser
                ());
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", doc.Charset().Name());
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?> <data>äöåéü</data>", TextUtil
                .StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestParseDeclarationAttributes() {
            String xml = "<?xml version='1' encoding='UTF-8' something='else'?><val>One</val>";
            Document doc = Org.Jsoup.Jsoup.Parse(xml, "", Org.Jsoup.Parser.Parser.XmlParser());
            XmlDeclaration decl = (XmlDeclaration)doc.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("1", decl.Attr("version"));
            NUnit.Framework.Assert.AreEqual("UTF-8", decl.Attr("encoding"));
            NUnit.Framework.Assert.AreEqual("else", decl.Attr("something"));
            NUnit.Framework.Assert.AreEqual("version=\"1\" encoding=\"UTF-8\" something=\"else\"", decl.GetWholeDeclaration
                ());
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1\" encoding=\"UTF-8\" something=\"else\"?>", decl.OuterHtml
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreatesValidProlog() {
            Document document = Document.CreateShell("");
            document.OutputSettings().Syntax(Syntax.xml);
            document.Charset(EncodingUtil.GetEncoding("utf-8"));
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<html>\n" + " <head></head>\n"
                 + " <body></body>\n" + "</html>", document.OuterHtml());
        }
    }
}
