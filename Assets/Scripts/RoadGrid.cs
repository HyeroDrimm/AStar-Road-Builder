using System.Collections.Generic;
using UnityEngine;

class RoadGrid
{
    private bool[,] grid;
    private int x, z;
    public RoadGrid(int x, int z)
    {
        grid = new bool[x, z];
        this.x = x;
        this.z = z;
    }

    public (RoadType, int) GetRoadForIndex(int x, int z)
    {
        (int x, int z)[] directions = new (int x, int z)[4]
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
        };
        int roadIndex = 0;
        // Convert neibors to int number in range 0-15
        for (int i = 0; i < 4; i++)
        {
            (int x, int z) newIndex = (directions[i].x + x, directions[i].z + z);
            if (newIndex.x >= 0 && newIndex.x < this.x && newIndex.z >= 0 && newIndex.z < this.z && grid[newIndex.x, newIndex.z])
            {
                roadIndex += (int)Mathf.Pow(2, i);
            }
        }
        // Convert this number to RoadType and orientation
        return roadTypeConverter[roadIndex];
    }
    public void AddNewPath(List<Node> path)
    {
        foreach (var node in path)
        {
            grid[(int)node.Position.x, (int)node.Position.y] = true;
        }
    }
    // Well Thouthout array with fixed combination with O(1) access and less branches
    (RoadType roadType, int rotation)[] roadTypeConverter = new (RoadType roadType, int rotation)[] 
    { 
        (RoadType.None   , 0),
        (RoadType.Road   , 0),
        (RoadType.Road   , 1),
        (RoadType.Road90 , 0),
        (RoadType.Road   , 0),
        (RoadType.Road   , 0),
        (RoadType.Road90 , 1),
        (RoadType.RoadT  , 3),
        (RoadType.Road   , 1),
        (RoadType.Road90 , 3),
        (RoadType.Road   , 1),
        (RoadType.RoadT  , 2),
        (RoadType.Road90 , 2),
        (RoadType.RoadT  , 1),
        (RoadType.RoadT  , 0),
        (RoadType.RoadX  , 0),
    };
}

enum RoadType
{
    None,
    Road,
    Road90,
    RoadT,
    RoadX,
}