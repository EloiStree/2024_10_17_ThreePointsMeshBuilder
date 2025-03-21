using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
namespace Eloi.ThreePoints
{

public class TriangleGeneratorMono : MonoBehaviour
{
    public string m_spaceName = "TriangleGenerator";
    public MeshFilter m_meshFilter;
    public Mesh m_mesh;
    [Range(0, 1)]
    public float m_rgbPercent = 0.1f;
     public bool m_clearAtAwake = true;

    [Header("Debug")]
    public List<STRUCT_ThreePointTriangle > m_triangles = new List<STRUCT_ThreePointTriangle>();
     ThreePointsTriangleDefault[] m_trianglesWithInfo = new ThreePointsTriangleDefault [0]; 
     Vector3[] m_pointMesh;
     int[] m_triangleMesh;

   
    public UnityEvent<Mesh> m_onColoringCall;
        public float m_errorAllowedAngle = 10;
    public float m_errorAllowedGroundDistance = 0.1f;

    [ContextMenu("Clear")]
    public void Clear()
    {
        m_triangles.Clear();
        m_trianglesWithInfo= new ThreePointsTriangleDefault[0];
        UpdateMesh();
    }
 
    [ContextMenu("Add Triangle for testing")]
    public void AddRandomTriangleForTesting() { 
    
        ThreePointsTriangleDefault triangle = new ThreePointsTriangleDefault();
        triangle.SetThreePoints(
            UnityEngine.Random.insideUnitSphere * 5
            , UnityEngine.Random.insideUnitSphere * 5
            , UnityEngine.Random.insideUnitSphere*5);
        AddTriangle(triangle);
    
    }

    private void Awake()
    {
        if (m_clearAtAwake)
        {
            Clear();
        }
    }


    public void AddTriangle(I_ThreePointsGet triangle) { 
    
        triangle.GetThreePoints(out Vector3 start, out Vector3 middle, out Vector3 end);
        m_triangles.Add(new STRUCT_ThreePointTriangle() {
            m_start = start,
            m_middle = middle,
            m_end = end
        });
        UpdateMesh();
    }

[ContextMenu("Update Mesh")]
public void UpdateMesh()
    {

        // Check if m_meshFilter is assigned
        if (m_meshFilter == null)
        {
            Debug.LogError("MeshFilter is not assigned.");
            return;
        }

        if(m_triangles.Count> m_trianglesWithInfo.Length)
        {
            m_trianglesWithInfo = new ThreePointsTriangleDefault[m_triangles.Count];
            for (int i = 0; i < m_triangles.Count; i++)
            {
                m_trianglesWithInfo[i] = new ThreePointsTriangleDefault();
            }
        }
        // Initialize mesh and set properties
        m_mesh = new Mesh()
        {
            name = m_spaceName,
            vertices = new Vector3[m_triangles.Count * 3],
            triangles = new int[m_triangles.Count * 3],
            normals = new Vector3[m_triangles.Count * 3],
            colors = new Color[m_triangles.Count * 3]
        };
        int []triangles= new int [m_triangles.Count * 3];
        Vector3[] vertices = new Vector3[m_triangles.Count * 3];
        Color[] colors = new Color[m_triangles.Count * 3];
        Vector3[] normals=  new Vector3[m_triangles.Count * 3];

        
        

        // Populate vertices, triangles, normals, and colors
        for (int i = 0; i < m_triangles.Count; i++)
        {
            // Set vertices
            vertices[i * 3] = m_triangles[i].m_start;
            vertices[i * 3 + 1] = m_triangles[i].m_middle;
            vertices[i * 3 + 2] = m_triangles[i].m_end;

            // Set triangles
            triangles[i * 3] = i * 3;
            triangles[i * 3 + 1] = i * 3 + 1;
            triangles[i * 3 + 2] = i * 3 + 2;

            // Calculate and set normals
            Vector3 normal = Vector3.Cross(
                m_triangles[i].m_middle - m_triangles[i].m_start,
                m_triangles[i].m_end - m_triangles[i].m_start).normalized;

            normals[i * 3] = normal;
            normals[i * 3 + 1] = normal;
            normals[i * 3 + 2] = normal;

            // Assign colors (example: different colors per vertex)


            m_trianglesWithInfo[i].SetThreePoints(m_triangles[i].m_start, m_triangles[i].m_middle, m_triangles[i].m_end);

            I_ThreePointsDistanceAngleGet t = m_trianglesWithInfo[i];
            
            
        }

        // Recalculate bounds and normals
        m_mesh.vertices = vertices;
        m_mesh.triangles = triangles;
        m_pointMesh= m_mesh.vertices;
        m_triangleMesh = m_mesh.triangles;
        m_mesh.normals = normals;
 
        
        //m_mesh.bounds = new Bounds(Vector3.zero, new Vector3(65, 65, 65));
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();

            m_onColoringCall.Invoke(m_mesh);
          //  TriangleVertexColoring.BasicVerticalHorizontalColor(ref m_mesh,
          //m_rgbPercent,
          //m_errorAllowedAngle,
          //m_errorAllowedGroundDistance);
            // Assign mesh to the mesh filter component
            m_meshFilter.mesh = m_mesh;

        // Draw debug lines to visualize triangles in the editor
        for (int i = 0; i < m_triangles.Count; i++)
        {
            //Draw lines from the mesh vertices
            Debug.DrawLine(m_mesh.vertices[i * 3], m_mesh.vertices[i * 3 + 1], Color.green);
            Debug.DrawLine(m_mesh.vertices[i * 3 + 1], m_mesh.vertices[i * 3 + 2], Color.red);
            Debug.DrawLine(m_mesh.vertices[i * 3 + 2], m_mesh.vertices[i * 3], Color.blue);

        }
    }

}

}