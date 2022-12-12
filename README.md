# Prerequisites
*  Visual Studio 2022 Community (development environment) - https://www.visualstudio.com/downloads/
*  ActiViz OpenSource Edition 5.8.0 (64-bit Windows XP or later) (3D library used for graphics - must be installed only on the development PC; users do not need it) - https://prepomax.fs.um.si/downloads/

# PrePoMax Visual Studio setup
*  Download a Master branch of the PrePoMax package and extract it to a PrePoMax folder
*  Open the solution: "PrePoMax\PrePoMax.sln"

# Recreate the references to the VTK library
First delete the two existing references to the VTK library:
*  Open Solution Explorer: View->Solution Explorer
*  In Solution Explorer find the vtkControl project and then find the References branch
*  Delete the references starting with Kitware. (right click and Delete).
 
Secondly add the two references again using the folowing procedure:
*  Right click on References from vtkControl project in the Solution Explorer Window and selected Add Reference
*  A Reference manager window opens where you select Browse on the left side and then press the Browse button on the bottom right
*  Browse for the file Kitware.VTK.dll which should be in the ActiViz installation folder: "C:\Program Files\ActiViz.NET 5.8.0 OpenSource Edition\bin\Kitware.VTK.dll"
*  Repeat the procedure for the file Kitware.mummy.Runtime.dll: "C:\Program Files\ActiViz.NET 5.8.0 OpenSource Edition\bin\Kitware.mummy.Runtime.dll"

At last change the active solution platform using the main menu: **Build** -> **Configuration Manager** and select **x64** as the **Active solution platform**.

Start the compilation and execution of the project by pressing the Start button...

Compiling PrePoMax only creates some of its subfolders and default settings are prepared. To fully use a compiled version of PrePoMax, first look at the latest released version of the PrePoMax’s base folder. Then copy all folders that are missing in the compiled version from the released version (Models, NetGen, Solver…). Then you have to set the working folder and solvers (CalculiX) executables file name in the Settings->Calculix.

# Structure

The PrePoMax is a solution which consists of 9 projects:
*  CaeGlobals: global classes for all other projects to use
*  CaeJob: classes for running the analysis
*  CaeMesh: classes for FE mesh: nodes, elements, sets, ...
*  CaeModel: classes for FE model. Model contains FE mesh + materials, sections, ...
*  CaeResults: classes for FE results. Results contain FE mesh + field outputs, ...
*  PrePoMax: classes for user interface
*  STL: classes for stl geometry import
*  UserControls: classes for more complex user controls, as model tree view...
*  vtkControl: classes for 3D visualization

PrePoMax is compiled in an exe file all the other projects are compiled in dll files.

The internal structure of the program is quite complex and there are almost no comments in the code (no time to write them) but I am using very descriptive names. There are also some simple classes. Each class has its own file with .cs extension. You can browse the files in the Solution Explorer.

The PrePoMax project has a Forms folder and inside it is a FrmMain.sc file/class. This is the main form. The form communicates exclusively with the Controller.sc file/class which holds all data about the model. The program records all user actions in order to be able to repeat them later (while running PrePoMax select Edit -> Regenerate) so all needed user functions/subroutines are not called directly but via Commands. There is a special Command class for each user action...
