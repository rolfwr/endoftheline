using System.Text;
using EnvDTE;

namespace EndOfTheLine
{
    static class LineEndingCommands
    {
        /// <summary>
        /// Allows a selection to be remembered and restored.
        /// </summary>
        /// <remarks>
        /// Replacing the text unsets the selection, which can seem
        /// jarring to the user, and makes it more difficult to apply
        /// additional commands on the same selection. Therefore restore
        /// the selection after replacing the line endings.
        /// </remarks>
        private class RememberedSelection
        {
            private readonly bool topWasActivePoint;
            private readonly TextSelection selection;
            private readonly int bottomLine;
            private readonly int bottomLineOffset;
            private readonly int topLine;
            private readonly int topLineOffset;

            public RememberedSelection(TextSelection selection)
            {
                topWasActivePoint = selection.TopPoint.EqualTo(selection.ActivePoint);
                this.selection = selection;
                bottomLine = selection.BottomPoint.Line;
                bottomLineOffset = selection.BottomPoint.LineCharOffset;
                topLine = selection.TopPoint.Line;
                topLineOffset = selection.TopPoint.LineCharOffset;
            }

            internal void Restore()
            {
                if (topWasActivePoint)
                {
                    selection.MoveToLineAndOffset(bottomLine, bottomLineOffset);
                    selection.MoveToLineAndOffset(topLine, topLineOffset, true);
                }
                else
                {
                    selection.MoveToLineAndOffset(topLine, topLineOffset);
                    selection.MoveToLineAndOffset(bottomLine, bottomLineOffset, true);
                }
            }
        }

        private static bool IsEndOfLineChar(char c)
        {
            return (c == '\r' || c == '\n');
        }

        private static string ReplaceLineEndings(string oldText, string newLineEnding)
        {
            var length = oldText.Length;

            var newText = new StringBuilder();
            var start = 0;
            for (var pos = 0; pos < length; ++pos)
            {
                var c = oldText[pos];
                if (!IsEndOfLineChar(c))
                {
                    continue;
                }

                newText.Append(oldText, start, pos - start);
                newText.Append(newLineEnding);

                if (pos + 1 < length)
                {
                    var nextChar = oldText[pos + 1];
                    if (nextChar != c && IsEndOfLineChar(nextChar))
                    {
                        ++pos;
                    }
                }

                start = pos + 1;
            }

            if (start != length)
            {
                newText.Append(oldText, start, length - start);
            }

            return newText.ToString();
        }

        internal static void ReplaceLineEndingsInActiveDocument(TextDocument textDoc, string newLineEnding)
        {
            if (textDoc == null)
            {
                return;
            }

            var selection = textDoc.Selection;
            if (selection == null)
            {
                return;
            }

            var rememberedSelection = new RememberedSelection(selection);

            EditPoint top;
            EditPoint bottom;
            if (selection.IsEmpty)
            {
                top = textDoc.StartPoint.CreateEditPoint();
                bottom = textDoc.EndPoint.CreateEditPoint();
            }
            else
            {
                top = selection.TopPoint.CreateEditPoint();
                bottom = selection.BottomPoint.CreateEditPoint();
            }

            top.ReplaceText(bottom, ReplaceLineEndings(top.GetText(bottom), newLineEnding), 0);

            rememberedSelection.Restore();
        }
    }
}