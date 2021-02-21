using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SparkLib;

public partial class FileSystem : Panel
{
    // handling file operations here

    public void Copy()
    {

    }

    public void Delete(FSViewTree.DirNode target)
    {

    }

    public void Delete(FSViewTree.FileNode target)
    {

    }

    public void Move(FSViewTree.DirNode source, FSViewTree.DirNode dest)
    {

    }

    public void Move(FSViewTree.FileNode source, FSViewTree.DirNode dest)
    {

    }

    public void Move(List<FSViewTree.Node> source, FSViewTree.DirNode dest)
    {
        if (source.Count == 0) return;
        foreach (FSViewTree.Node item in source)
        {
            if (item is FSViewTree.DirNode)
            {
                // Access item.thisDir to move it
                FSViewTree.DirNode temp = (item as FSViewTree.DirNode)!;
                temp.thisDir.MoveTo($"{dest.path}/{temp.name}");
            }
            else if (item is FSViewTree.FileNode)
            {
                // Access item.thisFile to move it
                FSViewTree.FileNode temp = (item as FSViewTree.FileNode)!;
                temp.thisFile.MoveTo($"{dest.path}/{temp.name}");
            }
        }
        RefreshFileSystem();
        return;
    }

    public void Move(Godot.Collections.Array<FSViewTree.Node> source, FSViewTree.DirNode dest)
    {
        if (source.Count == 0) return;
        foreach (FSViewTree.Node item in source)
        {
            if (item is FSViewTree.DirNode)
            {
                // Access item.thisDir to move it
                FSViewTree.DirNode temp = (item as FSViewTree.DirNode)!;
                temp.thisDir.MoveTo($"{dest.path}/{temp.name}");
            }
            else if (item is FSViewTree.FileNode)
            {
                // Access item.thisFile to move it
                FSViewTree.FileNode temp = (item as FSViewTree.FileNode)!;
                temp.thisFile.MoveTo($"{dest.path}/{temp.name}");
            }
        }
        RefreshFileSystem();
        return;
    }

    public void Rename(FSViewTree.FileNode target, string name)
    {

    }

    public void Rename(FSViewTree.DirNode target, string name)
    {

    }

    public void CreateFile(FSViewTree.DirNode location, string name)
    {

    }

    public void CreateFolder(FSViewTree.DirNode location, string name)
    {

    }
}