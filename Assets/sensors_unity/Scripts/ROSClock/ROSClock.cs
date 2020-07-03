using System;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ROSClock : UnityPublisher<MessageTypes.Rosgraph.Clock>
    {
        private MessageTypes.Rosgraph.Clock message;

        protected override void Start()
        {
            base.Start();
            message = new MessageTypes.Rosgraph.Clock();
        }

        private void FixedUpdate()
        {
            float sim_time = Time.time;
            uint secs = (uint)sim_time;
            uint nsecs = (uint)((sim_time % 1) * 1e6);
            message.clock.secs = secs;
            message.clock.nsecs = nsecs;
            Publish(message);
        }
    }
}
