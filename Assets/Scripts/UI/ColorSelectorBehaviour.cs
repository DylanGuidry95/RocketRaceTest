using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorSelectorBehaviour : MonoBehaviour
{
    private Button _buttonRef;
    public RawImage Border;
    public RawImage ColorSwatch;

    public UnityEvent<Color> OnColorSelected;

    private void Awake()
    {
        _buttonRef = GetComponent<Button>();        
        _buttonRef.onClick.AddListener(() =>
        {                            
            OnColorSelected.Invoke(ColorSwatch.color);                                    
        });
    }
}
