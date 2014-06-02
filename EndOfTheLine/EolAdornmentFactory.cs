using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace EndOfTheLine
{
     /// <summary>
    /// Define an <see cref="IAdornmentLayer" /> for end of line markers which
    /// will be populated by monitoring <see cref="IWpfTextView" />
    /// instances that are created.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class EolAdornmentFactory : IWpfTextViewCreationListener
    {
        /// <summary>Defines the adornment layer for end of line markers.</summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("EolAdornment")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        public AdornmentLayerDefinition EolAdornmentLayer = null;

        /// <summary>
        /// Piggybacks an EolAdornment manager on top of a text view that has been
        /// created in order to provide end of line markers for that views.
        /// </summary>
        /// <param name="textView">
        /// The <see cref="IWpfTextView" /> to adorn with end of
        /// line markers.
        /// </param>
        public void TextViewCreated(IWpfTextView textView)
        {
            var options = EditorOptionsFactoryService.GetOptions(textView);
            EolAdornment.Attach(textView, options, FormatMapService);
        }

        [Import]
        internal IEditorFormatMapService FormatMapService = null;

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService;
    }
}
