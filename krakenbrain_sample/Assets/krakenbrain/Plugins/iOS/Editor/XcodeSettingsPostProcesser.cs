using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using KrakenBrain.Editor;

public class XcodeSettingsPostProcesser
{
    [PostProcessBuildAttribute(0)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        KrakenBrainSettings instance = KrakenBrainSettings.LoadInstance();
        string sdWeight = instance.SDWeightFileName;        //Ex] CoreMLModels

        // it only works for iOS Platform
        if (buildTarget != BuildTarget.iOS)
            return;

        // Initialize PbxProject
        var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        // Add Framework to App Target
        string targetGuid = pbxProject.GetUnityMainTargetGuid();
        string unityFrameworkGuid = pbxProject.GetUnityFrameworkTargetGuid();

        // Add Framework to UnityFramework Target
        //string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();

        // StableDiffusionFramework Setting
        CopyAndReplaceDirectory(UnityEngine.Application.dataPath + "/krakenbrain/Plugins/iOS/StableDiffusionFramework.framework", Path.Combine(pathToBuiltProject, "Frameworks/StableDiffusionFramework.framework"));
        string fileGuid = pbxProject.AddFile("Frameworks/StableDiffusionFramework.framework", "Frameworks/StableDiffusionFramework.framework", PBXSourceTree.Source);
        pbxProject.AddFileToBuild(targetGuid, fileGuid);
        pbxProject.SetBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
        pbxProject.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/**");
        UnityEditor.iOS.Xcode.Extensions.PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, targetGuid, fileGuid);

        //pbxProject.RemoveFileFromBuild(targetGuid, fileGuid);
        //pbxProject.AddFileToBuild(unityFrameworkGuid, fileGuid);

        // DeepLearning Model Weight Files Setting
        CopyAndReplaceDirectory(UnityEngine.Application.dataPath + "/krakenbrain/Plugins/iOS/" + sdWeight, Path.Combine(pathToBuiltProject, "Libraries/krakenbrain/Plugins/" + sdWeight));
        string modelWeightGuid = pbxProject.AddFile("Libraries/krakenbrain/Plugins/" + sdWeight, "Libraries/krakenbrain/Plugins/" + sdWeight, PBXSourceTree.Source);
        pbxProject.AddFileToBuild(targetGuid, modelWeightGuid);

        // Set ENABLE_BITCODE Settings
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "false");
        pbxProject.SetBuildProperty(unityFrameworkGuid, "ENABLE_BITCODE", "false");

        // Remove Framework from UnityFramework target
        string unityFwTarget = pbxProject.TargetGuidByName("UnityFramework");
        string oldFileGuid = pbxProject.FindFileGuidByProjectPath("Frameworks/krakenbrain/Plugins/iOS/StableDiffusionFramework.framework");
        pbxProject.RemoveFileFromBuild(unityFwTarget, oldFileGuid);
        pbxProject.RemoveFile(oldFileGuid);

        // Reference
        // How to add frameworks from pods : https://stackoverflow.com/questions/75211075/unity-ios-development-how-to-add-frameworks-from-pods-via-post-build-script
        // Add Frameworks
        //pbxProject.AddFrameworkToProject(targetGuid, "", true);

        // Apply Settings
        File.WriteAllText(projectPath, pbxProject.WriteToString());
    }

    internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        // If already exists destination File or Directory then Remove them
        if (Directory.Exists(dstPath))
            Directory.Delete(dstPath);
        if (File.Exists(dstPath))
            File.Delete(dstPath);

        Directory.CreateDirectory(dstPath);

        // If SourcePath is the File Copy that Files
        foreach (var file in Directory.GetFiles(srcPath))
            File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
        

        // If SourcePath is the Directory Copy Files in that Directory
        foreach (var dir in Directory.GetDirectories(srcPath))
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
    }
}
