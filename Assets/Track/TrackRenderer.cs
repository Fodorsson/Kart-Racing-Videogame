using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRenderer : MonoBehaviour
{
    private static List<Vector3> RoadVertices;
    private static List<int> RoadTriangles;
    private static List<Vector2> RoadUVs;

    private static List<int> first, second, third = new List<int>();

    private static List<Vector3> WallVertices;
    private static List<int> WallTriangles;
    private static List<Vector2> WallUVs;

    public static Mesh RoadMesh;

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    void Awake()
    {
        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();

    }

    public static void TriangulateRoad(List<Vector3> srcVertices)
    {

        //VERTICES
        RoadVertices = new List<Vector3>();

        //Fill up the list with the new lines' vertices

        RoadVertices.Add(srcVertices[0]);
        //
        RoadVertices.Add((srcVertices[0] + srcVertices[1]) / 2);

        RoadVertices.Add(srcVertices[1]);

        for (int i = 2; i < srcVertices.Count - 1; i += 2)
        {
            RoadVertices.Add(srcVertices[i]);
            //
            RoadVertices.Add((srcVertices[i] + srcVertices[i + 1]) / 2);

            RoadVertices.Add(srcVertices[i + 1]);
            RoadVertices.Add(srcVertices[i]);
            //
            RoadVertices.Add((srcVertices[i] + srcVertices[i + 1]) / 2);

            RoadVertices.Add(srcVertices[i + 1]);
        }

        RoadVertices.Add(srcVertices[0]);
        //
        RoadVertices.Add((srcVertices[0] + srcVertices[1]) / 2);

        RoadVertices.Add(srcVertices[1]);

        //

        //TRIANGLES
        RoadTriangles = new List<int>();

        //for (int i = 0; i < 1; i += 6)
        for (int i = 0; i < RoadVertices.Count - 3; i += 6)
        {
            RoadTriangles.Add(i);
            RoadTriangles.Add(i + 1);
            RoadTriangles.Add(i + 3);

            RoadTriangles.Add(i + 3);
            RoadTriangles.Add(i + 1);
            RoadTriangles.Add(i + 4);

            RoadTriangles.Add(i + 1);
            RoadTriangles.Add(i + 2);
            RoadTriangles.Add(i + 4);

            RoadTriangles.Add(i + 4);
            RoadTriangles.Add(i + 2);
            RoadTriangles.Add(i + 5);
        }

        //UVS
        RoadUVs = new List<Vector2>();

        for (int i = 0; i < RoadVertices.Count / 6; i++)
        {

            //The UV is made so that the texture repeats at the same length regardless of the length differences between each sections of the track
            float sectStart = TrackGenerator.offsets[i];
            float sectEnd = sectStart + TrackGenerator.distances[i];

            //BOTTOM RIGHT VERTEX
            RoadUVs.Add(new Vector2(1f, sectStart));

            //BOTTOM MID VERTEX
            RoadUVs.Add(new Vector2(0.5f, sectStart));

            //BOTTOM LEFT VERTEX
            RoadUVs.Add(new Vector2(0f, sectStart));

            //TOP RIGHT VERTEX
            RoadUVs.Add(new Vector2(1f, sectEnd));

            //TOP MID VERTEX
            RoadUVs.Add(new Vector2(0.5f, sectEnd));

            //TOP LEFT VERTEX
            RoadUVs.Add(new Vector2(0f, sectEnd));

        }

        //        Debug.Log("vert: " + vertices.Count);
        //        Debug.Log("uv: " + uvs.Count);

        //END
        GameObject Track = FindGO.Track;

        Track.AddComponent<MeshFilter>();
        Track.AddComponent<MeshRenderer>();
        Track.AddComponent<MeshCollider>();

        //GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        Track.GetComponent<MeshRenderer>().material = Resources.Load("MatGround", typeof(Material)) as Material;

        RoadMesh = new Mesh();

        RoadMesh.Clear();
        RoadMesh.vertices = RoadVertices.ToArray();
        RoadMesh.triangles = RoadTriangles.ToArray();
        RoadMesh.uv = RoadUVs.ToArray();
        RoadMesh.RecalculateNormals();

        Track.GetComponent<MeshFilter>().mesh = RoadMesh;

        Track.GetComponent<MeshCollider>().sharedMesh = Track.GetComponent<MeshFilter>().mesh;

        TriangulateWalls(srcVertices);
    }

    public static void TriangulateWalls(List<Vector3> srcVertices)
    {

        //VERTICES
        WallVertices = new List<Vector3>();

        for (int i = 0; i < srcVertices.Count; i++)
        {
            WallVertices.Add(srcVertices[i]);
            //Add the same vertex again, but a few units above
            WallVertices.Add(srcVertices[i] + new Vector3(0f, 5f, 0f));

            //Repeat, because every vertex has to have a duplicate for a correct UV
            WallVertices.Add(srcVertices[i]);
            WallVertices.Add(srcVertices[i] + new Vector3(0f, 5f, 0f));
        }

        //TRIANGLES
        WallTriangles = new List<int>();

        for (int i = 0; i < WallVertices.Count - 8; i += 8)
        {
            WallTriangles.Add(i);
            WallTriangles.Add(i + 8);
            WallTriangles.Add(i + 1);

            WallTriangles.Add(i + 1);
            WallTriangles.Add(i + 8);
            WallTriangles.Add(i + 9);

            WallTriangles.Add(i + 12);
            WallTriangles.Add(i + 4);
            WallTriangles.Add(i + 13);

            WallTriangles.Add(i + 13);
            WallTriangles.Add(i + 4);
            WallTriangles.Add(i + 5);
        }

        //UVS
        WallUVs = new List<Vector2>();

        for (int i = 0; i < WallVertices.Count / 8; i++)
        {

            //The UV is made so that the texture repeats at the same length regardless of the length differences between each sections of the track
            float sectStart = TrackGenerator.offsets[i];
            float sectEnd = sectStart + TrackGenerator.distances[i];

            //RIGHT SIDE NEARER BOTTOM VERTEX
            WallUVs.Add(new Vector2(sectStart, 0f));

            //RIGHT SIDE NEARER TOP VERTEX
            WallUVs.Add(new Vector2(sectStart, 1f));

            //LEFT SIDE NEARER BOTTOM VERTEX
            WallUVs.Add(new Vector2(sectEnd, 0f));

            //LEFT SIDE NEARER TOP VERTEX
            WallUVs.Add(new Vector2(sectEnd, 1f));

            //LEFT SIDE FARTHER BOTTOM VERTEX
            WallUVs.Add(new Vector2(sectStart, 0f));

            //LEFT SIDE FARTHER TOP VERTEX
            WallUVs.Add(new Vector2(sectStart, 1f));

            //RIGHT SIDE FARTHER BOTTOM VERTEX
            WallUVs.Add(new Vector2(sectEnd, 0f));

            //RIGHT SIDE FARTHER TOP VERTEX
            WallUVs.Add(new Vector2(sectEnd, 1f));

        }

        //Debug.Log("vert: " + WallVertices.Count);
        //Debug.Log("uv: " + WallUVs.Count);

        //END
        GameObject Walls = FindGO.Walls;

        Walls.AddComponent<MeshFilter>();
        Walls.AddComponent<MeshRenderer>();
        Walls.AddComponent<MeshCollider>();

        //GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        Walls.GetComponent<MeshRenderer>().material = Resources.Load("MatWalls", typeof(Material)) as Material;
        //Walls.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("tex_fence1", typeof(Texture)) as Texture;

        Mesh WallMesh = new Mesh();

        WallMesh.Clear();
        WallMesh.vertices = WallVertices.ToArray();
        WallMesh.triangles = WallTriangles.ToArray();
        WallMesh.uv = WallUVs.ToArray();
        WallMesh.RecalculateNormals();

        Walls.GetComponent<MeshFilter>().mesh = WallMesh;

        Walls.GetComponent<MeshCollider>().sharedMesh = Walls.GetComponent<MeshFilter>().mesh;
        Walls.GetComponent<MeshCollider>().material = Resources.Load("Slippery", typeof(PhysicMaterial)) as PhysicMaterial;

        //Copt the track for the minimap
        GameObject MinimapTrack = FindGO.MinimapTrack;

        MinimapTrack.AddComponent<MeshFilter>();
        MinimapTrack.AddComponent<MeshRenderer>();
        MinimapTrack.GetComponent<MeshRenderer>().material = Resources.Load("minimapTrackMat", typeof(Material)) as Material;

        MinimapTrack.GetComponent<MeshFilter>().mesh = RoadMesh;

    }



}
