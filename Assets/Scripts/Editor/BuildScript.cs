using UnityEditor;

public class BuildScript 
{
    [MenuItem("Custom Utilities/Build StandaloneLinux64")]
    static void PerformBuild()
    {
        string[] scenes = { "Assets/Scenes/CardsExplorer.unity", "Assets/Scenes/DeckExplored.unity", "Assets/Scenes/MainMenu.unity", "Assets/Scenes/PlayGame.unity", "Assets/Scenes/Settings.unity", "Assets/Scenes/TitleScreen.unity" };
        BuildPipeline.BuildPlayer(scesnes, "../../../build/Card-Game-Simulator.app",
            BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    [MenuItem("Custom Utilities/Build Asset Bundle StandaloneLinux64")]
    static void PerformAssetBundleBuild()
    {
        BuildPipeline.BuildAssetBundles("../AssetBundles/", BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneOSX);
    }
}