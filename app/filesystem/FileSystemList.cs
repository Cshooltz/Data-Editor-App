using Godot;
using System;
using System.Collections.Generic;

public class FileSystemList : Tree
{
    FileSystem? fileSystem;

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
        TreeItem? treeTarget = GetItemAtPosition(position) ?? null;
        if (treeTarget == null) return false;

        if (data == null
            || (data is Godot.Collections.Array) == false
            || (data as Godot.Collections.Array)!.Count == 0
            || ((data as Godot.Collections.Array)![0] is FSViewTree.Node) == false) return false;
        //Godot.Collections.Array dataArray = (data as Godot.Collections.Array)!;

        FSViewTree.Node target = fileSystem!.GetFSAssocList()[treeTarget];
        if (target is FSViewTree.DirNode) return true;

        return false;
    }

    public override void DropData(Vector2 position, object data)
    {
        TreeItem? treeTarget = GetItemAtPosition(position) ?? null;
        if (treeTarget == null) return;

        if (data == null
            || (data is Godot.Collections.Array) == false
            || (data as Godot.Collections.Array)!.Count == 0
            || ((data as Godot.Collections.Array)![0] is FSViewTree.Node) == false) return;

        Godot.Collections.Array<FSViewTree.Node> dataArray = new Godot.Collections.Array<FSViewTree.Node>(data as Godot.Collections.Array);

        /*foreach (object item in (data as Godot.Collections.Array)!)
        {
            if (item is FSViewTree.Node) dataArray.Add((item as FSViewTree.Node)!);
        }*/

        fileSystem!.Move(dataArray, (fileSystem!.GetFSAssocList()[treeTarget] as FSViewTree.DirNode)!);
    }

    public class GodotBox<T> : Godot.Object
    {
        public T data;

        public GodotBox(T data)
        {
            this.data = data;
        }
    }
}
