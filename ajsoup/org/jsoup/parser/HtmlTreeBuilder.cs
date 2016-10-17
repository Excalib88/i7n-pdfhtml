using System;
using System.Collections.Generic;
using Org.Jsoup.Helper;
using Org.Jsoup.Nodes;
using Org.Jsoup.Select;

namespace Org.Jsoup.Parser {
    /// <summary>HTML Tree Builder; creates a DOM from Tokens.</summary>
    public class HtmlTreeBuilder : TreeBuilder {
        public static readonly String[] TagsSearchInScope = new String[] { "applet", "caption", "html", "table", "td"
            , "th", "marquee", "object" };

        private static readonly String[] TagSearchList = new String[] { "ol", "ul" };

        private static readonly String[] TagSearchButton = new String[] { "button" };

        private static readonly String[] TagSearchTableScope = new String[] { "html", "table" };

        private static readonly String[] TagSearchSelectScope = new String[] { "optgroup", "option" };

        private static readonly String[] TagSearchEndTags = new String[] { "dd", "dt", "li", "option", "optgroup", 
            "p", "rp", "rt" };

        private static readonly String[] TagSearchSpecial = new String[] { "address", "applet", "area", "article", 
            "aside", "base", "basefont", "bgsound", "blockquote", "body", "br", "button", "caption", "center", "col"
            , "colgroup", "command", "dd", "details", "dir", "div", "dl", "dt", "embed", "fieldset", "figcaption", 
            "figure", "footer", "form", "frame", "frameset", "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", 
            "hgroup", "hr", "html", "iframe", "img", "input", "isindex", "li", "link", "listing", "marquee", "menu"
            , "meta", "nav", "noembed", "noframes", "noscript", "object", "ol", "p", "param", "plaintext", "pre", 
            "script", "section", "select", "style", "summary", "table", "tbody", "td", "textarea", "tfoot", "th", 
            "thead", "title", "tr", "ul", "wbr", "xmp" };

        private HtmlTreeBuilderState state;

        private HtmlTreeBuilderState originalState;

        private bool baseUriSetFromDoc = false;

        private Element headElement;

        private FormElement formElement;

        private Element contextElement;

        private List<Element> formattingElements = new List<Element>();

        private IList<String> pendingTableCharacters = new List<String>();

        private Token.EndTag emptyEnd = new Token.EndTag();

        private bool framesetOk = true;

        private bool fosterInserts = false;

        private bool fragmentParsing = false;

        internal HtmlTreeBuilder() {
        }

        // tag searches
        // the current state
        // original / marked state
        // the current head element
        // the current form element
        // fragment parse context -- could be null even if fragment parsing
        // active (open) formatting elements
        // chars in table to be shifted out
        // reused empty end tag
        // if ok to go into frameset
        // if next inserts should be fostered
        // if parsing a fragment of html
        internal override Document Parse(String input, String baseUri, ParseErrorList errors) {
            state = HtmlTreeBuilderState.Initial;
            baseUriSetFromDoc = false;
            return base.Parse(input, baseUri, errors);
        }

        internal virtual IList<Node> ParseFragment(String inputFragment, Element context, String baseUri, ParseErrorList
             errors) {
            // context may be null
            state = HtmlTreeBuilderState.Initial;
            InitialiseParse(inputFragment, baseUri, errors);
            contextElement = context;
            fragmentParsing = true;
            Element root = null;
            if (context != null) {
                if (context.OwnerDocument() != null) {
                    // quirks setup:
                    doc.QuirksMode(context.OwnerDocument().QuirksMode());
                }
                // initialise the tokeniser state:
                String contextTag = context.TagName();
                if (Org.Jsoup.Helper.StringUtil.In(contextTag, "title", "textarea")) {
                    tokeniser.Transition(TokeniserState.Rcdata);
                }
                else {
                    if (Org.Jsoup.Helper.StringUtil.In(contextTag, "iframe", "noembed", "noframes", "style", "xmp")) {
                        tokeniser.Transition(TokeniserState.Rawtext);
                    }
                    else {
                        if (contextTag.Equals("script")) {
                            tokeniser.Transition(TokeniserState.ScriptData);
                        }
                        else {
                            if (contextTag.Equals(("noscript"))) {
                                tokeniser.Transition(TokeniserState.Data);
                            }
                            else {
                                // if scripting enabled, rawtext
                                if (contextTag.Equals("plaintext")) {
                                    tokeniser.Transition(TokeniserState.Data);
                                }
                                else {
                                    tokeniser.Transition(TokeniserState.Data);
                                }
                            }
                        }
                    }
                }
                // default
                root = new Element(Org.Jsoup.Parser.Tag.ValueOf("html"), baseUri);
                doc.AppendChild(root);
                stack.Add(root);
                ResetInsertionMode();
                // setup form element to nearest form on context (up ancestor chain). ensures form controls are associated
                // with form correctly
                Elements contextChain = context.Parents();
                contextChain.Add(0, context);
                foreach (Element parent in contextChain) {
                    if (parent is FormElement) {
                        formElement = (FormElement)parent;
                        break;
                    }
                }
            }
            RunParser();
            if (context != null && root != null) {
                return root.ChildNodes();
            }
            else {
                return doc.ChildNodes();
            }
        }

        internal override bool Process(Token token) {
            currentToken = token;
            return this.state.Process(token, this);
        }

        internal virtual bool Process(Token token, HtmlTreeBuilderState state) {
            currentToken = token;
            return state.Process(token, this);
        }

        internal virtual void Transition(HtmlTreeBuilderState state) {
            this.state = state;
        }

        internal virtual HtmlTreeBuilderState State() {
            return state;
        }

        internal virtual void MarkInsertionMode() {
            originalState = state;
        }

        internal virtual HtmlTreeBuilderState OriginalState() {
            return originalState;
        }

        internal virtual void FramesetOk(bool framesetOk) {
            this.framesetOk = framesetOk;
        }

        internal virtual bool FramesetOk() {
            return framesetOk;
        }

        internal virtual Document GetDocument() {
            return doc;
        }

        internal virtual String GetBaseUri() {
            return baseUri;
        }

        internal virtual void MaybeSetBaseUri(Element @base) {
            if (baseUriSetFromDoc) {
                // only listen to the first <base href> in parse
                return;
            }
            String href = @base.AbsUrl("href");
            if (href.Length != 0) {
                // ignore <base target> etc
                baseUri = href;
                baseUriSetFromDoc = true;
                doc.SetBaseUri(href);
            }
        }

        // set on the doc so doc.createElement(Tag) will get updated base, and to update all descendants
        internal virtual bool IsFragmentParsing() {
            return fragmentParsing;
        }

        internal virtual void Error(HtmlTreeBuilderState state) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), "Unexpected token [%s] when in state [%s]", currentToken.TokenType
                    (), state));
            }
        }

        internal virtual Element Insert(Token.StartTag startTag) {
            // handle empty unknown tags
            // when the spec expects an empty tag, will directly hit insertEmpty, so won't generate this fake end tag.
            if (startTag.IsSelfClosing()) {
                Element el = InsertEmpty(startTag);
                stack.Add(el);
                tokeniser.Transition(TokeniserState.Data);
                // handles <script />, otherwise needs breakout steps from script data
                tokeniser.Emit(((Token.Tag)emptyEnd.Reset()).Name(el.TagName()));
                // ensure we get out of whatever state we are in. emitted for yielded processing
                return el;
            }
            Element el_1 = new Element(Org.Jsoup.Parser.Tag.ValueOf(startTag.Name()), baseUri, startTag.attributes);
            Insert(el_1);
            return el_1;
        }

        internal virtual Element InsertStartTag(String startTagName) {
            Element el = new Element(Org.Jsoup.Parser.Tag.ValueOf(startTagName), baseUri);
            Insert(el);
            return el;
        }

        internal virtual void Insert(Element el) {
            InsertNode(el);
            stack.Add(el);
        }

        internal virtual Element InsertEmpty(Token.StartTag startTag) {
            Org.Jsoup.Parser.Tag tag = Org.Jsoup.Parser.Tag.ValueOf(startTag.Name());
            Element el = new Element(tag, baseUri, startTag.attributes);
            InsertNode(el);
            if (startTag.IsSelfClosing()) {
                if (tag.IsKnownTag()) {
                    if (tag.IsSelfClosing()) {
                        tokeniser.AcknowledgeSelfClosingFlag();
                    }
                }
                else {
                    // if not acked, promulagates error
                    // unknown tag, remember this is self closing for output
                    tag.SetSelfClosing();
                    tokeniser.AcknowledgeSelfClosingFlag();
                }
            }
            // not an distinct error
            return el;
        }

        internal virtual FormElement InsertForm(Token.StartTag startTag, bool onStack) {
            Org.Jsoup.Parser.Tag tag = Org.Jsoup.Parser.Tag.ValueOf(startTag.Name());
            FormElement el = new FormElement(tag, baseUri, startTag.attributes);
            SetFormElement(el);
            InsertNode(el);
            if (onStack) {
                stack.Add(el);
            }
            return el;
        }

        internal virtual void Insert(Token.Comment commentToken) {
            Comment comment = new Comment(commentToken.GetData(), baseUri);
            InsertNode(comment);
        }

        internal virtual void Insert(Token.Character characterToken) {
            Node node;
            // characters in script and style go in as datanodes, not text nodes
            String tagName = CurrentElement().TagName();
            if (tagName.Equals("script") || tagName.Equals("style")) {
                node = new DataNode(characterToken.GetData(), baseUri);
            }
            else {
                node = new TextNode(characterToken.GetData(), baseUri);
            }
            CurrentElement().AppendChild(node);
        }

        // doesn't use insertNode, because we don't foster these; and will always have a stack.
        private void InsertNode(Node node) {
            // if the stack hasn't been set up yet, elements (doctype, comments) go into the doc
            if (stack.Count == 0) {
                doc.AppendChild(node);
            }
            else {
                if (IsFosterInserts()) {
                    InsertInFosterParent(node);
                }
                else {
                    CurrentElement().AppendChild(node);
                }
            }
            // connect form controls to their form element
            if (node is Element && ((Element)node).Tag().IsFormListed()) {
                if (formElement != null) {
                    formElement.AddElement((Element)node);
                }
            }
        }

        internal virtual Element Pop() {
            int size = stack.Count;
            return stack.JRemoveAt(size - 1);
        }

        internal virtual void Push(Element element) {
            stack.Add(element);
        }

        internal virtual List<Element> GetStack() {
            return stack;
        }

        internal virtual bool OnStack(Element el) {
            return IsElementInQueue(stack, el);
        }

        private bool IsElementInQueue(List<Element> queue, Element element) {
            for (int pos = queue.Count - 1; pos >= 0; pos--) {
                Element next = queue[pos];
                if (next == element) {
                    return true;
                }
            }
            return false;
        }

        internal virtual Element GetFromStack(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    return next;
                }
            }
            return null;
        }

        internal virtual bool RemoveFromStack(Element el) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                if (next == el) {
                    stack.JRemoveAt(pos);
                    return true;
                }
            }
            return false;
        }

        internal virtual void PopStackToClose(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (next.NodeName().Equals(elName)) {
                    break;
                }
            }
        }

        internal virtual void PopStackToClose(params String[] elNames) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (Org.Jsoup.Helper.StringUtil.In(next.NodeName(), elNames)) {
                    break;
                }
            }
        }

        internal virtual void PopStackToBefore(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    break;
                }
                else {
                    stack.JRemoveAt(pos);
                }
            }
        }

        internal virtual void ClearStackToTableContext() {
            ClearStackToContext("table");
        }

        internal virtual void ClearStackToTableBodyContext() {
            ClearStackToContext("tbody", "tfoot", "thead");
        }

        internal virtual void ClearStackToTableRowContext() {
            ClearStackToContext("tr");
        }

        private void ClearStackToContext(params String[] nodeNames) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                if (Org.Jsoup.Helper.StringUtil.In(next.NodeName(), nodeNames) || next.NodeName().Equals("html")) {
                    break;
                }
                else {
                    stack.JRemoveAt(pos);
                }
            }
        }

        internal virtual Element AboveOnStack(Element el) {
            System.Diagnostics.Debug.Assert(OnStack(el));
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element next = stack[pos];
                if (next == el) {
                    return stack[pos - 1];
                }
            }
            return null;
        }

        internal virtual void InsertOnStackAfter(Element after, Element @in) {
            int i = stack.LastIndexOf(after);
            Validate.IsTrue(i != -1);
            stack.Add(i + 1, @in);
        }

        internal virtual void ReplaceOnStack(Element @out, Element @in) {
            ReplaceInQueue(stack, @out, @in);
        }

        private void ReplaceInQueue(List<Element> queue, Element @out, Element @in) {
            int i = queue.LastIndexOf(@out);
            Validate.IsTrue(i != -1);
            queue[i] = @in;
        }

        internal virtual void ResetInsertionMode() {
            bool last = false;
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element node = stack[pos];
                if (pos == 0) {
                    last = true;
                    node = contextElement;
                }
                String name = node.NodeName();
                if ("select".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InSelect);
                    break;
                }
                else {
                    // frag
                    if (("td".Equals(name) || "th".Equals(name) && !last)) {
                        Transition(HtmlTreeBuilderState.InCell);
                        break;
                    }
                    else {
                        if ("tr".Equals(name)) {
                            Transition(HtmlTreeBuilderState.InRow);
                            break;
                        }
                        else {
                            if ("tbody".Equals(name) || "thead".Equals(name) || "tfoot".Equals(name)) {
                                Transition(HtmlTreeBuilderState.InTableBody);
                                break;
                            }
                            else {
                                if ("caption".Equals(name)) {
                                    Transition(HtmlTreeBuilderState.InCaption);
                                    break;
                                }
                                else {
                                    if ("colgroup".Equals(name)) {
                                        Transition(HtmlTreeBuilderState.InColumnGroup);
                                        break;
                                    }
                                    else {
                                        // frag
                                        if ("table".Equals(name)) {
                                            Transition(HtmlTreeBuilderState.InTable);
                                            break;
                                        }
                                        else {
                                            if ("head".Equals(name)) {
                                                Transition(HtmlTreeBuilderState.InBody);
                                                break;
                                            }
                                            else {
                                                // frag
                                                if ("body".Equals(name)) {
                                                    Transition(HtmlTreeBuilderState.InBody);
                                                    break;
                                                }
                                                else {
                                                    if ("frameset".Equals(name)) {
                                                        Transition(HtmlTreeBuilderState.InFrameset);
                                                        break;
                                                    }
                                                    else {
                                                        // frag
                                                        if ("html".Equals(name)) {
                                                            Transition(HtmlTreeBuilderState.BeforeHead);
                                                            break;
                                                        }
                                                        else {
                                                            // frag
                                                            if (last) {
                                                                Transition(HtmlTreeBuilderState.InBody);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private String[] specificScopeTarget = new String[] { null };

        // frag
        // todo: tidy up in specific scope methods
        private bool InSpecificScope(String targetName, String[] baseTypes, String[] extraTypes) {
            specificScopeTarget[0] = targetName;
            return InSpecificScope(specificScopeTarget, baseTypes, extraTypes);
        }

        private bool InSpecificScope(String[] targetNames, String[] baseTypes, String[] extraTypes) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element el = stack[pos];
                String elName = el.NodeName();
                if (Org.Jsoup.Helper.StringUtil.In(elName, targetNames)) {
                    return true;
                }
                if (Org.Jsoup.Helper.StringUtil.In(elName, baseTypes)) {
                    return false;
                }
                if (extraTypes != null && Org.Jsoup.Helper.StringUtil.In(elName, extraTypes)) {
                    return false;
                }
            }
            Validate.Fail("Should not be reachable");
            return false;
        }

        internal virtual bool InScope(String[] targetNames) {
            return InSpecificScope(targetNames, TagsSearchInScope, null);
        }

        internal virtual bool InScope(String targetName) {
            return InScope(targetName, null);
        }

        internal virtual bool InScope(String targetName, String[] extras) {
            return InSpecificScope(targetName, TagsSearchInScope, extras);
        }

        // todo: in mathml namespace: mi, mo, mn, ms, mtext annotation-xml
        // todo: in svg namespace: forignOjbect, desc, title
        internal virtual bool InListItemScope(String targetName) {
            return InScope(targetName, TagSearchList);
        }

        internal virtual bool InButtonScope(String targetName) {
            return InScope(targetName, TagSearchButton);
        }

        internal virtual bool InTableScope(String targetName) {
            return InSpecificScope(targetName, TagSearchTableScope, null);
        }

        internal virtual bool InSelectScope(String targetName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                Element el = stack[pos];
                String elName = el.NodeName();
                if (elName.Equals(targetName)) {
                    return true;
                }
                if (!Org.Jsoup.Helper.StringUtil.In(elName, TagSearchSelectScope)) {
                    // all elements except
                    return false;
                }
            }
            Validate.Fail("Should not be reachable");
            return false;
        }

        internal virtual void SetHeadElement(Element headElement) {
            this.headElement = headElement;
        }

        internal virtual Element GetHeadElement() {
            return headElement;
        }

        internal virtual bool IsFosterInserts() {
            return fosterInserts;
        }

        internal virtual void SetFosterInserts(bool fosterInserts) {
            this.fosterInserts = fosterInserts;
        }

        internal virtual FormElement GetFormElement() {
            return formElement;
        }

        internal virtual void SetFormElement(FormElement formElement) {
            this.formElement = formElement;
        }

        internal virtual void NewPendingTableCharacters() {
            pendingTableCharacters = new List<String>();
        }

        internal virtual IList<String> GetPendingTableCharacters() {
            return pendingTableCharacters;
        }

        internal virtual void SetPendingTableCharacters(IList<String> pendingTableCharacters) {
            this.pendingTableCharacters = pendingTableCharacters;
        }

        /// <summary>
        /// 11.2.5.2 Closing elements that have implied end tags<p/>
        /// When the steps below require the UA to generate implied end tags, then, while the current node is a dd element, a
        /// dt element, an li element, an option element, an optgroup element, a p element, an rp element, or an rt element,
        /// the UA must pop the current node off the stack of open elements.
        /// </summary>
        /// <param name="excludeTag">
        /// If a step requires the UA to generate implied end tags but lists an element to exclude from the
        /// process, then the UA must perform the above steps as if that element was not in the above list.
        /// </param>
        internal virtual void GenerateImpliedEndTags(String excludeTag) {
            while ((excludeTag != null && !CurrentElement().NodeName().Equals(excludeTag)) && Org.Jsoup.Helper.StringUtil
                .In(CurrentElement().NodeName(), TagSearchEndTags)) {
                Pop();
            }
        }

        internal virtual void GenerateImpliedEndTags() {
            GenerateImpliedEndTags(null);
        }

        internal virtual bool IsSpecial(Element el) {
            // todo: mathml's mi, mo, mn
            // todo: svg's foreigObject, desc, title
            String name = el.NodeName();
            return Org.Jsoup.Helper.StringUtil.In(name, TagSearchSpecial);
        }

        internal virtual Element LastFormattingElement() {
            return formattingElements.Count > 0 ? formattingElements[formattingElements.Count - 1] : null;
        }

        internal virtual Element RemoveLastFormattingElement() {
            int size = formattingElements.Count;
            if (size > 0) {
                return formattingElements.JRemoveAt(size - 1);
            }
            else {
                return null;
            }
        }

        // active formatting elements
        internal virtual void PushActiveFormattingElements(Element @in) {
            int numSeen = 0;
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                Element el = formattingElements[pos];
                if (el == null) {
                    // marker
                    break;
                }
                if (IsSameFormattingElement(@in, el)) {
                    numSeen++;
                }
                if (numSeen == 3) {
                    formattingElements.JRemoveAt(pos);
                    break;
                }
            }
            formattingElements.Add(@in);
        }

        private bool IsSameFormattingElement(Element a, Element b) {
            // same if: same namespace, tag, and attributes. Element.equals only checks tag, might in future check children
            return a.NodeName().Equals(b.NodeName()) && a.Attributes().Equals(b.Attributes());
        }

        // a.namespace().equals(b.namespace()) &&
        // todo: namespaces
        internal virtual void ReconstructFormattingElements() {
            Element last = LastFormattingElement();
            if (last == null || OnStack(last)) {
                return;
            }
            Element entry = last;
            int size = formattingElements.Count;
            int pos = size - 1;
            bool skip = false;
            while (true) {
                if (pos == 0) {
                    // step 4. if none before, skip to 8
                    skip = true;
                    break;
                }
                entry = formattingElements[--pos];
                // step 5. one earlier than entry
                if (entry == null || OnStack(entry)) {
                    // step 6 - neither marker nor on stack
                    break;
                }
            }
            // jump to 8, else continue back to 4
            while (true) {
                if (!skip) {
                    // step 7: on later than entry
                    entry = formattingElements[++pos];
                }
                Validate.NotNull(entry);
                // should not occur, as we break at last element
                // 8. create new element from element, 9 insert into current node, onto stack
                skip = false;
                // can only skip increment from 4.
                Element newEl = InsertStartTag(entry.NodeName());
                // todo: avoid fostering here?
                // newEl.namespace(entry.namespace()); // todo: namespaces
                newEl.Attributes().AddAll(entry.Attributes());
                // 10. replace entry with new entry
                formattingElements[pos] = newEl;
                // 11
                if (pos == size - 1) {
                    // if not last entry in list, jump to 7
                    break;
                }
            }
        }

        internal virtual void ClearFormattingElementsToLastMarker() {
            while (!formattingElements.IsEmpty()) {
                Element el = RemoveLastFormattingElement();
                if (el == null) {
                    break;
                }
            }
        }

        internal virtual void RemoveFromActiveFormattingElements(Element el) {
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                Element next = formattingElements[pos];
                if (next == el) {
                    formattingElements.JRemoveAt(pos);
                    break;
                }
            }
        }

        internal virtual bool IsInActiveFormattingElements(Element el) {
            return IsElementInQueue(formattingElements, el);
        }

        internal virtual Element GetActiveFormattingElement(String nodeName) {
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                Element next = formattingElements[pos];
                if (next == null) {
                    // scope marker
                    break;
                }
                else {
                    if (next.NodeName().Equals(nodeName)) {
                        return next;
                    }
                }
            }
            return null;
        }

        internal virtual void ReplaceActiveFormattingElement(Element @out, Element @in) {
            ReplaceInQueue(formattingElements, @out, @in);
        }

        internal virtual void InsertMarkerToFormattingElements() {
            formattingElements.Add(null);
        }

        internal virtual void InsertInFosterParent(Node @in) {
            Element fosterParent;
            Element lastTable = GetFromStack("table");
            bool isLastTableParent = false;
            if (lastTable != null) {
                if (((Element)lastTable.Parent()) != null) {
                    fosterParent = ((Element)lastTable.Parent());
                    isLastTableParent = true;
                }
                else {
                    fosterParent = AboveOnStack(lastTable);
                }
            }
            else {
                // no table == frag
                fosterParent = stack[0];
            }
            if (isLastTableParent) {
                Validate.NotNull(lastTable);
                // last table cannot be null by this point.
                lastTable.Before(@in);
            }
            else {
                fosterParent.AppendChild(@in);
            }
        }

        public override String ToString() {
            return "TreeBuilder{" + "currentToken=" + currentToken + ", state=" + state + ", currentElement=" + CurrentElement
                () + '}';
        }
    }
}
