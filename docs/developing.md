Notes on developing End of the Line
===================================

Building
--------

All dependencies, including the Visual Studio SDK packages are pulled in using
NuGet. Simply build the solution from within Visual Studio while having
internet access to pull in all the dependencies.

Debugging
---------

To run an instance of Visual Studio with the built plugin in the debugger, add
the following values to the "Debug" page of the EndOfTheLine properties:

* Start external program: `C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe`
* Command line arguments: `/rootsuffix Exp`

Replace the program path with the path of the devenv.exe executable for the
version of Visual Studio you wish to debug.
