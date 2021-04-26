using Godot;
using System;
using System.Collections.Generic;

public class FileSystemList : Tree
{
    private FileSystem? fileSystem;
    private bool isDropValid = false;
    private FSViewTree.DirNode? targetNode = null;

    public override void _Ready()
    {
        fileSystem = Owner as FileSystem;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    // The set of override functions to implement drag and drop in Godot
    public override object GetDragData(Vector2 position)
    {
        Godot.Collections.Array<FSViewTree.Node> selection = new Godot.Collections.Array<FSViewTree.Node>(fileSystem!.GetSelectedEntries());
        SetDragPreview(new Button());
        return selection;
    }

    public override bool CanDropData(Vector2 position, object data)
    {
        // Make sure the user is hovering over an item
        TreeItem? treeTarget = GetItemAtPosition(position) ?? null;
        if (treeTarget == null)
        {
            isDropValid = false;
            return false;
        }

        // Make sure the data itself is correct
        if (data == null
            || (data is Godot.Collections.Array) == false
            || (data as Godot.Collections.Array)!.Count == 0
            || ((data as Godot.Collections.Array)![0] is FSViewTree.Node) == false)
        {
            isDropValid = false;
            return false;
        }
        Godot.Collections.Array dataArray = (data as Godot.Collections.Array)!;

        // Make sure the selection does not include the target (not dropping a directory onto itself)
        FSViewTree.Node target = fileSystem!.GetFSAssocList()[treeTarget];
        foreach (FSViewTree.Node node in dataArray)
        {
            if (node == null) continue;
            if (target.path!.Equals(node.path))
            {
                isDropValid = false;
                return false;
            }
        }

        // Make sure the target directory is not a child of any of the selected directories
        FSViewTree.Node? parent = target.parent;
        while (parent != null)
        {
            // WARNING!! A dirty array will crash the program!
            foreach (FSViewTree.Node node in dataArray)
            {
                if (node == null) continue;
                if (node.path!.Equals(parent.path))
                {
                    isDropValid = false;
                    return false;
                }
            }
            parent = parent.parent;
        }

        // Finally, if it is a DirNode, we know we can drop
        if (target is FSViewTree.DirNode)
        {
            targetNode = target as FSViewTree.DirNode;
            isDropValid = true;
            return true;
        }

        isDropValid = false;
        return false;
    }

    public override void DropData(Vector2 position, object data)
    {
        /*
        TreeItem? treeTarget = GetItemAtPosition(position) ?? null;
        if (treeTarget == null) return;

        if (data == null
            || (data is Godot.Collections.Array) == false
            || (data as Godot.Collections.Array)!.Count == 0
            || ((data as Godot.Collections.Array)![0] is FSViewTree.Node) == false) return;
        */
        if (isDropValid == false) return;
        Godot.Collections.Array<FSViewTree.Node> dataArray = new Godot.Collections.Array<FSViewTree.Node>(data as Godot.Collections.Array);


        /*foreach (object item in (data as Godot.Collections.Array)!)
        {
            if (item is FSViewTree.Node) dataArray.Add((item as FSViewTree.Node)!);
        }*/
        if (targetNode == null) return;
        fileSystem!.Move(dataArray, targetNode);
        targetNode = null;
        fileSystem.RefreshFileSystemQueued();
    }
}
