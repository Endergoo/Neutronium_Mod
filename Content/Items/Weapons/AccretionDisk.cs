using Microsoft.Xna.Framework;
using Neutronium.Content.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class AccretionDisk : ModItem
    {
        public override void SetDefaults()
        {
           
            Item.useTime = 10; 
            Item.useAnimation = 10; 
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.autoReuse = true; 
            Item.DamageType = DamageClass.Ranged; 
            Item.damage = 60; 
            Item.knockBack = 1f; 
            Item.noMelee = true;            
            Item.shoot = ProjectileID.FallingStar; 
            Item.shootSpeed = 10f; 
            Item.useAmmo = AmmoID.FallenStar; 
            Item.width = 54; 
            Item.height = 22; 
            Item.rare = ItemRarityID.Green; 
            Item.UseSound = SoundID.Item11; 
            Item.scale = 2f;
        }
   
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.38f;
        }

        
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-9f, -2f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override void AddRecipes()
        {
            Recipe AccretionDisk = CreateRecipe();
            AccretionDisk.AddIngredient(ItemID.FallenStar, 12);
            AccretionDisk.AddIngredient(ItemID.HallowedBar, 15);
            AccretionDisk.AddIngredient(ItemID.MeteoriteBar, 10);
            AccretionDisk.AddTile(TileID.MythrilAnvil);
            AccretionDisk.Register();
        }
    }
}