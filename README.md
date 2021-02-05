**Disclaimer:** This project is very, VERY, work in progress and is also a test ground for my own experimentation with code, so expect some messiness at this time. I actively work on this project and intend to update it on Github semi-regularly, so expect significant changes over time as well.
# Data Editor App
The purpose of this project is for me to gain practical experience writing a graphical application of my own design from scratch. My intent for this application is to create a simple data editor that will allow the user to view data sets, make their own, and then submit their data to be stored in either a file, database server, or embedded database. Additionally, I decided that I want this application to be deployable cross-platform and rely as much as possible on open source software. While I am currently focusing on this as a desktop app, I plan to eventually adapt it to a mobile app to get mobile app development experience.

This project is largely for educational purposes, so feel free to look around the code or even download it and try to run it yourself. To run the project, you will need to download the Mono version of Godot 3.2 from [GodotEngine.org](https://godotengine.org/) as well as the listed prerequisites. 

## App Feature Goals
The following is a list of my target features for this project and the status of each:
- Table UI element for displaying and editing data - **In Progress**, currently cannot save data that is entered, but otherwise works
  - Future goal: reimplement table UI as custom Godot node with custom drawing code. Current version is node heavy and inefficient. Also format is unappealing.
- Text viewer so user can view serialized versions of data objects - **Complete**
- Allow user to select file to save serialized data to and load data from - **In Progress**, currently can enter file name to save and load from. 
- Embedded file manager so the user can visually browse and edit files - **In Progress**, see sub-points for details, Code: [app/filesystem](app/filesystem)
  - Backing data structure for tracking user opened directories and synchronization of filesystem state with OS - **complete** Code: [FSViewTree.cs](app/filesystem/FSViewTree.cs)
  - UI element for displaying directory tree - **complete** Code: [FileSystem.cs](app/filesystem/FileSystem.cs)/[FileSystem.tscn](app/filesystem/FileSystem.tscn)
  - UI for directory navigation - **In Progress**
  - Backend for file operations - **In Progress**
  - UI for file operations - **In Progress**
  - Threading strategy to delegate file operations to separate thread(s) - **Complete**, I implemented 3 options for my scenario: a dedicated worker thread (using System.Threading), a queue based system using Task.ContinueWith(), and a non-queue based system that uses plain Tasks and mutexes to ensure critical code sections can't interfere with eachother. I encaspulated the first two options in classes found in [library/Threading](library/Threading).
- Backend for database connections - **Not Started**
- UI for database selector - **Not Started**
- UI for database viewer - **Not Started**
- UI for editing database entries (tables, etc), loading entries, and saving back to the database - **Not Started**

## Technology Stack
For this project I chose C#/.Net (Mono) as my primary platform, since I personally love C# and .Net and want to gain more experience with it. To the end of making a cross platform GUI app, however, I found my options with .Net to be fairly disappointing. After a lot of research, I decided to use a game engine for my UI as well as some cross platform functionality. The game engine I chose for this project is Godot, since it is robust, small, and open source. I could have also used Unity, but Unity feels bloated for this scale of project and is proprietary. 

My decision to use Godot for the UI has had a strong impact on the architecture of this application. If you are viewing the project from a background in .Net development and haven't used Godot before, the application structure probably looks alien to you. Because I am using Godot, this application is actually a Godot application (which itself is a C++ application), *not* a .Net application. The Mono runtime is used by the Godot runtime to execute compiled C# code as scripts, but Godot doesn't actually care what the scripts are written in as long as the module used comforms to Godot's scripting API. 

In Godot, execution of user code actually starts in the scene tree. Scenes in Godot are collections of specialized objects called nodes. The structure of a scene is defined by its scene (.tscn) file, which tells Godot what nodes and resources to load when loading the scene. The process is similar to how a HTML file tells your browser what elements and resources to load when loading a web page. The combination of the built-in functionality of the different node types and scripts the user attaches to the nodes defines the behavior of the application.

The execution of my code in this project relies almost exclusively on initialization code (overriding the virtual `_ready()` function) and Godot signals (events), which allows Godot to suspend updating the UI when the user is not interacting with it. The UI itself is almost entirely hand-built using Godot's UI nodes (nodes that inherit from the Control class). 

Outside of Godot, I rely on Nuget packages for implementing common functionality. For example, I use Newtonsoft's Json.NET library for JSON parsing and serialization. I have also created a namespace for my personal non-application-specific code library, which I have dubbed SparkLib. 

## Directory Structure
- The `app` directory contains all of the application specific code and scene files. This is the meat of the program
- I intend to use `docs` for supporting documentation for the application as needed, but I haven't used it yet. Might just remove it
- `library` is where I am storing non-application specific code that I will potentially reuse accross applications
- `resources` is for storing resources and assets. For this application, that will mostly just include fonts, icons, and themes. Note that in here you will see a lot of `*.import` verions of files. Those are used my Godot to keep track of the import settings for each asset.
- `project.godot` is the file the defines this repository as a Godot project, and contains all relevant project-specific configuration settings 
