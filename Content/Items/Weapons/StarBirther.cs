using System.Collections.Generic;
using System.Linq;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class StarBirther : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.DefaultToStaff(ProjectileID.SuperStar, 15, 20, 11);
            Item.width = 34;
            Item.height = 40;
            Item.UseSound = SoundID.Item125;
            Item.useTime = 10;

            // A special method that sets the damage, knockback, and bonus critical strike chance.
            Item.SetWeaponValues(100, 6, 20);

            Item.SetShopValues(ItemRarityColor.Purple11, 10000);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var lineToChange = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if(lineToChange != null)
            {
                string[] split = lineToChange.Text.Split(' ');
                lineToChange.Text = split.First()+" Stellar "+split.Last();
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += player.GetModPlayer<GlobalPlayer>().StellarDamage;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            // We can use ModifyManaCost to dynamically adjust the mana cost of this item, similar to how Space Gun works with the Meteor armor set.
            // See ExampleHood to see how accessories give the reduce mana cost effect.
            if (player.statLife < player.statLifeMax2 / 2)
            {
                mult *= 0.5f; // Half the mana cost when at low health. Make sure to use multiplication with the mult parameter.
            }
        }
        public override void AddRecipes()
        {
            Recipe StarBirther = CreateRecipe();
            StarBirther.AddIngredient(ModContent.ItemType<CondensedNeutronium>(), 5);
            StarBirther.AddIngredient(ItemID.Ectoplasm, 15);
            StarBirther.AddIngredient(ItemID.FragmentNebula, 10);
            StarBirther.AddTile(TileID.LunarCraftingStation);
            StarBirther.Register();
        }
    }
}