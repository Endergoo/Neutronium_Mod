using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;


namespace Neutronium.Content.Players
{
    public class NeutroniumPlayer : ModPlayer
    {
        // Tracks the current sword swing
        public bool swinging;
        public int swingTime;            // counts ticks for the swing
        public float swingCompletion;    // 0 -> 1
        public Vector2 bladeTip;         // tip position for hitbox / trail
        public bool trailSpawned;        // whether trail was spawned this swing

        // Optional: customize swing arc and length
        public float swingArc = MathHelper.ToRadians(180f);
        public float bladeLength = 60f;

        public override void ResetEffects()
        {
            // Reset any temporary flags each tick
            // (You can add more later if needed)
        }

        /// <summary>
        /// Call this every tick for a sword item.
        /// </summary>
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