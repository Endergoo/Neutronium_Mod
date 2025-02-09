using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Neutronium.Content.Projectiles;
using Terraria.Enums;

namespace Neutronium.Content.Items.Weapons
{
    internal class SBG : ModItem
    {
        public override void SetDefaults()
        {

            Item.DefaultToStaff(ModContent.ProjectileType<SBGprojectile>(), 18, 25, 0);
            Item.mana = 0;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item47;
            Item.SetWeaponValues(110, 5);
            Item.SetShopValues(ItemRarityColor.LightPurple6, 10000);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.scale = 0.7f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, 0f);
        }




    }

}

