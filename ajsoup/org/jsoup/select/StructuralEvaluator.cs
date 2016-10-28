using System;
using Org.Jsoup.Nodes;

namespace Org.Jsoup.Select {
    /// <summary>Base structural evaluator.</summary>
    internal abstract class StructuralEvaluator : Evaluator {
        internal Evaluator evaluator;

        internal class Root : Evaluator {
            public override bool Matches(Element root, Element element) {
                return root == element;
            }
        }

        internal class Has : StructuralEvaluator {
            public Has(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(Element root, Element element) {
                foreach (Element e in element.GetAllElements()) {
                    if (e != element && evaluator.Matches(root, e)) {
                        return true;
                    }
                }
                return false;
            }

            public override String ToString() {
                return String.Format(":has({0})", evaluator);
            }
        }

        internal class Not : StructuralEvaluator {
            public Not(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(Element root, Element node) {
                return !evaluator.Matches(root, node);
            }

            public override String ToString() {
                return String.Format(":not{0}", evaluator);
            }
        }

        internal class Parent : StructuralEvaluator {
            public Parent(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(Element root, Element element) {
                if (root == element) {
                    return false;
                }
                Element parent = ((Element)element.Parent());
                while (true) {
                    if (evaluator.Matches(root, parent)) {
                        return true;
                    }
                    if (parent == root) {
                        break;
                    }
                    parent = ((Element)parent.Parent());
                }
                return false;
            }

            public override String ToString() {
                return String.Format(":parent{0}", evaluator);
            }
        }

        internal class ImmediateParent : StructuralEvaluator {
            public ImmediateParent(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(Element root, Element element) {
                if (root == element) {
                    return false;
                }
                Element parent = ((Element)element.Parent());
                return parent != null && evaluator.Matches(root, parent);
            }

            public override String ToString() {
                return String.Format(":ImmediateParent{0}", evaluator);
            }
        }

        internal class PreviousSibling : StructuralEvaluator {
            public PreviousSibling(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(Element root, Element element) {
                if (root == element) {
                    return false;
                }
                Element prev = element.PreviousElementSibling();
                while (prev != null) {
                    if (evaluator.Matches(root, prev)) {
                        return true;
                    }
                    prev = prev.PreviousElementSibling();
                }
                return false;
            }

            public override String ToString() {
                return String.Format(":prev*{0}", evaluator);
            }
        }

        internal class ImmediatePreviousSibling : StructuralEvaluator {
            public ImmediatePreviousSibling(Evaluator evaluator) {
                this.evaluator = evaluator;
            }

            public override bool Matches(Element root, Element element) {
                if (root == element) {
                    return false;
                }
                Element prev = element.PreviousElementSibling();
                return prev != null && evaluator.Matches(root, prev);
            }

            public override String ToString() {
                return String.Format(":prev{0}", evaluator);
            }
        }
    }
}
