using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Neutronium.Content.Items.Armor;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Projectiles;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class BHL : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item92; 
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BHLProjectile>();
            Item.shootSpeed = 15f;
            Item.mana = 40; 
            Item.noMelee = true;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += player.GetModPlayer<GlobalPlayer>().StellarDamage;
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

        public override void AddRecipes()
        {
            Recipe BHL = CreateRecipe();
            BHL.AddIngredient(ItemID.FragmentNebula, 10);
            BHL.AddIngredient(ItemID.FragmentVortex, 10);
            BHL.AddIngredient(ItemID.FragmentStardust, 10);
            BHL.AddIngredient(ItemID.FragmentSolar, 10);
            BHL.AddIngredient(ItemID.LunarBar, 15);
            BHL.AddIngredient(ModContent.ItemType<AccretionDisk>(), 1);
            BHL.AddTile(TileID.LunarCraftingStation);
            BHL.Register();
        }
    }
}
