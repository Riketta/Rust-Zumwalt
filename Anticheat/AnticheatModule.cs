﻿using System.IO;
using Google.ProtocolBuffers.Collections;
using Rust;
using Fougerite;
using Fougerite.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Timers;
using UnityEngine;
using Module = Fougerite.Module;
using Player = Fougerite.Player;

namespace Anticheat
{
    public class AnticheatModule : Module
    {
        public override string Name
        {
            get { return "Anticheat"; }
        }

        public override string Author
        {
            get { return "Riketta, Skippi, DreTaX"; }
        }

        public override string Description
        {
            get { return "Base Fougerite Anticheat"; }
        }

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override uint Order
        {
            get { return 1; }
        }

        #region Config Fields

        private IniParser INIConfig;
        private bool AntiSpeedHack_Enabled = false;
        private int AntiSpeedHack_Timer = 0;
        private bool AntiSpeedHack_SayChat = false;
        private bool AntiSpeedHack_Kick = false;
        private bool AntiSpeedHack_Ban = false;
        private bool AntiSpeedHack_AllowTP = false;
        private int AntiSpeedHack_SayChatDist = 0;
        private int AntiSpeedHack_KickDist = 0;
        private int AntiSpeedHack_BanDist = 0;
        private int AntiSpeedHack_TPDist = 0;
        private bool AntiSpeedHack_AdminCheck = false;
        private int AntiSpeedHack_WarnLimit = 3;

        private bool AntiAIM_Enabled = false;
        private bool AntiAIM_HeadshotsOnly = false;
        private int AntiAIM_CountAIM = 0;
        private int AntiAIM_MaxDist = 0;
        private int AntiAIM_MaxKillDist = 0;

//        private bool AntiFlyHack_Enabled = false;
//        private int AntiFlyHack_MaxHeight = 0;
//        private int AntiFlyHack_TimeFlyCheck = 0;
//        private int AntiFlyHack_CountFly = 0;

        private bool NamesRestrict_Enabled = false;
        private string NamesRestrict_AllowedChars = "";
        private int NamesRestrict_MaxLength = 0;
        private int NamesRestrict_MinLength = 0;
        private string NamesRestrict_BannedNames = "";
        private string[] BannedNames;
        private bool NamesRestrict_BindName = false;
        private bool NamesRestrict_AdminsOnly = false;

        private bool HighPingKicking_Enabled = false;
        private int HighPingKicking_Timer = 0;
        private int HighPingKicking_MaxPing = 0;

        private bool RelogCooldown = false;
        private int Cooldown = 0;

        private bool GodModDetect = false;

        private bool LogPlayers = false;

        #endregion

        private string EchoBotName = "[AntiCheat]";
        private string ConsolePrefix = "[AC]";
        private DataStore DS = null;

        #region Init\DeInit

        public override void Initialize()
        {

            Logger.LogDebug(ConsolePrefix + " Loading...");
            string ConfigFile = Path.Combine(ModuleFolder, "Anticheat.cfg");
            if(File.Exists(ConfigFile))
                INIConfig = new IniParser(ConfigFile);
            else
            {
                Logger.LogError("Anticheat.cfg does not exist. Can't load module.");
                return;
            }
            DS = DataStore.GetInstance();
            DS.Flush("loginCooldown");
            ConfigInit();
            TimersInit();

            Hooks.OnEntityDecay += EntityDecay;
            Hooks.OnDoorUse += DoorUse;
            Hooks.OnEntityHurt += EntityHurt;
            Hooks.OnPlayerConnected += PlayerConnect;
            Hooks.OnPlayerDisconnected += PlayerDisconnect;
            Hooks.OnPlayerHurt += PlayerHurt;
            Hooks.OnPlayerSpawned += PlayerSpawned;
            if (AntiAIM_Enabled)
                Hooks.OnPlayerKilled += PlayerKilled;
            Hooks.OnServerShutdown += ServerShutdown;
            Hooks.OnShowTalker += ShowTalker;
            Hooks.OnChat += Chat;
            Hooks.OnChatRaw += new Hooks.ChatRawHandlerDelegate(ChatReceived);
        }

        public override void DeInitialize()
        {
            DS.Flush("loginCooldown");
            DS.Flush("AntiSpeedHack");
            DS.Flush("lastCoords");

            pingTimer.Elapsed -= pingEvent;
            pingTimer.Stop();
            takeCoordsTimer.Elapsed -= takeCoordsEvent;
            takeCoordsTimer.Stop();
                
            Hooks.OnEntityDecay -= EntityDecay;
            Hooks.OnDoorUse -= DoorUse;
            Hooks.OnEntityHurt -= EntityHurt;
            Hooks.OnPlayerConnected -= PlayerConnect;
            Hooks.OnPlayerDisconnected -= PlayerDisconnect;
            Hooks.OnPlayerHurt -= PlayerHurt;
            Hooks.OnPlayerSpawned -= PlayerSpawned;
            if (AntiAIM_Enabled)
                Hooks.OnPlayerKilled -= PlayerKilled;
            Hooks.OnServerShutdown -= ServerShutdown;
            Hooks.OnShowTalker -= ShowTalker;
            Hooks.OnChat -= Chat;
            Hooks.OnChatRaw -= new Hooks.ChatRawHandlerDelegate(ChatReceived);
        }

        #endregion



        private int GetIntSetting(string Section, string Name)
        {
            string Value = INIConfig.GetSetting(Section, Name);
            int INT = 0;
            if (int.TryParse(Value, out INT))
                return INT;
            return int.MinValue;
        }

        private bool GetBoolSetting(string Section, string Name)
        {
            return INIConfig.GetSetting(Section, Name).ToLower() == "true";
        }

        private void Log(string Msg)
        {
            Logger.Log(ConsolePrefix + " " + Msg);
        }



        private Timer pingTimer;
        private Timer takeCoordsTimer;

        private void TimersInit()
        {
            if (HighPingKicking_Enabled)
            {
                pingTimer = new Timer(HighPingKicking_Timer * 1000);
                pingTimer.Elapsed += pingEvent;
                pingTimer.Start();
                Logger.LogDebug(ConsolePrefix + " pingTimer started - interval " + HighPingKicking_Timer);
            }
            else Logger.LogDebug(ConsolePrefix + " HighPingKicking disabled");

            if (AntiSpeedHack_Enabled)
            {
                takeCoordsTimer = new Timer(AntiSpeedHack_Timer * 1000);
                takeCoordsTimer.Elapsed += takeCoordsEvent;
                takeCoordsTimer.Start();
                Logger.LogDebug(ConsolePrefix + " takeCoordsTimer started - interval " + AntiSpeedHack_Timer);
            }
            else Logger.LogDebug(ConsolePrefix + " AntiSpeedHack disabled");
        }

        private void pingEvent(object x, ElapsedEventArgs y)
        {
            foreach (var pl in Server.GetServer().Players)
                if (pl != null)
                    PlayerPingCheck(pl);
        }

        private void takeCoordsEvent(object x, ElapsedEventArgs y)
        {
            try
            {
                foreach (var pl in Server.GetServer().Players)
                {
                    try
                    {
                        if (pl == null || pl.PlayerClient.netPlayer == null || !pl.PlayerClient.netPlayer.isConnected)
                        {
                            Log(ConsolePrefix + " NotConnected: " + pl.Name);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        return;
                    }

                    if (!AntiSpeedHack_AdminCheck && pl.Admin)
                        continue;
                    // Not Y
                    Vector2 CurrentPlayerCoords = new Vector2(pl.Location.x, pl.Location.z);

                    object Coords = DS.Get("lastCoords", pl.SteamID.ToString());
                    DS.Add("lastCoords", pl.SteamID.ToString(), pl.Location);
                    if (Coords == null)
                        return;
                    Vector3 LastVector3Location = (Vector3) Coords;
                    Vector2 LastLocation = new Vector2(LastVector3Location.x, LastVector3Location.z);

                    if (LastLocation != Vector2.zero && LastLocation != CurrentPlayerCoords)
                    {
                        float distance = Math.Abs(Vector2.Distance(LastLocation, CurrentPlayerCoords));
                        Logger.LogDebug(ConsolePrefix + " " + pl.Name + " speed is " + distance.ToString());

                        int Warned = (int)DS.Get("AntiSpeedHack", pl.SteamID.ToString());

                        // If no alert - reduce warning level
                        if (distance < AntiSpeedHack_SayChatDist * AntiSpeedHack_Timer)
                            DS.Add("AntiSpeedHack", pl.SteamID.ToString(), (Warned > 0 ? Warned - 1 : 0));
                        else if (Warned == AntiSpeedHack_WarnLimit // Time to ban
                            && !(distance > AntiSpeedHack_TPDist * AntiSpeedHack_Timer && AntiSpeedHack_AllowTP)) // Not allow to TP
                        {
                            if (distance > AntiSpeedHack_BanDist * AntiSpeedHack_Timer && AntiSpeedHack_Ban)
                            {
                                Server.GetServer().BroadcastFrom(EchoBotName,
                                    "[color#FF6666]" + pl.Name + " was banned (Moved " + distance.ToString("F2") + "m)");
                                BanCheater(pl, "Moved " + distance.ToString("F2") + "m");
                            }
                            else if (distance > AntiSpeedHack_KickDist * AntiSpeedHack_Timer && AntiSpeedHack_Kick)
                            {
                                Server.GetServer().BroadcastFrom(EchoBotName,
                                    "[color#FF6666]" + pl.Name + " was kicked (Moved " + distance.ToString("F2") + "m, maybe lag)");
                                pl.MessageFrom(EchoBotName, "[color#FF2222]You have been kicked!");
                                Log("Kick: " + pl.Name + ". SpeedHack - may be lag (" + pl.Ping + ")");
                                pl.Disconnect();
                            }
                            else if (distance > AntiSpeedHack_SayChatDist * AntiSpeedHack_Timer && AntiSpeedHack_SayChat)
                                Server.GetServer().BroadcastFrom(EchoBotName,
                                    "[color#FF6666]" + pl.Name + " moved " + distance.ToString("F2") + "m!");
                        }
                        // If alert, but not time to ban
                        if (Warned < AntiSpeedHack_WarnLimit && distance > AntiSpeedHack_KickDist * AntiSpeedHack_Timer)
                        {
                            // Cause TP back may be seems as cheat
                            DS.Add("lastCoords", pl.SteamID.ToString(), LastVector3Location);
                            pl.TeleportTo(LastVector3Location);

                            int WarnLevel = Warned + 1;
                            DS.Add("AntiSpeedHack", pl.SteamID.ToString(), WarnLevel);
                            Log("Warn: " + pl.Name + ". Moved " + distance.ToString("F2") + ". Count: " + WarnLevel + ". Ping: " + pl.Ping);
                            pl.MessageFrom(EchoBotName, "WarnLevel: " + WarnLevel + "! Moved " + distance.ToString("F2"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }



        private void BanCheater(Fougerite.Player player, string StringLog)
        {
            try
            {
                IniParser iniBansIP;
                string ConfigFile = Path.Combine(ModuleFolder, "BansIP.ini");
                if (File.Exists(ConfigFile))
                    iniBansIP = new IniParser(ConfigFile);
                else
                {
                    Logger.LogError("BansIP.ini does not exist!");
                    return;
                }

                IniParser iniBansID;
                ConfigFile = Path.Combine(ModuleFolder,  "BansID.ini");
                if (File.Exists(ConfigFile))
                    iniBansID = new IniParser(ConfigFile);
                else
                {
                    Logger.LogError("BansID.ini does not exist!");
                    return;
                }

                string Date = DateTime.Now.ToShortDateString();
                string Time = DateTime.Now.ToShortTimeString();
                
                string BanMessage = 
                    "Nickname: " + player.Name + ", Date: " + Date + ", Time: " + Time + 
                        ", Reason: " + StringLog + ", Ping: " + player.Ping;

                iniBansIP.AddSetting("Ips", player.IP, BanMessage);
                iniBansID.AddSetting("Ids", player.SteamID, BanMessage);

                iniBansIP.Save();
                iniBansID.Save();
                player.MessageFrom(EchoBotName, "[color#FF2222]You have been banned.");
                Log("BAN: " + player.Name + " " + ". " + StringLog + ". Ping: " + player.Ping);
                player.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }



        private void ConfigInit()
        {
            try
            {
                AntiSpeedHack_Enabled = GetBoolSetting("AntiSpeedHack", "Enable");
                AntiSpeedHack_Timer = GetIntSetting("AntiSpeedHack", "Timer");
                AntiSpeedHack_SayChat = GetBoolSetting("AntiSpeedHack", "Chat");
                AntiSpeedHack_KickDist = GetIntSetting("AntiSpeedHack", "KickDist");
                AntiSpeedHack_Ban = GetBoolSetting("AntiSpeedHack", "Ban");
                AntiSpeedHack_AllowTP = GetBoolSetting("AntiSpeedHack", "Teleport");
                AntiSpeedHack_SayChatDist = GetIntSetting("AntiSpeedHack", "ChatDistance");
                AntiSpeedHack_KickDist = GetIntSetting("AntiSpeedHack", "KickDistance");
                AntiSpeedHack_BanDist = GetIntSetting("AntiSpeedHack", "BanDistance");
                AntiSpeedHack_TPDist = GetIntSetting("AntiSpeedHack", "TeleportDistance");
                AntiSpeedHack_AdminCheck = GetBoolSetting("AntiSpeedHack", "AdminCheck");
                AntiSpeedHack_WarnLimit = GetIntSetting("AntiSpeedHack", "WarnLimit");

                AntiAIM_Enabled = GetBoolSetting("AntiAIM", "Enable");
                AntiAIM_CountAIM = GetIntSetting("AntiAIM", "ShotsCount");
                AntiAIM_HeadshotsOnly = GetBoolSetting("AntiAIM", "HeadshotsOnly");
                AntiAIM_MaxDist = GetIntSetting("AntiAIM", "ShotMaxDistance");
                AntiAIM_MaxKillDist = GetIntSetting("AntiAIM", "MaxKillDistance");

                NamesRestrict_Enabled = GetBoolSetting("Names", "Enable");
                NamesRestrict_AllowedChars = INIConfig.GetSetting("Names", "AllowedChars");
                NamesRestrict_MaxLength = GetIntSetting("Names", "MaxLength");
                NamesRestrict_MinLength = GetIntSetting("Names", "MinLength");
                NamesRestrict_BannedNames = INIConfig.GetSetting("Names", "BannedNames");
                NamesRestrict_BannedNames = NamesRestrict_BannedNames.Replace(", ", "|");
                BannedNames = NamesRestrict_BannedNames.Split('|');
                NamesRestrict_BindName = GetBoolSetting("Names", "BindNameToSteamID");
                NamesRestrict_AdminsOnly = GetBoolSetting("Names", "BindOnlyAdmins");

                RelogCooldown = GetBoolSetting("RelogCooldown", "Enable");
                Cooldown = GetIntSetting("RelogCooldown", "Cooldown");

                HighPingKicking_Enabled = GetBoolSetting("AntiLagger", "Enable");
                HighPingKicking_Timer = GetIntSetting("AntiLagger", "SecondsToCheck");
                HighPingKicking_MaxPing = GetIntSetting("AntiLagger", "MaxPing");

                GodModDetect = GetBoolSetting("GodModDetect", "Enable");

                LogPlayers = GetBoolSetting("LogPlayers", "Enable");

                Logger.LogDebug(ConsolePrefix + " Config inited!");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void PlayerPingCheck(Fougerite.Player player)
        {
            try
            {
                if (player.Ping < HighPingKicking_MaxPing)
                {
                    DS.Add("ping", player.SteamID.ToString(), 0);
                    return;
                }

                int Warned = (int)DS.Get("ping", player.SteamID.ToString());
                if (Warned == 0)
                {
                    player.MessageFrom(EchoBotName,
                        "[color#FF2222]Fix your ping (" + player.Ping + ") or you will be kicked!");
                    DS.Add("ping", player.SteamID.ToString(), 1);
                }
                else if (Warned == 1)
                {
                    player.MessageFrom(EchoBotName,
                        "[color#FF2222]Your ping is " + player.Ping + " but maximum allowed is " +
                        HighPingKicking_MaxPing);
                    Log("Kick: " + player.Name + ". Lagger");
                    player.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        //

        private void ChatReceived(ref ConsoleSystem.Arg arg)
        {
        }

        private void Chat(Fougerite.Player p, ref ChatString text)
        {
        }

        private void ShowTalker(uLink.NetworkPlayer player, PlayerClient p)
        {
        }

        private void ServerShutdown()
        {
        }

        private void PlayerKilled(DeathEvent deathEvent)
        {
            try
            {
                if (!(deathEvent.Attacker is Fougerite.Player))
                    return;

                Fougerite.Player player = (Fougerite.Player) deathEvent.Attacker;
                Fougerite.Player victim = (Fougerite.Player) deathEvent.Victim;

                string weapon = deathEvent.WeaponName;
                if ((deathEvent.DamageType == "Bullet"
                     &&
                     (weapon == "HandCannon" && weapon == "Pipe Shotgun" && weapon == "Revolver" &&
                      weapon == "9mm Pistol" &&
                      weapon == "P250" && weapon == "Shotgun" && weapon == "Bolt Action Rifle" && weapon == "M4" &&
                      weapon == "MP5A4"))
                    || (deathEvent.DamageType == "Melee" && (int) (Math.Round(deathEvent.DamageAmount)) == 75
                        && string.IsNullOrEmpty(weapon)))
                {
                    Vector3 attacker_location = player.Location;
                    Vector3 victim_location = ((Fougerite.Player) deathEvent.Victim).Location;
                    float distance =
                        (float) Math.Round(Util.GetUtil().GetVectorsDistance(attacker_location, victim_location));
                    if (distance > RangeOf(weapon) && RangeOf(weapon) > 0)
                    {
                        player.Kill();
                        BanCheater(player, "AutoAIM. Gun: " + weapon + " Dist: " + distance);
                        Server.GetServer()
                            .BroadcastFrom(EchoBotName,
                                player.Name + " shooted " + victim.Name + " from " + distance + "m.");
                        Log("AutoAIM: " + player.Name + ". Gun: " + weapon + " Dist: " + distance);
                        player.Disconnect();
                        victim.TeleportTo(attacker_location.x, attacker_location.y, attacker_location.z);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private int RangeOf(string weapon)
        {
            int range = GetIntSetting("Range", weapon);
            return range;
        }

        private void EntityDecay(DecayEvent de)
        {
        }

        private void DoorUse(Fougerite.Player p, DoorEvent de)
        {
        }

        private void PlayerHurt(HurtEvent he)
        {
            try
            {
                if (GodModDetect)
                {
                    var Damage = Math.Round(he.DamageAmount);
                    Fougerite.Player Victim = (Fougerite.Player) he.Victim;
                    if ((!Victim.Admin) && (Damage == 0))
                    {
                        Log("GOD: " + Victim.Name + ".  received 0 damage. Check him for GodMode!");
                        foreach (var player in Server.GetServer().Players)
                            if (player.Admin)
                                player.MessageFrom(EchoBotName,
                                    "[color#FFA500]" + Victim.Name + " received 0 damage. Check him for GodMode!");

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ConsolePrefix + " GodDetect crash");
                Logger.LogException(ex);
            }
        }

        private void PlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            DS.Add("lastCoords", player.SteamID.ToString(), player.Location);
            DS.Add("AntiSpeedHack", player.SteamID.ToString(), 0);
        }

        private void PlayerDisconnect(Fougerite.Player player)
        {
            try
            {
                if (RelogCooldown)
                    if (!player.Admin)
                    {
                        var Time = System.Environment.TickCount;
                        if (Cooldown == 0)
                            DS.Add("loginCooldown", player.SteamID.ToString(), Time);
                    }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void PlayerConnect(Fougerite.Player player)
        {
            try
            {
                try
                {
                    if (AntiSpeedHack_Enabled)
                    {
                        DS.Add("lastCoords", player.SteamID.ToString(), player.Location);
                        DS.Add("AntiSpeedHack", player.SteamID.ToString(), 0);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ConsolePrefix + " DS fill fail");
                    Logger.LogException(ex);
                }

                try
                {
                    IniParser iniBansIP;
                    string ConfigFile = Path.Combine(ModuleFolder, "BansIP.ini");
                    if (File.Exists(ConfigFile))
                        iniBansIP = new IniParser(ConfigFile);
                    else
                    {
                        Logger.LogError("BansIP.ini does not exist!");
                        return;
                    }
                    string IpBanned = iniBansIP.GetSetting("Ips", player.IP);
                    if (!string.IsNullOrEmpty(IpBanned))
                    {
                        player.MessageFrom(EchoBotName, "[color#FF2222]You have been banned.");
                        Logger.LogDebug(ConsolePrefix + " " + player.Name + " banned by IP!");
                        player.Disconnect();
                        return;
                    }

                    IniParser iniBansID;
                    ConfigFile = Path.Combine(ModuleFolder, "BansID.ini");
                    if (File.Exists(ConfigFile))
                        iniBansID = new IniParser(ConfigFile);
                    else
                    {
                        Logger.LogError("BansID.ini does not exist!");
                        return;
                    }
                    string IdBanned = iniBansID.GetSetting("Ids", player.SteamID);
                    if (!string.IsNullOrEmpty(IdBanned))
                    {
                        player.MessageFrom(EchoBotName, "[color#FF2222]You have been banned.");
                        Logger.LogDebug(ConsolePrefix + " " + player.Name + " banned by ID!");
                        player.Disconnect();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ConsolePrefix + " Bans check fail");
                    Logger.LogException(ex);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

                try
                {
                    if (NamesRestrict_Enabled)
                    {
                        var name = player.Name;
                        var len = player.Name.Length;

                        try
                        {
                            if (len > NamesRestrict_MaxLength)
                            {
                                player.MessageFrom(EchoBotName,
                                    "[color#FF2222]You have too many characters in your name. Please Change it. Maximum is " +
                                    NamesRestrict_MaxLength);
                                Log("Nick: " + player.Name + ". Too many chars in name.");
                                player.Disconnect();
                                return;
                            }

                            if (len < NamesRestrict_MinLength)
                            {
                                player.MessageFrom(EchoBotName,
                                    "[color#FF2222]You have not enough characters in your name. Please Change it. Minimum is " +
                                    NamesRestrict_MinLength);
                                Log("Nick: " + player.Name + ". Low length of name.");
                                player.Disconnect();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ConsolePrefix + " NameLenght fail");
                            Logger.LogException(ex);
                        }

                        try
                        {
                            foreach (char Symbol in player.Name)
                                if (NamesRestrict_AllowedChars.IndexOf(Symbol) == -1)
                                {
                                    player.MessageFrom(EchoBotName,
                                        "[color#FF2222]You have invalid characters in your name");
                                    Log("Nick: " + player.Name + ". Banned chars in name.");
                                    player.Disconnect();
                                    return;
                                }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ConsolePrefix + " Name Symbols fail");
                            Logger.LogException(ex);
                        }

                        try
                        {
                            for (var i = 0; i < BannedNames.Length; i++)
                            {
                                if (player.Name.ToLower() == BannedNames[i].ToLower())
                                {
                                    player.MessageFrom(EchoBotName,
                                        "[color#FF2222]This name isn't allowed. Please change your name.");
                                    Log("Nick: " + player.Name + ". Banned name.");
                                    player.Disconnect();
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ConsolePrefix + " BannedNames fail");
                            Logger.LogException(ex);
                        }

                        try
                        {
                            if (NamesRestrict_BindName)
                            {
                                IniParser BoundNames;
                                if (File.Exists(Path.Combine(ModuleFolder, "BoundNames.ini")))
                                    BoundNames = new IniParser(Path.Combine(ModuleFolder, "BoundNames.ini"));
                                else
                                {
                                    Logger.LogError(Path.Combine(ModuleFolder, "BoundNames.ini") + " does not exist!");
                                    return;
                                }

                                var Name = player.Name.ToLower(); 
                                string ID = BoundNames.GetSetting("Names", Name);
                                if ((player.Admin && NamesRestrict_AdminsOnly) || !NamesRestrict_AdminsOnly)
                                {
                                    if (string.IsNullOrEmpty(ID))
                                    {
                                        player.MessageFrom(EchoBotName,
                                            "[color#22AAFF]Nick " + player.Name + " was bound to your SteamID.");
                                        BoundNames.AddSetting("Names", Name, player.SteamID);
                                        BoundNames.Save();
                                    }
                                    else if (ID != player.SteamID)
                                    {
                                        player.MessageFrom(EchoBotName, "[color#FF2222]This nickname doesn't belong to you.");
                                        Log("Nick: " + player.Name + ". Nick stealer.");
                                        player.Disconnect();
                                        return;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ConsolePrefix + " BindName fail");
                            Logger.LogException(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ConsolePrefix + " NickRestrict check fail");
                    Logger.LogException(ex);
                }

                try
                {
                    if (RelogCooldown)
                    {
                        var Time = Environment.TickCount;

                        object ObjCooldown = DS.Get("loginCooldown", player.SteamID.ToString());
                        if (ObjCooldown == null)
                            return;
                        int Disconnected = (int) ObjCooldown;
                        if (Time <= Cooldown * 1000 + Disconnected)
                        {
                            var Remaining = ((Cooldown * 1000 - (Time - Disconnected)) / 1000).ToString("F2");
                            player.MessageFrom(EchoBotName,
                                "[color#FF2222]You must wait " + Cooldown + " seconds before reconnecting. Remaining: " +
                                Remaining +
                                " seconds.");
                            Logger.LogDebug(ConsolePrefix + " " + player.Name + " connect cooldown " + Cooldown + " sec!");
                            player.Disconnect();
                            return;
                        }
                        if (Time > Cooldown * 1000 + Disconnected)
                            DS.Remove("loginCooldown", player.SteamID.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ConsolePrefix + " Cooldown check fail");
                    Logger.LogException(ex);
                }

            Logger.LogDebug(ConsolePrefix + " " + player.Name + " Connected!");
        }

        private void EntityHurt(HurtEvent he)
        {
        }
    }
}
