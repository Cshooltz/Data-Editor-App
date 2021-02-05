**Disclaimer:** This project is very, VERY, work in progress and is also a test ground for my own experimentation with code, so expect some messiness at this time. I actively work on this project and intend to update it on Github semi-regularly, so expect significant changes over time as well.
# Data Editor App
## Overview
The purpose of this project is for me to gain practical experience writing a graphical application of my own design from scratch. My intent for this application is to create a simple data editor that will allow the user to view data sets, make their own, and then submit their data to be stored in either a file, database server, or embedded database. Additionally, I decided that I want this application to be deployable cross-platform and rely as much as possible on open source software as possible. While I am currently focusing on this as a desktop app, I plan to eventually plan on adapting it to a mobile app to get mobile app development experience.

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
- UI for database selector - **Not Started**
- UI for database viewer - **Not Started**
- UI for editing database entries (tables, etc), loading entries, and saving back to the database - **Not Started**
