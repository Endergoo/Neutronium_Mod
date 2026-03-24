using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Neutronium.Core.Utils;

public class StingerGlobalItem : GlobalItem
{
    public override void SetDefaults(Item item)
    {
        if (item.type == ItemID.Stinger)
            item.ammo = ItemID.Stinger;
    }
}