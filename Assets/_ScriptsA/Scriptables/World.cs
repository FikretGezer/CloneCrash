using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level Creation/World", order = 0)]
public class World : ScriptableObject
{
    public Level[] levels;
}
