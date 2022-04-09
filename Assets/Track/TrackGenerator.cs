using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    private static int segments = 18;

    //List of the 3D coordinates of the checkpoints
    public Vector3[] points = new Vector3[segments];

    public List<Vector3> centerPoints = new List<Vector3>();
    public List<Vector3> outsidePoints = new List<Vector3>();

    //The prefab used as the marker
    public GameObject cubePrefab;

    private List<Vector3> catmullPoints = new List<Vector3>();

    private List<Vector3> catmullRight = new List<Vector3>();
    private List<Vector3> catmullLeft = new List<Vector3>();

    [SerializeField] public static List<float> distances = new List<float>();
    [SerializeField] public static List<float> offsets = new List<float>();

    public static List<Vector3> vertices = new List<Vector3>();

    public static float dist;

    public List<Vector3> CheckpointPositions;

    //Declare the ListGO GameObject, which has the FindGO script attached to it
    public GameObject ListGO;
    public static FindGO FindGO;

    //How many circles should there be between the edge of the track and the center of the map
    //It should correlate with the segments used by the catmull-rom algorithm
    //In order to result in square-like quads
    public static int circleSegs;

    public GameObject GrassPatchPrefab;
    public GameObject TreePrefab;
    public GameObject[] TombstonesPrefab;

    private void Awake()
    {
        //Get the script as the component of the ListGO GameObject
        FindGO = ListGO.GetComponent<FindGO>();
    }

    private GameObject RandPlantPrefab()
    {
        int i = Random.Range(0, 1000);

        if (i < 5)
            return TreePrefab;
        else
            return GrassPatchPrefab;
    }

    void Start()
    {
        circleSegs = Save.bgSegs;

        catmullPoints.Clear();
        catmullRight.Clear();
        catmullLeft.Clear();

        //We declare a variable which will keep track of the distance between the centre and the previous point
        //This is needed so that there won't be huge jumps between the main points
        int lastR = 6;
        int height = 0;

        for (int i = 0; i < points.Length; i++)
        {

            //We don't want arcs to be on our track so we don't let the program generate the same radius for two neighboring points
            int r = lastR;


            while (r == lastR)
            {
                //The distance of each point from the center
                r = Random.Range(lastR - 1, lastR + 2);

                //Clamp it so that it won't go out of the desired bounds
                r = Mathf.Clamp(r, 5, 8);
            }

            //Update the lastR value to the current value of R for the next iteration
            lastR = r;


            //We scale it up a bit
            r *= 16;

            //360 degrees divided by the number of sides our polygon has + we convert degrees to radians
            float alpha = 360f / points.Length * i * Mathf.Deg2Rad;

            int lastHeight = height / 4;

            height = lastHeight + 3;

            //Don't have two neighbouring points with a very big height difference
            while (Mathf.Abs(height - lastHeight) > 1)
            {
                height = Random.Range(0, 3);
            }


            height *= 4;

            //The 3D coordinates of each point are going to be radius * sine of the alpha degree, height of the point, and radius * cos of the alpha degree
            points[i] = new Vector3(r * Mathf.Sin(alpha), height, r * Mathf.Cos(alpha));
        }
        //We need to change the height of the two adjacent control points to the starting line, so that the players don't start to immediately roll down upon level start
        points[1] = new Vector3(points[1].x, points[0].y, points[1].z);
        points[2] = new Vector3(points[2].x, points[0].y, points[2].z);

        points[points.Length - 1] = new Vector3(points[points.Length - 1].x, points[0].y, points[points.Length - 1].z);
        points[points.Length - 2] = new Vector3(points[points.Length - 2].x, points[0].y, points[points.Length - 2].z);

        //Call the Interpolate.CatmullRom function and use the return value as position to instantiate the new markers at

        Vector3 p0, p1, p2, p3;

        for (int i = 3; i < points.Length + 3; i++)
        {
            p0 = points[(int)Mathf.Repeat(i - 3, points.Length)];
            p1 = points[(int)Mathf.Repeat(i - 2, points.Length)];
            p2 = points[(int)Mathf.Repeat(i - 1, points.Length)];
            p3 = points[(int)Mathf.Repeat(i, points.Length)];

            /////////////////////////////////////////////////////

            Interpolate.CatmullRom(p0, p1, p2, p3, Save.trackSegs);

        }

        //Width of the road
        dist = 6f;

        MeasureDistances();

        //Now draw the markers onto the positions
        DrawMarkers();

        MakeCenter();
        TriangulateCenter();

        MakeOutside();
        TriangulateOutside();

    }

    public void MakeCenter()
    {
        centerPoints.Clear();

        //The center of the whole map
        centerPoints.Add(new Vector3(0f, 20f, 0f));

        //The first circle (going from inside)
        for (int i = 0; i < catmullPoints.Count; i++)
        {
            float radiusScale = 1f / circleSegs * 1f;
            float pointOffset = 20f;

            centerPoints.Add(catmullPoints[i] * radiusScale + Vector3.up * pointOffset);

        }

        for (int j = 2; j < circleSegs; j++)
        {

            //The second circle (going from inside)
            for (int i = 0; i < catmullPoints.Count; i++)
            {
                float radiusScale = 1f / circleSegs * j;
                float pointOffset;

                //The last and first vertex of the circle should have the same height
                if (i != catmullPoints.Count - 1)
                    //The height offset is random, but gradually higher as we get closer to the center
                    pointOffset = Random.Range(0, 3) + (circleSegs - j) * (20f / circleSegs);
                else
                {
                    pointOffset = centerPoints[centerPoints.Count - catmullPoints.Count + 1].y - catmullPoints[i].y * radiusScale;
                }

                centerPoints.Add(catmullPoints[i] * radiusScale + Vector3.up * pointOffset);

            }

        }

        //The outmost circle is just the track's vertices copied
        for (int i = 0; i < catmullPoints.Count; i++)
        {
            centerPoints.Add(catmullRight[i]);

            if (Save.doodads)
            {
                //Put down a patch of grass
                GameObject GrassPatchPrefab = Resources.Load("grassPatchPrefab", typeof(GameObject)) as GameObject;
                Instantiate(GrassPatchPrefab, catmullRight[i] * 5f, Quaternion.identity);
            }

        }

        //Now go through the list and place down markers
        /*for (int i = 0; i < centerPoints.Count; i++)
        {
            float markerSize = 1.1f;
            GameObject marker = GameObject.Instantiate(cubePrefab, centerPoints[i] * 5f, Quaternion.identity);
            marker.transform.localScale *= markerSize * 4f;
        }*/

    }

    public void TriangulateCenter()
    {
        if (Save.doodads)
        {
            //Grass patch prefab declaration
            GrassPatchPrefab = Resources.Load("grassPatchPrefab", typeof(GameObject)) as GameObject;
            TreePrefab = Resources.Load("treePrefab", typeof(GameObject)) as GameObject;

            //Tombstone prefab declaration
            TombstonesPrefab = new GameObject[3];
            TombstonesPrefab[0] = Resources.Load("tombstonePrefab1", typeof(GameObject)) as GameObject;
            TombstonesPrefab[1] = Resources.Load("tombstonePrefab2", typeof(GameObject)) as GameObject;
            TombstonesPrefab[2] = Resources.Load("tombstonePrefab3", typeof(GameObject)) as GameObject;
        }


        //TRIANGLES
        List<int> centerTriangles = new List<int>();

        centerTriangles.Clear();

        //Circle 1
        for (int i = 1; i < catmullPoints.Count; i++)
        {
            centerTriangles.Add(0);
            centerTriangles.Add(i);
            centerTriangles.Add(i + 1);
        }

        centerTriangles.Add(0);
        centerTriangles.Add(catmullPoints.Count);
        centerTriangles.Add(1);

        //Circle 2, 3, 4
        for (int i = 0 * catmullPoints.Count + 1; i < (circleSegs - 1) * catmullPoints.Count; i++)
        {
            if (i % catmullPoints.Count != 0)
            {
                centerTriangles.Add(i);
                centerTriangles.Add(i + catmullPoints.Count);
                centerTriangles.Add(i + catmullPoints.Count + 1);

                centerTriangles.Add(i);
                centerTriangles.Add(i + catmullPoints.Count + 1);
                centerTriangles.Add(i + 1);


                if (Save.doodads)
                {
                    //There is no point in putting patches of grass to the inner area of the map, the players won't see them anyway
                    if (i > (circleSegs * 3 / 10) * catmullPoints.Count)
                    {

                        if (i > (circleSegs / 2) * catmullPoints.Count)
                        {
                            //Put a random tombstone at each vertices
                            int rand = Random.Range(0, 3);
                            Instantiate(TombstonesPrefab[rand], centerPoints[i] * 5f + new Vector3(0f, 3f, 0f), Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                        }

                        //Put a patch of grass between each vertices

                        //Patch at the point
                        Instantiate(RandPlantPrefab(), centerPoints[i] * 5f, Quaternion.identity);

                        //Patches between the point and the vertical neighbour
                        Instantiate(RandPlantPrefab(), (2 * centerPoints[i] + centerPoints[i + catmullPoints.Count]) / 3f * 5f, Quaternion.identity);
                        Instantiate(RandPlantPrefab(), (centerPoints[i] + 2 * centerPoints[i + catmullPoints.Count]) / 3f * 5f, Quaternion.identity);

                        //Patch between the point and the horizontal neighbour
                        Instantiate(RandPlantPrefab(), (centerPoints[i] + centerPoints[i + 1]) / 2f * 5f, Quaternion.identity);

                        //Patches between the point and the diagonal neighbour
                        Instantiate(RandPlantPrefab(), (centerPoints[i] + centerPoints[i + catmullPoints.Count + 1]) / 2f * 5f, Quaternion.identity);
                        Instantiate(RandPlantPrefab(), (3 * centerPoints[i] + centerPoints[i + catmullPoints.Count + 1]) / 4f * 5f, Quaternion.identity);
                        Instantiate(RandPlantPrefab(), (centerPoints[i] + 3 * centerPoints[i + catmullPoints.Count + 1]) / 4f * 5f, Quaternion.identity);


                    }

                }



            }


        }

        //UVS
        List<Vector2> centerUVs = new List<Vector2>();

        for (int i = 0; i < centerPoints.Count; i++)
        {

            //The UV is made with a simple top down projection, using the vertices' real world x-z coordinates
            //We scale the coordinates down to make the textures bigger
            centerUVs.Add(new Vector2(centerPoints[i].x, centerPoints[i].z) / 12f);

        }

        //END
        GameObject CenterMesh = FindGO.CenterMesh;

        CenterMesh.AddComponent<MeshFilter>();
        CenterMesh.AddComponent<MeshRenderer>();
        CenterMesh.AddComponent<MeshCollider>();

        //GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        CenterMesh.GetComponent<MeshRenderer>().material = Resources.Load("MatGrass", typeof(Material)) as Material;

        Mesh CenterMeshMesh = new Mesh();

        CenterMeshMesh.Clear();
        CenterMeshMesh.vertices = centerPoints.ToArray();
        CenterMeshMesh.triangles = centerTriangles.ToArray();
        CenterMeshMesh.uv = centerUVs.ToArray();
        CenterMeshMesh.RecalculateNormals();

        CenterMesh.GetComponent<MeshFilter>().mesh = CenterMeshMesh;

        CenterMesh.GetComponent<MeshCollider>().sharedMesh = CenterMesh.GetComponent<MeshFilter>().mesh;
    }

    public void MakeOutside()
    {
        outsidePoints.Clear();


        //The first circle (going from inside) is just the track's vertices copied
        for (int i = 0; i < catmullPoints.Count; i++)
        {
            outsidePoints.Add(catmullLeft[i]);

            if (Save.doodads)
            {
                //Put down a patch of grass
                GameObject GrassPatchPrefab = Resources.Load("grassPatchPrefab", typeof(GameObject)) as GameObject;
                Instantiate(GrassPatchPrefab, catmullLeft[i] * 5f, Quaternion.identity);
            }


        }

        for (int j = 1; j < circleSegs; j++)
        {

            //The second circle (going from inside)
            for (int i = 0; i < catmullPoints.Count; i++)
            {
                float radiusScale = (1f / circleSegs) * j + 1f;
                radiusScale /= 1f;
                float pointOffset;

                //The last and first vertex of the circle should have the same height
                if (i != catmullPoints.Count - 1)
                {
                    if (j < circleSegs / 2)
                        //The height offset is random, but gradually higher as we get farther away, and then begins to lower again
                        pointOffset = Random.Range(0, 3) + j * (20f / circleSegs);
                    else
                        pointOffset = Random.Range(0, 3) + (circleSegs - j) * (20f / circleSegs);
                }

                else
                {
                    pointOffset = outsidePoints[outsidePoints.Count - catmullPoints.Count + 1].y - catmullPoints[i].y * radiusScale;
                }

                outsidePoints.Add(catmullPoints[i] * radiusScale + Vector3.up * pointOffset);

            }

        }

        //Now go through the list and place down markers
        /*for (int i = 0; i < outsidePoints.Count; i++)
        {
            float markerSize = 1.1f;
            GameObject marker = GameObject.Instantiate(cubePrefab, outsidePoints[i] * 5f, Quaternion.identity);
            marker.transform.localScale *= markerSize * 4f;
        }*/

    }

    public void TriangulateOutside()
    {

        if (Save.doodads)
        {
            //Grass patch prefab declaration
            GrassPatchPrefab = Resources.Load("grassPatchPrefab", typeof(GameObject)) as GameObject;
            TreePrefab = Resources.Load("treePrefab", typeof(GameObject)) as GameObject;

            //Tombstone prefab declaration
            TombstonesPrefab = new GameObject[3];
            TombstonesPrefab[0] = Resources.Load("tombstonePrefab1", typeof(GameObject)) as GameObject;
            TombstonesPrefab[1] = Resources.Load("tombstonePrefab2", typeof(GameObject)) as GameObject;
            TombstonesPrefab[2] = Resources.Load("tombstonePrefab3", typeof(GameObject)) as GameObject;
        }

        //TRIANGLES
        List<int> outsideTriangles = new List<int>();

        outsideTriangles.Clear();

        //Circle 2, 3, 4
        for (int i = 0 * catmullPoints.Count; i < (circleSegs - 1) * catmullPoints.Count - 1; i++)
        {
            if ((i + 1) % catmullPoints.Count != 0)
            {
                outsideTriangles.Add(i);
                outsideTriangles.Add(i + catmullPoints.Count);
                outsideTriangles.Add(i + catmullPoints.Count + 1);

                outsideTriangles.Add(i);
                outsideTriangles.Add(i + catmullPoints.Count + 1);
                outsideTriangles.Add(i + 1);

                if (Save.doodads)
                {
                    //There is no point in putting patches of grass to the outer area of the map, the players won't see them anyway
                    if (i < (circleSegs * 7 / 10) * catmullPoints.Count)
                    {

                        if (i >= catmullPoints.Count)
                        {
                            //Put a random tombstone at each vertices
                            int rand = Random.Range(0, 3);
                            Instantiate(TombstonesPrefab[rand], outsidePoints[i] * 5f + new Vector3(0f, 3f, 0f), Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                        }


                        //Put a patch of grass between each vertices

                        //Patch at the point
                        Instantiate(RandPlantPrefab(), outsidePoints[i] * 5f, Quaternion.identity);

                        //Patches between the point and the vertical neighbour
                        Instantiate(RandPlantPrefab(), (2 * outsidePoints[i] + outsidePoints[i + catmullPoints.Count]) / 3f * 5f, Quaternion.identity);
                        Instantiate(RandPlantPrefab(), (outsidePoints[i] + 2 * outsidePoints[i + catmullPoints.Count]) / 3f * 5f, Quaternion.identity);

                        //Patch between the point and the horizontal neighbour
                        Instantiate(RandPlantPrefab(), (outsidePoints[i] + outsidePoints[i + 1]) / 2f * 5f, Quaternion.identity);

                        //Patches between the point and the diagonal neighbour
                        Instantiate(RandPlantPrefab(), (outsidePoints[i] + outsidePoints[i + catmullPoints.Count + 1]) / 2f * 5f, Quaternion.identity);
                        Instantiate(RandPlantPrefab(), (3 * outsidePoints[i] + outsidePoints[i + catmullPoints.Count + 1]) / 4f * 5f, Quaternion.identity);
                        Instantiate(RandPlantPrefab(), (outsidePoints[i] + 3 * outsidePoints[i + catmullPoints.Count + 1]) / 4f * 5f, Quaternion.identity);


                    }

                }




            }


        }

        //UVS
        List<Vector2> outsideUVs = new List<Vector2>();

        for (int i = 0; i < outsidePoints.Count; i++)
        {

            //The UV is made with a simple top down projection, using the vertices' real world x-z coordinates
            //We scale the coordinates down to make the textures bigger
            outsideUVs.Add(new Vector2(outsidePoints[i].x, outsidePoints[i].z) / 12f);

        }

        //END
        GameObject OutsideMesh = FindGO.OutsideMesh;

        OutsideMesh.AddComponent<MeshFilter>();
        OutsideMesh.AddComponent<MeshRenderer>();
        OutsideMesh.AddComponent<MeshCollider>();

        //GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        OutsideMesh.GetComponent<MeshRenderer>().material = Resources.Load("MatGrass", typeof(Material)) as Material;

        Mesh OutsiderMeshMesh = new Mesh();

        OutsiderMeshMesh.Clear();
        OutsiderMeshMesh.vertices = outsidePoints.ToArray();
        OutsiderMeshMesh.triangles = outsideTriangles.ToArray();
        OutsiderMeshMesh.uv = outsideUVs.ToArray();
        OutsiderMeshMesh.RecalculateNormals();

        OutsideMesh.GetComponent<MeshFilter>().mesh = OutsiderMeshMesh;

        OutsideMesh.GetComponent<MeshCollider>().sharedMesh = OutsideMesh.GetComponent<MeshFilter>().mesh;
    }

    public void AddPointToList(Vector3 point)
    {
        //We have to filter out duplicate points possibly generated by the CatmullRom algorithm

        if (catmullPoints.Count == 0)
            catmullPoints.Add(point);
        else if (Vector3.Distance(point, catmullPoints[catmullPoints.Count - 1]) > 0.1f)
        {
            catmullPoints.Add(point);
        }

    }

    public void MeasureDistances()
    {
        //Weld the first and last vertices
        Vector3 avgMid = (catmullPoints[0] + catmullPoints[catmullPoints.Count - 1]) / 2f;
        catmullPoints[0] = avgMid;
        catmullPoints[catmullPoints.Count - 1] = avgMid;

        //We keep track of distances between each neighbouring points, this will be needed at UV mapping
        float offset = 0f;

        for (int i = 0; i < catmullPoints.Count - 1; i++)
        {
            //Distance between this point and the next
            float segDist = Vector3.Distance(catmullPoints[i], catmullPoints[(int)Mathf.Repeat(i + 1, catmullPoints.Count)]);

            //Relative to the width of the road
            float relativeDist = segDist / (2 * dist);

            distances.Add(relativeDist);

            offsets.Add(offset);

            offset += relativeDist;

        }

        //And finally, the distance between the first and the last point
        float segDistFinal = Vector3.Distance(catmullPoints[catmullPoints.Count - 1], catmullPoints[0]);
        float relativeDistFinal = segDistFinal / (2 * dist);

        distances.Add(relativeDistFinal);

        offsets.Add(offset);

    }

    public void DrawMarkers()
    {
        //Without this line, every time a new track is loaded, the vertices of the previous track still remain in the list
        vertices.Clear();

        //Now we add thickness to the path
        //We have to determine the position of the vertices on both sides of the path
        //For this, we have to calculate the vector between the neighbours of the current point, 
        //take the vector perpendicular to that, and put down the two new vertices in that direction, to the chosen distance from the initial point

        for (int i = 0; i < catmullPoints.Count; i++)
        {
            //The next point of the curve
            Vector3 nextPoint = catmullPoints[(int)Mathf.Repeat(i + 1, catmullPoints.Count)];

            //The previous point of the curve
            Vector3 prevPoint = catmullPoints[(int)Mathf.Repeat(i - 1, catmullPoints.Count)];

            //The direction pointing to the previous point from the next point
            Vector3 dir = prevPoint - nextPoint;

            //The perpendicular vector
            dir = Quaternion.Euler(0, 90, 0) * dir;

            //We normalize the vector, then multiply it by the chosen distance
            dir = dir.normalized * dist;

            Vector3 leftVertexPos = catmullPoints[i] + dir;
            Vector3 rightVertexPos = catmullPoints[i] - dir;

            //We add each side of the track as a vertex to a list
            vertices.Add(rightVertexPos);
            vertices.Add(leftVertexPos);

            //Keep track of the inner vertices for the inner mesh generation
            catmullRight.Add(rightVertexPos);

            //Keep track of the outer vertices for the outer mesh generation
            catmullLeft.Add(leftVertexPos);

            //Drawing out the markers

            //We have to multiply by 5f because the Track gameobject is scaled by 5 in the scene

            /*float markerSize = 1.1f;

            GameObject rightMarker = GameObject.Instantiate(cubePrefab, rightVertexPos * 5f, Quaternion.identity);
            rightMarker.transform.localScale *= markerSize;

            GameObject leftMarker = GameObject.Instantiate(cubePrefab, leftVertexPos * 5f, Quaternion.identity);
            leftMarker.transform.localScale *= markerSize;*/



        }

        //We need to weld the first and last two vertices
        Vector3 avgRight = (vertices[0] + vertices[vertices.Count - 2]) / 2f;
        Vector3 avgLeft = (vertices[1] + vertices[vertices.Count - 1]) / 2f;

        vertices[0] = avgRight;
        vertices[vertices.Count - 2] = avgRight;

        catmullRight[0] = avgRight;
        catmullRight[catmullRight.Count - 1] = avgRight;

        vertices[1] = avgLeft;
        vertices[vertices.Count - 1] = avgLeft;

        catmullLeft[0] = avgLeft;
        catmullLeft[catmullLeft.Count - 1] = avgLeft;

        //We make triangles from the vertices
        TrackRenderer.TriangulateRoad(vertices);

        SetUpCheckpoints(points);

        PlaceDownPowerUps();

    }

    private void SetUpCheckpoints(Vector3[] CPpos)
    {
        GameObject CPprefab = Resources.Load("CPprefab", typeof(GameObject)) as GameObject;

        //There are 18 checkpoints, the first being the finish line

        Vector3 FLrot = CPpos[1] - CPpos[CPpos.Length - 1];

        //We have to multiply by 5f because the Track gameobject is scaled by 5 in the scene

        GameObject FinishLine = Instantiate(CPprefab, CPpos[0] * 5f, Quaternion.LookRotation(FLrot));
        FinishLine.name = "FinishLine";

        //The two lines below load the gate prefab as a visual representation for the finish line
        GameObject GatePrefab = Resources.Load("gatePrefab", typeof(GameObject)) as GameObject;
        Instantiate(GatePrefab, FinishLine.transform.position, FinishLine.transform.rotation * Quaternion.Euler(0f, -90f, 0f));

        FinishLine.GetComponent<CheckpointScript>().number = 0;

        GameObject FinishBlip = FindGO.FinishBlip;

        //FinishBlip.transform.SetParent(FinishLine.transform);

        //We have to multiply by 5f because the Track gameobject is scaled by 5 in the scene
        FinishBlip.transform.position = CPpos[0] * 5f;
        FinishBlip.transform.rotation = Quaternion.Euler(90f, FinishLine.transform.rotation.eulerAngles.y, 0f);
        FinishBlip.transform.localScale = new Vector3(60f, 60f, 1f);

        CheckpointPositions.Clear();
        CheckpointPositions.Add(FinishBlip.transform.position);


        for (int i = 1; i < CPpos.Length - 2; i += 2)
        {
            Vector3 rotation = CPpos[i + 1] - CPpos[i - 1];

            //We have to multiply by 5f because the Track gameobject is scaled by 5 in the scene
            GameObject CPGO = Instantiate(CPprefab, CPpos[i] * 5f, Quaternion.LookRotation(rotation));

            CPGO.GetComponent<CheckpointScript>().number = i - i / 2;

            CheckpointPositions.Add(CPpos[i] * 5f);

        }

        Vector3 LastRot = CPpos[0] - CPpos[CPpos.Length - 2];

        //We have to multiply by 5f because the Track gameobject is scaled by 5 in the scene
        GameObject LastCP = Instantiate(CPprefab, CPpos[CPpos.Length - 1] * 5f, Quaternion.LookRotation(LastRot));

        LastCP.GetComponent<CheckpointScript>().number = CPpos.Length - 1 - (CPpos.Length - 1) / 2;

        CheckpointPositions.Add(CPpos[CPpos.Length - 1] * 5f);

        //Fetch the two gameobjects
        GameObject P1 = FindGO.P1Avatar;
        GameObject P2 = FindGO.P2Avatar;

        //Start position is between the finish line and last checkpoint, but closer to the finish line
        Vector3 startPos = (points[points.Length - 1] + 2 * points[0]) / 3f;

        //Put the two player karts behind the start line
        P1.transform.position = startPos * 5f;
        P2.transform.position = startPos * 5f;

        //Set the initial orientation of the karts, determined by the rotation of the finish line
        P1.transform.rotation = Quaternion.LookRotation((FinishLine.transform.position - P1.transform.position).normalized);
        P2.transform.rotation = Quaternion.LookRotation((FinishLine.transform.position - P2.transform.position).normalized);

        //Set them apart a bit
        P1.transform.position += P1.transform.right * -(5f * dist / 3f) + Vector3.up * 1f;
        P2.transform.position += P1.transform.right * (5f * dist / 3f) + Vector3.up * 1f;

    }

    private void PlaceDownPowerUps()
    {

        //Placing down the PowerUps on each element of the CheckpointPositions list

        //Declare the prefab
        GameObject PUprefab = Resources.Load("PUprefab", typeof(GameObject)) as GameObject;

        //Instantiate a PowerUp from the prefab
        for (int i = 0; i < CheckpointPositions.Count; i++)
        {
            //Determining the orientation of the checkpoint that the pickup should be on
            Vector3 rotation = CheckpointPositions[(int)Mathf.Repeat(i + 1, CheckpointPositions.Count)] - CheckpointPositions[(int)Mathf.Repeat(i - 1, CheckpointPositions.Count)];

            GameObject PowerUpLeft = Instantiate(PUprefab, CheckpointPositions[i] + Vector3.up * 4f, Quaternion.LookRotation(rotation));

            //We need two powerups at each checkpoint, so we need them to be set apart, with a left and right offset
            //The offset is determined according to the width of the road
            Vector3 leftOffset = PowerUpLeft.transform.right * (-5f * dist / 3f);

            PowerUpLeft.transform.position += leftOffset;

            int chosenType = Random.Range(1, 4);

            PowerUpLeft.GetComponent<PowerUpScript>().SetType(chosenType);

            //Now the right one
            GameObject PowerUpRight = Instantiate(PUprefab, CheckpointPositions[i] + Vector3.up * 4f, Quaternion.LookRotation(rotation));

            Vector3 rightOffset = PowerUpRight.transform.right * (5f * dist / 3f);

            PowerUpRight.transform.position += rightOffset;

            chosenType = Random.Range(1, 4);

            PowerUpRight.GetComponent<PowerUpScript>().SetType(chosenType);

        }

    }

}
