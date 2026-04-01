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
        private const int Duration = 25;

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
                    // Orange/red debris
                    Dust debris = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.RedTorch);
                    debris.noGravity = false;
                    debris.scale = Main.rand.NextFloat(1.2f, 2.5f);
                    debris.velocity = Main.rand.NextVector2Circular(14f, 14f);
                    debris.color = new Color(255, Main.rand.Next(50, 150), 0) with { A = 0 };

                    // White hot core debris
                    Dust core = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.RedTorch);
                    core.noGravity = false;
                    core.scale = Main.rand.NextFloat(0.6f, 1.2f);
                    core.velocity = Main.rand.NextVector2Circular(8f, 8f);
                    core.color = Color.White with { A = 0 };
                }
            }

            // Red/orange light that fades
            float lightFade = 1f - progress;
            Lighting.AddLight(Projectile.Center, lightFade * 2f, lightFade * 0.4f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Neutronium/Content/Particles/SmoothCircle").Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = circle.Size() / 2f;

            float progress = Timer / Duration;
            float eased = (float)Math.Pow(progress, 0.4f); // fast expand then slow

            // --- White core flash (fades quickly) ---
            float coreScale = MathHelper.Lerp(0.5f, 1.5f, eased);
            float coreAlpha = Math.Max(0f, 1f - progress * 3f); // fades by 1/3 duration
            Main.spriteBatch.Draw(circle, center, null,
                Color.White with { A = 0 } * coreAlpha,
                0f, origin, coreScale, SpriteEffects.None, 0f);

            // --- Orange/yellow inner glow ---
            float innerScale = MathHelper.Lerp(0.3f, 2.5f, eased);
            float innerAlpha = Math.Max(0f, 1f - progress * 2f);
            Main.spriteBatch.Draw(circle, center, null,
                new Color(255, 150, 0) with { A = 0 } * innerAlpha * 0.8f,
                0f, origin, innerScale, SpriteEffects.None, 0f);

            // --- Expanding shockwave ring ---
            float ringScale = MathHelper.Lerp(0.5f, 4f, eased);
            float ringAlpha = Math.Max(0f, 1f - progress * 1.5f) * (float)Math.Sin(progress * Math.PI);
            // Draw ring as a thin stretched circle
            for (int i = 0; i < 8; i++)
            {
                float angle = i / 8f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * ringScale * 30f;
                Main.spriteBatch.Draw(circle, center + offset, null,
                    new Color(255, 50, 0) with { A = 0 } * ringAlpha * 0.5f,
                    angle, origin, new Vector2(0.15f, 0.4f) * ringScale * 0.3f,
                    SpriteEffects.None, 0f);
            }

            // --- Dark outer shockwave ---
            float outerScale = MathHelper.Lerp(1f, 6f, eased);
            float outerAlpha = (float)Math.Sin(progress * Math.PI) * 0.4f;
            Main.spriteBatch.Draw(circle, center, null,
                new Color(80, 20, 0) with { A = 0 } * outerAlpha,
                0f, origin, outerScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}