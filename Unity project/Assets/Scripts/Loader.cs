using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// Include these namespaces to use BinaryFormatter
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Loader : MonoBehaviour
{
    public string path = "";
    public bool online;
    public string url;
    [Multiline] public string dataString = "";

    void Start()
    {
        if(online) {
            //OnlineLoad();
            StartCoroutine(GetText());
        } else {
            if(path != null && path != "" && path != " ") {
                BinaryFormatter bFormatter = new BinaryFormatter();
                // Open the file using the path
                FileStream file = File.OpenRead(path);
                // Convert the file from a byte array into a string
                string fileData = bFormatter.Deserialize(file) as string;
                // We're done working with the file so we can close it
                file.Close();
                // send it to the save parser
                GetComponent<SavesManager>().UnpackData(fileData);
            } else if (dataString != null && dataString != "" && dataString != " ") {
                //dataString = GetComponentInChildren<Text>().text;
                GetComponent<SavesManager>().UnpackData(dataString);
            }
        }
    }

    IEnumerator GetText() {
        
        //UnityWebRequest www = UnityWebRequest.Get("https://daphnerr.github.io/test/data.save"); 
        UnityWebRequest www = UnityWebRequest.Get("/test/data.save"); 
        
        yield return www.SendWebRequest();
        
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);
 
            //GetComponent<SavesManager>().UnpackData(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            string hoi = System.Text.Encoding.UTF8.GetString(results, 0, results.Length);
            GetComponent<SavesManager>().UnpackData(hoi);
            Debug.Log(hoi);
        }
    }

    public void CopyToClipboard() {
        GUIUtility.systemCopyBuffer = GetComponent<SavesManager>().CompileSaveData();
    }
}
