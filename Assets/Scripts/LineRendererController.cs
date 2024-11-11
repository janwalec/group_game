using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer renderer;
    private Transform[] points;

    private void Awake()
    {
        renderer = GetComponent<LineRenderer>();

    }
    public void SetUpLine(Transform[] points)
    {
        renderer.positionCount = points.Length;
        this.points = points;
    }
    public void SetUpLine(Vector3[] points_)
    {
        renderer.positionCount = points_.Length;
        for (int i = 0; i < points_.Length; i++)
        {
            renderer.SetPosition(i, points_[i]);
        }

        //Debug.Log(renderer.positionCount + " position count");
        /*
        foreach(Vector3 point in points_)
        {
            Debug.Log(point + " point");
        }*/
    }

    public void SetUpLine(List<Vector3> points_)
    {
        renderer.positionCount = points_.Count;
        for (int i = 0; i < points_.Count; i++)
        {
            renderer.SetPosition(i, points_[i]);
        }

        /*Debug.Log(renderer.positionCount + " position count");
        foreach (Vector3 point in points_)
        {
            Debug.Log(point + " point");
        }*/
    }
    private void Update()
    {
        /*for(int i = 0; i < points.Length; i++) {
            renderer.SetPosition(i, points[i].position);
        }*/
    }

}