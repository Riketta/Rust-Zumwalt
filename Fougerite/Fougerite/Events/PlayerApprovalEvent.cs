
namespace Fougerite.Events
{
    using uLink;

    /// <summary>
    /// This class is created when the player is authenticating with the server.
    /// </summary>
    public class PlayerApprovalEvent
    {
        private readonly ConnectionAcceptor _ca;
        private readonly NetworkPlayerApproval _approval;
        private readonly ClientConnection _cc;
        private readonly bool _deny;
        private bool _ForceAccept = false;

        public PlayerApprovalEvent(ConnectionAcceptor ca, NetworkPlayerApproval approval, ClientConnection cc, bool AboutToDeny)
        {
            this._ca = ca;
            this._cc = cc;
            this._approval = approval;
            this._deny = AboutToDeny;
        }

        /// <summary>
        /// Gets the ConnectionAcceptor class
        /// </summary>
        public ConnectionAcceptor ConnectionAcceptor
        {
            get { return _ca; }
        }

        /// <summary>
        /// Gets the ClientConnection class
        /// </summary>
        public ClientConnection ClientConnection
        {
            get { return _cc; }
        }

        /// <summary>
        /// Gets the NetworkPlayerApproval class.
        /// </summary>
        public NetworkPlayerApproval NetworkPlayerApproval
        {
            get { return _approval; }
        }

        /// <summary>
        /// Is the player going to be denied?
        /// </summary>
        public bool AboutToDeny
        {
            get { return _deny; }
        }

        /// <summary>
        /// Accept the player no matter the cost?
        /// </summary>
        public bool ForceAccept
        {
            get { return _ForceAccept; }
            set { _ForceAccept = value; }
        }

        /// <summary>
        /// This just checks if the player's steamid is already found in the online list.
        /// </summary>
        public bool ServerHasPlayer
        {
            get
            {
                if (Fougerite.Server.Cache.ContainsKey(_cc.UserID))
                {
                    return Fougerite.Server.Cache[_cc.UserID].IsOnline;
                }
                return false;
            }
        }
    }
}
