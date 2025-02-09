using Neutronium.Content.Buffs;
using Neutronium.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class StrandOfLaniakea : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(StrandOfLaniakeaDebuff.TagDamage);

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<StrandOfLaniakeaProjectile>(), 20, 2, 4);
            Item.rare = ItemRarityID.Pink;
            Item.channel = true;
            Item.damage = 170;
        }

    // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}