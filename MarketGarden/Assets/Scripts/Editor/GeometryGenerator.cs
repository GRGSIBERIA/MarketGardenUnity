using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GeometryGenerator : EditorWindow
{
    Texture basemap;
    Texture bathymetry;
    Texture topography;

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

        int vtx_count = basemap.height * basemap.width * 4;
        float dtheta = (2f * Mathf.PI) / basemap.width;
        Vector3[] vertices = new Vector3[vtx_count];
        const float round_length = 40075f;
        // round_length = diameter * PI
        // round_length / PI = 2f * radius
        // round_length / 2f * PI = radius
        const float radius = round_length / (2f * Mathf.PI);
        

        for (int h = 0; h < basemap.height; ++h)
        {
            for (int w = 0; w < basemap.width; ++w)
            {
                int idx = h * basemap.height + w * 4;
                vertices[idx + 0] = new Vector3();
                vertices[idx + 1] = new Vector3();
                vertices[idx + 2] = new Vector3();
                vertices[idx + 3] = new Vector3();
            }
        }
    }

    private void OnGUI()
    {
        basemap = (Texture)EditorGUILayout.ObjectField("Basemap", basemap, typeof(Texture), true);
        topography = (Texture)EditorGUILayout.ObjectField("Topography", topography, typeof(Texture), true);
        bathymetry = (Texture)EditorGUILayout.ObjectField("Bathymetry", bathymetry, typeof(Texture), true);

        if (GUILayout.Button("Generate"))
        {
            GenerateGeometry();
        }
    }
}
