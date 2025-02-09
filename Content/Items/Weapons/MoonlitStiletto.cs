using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class MoonlitStiletto : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 80; // Match sprite width
            Item.height = 80; // Match sprite height
            Item.noMelee = false; // This is a melee weapon
            Item.noUseGraphic = false; // Show the weapon's sprite
            Item.autoReuse = true; // Automatically reuse the weapon
            Item.useStyle = ItemUseStyleID.Swing; // Use the Swing style for swinging
            Item.damage = 138; // Base damage
            Item.DamageType = DamageClass.Melee; // Melee weapon
            Item.useAnimation = Item.useTime = 13; // Attack speed
            Item.shootSpeed = 2.4f; // Speed of the projectile (if any)
            Item.knockBack = 8.5f; // Knockback strength
            Item.UseSound = SoundID.Item1; // Sound when used
            Item.rare = ItemRarityID.Pink; // Rarity

            Item.shoot = ModContent.ProjectileType<MoonlitStilettoProjectile>(); // Shoot the custom projectile
            Item.shootSpeed = 10f; // Speed of the projectile
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            // Add a recipe for crafting the Moonlit Stiletto
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 10) // Example ingredient
                .AddIngredient(ItemID.Moonglow, 3) // Example ingredient
                .AddTile(TileID.Anvils) // Crafted at an Anvil
                .Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            // Add some visual effects when the weapon is used
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.SilverFlame);
            }
        }

        public void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            // Add some effect when hitting an NPC
            target.AddBuff(BuffID.Frostburn, 180); // Example: Apply Frostburn debuff
        }

        public override void HoldItem(Player player)
        {
            // Make the player face the direction of the mouse
            if (player.whoAmI == Main.myPlayer) // Only apply to the local player
            {
                if (Main.MouseWorld.X > player.Center.X)
                {
                    player.ChangeDir(1); // Face right
                }
                else
                {
                    player.ChangeDir(-1); // Face left
                }
            }
        }
    }
}