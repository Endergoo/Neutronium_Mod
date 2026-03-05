using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Neutronium.Content.Items
{
    public class BookOfCelestials : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CelestialBeam>();
            Item.shootSpeed = 0f; // Beam has no velocity
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Set beam type based on current celestial alignment
            float beamType = 0; // 0=Star, 1=Sun, 2=Moon, 3=Cosmic
            
            // You could make this cycle based on time or something
            beamType = (float)(Main.time % 4);
            
            // Spawn the beam projectile at mouse position
            position = Main.MouseWorld;
            
            Projectile.NewProjectile(
                player.GetSource_ItemAI(Item),
                position,
                Vector2.Zero, // No initial velocity
                type,
                damage,
                knockback,
                player.whoAmI,
                ai0: 0.4f, // Attack speed
                ai1: beamType // Beam type
            );
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}