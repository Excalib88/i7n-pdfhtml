/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using iText.Html2pdf.Css;
using iText.Html2pdf.Css.Resolve.Shorthand;

namespace iText.Html2pdf.Css.Resolve.Shorthand.Impl {
    public abstract class AbstractBoxShorthandResolver : IShorthandResolver {
        private const String _0_LEFT_1 = "{0}-left{1}";

        private const String _0_RIGHT_1 = "{0}-right{1}";

        private const String _0_BOTTOM_1 = "{0}-bottom{1}";

        private const String _0_TOP_1 = "{0}-top{1}";

        protected internal abstract String GetPrefix();

        protected internal abstract String GetPostfix();

        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            String[] props = iText.IO.Util.StringUtil.Split(shorthandExpression, "\\s+");
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            String topProperty = String.Format(_0_TOP_1, GetPrefix(), GetPostfix());
            String rightProperty = String.Format(_0_RIGHT_1, GetPrefix(), GetPostfix());
            String bottomProperty = String.Format(_0_BOTTOM_1, GetPrefix(), GetPostfix());
            String leftProperty = String.Format(_0_LEFT_1, GetPrefix(), GetPostfix());
            if (props.Length == 1) {
                resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                resolvedDecl.Add(new CssDeclaration(rightProperty, props[0]));
                resolvedDecl.Add(new CssDeclaration(bottomProperty, props[0]));
                resolvedDecl.Add(new CssDeclaration(leftProperty, props[0]));
            }
            else {
                if (props.Length == 2) {
                    resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                    resolvedDecl.Add(new CssDeclaration(rightProperty, props[1]));
                    resolvedDecl.Add(new CssDeclaration(bottomProperty, props[0]));
                    resolvedDecl.Add(new CssDeclaration(leftProperty, props[1]));
                }
                else {
                    if (props.Length == 3) {
                        resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                        resolvedDecl.Add(new CssDeclaration(rightProperty, props[1]));
                        resolvedDecl.Add(new CssDeclaration(bottomProperty, props[2]));
                        resolvedDecl.Add(new CssDeclaration(leftProperty, props[1]));
                    }
                    else {
                        if (props.Length == 4) {
                            resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                            resolvedDecl.Add(new CssDeclaration(rightProperty, props[1]));
                            resolvedDecl.Add(new CssDeclaration(bottomProperty, props[2]));
                            resolvedDecl.Add(new CssDeclaration(leftProperty, props[3]));
                        }
                    }
                }
            }
            return resolvedDecl;
        }
    }
}
