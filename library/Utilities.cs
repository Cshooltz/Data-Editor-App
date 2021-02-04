using Godot;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Godot.Utilities
{
    public class Utilities : object
    {
        public static T? SafeGetNode<T>(Node ThisNode, string PathToNode) where T : Node
        {
            Node? NodeToGet = ThisNode.GetNode(PathToNode);
            if (NodeToGet == null)
            {
                GD.PrintErr($"Error: Attmpted to get Node {PathToNode}, but found null.");
                return null;

            }
            else if (NodeToGet.GetType().IsSubclassOf(typeof(T)))
            {
                GD.Print($@"Warning: Got Node {PathToNode} which is subclass of {typeof(T).ToString()}
                as type {NodeToGet.GetType()}.");

            }
            else if (NodeToGet.GetType() != typeof(T))
            {
                GD.PrintErr($@"Error: Got Node {PathToNode} as type {typeof(T).ToString()}
                 but Node is type {NodeToGet.GetType()} instead.");
                return null;
            }
            return NodeToGet as T;
        }

        public static void PrintChildrenRecursive(Node node)
        {
            // Note: This task only reads from data in the scene
            // In spite of calling methods on scene objects.
            // Any future changes to this code should avoid writing
            // data to anything other than the console. 
            Task.Run(() =>
            {
                int Depth = 0;
                PrintRecursive(node);
                return;

                void PrintRecursive(Node node1)
                {
                    int Counter = 0;
                    StringBuilder Tabs = new StringBuilder("");
                    for (int i = 0; i < Depth; i++) Tabs.Append("\t");
                    GD.Print($"{Tabs}Printing {node1.GetChildren().Count} children of {node1.Name} : {node1.GetType()}");
                    foreach (Node Child in node1.GetChildren())
                    {
                        GD.Print($"{Tabs}\tChild {Counter}: {Child.Name} is type {Child.GetType()}");
                        Counter++;
                        if (Child.GetChildren().Count != 0)
                        {
                            Depth++;
                            PrintRecursive(Child);
                            Depth--;
                        }
                    }
                    return;
                }
            });
        }
    }

    public class Debug : object
    {

    }

    public static class Extensions : object
    {
        // Use this as a place to store extensions for the project.
    }
}
