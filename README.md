**Disclaimer:** This project is very, VERY, work in progress and is also a test ground for my own experimentation with code, so expect some messiness at this time. I actively work on this project and intend to update it on Github semi-regularly, so expect significant changes over time as well.
# Data Editor App
## Overview
The purpose of this project is for me to gain practical experience writing a graphical application of my own design from scratch. My intent for this application is to create a simple data editor that will allow the user to view data sets, make their own, and then submit their data to be stored in either a file, database server, or embedded database. Additionally, I decided that I want this application to be deployable cross-platform and rely as much as possible on open source software as possible. While I am currently focusing on this as a desktop app, I plan to eventually plan on adapting it to a mobile app to get mobile app development experience.

## App Feature Goals
The following is a list of my target features for this project and the status of each:
- Implement table UI element for displaying and editing data - **In Progress**, currently cannot save data that is entered, but otherwise works
  - Future goal: reimplement table UI as custom Godot node with custom drawing code. Current version is node heavy and inefficient. Also format is unappealing.
- Implement text viewer so user can view serialized versions of data objects - **Complete**
- Allow user to select file to save serialized data to and load data from - **In Progress**, currently can enter file name to save and load from. 
- Implement embedded file manager so the user can visually browse and edit files - **In Progress**, see sub-points for details [See app/filesystem](app/filesystem)
  - Implement backing data structure for tracking user opened directories and synchronization of filesystem state with OS - **complete** [See FSViewTree.cs](app/filesystem/FSViewTree.cs)
