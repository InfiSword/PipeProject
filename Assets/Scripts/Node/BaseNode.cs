using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode : MonoBehaviour
{
    public Vector2Int Pos2D; 
    public abstract void Init();    
}
