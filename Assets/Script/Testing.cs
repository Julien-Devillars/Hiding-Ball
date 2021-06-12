using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    public int width;
    public int height;
    public float size;
    public GameObject ground;

    public Camera cam;
    private Grid grid;

    public int SUBDIVISION_MAX = 2;
    public GameObject emitter;

    private List<List<Vector3>> list_quads;

    private void Start()
    {
        grid = new Grid(width, height, size, ground);
        list_quads = new List<List<Vector3>>();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
           Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("CubesForGround") || hit.transform.gameObject.CompareTag("CubesForLevel"))
                {
                    Transform transform = hit.transform;
                    GameObject new_cube =  grid.instantiateCube((int)(transform.position.x / size), (int)(transform.position.z / size), hit.transform.gameObject);
                    new_cube.transform.tag = "CubesForLevel";
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("CubesForLevel"))
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(2))
        {

            list_quads.Clear();
            for (int i = 0; i < ground.transform.childCount; ++i)
            {
                if(ground.transform.GetChild(i).childCount == 0)
                {
                    getShadowFromObject(ground.transform.GetChild(i).gameObject);
                }
            }

            for (int i = 0; i < list_quads.Count; i++)
            {
                for (int j = 0; j < list_quads[i].Count; ++j)
                {
                    for(int k = 0; k < 5; k++)
                        Debug.DrawLine(list_quads[i][j], list_quads[i][(j + 1) % list_quads[i].Count], Color.white, 4f);
                }
            }
        }
    }

    
    public void getShadowFromObject(GameObject gameobject)
    {
        Vector3 pos = gameobject.transform.position;
        float midSize = size / 2f;
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
        List<List<Vector3>> quad = new List<List<Vector3>>();
        float sizeSubdivision = size / subdivision;

        if(subdivision < SUBDIVISION_MAX)
        {
            Debug.DrawLine(pos[0], pos[1], Color.red, 4f);
            Debug.DrawLine(pos[1], pos[2], Color.red, 4f);
            Debug.DrawLine(pos[2], pos[3], Color.red, 4f);
            Debug.DrawLine(pos[3], pos[0], Color.red, 4f);

            bool should_go_deeper = false;
            int all_vertices_on_shadow = 1;

            List<GameObject> neighbors = new List<GameObject>();

            List<Vector3> pos_in_shadows = new List<Vector3>();
            for (int i = 0; i < pos.Count; ++i)
            {
                List<Vector3> new_pos = new List<Vector3>();
                new_pos.Add(pos[i]);
                new_pos.Add((pos[i] + pos[(i + 1) % pos.Count])/2);
                new_pos.Add((pos[i] + pos[(i + 2) % pos.Count])/2);
                new_pos.Add((pos[i] + pos[(i + 3) % pos.Count])/2);

                GameObject cube_emitting_shadows = isInShadow(pos[i]);

                if (cube_emitting_shadows)
                {
                    if (neighbors.Count == 0)
                    {
                        neighbors = getNeighborHood(cube_emitting_shadows);
                    }
                    pos_in_shadows.Add(pos[i]);
                    should_go_deeper = true;         
                    if(neighbors.Contains(cube_emitting_shadows))
                    {
                        all_vertices_on_shadow <<= 1; // If 4 vertices are into shadow, stop going deeper
                    }
                }

                quad.Add(new_pos);
            }

            list_quads.Add(pos_in_shadows);

            if(should_go_deeper && all_vertices_on_shadow != 16)
            {
                for (int i = 0; i < quad.Count; ++i)
                {
                    quadTree(quad[i], subdivision + 1);
                }
            }

        }
    }

    GameObject isInShadow(Vector3 source)
    {
        Vector3 fromPosition = source;
        Vector3 pos_emitter = emitter.gameObject.transform.position;

        Vector3 toPosition = pos_emitter;

        Vector3 direction = toPosition - fromPosition;

        RaycastHit hit;

        if (Physics.Raycast(fromPosition, direction, out hit) == GameObject.FindGameObjectWithTag("CubesForLevel"))
        {
            if (hit.transform.CompareTag("CubesForLevel"))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

    List<GameObject> getNeighborHood(GameObject original_object)
    {
        List<GameObject> neighbors = new List<GameObject>();

        // Adding the original object
        neighbors.Add(original_object);

        List<Vector3> directions = getAllDirectionFromGameObject(original_object);

        // Add SideNeighbors
        for (int i = 0; i < directions.Count; ++i)
        {
            GameObject side_neighbor = getSideNeighbor(original_object, directions[i]);
            if(side_neighbor)
            {
                neighbors.Add(side_neighbor);
            }
        }

        return neighbors;
    }

    public int flag_left_right = 0;
    public int flag_front_back = 0;
    public int flag_top_down = 0;

    List<Vector3> getAllDirectionFromGameObject(GameObject original_object)
    {
        List<Vector3> directions = new List<Vector3>();

        for (int left_right = -1; left_right <= 1; ++ left_right)
        {
            for (int front_back = -1; front_back <= 1; ++front_back)
            {
                for (int top_down = -1; top_down <= 1; ++top_down)
                {
                    Vector3 direction = 
                            left_right  * original_object.transform.right * flag_left_right
                        +   front_back  * original_object.transform.forward * flag_front_back
                        +   top_down    * original_object.transform.up * flag_top_down;

                    directions.Add(direction);
                }
            }
        }
        return directions;
    }

    GameObject getSideNeighbor(GameObject original_object, Vector3 direction)
    {
        Vector3 fromPosition = original_object.transform.position;

        RaycastHit hit;

        if (Physics.Raycast(fromPosition, direction, out hit, size))
        {
            if (hit.transform.CompareTag("CubesForLevel"))
            {
                return hit.transform.gameObject;
            }
        }

        return null;
    }

}
