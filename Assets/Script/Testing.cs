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

    private void Start()
    {
        grid = new Grid(width, height, size, ground);
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
           Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                Transform transform = hit.transform;
                Debug.Log(transform.position);
                grid.instantiateCube((int)(transform.position.x/size), (int)(transform.position.z / size), hit.transform.gameObject);
            }
        }
    }

}
