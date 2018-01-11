﻿using UnityEngine;

public static class HexMetrics{
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;

    //creates the vertices of a hexagon with the point facing up and down.
    //CHANGE this so that the point is left and right
    public static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f*outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        //this extra vertice is here to prevent an IndexOutOfRangeException
        //in the HexMesh.triangulate function
        new Vector3(0f, 0f, outerRadius)
    };

}