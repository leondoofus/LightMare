using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour {

    
    public bool dragging = false;
    public bool moving = false;
    public bool rotating = false;

    private float distance;
    public GameObject RotationCircle;
    public GameObject Handle;
    public bool selected=false;

    float PressedTime;
    const float ClickDuration = 0.1f; // maximum click duration 
    Rigidbody2D rb;


    Vector3 PositionOffset;
    Vector3 InitialPos;
    public float angleMouse0;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        InitialPos = transform.position;
        PositionOffset = rayPoint - transform.position;
        angleMouse0 = Mathf.Atan2(PositionOffset.y, PositionOffset.x) * Mathf.Rad2Deg;
        angleSet = transform.localEulerAngles.z ;

        //transform.GetComponent<Rigidbody2D>().mass = transform.GetComponent<Rigidbody2D>().mass / 10;

        PressedTime = Time.time;
    }

    void OnMouseUp()
    {
        if (Time.time < PressedTime + ClickDuration) // C'est un click !
        {
            OnMouseClick();
        }
        else
        {
           OnMouseEndDrag();
        }

    }

    void OnMouseClick()
    {
        selected = !selected;
        if (Handle) Handle.SetActive(selected);
    }

    private void OnMouseDrag()
    {
        if (!dragging && Time.time > PressedTime + ClickDuration)
        {
            OnMouseBeginDrag();
        }
    }

    private void OnMouseBeginDrag()
    {
        dragging = true;
        
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == RotationCircle)
        {
            rotating = true;
            rb.constraints = RigidbodyConstraints2D.None;
        }
        else
        {
            moving = true;
            transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void OnMouseEndDrag()
    {
        moving = false;
        rotating = false;
        dragging = false;
        //transform.GetComponent<Rigidbody2D>().mass = transform.GetComponent<Rigidbody2D>().mass * 10;

        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }



    float AngleFromXY(float x, float y)
    {
        float angle= Mathf.Atan2(y, x);
        return angle;
    }


    public float angleAct;
    public float angleSet;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);

        Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();


        if (rotating)
        {
            Vector2 f;
            f.x = rayPoint.x - transform.position.x;
            f.y = rayPoint.y - transform.position.y;

            float angleMouse1 = Mathf.Atan2(f.y, f.x) * Mathf.Rad2Deg; // entre -180 et 180
            float deltaAngle = Mathf.DeltaAngle(angleMouse0, angleMouse1);//(angleMouse1 - angleMouse0);
            angleMouse0 = angleMouse1;

            angleSet = (angleSet + deltaAngle * 0.3f);

            angleAct = transform.localEulerAngles.z;

            float angle = Mathf.DeltaAngle(angleAct, angleSet);
    
            rb.AddTorque(angle*0.01f);
           
        }

        if (moving)
        {
            
            if (rb)
            {
                Vector2 f;
                if (!selected)
                {
                    f.x = rayPoint.x - PositionOffset.x - transform.position.x;
                    f.y = rayPoint.y - PositionOffset.y - transform.position.y;
                }
                else
                {
                    float r;
                    Vector3 v = rayPoint - PositionOffset - InitialPos;
                    v.z = 0;

                    r = Mathf.Clamp01(0.1f+0.2f*v.magnitude);
                    f.x = (r * (rayPoint.x - PositionOffset.x) + (1 - r) * InitialPos.x) - transform.position.x;
                    f.y = (r * (rayPoint.y - PositionOffset.y) + (1 - r) * InitialPos.y) - transform.position.y;
                }

                transform.GetComponent<Rigidbody2D>().AddForce(10*f);

            }
            
        }
    }
}
