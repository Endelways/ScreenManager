#if UNITY_EDITOR
using UnityEditor;

namespace Endelways.ScreenManager
{
    [InitializeOnLoad]
    public static class EditorPlayMode
    {
        public static bool Active { get; set; }

        static EditorPlayMode()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Active = state == PlayModeStateChange.EnteredPlayMode;
        }
    }
}
#endif