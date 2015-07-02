﻿namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class MuteCommand : ChatCommand
    {
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, "Mute Usage:  /mute playerName");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    MutePlayer(new PList.Player(entry.Key, entry.Value), Arguments.argUser);
                    return;
                } else if (entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(entry.Key, entry.Value);
            }
            if (list.Count == 1)
            {
                foreach (PlayerClient client in PlayerClient.All)
                {
                    if (client.netUser.displayName.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {
                        MutePlayer(new PList.Player(client.netUser.userID, client.netUser.displayName), Arguments.argUser);
                        return;
                    } else if (client.netUser.displayName.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(client.netUser.userID, client.netUser.displayName);
                }
            }
            if (list.Count == 1)
            {
                Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, "No player matches the name: " + playerName);
                return;
            }
            Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, string.Format("{0}  player{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "s match" : " matches"), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
            }
            Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, "0 - Cancel");
            Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, "Please enter the number matching the player you were looking for.");
            Core.muteWaitList[Arguments.argUser.userID] = list;
        }

        public void PartialNameMute(ref ConsoleSystem.Arg Arguments, int id)
        {
            if (id == 0)
            {
                Util.sayUser(Arguments.argUser.networkPlayer, Core.Name, "Cancelled!");
                return;
            }
            PList list = (PList)Core.muteWaitList[Arguments.argUser.userID];
            MutePlayer(list.PlayerList[id], Arguments.argUser);
        }

        public void MutePlayer(PList.Player mute, NetUser myAdmin)
        {
            if (mute.UserID == myAdmin.userID)
            {
                Util.sayUser(myAdmin.networkPlayer, Core.Name, "There is no point muting yourself.");
                return;
            }
            if (Core.muteList.Contains(mute.UserID))
            {
                Util.sayUser(myAdmin.networkPlayer, Core.Name, string.Format("{0} is already muted.", mute.DisplayName));
                return;
            }
            if (Administrator.IsAdmin(mute.UserID))
            {
                Administrator mutingAdmin = Administrator.GetAdmin(myAdmin.userID);
                Administrator mutedAdmin = Administrator.GetAdmin(mute.UserID);
                if (mutedAdmin.HasPermission("CanUnmute") || mutedAdmin.HasPermission("CanAddFlags") || mutedAdmin.HasPermission("RCON"))
                {
                    if (!mutedAdmin.HasPermission("RCON"))
                    {
                        if (mutingAdmin.HasPermission("RCON") || mutingAdmin.HasPermission("CanUnflag"))
                        {                                     
                            mutedAdmin.Flags.Remove("CanUnmute");
                            mutedAdmin.Flags.Remove("CanMute");
                            mutedAdmin.Flags.Remove("CanAddFlags");
                            mutedAdmin.Flags.Remove("CanUnflag");
                        }
                    } else
                    {
                        Util.sayUser(myAdmin.networkPlayer, Core.Name, string.Format("{0} is an administrator. You can't mute administrators.", mute.DisplayName));
                        return;
                    }
                }
            }
            Core.muteList.Add(mute);
            Administrator.NotifyAdmins(string.Format("{0} has been muted by {1}.", mute.DisplayName, myAdmin.displayName));
        }
    }
}