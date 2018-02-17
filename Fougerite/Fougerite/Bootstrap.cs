
namespace Fougerite
{
    using System;
    using System.IO;
    using UnityEngine;
    using System.Threading;

    public class Bootstrap : Facepunch.MonoBehaviour
    {
        public const string Version = "1.6.2";
        public static bool CR = false;
        public static bool BI = false;
        public static bool AutoBanCraft = true;
        public static bool EnableDefaultRustDecay = true;
        internal static readonly Thread CurrentThread = Thread.CurrentThread;

        public static void AttachBootstrap()
        {
            try
            {
                Bootstrap bootstrap = new Bootstrap();
                new GameObject(bootstrap.GetType().FullName).AddComponent(bootstrap.GetType());
                Debug.Log(string.Format("<><[ Fougerite v{0} ]><>", Version));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.Log("Error while loading Fougerite!");
            }
        }

        public void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        }

        public bool ApplyOptions()
        {
            // look for the string 'false' to disable.  **not a bool check**
            if (Fougerite.Config.GetValue("Fougerite", "enabled") == "false") 
            {
                Debug.Log("Fougerite is disabled. No modules loaded. No hooks called.");
                return false;
            }
            if (Fougerite.Config.GetValue("Fougerite", "RemovePlayersFromCache") != null)
            {
                CR = Fougerite.Config.GetBoolValue("Fougerite", "RemovePlayersFromCache");
            }
            if (Fougerite.Config.GetValue("Fougerite", "BanOnInvalidPacket") != null)
            {
                BI = Fougerite.Config.GetBoolValue("Fougerite", "BanOnInvalidPacket");
            }
            if (Fougerite.Config.GetValue("Fougerite", "AutoBanCraft") != null)
            {
                AutoBanCraft = Fougerite.Config.GetBoolValue("Fougerite", "AutoBanCraft");
            }
            if (!Fougerite.Config.GetBoolValue("Fougerite", "deployabledecay") && !Fougerite.Config.GetBoolValue("Fougerite", "decay"))
            {
                decay.decaytickrate = float.MaxValue / 2;
                decay.deploy_maxhealth_sec = float.MaxValue;
                decay.maxperframe = -1;
                decay.maxtestperframe = -1;
            }
            if (!Fougerite.Config.GetBoolValue("Fougerite", "structuredecay") && !Fougerite.Config.GetBoolValue("Fougerite", "decay"))
            {
                structure.maxframeattempt = -1;
                structure.framelimit = -1;
                structure.minpercentdmg = float.MaxValue;
            }
            if (Fougerite.Config.GetValue("Fougerite", "EnableDefaultRustDecay") != null)
            {
                EnableDefaultRustDecay = Fougerite.Config.GetBoolValue("Fougerite", "EnableDefaultRustDecay");
            }
            else
            {
                NetCull.Callbacks.beforeEveryUpdate += new NetCull.UpdateFunctor(EnvDecay.Callbacks.RunDecayThink);
                Logger.LogWarning("[RustDecay] The default Rust Decay is enabled. (Config option not found)");
            }
            if (EnableDefaultRustDecay)
            {
                NetCull.Callbacks.beforeEveryUpdate += new NetCull.UpdateFunctor(EnvDecay.Callbacks.RunDecayThink);
                Logger.LogWarning("[RustDecay] The default Rust Decay is enabled.");
            }
            else
            {
                Logger.LogWarning("[RustDecay] The default Rust Decay is disabled.");
            }
            return true;
        }

        public void Start()
        {
            string FougeriteDirectoryConfig = Path.Combine(Util.GetServerFolder(), "FougeriteDirectory.cfg");
            Config.Init(FougeriteDirectoryConfig);
            Logger.Init();

            Rust.Steam.Server.SetModded();
            Rust.Steam.Server.Official = false;

            if (ApplyOptions()) {
                ModuleManager.LoadModules();
                Fougerite.Hooks.ServerStarted();
                Fougerite.ShutdownCatcher.Hook();
            }
            SQLiteConnector.GetInstance.Setup();
        }
    }
}