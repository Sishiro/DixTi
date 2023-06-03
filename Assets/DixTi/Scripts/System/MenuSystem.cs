using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;

    public void ChangeScene(int panel)
    {
        DeactivatePanels();
        panels[panel].SetActive(true);
    }

    private void DeactivatePanels()
    {
        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }
    }
}
