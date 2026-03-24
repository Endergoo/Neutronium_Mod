using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class StingerGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 28;
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Stinger;
            Item.shootSpeed = 14f;
            Item.noMelee = true;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11;
            Item.useAmmo = AmmoID.Arrow; // uses arrows as ammo, swap to AmmoID.Bullet if preferred
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Override to always shoot StingerProj regardless of ammo type
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<StingerProj>(), damage, knockback, player.whoAmI);
            return false;
        }

         public override void AddRecipes()
        {
            CreateRecipe().
                .AddIngredient(ItemID.Stinger, 10)
                .AddIngredient(ItemID.JungleSpores, 12)
                .AddIngredient(ItemID.Vine, 2)
                .AddTile(TileID.Anvils)
                Register();
        }
    }
}