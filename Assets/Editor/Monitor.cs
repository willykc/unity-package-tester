using System;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

[InitializeOnLoad]
public static class Monitor
{
    static Monitor()
    {
        SetupListeners();
    }

    public static void SetupListeners()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        api.RegisterCallbacks(new MyCallbacks(api));
    }

    private class MyCallbacks : ICallbacks
    {
        private readonly TestRunnerApi api;

        private bool detected;

        public MyCallbacks(TestRunnerApi api)
        {
            this.api = api;
        }

        public void RunStarted(ITestAdaptor testsToRun)
        {
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            api.UnregisterCallbacks(this);
            Debug.Log(string.Format("############## Run finished {0} test(s) failed.", result.FailCount));
            Application.logMessageReceived += OnLogMessageReceived;
            ForceRecompile();
            Application.logMessageReceived -= OnLogMessageReceived;
            if (detected)
            {
                Debug.Log("errors or warnings detected");
                EditorApplication.Exit(1);
            }
            Debug.Log("nothing detected, continue");
        }

        public void TestStarted(ITestAdaptor test)
        {
        }

        public void TestFinished(ITestResultAdaptor result)
        {
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Log)
            {
                detected = true;
            }
        }

        private static void ForceRecompile()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var guid = Guid.NewGuid().ToString("N").ToUpperInvariant();
            var newSymbols = $"{symbols};S{guid}";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, newSymbols);
        }
    }
}
