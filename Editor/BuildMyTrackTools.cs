using System.Xml;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.MyTrack.Editor
{
    [BuildTools]
    public class BuildMyTrackTools
    {

        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            if (!context.BuildTarget.Equals(BuildTarget.Android) && !context.BuildTarget.Equals(BuildTarget.iOS))
                return;

            var buildParams = context.BuildChannels.Get<BuildMyTrackParams>();
            if (!buildParams.Enable)
                return;

            context.Symbols.Add("ENABLE_MYTRACK");
            context.AddLinkAssembly("PluginSet.MyTrack");

            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.AddConfig<PluginMyTrackConfig>("MyTrack");
            if (context.BuildTarget.Equals(BuildTarget.Android))
                config.sdkKey = buildParams.AndroidSdkKey;
            else if (context.BuildTarget.Equals(BuildTarget.iOS))
                config.sdkKey = buildParams.IOSSdkKey;

            Global.CopyDependenciesInLib("com.pluginset.mytrack");
        }

        [AndroidProjectModify]
        public static void OnAndroidProjectModify(BuildProcessorContext context, AndroidProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildMyTrackParams>();
            if (!buildParams.Enable)
                return;
            
            // add receiver mytrack Install Referrer
            const string path = "/manifest/application/receiver";
            const string attrName = "name";
            string attrValue = $"com.my.tracker.campaign.MultipleInstallReceiver";

            var doc = projectManager.LibraryManifest;
            var list = doc.findElements(path, AndroidConst.NS_PREFIX, attrName, attrValue);
            XmlElement element;
            if (list.Count <= 0)
            {
                element = doc.createElementWithPath(path);
                element.SetAttribute(attrName, AndroidConst.NS_URI, attrValue);
            }
            else
            {
                element = list[0];
            }

            element.SetAttribute("permission", AndroidConst.NS_URI, "android.permission.INSTALL_PACKAGES");
            element.SetAttribute("exported"  , AndroidConst.NS_URI, "true");
            var intentFilter = element.FirstChild;
            if (intentFilter == null)
                intentFilter = element.createSubElement("intent-filter");

            var actionNode = intentFilter.FirstChild;
            if (actionNode == null)
                actionNode = intentFilter.createSubElement("action");

            ((XmlElement) actionNode).SetAttribute("name", AndroidConst.NS_URI, "com.android.vending.INSTALL_REFERRER");
            
            // add service
            const string path2 = "/manifest/application/receiver";
            string serverName = $"com.my.tracker.campaign.CampaignService";

            var list2 = doc.findElements(path2, AndroidConst.NS_PREFIX, attrName, serverName);
            if (list2.Count <= 0)
            {
                element = doc.createElementWithPath(path2);
                element.SetAttribute(attrName, AndroidConst.NS_URI, serverName);
            }
        }
    }
}