

using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;

namespace EndOfTheLine
{
    // Make the CreatePkgDef.exe recognize this class as a Visual Studio
    // Package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // Provide information to show in Help/About dialog box.
    // The numeric identifiers refer to resources defined in VSPacakge.resx.
    [InstalledProductRegistration("#110", "#112", "1.3", IconResourceID = 400)]
    // Expose menus defined in EndOfTheLine.vsct file to the Visual Studio GUI
    // shell.
    //
    // The EndOfTheLine.csproj file defines "Menus.ctmenu" as the resource name
    // to create when compiling the EndOfTheLine.vsct file in the
    // /Project/ItemGroup/VSCTCompile/ResourceName element.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // Specifies the GUID of the package. This Guid matches
    // /CommandTable/Symbols/GuidSymbol[@name="guidEndOfTheLinePkg"]/@value
    // in the EndOfTheLine.vsct file.
    [Guid(Guids.GuidEndOfTheLinePkgString)]

    // Make our option page UI available.
    // By specifying that we support automation the IDE will persist the
    // page's state.
    [ProvideOptionPage(typeof(EolOptionPage), "End of the Line", "Visibility", 0, 0, true)]

    // Ensure that the package gets loaded somebody is requesting our options.
    [ProvideService(typeof(IEolOptions))]
    public sealed class EndOfTheLinePackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            if (null == mcs)
            {
                return;
            }

            AddCommandForMenuItem(mcs, CommandIds.CmdidMakeLineEndingsCrLf, MakeLineEndingsCrLf);
            AddCommandForMenuItem(mcs, CommandIds.CmdidMakeLineEndingsLf, MakeLineEndingsLf);

            // Fulfill the promise made by the "ProvideService" attribute.
            IServiceContainer services = this;
            services.AddService(typeof(IEolOptions), delegate { return GetDialogPage(typeof (EolOptionPage)); }, promote: true);
        }

        private static void AddCommandForMenuItem(IMenuCommandService mcs, uint cmdid, EventHandler handler)
        {
            mcs.AddCommand(new MenuCommand(handler, new CommandID(Guids.GuidEndOfTheLineCmdSet,
                (int) cmdid)));
        }

        private void MakeLineEndingsCrLf(object sender, EventArgs e)
        {
            LineEndingCommands.ReplaceLineEndingsInActiveDocument(GetActiveTextDocument(), "\r\n");
        }

        private void MakeLineEndingsLf(object sender, EventArgs e)
        {
            LineEndingCommands.ReplaceLineEndingsInActiveDocument(GetActiveTextDocument(), "\n");
        }

        private TextDocument GetActiveTextDocument()
        {
            var dte = (DTE)GetService(typeof(DTE));
            var doc = dte?.ActiveDocument;

            // The only valid data model type identifiers are "Document" and "TextDocument".
            return doc?.Object("TextDocument") as TextDocument;
        }
    }
}
