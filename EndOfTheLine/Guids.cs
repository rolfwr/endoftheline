using System;

namespace EndOfTheLine
{
    static class Guids
    {
        /// <summary>
        /// The identity of the whole Visual Studio package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// GUID value matches /PackageManifest/MetaData/Identity/@Id in the
        /// source.extension.vsixmanfiest file.
        /// </para>
        /// <para>
        /// GUID value matches
        /// /CommandTable/Symbols/GuidSymbol[@name="guidEndOfTheLinePkg"]/@value
        /// in the EndOfTheLine.vsct file.
        /// </para>
        /// <para>
        /// The EndOfTheLinePackage class is annotated with a Guid attribute
        /// with the GUID value from this field.
        /// </para>
        /// </remarks>
        internal const string GuidEndOfTheLinePkgString = "44e069cf-8802-4347-bbad-5e48b881efe0";


        /// <summary>
        /// The identity of the group of commands we place all our commands under.
        /// </summary>
        /// <remarks>
        /// <para>
        /// GUID value matches
        /// /CommandTable/Symbols/GuidSymbol[@name="guidEndOfTheLineCmdSet"]/@value
        /// in the EndOfTheLine.vsct file.
        /// </para>
        /// <para>
        /// This field is used when calling IMenuCommandService.AddCommand()
        /// from the initialization code of the EndOfTheLinePackage class.
        /// </para>
        /// </remarks>
        const string GuidEndOfTheLineCmdSetString = "e22c45ad-8059-460f-bee4-2c55f964781a";

        internal static readonly Guid GuidEndOfTheLineCmdSet = new Guid(GuidEndOfTheLineCmdSetString);
    }
}
