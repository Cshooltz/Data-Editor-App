using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json; // Full featured JSON serializer
using Newtonsoft.Json.Linq;

public class JSONInspector : Control
{

    ZooData ZooInfo = new ZooData();
    RichTextLabel? OutputNode;
    Directory FileDirectory = new Directory();
    LineEdit? UserPath;
    ItemList? DataObjectsList;
    DataTable? UserDataTable;
    FileSystem? FileSystemPanel;

    FSViewTree? TestFileSystem;

    public JSONInspector()
    {
        DefaultZooData();
    }

    public override void _Ready()
    {
        OutputNode = GetNode<RichTextLabel>("RightTabSet/JsonOutput/OutputText");
        UserPath = GetNode<LineEdit>("CenterControls/FilePath");
        FileSystemPanel = GetNode<FileSystem>("LeftTabSet/FileSystem");

        DataObjectsList = GetNode<ItemList>("CenterControls/DataObjectsList");
        DataObjectsList.AddItem("Zoos");
        DataObjectsList.AddItem("Animals");
        DataObjectsList.AddItem("ZooAnimals");

        UserDataTable = GetNode<DataTable>("RightTabSet/DataTable");
        //UserDataTable.LoadData(new string[] { "ID", "Name" }, new string[][] { new string[] { "1", "2" }, new string[] { "3", "4" } });
        UserDataTable.LoadData<Zoo>(ZooInfo.Zoos);

        //OutputNode.Text = ZooInfo.ToString();
        GD.Print($"Current Dir: {FileDirectory.GetCurrentDir()}");
        if (FileDirectory.Open("user://") == Error.Ok) GD.Print("user:// opened successfully");



        return;
    }

    public string LoadJsonFile()
    {
        File file = new File();
        string? path = FileSystemPanel!.GetSelectedPath();
        string buffer;
        Error result = file.Open(path, File.ModeFlags.Read);
        if (result == Error.Ok) buffer = file.GetAsText();
        else
        {
            GD.PrintErr($"JSONInspector.LoadJsonFile: Attempted to open file {path} and recieved error {result}");
            buffer = "";
        }
        file.Close();
        return buffer;
    }

    public void SaveJsonFile()
    {
        File file = new File();
        string path = GetEnteredPath();
        Error result = file.Open(path, File.ModeFlags.Write);
        if (result == Error.Ok) file.StoreString(OutputNode!.Text);
        else GD.PrintErr($"SaveJsonFile: Attempted to open file {path} and recieved error {result}");
        file.Close();
        return;
    }

    public string GetEnteredPath()
    {
        string FolderPath;
        string FileName;
        StringBuilder Path = new StringBuilder("");
        string PathBuffer = UserPath!.Text.ToLower();
        string[] SplitPath = PathBuffer.Split('/', 10, System.StringSplitOptions.RemoveEmptyEntries);
        if (!SplitPath[0].Equals("user:"))
        {
            GD.PrintErr($"User entered path of \"{PathBuffer}\" is invalid.");
            return "";
        }
        Path.Append(SplitPath[0] + "//");
        for (int i = 1; i < SplitPath.Length - 1; i++)
        {
            Path.Append(SplitPath[i] + "/");
        }
        FolderPath = Path.ToString();

        if (!FileDirectory.DirExists(FolderPath))
        {
            FileDirectory.MakeDirRecursive(FolderPath);
        }

        string Last = SplitPath[SplitPath.Length - 1];
        int Position = Last.Find(".");
        if (Position == -1)
        {
            FileName = Last + ".json";
        }
        else
        {
            FileName = Last;
        }
        Path.Append(FileName);

        //OutputNode!.Text = FolderPath + FileName;
        return Path.ToString();
    }

    protected void _OnClearTextPressed()
    {
        OutputNode!.Text = "";
    }

    protected void _OnConvertToJsonPressed()
    {
        Task.Run(() => OutputNode!.Text = ZooInfo.ToJson());
    }

    protected void _OnSaveToFilePressed()
    {
        SaveJsonFile();
    }

    protected void _OnLoadFromFilePressed()
    {
        OutputNode!.Text = LoadJsonFile();
        //GD.Print(FileSystemPanel!.GetSelectedPath());
    }

    protected void _OnLoadInTablePressed()
    {
        int[] selection = GetNode<ItemList>("CenterControls/DataObjectsList").GetSelectedItems();
    }

    protected void _on_CreateFSViewTree_pressed()
    {
        TestFileSystem = new FSViewTree();
        TestFileSystem.PrintRoot();
    }

    protected void _on_RefreshFSViewTree_pressed()
    {
        //GD.Print("Firing RefreshDirectories()");
        Task.Run(() =>
        {
            TestFileSystem!.RefreshDirectories();
            TestFileSystem.PrintTree(TestFileSystem.userRootDir!);
        });

    }

    protected void DefaultZooData()
    {
        ZooInfo.AddZoo("Detroit");
        ZooInfo.AddZoo("Chicago");
        ZooInfo.AddZoo("Tokyo");
        ZooInfo.AddZoo("New York");

        ZooInfo.AddAnimal("Elk");
        ZooInfo.AddAnimal("Polar Bear");
        ZooInfo.AddAnimal("Monkey");
        ZooInfo.AddAnimal("Shark");
        ZooInfo.AddAnimal("Parrot");
        ZooInfo.AddAnimal("Penguin");
        ZooInfo.AddAnimal("Tiger");
        ZooInfo.AddAnimal("Wolf");

        Zoo zoo = ZooInfo.GetZoo("Detroit");
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Elk"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Polar Bear"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Monkey"));

        zoo = ZooInfo.GetZoo("Chicago");
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Shark"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Parrot"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Penguin"));

        zoo = ZooInfo.GetZoo("Tokyo");
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Tiger"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Wolf"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Elk"));

        zoo = ZooInfo.GetZoo("New York");
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Polar Bear"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Monkey"));
        ZooInfo.AddZooAnimal(zoo, ZooInfo.GetAnimal("Shark"));
    }

}
