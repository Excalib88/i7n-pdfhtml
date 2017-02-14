using System;
using System.Collections.Generic;
using Org.Jsoup.Helper;

namespace Org.Jsoup.Nodes {
    /// <summary>Tests for FormElement</summary>
    /// <author>Jonathan Hedley</author>
    public class FormElementTest {
        [NUnit.Framework.Test]
        public virtual void HasAssociatedControls() {
            //"button", "fieldset", "input", "keygen", "object", "output", "select", "textarea"
            String html = "<form id=1><button id=1><fieldset id=2 /><input id=3><keygen id=4><object id=5><output id=6>"
                 + "<select id=7><option></select><textarea id=8><p id=9>";
            Document doc = Org.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.Select("form").First();
            NUnit.Framework.Assert.AreEqual(8, form.Elements().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CreatesFormData() {
            String html = "<form><input name='one' value='two'><select name='three'><option value='not'>" + "<option value='four' selected><option value='five' selected><textarea name=six>seven</textarea>"
                 + "<input name='seven' type='radio' value='on' checked><input name='seven' type='radio' value='off'>"
                 + "<input name='eight' type='checkbox' checked><input name='nine' type='checkbox' value='unset'>" + "<input name='ten' value='text' disabled>"
                 + "</form>";
            Document doc = Org.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.Select("form").First();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual(6, data.Count);
            NUnit.Framework.Assert.AreEqual("one=two", data[0].ToString());
            NUnit.Framework.Assert.AreEqual("three=four", data[1].ToString());
            NUnit.Framework.Assert.AreEqual("three=five", data[2].ToString());
            NUnit.Framework.Assert.AreEqual("six=seven", data[3].ToString());
            NUnit.Framework.Assert.AreEqual("seven=on", data[4].ToString());
            // set
            NUnit.Framework.Assert.AreEqual("eight=on", data[5].ToString());
        }

        // default
        // nine should not appear, not checked checkbox
        // ten should not appear, disabled
        //    @Test public void createsSubmitableConnection() {
        //        String html = "<form action='/search'><input name='q'></form>";
        //        Document doc = Jsoup.parse(html, "http://example.com/");
        //        doc.select("[name=q]").attr("value", "jsoup");
        //
        //        FormElement form = ((FormElement) doc.select("form").first());
        //        Connection con = form.submit();
        //
        //        assertEquals(Connection.Method.GET, con.request().method());
        //        assertEquals("http://example.com/search", con.request().url().toExternalForm());
        //        List<Connection.KeyVal> dataList = (List<Connection.KeyVal>) con.request().data();
        //        assertEquals("q=jsoup", dataList.get(0).toString());
        //
        //        doc.select("form").attr("method", "post");
        //        Connection con2 = form.submit();
        //        assertEquals(Connection.Method.POST, con2.request().method());
        //    }
        //
        //    @Test public void actionWithNoValue() {
        //        String html = "<form><input name='q'></form>";
        //        Document doc = Jsoup.parse(html, "http://example.com/");
        //        FormElement form = ((FormElement) doc.select("form").first());
        //        Connection con = form.submit();
        //
        //        assertEquals("http://example.com/", con.request().url().toExternalForm());
        //    }
        //
        //    @Test public void actionWithNoBaseUri() {
        //        String html = "<form><input name='q'></form>";
        //        Document doc = Jsoup.parse(html);
        //        FormElement form = ((FormElement) doc.select("form").first());
        //
        //
        //        boolean threw = false;
        //        try {
        //            Connection con = form.submit();
        //        } catch (IllegalArgumentException e) {
        //            threw = true;
        //            assertEquals("Could not determine a form action URL for submit. Ensure you set a base URI when parsing.",
        //                    e.getMessage());
        //        }
        //        assertTrue(threw);
        //    }
        [NUnit.Framework.Test]
        public virtual void FormsAddedAfterParseAreFormElements() {
            Document doc = Org.Jsoup.Jsoup.Parse("<body />");
            doc.Body().Html("<form action='http://example.com/search'><input name='q' value='search'>");
            Element formEl = doc.Select("form").First();
            NUnit.Framework.Assert.IsTrue(formEl is FormElement);
            FormElement form = (FormElement)formEl;
            NUnit.Framework.Assert.AreEqual(1, form.Elements().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ControlsAddedAfterParseAreLinkedWithForms() {
            Document doc = Org.Jsoup.Jsoup.Parse("<body />");
            doc.Body().Html("<form />");
            Element formEl = doc.Select("form").First();
            formEl.Append("<input name=foo value=bar>");
            NUnit.Framework.Assert.IsTrue(formEl is FormElement);
            FormElement form = (FormElement)formEl;
            NUnit.Framework.Assert.AreEqual(1, form.Elements().Count);
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual("foo=bar", data[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void UsesOnForCheckboxValueIfNoValueSet() {
            Document doc = Org.Jsoup.Jsoup.Parse("<form><input type=checkbox checked name=foo></form>");
            FormElement form = (FormElement)doc.Select("form").First();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual("on", data[0].Value());
            NUnit.Framework.Assert.AreEqual("foo", data[0].Key());
        }

        [NUnit.Framework.Test]
        public virtual void AdoptedFormsRetainInputs() {
            // test for https://github.com/jhy/jsoup/issues/249
            String html = "<html>\n" + "<body>  \n" + "  <table>\n" + "      <form action=\"/hello.php\" method=\"post\">\n"
                 + "      <tr><td>User:</td><td> <input type=\"text\" name=\"user\" /></td></tr>\n" + "      <tr><td>Password:</td><td> <input type=\"password\" name=\"pass\" /></td></tr>\n"
                 + "      <tr><td><input type=\"submit\" name=\"login\" value=\"login\" /></td></tr>\n" + "   </form>\n"
                 + "  </table>\n" + "</body>\n" + "</html>";
            Document doc = Org.Jsoup.Jsoup.Parse(html);
            FormElement form = (FormElement)doc.Select("form").First();
            IList<KeyVal> data = form.FormData();
            NUnit.Framework.Assert.AreEqual(3, data.Count);
            NUnit.Framework.Assert.AreEqual("user", data[0].Key());
            NUnit.Framework.Assert.AreEqual("pass", data[1].Key());
            NUnit.Framework.Assert.AreEqual("login", data[2].Key());
        }
    }
}
