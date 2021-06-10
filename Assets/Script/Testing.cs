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

    private List<Vector3> list_quads;

    private void Start()
    {
        grid = new Grid(width, height, size, ground);
        list_quads = new List<Vector3>();
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
                getShadowFromObject(ground.transform.GetChild(i).gameObject);
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
        for(int i = 0; i < list_quads.Count; i++)
        {
            Debug.DrawLine(list_quads[i], list_quads[(i+1)%list_quads.Count], Color.white, 10f);
        }
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

            for (int i = 0; i < pos.Count; ++i)
            {
                List<Vector3> new_pos = new List<Vector3>();
                new_pos.Add(pos[i]);
                new_pos.Add((pos[i] + pos[(i + 1) % pos.Count])/2);
                new_pos.Add((pos[i] + pos[(i + 2) % pos.Count])/2);
                new_pos.Add((pos[i] + pos[(i + 3) % pos.Count])/2);

                if (isInShadow(pos[i]))
                {
                    list_quads.Add(pos[i]);
                    should_go_deeper = true;
                }

                quad.Add(new_pos);
            }

            if(should_go_deeper)
            {
                for (int i = 0; i < quad.Count; ++i)
                {
                    quadTree(quad[i], subdivision + 1);
                }
            }

        }
    }

    bool isInShadow(Vector3 source)
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
                return true;
            }
        }
        return false;
    }

}
