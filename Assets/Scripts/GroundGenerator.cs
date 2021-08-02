using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    // ground mesh is what the player will see
    private Mesh gMesh;
    private List<Vector3> gVertices = new List<Vector3>();
    private List<int> gTriangles = new List<int>();
    private List<Vector2> gUV = new List<Vector2>();
    private int gQuadCount;

    // cMesh is the collision mesh, which will block walking on water
    private Mesh cMesh;
    private MeshCollider MeshCollider;
    private List<Vector3> cVertices = new List<Vector3>();
    private List<int> cTriangles = new List<int>();
    private List<Vector2> cUV = new List<Vector2>();
    private int cQuadCount;


    private const int MAXHOUSECOUNT = 8;
    public GameObject House;
    private List<GameObject> HouseList = new List<GameObject>();
    private List<GameObject> MapItemList = new List<GameObject>();

    private const int MAXSHOPCOUNT = 1;
    public GameObject Shop;
    

    private const int MAXTREECOUNT = 200;
    public GameObject Tree;
    

    private const int MAXBRIDGECOUNT = 3;
   
    private const int MAPWIDTH = 100;
    private const int MAPHEIGHT = 100;

    public byte[,] Tilemap;

    // Seeding for terrain gen
    private int RandomSeed;

    private const int LARGEINT = 1000000;

    private int debug = 0;

    // UV Stuff
    private enum GroundTextures { 
        GRASS,
        SAND,
        WATER,
        BRIDGE
    }

    private const float TUNIT = 1 / 4f;


    private enum GroundItems 
    {
        SHOP,
        PLAYERHOUSE,
        NPCHOUSE,
        TREE,
        FLOWER
    }   


    // Start is called before the first frame update
    void Start()
    {
        gMesh = GetComponent<MeshFilter>().mesh;
        cMesh = new Mesh();

        MeshCollider = GetComponent<MeshCollider>();
        RandomSeed = Random.Range(1,LARGEINT);
        NewMap();
        
    }

    public void NewMap() 
    {
        GenerateGround();
        BuildMesh();
        UpdateMesh();
    }

    public void SaveData() 
    {
        
    }

    public void LoadData(Database db)
    { 
        
    }

    public void Reset()
    {      
        RandomSeed = Random.Range(1, LARGEINT);       
        foreach (var i in MapItemList) Destroy(i);
        MapItemList = new List<GameObject>();
        

        GenerateGround();
        BuildMesh();
        UpdateMesh();
    }    

    // Places objects on map
    void PlaceMapItemRandomly(GameObject item, int max, GroundTextures allowedTile)
    {        
        int itemCount = 0;

        // keep us from an infinite loop.
        int MAXMISSES = 10;
        int misses = 0;       

        while (itemCount < max && misses < MAXMISSES)
        {
            // get random square            
            int[] p = IndexToCoordinates(Random.Range(0, MAPWIDTH * MAPHEIGHT));         
                       

            if (Tilemap[p[0], p[1]] == (byte)allowedTile && ItemAtPosition(p[0],p[1]) == null)
            {
                InstantiateItemAtPosition(item, p[0], p[1]);                
                itemCount++;
            }       
        }
    }

    /// <summary>
    /// Instantiates an item at the chose grid-square, <b>assumes data has been validated</b>
    /// </summary>
    /// <param name="item"> GameObject class to be created</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InstantiateItemAtPosition(GameObject item, int x, int y) 
    {
        float smallHeight = 0.07f;
        Vector3 pv = new Vector3((float)x + smallHeight, 0, (float)y - 0.3f);
        MapItemList.Add(Instantiate(item, pv, item.transform.rotation));
    }

    /// <summary>
    /// gets the item at a certain position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>GameObject at position, or null if none found</returns>
    GameObject ItemAtPosition(int x, int y) 
    {
        foreach (GameObject i in MapItemList) 
        {
            if ((int)i.transform.position.x == x && (int)i.transform.position.y == y) 
            {
                return i;
            }
        }

        return null;
    }

    /// <summary>
    /// Converts single-dimensional value into two dimensions
    /// </summary>
    /// <param name="i"></param>
    /// <returns>array of int[2] with x and y positions of param i</returns>
    int[] IndexToCoordinates(int i) 
    {
        //print(i);
        // maps 1d array index to 2d index (i hope)
        int[] p = new int[2];
        p[0] = i / MAPWIDTH;
        p[1] = i % MAPWIDTH;
        //print(p);
        return p;
    }


    /// <summary>
    /// Generates the ground textures using noise, and places objects on the map
    /// </summary>
    void GenerateGround()
    {
        int[] rivermiddles = new int[MAPHEIGHT];
        Tilemap = new byte[MAPWIDTH, MAPHEIGHT];

        for (int px = 0; px < MAPWIDTH; px++)
        {
            // generating cutoff point for sand and ocean   
            int sandCutoff =
                Noise(px, 0, 50, 5, 1) +
                Noise(px, 0, 25, 15, 1) +
                10;

            int waterCutoff =
                Noise(px, 0, 50, 5, 1) +
                Noise(px, 0, 25, 15, 1);

            for (int py = 0; py < MAPHEIGHT; py++)
            {

                int riverMiddle =
                Noise(0, py, 35, 25, 1) +
                (int)(Tilemap.GetLength(0) / 2);

                rivermiddles[py] = riverMiddle;

                if (py < waterCutoff)
                {
                    Tilemap[px, py] = (byte)GroundTextures.WATER;
                }
                else if (py < sandCutoff)
                {
                    Tilemap[px, py] = (byte)GroundTextures.SAND;
                }
                else if (py >= sandCutoff)
                {
                    Tilemap[px, py] = (byte)GroundTextures.GRASS;
                }

                // river generation
                if ((riverMiddle - 3) < px && px < (riverMiddle + 3))
                {
                    Tilemap[px, py] = (byte)GroundTextures.WATER;
                }

                // pond generaton
                if (Noise(px, py, 25, 13, 1) > 10 && py > 10 && py < 80)
                {
                    Tilemap[px, py] = (byte)GroundTextures.WATER;

                }
            }
        } // End Loop

        for (int i = 0; i < MAXBRIDGECOUNT; i++)
        {
            int bridgewidth = 5;
            int bridgeheight = 3;

            int bridgestartY = Random.Range(10, MAPHEIGHT - 10);
            int bridgestartX = rivermiddles[bridgestartY] - (bridgewidth);
            int bridgeendY = bridgestartY + bridgeheight;
            int bridgeendX = bridgestartX + (bridgewidth * 2);

            for (int x = bridgestartX; x <= bridgeendX; x++)
            {
                for (int y = bridgestartY; y < bridgeendY; y++)
                {
                    Tilemap[x, y] = (byte)GroundTextures.BRIDGE;
                }
            }
        }

        PlaceMapItemRandomly(House, MAXHOUSECOUNT, GroundTextures.GRASS);
        PlaceMapItemRandomly(Tree, MAXTREECOUNT, GroundTextures.GRASS);
        PlaceMapItemRandomly(Shop, MAXSHOPCOUNT, GroundTextures.GRASS);

    }

    /// <summary>
    /// Builds mesh from our squares array
    /// </summary>
    /// <remarks>Code adapted from tutorial: http://studentgamedev.blogspot.com/2013/08/unity-voxel-tutorial-part-1-generating.html </remarks>
    void BuildMesh() 
    {
        for (int px = 0; px < Tilemap.GetLength(0); px++)
        {
            for (int py = 0; py < Tilemap.GetLength(1); py++)
            {
                if (Tilemap[px, py] == (byte)GroundTextures.WATER)
                {
                    // generate T + NESW faces for water tiles
                    // this can be optimised a lot!
                    TopFace(px, py, -1f, Tilemap[px, py], ref cVertices, ref cTriangles, ref cUV, ref cQuadCount);                    

                    if (py < MAPHEIGHT - 1)
                    {
                        if (Tilemap[px, py + 1] != (byte)GroundTextures.WATER)
                            NorthFace(px, py, 0, Tilemap[px, py], ref cVertices, ref cTriangles, ref cUV, ref cQuadCount);
                    }

                    if (px < MAPWIDTH - 1)
                    {
                        if (Tilemap[px + 1, py] != (byte)GroundTextures.WATER)
                            EastFace(px, py, 0, Tilemap[px, py], ref cVertices, ref cTriangles, ref cUV, ref cQuadCount);
                    }

                    if (py > 0)
                    {
                        if (Tilemap[px, py - 1] != (byte)GroundTextures.WATER)
                            SouthFace(px, py, 0, Tilemap[px, py], ref cVertices, ref cTriangles, ref cUV, ref cQuadCount);
                    }

                    if (px > 0)
                    {
                        debug++; 

                        if (Tilemap[px - 1, py] != (byte)GroundTextures.WATER)
                            WestFace(px, py, 0, Tilemap[px, py], ref cVertices, ref cTriangles, ref cUV, ref cQuadCount);
                    }
                        

                }
                else 
                {
                    // generate T face for collision on ground tiles
                    TopFace(px, py, 0, Tilemap[px, py], ref cVertices, ref cTriangles, ref cUV, ref cQuadCount);
                }
                
                // generate T face for view mesh regardless
                TopFace(px, py, 0, Tilemap[px, py], ref gVertices, ref gTriangles, ref gUV, ref gQuadCount);
                
            }
        }

        print(debug);
        debug = 0;

    }

    /// <summary>
    /// Set up our mesh and collision data via
    /// </summary>
    /// <remarks>Code adapted from tutorial: http://studentgamedev.blogspot.com/2013/08/unity-voxel-tutorial-part-1-generating.html </remarks>
    void UpdateMesh() 
    {
        gMesh.Clear();
        gMesh.vertices = gVertices.ToArray();
        gMesh.triangles = gTriangles.ToArray();
        gMesh.uv = gUV.ToArray();
        gMesh.Optimize();
        gMesh.RecalculateNormals();

        cMesh.Clear();
        cMesh.vertices = cVertices.ToArray();
        cMesh.triangles = cTriangles.ToArray();
        cMesh.uv = cUV.ToArray();
        cMesh.Optimize();
        cMesh.RecalculateNormals();

        MeshCollider.sharedMesh = cMesh;

        gQuadCount = 0;
        cQuadCount = 0;

        gVertices.Clear();
        cVertices.Clear();

        gTriangles.Clear();
        cTriangles.Clear();
        
        gUV.Clear();
        cUV.Clear();
        
        RandomSeed = Random.Range(1, LARGEINT);

    }

    /// <summary>
    /// Generates top face of a cube at (x,y,z)
    /// </summary>
    /// <remarks>Code adapted from tutorial: http://studentgamedev.blogspot.com/2013/08/unity-voxel-tutorial-part-1-generating.html </remarks>  
    void TopFace(float x, float y, float z, int t, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uv, ref int counter) 
    {
        verts.Add(new Vector3(x, y, z));
        verts.Add(new Vector3(x + 1, y, z));
        verts.Add(new Vector3(x + 1, y - 1, z));
        verts.Add(new Vector3(x, y - 1, z));

        AddTriangles(ref tris, ref counter);

        AddUV(ref uv, t);

        counter++;
    } 
    
    void NorthFace(float x, float y, float z, int t, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uv, ref int counter) 
    {
        verts.Add(new Vector3(x + 1, y, z));
        verts.Add(new Vector3(x + 1, y, z - 1));
        verts.Add(new Vector3(x, y, z - 1));
        verts.Add(new Vector3(x, y, z));

        AddTriangles(ref tris, ref counter);

        AddUV(ref uv, t);

        counter++;
    }
    
    void EastFace(float x, float y, float z, int t, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uv, ref int counter) 
    {
        verts.Add(new Vector3(x + 1, y - 1, z));
        verts.Add(new Vector3(x + 1, y - 1, z - 1));
        verts.Add(new Vector3(x + 1, y, z - 1));
        verts.Add(new Vector3(x+1, y, z ));

        AddTriangles(ref tris, ref counter);

        AddUV(ref uv, t);

        counter++;
    }
    
    void SouthFace(float x, float y, float z, int t, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uv, ref int counter) 
    {
        verts.Add(new Vector3(x, y - 1, z));
        verts.Add(new Vector3(x, y - 1, z - 1));
        verts.Add(new Vector3(x + 1, y - 1, z - 1));
        verts.Add(new Vector3(x + 1, y - 1, z));

        AddTriangles(ref tris, ref counter);

        AddUV(ref uv, t);

        counter++;
    }
    
    void WestFace(float x, float y, float z, int t, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uv, ref int counter) 
    {        
        verts.Add(new Vector3(x, y - 1, z-1));
        verts.Add(new Vector3(x, y - 1, z));
        verts.Add(new Vector3(x, y, z));
        verts.Add(new Vector3(x, y, z-1));

        AddTriangles(ref tris, ref counter);

        AddUV(ref uv, t);

        counter++;
    }

    /// <summary>
    /// Add UV data to the UVs list
    /// </summary>
    void AddUV(ref List<Vector2> uv, int t) 
    {
        uv.Add(new Vector2(TUNIT * t, 1));
        uv.Add(new Vector2(TUNIT * t + TUNIT, 1));
        uv.Add(new Vector2(TUNIT * t + TUNIT, 0));
        uv.Add(new Vector2(TUNIT * t, 0));
    }

    /// <summary>
    /// Adds triangles to the tris list, which assign edges to mesh vertices
    /// </summary>
    /// <param name="tris"></param>
    /// <param name="counter"></param>
    void AddTriangles(ref List<int> tris, ref int counter) 
    {
        tris.Add(counter * 4);
        tris.Add((counter * 4) + 1);
        tris.Add((counter * 4) + 3);
        tris.Add((counter * 4) + 1);
        tris.Add((counter * 4) + 2);
        tris.Add((counter * 4) + 3);
    }


    /// <summary>
    /// Modified perlin-noise generator
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="scale"></param>
    /// <param name="mag"></param>
    /// <param name="exp"></param>
    /// <returns>noise value</returns>
    /// <remarks>Code from tutorial here: http://studentgamedev.blogspot.com/2013/09/unity-voxel-tutorial-part-3-perlin.html</remarks>
    int Noise(int x, int y, float scale, float mag, float exp) 
    {
        // returns a modifiable value from the perlin noise function.
        return (int)(Mathf.Pow((Mathf.PerlinNoise(RandomSeed + x / scale, RandomSeed + y / scale) * mag), (exp)));
    }
    

}
