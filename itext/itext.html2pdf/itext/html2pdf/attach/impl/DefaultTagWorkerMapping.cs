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
    address: sales@itextpdf.com */
using System;
using iText.Html2pdf.Attach.Impl.Tags;
using iText.Html2pdf.Css;
using iText.Html2pdf.Css.Pseudo;
using iText.Html2pdf.Html;
using iText.Html2pdf.Util;

namespace iText.Html2pdf.Attach.Impl {
    internal class DefaultTagWorkerMapping {
        private DefaultTagWorkerMapping() {
        }

        private static TagProcessorMapping workerMapping;

        static DefaultTagWorkerMapping() {
            workerMapping = new TagProcessorMapping();
            workerMapping.PutMapping(TagConstants.A, typeof(ATagWorker));
            workerMapping.PutMapping(TagConstants.ABBR, typeof(AbbrTagWorker));
            workerMapping.PutMapping(TagConstants.ADDRESS, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.ARTICLE, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.ASIDE, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.B, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.BDI, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.BDO, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.BLOCKQUOTE, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.BODY, typeof(BodyTagWorker));
            workerMapping.PutMapping(TagConstants.BR, typeof(BrTagWorker));
            workerMapping.PutMapping(TagConstants.CENTER, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.CITE, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.CODE, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.COL, typeof(ColTagWorker));
            workerMapping.PutMapping(TagConstants.COLGROUP, typeof(ColgroupTagWorker));
            workerMapping.PutMapping(TagConstants.DD, typeof(LiTagWorker));
            workerMapping.PutMapping(TagConstants.DEL, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.DFN, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.DIV, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.DL, typeof(UlOlTagWorker));
            workerMapping.PutMapping(TagConstants.DT, typeof(LiTagWorker));
            workerMapping.PutMapping(TagConstants.EM, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.FIGCAPTION, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.FIGURE, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.FONT, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.FOOTER, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.H1, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.H2, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.H3, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.H4, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.H5, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.H6, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.HEADER, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.HR, typeof(HrTagWorker));
            workerMapping.PutMapping(TagConstants.HTML, typeof(HtmlTagWorker));
            workerMapping.PutMapping(TagConstants.I, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.IMG, typeof(ImgTagWorker));
            workerMapping.PutMapping(TagConstants.INPUT, typeof(InputTagWorker));
            workerMapping.PutMapping(TagConstants.INS, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.KBD, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.LABEL, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.LI, typeof(LiTagWorker));
            workerMapping.PutMapping(TagConstants.LINK, typeof(LinkTagWorker));
            workerMapping.PutMapping(TagConstants.MAIN, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.MARK, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.META, typeof(MetaTagWorker));
            workerMapping.PutMapping(TagConstants.NAV, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.OL, typeof(UlOlTagWorker));
            workerMapping.PutMapping(TagConstants.P, typeof(PTagWorker));
            workerMapping.PutMapping(TagConstants.PRE, typeof(PreTagWorker));
            workerMapping.PutMapping(TagConstants.Q, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.S, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.SAMP, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.SECTION, typeof(DivTagWorker));
            workerMapping.PutMapping(TagConstants.SMALL, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.SPAN, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.STRIKE, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.STRONG, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.SUB, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.SUP, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.TABLE, typeof(TableTagWorker));
            workerMapping.PutMapping(TagConstants.TD, typeof(TdTagWorker));
            workerMapping.PutMapping(TagConstants.TFOOT, typeof(TableFooterTagWorker));
            workerMapping.PutMapping(TagConstants.TH, typeof(TdTagWorker));
            workerMapping.PutMapping(TagConstants.THEAD, typeof(TableHeaderTagWorker));
            workerMapping.PutMapping(TagConstants.TIME, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.TITLE, typeof(TitleTagWorker));
            workerMapping.PutMapping(TagConstants.TR, typeof(TrTagWorker));
            workerMapping.PutMapping(TagConstants.U, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.UL, typeof(UlOlTagWorker));
            workerMapping.PutMapping(TagConstants.VAR, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.LI, CssConstants.INLINE, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.DD, CssConstants.INLINE, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.DT, CssConstants.INLINE, typeof(SpanTagWorker));
            workerMapping.PutMapping(TagConstants.SPAN, CssConstants.BLOCK, typeof(DivTagWorker));
            // pseudo elements mapping
            String beforePseudoElemName = CssPseudoElementNode.CreatePseudoElementTagName(CssConstants.BEFORE);
            String afterPseudoElemName = CssPseudoElementNode.CreatePseudoElementTagName(CssConstants.AFTER);
            workerMapping.PutMapping(beforePseudoElemName, typeof(SpanTagWorker));
            workerMapping.PutMapping(afterPseudoElemName, typeof(SpanTagWorker));
            workerMapping.PutMapping(beforePseudoElemName, CssConstants.BLOCK, typeof(DivTagWorker));
            workerMapping.PutMapping(afterPseudoElemName, CssConstants.BLOCK, typeof(DivTagWorker));
        }

        internal static TagProcessorMapping GetDefaultTagWorkerMapping() {
            return workerMapping;
        }
    }
}