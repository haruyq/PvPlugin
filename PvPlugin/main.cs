using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled;
using Exiled.API.Features;
using PvPluginEventHandler;
using PvPluginConfig;
using Exiled.Events;
using Config = PvPluginConfig.Config;
using Exiled.Events.EventArgs.Map;

namespace moyowachaplugin
{
    public class main : Plugin<Config>
    {
        private PvPluginEventHandler.EventHandler events;

        public override void OnEnabled()
        {
            base.OnEnabled();
            events = new PvPluginEventHandler.EventHandler();
            events.RegisterEvents();
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            events = new PvPluginEventHandler.EventHandler();
            events.UnregisterEvents();
        }
    }
}