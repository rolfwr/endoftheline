using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Text.Formatting;

namespace EndOfTheLine
{
    ///<summary>
    /// EolAdornment places end of line markers at the end of each line.
    ///</summary>
    public class EolAdornment
    {
        private readonly IAdornmentLayer layer;
        private readonly IWpfTextView view;
        private readonly IEditorOptions options;
        private readonly IEditorFormatMap formatMap;
        private Brush whitespaceBrush;
        private bool visible;

        public static void Attach(IWpfTextView view, IEditorOptions options, IEditorFormatMapService formatMapService)
        {
            var adornment = new EolAdornment(view, options, formatMapService);
            view.Closed += adornment.OnClosed;
        }

        private EolAdornment(IWpfTextView view, IEditorOptions options, IEditorFormatMapService formatMapService)
        {
            this.view = view;
            layer = view.GetAdornmentLayer("EolAdornment");

            this.options = options;
            options.OptionChanged += OnOptionChanged;

            formatMap = formatMapService.GetEditorFormatMap(this.view);
            formatMap.FormatMappingChanged += FormatMapOnFormatMappingChanged;

            ReadWhitespaceBrushSetting();

            Visible = options.IsVisibleWhitespaceEnabled();
        }

        private void OnClosed(object sender, EventArgs args)
        {
            view.Closed -= OnClosed;
            options.OptionChanged -= OnOptionChanged;
            if (Visible)
            {
                view.LayoutChanged -= OnLayoutChanged;
            }
        }

        private bool Visible
        {
            get { return visible; }
            set
            {
                if (value == visible)
                    return;

                if (!value)
                {
                    view.LayoutChanged -= OnLayoutChanged;
                    layer.RemoveAllAdornments();
                    visible = false;
                }
                else
                {
                    view.LayoutChanged += OnLayoutChanged;
                    if (view.TextViewLines != null)
                    {
                        CreateVisuals();
                    }
                    visible = true;
                }
            }
        }

        private void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultTextViewOptions.UseVisibleWhitespaceId.Name)
            {
                return;
            }

            Visible = options.IsVisibleWhitespaceEnabled();
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

            if (!Visible)
            {
                return;
            }

            layer.RemoveAllAdornments();

            // Recreate adornments for all lines in the view.
            CreateVisuals();
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (!visible)
            {
                return;
            }

            foreach (var line in e.NewOrReformattedLines)
            {
                CreateLineVisuals(line);
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
                CreateLineVisuals(aboveLine);
            }
        }

        private ITextViewLine GetLineAbove(ITextViewLine first)
        {
            return ListItems.PreviousItemOrDefault(view.TextViewLines, first);
        }

        private void CreateVisuals()
        {
            foreach (var line in view.TextViewLines)
            {
                CreateLineVisuals(line);
            }
        }

        /// <summary>
        /// Within the given line add the scarlet box behind the a
        /// </summary>
        private void CreateLineVisuals(ITextViewLine line)
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
