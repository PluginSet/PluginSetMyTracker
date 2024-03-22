using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.MyTrack.Editor
{
    [BuildChannelsParams("MyTrack", "MyTrack SDK参数")]
    public class BuildMyTrackParams: ScriptableObject
    {
        [Tooltip("是否启用SDK")]
        public bool Enable;

        [Tooltip("安卓平台SDKKey")]
        public string AndroidSdkKey;
        
        [Tooltip("iOS平台SDKKey")]
        public string IOSSdkKey;
    }
}