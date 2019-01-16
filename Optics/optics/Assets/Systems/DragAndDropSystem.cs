using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;

public class DragAndDropSystem : FSystem {
    
    private Family _ddGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)), new NoneOfComponents(typeof(LightSource), typeof(Target)));
    private Family _dd = FamilyManager.getFamily(new AllOfComponents(typeof(DragAndDrop)));
    private Family _ss = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)), new AnyOfComponents(typeof(LightSource),typeof(Target)));
    private bool created = false;

    public DragAndDropSystem()
    {
        foreach (GameObject go in _dd)
        {
            go.GetComponent<DragAndDrop>().rb = go.GetComponent<DragAndDrop>().transform.GetComponent<Rigidbody2D>();
            created = true;
        }
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        if (!created)
        {
            if (_dd.First() == null) return;
            else
            {
                foreach (GameObject g in _dd)
                {
                    g.GetComponent<DragAndDrop>().rb = g.GetComponent<DragAndDrop>().transform.GetComponent<Rigidbody2D>();
                    created = true;
                }
            }
        }
        GameObject go = _ddGO.First();
        if (go != null)
        {
            DragAndDrop dd = go.GetComponent<DragAndDrop>();
            if (Input.GetMouseButtonDown(0))
            {
                foreach (GameObject go2 in _dd)
                {
                    if (!go2.Equals(go))
                    {
                        if (go2.GetComponent<DragAndDrop>().dragging) return;
                        DragAndDrop ddcancel = go2.GetComponent<DragAndDrop>();
                        ddcancel.dragging = false;
                        ddcancel.moving = false;
                        ddcancel.rotating = false;
                        ddcancel.selected = false;
                        if (ddcancel.Handle) ddcancel.Handle.SetActive(ddcancel.selected);
                    }
                }

                dd.distance = Vector3.Distance(dd.transform.position, Camera.main.transform.position);
                Ray ray_tmp = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint_tmp = ray_tmp.GetPoint(dd.distance);
                dd.InitialPos = dd.transform.position;
                dd.PositionOffset = rayPoint_tmp - dd.transform.position;
                dd.angleMouse0 = Mathf.Atan2(dd.PositionOffset.y, dd.PositionOffset.x) * Mathf.Rad2Deg;
                dd.angleSet = dd.transform.localEulerAngles.z;

                dd.PressedTime = Time.time;
                GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>().log("Mouse Down;"+go.name+";"+go.transform.position.x+";"+go.transform.position.y+";"
                                                                                    +go.transform.rotation.x+";"+go.transform.rotation.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (Time.time < dd.PressedTime + dd.ClickDuration) // C'est un click !
                {
                    dd.selected = !dd.selected;
                    if (dd.Handle) dd.Handle.SetActive(dd.selected);
                    GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>().log("Mouse Up;"+go.name+";"+"selected="+dd.selected);
                }
                else
                {
                    dd.moving = false;
                    dd.rotating = false;
                    dd.dragging = false;
                    dd.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    GameObject.Find("FYFYGameEngine").GetComponent<FYFYGameEngine>().log("Mouse Up;" + go.name + ";" + go.transform.position.x + ";" + go.transform.position.y + ";"
                                                                                        + go.transform.rotation.x + ";" + go.transform.rotation.y);
                }
            }
            if (Input.GetMouseButton(0))
            {
                foreach (GameObject go2 in _dd)
                {
                    if (!go2.Equals(go))
                    {
                        if (go2.GetComponent<DragAndDrop>().dragging) return;
                    }
                }
                if (!dd.dragging && Time.time > dd.PressedTime + dd.ClickDuration)
                {
                    dd.dragging = true;

                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == dd.RotationCircle)
                    {
                        dd.rotating = true;
                        dd.rb.constraints = RigidbodyConstraints2D.None;
                    }
                    else
                    {
                        dd.moving = true;
                        dd.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint = ray.GetPoint(dd.distance);

                Rigidbody2D rb = dd.transform.GetComponent<Rigidbody2D>();

                if (dd.rotating)
                {
                    Vector2 f;
                    f.x = rayPoint.x - dd.transform.position.x;
                    f.y = rayPoint.y - dd.transform.position.y;

                    float angleMouse1 = Mathf.Atan2(f.y, f.x) * Mathf.Rad2Deg; // entre -180 et 180
                    float deltaAngle = Mathf.DeltaAngle(dd.angleMouse0, angleMouse1);//(angleMouse1 - angleMouse0);
                    dd.angleMouse0 = angleMouse1;

                    dd.angleSet = (dd.angleSet + deltaAngle * 0.3f);

                    dd.angleAct = dd.transform.localEulerAngles.z;

                    float angle = Mathf.DeltaAngle(dd.angleAct, dd.angleSet);

                    rb.AddTorque(angle * 0.01f);

                }

                if (dd.moving)
                {
                    if (_ss.First() != null) return;
                    if (rb)
                    {
                        Vector2 f;
                        if (!dd.selected)
                        {
                            f.x = rayPoint.x - dd.PositionOffset.x - dd.transform.position.x;
                            f.y = rayPoint.y - dd.PositionOffset.y - dd.transform.position.y;
                        }
                        else
                        {
                            float r;
                            Vector3 v = rayPoint - dd.PositionOffset - dd.InitialPos;
                            v.z = 0;

                            r = Mathf.Clamp01(0.1f + 0.2f * v.magnitude);
                            f.x = (r * (rayPoint.x - dd.PositionOffset.x) + (1 - r) * dd.InitialPos.x) - dd.transform.position.x;
                            f.y = (r * (rayPoint.y - dd.PositionOffset.y) + (1 - r) * dd.InitialPos.y) - dd.transform.position.y;
                        }
                        dd.transform.GetComponent<Rigidbody2D>().AddForce(10 * f);
                    }
                }
            }
        }
        foreach (GameObject go2 in _dd)
        {
            if (Input.GetMouseButton(0))
            {
                DragAndDrop dd2 = go2.GetComponent<DragAndDrop>();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint = ray.GetPoint(dd2.distance);

                Rigidbody2D rb = dd2.transform.GetComponent<Rigidbody2D>();

                if (dd2.moving)
                {
                    if (_ss.First() != null) return;
                    if (rb)
                    {
                        Vector2 f;
                        if (!dd2.selected)
                        {
                            f.x = rayPoint.x - dd2.PositionOffset.x - dd2.transform.position.x;
                            f.y = rayPoint.y - dd2.PositionOffset.y - dd2.transform.position.y;
                        }
                        else
                        {
                            float r;
                            Vector3 v = rayPoint - dd2.PositionOffset - dd2.InitialPos;
                            v.z = 0;

                            r = Mathf.Clamp01(0.1f + 0.2f * v.magnitude);
                            f.x = (r * (rayPoint.x - dd2.PositionOffset.x) + (1 - r) * dd2.InitialPos.x) - dd2.transform.position.x;
                            f.y = (r * (rayPoint.y - dd2.PositionOffset.y) + (1 - r) * dd2.InitialPos.y) - dd2.transform.position.y;
                        }
                        dd2.transform.GetComponent<Rigidbody2D>().AddForce(10 * f);
                    }
                }

                if (dd2.rotating)
                {
                    Vector2 f;
                    f.x = rayPoint.x - dd2.transform.position.x;
                    f.y = rayPoint.y - dd2.transform.position.y;

                    float angleMouse1 = Mathf.Atan2(f.y, f.x) * Mathf.Rad2Deg; // entre -180 et 180
                    float deltaAngle = Mathf.DeltaAngle(dd2.angleMouse0, angleMouse1);//(angleMouse1 - angleMouse0);
                    dd2.angleMouse0 = angleMouse1;

                    dd2.angleSet = (dd2.angleSet + deltaAngle * 0.3f);

                    dd2.angleAct = dd2.transform.localEulerAngles.z;

                    float angle = Mathf.DeltaAngle(dd2.angleAct, dd2.angleSet);

                    rb.AddTorque(angle * 0.01f);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                DragAndDrop dd2 = go2.GetComponent<DragAndDrop>();
                if (dd2.rotating)
                {
                    //dd2.moving = false;
                    //dd2.rotating = false;
                    dd2.dragging = false;
                    //dd2.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }   
        }
	}
}