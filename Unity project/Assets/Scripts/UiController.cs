using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiController : MonoBehaviour
{
    public GameObject _datasets;
    public GameObject _file;
    public GameObject _options;
    public Toggle _axesToggle;
    public Slider _controller;
    public Slider _speedSlider;
    public Button _play;
    public Sprite playSprite;
    public Sprite stopSprite;
    public Animator _left;
    public Animator _right;

    private CameraOrbit mCamera;
    
    public Renderer _brainLeftRenderer;
    public Renderer _brainRightRenderer;
    //public Material _brainMaterial;

    /////
    int reset;
    //// 
    
    private float _baseSpeed = 975;
    private float _maxSpeed = 1100;
    private float _speed = 550;

    // Background color button
    public Image bgButton;
    // Brain color button
    public Image brainButton;

    // Colors accessed by the electrodes
    // 0 is default | 1 is under threshold | 2 is above or equal to threshold
    public Color[] thresholdColors = new Color[3];
    int colorToSet = 0;
    public Image[] colorButtons;
    // Colors visibility
    public bool[] colorsVisible = new bool[3];
    // Color mode
    public bool colormapMode = false;
    public Toggle[] modeToggles;

    public bool outsideDefault = false;

    public GameObject prefabUI;
    public GameObject colormapNameWindow;

    // Color maps
    public List<Colormap> maps;
    // currently selected colormap
    public int currentColormap = 0;

    private void Awake()
    {
        thresholdColors[0] = new Color(0, 0, 0, 0.8f);
        thresholdColors[1] = new Color(1f, 0, 0, 1f);
        thresholdColors[2] = new Color(0.5f, 0.5f, 0.5f, 1f);

        if(colorButtons[0] != null) colorButtons[0].color = thresholdColors[0];
        if(colorButtons[1] != null) colorButtons[1].color = thresholdColors[1];
        if(colorButtons[2] != null) colorButtons[2].color = thresholdColors[2];

        colorsVisible[0] = true;
        colorsVisible[1] = true;
        colorsVisible[2] = true;

        maps = new List<Colormap>();
        bgButton.color = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor;
        brainButton.color = _brainLeftRenderer.material.color;
        _play.onClick.AddListener(Play);
        mCamera = GameObject.FindWithTag("MainCamera").transform.parent.GetComponent<CameraOrbit>();
    }

    private void Update()
    {
        if(Input.GetKeyDown("left")) _controller.value--;
        if(Input.GetKeyDown("right")) _controller.value++;

        if(Input.GetMouseButtonDown(0)) {
            // if the click is over a UI element, we set the bool telling a UI event is ongoing to true
            if(EventSystem.current.IsPointerOverGameObject()) mCamera.currentUI = true;
            // if the click is not over a UI element and some UI window is displayed, we close all windows
            else if(!EventSystem.current.IsPointerOverGameObject() && !mCamera._noUI) CloseAllWindows();
        } else if(Input.GetMouseButtonUp(0)) {
            mCamera.currentUI = false;
        }
    }

    public void CloseAllWindows() {
        // close other windows
        if(_file != null) _file.SetActive(false);
        if(_datasets != null) _datasets.SetActive(false);
        if(_options != null)_options.SetActive(false);
        mCamera._noUI = true;
    }

    public void DisplayDatasets () {
        if(_datasets.activeSelf) {
            CloseAllWindows();
            EnableCamera();
            //_datasets.SetActive(false);
        } else {
            CloseAllWindows();
            DisableCamera();
            _datasets.SetActive(true);
        }
    }

    public void DisplayOptions () {
        if(_options.activeSelf) {
            CloseAllWindows();
            EnableCamera();
            //_options.SetActive(false);
        } else {
            CloseAllWindows();
            DisableCamera();
            _options.SetActive(true);
        }
    }

    public void DisplayFile () {
        if(_file.activeSelf) {
            CloseAllWindows();
            EnableCamera();
            //_file.SetActive(false);
        } else {
            CloseAllWindows();
            DisableCamera();
            _file.SetActive(true);
        }
    }

    public void DisableCamera () {
        mCamera._noUI = false;
        mCamera.CameraDisabled = true;
    }

    public void EnableCamera () {
        mCamera._noUI = true;
    }

        // suspend execution for waitTime seconds
    private IEnumerator<UnityEngine.WaitForSeconds> IncrementWithSpeed()
    {
        _play.image.sprite = stopSprite;
        reset = (int)_controller.value;

        for(int i = reset; i<=_controller.maxValue; i=(int)++_controller.value) {
            // wait for speed milliseconds
            yield return new WaitForSeconds(_speed/1000);   
        }

        _controller.value = reset;
        //Stop();
        //_play.GetComponentInChildren<Text>().text = "Play";
        _play.image.sprite = playSprite;
        _play.onClick.RemoveAllListeners();
        _play.onClick.AddListener(Play);
    }

    public void Play() {
        StartCoroutine(IncrementWithSpeed());
        //_play.GetComponentInChildren<Text>().text = "Stop";
        _play.onClick.RemoveAllListeners();
        _play.onClick.AddListener(Stop);
    }

    public void Stop() {
        StopAllCoroutines();
        _play.image.sprite = playSprite;
        //_play.GetComponentInChildren<Text>().text = "Play";
        _play.onClick.RemoveAllListeners();
        _play.onClick.AddListener(Play);
    }

    public void GoBack() {
        _controller. value = 0f;
    }

    public void SetSpeed() {
        _speed = _maxSpeed-(_baseSpeed*_speedSlider.value);
    }

    public void OpenBrain() {
        _left.SetBool("Open",true);
        _right.SetBool("Open",true);
    }

    public void CloseBrain() {
        _left.SetBool("Open",false);
        _right.SetBool("Open",false);
    }

    /*public void BrainTransparency(string s) {
        float percent = float.Parse(s);
        _brainMaterial.color = new Color(_brainMaterial.color.r, _brainMaterial.color.g, _brainMaterial.color.b, percent/100);
    }*/

    public void BrainSpecular(string s) {
        //_brainLeftRenderer.material.shader = Shader.Find("Specular");
        float percent = float.Parse(s);
        percent = Mathf.Clamp(percent,0.0f,100.0f);
        _brainLeftRenderer.material.SetFloat("_GlossMapScale",percent/100);
        _brainRightRenderer.material.SetFloat("_GlossMapScale",percent/100);
    }

    public void ColorToSet(string which) {
        if(which == "default") {
            GetComponentInChildren<ColorPicker>().CurrentColor = thresholdColors[0];
            colorToSet = 0;
        } else if (which == "under") {
            GetComponentInChildren<ColorPicker>().CurrentColor = thresholdColors[1];
            colorToSet = 1;
        } else if (which == "above") {
            GetComponentInChildren<ColorPicker>().CurrentColor = thresholdColors[2];
            colorToSet = 2;
        } else if (which == "background") {
            GetComponentInChildren<ColorPicker>().CurrentColor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor;
            colorToSet = 3;
        } else if (which == "brain") {
            GetComponentInChildren<ColorPicker>().CurrentColor = _brainLeftRenderer.material.color;
            colorToSet = 4;
        }
    }

    public void SetColor() {
        // Si c'est la couleur de background
        if (colorToSet == 3) {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = GetComponentInChildren<ColorPicker>().CurrentColor;
            bgButton.color = GetComponentInChildren<ColorPicker>().CurrentColor;
            // On sort de la fonction 
            return;
        }
        // Si c'est la couleur du cerveau
        else if (colorToSet == 4) {
            _brainLeftRenderer.material.color = brainButton.color = GetComponentInChildren<ColorPicker>().CurrentColor;
            _brainRightRenderer.material.color = brainButton.color = GetComponentInChildren<ColorPicker>().CurrentColor;
            return;
        }

        thresholdColors[colorToSet] = GetComponentInChildren<ColorPicker>().CurrentColor;
        if(colorToSet == 0) {
            colorButtons[0].color = thresholdColors[0];
        } else if (colorToSet == 1) {
            colorButtons[1].color = thresholdColors[1];
        } else if (colorToSet == 2) {
            colorButtons[2].color = thresholdColors[2];
        }

        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }

    public void InvertSelection(string a) {
        Toggle t = modeToggles[0];
        Toggle c = modeToggles[1];
        
        if(a == "T") {
            c.isOn = !t.isOn;
        } else if (a == "C") {
            t.isOn = !c.isOn;
        }
        colormapMode = c.isOn;

        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }

    public void AddColormap(string data) {
        List<string> dataArray;
        List<string> tempData;
        
        // on sépare les données par ligne dans une liste
        dataArray = data.Split('\n').ToList();
        // on enlève les espaces en début et fin de chaque ligne (pour ne pas avoir de ligne vide)
        foreach(string s in dataArray) {
            s.Trim();
        }

        // on crée une colormap
        Colormap temp = new Colormap(dataArray.Count);

        // pour chaque ligne
        foreach(string s in dataArray) {
            // on extrait les données
            tempData = s.Split(' ').ToList();

            // on crée et ajoute un Vector3 (RGB) dans la Colormap créée       
            temp.values.Add(new Vector3(int.Parse(tempData[0]),
                                        int.Parse(tempData[1]),
                                        int.Parse(tempData[2])));      
        }

        // on ajoute la nouvelle Colormap à la liste
        maps.Add(temp);

        // on crée le prefab d'UI
        GameObject tempPrefab = (GameObject)Instantiate(prefabUI, new Vector3(0, 0, 0), Quaternion.identity, 
        GameObject.FindWithTag("ColormapsPanel").transform);
        // on met les bons textes dans le prefab
        tempPrefab.GetComponentsInChildren<Text>().ElementAt(0).text = temp.size.ToString();
        // on change l'ordre du prefab dans la hiérarchie
        tempPrefab.transform.SetSiblingIndex(tempPrefab.transform.GetSiblingIndex()-1);
        // on assigne le bon callback au bouton remove
        tempPrefab.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveColormap(tempPrefab));
        // on assigne le bon callback au toggle
        tempPrefab.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate {
                                                                                SelectColormap();   });
        // on active le toggle si il n'y a qu'une seule colormap
        if(maps.Count == 1) {
            tempPrefab.GetComponentInChildren<Toggle>().isOn = true;
            tempPrefab.GetComponentInChildren<Toggle>().interactable = false;
            tempPrefab.GetComponentInChildren<Button>().interactable = false;
        }
        // on set l'index du toggle d'inversion
        tempPrefab.GetComponentInChildren<InverterScript>().index = maps.Count-1;
        // on affiche la fenêtre pour demander un nom de colormap
        colormapNameWindow.SetActive(true);
    }

    public void RemoveColormap(GameObject caller) {
        // supprimer le dataset dans la liste
        foreach (var item in maps)
        {
            if(item.name == caller.GetComponentsInChildren<Text>().ElementAt(1).text) {  
                maps.Remove(item);
                break;
            }
        }

        // détruire le prefab
        Destroy(caller);
    }

    public void SetColormapName () {
        GameObject cpanel = GameObject.FindWithTag("ColormapsPanel");
        
        // on set le nom dans l'UI
        cpanel.transform.GetChild(cpanel.transform.childCount-2).GetComponentsInChildren<Text>().ElementAt(1).text =
            colormapNameWindow.GetComponentInChildren<InputField>().text;
        
        // on set le nom dans la colormap
        maps.Last().name = colormapNameWindow.GetComponentInChildren<InputField>().text;

        // on reset le text du InputField
        colormapNameWindow.GetComponentInChildren<InputField>().text = "";

        // on cache la fenêtre pour demander un nom de colormap
        colormapNameWindow.SetActive(false);
    }

    public void SelectColormap() {
        int index = 0;
        foreach(GameObject m in GameObject.FindGameObjectsWithTag("Colormap")) {
            if(m.GetComponentInChildren<Toggle>().isOn == true) {

                if(m.GetComponentInChildren<Toggle>().interactable == false) {
                    m.GetComponentInChildren<Toggle>().SetIsOnWithoutNotify(false);
                    m.GetComponentInChildren<Toggle>().interactable = true;
                    m.GetComponentInChildren<Button>().interactable = true;
                } else {
                    m.GetComponentInChildren<Toggle>().interactable = false;
                    m.GetComponentInChildren<Button>().interactable = false;
                    currentColormap = index;
                }
            }

            ++index;
        }

        // update couleurs électrodes
        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }

    /*public void SetColormapMode (bool h) {
        colormapMode = h;
    }*/

    public void UpdateUnderVisible(bool a) {
        colorsVisible[1] = a;
        
        // on met à jour les électrodes
        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }
    public void UpdateGreaterVisible(bool a) {
        colorsVisible[2] = a;
        
        // on met à jour les électrodes
        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }
    public void UpdateDefaultVisible(bool a) {
        colorsVisible[0] = a;

        // on met à jour les électrodes
        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }

    public void CloseApp() {
        Application.Quit();
    }
}

public class Colormap
{
    public int size;
    public List<Vector3> values;
    public string name;
    public bool inverted;

    public Colormap(int s) {
        size = s;
        values = new List<Vector3>();
        inverted = false;
    }
}
