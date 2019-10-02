using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LabelRenderer : MonoBehaviour
{
    const float LOG_DELAY = 1;
    const int TEXT_SIZE = 20;

    GUIStyle fontStyle = new GUIStyle();
    List<string> labels = new List<string>();
    bool isDirty;

    List<DelayedLabel> delayedLabelQueue = new List<DelayedLabel>();
    List<string> delayedLabels = new List<string>();

    struct DelayedLabel
    {
        public string conent;
        public float time;
    }

    public void AddLabel(string str)
    {
        if (isDirty)
        {
            isDirty = false;
            labels.Clear();
        }
        labels.Add(str);
    }

    public void AddTimedLabel(string str, float delay)
    {
        delayedLabelQueue.Add(new DelayedLabel{conent = str, time = delay});
    }

    float fps;

    private void Start()
    {
        InvokeRepeating("UpdateFPS", 0, 1.5f);
        fontStyle.fontSize = TEXT_SIZE;
        fontStyle.normal.textColor = Color.white;

        Debug.unityLogger.logHandler = new LogHandler(Debug.unityLogger.logHandler, this);
        // AddTimedLabel("timed label", 5);
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
        for (int i = 0; i < delayedLabels.Count; i++)
        {
            GUI.Label(new Rect(0, i * TEXT_SIZE + labels.Count * TEXT_SIZE, 400, TEXT_SIZE), delayedLabels[i], fontStyle);
        }
    }

    private void LateUpdate()
    {
        isDirty = true;
        foreach (DelayedLabel l in delayedLabelQueue)
        {
            delayedLabels.Add(l.conent);
            StartCoroutine(StartDelayRemove(l.conent, l.time));
        }
        delayedLabelQueue.Clear();
    }

    private IEnumerator StartDelayRemove(string label, float time)
    {
        yield return new WaitForSeconds(time);
        delayedLabels.Remove(label);
    }

    private class LogHandler : ILogHandler
    {
        private LabelRenderer labelRenderer;
        private ILogHandler original;

        public LogHandler(ILogHandler original, LabelRenderer renderer)
        {
            labelRenderer = renderer;
            this.original = original;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            labelRenderer.AddTimedLabel(exception.ToString(), LOG_DELAY * 3);
            original.LogException(exception, context);
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (logType == LogType.Log)
            {
                labelRenderer.AddTimedLabel(string.Format(format, args), LOG_DELAY);
            }
            original.LogFormat(logType, context, format, args);
        }
    }
}
