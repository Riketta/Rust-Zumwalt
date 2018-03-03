

namespace Fougerite.Events
{
    public class ResearchEvent
    {
        private readonly IInventoryItem _item;
        private readonly Fougerite.Player _player;
        private bool _cancelled;

        public ResearchEvent(IInventoryItem item)
        {
            this._item = item;
            this._player = Fougerite.Server.Cache[item.character.netUser.userID];
        }

        public Fougerite.Player Player
        {
            get { return this._player; }
        }

        public IInventoryItem Item
        {
            get { return this._item; }
        }

        public ItemDataBlock ItemDataBlock
        {
            get { return this._item.datablock; }
        }

        public string ItemName 
        {
            get { return this._item.datablock.name; }
        }
        
        public bool Cancelled 
        {
            get { return this._cancelled; }
        }

        public void Cancel()
        {
            //PlayerInventory invent = Player.Inventory.InternalInventory as PlayerInventory;
            //if (invent != null) invent.GetBoundBPs().Remove(Util.GetUtil().BlueprintOfItem(ItemDataBlock));
            _cancelled = true;
        }
    }
}
