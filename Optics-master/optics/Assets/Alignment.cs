using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : MonoBehaviour {

    bool dragging = false;
    float distance;
    public GameObject Target;
    

    Vector3 Offset;
    void OnMouseDown()
    {
        Debug.Log("touch!");
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        Offset = transform.position - rayPoint;

        dragging = true;


        RaycastHit2D[] hit = new RaycastHit2D[6];
        Vector2 rayP = new Vector2(rayPoint.x, rayPoint.y);
        if (GetComponent<Collider2D>().Raycast(rayP, hit) > 0)
        {
            Debug.Log("touch!");
        }
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void OnMouseDrag()
    {
        /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         Vector3 rayPoint = ray.GetPoint(distance);
         //rayPoint.z = 0;//transform.position.z;
         transform.position = rayPoint;
         //Debug.Log("drag!!");*/
    }

    void Update()
    {



        if (dragging||true)
        {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject==gameObject)
            {
                Debug.Log("Target Position: " + hit.collider.gameObject);
            }

            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            RaycastHit2D[] hit=new RaycastHit2D[6];
            Vector2 rayP = new Vector2( rayPoint.x,rayPoint.y);
            if (GetComponent<Collider2D>().Raycast(rayP, hit)>0)
            {
                Debug.Log("touch!");
            }*/

           /* Rigidbody2D rb = Target.transform.GetComponent<Rigidbody2D>();

            //if (rb)
            {
                Vector2 f = Vector2.zero;
                f.x = rayPoint.x + Offset.x - transform.position.x;
                f.y = rayPoint.y + Offset.y - transform.position.y;


                //transform.GetComponent<Rigidbody2D>().AddForce(f);
                //rb.AddTorque(f.x);
                Quaternion rot;
                //Target.transform.localRotation.z=90;
                Target.transform.rotation = Quaternion.AngleAxis(10*f.x, Vector3.forward);
            }

            //Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           */
            /*else
            {
                float z = transform.localPosition.z; // Conserve le z de l'objet

                transform.position = rayPoint + Offset;

                Vector3 locPos = transform.localPosition;



                locPos.z = z;
                transform.localPosition = locPos;
            }*/
        }
    }
}
