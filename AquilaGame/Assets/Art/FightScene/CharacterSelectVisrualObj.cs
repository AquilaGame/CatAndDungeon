using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectVisrualObj : MonoBehaviour
{
    static Vector3 SurfaceDelta = new Vector3(0, 0.03f, 0);
    public void Set(Vector3 pos)
    {
        transform.position = pos + SurfaceDelta;
    }
    public void Set(Vector3 pos, float size)
    {
        transform.position = pos + SurfaceDelta;
        transform.localScale = new Vector3(size, size, 0);
    }
    public void Set(Vector3 pos, float size, Vector3 startPoint, float Maxdistance)
    {
        Vector2 p0 = new Vector2(startPoint.x, startPoint.z);
        Vector2 p1 = new Vector2(pos.x, pos.z);
        Vector2 delta = p1 - p0;
        if (delta.magnitude > Maxdistance)
        {
            p1 = p0 + delta.normalized * Maxdistance;
        }
        Vector3 v;
        if (Physics.Raycast(new Vector3(p1.x, 10, p1.y), Vector3.down,out RaycastHit hit, 20, LayerMask.GetMask("Default")))
        {
            v = hit.point + SurfaceDelta;
        }
        else
        {
            v = new Vector3(p1.x, startPoint.y, p1.y) + SurfaceDelta;
        }
        transform.position = v;
        transform.localScale = new Vector3(size, size, 0);
        }
}
