using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.MyTrack.Editor
{
    [InitializeOnLoad]
    public static class PluginsMyTrackFilter
    {
        static PluginsMyTrackFilter()
        {
            var fileter = PluginFilter.IsBuildParamsEnable<BuildMyTrackParams>();
            PluginFilter.RegisterFilter("com.pluginset.mytrack/Plugins/iOS", fileter);
            PluginFilter.RegisterFilter("com.pluginset.mytrack/Plugins/Android", fileter);
        }
    }
}