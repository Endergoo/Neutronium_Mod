using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class MaliceExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public ref float Timer => ref Projectile.ai[0];
        private const int Duration = 75;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Duration;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity = Vector2.Zero;

            float progress = Timer / Duration;

            // Spawn debris particles in first few frames
            if (Timer < 5)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust debris = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.RedTorch);
                    debris.noGravity = false;
                    debris.scale = Main.rand.NextFloat(1.2f, 2.5f);
                    debris.velocity = Main.rand.NextVector2Circular(14f, 14f);
                    debris.color = new Color(255, Main.rand.Next(50, 150), 0) with { A = 0 };

                    Dust core = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.RedTorch);
                    core.noGravity = false;
                    core.scale = Main.rand.NextFloat(0.6f, 1.2f);
                    core.velocity = Main.rand.NextVector2Circular(8f, 8f);
                    core.color = Color.White with { A = 0 };
                }
            }

            float lightFade = 1f - progress;
            Lighting.AddLight(Projectile.Center, lightFade * 2f, lightFade * 0.4f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Neutronium/Content/Particles/SmoothCircle").Value;
            Texture2D line = ModContent.Request<Texture2D>("Neutronium/Content/Particles/BloomLineThick").Value;

            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 circleOrigin = circle.Size() / 2f;
            Vector2 lineOrigin = new Vector2(line.Width / 2f, line.Height);

            float progress = Timer / Duration;
            float eased = (float)Math.Pow(progress, 0.4f);

            // --- White core flash ---
            float coreScale = MathHelper.Lerp(0.3f, 1.2f, eased);
            float coreAlpha = Math.Max(0f, 1f - progress * 3f);
            Main.spriteBatch.Draw(circle, center, null,
                Color.White with { A = 0 } * coreAlpha,
                0f, circleOrigin, coreScale, SpriteEffects.None, 0f);

            // --- Orange inner glow ---
            float innerScale = MathHelper.Lerp(0.2f, 2f, eased);
            float innerAlpha = Math.Max(0f, 1f - progress * 2f);
            Main.spriteBatch.Draw(circle, center, null,
                new Color(255, 150, 0) with { A = 0 } * innerAlpha * 0.8f,
                0f, circleOrigin, innerScale, SpriteEffects.None, 0f);

            // --- Rays radiating outward using BloomLineThick ---
            float rayAlpha = Math.Max(0f, 1f - progress * 2.5f);
            float rayLength = MathHelper.Lerp(0.02f, 0.12f, eased);
            int rayCount = 8;
            for (int i = 0; i < rayCount; i++)
            {
                float individualAlpha = rayAlpha * (0.7f + 0.3f * (float)Math.Sin(i * 1.3f));
                float angle = i / (float)rayCount * MathHelper.TwoPi;

                // Outer ray
                Main.EntitySpriteDraw(line, center, null,
                    new Color(163, 158, 158) with { A = 0 } * individualAlpha * 0.6f,
                    angle + MathHelper.PiOver2,
                    lineOrigin,
                    new Vector2(0.04f, rayLength),
                    SpriteEffects.None, 0);

                // Bright core ray
                Main.EntitySpriteDraw(line, center, null,
                    Color.White with { A = 0 } * individualAlpha * 0.4f,
                    angle + MathHelper.PiOver2,
                    lineOrigin,
                    new Vector2(0.015f, rayLength * 0.7f),
                    SpriteEffects.None, 0);
            }

            // --- Expanding shockwave ring using SmoothCircle ---
            float ringScale = MathHelper.Lerp(0.5f, 5f, eased);
            float ringAlpha = (float)Math.Sin(progress * Math.PI) * 0.5f;
            // Draw as a thin ring by drawing two circles — outer minus inner
            Main.spriteBatch.Draw(circle, center, null,
                new Color(255, 60, 0) with { A = 0 } * ringAlpha,
                0f, circleOrigin, ringScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}