using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite
{
    public class GlobalPluginCollector
    {
        private readonly Dictionary<string, object> AllPlugins;
        private readonly Dictionary<string, string> Types;
        private static GlobalPluginCollector pcollector;

        public GlobalPluginCollector()
        {
            AllPlugins = new Dictionary<string, object>();
            Types = new Dictionary<string, string>();
        }

        public static GlobalPluginCollector GetPluginCollector()
        {
            if (pcollector == null)
            {
                pcollector = new GlobalPluginCollector();
            }
            return pcollector;
        }

        public void AddPlugin(string name, object plugin, string typename)
        {
            if (AllPlugins.ContainsKey(name))
            {
                Logger.LogError("[Fougerite AddPlugin] Tried adding a plugin to the GlobalPluginCollector, with the same name? Rename the duplicate plugin! " + name);
                return;
            }
            AllPlugins[name] = plugin;
            Types[name] = typename;
        }

        public void RemovePlugin(string name)
        {
            if (AllPlugins.Keys.Contains(name))
            {
                AllPlugins.Remove(name);
            }
            if (Types.Keys.Contains(name))
            {
                Types.Remove(name);
            }
        }

        public object GetPlugin(string name)
        {
            if (AllPlugins.Keys.Contains(name))
            {
                return AllPlugins[name];
            }
            return null;
        }

        public string GetPluginType(string name)
        {
            if (Types.Keys.Contains(name))
            {
                return Types[name];
            }
            return null;
        }
    }
}
