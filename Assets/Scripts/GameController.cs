using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int xFloorSize = 8;
    [SerializeField] private int zFloorSize = 8;
    [SerializeField] private List<Vector2> buildings;

    private Camera cam;

    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private Transform floorTileHolder;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material grayMaterial;

    [SerializeField] private GameObject[] RoadTiles = new GameObject[4];
    [SerializeField] private GameObject buildingPrefab;

    private GridGraph map;
    private RoadGrid roadGrid;
    private FloorTile[,] floorTiles;

    private bool firtstChosen;
    private (int x, int z) firstPick;
    private (int x, int z) secondPick;

    private void Start()
    {
        floorTiles = new FloorTile[xFloorSize, zFloorSize];
        map = new GridGraph(xFloorSize, zFloorSize);
        map.Walls = buildings;
        roadGrid = new RoadGrid(xFloorSize, zFloorSize);
        // Generating Visaul grid
        for (int x = 0; x < xFloorSize; x++)
        {
            for (int z = 0; z < zFloorSize; z++)
            {
                Transform floorTile = Instantiate(floorTilePrefab, new Vector3(x, 0, z), Quaternion.identity, floorTileHolder).transform.GetChild(0);
                floorTiles[x, z] = floorTile.GetComponent<FloorTile>();
                floorTiles[x, z].x = x;
                floorTiles[x, z].z = z;

                if ((x + z) % 2 == 0)
                    floorTile.GetComponent<MeshRenderer>().material = yellowMaterial;                
                else
                    floorTile.GetComponent<MeshRenderer>().material = grayMaterial;
            }
        }
        foreach (var building in buildings)
        {
            Instantiate(buildingPrefab, floorTiles[(int)building.x, (int)building.y].transform);
            floorTiles[(int)building.x, (int)building.y].GetComponent<MeshCollider>().enabled = false;
        }

        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.CompareTag("FloorTile"))
            {
                FloorTile choosenFloorTile = hit.transform.GetComponent<FloorTile>();
                if (!firtstChosen)
                {
                    firstPick = (choosenFloorTile.x, choosenFloorTile.z);
                    firtstChosen = true;
                }
                else
                {
                    secondPick = (choosenFloorTile.x, choosenFloorTile.z);
                    firtstChosen = false;
                    AddNewPath(AStar.Search(map, map.Grid[firstPick.x, firstPick.z], map.Grid[secondPick.x, secondPick.z]));
                }
            }
        }
        void AddNewPath(List<Node> path)
        {
            roadGrid.AddNewPath(path);
            HashSet<Node> pathWithNeighbours = AddPathNeighbours(path);
            UpdateRoads(pathWithNeighbours);
        }

        HashSet<Node> AddPathNeighbours(List<Node> path)
        {
            HashSet<Node> result = new HashSet<Node>();
            foreach (var node in path)
            {
                result.Add(node);
                foreach (var neigbour in map.Neighbours(node))
                {
                    result.Add(neigbour);
                }
            }
            return result;
        }
        
        void UpdateRoads(HashSet<Node> path)
        {
            foreach (var node in path)
            {
                int x = (int)node.Position.x;
                int z = (int)node.Position.y;
                (RoadType roadType, int rotation) = roadGrid.GetRoadForIndex(x, z);
                if (roadType != RoadType.None)
                {
                    floorTiles[x, z].SpawnRoadTile(RoadTiles[(int)roadType - 1], rotation);
                }
            }
        }
    }
}
