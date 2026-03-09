using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class WebGLBuildPostProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        string outputPath = report.summary.outputPath;
        string indexPath = Path.Combine(outputPath, "index.html");

        if (!File.Exists(indexPath))
        {
            Debug.LogWarning($"[WebGLBuildPostProcessor] index.html을 찾을 수 없습니다: {indexPath}");
            return;
        }

        NetworkSetting setting = LoadNetworkSetting();
        if (setting == null)
        {
            Debug.LogWarning("[WebGLBuildPostProcessor] NetworkSetting asset을 찾을 수 없습니다. __BACKEND_URL__ 이 치환되지 않습니다.");
            return;
        }

        string html = File.ReadAllText(indexPath);
        string replaced = html.Replace("__BACKEND_URL__", setting.fastApiBaseUrl);
        File.WriteAllText(indexPath, replaced);

        Debug.Log($"[WebGLBuildPostProcessor] BACKEND_URL 주입 완료: {setting.fastApiBaseUrl}");
    }

    private static NetworkSetting LoadNetworkSetting()
    {
        string[] guids = AssetDatabase.FindAssets("t:NetworkSetting");
        if (guids.Length == 0)
            return null;

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<NetworkSetting>(path);
    }
}
