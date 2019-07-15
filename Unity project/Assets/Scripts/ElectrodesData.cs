using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GracesGames.SimpleFileBrowser.Scripts;
using org.mariuszgromada.math.mxparser;

public class ElectrodesData : MonoBehaviour
{
    public Object prefabElectrode;
    public Object prefabUI;
    //public TextAsset data;
    public string data;
    
    //public List<int> timeStamps;
    public List<Dataset> sets;
    public Dataset proSet;
    private List<int> usedSets;
   
    public Slider controller;
    public InputField minField;
    public InputField maxField;
    public InputField thresholdField;
    public InputField thresholdFieldColors;
    public InputField formulaField;
    public GameObject loadingPanel;
    public InputField electrodeScale;

    public float minValue = -5;
    public float maxValue = 5;
    public float threshold = 0;

    public string timeUnit = "ms";

    public int datasetsCount = 0;

    private CultureInfo currentCulture;
    private NumberFormatInfo numberFormat;

    //public Color cMin;
    //public Color cMid1;
    //public Color cMid2;
    //public Color cMax;
////////////////////////////////////
    public char separator = ',';
///////////////////////////////////    

    //private CultureInfo ci;

    //private int electrodesNb;

    void Awake() {
        // GameObject.Find("DemoCaller").GetComponent<DemoCaller>().OpenFileBrowser(false);
        //timeStamps = new List<int>();
        sets = new List<Dataset>();
        usedSets = new List<int>();

        //proSet = new Dataset("proSet", 0);
        if(minField != null) minField.text = minValue.ToString();
        if(minField != null) maxField.text = maxValue.ToString();
        if(thresholdField != null) thresholdField.text = thresholdFieldColors.text = threshold.ToString();

        currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
        numberFormat = (System.Globalization.NumberFormatInfo) currentCulture .NumberFormat.Clone();
        numberFormat .NumberDecimalSeparator = ".";

        electrodeScale.text = "2,5";
    }

    public void UpdateElectrodesColor() {
        // mettre à jour la couleur des électrodes
        foreach(Electrode elec in GetComponentsInChildren<Electrode>()) {
            elec.UpdateColor((int)controller.value, threshold, minValue, maxValue);
        }

        // mettre à jour l'affichage du temps
        if(proSet != null && proSet.timestamps.Count > 0)
        GameObject.Find("TimeStamp").GetComponent<Text>().text = proSet.timestamps[(int)controller.value].ToString() + " " + proSet.timeUnit;
    }

    public void UpdateElecrodesScale (string s) {
        float scale = float.Parse(s);

        // mettre à jour la couleur des électrodes
        foreach(Electrode elec in GetComponentsInChildren<Electrode>()) {
            elec.transform.localScale = new Vector3(scale,scale,scale);
        }
    }
    
    public void SetSeparator(string s) {
        separator = s[0];

        if(separator == ',') {
            numberFormat .NumberDecimalSeparator = ".";
        } else {
            numberFormat .NumberDecimalSeparator = ",";
        }
    }

    private void ReInit() {
        // on réinitialise le dataset calculé
        proSet.timestamps.Clear();
        proSet.data.Clear();
        proSet.electrodesNb = 0;
        proSet.name = "";

        // on clear les électrodes
        foreach (Electrode el in GetComponentsInChildren<Electrode>()) {
            Destroy(el.transform.gameObject);
        }
    }

    // extraire les données de data dans 
 /*   public void ExtractNInstantiate () {
        ReInit();

        List<string> tempData;
        List<string> dataArray;
        int temp;
        float x, y, z;
        GameObject tempPrefab;
        
        // on sépare les données par ligne dans une liste
        //dataArray = data.text.Split('\n').ToList();
        dataArray = data.Split('\n').ToList();
        // on enlève les espaces en début et fin de chaque ligne (pour ne pas avoir de ligne vide)
        foreach(string s in dataArray) {
            s.Trim();
        }

        // on parse la première ligne
        tempData = dataArray[0].Split(separator).ToList();
        // on récupère le nb d'électrodes
        //int.TryParse(tempData[0],out electrodesNb);
        // on récupère les timeStamps
        for(int i = 1; i<tempData.Count; ++i) {
            // si la string peut être convertie en int, on ajoute cet int à la liste
            if(int.TryParse(tempData[i],out temp)) {
                timeStamps.Add(temp);
            }
        }

        // on met à jour l'UI en adaptant le slider
        controller.maxValue = timeStamps.Count-1;

        // on parse les électrodes
        for(int i = 1; i<dataArray.Count; ++i) {
            if(dataArray[i] != "") {
                // on récupère les infos de la ligne
                tempData = dataArray[i].Split(separator).ToList();
                
                // on instancie un prefab d'électrode pour chaque ligne
                int.TryParse(tempData[0],out temp);
                /*x = float.Parse(tempData[2], CultureInfo.InvariantCulture);
                y = float.Parse(tempData[3], CultureInfo.InvariantCulture);
                z = float.Parse(tempData[4], CultureInfo.InvariantCulture);*
                x = float.Parse(tempData[2]);
                y = float.Parse(tempData[3]);
                z = float.Parse(tempData[4]);
                tempPrefab = (GameObject)Instantiate(prefabElectrode, new Vector3(x, y, z), Quaternion.identity, transform);
                tempPrefab.GetComponent<Electrode>().SetData(temp, tempData[1]);
                tempPrefab.GetComponent<Electrode>().CreateValues(timeStamps.Count);

                /*
                for(int j = 5; j<tempData.Count; ++j) {
                    if((j-5)/timeStamps.Count < 1) {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M1values[j-5]);
//??????                        tempPrefab.GetComponent<Electrode>().M1values[j-5] = float.Parse(tempData[j], CultureInfo.InvariantCulture);
                    } else if ((j-5)/timeStamps.Count < 2) {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M2values[j-timeStamps.Count-5]);
//??????                        tempPrefab.GetComponent<Electrode>().M2values[j-timeStamps.Count-5] = float.Parse(tempData[j], CultureInfo.InvariantCulture);
                    } else {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M3values[j-(timeStamps.Count*2)-5]);
//??????                        tempPrefab.GetComponent<Electrode>().M3values[j-(timeStamps.Count*2)-5] = float.Parse(tempData[j], CultureInfo.InvariantCulture);
                    }
                    //tempPrefab.GetComponent<Electrode>().values[j-5] = float.Parse(tempData[j], CultureInfo.InvariantCulture);
                    //tempPrefab.GetComponent<Electrode>().values[j-5] = float.Parse(tempData[j]);
                }
                *
                for(int j = 5; j<tempData.Count; ++j) {
                    if((j-5)/timeStamps.Count < 1) {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M1values[j-5]);
                    } else { //if ((j-5)/timeStamps.Count < 2) {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M2values[j-timeStamps.Count-5]);
                    } /*else if ((j-5)/timeStamps.Count < 3) {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M3values[j-(timeStamps.Count*2)-5]);
                    } else {
                        float.TryParse(tempData[j], out tempPrefab.GetComponent<Electrode>().M4values[j-(timeStamps.Count*3)-5]);
                    }*
                }
            }
        }
    }*/

    public void UpdateThreshold(InputField caller) {

        threshold = float.Parse(caller.text, numberFormat);

        thresholdField.text = thresholdFieldColors.text = threshold.ToString();

        // on met à jour les électrodes
        UpdateElectrodesColor();
    }

    public void UpdateMin() {
        minValue = float.Parse(minField.text, numberFormat);
        
        // on met à jour les électrodes
        UpdateElectrodesColor();
    }
    public void UpdateMax() {
        maxValue = float.Parse(maxField.text, numberFormat);        
        
        // on met à jour les électrodes
        UpdateElectrodesColor();
    }

    // extraire les données de data dans 
    public void CreateDataset (string path) {
        List<string> tempData;
        List<string> dataArray;

        GameObject tempPrefab;
        Dataset tempSet;
        int temp;
        int tempElectrodeNb;
        
        // on sépare les données par ligne dans une liste
        dataArray = data.Split('\n').ToList();
        // on enlève les espaces en début et fin de chaque ligne (pour ne pas avoir de ligne vide)
        foreach(string s in dataArray) {
            s.Trim();
        }

        // on parse la première ligne
        tempData = dataArray[0].Split(separator).ToList();
        // on récupère le nb d'électrodes
        int.TryParse(tempData[0],out tempElectrodeNb);

        // On crée un dataset
        tempSet = new Dataset(path.Split('\\').Last(), tempElectrodeNb);

        // on sauvegarde l'unité de temps
        tempSet.timeUnit = tempData[1];

        // on récupère les timeStamps
        for(int i = 2; i<tempData.Count(); ++i) {
            // si la string peut être convertie en int, on ajoute cet int à la liste dans le dataset
            if(int.TryParse(tempData[i],out temp)) {
                tempSet.timestamps.Add(temp);
            }
        }

        // on parse les électrodes
        for(int i = 1; i<dataArray.Count(); ++i) {
            if(dataArray[i] != "") {
                // on récupère les infos de la ligne
                tempData = dataArray[i].Split(separator).ToList();
                
                // on crée une nouvelle struct d'electrode et on lui met les infos de base
                ElectrodeLocal EL = new ElectrodeLocal(int.Parse(tempData[0],numberFormat),tempData[1],tempData[2],
                    float.Parse(tempData[3],numberFormat),float.Parse(tempData[4],numberFormat),float.Parse(tempData[5],numberFormat),tempData.Count()-6);

                // on récupère les valeurs de relevés et on les mets dans la struct
                for(int j = 6; j<tempData.Count(); ++j) {
                    float.TryParse(tempData[j], NumberStyles.Float, numberFormat, out EL.values[j-6]);
                }

                // on ajoute la structure ainsi créée à la liste des électrodes du dataset
                tempSet.data.Add(EL);
            }
        }
        
        // ici on peut faire différent check => UNIQUEMENT TEST SUR DATASET ACTUEL
        // si le nb d'électrodes dans la liste est différent de nbelectrodes, il y a un soucis on interrompt l'ajout
        if(tempSet.data.Count != tempElectrodeNb) {
            Debug.Log("Error : Specified number of electrodes doesn't match with the actual number of electrode lines in the file, please check for discrepancies. Dataset will not be added");
            return;
            
            ////////////////// AJOUTER ///////////////
            // affichage d'une pop-up avec les différents messages d'erreur pour version buildée
            ////////////////// AJOUTER ///////////////

        } else {
            foreach (var item in tempSet.data)
            {
                // si le nombre de relevé d'une électrode est différent du nombre de timestamps
                if(item.values.Length != tempSet.timestamps.Count()) {
                    Debug.Log("Error : At least one electrode has a number of values different from the provided number of timestamps, please check for discrepancies. Dataset will not be added.");
                    return;
                }
            }
        }

        /*! Si tout va bien : !*/
        // on incrémente le compteur de datasets
        ++datasetsCount;
        // une fois le dataset complet, on ajoute le dataset à la liste globale
        sets.Add(tempSet);

        // on crée le prefab d'UI
        tempPrefab = (GameObject)Instantiate(prefabUI, new Vector3(0, 0, 0), Quaternion.identity, 
            GameObject.FindWithTag("DatasetsPanel").transform);
        // on met les bons textes dans le prefab
        tempPrefab.GetComponentsInChildren<Text>().ElementAt(0).text = "Set" + datasetsCount.ToString();
        tempPrefab.GetComponentsInChildren<Text>().ElementAt(1).text = tempSet.electrodesNb.ToString() + " electrodes";
        tempPrefab.GetComponentsInChildren<Text>().ElementAt(2).text = tempSet.name;
        // on change l'ordre du prefab dans la hiérarchie
        tempPrefab.transform.SetSiblingIndex(tempPrefab.transform.GetSiblingIndex()-1);
        // on assigne le bon callback au bouton remove
        tempPrefab.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveDataset(tempPrefab));
    }

    public void RemoveDataset (GameObject caller) {
        // supprimer le dataset dans la liste
        foreach (var item in sets)
        {
            if(item.name == caller.GetComponentsInChildren<Text>().ElementAt(2).text) {                
                sets.Remove(item);
                break;
            }
        }
    
        Transform tSets = caller.transform.parent;
        // refaire les var ID
        int counter = 1;
        for (int i = 1; i<datasetsCount; ++i) {
            //tSets.GetChild(i).GetChild(0).GetComponent<Text>().text = (i+1).ToString();
            //Debug.Log(i + " " + tSets.GetChild(i).GetChild(0).GetComponent<Text>().text);
            // CE TEST MARCHE PAS NON PLUS
            if(tSets.GetChild(i) != caller) {
                tSets.GetChild(i).GetChild(0).GetComponent<Text>().text = "Set" + counter.ToString();
                ++counter;
            }
        }
        // décrémenter le compteur de datasets
        --datasetsCount;
        // détruire le prefab
        Destroy(caller);
    }

    public bool CheckFormula() {
        if(datasetsCount <= 0) return false;  

        // on reset usedSets
        usedSets.Clear();

        // si l'utilisateur n'a rien entré dans le champ de formule
        if(formulaField.text.Length <= 0) {
            // on utilise la formule par défaut
            formulaField.text = "Set1";

            usedSets.Add(1);
            // on valide
            return true;
        }

        // si il n'y a pas de SetN  dans la string | Si la formule ne contient pas au moins une variable
        if (formulaField.text.IndexOf("Set") == -1) {

            ////////////////// AJOUTER ///////////////
            // on envoie un message d'erreur
            ////////////////// AJOUTER ///////////////

            // on invalide la formule
            return false;
        }

        // sinon, on lance la récupération des datasets utilisés
        int at = 0, start = 0;
        while(start<formulaField.text.Length && at != -1)
        {
            at = formulaField.text.IndexOf("Set", start);
            
            if(at != -1) {
                // on crée le sous-compteur de test et on lui ajoute 3 (pour aller au début des chiffres du SetN)
                int cat = at + 3;
                // tant qu'il y a un chiffre
                while(cat<formulaField.text.Length && char.IsDigit(formulaField.text,cat)){
//Debug.Log(formulaField.text.ElementAt(cat));
                    ++cat;
                }
            
                // on convertit la portion de string correspondante en int, qu'on ajoute à usedSets
//Debug.Log(formulaField.text.Substring(at+3,cat-(at+3)));
                usedSets.Add(int.Parse(formulaField.text.Substring(at+3,cat-(at+3))));
            }
            
            start = at + 1;
        }
//Debug.Log(usedSets.Count);
        // si aucun problème constaté : on return true
        return true;
    }

    public void CalculateFinalSet () {
        // si le check de la formule ne passe pas
        if (!CheckFormula()) {
            // afficher message pour dire de régler les erreurs d'abord

            // sortir de la fonction : on ne fait pas le calcul
            return;
        }

        // pour stocker min et max
        float minTest = 0;
        float maxTest = 0;

        // On active le panneau d'info de loading
        loadingPanel.SetActive(true);

        // On vide le dataset calculé avant s'il y en a un
        //proSet.Clear();
        // on supprime les électrodes déjà instanciées
        foreach(Electrode elec in GetComponentsInChildren<Electrode>()) {
            Destroy(elec.gameObject);
        }
        // on remet à 0 le controller
        controller.value = 0;

        // on crée le compteur et le tableau pour stocker les arguments
        int argCounter = 0;
        Argument[] arg =  new Argument[usedSets.Count];

        // on instancie les arguments et on set leur nom
        foreach (int k in usedSets){
           arg[argCounter] = new Argument("Set"+k, 0);
           ++argCounter;
//Debug.Log("Set"+k);      
        }
        // on reset le compteur utilisé
        argCounter = 0;

        // On crée l'expression
        Expression e = new Expression(formulaField.text,arg);

        // on crée le dataset final
//Debug.Log(sets[0].name);        
        proSet = new Dataset("proSet",sets[usedSets[0]-1].electrodesNb);
        // on choisit l'unité de temps du premier set
        proSet.timeUnit = sets[usedSets[0]-1].timeUnit;

        // on instancie les timestamps réels des datasets choisis
        //proSet.timestamps = sets[usedSets[0]-1].timestamps;
        foreach (int item in sets[usedSets[0]-1].timestamps){
            proSet.timestamps.Add(item);
        }
        // on met à jour l'UI en adaptant le slider
        controller.maxValue = proSet.timestamps.Count-1;

        // pour le nombre d'électrodes des sets
        for(int i = 0; i<proSet.electrodesNb; ++i) {
            // On crée une nouvelle électrode
            ElectrodeLocal ELL = new ElectrodeLocal(sets[usedSets[0]-1].data[i].ID, sets[usedSets[0]-1].data[i].name,
                sets[usedSets[0]-1].data[i].patient, sets[usedSets[0]-1].data[i].coords.x, sets[usedSets[0]-1].data[i].coords.y,
                sets[usedSets[0]-1].data[i].coords.z, sets[usedSets[0]-1].data[i].values.Length);

                // on boucle sur le nombre de timestamps
                for(int j=0; j<proSet.timestamps.Count; ++j) {
                    // on set toutes les variables du calcul
                    foreach (int k in usedSets){
                        arg[argCounter].setArgumentValue(sets[k-1].data[i].values[j]);
                        ++argCounter;
                    }

                    // on calcule la valeur et on la met dans l'électrode en train d'être créée
                    //Expression e = new Expression(formulaField.text,arg);
//Debug.Log(e.calculate());
                    ELL.values[j] = (float)e.calculate();
                    if(ELL.values[j] < minTest) minTest = ELL.values[j];
                    else if(ELL.values[j] > maxTest) maxTest = ELL.values[j];

                    // on reset argCounter
                    argCounter = 0;
                }
            
            // On insère l'électrode dans le dataset final
            proSet.data.Add(ELL);

            // On met à jour les données de min et max
            minValue = minTest;
            maxValue = maxTest;
            // On met à jour leur affichage
            minField.text = minValue.ToString();
            maxField.text = maxValue.ToString();
        }

//        // Si le champ max n'a pas été rempli
//            
//            // On cherche le max de la série de valeurs et il devient le max
//
//        // Si le champ min n'a pas été rempli
//
//            // On cherche le min de la série de valeurs et il devient le min
//

        // On crée les électrodes dans la visualisation
        InstantiateElectrodes();

        // On a fini, on désactive le panneau d'info de loading
        loadingPanel.SetActive(false);

        // On affiche les bonnes couleurs pour les électrodes
        UpdateElectrodesColor();

        // On update la taille des électrodes
        UpdateElecrodesScale(electrodeScale.text);
    }

    public void InstantiateElectrodes() {

        // on remet le cerveau en position par défaut pour éviter les soucis de coordonnées
        GameObject.FindWithTag("Player").transform.Find("Left").transform.rotation = Quaternion.identity;
        GameObject.FindWithTag("Player").transform.Find("Left").transform.position = Vector3.zero;
        GameObject.FindWithTag("Player").transform.Find("Right").transform.rotation = Quaternion.identity;
        GameObject.FindWithTag("Player").transform.Find("Right").transform.position = Vector3.zero;

        GameObject tempPrefab; 
        int index = 0;

        foreach(ElectrodeLocal elToInstantiate in proSet.data) {

            ////////////// Invert x coord //////////////
            //elToInstantiate.coords.x = -elToInstantiate.coords.x;
            //elToInstantiate.coords = new Vector3(0,0,0);
            ////////////////////////////////////////////

            if(elToInstantiate.coords.x < 0) {
                tempPrefab = (GameObject)Instantiate(prefabElectrode, 
                            new Vector3(-elToInstantiate.coords.x,elToInstantiate.coords.y,elToInstantiate.coords.z), 
                            Quaternion.identity, transform.GetChild(1));
            } else {
                tempPrefab = (GameObject)Instantiate(prefabElectrode, 
                            new Vector3(-elToInstantiate.coords.x,elToInstantiate.coords.y,elToInstantiate.coords.z), 
                            Quaternion.identity, transform.GetChild(0));
            }
            tempPrefab.GetComponent<Electrode>().SetData(elToInstantiate.ID, elToInstantiate.name, elToInstantiate.patient);
            tempPrefab.GetComponent<Electrode>().CreateValues(proSet.timestamps.Count);
            
            foreach (float item in elToInstantiate.values){
//Debug.Log(item + " " + index);
                tempPrefab.GetComponent<Electrode>().values[index] = item;
                ++index;
            }
            
            index = 0;
        }
    }
}

public class Dataset {
    public string name;
    public List<int> timestamps;
    public int electrodesNb;
    public List<ElectrodeLocal> data;
    public string timeUnit;

    public Dataset(string _name, int _electrodesNb) {
        name = _name;
        electrodesNb = _electrodesNb;
        timestamps = new List<int>();
        data = new List<ElectrodeLocal>();
    }

    /*public void Clear() {
        name = "";
        timestamps.Clear();
        electrodesNb = 0;
        data.Clear();
    }*/
}

// structure pour stocker les electrodes dans les datasets (similaire à ce qu'il y a dans électrodes finale)
public struct ElectrodeLocal {
    public int ID;
    public string patient;
    public string name;
    public Vector3 coords;
    public float[] values;

    public ElectrodeLocal (int _ID, string _name, string _patient, float x, float y, float z, int size){
        ID = _ID;
        name = _name;
        patient = _patient;
        coords = new Vector3(x,y,z);
        values = new float[size];
    }
}