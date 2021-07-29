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
                int idx = h * basemap.width + w;
                float xsin = Mathf.Sin(dtheta * w);
                float ycos = Mathf.Cos(dtheta * w);
                float x = xsin * radius;
                float z = ycos * radius;
                vertices[idx] = new Vector3(x, h, z);
                colors[idx] = basemap.GetPixel(w, h);
                normals[idx] = new Vector3(xsin, 0, ycos);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors = colors;

        int polygon_count = (basemap.width - 1) * (basemap.height - 1);
        int triangle_count = polygon_count * 6;
        int[] triangles = new int[triangle_count];
        for (int h = 0; h < basemap.height - 1; ++h)
        {
            for (int w = 0; w < basemap.width - 1; ++w)
            {
                int a = basemap.width * h + w;
                int b = basemap.width * h + w + 1;
                int c = basemap.width * (h + 1) + w;
                int d = basemap.width * (h + 1) + w + 1;
                int idx = h * basemap.width + w;
                triangles[idx * 6 + 0] = a;
                triangles[idx * 6 + 1] = b;
                triangles[idx * 6 + 2] = c;
                triangles[idx * 6 + 3] = b;
                triangles[idx * 6 + 4] = d;
                triangles[idx * 6 + 5] = c;
            }
        }
        mesh.triangles = triangles;
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
