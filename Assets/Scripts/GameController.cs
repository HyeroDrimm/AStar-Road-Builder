using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private Transform floorTileHolder;
    [SerializeField] private int xFloorSize = 8;
    [SerializeField] private int zFloorSize = 8;

    private Camera cam;

    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material grayMaterial;

    [SerializeField] private GameObject[] RoadTiles = new GameObject[4];

    private GridGraph map;
    private RoadGrid roadGrid;
    private FloorTile[,] floorTiles;

    // TEMP
    private bool firtstChosen;
    private int fz, fx, sx, sz;

    private void Start()
    {
        floorTiles = new FloorTile[xFloorSize, zFloorSize];
        map = new GridGraph(xFloorSize, zFloorSize);
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

        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.CompareTag("FloorTile"))
            {
                if (!firtstChosen)
                {
                    fx = hit.transform.GetComponent<FloorTile>().x;
                    fz = hit.transform.GetComponent<FloorTile>().z;
                    firtstChosen = true;
                }
                else
                {
                    sx = hit.transform.GetComponent<FloorTile>().x;
                    sz = hit.transform.GetComponent<FloorTile>().z;
                    firtstChosen = false;
                    AddNewPath(AStar.Search(map, map.Grid[sx, sz], map.Grid[fx, fz]));
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
