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

namespace Neutronium.Content.Players
{
    public class NeutroniumPlayer : ModPlayer
    {
        public bool swinging;
        public int swingTime;
        public float swingCompletion;
        public Vector2 bladeTip;
        public bool trailSpawned;

        public float swingArc = MathHelper.ToRadians(180f);
        public float bladeLength = 60f;

        public bool shimmeringSapphire = false;
        public bool shimmeringEmerald = false;
        public bool shimmeringRuby = false;
        public bool corruptorChunk = false;

        public override void ResetEffects()
        {
            shimmeringSapphire = false;
            shimmeringEmerald = false;
            shimmeringRuby = false;
            corruptorChunk = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (shimmeringSapphire)
                target.AddBuff(BuffID.Frostburn, 180);

            if (shimmeringEmerald)
                target.AddBuff(BuffID.Poisoned, 180);

            if (shimmeringRuby)
                target.AddBuff(BuffID.OnFire, 180);

            if (corruptorChunk && Main.rand.NextFloat() < 0.20f)
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        public void UpdateSwordSwing(Player player, int useAnimation)
        {
            if (!swinging)
                return;

            swingTime++;
            swingCompletion = swingTime / (float)useAnimation;

            // Smoothstep easing
            float smoothT = swingCompletion * swingCompletion * (3f - 2f * swingCompletion);

            // Mouse relative to player
            Vector2 toMouse = Main.MouseWorld - player.Center;
            float mouseAngle = toMouse.ToRotation();

            // Player facing
            int dir = -Math.Sign(player.Center.X - Main.MouseWorld.X);
            player.direction = dir;

            // Swing arc offsets
            float startOffset = -swingArc / 2 * dir;
            float endOffset = swingArc / 2 * dir;

            // Item rotation
            player.itemRotation = mouseAngle + MathHelper.Lerp(startOffset, endOffset, smoothT);

            // Arms follow the sword
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            // Sword position
            player.itemLocation = player.Center;

            // Blade tip
            bladeTip = player.Center + player.itemRotation.ToRotationVector2() * bladeLength;
        }

        /// <summary>
        /// Call when swing starts
        /// </summary>
        public void StartSwing()
        {
            swinging = true;
            swingTime = 0;
            swingCompletion = 0f;
            trailSpawned = false;
        }

        /// <summary>
        /// Call when swing ends
        /// </summary>
        public void EndSwing()
        {
            swinging = false;
            swingTime = 0;
            swingCompletion = 0f;
            trailSpawned = false;
        }
    }
}