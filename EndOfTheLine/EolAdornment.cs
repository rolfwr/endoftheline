using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEditorOptions editorOptions;
        private readonly IEditorFormatMap formatMap;
        private bool isAdorning;
        private readonly EolAdornedTextView adornedTextView;
        private readonly IEolOptions eolOptions;

        internal static void Attach(IWpfTextView view, IEditorOptions options, IEditorFormatMapService formatMapService, IEolOptions eolOptions)
        {
            var textView = new EolAdornedTextView(view, view.GetAdornmentLayer("EolAdornment"), eolOptions);
            var adornment = new EolAdornment(options, formatMapService, textView, eolOptions);
            view.Closed += adornment.OnClosed;
        }

        private EolAdornment(IEditorOptions editorOptions, IEditorFormatMapService formatMapService, EolAdornedTextView textView, IEolOptions eolOptions)
        {
            adornedTextView = textView;
            this.eolOptions = eolOptions;
            this.editorOptions = editorOptions;
            this.editorOptions.OptionChanged += OnEditorOptionChanged;
            this.eolOptions.OptionChanged += OnEolOptionChanged;

            formatMap = formatMapService.GetEditorFormatMap(adornedTextView.View);
            formatMap.FormatMappingChanged += FormatMapOnFormatMappingChanged;
            ReadWhitespaceBrushSetting();
            UpdateAdorningState();
        }

        private void UpdateAdorningState()
        {
            if (CalculateVisibility())
            {
                StartAdorning();
                return;
            }

            StopAdorning();
        }

        private bool CalculateVisibility()
        {
            if (eolOptions.Visibility == VisibilityPolicy.WhenOtherWhitespaceIsVisible)
            {
                return editorOptions.IsVisibleWhitespaceEnabled();
            }

            return eolOptions.Visibility != VisibilityPolicy.Never;
        }

        private void OnClosed(object sender, EventArgs args)
        {
            adornedTextView.View.Closed -= OnClosed;
            editorOptions.OptionChanged -= OnEditorOptionChanged;
            eolOptions.OptionChanged -= OnEolOptionChanged;
            StopAdorning();
        }

        private void StartAdorning()
        {
            if (isAdorning)
            {
                return;
            }

            adornedTextView.View.LayoutChanged += OnLayoutChanged;
            if (adornedTextView.View.TextViewLines != null)
            {
                adornedTextView.CreateVisuals();
            }

            isAdorning = true;
        }

        private void StopAdorning()
        {
            if (!isAdorning)
            {
                return;
            }

            adornedTextView.View.LayoutChanged -= OnLayoutChanged;
            adornedTextView.RemoveAllAdornments();

            isAdorning = false;
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultTextViewOptions.UseVisibleWhitespaceId.Name)
            {
                return;
            }

            UpdateAdorningState();
        }

        private void OnEolOptionChanged(object sender, EventArgs eventArgs)
        {
            RecreateAdornments();
        }

        private void FormatMapOnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("Visible Whitespace"))
            {
                return;
            }

            ReadWhitespaceBrushSetting();
            RecreateAdornments();
        }

        private void RecreateAdornments()
        {
            StopAdorning();
            UpdateAdorningState();
        }

        private void ReadWhitespaceBrushSetting()
        {
            var visibleWhitespace = formatMap.GetProperties("Visible Whitespace");
            adornedTextView.WhitespaceBrush = (Brush)visibleWhitespace[EditorFormatDefinition.ForegroundBrushId];
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (!isAdorning)
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
        internal static void RefreshAdornments<TLine>(IAdornedTextView<TLine> adornmentView, IReadOnlyList<TLine> changedLines) where TLine : class
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

            var uncertainLinesAbove = changedLines.Select(line => ListItems.PreviousItemOrDefault(adornmentView.Lines, line)).Except(changedLines).Where(line => line != null);

            foreach (var line in uncertainLinesAbove)
            {
                adornmentView.ClearAdornmentsFromLine(line);
                adornmentView.AddAdornmentToLine(line);
            }
        }
    }
}
