using Godot;
using System;
using System.Linq;
using System.Threading;
//using System.Data.SQLite; // SQLite ADO.Net data provider
using Newtonsoft.Json; // Full featured JSON serializer
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
//using MySql.Data; // MySQLADO.Net data provider
using System.Xml.Linq;
using LiteDB;
// using System.IO;

public class Program : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Hello world!");
        GD.Print($"Main Thread # {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        GD.Print($"user:// data directory: {OS.GetUserDataDir()}");
        return;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
