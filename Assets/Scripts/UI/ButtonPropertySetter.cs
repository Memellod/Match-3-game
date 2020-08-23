using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPropertySetter : MonoBehaviour
{
    [SerializeField] private string property = string.Empty;
    private void Start()
    {
        if (!PlayerPrefs.HasKey(property)) return;

        gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPrefs.GetInt(property) == 1);
    }

}
