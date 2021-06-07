using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{

    private int width;
    private int height;
    private float cell_size;
    private int[,] gridArray;

    public Grid(int width, int height, float cell_size, GameObject Ground)
    {
        this.width = width/2;
        this.height = height/2;
        this.cell_size = cell_size;

        gridArray = new int[width, height];

        for(int x = -gridArray.GetLength(0)/2; x < gridArray.GetLength(0)/2; x++)
        {
            for(int z = -gridArray.GetLength(1)/2; z < gridArray.GetLength(1)/2; z++)
            {
                instantiateCube(x, z, Ground);
            }
        }
    }

    private Vector3 getWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cell_size;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    public void instantiateCube(int x, int z, GameObject parent)
    {
        // Position
        
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 pos2D = getWorldPosition(x, z);
        Debug.Log(pos2D);
        cube.transform.position = new Vector3(pos2D.x, parent.transform.position.y + cell_size, pos2D.z);  
        cube.transform.localScale = new Vector3(cell_size, cell_size, cell_size);
        cube.transform.parent = parent.transform;

        // Colors

        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        Vector3 pos = cube.transform.position.normalized;
        Color color = new Color(pos.x, pos.y, pos.z);
        cubeRenderer.material.SetColor("_Color", color);

        // RigidBody

        Rigidbody rigidbody = cube.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        // Collider

        BoxCollider boxCollider = cube.AddComponent<BoxCollider>();
    }
}
