using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    public float mBallSpeed = 10.0f;
    public float mCloseness = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 mPosition;
    public Vector3 mDestination;
    private GameObject mDestinationGameObject;
    public Camera mCamera;

    // Update is called once per frame
    void Update()
    {

        float x_speed = Input.GetAxis("Horizontal");
        float z_speed = Input.GetAxis("Vertical");

        Rigidbody body = GetComponent<Rigidbody>();
        body.AddForce(new Vector3(x_speed, 0, z_speed) * mBallSpeed * Time.deltaTime);



        if(Input.GetMouseButtonDown(2))
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(closeTodestination(mPosition, mDestination));

                List<List<Vector3>> shadow_quads = Testing.mWalls[Random.Range(0, Testing.mWalls.Count)].getShadowQuads();
                int index_random_quad = -1;
                int index_random_shadow = -1;

                if (shadow_quads.Count !=0)
                    index_random_quad = Random.Range(0, shadow_quads.Count);

                if(index_random_quad != -1 && shadow_quads[index_random_quad].Count != 0)
                    index_random_shadow = Random.Range(0, shadow_quads[index_random_quad].Count);

                if (index_random_quad != -1 && index_random_shadow != -1)
                    mDestination = shadow_quads[index_random_quad][index_random_shadow];

                Destroy(mDestinationGameObject);
                mDestinationGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                BoxCollider collider = mDestinationGameObject.GetComponent<BoxCollider>();
                Destroy(collider);
                mDestinationGameObject.transform.position = mDestination;
                mDestinationGameObject.transform.localScale = new Vector3(mCloseness, mCloseness, mCloseness);
            }
        }

        moveToPosition(mDestination);
    }

    void moveToPosition(Vector3 destination)
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (closeTodestination(this.transform.position, destination, 0.25f))
        {
            body.velocity = body.velocity  * 0.95f * Time.deltaTime;
        }
        else
        {
            Vector3 direction = destination - this.transform.position;
            body.AddForce(direction * mBallSpeed * Time.deltaTime);
        }
        

    }

    bool closeTodestination(Vector3 position, Vector3 destination, float closeness)
    {

        
        return position.x > destination.x - closeness && position.x < destination.x + closeness
            && position.y > destination.y - closeness && position.y < destination.y + closeness
            && position.z > destination.z - closeness && position.z < destination.z + closeness;
    }
}
