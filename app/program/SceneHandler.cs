using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SceneHandler : Node
{
    /*
        WARNING: Currently, switching scenes DOES NOT
        retain the state of the scene you left. 
        I will have to resolve this in the future so users
        don't inadvertently erase work. 
        I will tackle this with the overall system for storing
        user data and program resume.
    */
    Program? Main;
    Node? AppHook;
    string HtmlDownloadPath = @"res://app/htmldownload/HtmlDownload.tscn";
    string JsonInspectorPath = @"res://app/jsoninspector/JSONInspector.tscn";
    PackedScene? SceneOnDeck;

    public SceneHandler()
    {
        PackedSceneLoaded += this.OnPackedSceneLoaded; // Subscribe our method to our own event.
    }

    public override void _Ready()
    {
        // IMPORTANT: The owner is always the scene root
        // of the file that the node was loaded from.
        // Otherwise, to get the top node, you have to use "/root/Main"
        Main = Owner as Program;
        AppHook = Owner.GetNode("AppHook");
    }

    public void SwitchToScene(int s)
    // Stopgap function for switching between scenes.
    {
        string scenePath;
        switch (s)
        {
            case (0):
                scenePath = HtmlDownloadPath;
                break;
            case (1):
                scenePath = JsonInspectorPath;
                break;
            default:
                return;
        }

        Task.Run(() =>
        {
            LoadPackedScene(scenePath);
            PackedSceneLoaded(); // Fire event after scene is loaded so we can switch on the main thread.
        });
    }

    protected void LoadPackedScene(string scenePath)
    {
        SceneOnDeck = GD.Load<PackedScene>(scenePath);
        return;
    }

    protected void SwitchApp()
    {
        Node NewScene = SceneOnDeck!.Instance();
        foreach (Node Child in AppHook!.GetChildren())
        {
            AppHook.RemoveChild(Child);
            Child.QueueFree();
        }
        AppHook.AddChild(NewScene);
        return;
    }

    public event Action PackedSceneLoaded; // An event with no arguments or return value.

    protected virtual void OnPackedSceneLoaded() // Our function to call when PackedSceneLoaded is triggered. 
    {
        SwitchApp();
    }

}
