using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace EndOfTheLine
{
    /// <summary>
    /// Adds and removes end of line adornments for the lines in a text view.
    /// </summary>
    public class EolAdornedTextView : IAdornedTextView<ITextViewLine>
    {
        private readonly IAdornmentLayer layer;
        private readonly IWpfTextView view;
        private Brush whitespaceBrush;

        public EolAdornedTextView(IWpfTextView view, IAdornmentLayer layer)
        {
            this.view = view;
            this.layer = layer;
        }

        public IWpfTextView View
        {
            get { return view; }
        }

        public Brush WhitespaceBrush
        {
            set { whitespaceBrush = value; }
            get { return whitespaceBrush; }
        }

        public IList<ITextViewLine> Lines
        {
            get { return view.TextViewLines; }
        }

        public void ClearAdornmentsFromLine(ITextViewLine line)
        {
            var lineBreak = new SnapshotSpan(view.TextSnapshot, Span.FromBounds(line.End, line.EndIncludingLineBreak));
            layer.RemoveAdornmentsByVisualSpan(lineBreak);
        }

        public void AddAdornmentToLine(ITextViewLine line)
        {
            var lineBreak = new SnapshotSpan(view.TextSnapshot, Span.FromBounds(line.End, line.EndIncludingLineBreak));
            var markerGeom = view.TextViewLines.GetMarkerGeometry(lineBreak);
            if (markerGeom == null)
            {
                return; 
            }

            var eolLabel = GetEolLabel(lineBreak.GetText());

            var textProp = view.FormattedLineSource.DefaultTextProperties;
            var typeface = textProp.Typeface;

            var textBlock = new TextBlock
            {
                Text = eolLabel,
                FontFamily = typeface.FontFamily,
                FontSize = textProp.FontRenderingEmSize,
                FontWeight = typeface.Weight,
                FontStretch = typeface.Stretch,
                FontStyle = typeface.Style,
                Foreground = whitespaceBrush
            };

            UIElement adornment = textBlock;

            Canvas.SetLeft(adornment, markerGeom.Bounds.Left);
            Canvas.SetTop(adornment, markerGeom.Bounds.Top);

            layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, lineBreak, null, adornment, null);
        }

        public void CreateVisuals()
        {
            foreach (var line in view.TextViewLines)
            {
                AddAdornmentToLine(line);
            }
        }

        private static string GetEolLabel(string lineBreakText)
        {
            var sb = new StringBuilder();
            foreach (var c in lineBreakText)
            {
                switch (c)
                {
                    case '\r':
                        sb.Append("¤");
                        break;
                    case '\n':
                        sb.Append("¶");
                        break;
                    default:
                        sb.Append("<" + (int) c + ">");
                        break;
                }
            }

            return sb.ToString();
        }

        internal void RemoveAllAdornments()
        {
            layer.RemoveAllAdornments();
        }
    }
}