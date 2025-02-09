using Neutronium.Content.Items;
using Neutronium.Content.Projectiles;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Content.Items.Weapons
{
   
    public class LogisticalStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true; // This makes the useStyle animate as a staff instead of as a gun.
        }

        public override void SetDefaults()
        {
            // DefaultToStaff handles setting various Item values that magic staff weapons use.
            // Hover over DefaultToStaff in Visual Studio to read the documentation!
            Item.DefaultToStaff(ModContent.ProjectileType<LogisticalStaffProjectile>(), 16, 25, 12);

            // Customize the UseSound. DefaultToStaff sets UseSound to SoundID.Item43, but we want SoundID.Item20
            Item.UseSound = SoundID.Item73;

            // Set damage and knockBack
            Item.SetWeaponValues(110, 5);

            // Set rarity and value
            Item.SetShopValues(ItemRarityColor.LightPurple6, 10000);
            Item.scale = 2f;
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


    }
}