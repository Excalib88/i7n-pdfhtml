using System;
using System.Collections.Generic;
using Org.Jsoup.Helper;
using Org.Jsoup.Nodes;

namespace Org.Jsoup.Parser {
    /// <author>Jonathan Hedley</author>
    public abstract class TreeBuilder {
        internal CharacterReader reader;

        internal Tokeniser tokeniser;

        protected internal Document doc;

        protected internal List<Element> stack;

        protected internal String baseUri;

        internal Token currentToken;

        internal ParseErrorList errors;

        private Token.StartTag start = new Token.StartTag();

        private Token.EndTag end = new Token.EndTag();

        // current doc we are building into
        // the stack of open elements
        // current base uri, for creating new elements
        // currentToken is used only for error tracking.
        // null when not tracking errors
        // start tag to process
        internal virtual void InitialiseParse(String input, String baseUri, ParseErrorList errors) {
            Validate.NotNull(input, "String input must not be null");
            Validate.NotNull(baseUri, "BaseURI must not be null");
            doc = new Document(baseUri);
            reader = new CharacterReader(input);
            this.errors = errors;
            tokeniser = new Tokeniser(reader, errors);
            stack = new List<Element>(32);
            this.baseUri = baseUri;
        }

        internal virtual Document Parse(String input, String baseUri) {
            return Parse(input, baseUri, ParseErrorList.NoTracking());
        }

        internal virtual Document Parse(String input, String baseUri, ParseErrorList errors) {
            InitialiseParse(input, baseUri, errors);
            RunParser();
            return doc;
        }

        protected internal virtual void RunParser() {
            while (true) {
                Token token = tokeniser.Read();
                Process(token);
                token.Reset();
                if (token.type == Org.Jsoup.Parser.TokenType.EOF) {
                    break;
                }
            }
        }

        internal abstract bool Process(Token token);

        protected internal virtual bool ProcessStartTag(String name) {
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().Name(name));
            }
            return Process(((Token.Tag)start.Reset()).Name(name));
        }

        public virtual bool ProcessStartTag(String name, Attributes attrs) {
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().NameAttr(name, attrs));
            }
            start.Reset();
            start.NameAttr(name, attrs);
            return Process(start);
        }

        protected internal virtual bool ProcessEndTag(String name) {
            if (currentToken == end) {
                // don't recycle an in-use token
                return Process(new Token.EndTag().Name(name));
            }
            return Process(((Token.Tag)end.Reset()).Name(name));
        }

        protected internal virtual Element CurrentElement() {
            int size = stack.Count;
            return size > 0 ? stack[size - 1] : null;
        }
    }
}
