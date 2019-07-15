using UnityEngine;

public class Electrode : MonoBehaviour
{
    new public string name;
    public string patient;
    public float[] values;

    public UiController UI;
    //public float[] M1values;
    //public float[] M2values;
    //public float[] M3values;
    //public float[] M4values;
    
    public Vector3 RGB;

    private int ID;
    private float r, g, b;
    
    public void SetData(int inI, string inN, string inP) {
        ID = inI;
        name = inN;
        patient = inP;

        // on link le UIController
        UI = GameObject.FindWithTag("UIController").GetComponent<UiController>();
    }

    public void CreateValues(int size) {
        values = new float[size];
        //M1values = new float[size];
        //M2values = new float[size];
        //M3values = new float[size];
        //M4values = new float[size];
    }

    public void UpdateColor(int index, float threshold, float min, float max) {
        
        // si on est en dehors des bornes données et en mode (outsideDefault)
        if(UI.outsideDefault && (values[index] > max || values[index] < min)) {
            // on applique la couleur par défaut
            if(!UI.colorsVisible[0]) GetComponent<MeshRenderer>().enabled = false;
            else { 
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<Renderer>().material.color = UI.thresholdColors[0]; 
                }

        }
        // si mode colormap
        else if(UI.colormapMode && UI.maps != null && UI.maps.Count > 0) { 
                GetComponent<MeshRenderer>().enabled = true;
                // on calcule où on est dans la colormap
                int step = Mathf.RoundToInt ( (values[index] - min) / ( (max-min) / UI.maps[UI.currentColormap].size ) );
                if(UI.maps[UI.currentColormap].inverted) step = UI.maps[UI.currentColormap].size - step;
                // pour être sûrs
                step = Mathf.Clamp(step,0,UI.maps[UI.currentColormap].size-1);
//Debug.Log(step);
                // on crée la couleur correspondante
                Color thisOne = new Color(UI.maps[UI.currentColormap].values[step].x/255f, 
                                            UI.maps[UI.currentColormap].values[step].y/255f, 
                                            UI.maps[UI.currentColormap].values[step].z/255f);
                GetComponent<Renderer>().material.color = thisOne;
        }
        // si mode threshold
        else {
            if(values[index] >= threshold) {
                if(!UI.colorsVisible[2]) GetComponent<MeshRenderer>().enabled = false;
                else {
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<Renderer>().material.color = UI.thresholdColors[2];
                }
            }
            else if(values[index] < threshold) {
                if(!UI.colorsVisible[1]) GetComponent<MeshRenderer>().enabled = false;
                else {
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<Renderer>().material.color = UI.thresholdColors[1];
                }
            }
            else {
                if(!UI.colorsVisible[0]) GetComponent<MeshRenderer>().enabled = false;
                else { 
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<Renderer>().material.color = UI.thresholdColors[0]; }
            }
        }

        /*float where = (M2values[index] - min)/(max - min);
        if(M1values[index] > threshold && M2values[index] > 0) {
            GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, Color.red, where);
        } else if(M1values[index] > threshold && M2values[index] < 0) {
            GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, Color.blue, where);
        } else if(M1values[index] < threshold && M2values[index] > 0) {
            GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, Color.green, where);
        } else if(M1values[index] < threshold && M2values[index] < 0) {
            GetComponent<Renderer>().material.color = new Color(0.9f,0.9f,0f,1f);
        } else GetComponent<Renderer>().material.color = new Color(0.2f,0.2f,0.2f);
        */

        //float where;

        //if(-1 < values[index] && values[index] < 1) {
        //    GetComponent<Renderer>().material.color = Color.grey;
        //} else 
        /* 
        if(values[index] < (max + min)/2) {
            float where = (values[index] - min)/((max + min)/2 - min);

            //float where = (Mathf.Clamp(values[index], min, max) - min)/(max - min);

            GetComponent<Renderer>().material.color = Color.LerpUnclamped(GetComponentInParent<ElectrodesData>().cMin, GetComponentInParent<ElectrodesData>().cMid, where);
        } else if (values[index] > (max + min)/2) {
            float where = (values[index] - (max + min)/2)/(max - (max + min)/2);

            GetComponent<Renderer>().material.color = Color.LerpUnclamped(GetComponentInParent<ElectrodesData>().cMid, GetComponentInParent<ElectrodesData>().cMax, where);
        }
        */

        /*if(values[index] < (max + min)/3) {
            where = (values[index] - min)/((max + min)/3 - min);

            GetComponent<Renderer>().material.color = Color.LerpUnclamped(GetComponentInParent<ElectrodesData>().cMin, GetComponentInParent<ElectrodesData>().cMid1, where);
        } else if (values[index] > (max + min)/3 && values[index] < (max + min)*2/3) {
            where = (values[index] - (max + min)/3)/((max + min)/3);

            GetComponent<Renderer>().material.color = Color.LerpUnclamped(GetComponentInParent<ElectrodesData>().cMid1, GetComponentInParent<ElectrodesData>().cMid2, where);
        } else if (values[index] > (max + min)*2/3) {
            where = (values[index] - (max + min)*2/3)/((max + min)/3);

            GetComponent<Renderer>().material.color = Color.LerpUnclamped(GetComponentInParent<ElectrodesData>().cMid2, GetComponentInParent<ElectrodesData>().cMax, where);
        }*/
        
        /*float xax = M3values[index]-M1values[index];
        float yax = M3values[index]-M2values[index];
        float zax = M2values[index]-M1values[index];
        //Debug.Log(xax + " " + yax + " " + zax);
        
        float gray = 0.2f;
        float noiseLevel = 0;
        Color c = Color.gray;

        if(xax>0 && yax>0){
            r = Mathf.Sqrt(Mathf.Pow(xax,2) + Mathf.Pow(yax,2));
            RGB.x = (r - min)/(max - min);

            if(r < noiseLevel) //RGB.x = RGB.y = RGB.z = gray;
                GetComponent<Renderer>().material.color = Color.gray;
            else { RGB.y = RGB.z = 0; c = Color.red; 
            GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, c, RGB.x); }

        } else if(xax<0 && zax<0){
            g = Mathf.Sqrt(Mathf.Pow(xax,2) + Mathf.Pow(zax,2));
            RGB.y = (g - min)/(max - min);

            if(g < noiseLevel) //RGB.x = RGB.y = RGB.z = gray;
                GetComponent<Renderer>().material.color = Color.gray;
            else { RGB.x = RGB.z = 0; c = Color.green; 
            GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, c, RGB.y); }

        } else if(zax>0 && yax<0){        
            b = Mathf.Sqrt(Mathf.Pow(zax,2) + Mathf.Pow(yax,2));
            RGB.z = (b - min)/(max - min);

            if(b < noiseLevel) //RGB.x = RGB.y = RGB.z = gray;
                GetComponent<Renderer>().material.color = Color.gray;
            else { RGB.y = RGB.x = 0; c = Color.blue; 
            GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, c, RGB.z); }

        //} else RGB.x = RGB.y = RGB.z = gray;    
        } else GetComponent<Renderer>().material.color = Color.gray;

        //GetComponent<Renderer>().material.color = new Color(RGB.x,RGB.y,RGB.z,1);
        //GetComponent<Renderer>().material.color = Color.LerpUnclamped(Color.yellow, c, where);
        */
    }
}
