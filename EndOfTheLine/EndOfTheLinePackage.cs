

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

            if (dte == null)
            {
                return null;
            }

            var doc = dte.ActiveDocument;

            if (doc == null)
            {
                return null;
            }

            // The only valid data model type identifiers are "Document" and "TextDocument".
            return doc.Object("TextDocument") as TextDocument;

        }
    }
}
