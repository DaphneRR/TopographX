using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;
 
    public Vector3 _LocalRotation;
    public float _CameraDistance = 10f;
 
    public float MouseSensitivity = 4f;
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;
 
    public bool CameraDisabled = true;
    public bool _noUI = true;
    public bool currentUI = false; // is there currently an action on the UI ?

    public GameObject UI;

    private float cooldown = 0;
 
    // Use this for initialization
    void Awake() {
        this._XForm_Camera = this.transform.GetChild(0);
        this._XForm_Parent = this.transform;
    }

    private void Update()
    {
        if(_noUI && !currentUI) {
            if (Input.GetMouseButtonDown(0)) CameraDisabled = false;
            if (Input.GetMouseButtonUp(0)) CameraDisabled = true;
        }

        cooldown += Time.deltaTime;
        if(cooldown > 0.250) {
            if(Input.GetButton("Hide all UI"))
            {
                UI.SetActive(!UI.activeSelf);
                cooldown = 0;
            }
        }
    }
 
    private void LateUpdate() { 
        if (!CameraDisabled)
        {
/*         
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.z += Input.GetAxis("Mouse X") * MouseSensitivity;
                _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;
                //_XForm_Parent.Rotate(0, _LocalRotation.y, _LocalRotation.z, Space.Self);
                //_XForm_Parent.Rotate(Vector3.up, _LocalRotation.z, Space.Self);
                _XForm_Parent.Rotate(Vector3.forward, _LocalRotation.y, Space.Self);
            }
*/
///*
            //Rotation of the Camera based on Mouse Coordinates
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _LocalRotation.z += Input.GetAxis("Mouse X") * MouseSensitivity;
                _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;
 
                //Clamp the y Rotation to horizon and not flipping over at the top
                if (_LocalRotation.y < -90f)
                    _LocalRotation.y = -90f;
                else if (_LocalRotation.y > 90f)
                    _LocalRotation.y = 90f;
            }
//*/
        }
///*
        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(0, _LocalRotation.y, _LocalRotation.z);
        this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);
//*/ 
        //Zooming Input from our Mouse Scroll Wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

            ScrollAmount *= (this._CameraDistance * 0.3f);

            this._CameraDistance += ScrollAmount * -1f;

            this._CameraDistance = Mathf.Clamp(this._CameraDistance, -30f, 320f);
        }

        if ( this._XForm_Camera.localPosition.z != this._CameraDistance * -1f )
        {
            //this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
            this._XForm_Camera.localPosition = new Vector3(Mathf.Lerp(this._XForm_Camera.localPosition.x, (this._CameraDistance * -1f)-40.0f, Time.deltaTime * ScrollDampening), 0f, 0f);
        }
    }
}
