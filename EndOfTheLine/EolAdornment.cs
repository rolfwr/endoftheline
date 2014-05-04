using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace EndOfTheLine
{
    ///<summary>
    /// EolAdornment places end of line markers at the end of each line.
    ///</summary>
    public class EolAdornment
    {
        readonly IAdornmentLayer layer;
        readonly IWpfTextView view;
        private Brush whitespaceBrush;
        private readonly IEditorFormatMap formatMap;

        public EolAdornment(IWpfTextView view, IEditorFormatMapService formatMapService)
        {
            this.view = view;
            layer = view.GetAdornmentLayer("EolAdornment");

            formatMap = formatMapService.GetEditorFormatMap(this.view);
            formatMap.FormatMappingChanged += FormatMapOnFormatMappingChanged;

            ReadWhitespaceBrushSetting();

            // Listen to any event that may require us to apply end of line
            // markers.
            this.view.LayoutChanged += OnLayoutChanged;
        }

        public void Close()
        {
            view.LayoutChanged -= OnLayoutChanged;
        }

        private void ReadWhitespaceBrushSetting()
        {
            var visibleWhitespace = formatMap.GetProperties("Visible Whitespace");
            whitespaceBrush = (Brush)visibleWhitespace[EditorFormatDefinition.ForegroundBrushId];
        }

        private void FormatMapOnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("Visible Whitespace"))
            {
                return;
            }

            ReadWhitespaceBrushSetting();
            
            // Recreate adornments for all lines in the view.
            foreach (var line in view.TextViewLines)
            {
                CreateVisuals(line);
            }
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var line in e.NewOrReformattedLines)
            {
                CreateVisuals(line);
            }

            // When pressing enter on an empty line VS2013 removes the
            // adornment of the empty line, but only sends the newly
            // created line below in e.NewOrReformattedLines

            if (e.NewOrReformattedLines.Count == 0)
            {
                return;
            }

            var aboveLine = GetLineAbove(e.NewOrReformattedLines[0]);
            if (aboveLine != null)
            {
                CreateVisuals(aboveLine);
            }
        }

        private ITextViewLine GetLineAbove(ITextViewLine first)
        {
            return ListItems.PreviousItemOrDefault(view.TextViewLines, first);
        }

        /// <summary>
        /// Within the given line add the scarlet box behind the a
        /// </summary>
        private void CreateVisuals(ITextViewLine line)
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

            // Normally previous adornments are automatically removed when
            // lines are reformatted. However, we are applying new updated
            // adornments when the font settings change. In this case the
            // old adornments remain, and must be removed.
            layer.RemoveAdornmentsByVisualSpan(lineBreak);

            layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, lineBreak, null, adornment, null);
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
    }
}
