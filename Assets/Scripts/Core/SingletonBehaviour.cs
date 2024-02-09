using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour
{
    public static SingletonBehaviour<T> Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
}
