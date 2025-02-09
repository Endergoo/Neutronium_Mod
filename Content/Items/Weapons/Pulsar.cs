using Neutronium.Content.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class Pulsar : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true; 
            ItemID.Sets.GamepadExtraRange[Item.type] = 30; 
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true; 
        }
     
        public override void SetDefaults()
        {
            Item.width = 24; // hitbox
            Item.height = 24; // hitbox

            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.useTime = 25; 
            Item.useAnimation = 25; 
            Item.noMelee = true; 
            Item.noUseGraphic = true; 
            Item.UseSound = SoundID.Item1; 

            Item.damage = 50; 
            Item.DamageType = DamageClass.MeleeNoSpeed; 
            Item.knockBack = 2.5f;
            Item.crit = 8;
            Item.channel = true; 
            Item.rare = ItemRarityID.Master;
            Item.value = Item.buyPrice(gold: 1); 

            ProjectileID.Sets.YoyosLifeTimeMultiplier[ModContent.ProjectileType<PulsarProjectile>()] = -1f;
            ProjectileID.Sets.YoyosTopSpeed[ModContent.ProjectileType<PulsarProjectile>()] = 25f;
            ProjectileID.Sets.YoyosMaximumRange[ModContent.ProjectileType<PulsarProjectile>()] = 600f;

            Item.shoot = ModContent.ProjectileType<PulsarProjectile>(); 
            Item.shootSpeed = 20f; // velocity		
        }


        private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy };

        public override bool AllowPrefix(int pre)
        {
            if (Array.IndexOf(unwantedPrefixes, pre) > -1)
            {
                return false;
            }
  
            return true;
        }
    }
}
