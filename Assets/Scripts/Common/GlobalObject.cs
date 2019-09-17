using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    static GameObject instance;

    private void Start() {
        if (instance == null)
            instance = gameObject;
        else
            Destroy(gameObject);
    }

    public static T Get<T>()
    {
        return instance.GetComponent<T>();
    }
}
