
namespace Fougerite.Events
{
    public class CraftingEvent
    {
        private readonly CraftingInventory _inv;
        private readonly BlueprintDataBlock _block;
        private readonly int _amount;
        private readonly ulong _startTime;
        private readonly Fougerite.Player _player;
        private readonly bool _legit = true;
        private readonly NetUser _user;

        public CraftingEvent(CraftingInventory inv, BlueprintDataBlock blueprint, int amount, ulong startTime)
        {
            this._inv = inv;
            this._block = blueprint;
            this._amount = amount;
            this._startTime = startTime;
            var netUser = inv.GetComponent<Character>().netUser;
            this._player = Fougerite.Server.Cache[netUser.userID];
            this._user = netUser;
            if (!_player.HasBlueprint(blueprint))
            {
                _legit = false;
                Cancel();
                Logger.LogWarning("[CraftingHack] Detected: " + _player.Name + " | " + _player.SteamID + " | " + _player.IP + " | " + blueprint.name);
                Fougerite.Server.GetServer().Broadcast("CraftingHack Detected: " + _player.Name);
                if (Bootstrap.AutoBanCraft)
                {
                    Fougerite.Server.GetServer().BanPlayer(_player, "Console", "CraftingHack");
                }
            }
        }

        public bool IsLegit
        {
            get { return _legit; }
        }

        public Fougerite.Player Player
        {
            get { return _player; }
        }

        public NetUser NetUser
        {
            get { return _user; }
        }

        public ulong StartTime
        {
            get { return _startTime; }
        }

        public void Cancel()
        {
            this._inv.CancelCrafting();
        }

        public CraftingInventory CraftingInventory
        {
            get { return _inv; }
        }

        public float LastWorkBenchTime
        {
            get { return _inv._lastWorkBenchTime; }
        }

        public BlueprintDataBlock BlueprintDataBlock
        {
            get { return _block; }
        }

        public string ItemName
        {
            get { return _block.name; }
        }

        public BlueprintDataBlock.IngredientEntry[] Ingredients
        {
            get { return _block.ingredients.ToArray(); }
        }

        public bool RequireWorkbench
        {
            get { return _block.RequireWorkbench; }
        }

        public ItemDataBlock ResultItem
        {
            get { return _block.resultItem; }
        }

        public int ResultItemNumber
        {
            get { return _block.numResultItem; }
        }

        public int Amount
        {
            get { return _amount; }
        }

    }
}
