using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class ThePlanck : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 51;
            Item.knockBack = 5;
            Item.crit = 7;

            Item.value = Item.buyPrice(silver: 25);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.scale = 1.4f;

            DescriptionAttribute descriptionAttribute = new DescriptionAttribute();
           
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }

    }
}
