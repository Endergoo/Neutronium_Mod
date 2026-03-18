using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Terraria.DataStructures;
using Neutronium.Content.Projectiles;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
        }

        public override void UseAnimation(Player player)
        {
            // Start swing in NeutroniumPlayer
            player.GetModPlayer<NeutroniumPlayer>().StartSwing();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            var nPlayer = player.GetModPlayer<NeutroniumPlayer>();
            
            // Update swing every tick
            nPlayer.UpdateSwordSwing(player, Item.useAnimation);

            // Spawn trail once mid-swing
            if (!nPlayer.trailSpawned && nPlayer.swingCompletion >= 0.25f)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), nPlayer.bladeTip, Vector2.Zero,
                    ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
                nPlayer.trailSpawned = true;

                // Optional swoosh sound
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            var nPlayer = player.GetModPlayer<NeutroniumPlayer>();
            float size = 40f;
            hitbox = new Rectangle((int)(nPlayer.bladeTip.X - size / 2), (int)(nPlayer.bladeTip.Y - size / 2),
                                    (int)size, (int)size);
        }
    }
}