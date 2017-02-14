using System;

namespace Org.Jsoup.Parser {
    /// <summary>A Parse Error records an error in the input HTML that occurs in either the tokenisation or the tree building phase.
    ///     </summary>
    public class ParseError {
        private int pos;

        private String errorMsg;

        internal ParseError(int pos, String errorMsg) {
            this.pos = pos;
            this.errorMsg = errorMsg;
        }

        internal ParseError(int pos, String errorFormat, params Object[] args) {
            this.errorMsg = String.Format(errorFormat, args);
            this.pos = pos;
        }

        /// <summary>Retrieve the error message.</summary>
        /// <returns>the error message.</returns>
        public virtual String GetErrorMessage() {
            return errorMsg;
        }

        /// <summary>Retrieves the offset of the error.</summary>
        /// <returns>error offset within input</returns>
        public virtual int GetPosition() {
            return pos;
        }

        public override String ToString() {
            return pos + ": " + errorMsg;
        }
    }
}
