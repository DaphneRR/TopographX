using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Include these namespaces to use BinaryFormatter
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SavesManager : MonoBehaviour
{
    private UiController UI;
    private ElectrodesData electrodesData;
    private GameObject mCamera;

    public GameObject rightBrain;
    public GameObject leftBrain;
    public InputField electrodesScale;
    public Toggle[] togglesBrain;
    public Toggle toggleColorMap;
    public Transform colormapsPanel;
    public InputField glossinessField;
    public Toggle[] toggleVisible;

    public Transform datasetsPanel;

    CultureInfo c;

    //private string dataToSave;

    // Start is called before the first frame update
    void Awake()
    {
        UI = GameObject.FindWithTag("UIController").GetComponent<UiController>();
        electrodesData = GameObject.FindWithTag("Player").GetComponent<ElectrodesData>();
        mCamera = GameObject.FindWithTag("MainCamera");

        c = CultureInfo.InvariantCulture;
    }

    // creates a string with every data necessary for the save
    public string CompileSaveData() {
        string dataToSave = "\n";

        // Get camera position and add it to the string
        // camera position (x,y,z) EDIT : only x needed for distance
        dataToSave += mCamera.transform.localPosition.x.ToString(c);
        // add a separator between data blocks (done here for clarity)
        dataToSave += ';';
        // camera rotation
        /*dataToSave += mCamera.transform.parent.rotation.x.ToString() + ';'
                    + mCamera.transform.parent.rotation.y.ToString() + ';'
                    + mCamera.transform.parent.rotation.z.ToString() + ';'
                    + mCamera.transform.parent.rotation.w; */
        // actually needs to be done in euler angles
        dataToSave += mCamera.transform.parent.eulerAngles.y.ToString(c) + ';';
        dataToSave += mCamera.transform.parent.eulerAngles.z.ToString(c);
        
        // add an end line separator (done here for clarity)
        dataToSave += '\n';
        
        // Get Display data
        // right brain hemisphere visibility
        if(rightBrain.activeSelf) dataToSave += "1"; else dataToSave += "0";
        dataToSave += ';';
        // left brain hemisphere visibility
        if(leftBrain.activeSelf) dataToSave += "1"; else dataToSave += "0";
        dataToSave += ';';
        // mesh open or closed
        if(rightBrain.transform.position.x != 0 || leftBrain.transform.position.x != 0) dataToSave += "1"; else dataToSave += "0";
        dataToSave += ';';
        // electrodes scale
        dataToSave += electrodesScale.text;

        // add an end line separator (done here for clarity)
        dataToSave += '\n';

        // Get Options data
        // background color
        dataToSave += GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor.r.ToString(c) + ';'
                    + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor.g.ToString(c) + ';'
                    + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor.b.ToString(c) + ';'
                    + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor.a.ToString(c);
        // add a separator between data blocks (done here for clarity)
        dataToSave += ';';
        // mesh color
        dataToSave += leftBrain.GetComponentInChildren<Renderer>().material.color.r.ToString(c) + ';'
                    + leftBrain.GetComponentInChildren<Renderer>().material.color.g.ToString(c) + ';'
                    + leftBrain.GetComponentInChildren<Renderer>().material.color.b.ToString(c) + ';'
                    + leftBrain.GetComponentInChildren<Renderer>().material.color.a.ToString(c);
        // add a separator between data blocks (done here for clarity)
        dataToSave += ';';
        // mesh glossiness
        dataToSave += leftBrain.GetComponentInChildren<Renderer>().material.GetFloat("_GlossMapScale").ToString(c) + ';';
        // axis display
        dataToSave += UI._axesToggle.isOn.ToString(c);
        
        // add an end line separator (done here for clarity)
        dataToSave += '\n';

        // Get Colors data
        // color mode
        if(UI.colormapMode) dataToSave += "1"; else dataToSave += "0";
        dataToSave += ';';
        // colors visibility
        foreach(bool c in UI.colorsVisible) {
            if(c) dataToSave += "1"; else dataToSave += "0";
            dataToSave += ';';
        }
        // default, lower and greater colors
        foreach(Color col in UI.thresholdColors) { 
            dataToSave += col.r.ToString(c) + ';' + col.g.ToString(c) + ';' + col.b.ToString(c) + ';' + col.a.ToString(c) + ';';
        }

        // add an end line separator (done here for clarity)
        dataToSave += '\n';

        // Get colormaps (one per line)
        // insert line to know how many colormaps there is
        dataToSave += UI.maps.Count().ToString(c) + '\n';
        // loaded colormaps
        foreach (Colormap cm in UI.maps)
        {
            // get name and size
            dataToSave += cm.name + ';' + cm.size.ToString(c) + ';' + cm.inverted.ToString(c) + ';';
            foreach(Vector3 v in cm.values) {
                dataToSave += v.x.ToString(c) + ';' + v.y.ToString(c) + ';' + v.z.ToString(c) + ';';
            }
            // next line 
            dataToSave += '\n';
        }
        // selected colormap
        dataToSave += UI.currentColormap.ToString(c);

        // add an end line separator (done here for clarity)
        dataToSave += '\n';

        // Get dataset data
        // min, max and threshold values
        dataToSave += electrodesData.minValue.ToString(c) + ';' + electrodesData.maxValue.ToString(c) +';' + electrodesData.threshold.ToString(c) + ';';
        // formula
        dataToSave += electrodesData.formulaField.text;

        // add an end line separator (done here for clarity)
        dataToSave += '\n';

        // Get datasets
        // if proSet exists, we add it to the save file
        if(electrodesData.proSet != null) dataToSave += SaveDataset(electrodesData.proSet);
        // then add the number of sets loaded
        dataToSave += electrodesData.datasetsCount.ToString(c) + '\n';
        foreach(Dataset d in electrodesData.sets) {
            dataToSave += SaveDataset(d);
        }

//dataToSave += "For tests purposes";
//Debug.Log(dataToSave);
        // return the final string
        return dataToSave;
    }

    string SaveDataset(Dataset d) {
        string toAdd = "";
        // set name
        toAdd += d.name + ';';
        // set time unit
        toAdd += d.timeUnit + ';';
        // set number of electrodes
        toAdd += d.electrodesNb.ToString(c) + ';';
        // set timestamps
        foreach(int t in d.timestamps) toAdd += t.ToString(c) + ';';

        // add an end line separator (done here for clarity)
        toAdd += '\n';    

        // electrodes (one per line)
        foreach(ElectrodeLocal e in d.data) {
            toAdd += e.ID.ToString(c) + ';';
            toAdd += e.name + ';';
            toAdd += e.patient + ';';
            toAdd += e.coords.x.ToString(c) + ';' + e.coords.y.ToString(c) + ';' + e.coords.z.ToString(c) + ';';

            foreach(float v in e.values) {
                toAdd += v.ToString(c) + ';';
            }
            // add an end line separator (done here for clarity)
            toAdd += '\n';
        }

        return toAdd;
    }

    // unpacks all the data contained in "save"
    // NOTE THAT NOTHING IS SECURED, as the function considers the save data to be perfect (this needs to be changed later)
    public void UnpackData(string save) {
        List<string> tempData;
        List<string> dataArray;
int index = 1; // index to keep the line we're at

        // FIRST need to ensure the current data is cleared
            // Better way to do it in the future : autosave current data, try to load save. If failed, reload autosave instead

        // getting ready to parse the string
         // on sépare les données par ligne dans une liste
        dataArray = save.Split('\n').ToList();
        // on enlève les espaces en début et fin de chaque ligne (pour ne pas avoir de ligne vide)
        foreach(string s in dataArray) {
            s.Trim();
        }


        // now we parse the camera position and rotation (first line)
        float x, y, z;
        tempData = dataArray[index].Split(';').ToList(); index++;
        // getting the position
            //  float.TryParse(tempData [0], out x); float.TryParse(tempData [1], out y); float.TryParse(tempData [2], out z);
            //  mCamera.transform.position = new Vector3(x,y,z);
        // getting the rotation
            //  float.TryParse(tempData [3], out x); float.TryParse(tempData [4], out y); float.TryParse(tempData [5], out z); float.TryParse(tempData [6], out w);
            //  mCamera.transform.parent.rotation = new Quaternion(x,y,z,w);
        float.TryParse(tempData[0], NumberStyles.Any, c, out x); 
        float.TryParse(tempData[1], NumberStyles.Any, c, out y); 
        float.TryParse(tempData[2], NumberStyles.Any, c, out z);
        mCamera.transform.parent.GetComponent<CameraOrbit>()._CameraDistance = -x;
        mCamera.transform.parent.GetComponent<CameraOrbit>()._LocalRotation.y = y;
        mCamera.transform.parent.GetComponent<CameraOrbit>()._LocalRotation.z = z;

        // now parsing the mesh display values (second line)
        tempData = dataArray[index].Split(';').ToList(); index++;
        // right visible
        if(int.Parse(tempData[0]) == 1) togglesBrain[0].isOn = true;
        else togglesBrain[0].isOn = false;
        // left visible
        if(int.Parse(tempData[1]) == 1) togglesBrain[1].isOn = true;
        else togglesBrain[1].isOn = false;
        // open or not
        if(int.Parse(tempData[2]) == 1) UI.OpenBrain();
        else UI.CloseBrain();
        // electrodes scale (storing it, to update their scale later)
        string tempScale = tempData[3];

        // now parsing the options data (third line)
        tempData = dataArray[index].Split(';').ToList(); index++;
        // background color
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = 
            new Color(float.Parse(tempData[0],c),
            float.Parse(tempData[1],c),
            float.Parse(tempData[2],c),
            float.Parse(tempData[3],c));
        // mesh color
        leftBrain.GetComponentInChildren<Renderer>().material.color = rightBrain.GetComponentInChildren<Renderer>().material.color = 
            new Color(float.Parse(tempData[4],c),
            float.Parse(tempData[5],c),
            float.Parse(tempData[6],c),
            float.Parse(tempData[7],c));
        // glossiness
        leftBrain.GetComponentInChildren<Renderer>().material.SetFloat("_GlossMapScale",float.Parse(tempData[8],c));
        rightBrain.GetComponentInChildren<Renderer>().material.SetFloat("_GlossMapScale",float.Parse(tempData[8],c));
        // axis display
        if(tempData.Count > 9) UI._axesToggle.isOn = bool.Parse(tempData[9]);

        // now parsing the colors data (fourth line)
        tempData = dataArray[index].Split(';').ToList(); index++;
        // color mode
        // if the colormap toggle exists
        if(toggleColorMap != null) {
            if(int.Parse(tempData[0]) == 1) toggleColorMap.isOn = true;
            else toggleColorMap.isOn = false;
        }
        // else, we bypass it
        else {
            if(int.Parse(tempData[0]) == 1) UI.colormapMode = true;
            else UI.colormapMode = false;
        }
        // colors visible
        if(tempData[1] == "1") UI.UpdateDefaultVisible(true); else UI.UpdateDefaultVisible(false);
        if(tempData[2] == "1") UI.UpdateUnderVisible(true); else UI.UpdateUnderVisible(false);
        if(tempData[3] == "1") UI.UpdateGreaterVisible(true); else UI.UpdateGreaterVisible(false);
        // update in UI
        if(toggleVisible.Count() > 0) {
            if(tempData[1] == "1") toggleVisible[0].SetIsOnWithoutNotify(true); else toggleVisible[0].SetIsOnWithoutNotify(false);
            if(tempData[2] == "1") toggleVisible[0].SetIsOnWithoutNotify(true); else toggleVisible[1].SetIsOnWithoutNotify(false);
            if(tempData[3] == "1") toggleVisible[0].SetIsOnWithoutNotify(true); else toggleVisible[2].SetIsOnWithoutNotify(false);
        }
        // default, under and greater
        for(int i=0;i<UI.thresholdColors.Count();++i) {
            UI.thresholdColors[i] = new Color(float.Parse(tempData[4+(4*i)],c),
                    float.Parse(tempData[5+(4*i)],c),
                    float.Parse(tempData[6+(4*i)],c),
                    float.Parse(tempData[7+(4*i)],c));
        }

        // now parsing the colormaps
        // FIRST, clear all current colormaps (if there is a UI panel to clear)
        if(colormapsPanel != null) ClearColormaps();
        // starting with the number of maps to import (fifth line)
        int mapCount = int.Parse(dataArray[index],c); index++;
        // for each map, we extract the data and call AddColormap
        for(int i=0;i<mapCount;++i) {
            tempData = dataArray[index].Split(';').ToList(); index++;
            AddColormap(tempData);
        }
        // selected colormap (line mapscount+6)
        UI.currentColormap = int.Parse(dataArray[index],c); index++;
        float indexL = 0;
        if(colormapsPanel != null) foreach(Toggle m in colormapsPanel.GetComponentsInChildren<Toggle>()) { 
            if(indexL == (float)UI.currentColormap) { 
                m.isOn = true;
                m.interactable = false;
                m.transform.parent.GetComponentInChildren<Button>().interactable = false;
            }
            indexL += 0.5f;
        }

        // dataset data (line mapcount+7)
        tempData = dataArray[index].Split(';').ToList(); index++;
        // min max threshold formula
        electrodesData.minValue = float.Parse(tempData[0],c); if(electrodesData.minField != null) electrodesData.minField.text = tempData[0];
        electrodesData.maxValue = float.Parse(tempData[1],c); if(electrodesData.maxField != null) electrodesData.maxField.text = tempData[1];
        electrodesData.threshold = float.Parse(tempData[2],c); if(electrodesData.thresholdField != null) electrodesData.thresholdField.text = electrodesData.thresholdFieldColors.text = tempData[2];
        if(electrodesData.formulaField != null) electrodesData.formulaField.text = tempData[3];

        // datasets
        // FIRST, clear all current datasets
        if(datasetsPanel != null) ClearDatasets();
        // then add the calculated set
        index = AddDataset(dataArray,index,true);
        // get the number of datasets loaded
        int nbDatasets = int.Parse(dataArray[index],c); index++;
        // loop for each dataset
        for(int i=0;i<nbDatasets;++i) {
            index = AddDataset(dataArray, index, false);
        }

        // at the end, update everything
        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
        // including electrodes scale
        electrodesData.electrodeScale.text = tempScale;
        electrodesData.UpdateElecrodesScale(tempScale);
        // and the UI in options...
        UI.bgButton.color = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor;
        glossinessField.text = (UI._brainLeftRenderer.material.GetFloat("_GlossMapScale")*100).ToString();
        UI.brainButton.color = UI._brainLeftRenderer.material.color;
        // ... and in colors
        if(UI.colorButtons[0] != null) UI.colorButtons[0].color = UI.thresholdColors[0];
        if(UI.colorButtons[1] != null)UI.colorButtons[1].color = UI.thresholdColors[1];
        if(UI.colorButtons[2] != null)UI.colorButtons[2].color = UI.thresholdColors[2];
        // and finally update the number of steps in the slider
        electrodesData.controller.maxValue = electrodesData.proSet.timestamps.Count-1;
    }
    
    void ClearColormaps() {
        if(UI.maps != null) UI.maps.Clear();

        Transform[] L = colormapsPanel.GetComponentsInChildren<Transform>();
        for(int i = 0; i<L.Length;++i) {
            if(L[i].gameObject.CompareTag("Colormap")) Object.Destroy(L[i].gameObject);
        }
    }

    void AddColormap(List<string> data) {
        // on crée une colormap
        Colormap temp = new Colormap(int.Parse(data[1]));
        // on assigne son nom
        temp.name = data[0];
        // on assigne son état inversé ou non
        temp.inverted = bool.Parse(data[2]);
  
        // on boucle sur toute la ligne, par pas de 3, avec temp.size nombre de pas
        for(int i=0; i<temp.size; i++) {
            // on crée et ajoute un Vector3 (RGB) dans la Colormap créée       
            temp.values.Add(new Vector3(int.Parse(data[3+(i*3)],c),
                                        int.Parse(data[4+(i*3)],c),
                                        int.Parse(data[5+(i*3)],c)));
        }

        // on ajoute la nouvelle Colormap à la liste
        UI.maps.Add(temp);

        // on crée le prefab d'UI (si il y a un endroit où le mettre)
        if(colormapsPanel != null) {
            GameObject tempPrefab = (GameObject)Instantiate(UI.prefabUI, new Vector3(0, 0, 0), Quaternion.identity, 
            colormapsPanel);
            // on met les bons textes dans le prefab
            tempPrefab.GetComponentsInChildren<Text>().ElementAt(0).text = temp.size.ToString();
            // on change l'ordre du prefab dans la hiérarchie
            tempPrefab.transform.SetSiblingIndex(tempPrefab.transform.GetSiblingIndex()-1);
            // on assigne le bon callback au bouton remove
            tempPrefab.GetComponentInChildren<Button>().onClick.AddListener(() => UI.RemoveColormap(tempPrefab));
            // on assigne le bon callback au toggle
            tempPrefab.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate {
                                                                                    UI.SelectColormap();   });
            // on assigne le nom de la colormap
            tempPrefab.GetComponentsInChildren<Text>().ElementAt(1).text = temp.name;
            // on set l'index du toggle d'inversion
            tempPrefab.GetComponentInChildren<InverterScript>().index = UI.maps.Count-1;
            // on update l'UI de ce toggle
            tempPrefab.GetComponentInChildren<InverterScript>().GetComponent<Toggle>().SetIsOnWithoutNotify(temp.inverted);
        }
    }

    void ClearDatasets() {
        if(electrodesData.sets != null) electrodesData.sets.Clear();

        Transform[] L = datasetsPanel.GetComponentsInChildren<Transform>();
        for(int i = 0; i<L.Length;++i) {
            if(L[i].gameObject.CompareTag("Dataset")) Object.Destroy(L[i].gameObject);
        }

        electrodesData.datasetsCount = 0;
    }

    int AddDataset(List<string> data, int index, bool pro) {
        List<string> tempData;
        Dataset tempSet;
        int temp;
        GameObject tempPrefab;

        // on parse la première ligne
        tempData = data[index].Split(';').ToList(); index++;
        // On crée un dataset
        tempSet = new Dataset(tempData[0], int.Parse(tempData[2],c));

        // Get the time unit
        tempSet.timeUnit = tempData[1];

        // on récupère les timeStamps
        for(int i = 3; i<tempData.Count(); ++i) {
            // si la string peut être convertie en int, on ajoute cet int à la liste dans le dataset
            if(int.TryParse(tempData[i],NumberStyles.Any,c,out temp)) {
                tempSet.timestamps.Add(temp);
            }
        }

        // on parse les électrodes
        for(int i = 0; i<tempSet.electrodesNb; ++i) {
            // on récupère les infos de la ligne
            tempData = data[index].Split(';').ToList();
//Debug.Log(tempData.Count + " " + data[index]);
            // on crée une nouvelle struct d'electrode et on lui met les infos de base
            ElectrodeLocal EL = new ElectrodeLocal(int.Parse(tempData[0],c),tempData[1],tempData[2],
                float.Parse(tempData[3],c),float.Parse(tempData[4],c),
                float.Parse(tempData[5],c),tempData.Count()-7);

            // on récupère les valeurs de relevés et on les mets dans la struct
            for(int j = 6; j<tempData.Count()-1; ++j) {
                    float.TryParse(tempData[j],NumberStyles.Any,c, out EL.values[j-6]);
            }

            // on ajoute la structure ainsi créée à la liste des électrodes du dataset
            tempSet.data.Add(EL);

            index++;
        }
        
        // si on assigne le set calculé
        if(pro) {
            electrodesData.proSet = tempSet;
            electrodesData.InstantiateElectrodes();
        }
        // sinon, on fait la création normale d'un dataset
        else {
            // on incrémente le compteur de datasets
            electrodesData.datasetsCount++;
            // une fois le dataset complet, on ajoute le dataset à la liste globale
            electrodesData.sets.Add(tempSet);
        
            // on crée le prefab d'UI (si il y a un endroit où le mettre)
            if(datasetsPanel != null) {
                tempPrefab = (GameObject)Instantiate(electrodesData.prefabUI, new Vector3(0, 0, 0), Quaternion.identity, 
                    datasetsPanel.transform);
                // on met les bons textes dans le prefab
                tempPrefab.GetComponentsInChildren<Text>().ElementAt(0).text = "Set" + electrodesData.datasetsCount.ToString();
                tempPrefab.GetComponentsInChildren<Text>().ElementAt(1).text = tempSet.electrodesNb.ToString() + " electrodes";
                tempPrefab.GetComponentsInChildren<Text>().ElementAt(2).text = tempSet.name;
                // on change l'ordre du prefab dans la hiérarchie
                tempPrefab.transform.SetSiblingIndex(tempPrefab.transform.GetSiblingIndex()-1);
                // on assigne le bon callback au bouton remove
                tempPrefab.GetComponentInChildren<Button>().onClick.AddListener(() => electrodesData.RemoveDataset(tempPrefab));
            }
        }

        return index;
    }
    
    public void AutoSave() {
        BinaryFormatter bFormatter = new BinaryFormatter();
		// Create a file using the path
		FileStream file = File.Create(GetComponent<Loader>().path);
		// Serialize the data (textToSave)
		bFormatter.Serialize(file, CompileSaveData());
		// Close the created file
		file.Close();
    }

    private void OnApplicationQuit()
    {
        // if the datasets panel exists (i.e. if we're in the desktop software and not in an online build)
        if(datasetsPanel != null) {
            AutoSave();
        }
    }
    //////// TO DO NEXT /////////
    //  -> save options in another file, and make it autoload whenever the software is started
    //  -> autosave
    //      - before loading a save
    //      - whenever a data changes 
    //  -> option to automatically load the latest autosave on startup
}
