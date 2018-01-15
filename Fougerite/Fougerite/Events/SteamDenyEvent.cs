
using uLink;

namespace Fougerite.Events
{
    public class SteamDenyEvent
    {
        private readonly ClientConnection _cc;
        private readonly NetworkPlayerApproval _approval;
        private readonly string _strReason;
        private readonly NetError _errornum;
        private bool _forceallow = false;

        public SteamDenyEvent(ClientConnection cc, NetworkPlayerApproval approval, string strReason, NetError errornum)
        {
            this._cc = cc;
            this._approval = approval;
            this._strReason = strReason;
            this._errornum = errornum;
        }

        public NetUser NetUser
        {
            get { return _cc.netUser; }
        }

        public ClientConnection ClientConnection
        {
            get { return _cc; }
        }

        public NetworkPlayerApproval Approval
        {
            get { return _approval; }
        }

        public string Reason
        {
            get { return _strReason; }
        }

        public NetError ErrorNumber
        {
            get { return _errornum; }
        }

        public bool ForceAllow
        {
            get { return _forceallow; }
            set { _forceallow = value; }
        }
    }
}
