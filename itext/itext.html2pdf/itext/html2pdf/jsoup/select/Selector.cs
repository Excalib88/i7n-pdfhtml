/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System.Collections.Generic;
using iText.Html2pdf.Jsoup.Helper;
using iText.IO.Util;

namespace iText.Html2pdf.Jsoup.Select {
    /// <summary>CSS-like element selector, that finds elements matching a query.</summary>
    /// <remarks>
    /// CSS-like element selector, that finds elements matching a query.
    /// <h2>Selector syntax</h2>
    /// <p>
    /// A selector is a chain of simple selectors, separated by combinators. Selectors are case insensitive (including against
    /// elements, attributes, and attribute values).
    /// </p>
    /// <p>
    /// The universal selector (*) is implicit when no element selector is supplied (i.e.
    /// <c>*.header</c>
    /// and
    /// <c>.header</c>
    /// is equivalent).
    /// </p>
    /// <table summary="">
    /// <tr><th align="left">Pattern</th><th align="left">Matches</th><th align="left">Example</th></tr>
    /// <tr><td><code>*</code></td><td>any element</td><td><code>*</code></td></tr>
    /// <tr><td><code>tag</code></td><td>elements with the given tag name</td><td><code>div</code></td></tr>
    /// <tr><td><code>ns|E</code></td><td>elements of type E in the namespace <i>ns</i></td><td><code>fb|name</code> finds <code>&lt;fb:name&gt;</code> elements</td></tr>
    /// <tr><td><code>#id</code></td><td>elements with attribute ID of "id"</td><td><code>div#wrap</code>, <code>#logo</code></td></tr>
    /// <tr><td><code>.class</code></td><td>elements with a class name of "class"</td><td><code>div.left</code>, <code>.result</code></td></tr>
    /// <tr><td><code>[attr]</code></td><td>elements with an attribute named "attr" (with any value)</td><td><code>a[href]</code>, <code>[title]</code></td></tr>
    /// <tr><td><code>[^attrPrefix]</code></td><td>elements with an attribute name starting with "attrPrefix". Use to find elements with HTML5 datasets</td><td><code>[^data-]</code>, <code>div[^data-]</code></td></tr>
    /// <tr><td><code>[attr=val]</code></td><td>elements with an attribute named "attr", and value equal to "val"</td><td><code>img[width=500]</code>, <code>a[rel=nofollow]</code></td></tr>
    /// <tr><td><code>[attr=&quot;val&quot;]</code></td><td>elements with an attribute named "attr", and value equal to "val"</td><td><code>span[hello="Cleveland"][goodbye="Columbus"]</code>, <code>a[rel=&quot;nofollow&quot;]</code></td></tr>
    /// <tr><td><code>[attr^=valPrefix]</code></td><td>elements with an attribute named "attr", and value starting with "valPrefix"</td><td><code>a[href^=http:]</code></td></tr>
    /// <tr><td><code>[attr$=valSuffix]</code></td><td>elements with an attribute named "attr", and value ending with "valSuffix"</td><td><code>img[src$=.png]</code></td></tr>
    /// <tr><td><code>[attr*=valContaining]</code></td><td>elements with an attribute named "attr", and value containing "valContaining"</td><td><code>a[href*=/search/]</code></td></tr>
    /// <tr><td><code>[attr~=<em>regex</em>]</code></td><td>elements with an attribute named "attr", and value matching the regular expression</td><td><code>img[src~=(?i)\\.(png|jpe?g)]</code></td></tr>
    /// <tr><td></td><td>The above may be combined in any order</td><td><code>div.header[title]</code></td></tr>
    /// <tr><td><td colspan="3"><h3>Combinators</h3></td></tr>
    /// <tr><td><code>E F</code></td><td>an F element descended from an E element</td><td><code>div a</code>, <code>.logo h1</code></td></tr>
    /// <tr><td><code>E
    /// <literal>&gt;</literal>
    /// F</code></td><td>an F direct child of E</td><td><code>ol
    /// <literal>&gt;</literal>
    /// li</code></td></tr>
    /// <tr><td><code>E + F</code></td><td>an F element immediately preceded by sibling E</td><td><code>li + li</code>, <code>div.head + div</code></td></tr>
    /// <tr><td><code>E ~ F</code></td><td>an F element preceded by sibling E</td><td><code>h1 ~ p</code></td></tr>
    /// <tr><td><code>E, F, G</code></td><td>all matching elements E, F, or G</td><td><code>a[href], div, h3</code></td></tr>
    /// <tr><td><td colspan="3"><h3>Pseudo selectors</h3></td></tr>
    /// <tr><td><code>:lt(<em>n</em>)</code></td><td>elements whose sibling index is less than <em>n</em></td><td><code>td:lt(3)</code> finds the first 3 cells of each row</td></tr>
    /// <tr><td><code>:gt(<em>n</em>)</code></td><td>elements whose sibling index is greater than <em>n</em></td><td><code>td:gt(1)</code> finds cells after skipping the first two</td></tr>
    /// <tr><td><code>:eq(<em>n</em>)</code></td><td>elements whose sibling index is equal to <em>n</em></td><td><code>td:eq(0)</code> finds the first cell of each row</td></tr>
    /// <tr><td><code>:has(<em>selector</em>)</code></td><td>elements that contains at least one element matching the <em>selector</em></td><td><code>div:has(p)</code> finds divs that contain p elements </td></tr>
    /// <tr><td><code>:not(<em>selector</em>)</code></td><td>elements that do not match the <em>selector</em>. See also
    /// <see cref="Elements.Not(System.String)"/>
    /// </td><td><code>div:not(.logo)</code> finds all divs that do not have the "logo" class.<p><code>div:not(:has(div))</code> finds divs that do not contain divs.</p></td></tr>
    /// <tr><td><code>:contains(<em>text</em>)</code></td><td>elements that contains the specified text. The search is case insensitive. The text may appear in the found element, or any of its descendants.</td><td><code>p:contains(jsoup)</code> finds p elements containing the text "jsoup".</td></tr>
    /// <tr><td><code>:matches(<em>regex</em>)</code></td><td>elements whose text matches the specified regular expression. The text may appear in the found element, or any of its descendants.</td><td><code>td:matches(\\d+)</code> finds table cells containing digits. <code>div:matches((?i)login)</code> finds divs containing the text, case insensitively.</td></tr>
    /// <tr><td><code>:containsOwn(<em>text</em>)</code></td><td>elements that directly contain the specified text. The search is case insensitive. The text must appear in the found element, not any of its descendants.</td><td><code>p:containsOwn(jsoup)</code> finds p elements with own text "jsoup".</td></tr>
    /// <tr><td><code>:matchesOwn(<em>regex</em>)</code></td><td>elements whose own text matches the specified regular expression. The text must appear in the found element, not any of its descendants.</td><td><code>td:matchesOwn(\\d+)</code> finds table cells directly containing digits. <code>div:matchesOwn((?i)login)</code> finds divs containing the text, case insensitively.</td></tr>
    /// <tr><td></td><td>The above may be combined in any order and with other selectors</td><td><code>.light:contains(name):eq(0)</code></td></tr>
    /// <tr><td colspan="3"><h3>Structural pseudo selectors</h3></td></tr>
    /// <tr><td><code>:root</code></td><td>The element that is the root of the document. In HTML, this is the <code>html</code> element</td><td><code>:root</code></td></tr>
    /// <tr><td><code>:nth-child(<em>a</em>n+<em>b</em>)</code></td><td><p>elements that have <code><em>a</em>n+<em>b</em>-1</code> siblings <b>before</b> it in the document tree, for any positive integer or zero value of <code>n</code>, and has a parent element. For values of <code>a</code> and <code>b</code> greater than zero, this effectively divides the element's children into groups of a elements (the last group taking the remainder), and selecting the <em>b</em>th element of each group. For example, this allows the selectors to address every other row in a table, and could be used to alternate the color of paragraph text in a cycle of four. The <code>a</code> and <code>b</code> values must be integers (positive, negative, or zero). The index of the first child of an element is 1.</p>
    /// In addition to this, <code>:nth-child()</code> can take <code>odd</code> and <code>even</code> as arguments instead. <code>odd</code> has the same signification as <code>2n+1</code>, and <code>even</code> has the same signification as <code>2n</code>.</td><td><code>tr:nth-child(2n+1)</code> finds every odd row of a table. <code>:nth-child(10n-1)</code> the 9th, 19th, 29th, etc, element. <code>li:nth-child(5)</code> the 5h li</td></tr>
    /// <tr><td><code>:nth-last-child(<em>a</em>n+<em>b</em>)</code></td><td>elements that have <code><em>a</em>n+<em>b</em>-1</code> siblings <b>after</b> it in the document tree. Otherwise like <code>:nth-child()</code></td><td><code>tr:nth-last-child(-n+2)</code> the last two rows of a table</td></tr>
    /// <tr><td><code>:nth-of-type(<em>a</em>n+<em>b</em>)</code></td><td>pseudo-class notation represents an element that has <code><em>a</em>n+<em>b</em>-1</code> siblings with the same expanded element name <em>before</em> it in the document tree, for any zero or positive integer value of n, and has a parent element</td><td><code>img:nth-of-type(2n+1)</code></td></tr>
    /// <tr><td><code>:nth-last-of-type(<em>a</em>n+<em>b</em>)</code></td><td>pseudo-class notation represents an element that has <code><em>a</em>n+<em>b</em>-1</code> siblings with the same expanded element name <em>after</em> it in the document tree, for any zero or positive integer value of n, and has a parent element</td><td><code>img:nth-last-of-type(2n+1)</code></td></tr>
    /// <tr><td><code>:first-child</code></td><td>elements that are the first child of some other element.</td><td><code>div
    /// <literal>&gt;</literal>
    /// p:first-child</code></td></tr>
    /// <tr><td><code>:last-child</code></td><td>elements that are the last child of some other element.</td><td><code>ol
    /// <literal>&gt;</literal>
    /// li:last-child</code></td></tr>
    /// <tr><td><code>:first-of-type</code></td><td>elements that are the first sibling of its type in the list of children of its parent element</td><td><code>dl dt:first-of-type</code></td></tr>
    /// <tr><td><code>:last-of-type</code></td><td>elements that are the last sibling of its type in the list of children of its parent element</td><td><code>tr
    /// <literal>&gt;</literal>
    /// td:last-of-type</code></td></tr>
    /// <tr><td><code>:only-child</code></td><td>elements that have a parent element and whose parent element hasve no other element children</td><td></td></tr>
    /// <tr><td><code>:only-of-type</code></td><td> an element that has a parent element and whose parent element has no other element children with the same expanded element name</td><td></td></tr>
    /// <tr><td><code>:empty</code></td><td>elements that have no children at all</td><td></td></tr>
    /// </table>
    /// </remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    /// <seealso cref="iText.Html2pdf.Jsoup.Nodes.Element.Select(System.String)"/>
    public class Selector {
        private readonly Evaluator evaluator;

        private readonly iText.Html2pdf.Jsoup.Nodes.Element root;

        private Selector(String query, iText.Html2pdf.Jsoup.Nodes.Element root) {
            Validate.NotNull(query);
            query = query.Trim();
            Validate.NotEmpty(query);
            Validate.NotNull(root);
            this.evaluator = QueryParser.Parse(query);
            this.root = root;
        }

        private Selector(Evaluator evaluator, iText.Html2pdf.Jsoup.Nodes.Element root) {
            Validate.NotNull(evaluator);
            Validate.NotNull(root);
            this.evaluator = evaluator;
            this.root = root;
        }

        /// <summary>Find elements matching selector.</summary>
        /// <param name="query">CSS selector</param>
        /// <param name="root">root element to descend into</param>
        /// <returns>matching elements, empty if none</returns>
        /// <exception cref="SelectorParseException">(unchecked) on an invalid CSS query.</exception>
        public static Elements Select(String query, iText.Html2pdf.Jsoup.Nodes.Element root) {
            return new iText.Html2pdf.Jsoup.Select.Selector(query, root).Select();
        }

        /// <summary>Find elements matching selector.</summary>
        /// <param name="evaluator">CSS selector</param>
        /// <param name="root">root element to descend into</param>
        /// <returns>matching elements, empty if none</returns>
        public static Elements Select(Evaluator evaluator, iText.Html2pdf.Jsoup.Nodes.Element root) {
            return new iText.Html2pdf.Jsoup.Select.Selector(evaluator, root).Select();
        }

        /// <summary>Find elements matching selector.</summary>
        /// <param name="query">CSS selector</param>
        /// <param name="roots">root elements to descend into</param>
        /// <returns>matching elements, empty if none</returns>
        public static Elements Select(String query, IEnumerable<iText.Html2pdf.Jsoup.Nodes.Element> roots) {
            Validate.NotEmpty(query);
            Validate.NotNull(roots);
            Evaluator evaluator = QueryParser.Parse(query);
            List<iText.Html2pdf.Jsoup.Nodes.Element> elements = new List<iText.Html2pdf.Jsoup.Nodes.Element>();
            IdentityDictionary<iText.Html2pdf.Jsoup.Nodes.Element, bool?> seenElements = new IdentityDictionary<iText.Html2pdf.Jsoup.Nodes.Element
                , bool?>();
            // dedupe elements by identity, not equality
            foreach (iText.Html2pdf.Jsoup.Nodes.Element root in roots) {
                Elements found = Select(evaluator, root);
                foreach (iText.Html2pdf.Jsoup.Nodes.Element el in found) {
                    if (!seenElements.ContainsKey(el)) {
                        elements.Add(el);
                        seenElements.Put(el, true);
                    }
                }
            }
            return new Elements(elements);
        }

        private Elements Select() {
            return Collector.Collect(evaluator, root);
        }

        // exclude set. package open so that Elements can implement .not() selector.
        internal static Elements FilterOut(ICollection<iText.Html2pdf.Jsoup.Nodes.Element> elements, ICollection<iText.Html2pdf.Jsoup.Nodes.Element
            > outs) {
            Elements output = new Elements();
            foreach (iText.Html2pdf.Jsoup.Nodes.Element el in elements) {
                bool found = false;
                foreach (iText.Html2pdf.Jsoup.Nodes.Element @out in outs) {
                    if (el.Equals(@out)) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    output.Add(el);
                }
            }
            return output;
        }

        public class SelectorParseException : InvalidOperationException {
            public SelectorParseException(String msg, params Object[] @params)
                : base(MessageFormatUtil.Format(msg, @params)) {
            }
        }
    }
}
