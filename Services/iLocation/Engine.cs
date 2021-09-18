using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iLocation
{
    public class Engine
    {
        #region TestOnlyLocationChanged

        internal static void RegisterMember(dynamic metadata, dynamic content)
        {
            MemberKey = content.Member.ToString();
        }

        internal static void TestOnlyLocationChanged(dynamic metadata, dynamic content)
        {
            MemberKey = metadata.GroupKey.ToString();
            MemeberMoveToNewLocation(content.Location.ToString());
        }

        internal static void MemeberMoveToNewLocation(string newLocation)
        {
            SendFeedbackMessage(type: MsgType.Info, actionTime: DateTimeOffset.Now, action: MapAction.LocationFeedback.LocationChanged.Name, groupkey: MemberKey, content: new { NewLocation = newLocation });
        }

        #endregion

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }


        #region Implement

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                ProducerHelper.SendAMessage(
                        MessageTopic.LocationFeedback,
                        new Feedback(type: type, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                        )
                .GetAwaiter().GetResult();
            }
        }

        #endregion

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            LastLocation = "";
        }

        public static string LastLocation;
        public static string MemberKey;

        public static bool MemberRegisterd => string.IsNullOrEmpty(MemberKey);
        public static string ReadLastLocation()
        {
            return LastLocation;
        }
        #endregion
    }
}
