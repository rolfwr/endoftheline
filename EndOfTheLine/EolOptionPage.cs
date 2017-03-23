using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.VisualStudio.Shell;

namespace EndOfTheLine
{
    [Guid("17892819-0418-4B93-B5F0-AD083984E498")]
    public class EolOptionPage : DialogPage, IEolOptions
    {
        public VisibilityPolicy Visibility { get; set; }
        public EndingRepresentationStyle Style { get; set; }
        public EndingColorStyle Color { get; set; }
        public bool WhenCrlf { get; set; }
        public bool WhenLf { get; set; }
        public bool WhenOther { get; set; }
        public event EventHandler OptionChanged;

        public EolOptionPage()
        {
            WhenLf = true;
            WhenOther = true;
        }

        protected override IWin32Window Window => new ElementHost
        {
            Dock = DockStyle.Fill,
            Child = new EolOptionsUI(this)
        };

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            OptionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
