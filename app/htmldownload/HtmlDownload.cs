using Godot;
using System;

public class HtmlDownload : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public void _OnButtonPressed()
    {
        GD.Print("Button pressed.");
        GetNode<Label>("VSplitContainer/Label").Text = "Hello!";
    }

    public void _OnStartDownload()
    {
        Button dlButton = GetNode<Button>("Button");
        HttpDownload.Download(dlButton);
    }
}
