using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LabelRenderer : MonoBehaviour
{
    const int TEXT_SIZE = 20;

    GUIStyle fontStyle = new GUIStyle();
    List<string> labels = new List<string>();
    bool isDirty;

    public void AddLabel(string str)
    {
        if (isDirty)
        {
            isDirty = false;
            labels.Clear();
        }
        labels.Add(str);
    }

    float fps;

    private void Start()
    {
        InvokeRepeating("UpdateFPS", 0, 1.5f);
        fontStyle.fontSize = TEXT_SIZE;
        fontStyle.normal.textColor = Color.white;
    }

    private void Update()
    {
        AddLabel($"FPS: {fps}");
    }

    private void UpdateFPS()
    {
        fps = 1 / Time.deltaTime;
    }

    private void OnGUI()
    {
        for (int i = 0; i < labels.Count; i++)
        {
            GUI.Label(new Rect(0, i * TEXT_SIZE, 400, TEXT_SIZE), labels[i], fontStyle);
        }
    }

    private void LateUpdate()
    {
        isDirty = true;
    }
}
