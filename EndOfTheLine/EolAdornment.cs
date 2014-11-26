using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace EndOfTheLine
{
    ///<summary>
    /// EolAdornment listens to editor state to determine when to add and
    /// remove end of line markers in an adorned text view.
    ///</summary>
    public class EolAdornment
    {
        private readonly IEditorOptions options;
        private readonly IEditorFormatMap formatMap;
        private bool visible;
        private readonly EolAdornedTextView adornedTextView;

        internal static void Attach(IWpfTextView view, IEditorOptions options, IEditorFormatMapService formatMapService)
        {
            var textView = new EolAdornedTextView(view, view.GetAdornmentLayer("EolAdornment"));
            var adornment = new EolAdornment(options, formatMapService, textView);
            view.Closed += adornment.OnClosed;
        }

        private EolAdornment(IEditorOptions options, IEditorFormatMapService formatMapService, EolAdornedTextView textView)
        {
            adornedTextView = textView;
            this.options = options;
            options.OptionChanged += OnOptionChanged;

            formatMap = formatMapService.GetEditorFormatMap(adornedTextView.View);
            formatMap.FormatMappingChanged += FormatMapOnFormatMappingChanged;

            ReadWhitespaceBrushSetting();

            Visible = options.IsVisibleWhitespaceEnabled();
        }

        private void OnClosed(object sender, EventArgs args)
        {
            adornedTextView.View.Closed -= OnClosed;
            options.OptionChanged -= OnOptionChanged;
            if (Visible)
            {
                adornedTextView.View.LayoutChanged -= OnLayoutChanged;
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
                    adornedTextView.View.LayoutChanged -= OnLayoutChanged;
                    adornedTextView.RemoveAllAdornments();
                    visible = false;
                }
                else
                {
                    adornedTextView.View.LayoutChanged += OnLayoutChanged;
                    if (adornedTextView.View.TextViewLines != null)
                    {
                        adornedTextView.CreateVisuals();
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
            adornedTextView.WhitespaceBrush = (Brush)visibleWhitespace[EditorFormatDefinition.ForegroundBrushId];
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

            adornedTextView.RemoveAllAdornments();

            // Recreate adornments for all lines in the view.
            adornedTextView.CreateVisuals();
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

            RefreshAdornments(adornedTextView, e.NewOrReformattedLines);
        }

        /// <summary>
        /// Refresh the adornments of the view based on line
        /// information from a view layout changed event.
        /// </summary>
        /// <typeparam name="TLine">The type representing lines in the view.</typeparam>
        /// <param name="adornmentView">The view to refresh adornments in.</param>
        /// <param name="changedLines">
        /// The lines that have changed according to a view layout changed
        /// event.</param>
        internal static void RefreshAdornments<TLine>(
            IAdornedTextView<TLine> adornmentView, 
            IReadOnlyList<TLine> changedLines) where TLine : class
        {
            // VS2013 and VS2015 Preview under certain circumstances leave
            // out the first line in runs of lines affected by changes.
            //
            // For example when pressing enter on an empty line it removes
            // the adornment of the empty line, but only sends the newly
            // created line below in e.NewOrReformattedLines.
            //
            // A single call to OnLayoutChanged can contain multiple runs
            // of affected lines, and each run may be missing a first line
            // which have been stripped of its previous adornments.

            foreach (var line in changedLines)
            {
                adornmentView.AddAdornmentToLine(line);
            }

            var uncertainLinesAbove =
                changedLines.Select(line => ListItems.PreviousItemOrDefault(adornmentView.Lines, line))
                    .Except(changedLines)
                    .Where(line => line != null);

            foreach (var line in uncertainLinesAbove)
            {
                adornmentView.ClearAdornmentsFromLine(line);
                adornmentView.AddAdornmentToLine(line);
            }
        }
    }
}
