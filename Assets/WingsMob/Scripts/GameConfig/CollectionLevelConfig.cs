using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Config/Collections", fileName = "CollectionLevel")]
public class CollectionLevelConfig : ScriptableObject
{
    public Vector2[] positions;
}