using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GeometryGenerator : EditorWindow
{
    Texture2D basemap;
    Texture2D bathymetry;
    Texture2D topography;

    [MenuItem("Generator/Geometry")]
    static void Generate()
    {
        GeometryGenerator window = (GeometryGenerator)EditorWindow.GetWindow(typeof(GeometryGenerator));
        window.Show();
    }

    private void GenerateGeometry()
    {
        if (
            basemap.width != bathymetry.width ||
            bathymetry.width != topography.width ||
            topography.width != basemap.width)
        {
            Debug.LogError("Not equals texture's width");
            return;
        }
        if (
            basemap.height != bathymetry.height ||
            bathymetry.height != topography.height ||
            topography.height != basemap.height)
        {
            Debug.LogError("Not equals texture's height");
            return;
        }

        int vtx_count = basemap.height * basemap.width;
        float dtheta = (2f * Mathf.PI) / basemap.width;
        Vector3[] vertices = new Vector3[vtx_count];
        Color[] colors = new Color[vtx_count];
        Vector3[] normals = new Vector3[vtx_count];
        const float round_length = 40075f;
        // round_length = diameter * PI
        // round_length / PI = 2f * radius
        // round_length / 2f * PI = radius
        const float radius = round_length / (2f * Mathf.PI);

        for (int h = 0; h < basemap.height; ++h)
        {
            for (int w = 0; w < basemap.width; ++w)
            {
                int idx = h * basemap.height + w;
                float xsin = Mathf.Sin(dtheta * w);
                float ycos = Mathf.Cos(dtheta * w);
                float x = xsin * radius;
                float z = ycos * radius;
                vertices[idx] = new Vector3(x, h, z);
                colors[idx] = basemap.GetPixel(w, h);
                normals[idx] = new Vector3(x, 0, z);
            }
        }
    }

    private void OnGUI()
    {
        basemap = (Texture2D)EditorGUILayout.ObjectField("Basemap", basemap, typeof(Texture2D), true);
        topography = (Texture2D)EditorGUILayout.ObjectField("Topography", topography, typeof(Texture2D), true);
        bathymetry = (Texture2D)EditorGUILayout.ObjectField("Bathymetry", bathymetry, typeof(Texture2D), true);

        if (GUILayout.Button("Generate"))
        {
            GenerateGeometry();
        }
    }
}
