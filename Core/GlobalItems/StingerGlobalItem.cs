public class StingerGlobalItem : GlobalItem
{
    public override void SetDefaults(Item item)
    {
        if (item.type == ItemID.Stinger)
            item.ammo = ItemID.Stinger;
    }
}