using Godot;
using System;
using System.Text;
using Godot.Utilities;

public class MenuPanel : Panel
{
    Program? Main;
    SceneHandler? Schd; // Why the hell did I choose this name?
    MenuButton? MyMenu;

    public override void _Ready()
    {
        Main = Owner as Program;
        Schd = Main!.GetNode<SceneHandler>("SceneHandler");

        MyMenu = GetNode<MenuButton>("MenuHBox/MenuButton");
        PopupMenu myPopup = MyMenu.GetPopup();
        myPopup.Connect("id_pressed", this, "_OnMenuPopupPressed");
    }

    public void _OnMenuPopupPressed(int id)
    {
        switch (id)
        {
            case 2:
                Schd!.SwitchToScene(0);
                break;
            case 3:
                Schd!.SwitchToScene(1);
                break;
            default:
                break;
        }
        return;
    }

    public void _OnPrintMenuChildren()
    {
        Utilities.PrintChildrenRecursive(MyMenu!);
    }

}