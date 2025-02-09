using Neutronium.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Tiles;

namespace Neutronium.Content.Items.Weapons
{
    public class GammaRayBurst : ModItem
    {
        // You can use a vanilla texture for your item by using the format: "Terraria/Item_<Item ID>".
        public override string Texture => "Terraria/Images/Item_" + ItemID.LastPrism;
        public static Color OverrideColor = new(46, 110, 209);

        public override void SetDefaults()
        {
            // Start by using CloneDefaults to clone all the basic item properties from the vanilla Last Prism.
            // For example, this copies sprite size, use style, sell price, and the item being a magic weapon.
            Item.CloneDefaults(ItemID.LastPrism);
            Item.mana = 40;
            Item.damage = 400;
            Item.shoot = ModContent.ProjectileType<GammaRayBurstHoldout>();
            Item.shootSpeed = 30f;

            // Change the item's draw color so that it is visually distinct from the vanilla Last Prism.
            Item.color = OverrideColor;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var lineToChange = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (lineToChange != null)
            {
                string[] split = lineToChange.Text.Split(' ');
                lineToChange.Text = split.First() + " Stellar " + split.Last();
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += player.GetModPlayer<GlobalPlayer>().StellarDamage;
        }

        public override void AddRecipes()
        {
            Recipe SchwarschildChestplate = CreateRecipe();
            SchwarschildChestplate.AddIngredient(ModContent.ItemType<CondensedNeutronium>(), 12);
            SchwarschildChestplate.AddIngredient(ItemID.FragmentSolar, 15);
            SchwarschildChestplate.AddIngredient(ItemID.FragmentNebula, 15);
            SchwarschildChestplate.AddIngredient(ItemID.FragmentVortex, 15);
            SchwarschildChestplate.AddIngredient(ItemID.FragmentStardust, 15);
            SchwarschildChestplate.AddTile(TileID.AdamantiteForge);
            SchwarschildChestplate.Register();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<GammaRayBurstHoldout>()] <= 0;
        }
    }
}