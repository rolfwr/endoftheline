End of the Line
===============

_End of the Line_ is a Visual Studio extension that shows end of line markers
in text editor allowing users to differentiate between CRLF and LF line
endings.

Visual Studio provides the Edit ▸ Advanced ▸ View White Space (Ctrl+R, Ctrl+W)
option to visualize spaces and tabs. Unfortunately this option does not
visualize line break characters such as carrage returns (CR) and line feeds
(LF). The _End of the Line_ is a Visual Studio extension fixes this. When used
it will CR and LF characters as ¤ and ¶ respectively using the same font style
as Visual Studio displays spaces and tabs in.

After installing the extension be sure to enable "View White Space" (Ctrl+R,
Ctrl+W) to see the end of line markers in the text editor.

Sources of inconsistent line endings
------------------------------------

Have you ever wondered why Visual Studio starts showing the
_Inconsistent Line Endings_ when you open a file, even though you've only ever
used Visual Studio to edit the file? Detecting where these inconsistent line
endings originate from is a lot easier when you're able to see the line
endings while editing the file. Common ways in which inconsisten line endings
gets introduced is:

* Copying and pasting code from a file with different line endings.
* Using Visual Studio extensions that always assumes CRLF endings to reformat regions of code.

End of the Line 1.1
-------------------

Features:
* Only show EOL markers when show whitespace is active.
* Support VS2012 in addition to VS2013.

Bug fixes:
* EOL marker on first view line vanished when editing second line.

End of The Line 1.0
-------------------

* Initial release

License
-------

MIT

Authors
-------

Contributors in the order of first contribution
* [Rolf W. Rasmussen](https://bitbucket.org/rolfwr)
* [Matt Ellis](https://bitbucket.org/citizenmatt)
