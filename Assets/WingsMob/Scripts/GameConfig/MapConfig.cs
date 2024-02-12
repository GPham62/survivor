using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Map", fileName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    public string Name;
    [TextArea]
    public string Description;
    public Sprite Icon;
}