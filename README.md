End of the Line
===============

_End of the Line_ is a Visual Studio extension that shows end of line markers
in text editor allowing users to differentiate between CR LF and LF line
endings.

Visual Studio provides the Edit ▸ Advanced ▸ View White Space (Ctrl+R, Ctrl+W)
option to visualize spaces and tabs. Unfortunately this option does not
visualize line break characters such as carrage returns (CR) and line feeds
(LF). The _End of the Line_ is a Visual Studio extension fixes this. When used
it will CR and LF characters as ¤ and ¶ respectively using the same font style
as Visual Studio displays spaces and tabs in.

After installing the extension be sure to enable "View White Space" (Ctrl+R,
Ctrl+W) to see the end of line markers in the text editor or specify when you
wish line endings to be visible in Tools ▸ Options... ▸ End of the Line. If
you encounter any difficulties take a look at the
[troubleshooting guide](https://bitbucket.org/rolfwr/endoftheline/src/default/troubleshooting.md).


You can use Edit ▸ Advanced ▸ Make line endings CR LF (Ctrl+R, Ctrl+C), or
Edit ▸ Advanced ▸ Make line endings LF (Ctrl+R, Ctrl+L) to change the line
endings for the document as a whole or for the text selection if any.

Links
-----

* [Source code on Bitbucket](https://bitbucket.org/rolfwr/endoftheline)
* [Extension gallery download](https://visualstudiogallery.msdn.microsoft.com/545e56a7-98d7-47f9-9d84-4681f2903060)

Sources of inconsistent line endings
------------------------------------

Have you ever wondered why Visual Studio starts showing the
_Inconsistent Line Endings_ when you open a file, even though you've only ever
used Visual Studio to edit the file? Detecting where these inconsistent line
endings originate from is a lot easier when you're able to see the line
endings while editing the file. Common ways in which inconsistent line endings
gets introduced is:

* Copying and pasting code from a file with different line endings.
* Using Visual Studio extensions that always assumes CR LF endings to reformat
  regions of code.

Release history
---------------

### End of the line 1.5 BETA (No release. For VS 2017)

Features:

* Support VS2017 (15.0) (Not tested on older versions of Visual Studio).

### End of the line 1.4

Features:

* Add Options page for configuring visibility policy and line ending
  representation.

### End of the line 1.3

Features:

* Support VS2015 (14.0).

Bug fixes:

* EOL markers sometimes vanished when part of compound editing operation.

### End of the Line 1.2

Features:

* Editing commands for changing line endings.

### End of the Line 1.1

Features:

* Only show EOL markers when show whitespace is active.
* Support VS2012 in addition to VS2013.

Bug fixes:

* EOL marker on first view line vanished when editing second line.

### End of The Line 1.0

* Initial release

License
-------

[MIT](https://bitbucket.org/rolfwr/endoftheline/raw/default/EndOfTheLine/license.txt)

Authors
-------

Contributors in the order of first contribution

* [Rolf W. Rasmussen](https://bitbucket.org/rolfwr)
* [Matt Ellis](https://bitbucket.org/citizenmatt)

Useful tools and tricks
-----------------------

### Using text search to find inconsistent line endings

If you just want to locate places where files change from one line ending type
to another, or you want to quickly check all the files in the whole solution,
you can do a text search using the following regular expression:

    \r\n.*[^\r]\n|[^\r]\n.*\r\n

For example, to find the locations of all line ending consistencies in the
solution, do the following:

1. Press Ctrl+Shift+F (Edit ▸ Find and Replace ▸ Find in Files)
2. Enter `\r\n.*[^\r]\n|[^\r]\n.*\r\n` into the "Find what:" edit box.
3. Turn on "Find options" ▸ "Use Regular Expressions".
4. Press the "Find All" button.

This will give you a find results window containing something like this:

    Find all "\r\n.*[^\r]\n|[^\r]\n.*\r\n", Regular expressions, Subfolders, Find Results 1, Entire Solution, ""
      C:\...\ConsoleApplication1\Other.cs(1):using System;
      C:\...\ConsoleApplication1\Other.cs(2):using System.Collections.Generic;
      C:\...\ConsoleApplication1\Program.cs(9):    class Program
      C:\...\Program.cs(11):        static void Main(string[] args)
      Matching lines: 4    Matching files: 2    Total files searched: 4

You can also use the same regular expression to find inconsistencies within a
single file.

### Change line endings automatically when saving files

Here are two Visual Studio extensions that provide the ability to change line
endings on the fly when saving files:

1. Strip'em
    * [Project page](http://www.grebulon.com/software/stripem.php)
    * [Source code](http://www.grebulon.com/software/stripem.php#download)
2. Editor Config
    * [Project page](http://editorconfig.org/)
    * [Source code](https://github.com/editorconfig/editorconfig-visualstudio)
    * [Extension gallery page](https://visualstudiogallery.msdn.microsoft.com/c8bccfe2-650c-4b42-bc5c-845e21f96328)

I (Rolf) have used Editor Config myself in the past to keep source code trees
consistent across different platforms. I have no experience with Strip'em.
