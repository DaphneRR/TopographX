using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCreator : MonoBehaviour
{
    public TextAsset data;

    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;

    private CultureInfo ci;

    private void Awake () {

        // on load le fichier .txt [RENDRE LE FICHIER CHARGé DYNAMIQUE ET CHARGER NV DIRECTEMENT]
        //data = Resources.Load<TextAsset>("BrainMesh_ICBM152");
        
        ExtractData();
		Generate();
	}

    // extraire les données de data dans vertices[] et triangles[]
    private void ExtractData () {
        List<string> tempCoordinates;
        List<string> dataArray;

        // on sépare les données par ligne dans une liste
        dataArray = data.text.Split('\n').ToList();

        // on instancie le nb de vertices dont on a besoin
		vertices = new Vector3[int.Parse(dataArray[0])];
        // on récupère les coordonnées
        for(int i = 0; i<vertices.Length; ++i) {
            tempCoordinates = dataArray[i+1].Split(' ').ToList();
            vertices[i] = new Vector3(float.Parse(tempCoordinates[0],CultureInfo.InvariantCulture),float.Parse(tempCoordinates[1],CultureInfo.InvariantCulture),float.Parse(tempCoordinates[2],CultureInfo.InvariantCulture));
        }
/*
vertices = new Vector3[3];
vertices[0].x = 12.5233f; vertices[0].y = -58.59f; vertices[0].z = 20.3353f;
vertices[1].x = 15.5437f; vertices[0].y = -57.5528f; vertices[0].z = 19.9662f;
vertices[2].x = 13.3717f; vertices[0].y = -57.5303f; vertices[0].z = 19.7171f; */


        // on instancie le nb de triangles dont on a besoin
        triangles = new int[int.Parse(dataArray[vertices.Length+1])*3];
//       triangles = new int[vertices.Length*3];

        for(int i=0; i<triangles.Length/3; ++i){
        //for(int i=119816;i<triangles.Length/3;++i){
        //for(int i=119816; i<triangles.Length/3; ++i){  
        //for(int i=119817; i<119818; ++i){
                                                                                        //Debug.Log(i+vertices.Length+2);
            tempCoordinates = dataArray[i+vertices.Length+2].Split(' ').ToList();
                                                                                        //Debug.Log(tempCoordinates[0]);
                                                                                        //Debug.Log(int.Parse(tempCoordinates[0])+" "+tempCoordinates[1]+" "+tempCoordinates[2]);
            for(int j=0; j<3; ++j) {
                                                                                        //Debug.Log(tempCoordinates[j]);
               int.TryParse(tempCoordinates[j], out triangles[i*3+j]);
               triangles[i*3+j] -= 1;
                                                                                        //Debug.Log(triangles[i*3+j]);
            }
        }

/*triangles = new int[3];
triangles[0] = 0;
triangles[1] = 1;
triangles[2] = 2; */
    }

	private void Generate () {
        
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

        mesh.vertices = vertices;

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
	}
/*
    // Visualiser les vertices 
    private void OnDrawGizmos () {
        if (vertices == null) {
			return;
		}
		Gizmos.color = Color.black;
      for (int i = 0; i < vertices.Length; i++) {
	    	Gizmos.DrawSphere(vertices[i], 0.1f);
	    }
//        Gizmos.DrawSphere(vertices[40963], 0.1f);
      //Gizmos.DrawSphere(vertices[71282], 0.2f);
        //Gizmos.DrawSphere(vertices[81922], 0.2f);
        //Gizmos.DrawSphere(vertices[79457], 0.2f);
	}
*/
}