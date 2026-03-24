using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Ammo
{
    public class StingerAmmo : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Stinger;

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.buyPrice(copper: 10);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileID.Stinger;
            Item.shootSpeed = 14f;
            Item.ammo = Item.type;
        }
    }
}