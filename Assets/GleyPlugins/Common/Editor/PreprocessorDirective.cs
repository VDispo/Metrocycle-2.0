using UnityEditor;

namespace Gley.Common
{
    public class PreprocessorDirective
    {
        public static void AddToPlatform(string directive, bool remove, BuildTargetGroup target)
        {
#if UNITY_2023_1_OR_NEWER
            string textToWrite = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(target));
#else
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
#endif

            if (remove)
            {
                if (textToWrite.Contains(directive))
                {
                    textToWrite = textToWrite.Replace(directive, "");
                }
            }
            else
            {
                if (!textToWrite.Contains(directive))
                {
                    if (textToWrite == "")
                    {
                        textToWrite += directive;
                    }
                    else
                    {
                        textToWrite += "," + directive;
                    }
                }
            }
#if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(target), textToWrite);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, textToWrite);
#endif
        }

        public static void AddToCurrent(string directive, bool remove)
        {
            AddToPlatform(directive, remove, EditorUserBuildSettings.selectedBuildTargetGroup);
        }
    }
}
