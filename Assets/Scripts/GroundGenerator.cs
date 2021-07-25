using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    private List<Vector3> nVertices = new List<Vector3>();
    private List<int> nTriangles = new List<int>();
    private List<Vector2> nUV = new List<Vector2>();
    private Mesh mesh;
    private MeshCollider meshCollider;

    private int squareCount;

    private const int MAXHOUSECOUNT = 8;
    public GameObject House;
    private List<GameObject> HouseList = new List<GameObject>();
    private List<GameObject> MapItemList = new List<GameObject>();

    private const int MAXSHOPCOUNT = 1;
    public GameObject Shop;
    

    private const int MAXTREECOUNT = 200;
    public GameObject Tree;
    

    private const int MAXBRIDGECOUNT = 3;
   
    private int mapWidth = 100;
    private int mapHeight = 100;

    public byte[,] squares;

    // Seeding for terrain gen
    private int seed;

    private const int LARGEINT = 1000000;

    // UV Stuff
    private enum GroundTextures { 
        GRASS,
        SAND,
        WATER,
        BRIDGE
    }
    private float tUnit = 1 / 4f;


    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        seed = Random.Range(1,LARGEINT);
        GenerateGround();
        BuildMesh();
        UpdateMesh();
    }

    public void Reset()
    {      
        seed = Random.Range(1, LARGEINT);

        foreach (var i in MapItemList) Destroy(i);
        MapItemList = new List<GameObject>();
        

        GenerateGround();
        BuildMesh();
        UpdateMesh();
    }

    void GenerateGround() 
    {        
        int[] rivermiddles = new int[mapHeight];
        squares = new byte[mapWidth, mapHeight];

        for (int px = 0; px < mapWidth; px++)
        {
            // generating cutoff point for sand and ocean   
            int sandCutoff = 
                Noise(px, 0, 50, 5, 1) +
                Noise(px, 0, 25, 15, 1)  +
                10;
            
            int waterCutoff = 
                Noise(px, 0, 50, 5, 1) +
                Noise(px, 0, 25, 15, 1);        

            for (int py = 0; py < mapHeight; py++)
            {

                int riverMiddle =
                Noise(0, py, 35, 25, 1) +
                (int)(squares.GetLength(0) / 2);

                rivermiddles[py] = riverMiddle;

                if (py < waterCutoff) 
                {
                    squares[px, py] = (byte)GroundTextures.WATER;
                }                    
                else if (py < sandCutoff)
                {
                    squares[px, py] = (byte)GroundTextures.SAND;
                }
                else if (py >= sandCutoff) 
                {
                    squares[px, py] = (byte)GroundTextures.GRASS;
                }
                
                // river generation
                if ((riverMiddle - 3) < px && px < (riverMiddle + 3))
                {
                    squares[px, py] = (byte)GroundTextures.WATER;
                }                

                // pond generaton
                if (Noise(px, py, 25, 13, 1) > 10 && py > 10 && py < 80) 
                {
                    squares[px, py] = (byte)GroundTextures.WATER;

                }
            }
        } // End Loop

        for (int i = 0; i < MAXBRIDGECOUNT; i++) 
        {
            int bridgewidth = 5;
            int bridgeheight = 3;

            int bridgestartY = Random.Range(10, mapHeight - 10);
            int bridgestartX = rivermiddles[bridgestartY] - (bridgewidth);
            int bridgeendY = bridgestartY + bridgeheight;
            int bridgeendX = bridgestartX + (bridgewidth*2);

            for (int x = bridgestartX; x <= bridgeendX; x++) 
            {
                for (int y = bridgestartY; y < bridgeendY; y++) 
                {
                    squares[x, y] = (byte)GroundTextures.BRIDGE;
                }
            }           
        }

        PlaceMapItemRandomly(House, MAXHOUSECOUNT, GroundTextures.GRASS);
        PlaceMapItemRandomly(Tree, MAXTREECOUNT, GroundTextures.GRASS);
        PlaceMapItemRandomly(Shop, MAXSHOPCOUNT, GroundTextures.GRASS);

    }

    //void PlaceHouses() 
    //{
    //    // TODO make this agnostic of map element
    //    int houseCount = 0;
    //    // keep us from an infinite loop.
    //    int MAXMISSES = 10;
    //    int misses = 0;
    //    while (houseCount <= MAXHOUSECOUNT && misses < MAXMISSES)        {
    //        // get random square
    //        //print(squareCount);
    //        int[] p = IndexToCoordinates(Random.Range(0, mapWidth * mapHeight));
    //        // check if it's valid (on grass)
    //        print(p[0] + " " + p[1]);
    //        switch (squares[p[0], p[1]]) 
    //        {
    //            case (byte)GroundTextures.GRASS:
    //                //make new house!         
    //                HouseList.Add(
    //                    Instantiate(House, new Vector3(p[0] + 0.07f, 0 , p[1] - 0.3f), House.transform.rotation));
    //                houseCount++;
    //                break;
    //            default:
    //                misses++;
    //                break;
    //        }
    //    }        
    //}

    // Places objects on map
    void PlaceMapItemRandomly(GameObject item, int max, GroundTextures allowedTile)
    {
        float smallHeight = 0.07f;
        int itemCount = 0;

        // keep us from an infinite loop.
        int MAXMISSES = 10;
        int misses = 0;       

        while (itemCount < max && misses < MAXMISSES)
        {
            // get random square            
            int[] p = IndexToCoordinates(Random.Range(0, mapWidth * mapHeight));
            
            Vector3 pv = new Vector3(p[0] + smallHeight, 0, p[1] - 0.3f);            

            if (squares[p[0], p[1]] == (byte)allowedTile && ItemAtPosition(p[0],p[1]) == null)
            {           
                MapItemList.Add(Instantiate(item, pv, item.transform.rotation));
                itemCount++;
            }       
        }
    }

    // gets the item at a certain position, null if not found
    GameObject ItemAtPosition(float x, float y) 
    {
        foreach (GameObject i in MapItemList) 
        {
            if (i.transform.position.x == x && i.transform.position.y == y) 
            {
                return i;
            }
        }

        return null;
    }

    int[] IndexToCoordinates(int i) 
    {
        //print(i);
        // maps 1d array index to 2d index (i hope)
        int[] p = new int[2];
        p[0] = i / mapWidth;
        p[1] = i % mapWidth;
        //print(p);
        return p;
    }


    void BuildMesh() 
    {
        for (int px = 0; px < squares.GetLength(0); px++)
        {
            for (int py = 0; py < squares.GetLength(1); py++)
            {
                GenSquare(px, py, 0, squares[px, py]);
            }
        }
    }

    void UpdateMesh() 
    {
        mesh.Clear();
        mesh.vertices = nVertices.ToArray();
        mesh.triangles = nTriangles.ToArray();
        mesh.uv = nUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

        squareCount = 0;
        nVertices.Clear();
        nTriangles.Clear();
        nUV.Clear();
        seed = Random.Range(1, LARGEINT);

    }

    void GenSquare(int x, int y, int z, int t) 
    {
        nVertices.Add(new Vector3(x, y, z));
        nVertices.Add(new Vector3(x + 1, y, z));
        nVertices.Add(new Vector3(x + 1, y - 1, z));
        nVertices.Add(new Vector3(x, y - 1, z));

        nTriangles.Add(squareCount * 4);
        nTriangles.Add((squareCount * 4) + 1);
        nTriangles.Add((squareCount * 4) + 3);
        nTriangles.Add((squareCount * 4) + 1);
        nTriangles.Add((squareCount * 4) + 2);
        nTriangles.Add((squareCount * 4) + 3);

        nUV.Add(new Vector2(tUnit * t,         1));
        nUV.Add(new Vector2(tUnit * t + tUnit, 1));
        nUV.Add(new Vector2(tUnit * t + tUnit, 0));
        nUV.Add(new Vector2(tUnit * t,         0));

        squareCount++;
    }


    // Code snipped from http://studentgamedev.blogspot.com/2013/09/unity-voxel-tutorial-part-3-perlin.html
    int Noise(int x, int y, float scale, float mag, float exp) 
    {
        // returns a modifiable value from the perlin noise function.
        return (int)(Mathf.Pow((Mathf.PerlinNoise(seed + x / scale, seed + y / scale) * mag), (exp)));
    }
    // End snipped

    // Update is called once per frame
    void Update()
    {
        
    }
}
