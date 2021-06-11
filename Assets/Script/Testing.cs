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
                if (hit.transform.gameObject.CompareTag("CubesForLevel"))
                {
                    Transform transform = hit.transform;
                    grid.instantiateCube((int)(transform.position.x / size), (int)(transform.position.z / size), hit.transform.gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("CubesForLevel") && hit.transform.position.y != size)
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
                getShadowFromObject(ground.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < list_quads.Count; i++)
            {
                for (int j = 0; j < list_quads[i].Count; ++j)
                {
                    for (int k = 0; k < 5; ++k)
                        Debug.DrawLine(list_quads[i][j], list_quads[i][(j + 1) % list_quads[i].Count], Color.white, 10f);
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
            Debug.DrawLine(pos[0], pos[1], Color.red, 10f);
            Debug.DrawLine(pos[1], pos[2], Color.red, 10f);
            Debug.DrawLine(pos[2], pos[3], Color.red, 10f);
            Debug.DrawLine(pos[3], pos[0], Color.red, 10f);

            bool should_go_deeper = false;
            int all_vertices_on_shadow = 1;
            GameObject same_emitting_cube = null;
            List<Vector3> pos_in_shadows = new List<Vector3>();
            for (int i = 0; i < pos.Count; ++i)
            {
                List<Vector3> new_pos = new List<Vector3>();
                new_pos.Add(pos[i]);
                new_pos.Add((pos[i] + pos[(i + 1) % pos.Count])/2);
                new_pos.Add((pos[i] + pos[(i + 2) % pos.Count])/2);
                new_pos.Add((pos[i] + pos[(i + 3) % pos.Count])/2);

                GameObject cube_emitting_shadows = isInShadow(pos[i]);
                if(!same_emitting_cube)
                {
                    same_emitting_cube = cube_emitting_shadows;
                }
                if (cube_emitting_shadows)
                {
                    pos_in_shadows.Add(pos[i]);
                    should_go_deeper = true;
                    if(cube_emitting_shadows == same_emitting_cube)
                    {
                        all_vertices_on_shadow <<= 1;
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

        if (Physics.Raycast(fromPosition, direction, out hit))
        {
            if (hit.transform.CompareTag("CubesForLevel"))
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }

}
