using Microsoft.Xna.Framework;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Projectiles;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class BibbleglogBlaster : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Magic;
            Item.width = 38;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item122; 
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ChainLightningProj>();
            Item.shootSpeed = 18f;
            Item.mana = 10; 
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 55f + new Vector2(0f, -8f);
            position += muzzleOffset;

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ChainLightningProj>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 12)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}