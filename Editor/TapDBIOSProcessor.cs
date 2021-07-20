using System.IO;
using TapTap.Common.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TapTap.TapDB.Editor
{
    public class TapDBIOSProcessor
    {
        [PostProcessBuild(104)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;

            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);
            var unityFrameworkTarget = TapCommonCompile.GetUnityFrameworkTarget(proj);
            if (TapCommonCompile.CheckTarget(target))
            {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }
            
            proj.AddFrameworkToProject(unityFrameworkTarget, "AdSupport.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "CoreMotion.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "Security.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "AppTrackingTransparency.framework", true);
            proj.AddFrameworkToProject(unityFrameworkTarget, "iAd.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTarget, "AdServices.framework", true);
            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}