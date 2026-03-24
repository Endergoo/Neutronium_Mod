using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;
using Neutronium.Core.GlobalItems;

namespace Neutronium.Content.Items.Weapons
{
    public class SpitefulPrick : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 28;
            Item.damage = 13;
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
            Item.shoot = ModContent.ProjectileType<StingerProjectile>();
            Item.useAmmo = ItemID.Stinger;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<StingerProjectile>(), damage, knockback, player.whoAmI);

            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ItemID.Stinger)
                {
                    player.inventory[i].stack--;
                    if (player.inventory[i].stack <= 0)
                        player.inventory[i].TurnToAir();
                    break;
                }
            }

            return false;
        }

         public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 10) 
                .AddIngredient(ItemID.JungleSpores, 12) 
                .AddIngredient(ItemID.Vine, 2) 
                .AddTile(TileID.Anvils) 
                .Register();
        }
    }
}