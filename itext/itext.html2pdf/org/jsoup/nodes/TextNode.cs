using System;
using System.Text;
using Org.Jsoup.Helper;

namespace Org.Jsoup.Nodes {
    /// <summary>A text node.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class TextNode : Node {
        private const String TEXT_KEY = "text";

        internal String text;

        /// <summary>Create a new TextNode representing the supplied (unencoded) text).</summary>
        /// <param name="text">raw text</param>
        /// <param name="baseUri">base uri</param>
        /// <seealso cref="CreateFromEncoded(System.String, System.String)"/>
        public TextNode(String text, String baseUri) {
            /*
            TextNode is a node, and so by default comes with attributes and children. The attributes are seldom used, but use
            memory, and the child nodes are never used. So we don't have them, and override accessors to attributes to create
            them as needed on the fly.
            */
            this.baseUri = baseUri;
            this.text = text;
        }

        public override String NodeName() {
            return "#text";
        }

        /// <summary>Get the text content of this text node.</summary>
        /// <returns>Unencoded, normalised text.</returns>
        /// <seealso cref="GetWholeText()"/>
        public virtual String Text() {
            return NormaliseWhitespace(GetWholeText());
        }

        /// <summary>Set the text content of this text node.</summary>
        /// <param name="text">unencoded text</param>
        /// <returns>this, for chaining</returns>
        public virtual Org.Jsoup.Nodes.TextNode Text(String text) {
            this.text = text;
            if (attributes != null) {
                attributes.Put(TEXT_KEY, text);
            }
            return this;
        }

        /// <summary>Get the (unencoded) text of this text node, including any newlines and spaces present in the original.
        ///     </summary>
        /// <returns>text</returns>
        public virtual String GetWholeText() {
            return attributes == null ? text : attributes.Get(TEXT_KEY);
        }

        /// <summary>Test if this text node is blank -- that is, empty or only whitespace (including newlines).</summary>
        /// <returns>true if this document is empty or only whitespace, false if it contains any text content.</returns>
        public virtual bool IsBlank() {
            return Org.Jsoup.Helper.StringUtil.IsBlank(GetWholeText());
        }

        /// <summary>Split this text node into two nodes at the specified string offset.</summary>
        /// <remarks>
        /// Split this text node into two nodes at the specified string offset. After splitting, this node will contain the
        /// original text up to the offset, and will have a new text node sibling containing the text after the offset.
        /// </remarks>
        /// <param name="offset">string offset point to split node at.</param>
        /// <returns>the newly created text node containing the text after the offset.</returns>
        public virtual Org.Jsoup.Nodes.TextNode SplitText(int offset) {
            Validate.IsTrue(offset >= 0, "Split offset must be not be negative");
            Validate.IsTrue(offset < text.Length, "Split offset must not be greater than current text length");
            String head = GetWholeText().JSubstring(0, offset);
            String tail = GetWholeText().Substring(offset);
            Text(head);
            Org.Jsoup.Nodes.TextNode tailNode = new Org.Jsoup.Nodes.TextNode(tail, this.BaseUri());
            if (Parent() != null) {
                Parent().AddChildren(SiblingIndex() + 1, tailNode);
            }
            return tailNode;
        }

        /// <exception cref="System.IO.IOException"/>
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            if (@out.PrettyPrint() && ((SiblingIndex() == 0 && parentNode is Element && ((Element)parentNode).Tag().FormatAsBlock
                () && !IsBlank()) || (@out.Outline() && SiblingNodes().Count > 0 && !IsBlank()))) {
                Indent(accum, depth, @out);
            }
            bool normaliseWhite = @out.PrettyPrint() && Parent() is Element && !Element.PreserveWhitespace(Parent());
            Entities.Escape(accum, GetWholeText(), @out, false, normaliseWhite, false);
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }

        /// <summary>Create a new TextNode from HTML encoded (aka escaped) data.</summary>
        /// <param name="encodedText">Text containing encoded HTML (e.g. &amp;lt;)</param>
        /// <param name="baseUri">Base uri</param>
        /// <returns>TextNode containing unencoded data (e.g. &lt;)</returns>
        public static Org.Jsoup.Nodes.TextNode CreateFromEncoded(String encodedText, String baseUri) {
            String text = Entities.Unescape(encodedText);
            return new Org.Jsoup.Nodes.TextNode(text, baseUri);
        }

        internal static String NormaliseWhitespace(String text) {
            text = Org.Jsoup.Helper.StringUtil.NormaliseWhitespace(text);
            return text;
        }

        internal static String StripLeadingWhitespace(String text) {
            return text.ReplaceFirst("^\\s+", "");
        }

        internal static bool LastCharIsWhitespace(StringBuilder sb) {
            return sb.Length != 0 && sb[sb.Length - 1] == ' ';
        }

        // attribute fiddling. create on first access.
        private void EnsureAttributes() {
            if (attributes == null) {
                attributes = new Org.Jsoup.Nodes.Attributes();
                attributes.Put(TEXT_KEY, text);
            }
        }

        public override String Attr(String attributeKey) {
            EnsureAttributes();
            return base.Attr(attributeKey);
        }

        public override Org.Jsoup.Nodes.Attributes Attributes() {
            EnsureAttributes();
            return base.Attributes();
        }

        public override Node Attr(String attributeKey, String attributeValue) {
            EnsureAttributes();
            return base.Attr(attributeKey, attributeValue);
        }

        public override bool HasAttr(String attributeKey) {
            EnsureAttributes();
            return base.HasAttr(attributeKey);
        }

        public override Node RemoveAttr(String attributeKey) {
            EnsureAttributes();
            return base.RemoveAttr(attributeKey);
        }

        public override String AbsUrl(String attributeKey) {
            EnsureAttributes();
            return base.AbsUrl(attributeKey);
        }
    }
}
