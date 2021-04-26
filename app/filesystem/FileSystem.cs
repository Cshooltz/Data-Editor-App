using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SparkLib;

public partial class FileSystem : Panel
{
    /*
        Custom FileSystem Control for Godot.
        Used for browsing and operating on a local working directory in the OS filesystem.

        ToDo:
            User selectable root folder
            Implement file operations
                Enable de-select of entries when clicking on background
                Resize context menu to fit entries
            Automatic refresh on filesystem changes (only when main window is back in focus)
            Implement file/folder sorting.
    */
    protected bool userEditable = false;
    public FSViewTree? userWorkingTree;
    public Tree? FileSystemListNode;
    protected PopupMenu? ContextMenuNode;
    protected LineEdit? FilePathNode;
    public string path = "user://";
    protected System.Collections.Generic.Dictionary<TreeItem, FSViewTree.Node> FSAssocList = new();

    public System.Collections.Generic.Dictionary<TreeItem, FSViewTree.Node> GetFSAssocList()
    {
        return FSAssocList;
    }

    // Instead of using a queue, opting to throw out duplicate requests if a request is already running.
    //private WorkQueueThread workerThread = new();
    //private WorkQueueTask workerTask = new();

    protected FSViewTree.Node[] clipBoard = new FSViewTree.Node[0];

    public override void _Ready()
    {
        FileSystemListNode = GetNode<Tree>("FileSystemScroll/FileSystemList");
        ContextMenuNode = GetNode<PopupMenu>("ContextMenu");
        FilePathNode = GetNode<LineEdit>("HBoxContainer/FilePath");
        userWorkingTree = new FSViewTree(path);
        FilePathNode.Text = userWorkingTree.userRootDir!.path;
        // This is a best example of how these classes should be used
        // Just run blocking operations in their own threads. 
        /*workerThread.EnqueueWork(() =>
        {
            RefreshFileSystem();
        });*/
        Task.Run(RefreshFileSystem);
    }

    public FileSystem()
    {

    }

    bool isRefreshing = false;
    EventWaitHandle refreshEvent = new(true, EventResetMode.ManualReset);
    Task RefreshTask;
    /// <summary>
    /// Refreshes the file system GUI. Is synchronous by default.
    /// refreshEvent and isRefreshing are state variables
    /// to help control when RefreshFileSystem is allowed to run.
    /// </summary>
    internal void RefreshFileSystem()
    {
        //if (isRefreshing) return; // Going to delegate this responsibility to functions that call this one
        refreshEvent.Reset();
        isRefreshing = true;

        userWorkingTree!.RefreshDirectories();
        UpdateTree();

        isRefreshing = false;
        refreshEvent.Set();
    }

    internal void RefreshFileSystemQueued()
    {
        Task.Run(() =>
        {
            refreshEvent.WaitOne();
            RefreshFileSystem();
        });
    }

    internal void RefreshFileSystemDiscard()
    {
        if (isRefreshing) return;
        Task.Run(RefreshFileSystem);
    }

    bool isUpdating = false;
    public void UpdateTree()
    {
        //if (isUpdating) return;

        userEditable = false;
        isUpdating = true;

        userWorkingTree!.FSLock.WaitOne();

        FileSystemListNode!.Clear();
        FSAssocList.Clear();
        TreeItem treeRoot;
        treeRoot = FileSystemListNode.CreateItem();
        treeRoot.SetText(0, userWorkingTree!.userRootDir!.name + "/");
        FSAssocList.Add(treeRoot, userWorkingTree.userRootDir);

        TreeItem workingTreeItem = treeRoot;
        FSViewTree.DirNode workingDirNode = userWorkingTree.userRootDir;
        Stack<int[]> counters = new();
        Stack<TreeItem> treeItemStack = new();
        int[] currentCounter = { 0, workingDirNode.folders.Count - 1, 0 }; // [0] = current index, [1] = final index, [2] = files processed
        bool scanning = true;

        while (scanning)
        {

            if (currentCounter[0] <= currentCounter[1])
            {
                FSViewTree.DirNode currentFolder = workingDirNode.folders[currentCounter[0]];
                TreeItem treeBuffer = FileSystemListNode.CreateItem(workingTreeItem);
                treeBuffer.SetText(0, currentFolder.name + "/");
                FSAssocList.Add(treeBuffer, currentFolder);
                if (currentFolder.isOpen) treeBuffer.Collapsed = false;
                else treeBuffer.Collapsed = true;

                if (currentFolder.folders.Count > 0 || currentFolder.files.Count > 0)
                {
                    counters.Push(currentCounter);
                    treeItemStack.Push(workingTreeItem);
                    workingTreeItem = treeBuffer;
                    workingDirNode = currentFolder;
                    currentCounter = new int[] { 0, workingDirNode.folders.Count - 1 };
                    continue;
                }
            }

            if (currentCounter[0] >= currentCounter[1])
            {
                foreach (FSViewTree.FileNode file in workingDirNode.files)
                {
                    TreeItem treeBuffer = FileSystemListNode.CreateItem(workingTreeItem);
                    treeBuffer.SetText(0, file.name);
                    FSAssocList.Add(treeBuffer, file);
                }
            }

            if ((currentCounter[0] >= currentCounter[1]) && workingDirNode.parent == null)
            {
                scanning = false;
            }
            else if ((currentCounter[0] >= currentCounter[1]) && workingDirNode.parent != null)
            {
                currentCounter = counters.Pop();
                workingDirNode = workingDirNode.parent;
                workingTreeItem = treeItemStack.Pop();
            }
            currentCounter[0]++;
        }

        if ((currentCounter[0] >= currentCounter[1]) && workingDirNode.parent == null)
        {
            scanning = false;
        }
        else if ((currentCounter[0] >= currentCounter[1]) && workingDirNode.parent != null)
        {
            currentCounter = counters.Pop();
            workingDirNode = workingDirNode.parent;
            workingTreeItem = treeItemStack.Pop();
        }
        currentCounter[0]++;
        userEditable = true;
        isUpdating = false;
        userWorkingTree!.FSLock.ReleaseMutex();
        return;
    }


    public string? GetSelectedPath()
    {
        TreeItem selection = FileSystemListNode!.GetSelected();
        return FSAssocList[selection].path;
    }

    protected void _OnFileSystemListGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent /*&& mouseEvent.Pressed*/)
        {
            /*
            switch ((ButtonList)mouseEvent.ButtonIndex)
            {
                case ButtonList.Right:
                    GD.Print("Right mouse button was pressed.");
                    break;
            }
            */
            if (Input.IsActionJustReleased("mouse_right_click"))
            {
                //GD.Print("Right mouse button was released.");
                ShowContextMenu(mouseEvent.GlobalPosition);
            }
        }

        return;
    }

    protected void _OnFileSystemListNothingSelected()
    {
        FileSystemListNode!.GetRoot().CallRecursive("deselect", 0);
    }

    protected void ShowContextMenu(Vector2 globalPosition)
    {
        PopulateContextMenu();
        Vector2 view = GetViewport().GetVisibleRect().Size;
        Vector2 mousePos = globalPosition;
        Vector2 contextSize = ContextMenuNode!.RectSize;
        Vector2 contextPos = new(mousePos);
        if (mousePos.x + contextSize.x > view.x)
        {
            contextPos.x = view.x - contextSize.x;
        }
        else if (mousePos.x < 0)
        {
            contextPos.x = 0;
        }
        if (mousePos.y + contextSize.y > view.y)
        {
            contextPos.y = view.y - contextSize.y;
        }
        else if (mousePos.y < 0)
        {
            mousePos.y = 0;
        }
        ContextMenuNode!.SetGlobalPosition(contextPos);
        ContextMenuNode!.Show();
        ContextMenuNode!.GrabFocus();
    }

    protected System.Collections.Generic.Dictionary<int, FileOperations> contextMenuAssocList = new();

    public enum FileOperations
    {
        NewFolder, NewFile, Rename, Delete, Move, Copy,
        Paste
    }

    protected void PopulateContextMenu()
    {
        ContextMenuNode!.Clear();
        ContextMenuNode.RectSize = new Vector2(ContextMenuNode.RectSize.x, 16);
        contextMenuAssocList.Clear();
        List<FSViewTree.Node> selectedEntries = GetSelectedEntries();
        //bool containsFiles = false;
        //bool containsFolders = false;
        int id = 0;
        if (selectedEntries.Count == 0)
        {
            // New file
            // New folder
            ContextMenuNode.AddItem("New Folder...", id);
            contextMenuAssocList.Add(id, FileOperations.NewFolder);
            id++;
            ContextMenuNode.AddItem("New File...", id);
            contextMenuAssocList.Add(id, FileOperations.NewFile);
        }
        else if (selectedEntries.Count == 1)
        {
            // New File
            // New Folder
            // Rename
            // Delete
            ContextMenuNode.AddItem("Copy", id);
            contextMenuAssocList.Add(id, FileOperations.Copy);
            id++;
            if (clipBoard.Length > 0 && selectedEntries[0] is FSViewTree.DirNode)
            {
                if (clipBoard.Length > 1) ContextMenuNode.AddItem("Paste Items", id);
                else ContextMenuNode.AddItem("Paste", id);
                contextMenuAssocList.Add(id, FileOperations.Paste);
                id++;
            }
            ContextMenuNode.AddItem("Rename", id);
            contextMenuAssocList.Add(id, FileOperations.Rename);
            id++;
            ContextMenuNode.AddItem("Delete", id);
            contextMenuAssocList.Add(id, FileOperations.Delete);
            id++;
            ContextMenuNode.AddItem("New Folder...", id);
            contextMenuAssocList.Add(id, FileOperations.NewFolder);
            id++;
            ContextMenuNode.AddItem("New File...", id);
            contextMenuAssocList.Add(id, FileOperations.NewFile);
        }
        else
        {
            ContextMenuNode.AddItem("Copy Items", id);
            contextMenuAssocList.Add(id, FileOperations.Copy);
            id++;
            // Maybe make a default case later where paste defaults to the root space
            /*
            if (clipBoard.Length > 0)
            {
                if (clipBoard.Length > 1) ContextMenuNode.AddItem("Paste Items", id);
                else ContextMenuNode.AddItem("Paste", id);
                contextMenuAssocList.Add(id, FileOperations.Paste);
                id++;
            }
            */
            ContextMenuNode.AddItem("Delete Items", id);
            contextMenuAssocList.Add(id, FileOperations.Delete);
        }
    }

    public List<FSViewTree.Node> GetSelectedEntries()
    {
        List<FSViewTree.Node> selectedEntries = new();
        TreeItem? nextSelected = null;
        List<TreeItem> selectedTreeItems = new();
        bool continueScan = true;
        while (continueScan)
        {
            nextSelected = FileSystemListNode!.GetNextSelected(nextSelected);
            if (nextSelected == null)
            {
                continueScan = false;
            }
            else
            {
                selectedTreeItems.Add(nextSelected);
            }
        }

        foreach (TreeItem item in selectedTreeItems)
        {
            selectedEntries.Add(FSAssocList[item]);
        }

        return selectedEntries;
    }

    protected void _OnContextMenuIDPressed(int id)
    {
        FileOperations selectedOp = contextMenuAssocList[id];
        List<FSViewTree.Node> selection = GetSelectedEntries(); // TODO: Save wasted work by only updating selection when needed
        switch (selectedOp)
        {
            case (FileOperations.Copy):
                clipBoard = selection.ToArray();
                break;
            case (FileOperations.Paste):
                foreach (FSViewTree.Node item in clipBoard)
                {
                    if (item is FSViewTree.DirNode)
                    {
                        Copy((FSViewTree.DirNode)item, (FSViewTree.DirNode)selection[0]); // TODO: make it not dangerous
                    }
                    else if (item is FSViewTree.FileNode)
                    {
                        Copy((FSViewTree.FileNode)item, (FSViewTree.DirNode)selection[0]);
                    }
                }
                break;
            case (FileOperations.Move):
                break;
            case (FileOperations.Rename):
                break;
            case (FileOperations.Delete):
                foreach (FSViewTree.Node item in selection)
                {
                    // TODO: Add prompt and implement SoftDelete!
                    if (item.type == FSViewTree.NodeType.folder)
                    {
                        HardDelete((FSViewTree.DirNode)item);
                    }
                    else if (item.type == FSViewTree.NodeType.file)
                    {
                        HardDelete((FSViewTree.FileNode)item);
                    }
                }
                break;
            case (FileOperations.NewFile):
                break;
            case (FileOperations.NewFolder):
                break;
            default:
                break;
        }
        RefreshFileSystemQueued();
    }

    protected void _OnContextMenuPopupHide()
    {

    }

    bool FSCollapseRunning = false;
    protected void _OnFileSystemListItemCollapsed(TreeItem item)
    {
        if (!userEditable) return; // This is important because the signal is fired when the tree is being built.
                                   //if (FSCollapseRunning) return;
        FSCollapseRunning = true;
        userEditable = false;

        Task.Run(() =>
       {
           // If the user collapses a folder,
           // we can only update AFTER the latest refresh
           refreshEvent.WaitOne();
           FSViewTree.DirNode? fsNode = FSAssocList[item] as FSViewTree.DirNode;
           if (fsNode!.parent != null)
           {
               if (item.Collapsed == true)
               {
                   userWorkingTree!.CloseDirectory(fsNode);
               }
               else
               {
                   userWorkingTree!.OpenDirectory(fsNode);
               }
               RefreshFileSystem();
           }
           FSCollapseRunning = false;
           return;
       });
    }

    protected void _OnRefreshButtonPressed()
    {
        RefreshFileSystemDiscard();
    }
}