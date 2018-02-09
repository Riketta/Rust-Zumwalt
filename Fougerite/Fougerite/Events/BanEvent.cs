using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fougerite.Events
{
    public enum BanType
    {
        Player,
        IDandIP,
        OnlyID,
        OnlyIP
    }

    public class BanEvent
    {
        private BanType _type;
        private Fougerite.Player _player;
        private Fougerite.Player _sender;
        private string _ip;
        private string _id;
        private string _name;
        private string _reason;
        private string _banner;
        private bool _cancel = false;

        public BanEvent(Fougerite.Player player, string Banner, string reason, Fougerite.Player Sender)
        {
            _type = BanType.Player;
            _player = player;
            _reason = reason;
            _ip = player.IP;
            _id = player.SteamID;
            _name = player.Name;
            _sender = Sender;
            _banner = Banner;
        }

        public BanEvent(string ip, string id, string name, string reason, string adminname)
        {
            _type = BanType.IDandIP;
            _reason = reason;
            _ip = ip;
            _id = id;
            _name = name;
            _banner = adminname;
        }

        public BanEvent(string iporid, string name, string reason, string adminname, bool IsID)
        {
            if (IsID)
            {
                _type = BanType.OnlyID;
                _reason = reason;
                _id = iporid;
                _name = name;
                _banner = adminname;
            }
            else
            {
                _type = BanType.OnlyIP;
                _reason = reason;
                _ip = iporid;
                _name = name;
                _banner = adminname;
            }
        }

        public void Cancel()
        {
            _cancel = true;
        }

        public BanType BanType
        {
            get { return _type; }
        }

        public Fougerite.Player BannedUser
        {
            get { return _player; }
        }

        public Fougerite.Player BanSender
        {
            get { return _sender; }
        }

        public string IP
        {
            get { return _ip; }
        }
        public string ID
        {
            get { return _id; }
        }
        public string Name
        {
            get { return _name; }
        }
        public string Reason
        {
            get { return _reason; }
        }
        public string BannerName
        {
            get { return _banner; }
        }
        public bool Cancelled
        {
            get { return _cancel; }
        }
    }
}
