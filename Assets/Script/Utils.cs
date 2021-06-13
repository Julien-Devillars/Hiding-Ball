using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{

    public List<GameObject> getNeighborHood(GameObject original_object, float distance)
    {
        List<GameObject> neighbors = new List<GameObject>();

        // Adding the original object
        neighbors.Add(original_object);

        List<Vector3> directions = getAllDirectionFromGameObject(original_object);

        // Add SideNeighbors
        for (int i = 0; i < directions.Count; ++i)
        {
            GameObject side_neighbor = getSideNeighbor(original_object, directions[i], distance);
            if (side_neighbor)
            {
                neighbors.Add(side_neighbor);
            }
        }

        return neighbors;
    }

    public List<Vector3> getAllDirectionFromGameObject(GameObject original_object)
    {
        List<Vector3> directions = new List<Vector3>();

        for (int left_right = -1; left_right <= 1; ++left_right)
        {
            for (int front_back = -1; front_back <= 1; ++front_back)
            {
                for (int top_down = -1; top_down <= 1; ++top_down)
                {
                    Vector3 direction =
                          left_right * original_object.transform.right
                        + front_back * original_object.transform.forward
                        + top_down * original_object.transform.up;

                    directions.Add(direction);
                }
            }
        }
        return directions;
    }

    public GameObject getSideNeighbor(GameObject original_object, Vector3 direction, float distance)
    {
        Vector3 fromPosition = original_object.transform.position;

        RaycastHit hit;

        if (Physics.Raycast(fromPosition, direction, out hit, distance))
        {
            if (hit.transform.CompareTag("CubesForLevel"))
            {
                return hit.transform.gameObject;
            }
        }

        return null;
    }


    public GameObject isInShadow(Vector3 source, GameObject emitter)
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
