using Microsoft.Xna.Framework;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class ParticleCollider : ModItem
    {
        public override void SetDefaults()
        {
  
            Item.width = 44; 
            Item.height = 18; 
            Item.rare = ItemRarityID.Orange; 
            Item.scale = 1.5f;      
            Item.useTime = 55; 
            Item.useAnimation = 55; 
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.autoReuse = true; 
            Item.UseSound = SoundID.Item94;         
            Item.DamageType = DamageClass.Ranged; 
            Item.damage = 45; 
            Item.knockBack = 6f;
            Item.noMelee = true;            
            Item.shoot = ProjectileID.PurificationPowder; 
            Item.shootSpeed = 10f; 
            Item.useAmmo = AmmoID.Bullet; 
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int NumProjectiles = 8; // The number of projectiles that this gun will shoot.

            for (int i = 0; i < NumProjectiles; i++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

                // Decrease velocity randomly for nicer visuals.
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);

                // Create a projectile.
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            return false; // Return false because we don't want tModLoader to shoot projectile
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe ParticleCollider = CreateRecipe();
            ParticleCollider.AddIngredient(ModContent.ItemType<CondensedNeutronium>(), 5);
            ParticleCollider.AddIngredient(ItemID.DarkShard, 2);
            ParticleCollider.AddIngredient(ItemID.ChlorophyteBar, 12);
            ParticleCollider.AddTile(TileID.MythrilAnvil);
            ParticleCollider.Register();
        }

        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 2.4f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
    }
}