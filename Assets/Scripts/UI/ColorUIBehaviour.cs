using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColorUIBehaviour : MonoBehaviour
{
    public ColorSelectorBehaviour SelectorPrefab;
    public TeamColorSelections AvailiableColors;

    public UnityEvent<Color> OnColorSelected;

    public Color DefaultColor => AvailiableColors.Colors[0];    

    private void Start()
    {        
        foreach(var col in AvailiableColors.Colors)
        {
            var newSwatch = Instantiate(SelectorPrefab, this.transform) as ColorSelectorBehaviour;
            newSwatch.ColorSwatch.color = col;
            newSwatch.OnColorSelected.AddListener((arg) =>
            {
                OnColorSelected.Invoke(arg);                
            });
        }        
    }
}
