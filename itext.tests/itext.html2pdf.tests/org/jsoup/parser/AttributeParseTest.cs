using System;
using System.Collections.Generic;
using Org.Jsoup.Nodes;
using Org.Jsoup.Select;

namespace Org.Jsoup.Parser {
    /// <summary>Test suite for attribute parser.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class AttributeParseTest {
        [NUnit.Framework.Test]
        public virtual void ParsesRoughAttributeString() {
            String html = "<a id=\"123\" class=\"baz = 'bar'\" style = 'border: 2px'qux zim foo = 12 mux=18 />";
            // should be: <id=123>, <class=baz = 'bar'>, <qux=>, <zim=>, <foo=12>, <mux.=18>
            Element el = Org.Jsoup.Jsoup.Parse(html).GetElementsByTag("a")[0];
            Attributes attr = el.Attributes();
            NUnit.Framework.Assert.AreEqual(7, attr.Size());
            NUnit.Framework.Assert.AreEqual("123", attr.Get("id"));
            NUnit.Framework.Assert.AreEqual("baz = 'bar'", attr.Get("class"));
            NUnit.Framework.Assert.AreEqual("border: 2px", attr.Get("style"));
            NUnit.Framework.Assert.AreEqual("", attr.Get("qux"));
            NUnit.Framework.Assert.AreEqual("", attr.Get("zim"));
            NUnit.Framework.Assert.AreEqual("12", attr.Get("foo"));
            NUnit.Framework.Assert.AreEqual("18", attr.Get("mux"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesNewLinesAndReturns() {
            String html = "<a\r\nfoo='bar\r\nqux'\r\nbar\r\n=\r\ntwo>One</a>";
            Element el = Org.Jsoup.Jsoup.Parse(html).Select("a").First();
            NUnit.Framework.Assert.AreEqual(2, el.Attributes().Size());
            NUnit.Framework.Assert.AreEqual("bar\r\nqux", el.Attr("foo"));
            // currently preserves newlines in quoted attributes. todo confirm if should.
            NUnit.Framework.Assert.AreEqual("two", el.Attr("bar"));
        }

        [NUnit.Framework.Test]
        public virtual void ParsesEmptyString() {
            String html = "<a />";
            Element el = Org.Jsoup.Jsoup.Parse(html).GetElementsByTag("a")[0];
            Attributes attr = el.Attributes();
            NUnit.Framework.Assert.AreEqual(0, attr.Size());
        }

        [NUnit.Framework.Test]
        public virtual void CanStartWithEq() {
            String html = "<a =empty />";
            Element el = Org.Jsoup.Jsoup.Parse(html).GetElementsByTag("a")[0];
            Attributes attr = el.Attributes();
            NUnit.Framework.Assert.AreEqual(1, attr.Size());
            NUnit.Framework.Assert.IsTrue(attr.HasKey("=empty"));
            NUnit.Framework.Assert.AreEqual("", attr.Get("=empty"));
        }

        [NUnit.Framework.Test]
        public virtual void StrictAttributeUnescapes() {
            String html = "<a id=1 href='?foo=bar&mid&lt=true'>One</a> <a id=2 href='?foo=bar&lt;qux&lg=1'>Two</a>";
            Elements els = Org.Jsoup.Jsoup.Parse(html).Select("a");
            NUnit.Framework.Assert.AreEqual("?foo=bar&mid&lt=true", els.First().Attr("href"));
            NUnit.Framework.Assert.AreEqual("?foo=bar<qux&lg=1", els.Last().Attr("href"));
        }

        [NUnit.Framework.Test]
        public virtual void MoreAttributeUnescapes() {
            String html = "<a href='&wr_id=123&mid-size=true&ok=&wr'>Check</a>";
            Elements els = Org.Jsoup.Jsoup.Parse(html).Select("a");
            NUnit.Framework.Assert.AreEqual("&wr_id=123&mid-size=true&ok=&wr", els.First().Attr("href"));
        }

        [NUnit.Framework.Test]
        public virtual void ParsesBooleanAttributes() {
            String html = "<a normal=\"123\" boolean empty=\"\"></a>";
            Element el = Org.Jsoup.Jsoup.Parse(html).Select("a").First();
            NUnit.Framework.Assert.AreEqual("123", el.Attr("normal"));
            NUnit.Framework.Assert.AreEqual("", el.Attr("boolean"));
            NUnit.Framework.Assert.AreEqual("", el.Attr("empty"));
            IList<Org.Jsoup.Nodes.Attribute> attributes = el.Attributes().AsList();
            NUnit.Framework.Assert.AreEqual(3, attributes.Count, "There should be 3 attribute present");
            // Assuming the list order always follows the parsed html
            NUnit.Framework.Assert.IsFalse(attributes[0] is BooleanAttribute, "'normal' attribute should not be boolean"
                );
            NUnit.Framework.Assert.IsTrue(attributes[1] is BooleanAttribute, "'boolean' attribute should be boolean");
            NUnit.Framework.Assert.IsFalse(attributes[2] is BooleanAttribute, "'empty' attribute should not be boolean"
                );
            NUnit.Framework.Assert.AreEqual(html, el.OuterHtml());
        }
    }
}
