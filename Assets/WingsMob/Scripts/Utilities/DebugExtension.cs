using System.Text;
using UnityEngine;

namespace WingsMob.Survival.Utils
{
    public static class DebugExtension
    {
        private static StringBuilder sb = new StringBuilder();

        public static void Log(object message, Color color = default)
        {
            Common.Log(sb.Clear()
                        .Append("<color=#")
                        .Append(ColorUtility.ToHtmlStringRGB(color))
                        .Append(">")
                        .Append(message)
                        .Append("</color>"));
        }

        public static void LogWarning(object message, Color color = default)
        {
            Common.LogWarning(sb.Clear()
                        .Append("<color=#")
                        .Append(ColorUtility.ToHtmlStringRGB(color))
                        .Append(">")
                        .Append(message)
                        .Append("</color>"));
        }
    }
}