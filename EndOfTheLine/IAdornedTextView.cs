using System.Collections.Generic;

namespace EndOfTheLine
{
    /// <summary>
    /// Represents a text view containing lines that can be given adornments.
    /// </summary>
    /// <typeparam name="TLine">The type representing the lines visible in the text view.</typeparam>
    internal interface IAdornedTextView<TLine>
    {
        /// <summary>
        /// Gets the text view lines.
        /// </summary>
        IList<TLine> Lines { get; }

        /// <summary>
        /// Create an appropriate adornment and add it to the line.
        /// </summary>
        /// <param name="line">The line to add the adornment to.</param>
        void AddAdornmentToLine(TLine line);

        /// <summary>
        /// Remove all adornments that have previously been added to a line.
        /// </summary>
        /// <param name="line">The line to remove the adornments from.</param>
        void ClearAdornmentsFromLine(TLine line);
    }
}