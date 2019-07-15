using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterScript : MonoBehaviour
{
    // index of the linked colormap
    public int index;

    // invert shit
    public void InvertColormap(bool x) {
        GameObject.FindWithTag("UIController").GetComponent<UiController>().maps[index].inverted = x;
        GameObject.FindWithTag("Player").GetComponent<ElectrodesData>().UpdateElectrodesColor();
    }
}
