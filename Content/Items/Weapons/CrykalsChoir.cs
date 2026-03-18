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

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem
    {
        private int swingTime;           // counts ticks of the current swing
        private bool trailSpawned;       // spawn trail once per swing
        private Vector2 bladeTip;        // tip of the sword for hitbox/trail

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
            // reset swing at the start
            swingTime = 0;
            trailSpawned = false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            swingTime++;
            float completion = swingTime / (float)Item.useAnimation;

            // Smoothstep easing: 0→1, accelerates then decelerates
            float smoothT = completion * completion * (3f - 2f * completion);

            // Mouse relative to player
            Vector2 toMouse = Main.MouseWorld - player.Center;
            float mouseAngle = toMouse.ToRotation();

            // Update player direction to face mouse
            int dir = -Math.Sign(player.Center.X - Main.MouseWorld.X);
            player.direction = dir;

            // Swing arc relative to pivot
            float swingArc = MathHelper.ToRadians(180f); // total swing in degrees
            float startOffset = -swingArc / 2 * dir;
            float endOffset = swingArc / 2 * dir;

            // Smooth rotation along the arc
            player.itemRotation = mouseAngle + MathHelper.Lerp(startOffset, endOffset, smoothT);

            // Arms follow the sword
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            // Draw sword at player center
            player.itemLocation = player.Center;

            // Blade tip position for trail/hitbox
            bladeTip = player.Center + player.itemRotation.ToRotationVector2() * 60f;

            // Spawn trail mid-swing once
            if (!trailSpawned && completion >= 0.25f)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), bladeTip, Vector2.Zero,
                    ModContent.ProjectileType<CrykalsChoirTrail>(), 0, 0f, player.whoAmI);
                trailSpawned = true;

                // Optional swoosh sound
                SoundEngine.PlaySound(SoundID.Item1, player.Center);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            float size = 40f;
            hitbox = new Rectangle((int)(bladeTip.X - size / 2), (int)(bladeTip.Y - size / 2),
                                    (int)size, (int)size);
        }
    }
}