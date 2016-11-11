using System;
using System.IO;
using System.Text;
using Org.Jsoup.Nodes;

namespace Org.Jsoup.Helper {
    public class DataUtilTest {
        [NUnit.Framework.Test]
        public virtual void TestCharset() {
            NUnit.Framework.Assert.AreEqual("utf-8", DataUtil.GetCharsetFromContentType("text/html;charset=utf-8 "));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html; charset=UTF-8"));
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=ISO-8859-1"
                ));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html"));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType(null));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html;charset=Unknown"));
        }

        [NUnit.Framework.Test]
        public virtual void TestQuotedCharset() {
            NUnit.Framework.Assert.AreEqual("utf-8", DataUtil.GetCharsetFromContentType("text/html; charset=\"utf-8\""
                ));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html;charset=\"UTF-8\"")
                );
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=\"ISO-8859-1\""
                ));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset=\"Unsupported\""
                ));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html; charset='UTF-8'"));
        }

        [NUnit.Framework.Test]
        public virtual void DiscardsSpuriousByteOrderMark() {
            String html = "\uFEFF<html><head><title>One</title></head><body>Two</body></html>";
            ByteBuffer buffer = System.Text.Encoding.GetEncoding("UTF-8").Encode(html);
            Document doc = DataUtil.ParseByteData(buffer, "UTF-8", "http://foo.com/", Org.Jsoup.Parser.Parser.HtmlParser
                ());
            NUnit.Framework.Assert.AreEqual("One", doc.Head().Text());
        }

        [NUnit.Framework.Test]
        public virtual void DiscardsSpuriousByteOrderMarkWhenNoCharsetSet() {
            String html = "\uFEFF<html><head><title>One</title></head><body>Two</body></html>";
            ByteBuffer buffer = System.Text.Encoding.GetEncoding("UTF-8").Encode(html);
            Document doc = DataUtil.ParseByteData(buffer, null, "http://foo.com/", Org.Jsoup.Parser.Parser.HtmlParser(
                ));
            NUnit.Framework.Assert.AreEqual("One", doc.Head().Text());
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().DisplayName());
        }

        [NUnit.Framework.Test]
        public virtual void ShouldNotThrowExceptionOnEmptyCharset() {
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset="));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset=;"));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldSelectFirstCharsetOnWeirdMultileCharsetsInMetaTags() {
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=ISO-8859-1, charset=1251"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldCorrectCharsetForDuplicateCharsetString() {
            NUnit.Framework.Assert.AreEqual("iso-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=charset=iso-8859-1"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldReturnNullForIllegalCharsetNames() {
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset=$HJKDF§$/("));
        }

        [NUnit.Framework.Test]
        public virtual void GeneratesMimeBoundaries() {
            String m1 = DataUtil.MimeBoundary();
            String m2 = DataUtil.MimeBoundary();
            NUnit.Framework.Assert.AreEqual(DataUtil.boundaryLength, m1.Length);
            NUnit.Framework.Assert.AreEqual(DataUtil.boundaryLength, m2.Length);
            NUnit.Framework.Assert.AreNotSame(m1, m2);
        }

        [NUnit.Framework.Test]
        public virtual void WrongMetaCharsetFallback() {
            try {
                byte[] input = "<html><head><meta charset=iso-8></head><body></body></html>".GetBytes("UTF-8");
                ByteBuffer inBuffer = ByteBuffer.Wrap(input);
                Document doc = DataUtil.ParseByteData(inBuffer, null, "http://example.com", Org.Jsoup.Parser.Parser.HtmlParser
                    ());
                String expected = "<html>\n" + " <head>\n" + "  <meta charset=\"iso-8\">\n" + " </head>\n" + " <body></body>\n"
                     + "</html>";
                NUnit.Framework.Assert.AreEqual(expected, doc.ToString());
            }
            catch (ArgumentException ex) {
                NUnit.Framework.Assert.Fail(ex.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void SupportsBOMinFiles() {
            // test files from http://www.i18nl10n.com/korean/utftest/
            FileInfo @in = Org.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf16be.html");
            Document doc = Org.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-16BE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
            @in = Org.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf16le.html");
            doc = Org.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-16LE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
            @in = Org.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf32be.html");
            doc = Org.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-32BE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
            @in = Org.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf32le.html");
            doc = Org.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-32LE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
        }
    }
}
