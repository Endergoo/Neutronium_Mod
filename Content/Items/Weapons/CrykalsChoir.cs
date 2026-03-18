using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.width = 140;
            Item.height = 140;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Do NOT set Item.shoot here for melee trails
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Spawn the trail projectile at the player's center
            if (player.itemAnimation == Item.useAnimation - 1) // only spawn once per swing
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
            }
            return false; // prevents shooting a normal projectile
        }
    }
}