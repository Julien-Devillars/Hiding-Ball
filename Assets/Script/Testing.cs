using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    public int mWidth;
    public int mHeight;
    public float mSize;
    public GameObject mFloor;

    private bool mFlagLeftClickPressed = false;
    private bool mFlagRightClickPressed = false;

    public Camera mCamera;
    private Grid mGrid;

    public int SUBDIVISION_MAX = 2;
    public int SUBDIVISION_MIN = 0;
    public GameObject mEmitter;

    Utils mUtils = new Utils();
    public static List<Wall> mWalls;

    private void Start()
    {
        mGrid = new Grid(mWidth, mHeight, mSize, mFloor);
        mWalls = new List<Wall>();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
            mFlagLeftClickPressed = true;
        
        if(Input.GetMouseButtonUp(0))
            mFlagLeftClickPressed = false;
        
        if (Input.GetMouseButtonDown(1))
            mFlagRightClickPressed = true;
        
        if (Input.GetMouseButtonUp(1))
            mFlagRightClickPressed = false;


        if (mFlagLeftClickPressed)
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.transform.gameObject.CompareTag("CubesForGround") || hit.transform.gameObject.CompareTag("CubesForLevel"))
                    && hit.transform.childCount == 0)
                {
                    Transform transform = hit.transform;
                    GameObject new_cube = mGrid.instantiateCube((int)(transform.position.x / mSize), (int)(transform.position.z / mSize), hit.transform.gameObject);
                    new_cube.transform.tag = "CubesForLevel";
                    updateWallsOnModification();
                }
            }
        }

        if (mFlagRightClickPressed)
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("CubesForLevel"))
                {
                    Destroy(hit.transform.gameObject);
                    updateWallsOnModification();
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            for (int i = 0; i < mWalls.Count; ++i)
            {
                mWalls[i].clearShadowQuad();
            }

            for (int i = 0; i < mFloor.transform.childCount; ++i)
            {
                if(mFloor.transform.GetChild(i).childCount == 0)
                {
                    getShadowFromObject(mFloor.transform.GetChild(i).gameObject);
                }
            }

            for(int i = 0; i < mWalls.Count; ++i)
            {
                mWalls[i].drawShadowQuad();
            }
        }
    }

    void updateWallsOnModification()
    {
        mWalls.Clear();
        for (int index_ground = 0; index_ground < mFloor.transform.childCount; ++index_ground)
        {
            GameObject ground_cube = mFloor.transform.GetChild(index_ground).gameObject;
            if (ground_cube.transform.childCount > 0)
            {
                GameObject child_wall = ground_cube.transform.GetChild(0).gameObject;

                bool already_seen = false;

                // Check if the cube is already in a neighbor
                for (int index_seen_neighbors = 0; index_seen_neighbors < mWalls.Count; index_seen_neighbors++)
                {
                    if (mWalls[index_seen_neighbors].getWalls().Contains(child_wall))
                        already_seen = true;
                }

                // If not, get the new neighbor
                if (!already_seen)
                {
                    Wall neighbor_walls = new Wall();
                    neighbor_walls.addWall(child_wall);

                    for(int index_wall = 0; index_wall < neighbor_walls.getWalls().Count; ++index_wall)
                    {
                        List<GameObject> local_neighbor = mUtils.getNeighborHood(neighbor_walls.getWalls()[index_wall], mSize);

                        for (int index_local_wall = 0; index_local_wall < local_neighbor.Count; ++index_local_wall)
                        {
                            if(!neighbor_walls.getWalls().Contains(local_neighbor[index_local_wall]))
                            {
                                neighbor_walls.addWall(local_neighbor[index_local_wall]);
                            }
                        }
                    }

                    mWalls.Add(neighbor_walls);
                }
            }
        }

    }

    public void getShadowFromObject(GameObject gameobject)
    {
        Vector3 pos = gameobject.transform.position;
        float midSize = mSize / 2f;
        Vector3 pos_00 = new Vector3(pos.x - midSize, pos.y + midSize - 0.01f, pos.z - midSize);
        Vector3 pos_01 = new Vector3(pos.x + midSize, pos.y + midSize - 0.01f, pos.z - midSize);
        Vector3 pos_10 = new Vector3(pos.x + midSize, pos.y + midSize - 0.01f, pos.z + midSize);
        Vector3 pos_11 = new Vector3(pos.x - midSize, pos.y + midSize - 0.01f, pos.z + midSize);

        List<Vector3> list_postitions = new List<Vector3>();
        list_postitions.Add(pos_00);
        list_postitions.Add(pos_01);
        list_postitions.Add(pos_10);
        list_postitions.Add(pos_11);

        quadTree(list_postitions, 0);
    }

    void quadTree(List<Vector3> pos, int subdivision)
    {
        List<List<Vector3>> next_quad = new List<List<Vector3>>();

        if(subdivision < SUBDIVISION_MAX)
        {
            Debug.DrawLine(pos[0], pos[1], Color.red, 4f);
            Debug.DrawLine(pos[1], pos[2], Color.red, 4f);
            Debug.DrawLine(pos[2], pos[3], Color.red, 4f);
            Debug.DrawLine(pos[3], pos[0], Color.red, 4f);

            bool should_go_deeper = false;
            int all_vertices_on_shadow = 1;

            int index_current_wall = -1;

            List<Vector3> quad_vertices = new List<Vector3>();
            for (int i = 0; i < pos.Count; ++i)
            {
                List<Vector3> quad_new_vertices = new List<Vector3>();
                quad_new_vertices.Add(pos[i]);
                quad_new_vertices.Add((pos[i] + pos[(i + 1) % pos.Count])/2);
                quad_new_vertices.Add((pos[i] + pos[(i + 2) % pos.Count])/2);
                quad_new_vertices.Add((pos[i] + pos[(i + 3) % pos.Count])/2);

                GameObject cube_emitting_shadows = mUtils.isInShadow(pos[i], mEmitter);

                if (cube_emitting_shadows)
                {
                    should_go_deeper = true;

                    // First time we're checking on which Wall neigbhorhood we are 
                    if (index_current_wall == -1)
                    {
                        for(int j = 0; j < mWalls.Count; ++j)
                        {
                            if(mWalls[j].getWalls().Contains(cube_emitting_shadows))
                            {
                                index_current_wall = j;
                            }
                        }
                    }

                    // Add the vertice into the wall's shadow quad
                    quad_vertices.Add(pos[i]);

                    if(mWalls[index_current_wall].getWalls().Contains(cube_emitting_shadows))
                    {
                        all_vertices_on_shadow <<= 1; // If 4 vertices are into shadow, stop going deeper
                    }
                }

                next_quad.Add(quad_new_vertices);
            }
            if(index_current_wall != -1)
                mWalls[index_current_wall].addShadowQuad(quad_vertices);

            if(subdivision < SUBDIVISION_MIN || should_go_deeper && all_vertices_on_shadow != 16)
            {
                for (int i = 0; i < next_quad.Count; ++i)
                {
                    quadTree(next_quad[i], subdivision + 1);
                }
            }

        }
    }


}
