#if ENABLE_MYTRACK
using System;
using System.Collections.Generic;
using Mycom.Tracker.Unity;
using PluginSet.Core;

namespace PluginSet.MyTrack
{
    [PluginRegister]
    public class PluginMyTrack: PluginBase, IPrivacyAuthorizationCallback, IAnalytics, IUserSet, ICustomPlugin
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("MyTrack");
        
        public override string Name => "MyTrack";

        private string _sdkKey;

        private string _deeplink;
        private bool _deeplinkCallback = false;

        protected override void Init(PluginSetConfig config)
        {
            var cfg = config.Get<PluginMyTrackConfig>();
            _sdkKey = cfg.sdkKey;
        }


        public void OnPrivacyAuthorization()
        {
            Mycom.Tracker.Unity.MyTracker.SetAttributionListener(OnMyTrackDeeplinkAttribution);
#if DEBUG
            Mycom.Tracker.Unity.MyTracker.IsDebugMode = true;
#else
            Mycom.Tracker.Unity.MyTracker.IsDebugMode = false;
#endif
            Mycom.Tracker.Unity.MyTracker.Init(_sdkKey);
        }

        private void OnMyTrackDeeplinkAttribution(MyTrackerAttribution obj)
        {
            _deeplink = obj.Deeplink;
            _deeplinkCallback = true;
            Logger.Debug($"OnMyTrackDeeplinkAttribution ::: deeplink = {_deeplink}");
            
        }

        private Dictionary<string, string> ConvertEventData(Dictionary<string, object> eventData = null)
        {
            if (eventData == null)
                return null;
            
            var result = new Dictionary<string, string>();
            foreach (var kv in eventData)
            {
                result.Add(kv.Key, kv.Value.ToString());
            }

            return result;
        }

        public void CustomEvent(string customEventName, Dictionary<string, object> eventData = null)
        {
            Mycom.Tracker.Unity.MyTracker.TrackEvent(customEventName, ConvertEventData(eventData));
        }

        public void SetUserInfo(bool isNewUser, string userId, Dictionary<string, object> pairs = null)
        {
            Mycom.Tracker.Unity.MyTracker.MyTrackerParams.CustomUserId = userId;
            // if (isNewUser)
            // {
            //     Mycom.Tracker.Unity.MyTracker.TrackRegistrationEvent(userId);
            // }
            // Mycom.Tracker.Unity.MyTracker.TrackLoginEvent(userId);
        }

        public void ClearUserInfo()
        {
            Mycom.Tracker.Unity.MyTracker.MyTrackerParams.CustomUserId = string.Empty;
        }

        public void FlushUserInfo()
        {
            Mycom.Tracker.Unity.MyTracker.Flush();
        }
        
        public void CustomCall(string func, Action<Result> callback = null, string json = null)
        {
            if (func == "GetDeepLink")
            {
                if (_deeplinkCallback)
                {
                    callback?.Invoke(new Result()
                    {
                        Success = true,
                        PluginName = Name,
                        Code = PluginConstants.SuccessCode,
                        Data = _deeplink
                    });
                }
                else
                {
                    callback?.Invoke(new Result()
                    {
                        Success = false,
                        PluginName = Name,
                        Error = "Not callback yet",
                        Code = PluginConstants.FailDefaultCode
                    });
                }
            }
            else
            {
                callback?.Invoke(new Result()
                {
                    Success = false,
                    PluginName = Name,
                    Error = $"Invalid func: {func}",
                    Code = PluginConstants.InvalidCode
                });
            }
        }
    }
}
#endif