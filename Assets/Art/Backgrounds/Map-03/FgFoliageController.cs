using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FgFoliageController : MonoBehaviour
{
    public GameObject FoliageFG;
    // Update is called once per frame
    void Update()
    {
        if (MapManager.currentMap == 3)
        {
            FoliageFG.SetActive(true);
        }
        else
        {
            FoliageFG.SetActive(false);
        }
    }
}
