using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Colors", menuName = "Custom/ColorLibrary")]
public class ColorLibrary : ScriptableObject
{
    public List<ColorPreset> presets = new List<ColorPreset>();  // 컬러 리스트

}

[System.Serializable]
public class ColorPreset
{
    public string name;  // 이름 지정 
    public Color color;  // 컬러 값
}
