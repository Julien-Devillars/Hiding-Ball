using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{

    private int mWidth;
    private int mHeight;
    private float mCellSize;
    private int[,] mGridArray;

    public Grid(int width, int height, float cell_size, GameObject Ground)
    {
        this.mWidth = width/2;
        this.mHeight = height/2;
        this.mCellSize = cell_size;

        mGridArray = new int[width, height];

        for(int x = -mGridArray.GetLength(0)/2; x < mGridArray.GetLength(0)/2; x++)
        {
            for(int z = -mGridArray.GetLength(1)/2; z < mGridArray.GetLength(1)/2; z++)
            {
                instantiateCube(x, z, Ground);
            }
        }
    }

    private Vector3 getWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * mCellSize;
    }

    public int getWidth()
    {
        return mWidth;
    }

    public int getHeight()
    {
        return mHeight;
    }

    public GameObject instantiateCube(int x, int z, GameObject parent)
    {
        // Position
        
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 pos2D = getWorldPosition(x, z);
        cube.transform.position = new Vector3(pos2D.x, parent.transform.position.y + mCellSize, pos2D.z);  
        cube.transform.localScale = new Vector3(mCellSize, mCellSize, mCellSize);
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

        // Tag

        cube.gameObject.tag = "CubesForGround";

        return cube;
    }

}
