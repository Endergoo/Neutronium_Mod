using Neutronium.Content.Items;
using Neutronium.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class Blazar : ModItem, ILocalizedModType
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 33;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BlazarProjectile>();
            Item.shootSpeed = 13f;
        }

    }
}