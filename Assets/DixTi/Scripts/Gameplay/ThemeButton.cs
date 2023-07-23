using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeButton : MonoBehaviour
{
    [SerializeField] private ImageData imageData;

    public void OnClickEvent()
    {
        PlayerManager.Instance.controller.ChooseTheme(imageData);
    }
}
