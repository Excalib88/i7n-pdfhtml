using System;
using System.Collections.Generic;
using Org.Jsoup.Nodes;

namespace Org.Jsoup.Parser {
    /// <summary>
    /// Parses HTML into a
    /// <see cref="Org.Jsoup.Nodes.Document"/>
    /// . Generally best to use one of the  more convenient parse methods
    /// in
    /// <see cref="Org.Jsoup.Jsoup"/>
    /// .
    /// </summary>
    public class Parser {
        private const int DEFAULT_MAX_ERRORS = 0;

        private TreeBuilder treeBuilder;

        private int maxErrors = DEFAULT_MAX_ERRORS;

        private ParseErrorList errors;

        /// <summary>Create a new Parser, using the specified TreeBuilder</summary>
        /// <param name="treeBuilder">TreeBuilder to use to parse input into Documents.</param>
        public Parser(TreeBuilder treeBuilder) {
            // by default, error tracking is disabled.
            this.treeBuilder = treeBuilder;
        }

        public virtual Document ParseInput(String html, String baseUri) {
            errors = IsTrackErrors() ? ParseErrorList.Tracking(maxErrors) : ParseErrorList.NoTracking();
            return treeBuilder.Parse(html, baseUri, errors);
        }

        // gets & sets
        /// <summary>Get the TreeBuilder currently in use.</summary>
        /// <returns>current TreeBuilder.</returns>
        public virtual TreeBuilder GetTreeBuilder() {
            return treeBuilder;
        }

        /// <summary>Update the TreeBuilder used when parsing content.</summary>
        /// <param name="treeBuilder">current TreeBuilder</param>
        /// <returns>this, for chaining</returns>
        public virtual Org.Jsoup.Parser.Parser SetTreeBuilder(TreeBuilder treeBuilder) {
            this.treeBuilder = treeBuilder;
            return this;
        }

        /// <summary>Check if parse error tracking is enabled.</summary>
        /// <returns>current track error state.</returns>
        public virtual bool IsTrackErrors() {
            return maxErrors > 0;
        }

        /// <summary>Enable or disable parse error tracking for the next parse.</summary>
        /// <param name="maxErrors">the maximum number of errors to track. Set to 0 to disable.</param>
        /// <returns>this, for chaining</returns>
        public virtual Org.Jsoup.Parser.Parser SetTrackErrors(int maxErrors) {
            this.maxErrors = maxErrors;
            return this;
        }

        /// <summary>Retrieve the parse errors, if any, from the last parse.</summary>
        /// <returns>list of parse errors, up to the size of the maximum errors tracked.</returns>
        public virtual IList<ParseError> GetErrors() {
            return errors;
        }

        // static parse functions below
        /// <summary>Parse HTML into a Document.</summary>
        /// <param name="html">HTML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>parsed Document</returns>
        public static Document Parse(String html, String baseUri) {
            TreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.Parse(html, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>Parse a fragment of HTML into a list of nodes.</summary>
        /// <remarks>Parse a fragment of HTML into a list of nodes. The context element, if supplied, supplies parsing context.
        ///     </remarks>
        /// <param name="fragmentHtml">the fragment of HTML to parse</param>
        /// <param name="context">
        /// (optional) the element that this HTML fragment is being parsed for (i.e. for inner HTML). This
        /// provides stack context (for implicit element creation).
        /// </param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>list of nodes parsed from the input HTML. Note that the context element, if supplied, is not modified.
        ///     </returns>
        public static IList<Node> ParseFragment(String fragmentHtml, Element context, String baseUri) {
            HtmlTreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.ParseFragment(fragmentHtml, context, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>Parse a fragment of XML into a list of nodes.</summary>
        /// <param name="fragmentXml">the fragment of XML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>list of nodes parsed from the input XML.</returns>
        public static IList<Node> ParseXmlFragment(String fragmentXml, String baseUri) {
            XmlTreeBuilder treeBuilder = new XmlTreeBuilder();
            return treeBuilder.ParseFragment(fragmentXml, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>
        /// Parse a fragment of HTML into the
        /// <c>body</c>
        /// of a Document.
        /// </summary>
        /// <param name="bodyHtml">fragment of HTML</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>Document, with empty head, and HTML parsed into body</returns>
        public static Document ParseBodyFragment(String bodyHtml, String baseUri) {
            Document doc = Document.CreateShell(baseUri);
            Element body = doc.Body();
            IList<Node> nodeList = ParseFragment(bodyHtml, body, baseUri);
            Node[] nodes = nodeList.ToArray(new Node[nodeList.Count]);
            // the node list gets modified when re-parented
            for (int i = nodes.Length - 1; i > 0; i--) {
                nodes[i].Remove();
            }
            foreach (Node node in nodes) {
                body.AppendChild(node);
            }
            return doc;
        }

        /// <summary>Utility method to unescape HTML entities from a string</summary>
        /// <param name="string">HTML escaped string</param>
        /// <param name="inAttribute">if the string is to be escaped in strict mode (as attributes are)</param>
        /// <returns>an unescaped string</returns>
        public static String UnescapeEntities(String @string, bool inAttribute) {
            Tokeniser tokeniser = new Tokeniser(new CharacterReader(@string), ParseErrorList.NoTracking());
            return tokeniser.UnescapeEntities(inAttribute);
        }

        /// <param name="bodyHtml">HTML to parse</param>
        /// <param name="baseUri">baseUri base URI of document (i.e. original fetch location), for resolving relative URLs.
        ///     </param>
        /// <returns>parsed Document</returns>
        [System.ObsoleteAttribute(@"Use ParseBodyFragment(System.String, System.String) or ParseFragment(System.String, Org.Jsoup.Nodes.Element, System.String) instead."
            )]
        public static Document ParseBodyFragmentRelaxed(String bodyHtml, String baseUri) {
            return Parse(bodyHtml, baseUri);
        }

        // builders
        /// <summary>Create a new HTML parser.</summary>
        /// <remarks>
        /// Create a new HTML parser. This parser treats input as HTML5, and enforces the creation of a normalised document,
        /// based on a knowledge of the semantics of the incoming tags.
        /// </remarks>
        /// <returns>a new HTML parser.</returns>
        public static Org.Jsoup.Parser.Parser HtmlParser() {
            return new Org.Jsoup.Parser.Parser(new HtmlTreeBuilder());
        }

        /// <summary>Create a new XML parser.</summary>
        /// <remarks>
        /// Create a new XML parser. This parser assumes no knowledge of the incoming tags and does not treat it as HTML,
        /// rather creates a simple tree directly from the input.
        /// </remarks>
        /// <returns>a new simple XML parser.</returns>
        public static Org.Jsoup.Parser.Parser XmlParser() {
            return new Org.Jsoup.Parser.Parser(new XmlTreeBuilder());
        }
    }
}
