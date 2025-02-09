using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neutronium.Content.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class StrandOfLaniakeaProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.RangeMultiplier = 2f;
            Projectile.WhipSettings.Segments = 20;
            Projectile.damage = 100;

            // use these to change from the vanilla defaults
            // Projectile.WhipSettings.Segments = 20;
            // Projectile.WhipSettings.RangeMultiplier = 1f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        // This example uses PreAI to implement a charging mechanic.
        // If you remove this, also remove Item.channel = true from the item's SetDefaults.
        public override bool PreAI()
        {
            Player owner = Main.player[Projectile.owner];

            // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
            if (!owner.channel || ChargeTime >= 0)
            {
                return true; // Let the vanilla whip AI run.
            }


            return false; // Prevent the vanilla whip AI from running.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<StrandOfLaniakeaDebuff>(), 240);
            target.AddBuff(BuffID.OnFire, 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.5f); // Multihit penalty. Decrease the damage the more enemies the whip hits.
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            // Spawn yellow dust effect along the whip
            for (int i = 0; i < list.Count - 1; i++)
            {
                // Get the position of the dust (midpoint between each segment of the whip)
                Vector2 dustPosition = (list[i] + list[i + 1]) / 2;

                // Create the dust, set its position, and make it yellow
                Dust dust = Dust.NewDustDirect(dustPosition, 0, 0, DustID.GoldCoin, 0f, 0f, 100, default, 1.5f);

                // Modify dust properties to create a glowing light effect
                dust.noGravity = true;  // Make the dust float in the air without falling
                dust.scale = 1.2f;       // Slightly scale up the dust particles for a glowing effect
                dust.alpha = 100;        // Add some transparency to make it look softer
                dust.velocity *= 0.2f;   // Slow down the dust particles for a more subtle effect
            }

            // Continue with the custom whip drawing (this code remains the same)
            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!
                Rectangle frame = new Rectangle(0, 0, 10, 26); // The size of the Handle (measured in pixels)
                Vector2 origin = new Vector2(5, 8); // Offset for where the player's hand will start measured from the top left of the image.
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.
                if (i == list.Count - 2)
                {
                    // This is the head of the whip. You need to measure the sprite to figure out these values.
                    frame.Y = 74; // Distance from the top of the sprite to the start of the frame.
                    frame.Height = 18; // Height of the frame.

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    // Third segment
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    // Second Segment
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    // First Segment
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }

    }
}