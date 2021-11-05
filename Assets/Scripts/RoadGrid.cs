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

    // TODO: Napisać to lepiej
    public (RoadType roadType, int orientation) GetRoadForIndex(int x, int z)
    {
        (int x, int z)[] directions = new (int x, int z)[4]
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
        };
        bool[] roadNeigours = new bool[4];
        int howManyRoadNeibor = 0;

        for (int i = 0; i < 4; i++)
        {
            (int x, int z) newIndex = (directions[i].x + x, directions[i].z + z);
            if (newIndex.x >=0 && newIndex.x < this.x && newIndex.z >= 0 && newIndex.z < this.z && grid[newIndex.x, newIndex.z])
            {
                roadNeigours[i] = true;
                howManyRoadNeibor++;
            }
        }
        switch (howManyRoadNeibor)
        {
            case 0:
                // ???
                    return (0, 0);
                break;
            case 1:
                if (roadNeigours[0] == true || roadNeigours[2] == true)
                {
                    return (RoadType.Road, 0);
                }
                else
                {
                    return (RoadType.Road, 1);
                }
                break;
            case 2:
                if (roadNeigours[0] == true && roadNeigours[2] == true)
                {
                    return (RoadType.Road, 0);
                }
                else if(roadNeigours[1] == true && roadNeigours[3] == true)
                {
                    return (RoadType.Road, 1);
                }
                else if (roadNeigours[0] == true)
                {
                    if (roadNeigours[3] == true)
                    {
                        return (RoadType.Road90, 3);
                    }
                    else if (roadNeigours[1] == true)
                    {
                        return (RoadType.Road90, 0);
                    }
                }
                else //if (roadNeigours[2] == true)
                {
                    if (roadNeigours[3] == true)
                    {
                        return (RoadType.Road90, 2);
                    }
                    else if (roadNeigours[1] == true)
                    {
                        return (RoadType.Road90, 1);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    if (roadNeigours[i] == false)
                    {
                        return (RoadType.RoadT, i);
                    }
                }
                break;
            case 4:
                return (RoadType.RoadX, 0);
                break;
        }
        return (0, 0);
    }

    public void AddNewPath(List<Node> path)
    {
        foreach (var node in path)
        {
            grid[(int)node.Position.x, (int)node.Position.y] = true;
        }
    }

}

enum RoadType
{
    Road,
    Road90,
    RoadT,
    RoadX,
}