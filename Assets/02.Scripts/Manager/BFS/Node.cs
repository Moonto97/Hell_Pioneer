using UnityEngine;

public class Node
{
    public bool Walkable{ get; private set; }
    public Vector3 WorldPos{ get; private set; }
    public int X{ get; private set; }
    public int Y{ get; private set; }
    
    public int Distance{ get; set; }

    public Node(bool walkable, Vector3 worldPos, int x, int y)
    {
        Walkable = walkable;
        WorldPos = worldPos;
        X = x;
        Y = y;
        Distance = int.MaxValue;
    }
}