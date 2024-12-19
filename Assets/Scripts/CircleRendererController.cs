using System.Collections.Generic;
using UnityEngine;

public class CircleRendererController : MonoBehaviour
{
    private LineRenderer renderer;

    public int segments = 50;

    private void Start()
    {
        
        renderer = GetComponent<LineRenderer>();
        Debug.Log("Is renderer null  " +  renderer == null);
        renderer.positionCount = 50;
    }
    public void SetUpLine(Transform center, float radius)
    {
        renderer = GetComponent<LineRenderer>();
        Debug.Log("Is renderer null  " + renderer == null);
        renderer.positionCount = 51;

        float angle = 20f;
        float x;
        float y;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius + center.transform.position.x;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius + center.transform.position.y;

            renderer.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
    /*
    public void SetUpLine(Vector3 center_, float radius)
    {
        renderer.positionCount = points_.Length;
        for (int i = 0; i < points_.Length; i++)
        {
            renderer.SetPosition(i, points_[i]);
        }
    }

    public void SetUpLine(List<Vector3> center_)
    {
        renderer.positionCount = points_.Count;
        for (int i = 0; i < points_.Count; i++)
        {
            renderer.SetPosition(i, points_[i]);
        }

    }*/
   

}