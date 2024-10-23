using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Test : MonoBehaviour{

    [SerializeField] TMP_Text logText;

    float deltaTime = 0.0f;
    float updateInterval = 1.0f; // 每秒更新一次
    float frames = 0;
    float totalDeltaTime = 0;

    public static Test Instance;

    private bool showFPS = true;

    public void ShowErrorOrText(string s) {
        showFPS = false;
        logText.text = s;
    }
    private void Start() {
        Instance = this;

        Application.logMessageReceived += HandleLog;
    }
    private void OnDestroy() {
        Application.logMessageReceived -= HandleLog;
    }
    void HandleLog(string logString, string stackTrace, LogType type) {
        logText.text = logString + " / " + stackTrace;
    }

    private void Update() {
        if (!showFPS) return;

        frames++;
        totalDeltaTime += Time.deltaTime;

        deltaTime += Time.unscaledDeltaTime;
        if (deltaTime >= updateInterval) {
            float msec = deltaTime * 1000.0f;
            float fps = frames / totalDeltaTime;

            logText.text = fps.ToString("0.0") + "fps";

            deltaTime = 0.0f;
            frames = 0;
            totalDeltaTime = 0;
        }
    }
}
