## Troubleshooting missing end of line markers
### Restarting Visual Studio
If you’ve just installed the extension, you will need to close and reopen
Visual Studio for it to see the extension.  You can check whether Visual
Studio has successfully loaded the extension by going to Help ▸ About Microsoft
Visual Studio. In the “Installed products” list you should see a product named
“End of the Line”.

### Turn on View Whitespace option
Make sure that you’ve turned on on the “View Whitespace” option. Normally, the
keyboard shortcut Ctrl+R, Ctrl+W should toggle this option on and off, but it
is possible for this keyboard shortcut to be remapped. To verify that you’ve
turned on “View Whitespace” open the submenu Edit ▸ Advanced, and look at the
icon next to the “View Whitespace” menu item that look like “a·b” is
highlighted and has a border around it. Note that the Edit ▸ Advanced only
show up when you are editing a text file of some sort in Visual Studio.

### Check End of the Line Options
Make sure the “Line endings visible” option is not set to “Never”. The default
setting is to display line endings when other whitespace is visible, however it
it is possible to set this option to never show any line endings. To verify
that this option has the default setting, go look at Tools ▸ Options... ▸
End of the Line ▸ Line endings visible. The “When other whitespace is visible”
option should be active.

### Check color theme and color settings
Make sure that the Visual Studio color theme and color settings assign a color
to the display item “Visible White Space” that differs from the background
color. This is not a problem for the regular “Blue”, “Dark” and “Light” color
themes, but custom color themes or settings may potentially make the whitespace
characters the same color as the background, thus making them invisible. To
check if this is an issue, write a simple text file containing a few spaces and
tab characters. If you see `·` characters where you type space, `→` characters
where you type space, and a `▯` at the end of the file, then you’re color
settings are set up correctly. It these characters do not show up, please
review your theme and color settings at Tools ▸ Options … ▸ Environment ▸
General ▸ Visual experience: Color Theme, and Tools ▸ Options … ▸ Environment ▸
Fonts and Colors ▸ Display items ▸ Visible White Space.

### Check the font
Make sure the font you’re using in the editor supports the `¤` and `¶`
characters. If you’re using a font other than the standard “Consolas” font,
then try switching back to the “Consolas” font to see if that makes any
difference. You can change the font at  Tools ▸ Options … ▸ Environment ▸
Fonts and Colors ▸ Font.

### Reporting the problem as a bug
If you’ve gone through all the preceding troubleshooting steps, but are still
not seeing any end of line characters, then you may have found a bug in the
“End of the Line” extension. Please report the problem as a bug, and provide
the following information:

1. Specify the results of each of the troubleshooting steps above.
2. Provide the list of installed products in Visual Studio. Go to Help ▸ About
   Microsoft Visual Studio, click the “Copy Info” button, and paste the text
   into a text file.
3. Provide a copy of file located at the
   _%APPDATA%_\Microsoft\VisualStudio\\_14.0_\ActivityLog.xml location. Here
   _%APPDATA%_ signifies the value of the APPDATA environment variable, which
   refers to the base location where applications store their data by default
   for the current user. The path segment “_14.0_” corresponds to the version
   of Visual Studio, which in this case is VS 2015. As a concrete example, on
   my machine the file is located at
   `C:\Users\RolfW\AppData\Roaming\Microsoft\VisualStudio\14.0\ActivityLog.xml`

The best place to report bugs is at https://bitbucket.org/rolfwr/endoftheline/issues/new
