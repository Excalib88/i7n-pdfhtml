using System;
using System.Collections.Generic;
using Org.Jsoup.Helper;
using Org.Jsoup.Nodes;

namespace Org.Jsoup.Parser {
    /// <summary>
    /// Use the
    /// <c>XmlTreeBuilder</c>
    /// when you want to parse XML without any of the HTML DOM rules being applied to the
    /// document.
    /// <p>Usage example:
    /// <c>Document xmlDoc = Jsoup.parse(html, baseUrl, Parser.xmlParser());</c>
    /// </p>
    /// </summary>
    /// <author>Jonathan Hedley</author>
    public class XmlTreeBuilder : TreeBuilder {
        internal override void InitialiseParse(String input, String baseUri, ParseErrorList errors) {
            base.InitialiseParse(input, baseUri, errors);
            stack.Add(doc);
            // place the document onto the stack. differs from HtmlTreeBuilder (not on stack)
            doc.OutputSettings().Syntax(Syntax.xml);
        }

        internal override bool Process(Token token) {
            switch (token.type) {
                case Org.Jsoup.Parser.TokenType.StartTag: {
                    // start tag, end tag, doctype, comment, character, eof
                    Insert(token.AsStartTag());
                    break;
                }

                case Org.Jsoup.Parser.TokenType.EndTag: {
                    PopStackToClose(token.AsEndTag());
                    break;
                }

                case Org.Jsoup.Parser.TokenType.Comment: {
                    Insert(token.AsComment());
                    break;
                }

                case Org.Jsoup.Parser.TokenType.Character: {
                    Insert(token.AsCharacter());
                    break;
                }

                case Org.Jsoup.Parser.TokenType.Doctype: {
                    Insert(token.AsDoctype());
                    break;
                }

                case Org.Jsoup.Parser.TokenType.EOF: {
                    // could put some normalisation here if desired
                    break;
                }

                default: {
                    Validate.Fail("Unexpected token type: " + token.type);
                    break;
                }
            }
            return true;
        }

        private void InsertNode(Node node) {
            CurrentElement().AppendChild(node);
        }

        internal virtual Element Insert(Token.StartTag startTag) {
            Org.Jsoup.Parser.Tag tag = Org.Jsoup.Parser.Tag.ValueOf(startTag.Name());
            // todo: wonder if for xml parsing, should treat all tags as unknown? because it's not html.
            Element el = new Element(tag, baseUri, startTag.attributes);
            InsertNode(el);
            if (startTag.IsSelfClosing()) {
                tokeniser.AcknowledgeSelfClosingFlag();
                if (!tag.IsKnownTag()) {
                    // unknown tag, remember this is self closing for output. see above.
                    tag.SetSelfClosing();
                }
            }
            else {
                stack.Add(el);
            }
            return el;
        }

        internal virtual void Insert(Token.Comment commentToken) {
            Comment comment = new Comment(commentToken.GetData(), baseUri);
            Node insert = comment;
            if (commentToken.bogus) {
                // xml declarations are emitted as bogus comments (which is right for html, but not xml)
                // so we do a bit of a hack and parse the data as an element to pull the attributes out
                String data = comment.GetData();
                if (data.Length > 1 && (data.StartsWith("!") || data.StartsWith("?"))) {
                    Document doc = Org.Jsoup.Jsoup.Parse("<" + data.JSubstring(1, data.Length - 1) + ">", baseUri, Org.Jsoup.Parser.Parser
                        .XmlParser());
                    Element el = doc.Child(0);
                    insert = new XmlDeclaration(el.TagName(), comment.BaseUri(), data.StartsWith("!"));
                    insert.Attributes().AddAll(el.Attributes());
                }
            }
            InsertNode(insert);
        }

        internal virtual void Insert(Token.Character characterToken) {
            Node node = new TextNode(characterToken.GetData(), baseUri);
            InsertNode(node);
        }

        internal virtual void Insert(Token.Doctype d) {
            DocumentType doctypeNode = new DocumentType(d.GetName(), d.GetPublicIdentifier(), d.GetSystemIdentifier(), 
                baseUri);
            InsertNode(doctypeNode);
        }

        /// <summary>If the stack contains an element with this tag's name, pop up the stack to remove the first occurrence.
        ///     </summary>
        /// <remarks>
        /// If the stack contains an element with this tag's name, pop up the stack to remove the first occurrence. If not
        /// found, skips.
        /// </remarks>
        /// <param name="endTag"/>
        private void PopStackToClose(Token.EndTag endTag) {
            String elName = endTag.Name();
            Element firstFound = null;
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    firstFound = next;
                    break;
                }
            }
            if (firstFound == null) {
                return;
            }
            // not found, skip
            for (int pos_1 = stack.Count - 1; pos_1 >= 0; pos_1--) {
                Element next = stack[pos_1];
                stack.JRemoveAt(pos_1);
                if (next == firstFound) {
                    break;
                }
            }
        }

        internal virtual IList<Node> ParseFragment(String inputFragment, String baseUri, ParseErrorList errors) {
            InitialiseParse(inputFragment, baseUri, errors);
            RunParser();
            return doc.ChildNodes();
        }
    }
}
