
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

public class BuildScript : MonoBehaviour
{
    [MenuItem("Build/Build OSX")]
    public static void PreformBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/CardsExplorer.unity", "Assets/Scenes/DeckEditor.unity", "Assets/Scenes/MainMenu.unity", 
            "Assets/Scenes/PlayGame.unity", "Assets/Scenes/Settings.unity", "Assets/Scenes/TitleScreen.unity" };
        buildPlayerOptions.locationPathName = "OSXbuild";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}