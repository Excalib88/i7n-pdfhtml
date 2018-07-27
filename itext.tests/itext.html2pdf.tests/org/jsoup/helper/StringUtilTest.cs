/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.IO.Util;

namespace Org.Jsoup.Helper {
    public class StringUtilTest {
        [NUnit.Framework.Test]
        public virtual void Join() {
            NUnit.Framework.Assert.AreEqual("", Org.Jsoup.Helper.StringUtil.Join(JavaUtil.ArraysAsList(""), " "));
            NUnit.Framework.Assert.AreEqual("one", Org.Jsoup.Helper.StringUtil.Join(JavaUtil.ArraysAsList("one"), " ")
                );
            NUnit.Framework.Assert.AreEqual("one two three", Org.Jsoup.Helper.StringUtil.Join(JavaUtil.ArraysAsList("one"
                , "two", "three"), " "));
        }

        [NUnit.Framework.Test]
        public virtual void Padding() {
            NUnit.Framework.Assert.AreEqual("", Org.Jsoup.Helper.StringUtil.Padding(0));
            NUnit.Framework.Assert.AreEqual(" ", Org.Jsoup.Helper.StringUtil.Padding(1));
            NUnit.Framework.Assert.AreEqual("  ", Org.Jsoup.Helper.StringUtil.Padding(2));
            NUnit.Framework.Assert.AreEqual("               ", Org.Jsoup.Helper.StringUtil.Padding(15));
        }

        [NUnit.Framework.Test]
        public virtual void IsBlank() {
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsBlank(null));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsBlank(""));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsBlank("      "));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsBlank("   \r\n  "));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsBlank("hello"));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsBlank("   hello   "));
        }

        [NUnit.Framework.Test]
        public virtual void IsNumeric() {
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsNumeric(null));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsNumeric(" "));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsNumeric("123 546"));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsNumeric("hello"));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsNumeric("123.334"));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsNumeric("1"));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsNumeric("1234"));
        }

        [NUnit.Framework.Test]
        public virtual void IsWhitespace() {
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsWhitespace('\t'));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsWhitespace('\n'));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsWhitespace('\r'));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsWhitespace('\f'));
            NUnit.Framework.Assert.IsTrue(Org.Jsoup.Helper.StringUtil.IsWhitespace(' '));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsWhitespace('\u00a0'));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsWhitespace('\u2000'));
            NUnit.Framework.Assert.IsFalse(Org.Jsoup.Helper.StringUtil.IsWhitespace('\u3000'));
        }

        [NUnit.Framework.Test]
        public virtual void NormaliseWhiteSpace() {
            NUnit.Framework.Assert.AreEqual(" ", Org.Jsoup.Helper.StringUtil.NormaliseWhitespace("    \r \n \r\n"));
            NUnit.Framework.Assert.AreEqual(" hello there ", Org.Jsoup.Helper.StringUtil.NormaliseWhitespace("   hello   \r \n  there    \n"
                ));
            NUnit.Framework.Assert.AreEqual("hello", Org.Jsoup.Helper.StringUtil.NormaliseWhitespace("hello"));
            NUnit.Framework.Assert.AreEqual("hello there", Org.Jsoup.Helper.StringUtil.NormaliseWhitespace("hello\nthere"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NormaliseWhiteSpaceHandlesHighSurrogates() {
            String test71540chars = "\ud869\udeb2\u304b\u309a  1";
            String test71540charsExpectedSingleWhitespace = "\ud869\udeb2\u304b\u309a 1";
            NUnit.Framework.Assert.AreEqual(test71540charsExpectedSingleWhitespace, Org.Jsoup.Helper.StringUtil.NormaliseWhitespace
                (test71540chars));
            String extractedText = Org.Jsoup.Jsoup.Parse(test71540chars).Text();
            NUnit.Framework.Assert.AreEqual(test71540charsExpectedSingleWhitespace, extractedText);
        }

        [NUnit.Framework.Test]
        public virtual void ResolvesRelativeUrls() {
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two?three", Org.Jsoup.Helper.StringUtil.Resolve("http://example.com"
                , "./one/two?three"));
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two?three", Org.Jsoup.Helper.StringUtil.Resolve("http://example.com?one"
                , "./one/two?three"));
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two?three#four", Org.Jsoup.Helper.StringUtil.Resolve
                ("http://example.com", "./one/two?three#four"));
            NUnit.Framework.Assert.AreEqual("https://example.com/one", Org.Jsoup.Helper.StringUtil.Resolve("http://example.com/"
                , "https://example.com/one"));
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two.html", Org.Jsoup.Helper.StringUtil.Resolve("http://example.com/two/"
                , "../one/two.html"));
            NUnit.Framework.Assert.AreEqual("https://example2.com/one", Org.Jsoup.Helper.StringUtil.Resolve("https://example.com/"
                , "//example2.com/one"));
            NUnit.Framework.Assert.AreEqual("https://example.com:8080/one", Org.Jsoup.Helper.StringUtil.Resolve("https://example.com:8080"
                , "./one"));
            NUnit.Framework.Assert.AreEqual("https://example2.com/one", Org.Jsoup.Helper.StringUtil.Resolve("http://example.com/"
                , "https://example2.com/one"));
            NUnit.Framework.Assert.AreEqual("https://example.com/one", Org.Jsoup.Helper.StringUtil.Resolve("wrong", "https://example.com/one"
                ));
            NUnit.Framework.Assert.AreEqual("https://example.com/one", Org.Jsoup.Helper.StringUtil.Resolve("https://example.com/one"
                , ""));
            NUnit.Framework.Assert.AreEqual("", Org.Jsoup.Helper.StringUtil.Resolve("wrong", "also wrong"));
            NUnit.Framework.Assert.AreEqual("ftp://example.com/one", Org.Jsoup.Helper.StringUtil.Resolve("ftp://example.com/two/"
                , "../one"));
            NUnit.Framework.Assert.AreEqual("ftp://example.com/one/two.c", Org.Jsoup.Helper.StringUtil.Resolve("ftp://example.com/one/"
                , "./two.c"));
            NUnit.Framework.Assert.AreEqual("ftp://example.com/one/two.c", Org.Jsoup.Helper.StringUtil.Resolve("ftp://example.com/one/"
                , "two.c"));
        }
    }
}
