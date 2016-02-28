using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
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
        public AdornmentLayerDefinition EolAdornmentLayer;

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
            EolAdornment.Attach(textView, options, FormatMapService, EolOptions);
        }

        [Import]
        internal IEditorFormatMapService FormatMapService;

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService;

        [Import(typeof (SVsServiceProvider))]
        internal IServiceProvider Services;

        // We request the IEolOptions instance from the
        // IServiceProvider interface implemented by SVsServiceProvider
        // which we obtain through MEF composition.
        //
        // When we call GetService(typeof (IEolOptions)) the
        // EndOfTheLinePackage class gets instantiated because it has a
        // [ProvideService(typeof(IEolOptions))] attribute. Its
        // constructor the adds the EolOptionPage instance implementing
        // IEolOptions to the service container, so that the GetService()
        // request can be fulfilled.
        //
        // This may seem complicated, but it is neccessary because:
        //  * We can't instantiate EolOptionPage directly because it
        //    requires the ISite provided by EndOfTheLinePackage. 
        //  * We can't instantiate EndOfTheLinePackage because unless
        //    its constructed by the visual studio shell, it will remain
        //    in an incomplete unusable state.
        //  * We can't instantiate any of the above through MEF imports
        //    because we'll encounter the same problems as when
        //    instantiating them directly.
        private IEolOptions EolOptions => (IEolOptions)Services.GetService(typeof (IEolOptions));
    }
}
