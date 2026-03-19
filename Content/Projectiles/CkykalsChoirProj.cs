using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace Neutronium.Content.Projectiles
{
    public class CrykalsChoirProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0"; // InvisibleProj placeholder
        public ref float Time => ref Projectile.ai[0];
        public Color mainColor = Color.Purple;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // Smooth sine rotation for gyration
            float sine = (float)Math.Sin(Time * 0.2f);
            Projectile.rotation = 0.25f * sine;

            // Early curved motion
            if (Time < 20)
            {
                Projectile.extraUpdates = 1;
                Projectile.velocity = Projectile.velocity.RotatedBy(0.03f * Projectile.direction);
            }
            else
            {
                Projectile.extraUpdates = 0;
                NPC target = FindClosestNPC(600f);
                if (target != null)
                {
                    Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.05f);
                }
            }

            // Particle sparks (like StarofOrder rays)
            if (Time % 2 == 0)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GemAmethyst
                );
                dust.noGravity = true;
                dust.scale = 0.6f + Main.rand.NextFloat() * 0.4f;
                dust.color = mainColor * 0.7f;
                dust.velocity *= 0.2f;
            }

            Time++;
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closest = null;
            float sqrMaxDist = maxDetectDistance * maxDetectDistance;
            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    float sqrDist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                    if (sqrDist < sqrMaxDist)
                    {
                        sqrMaxDist = sqrDist;
                        closest = npc;
                    }
                }
            }
            return closest;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Neutronium/Content/Particles/SmoothCircle").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2f;

            // Additive-style color (A = 0 = additive blending in SpriteBatch default)
            Color drawColor = mainColor with { A = 0 };

            // Lightweight trail
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float progress = 1f - i / (float)Projectile.oldPos.Length;
                float scale = Projectile.scale * 0.2f * progress;
                Color trailColor = drawColor * progress * 0.4f;

                bool roted = true;
                for (int j = 0; j < 5; j++)
                {
                    Main.spriteBatch.Draw(
                        texture, pos, null,
                        trailColor,
                        (roted ? 0 : MathHelper.PiOver2) + Projectile.rotation,
                        origin,
                        new Vector2(1 - 0.12f * j, 1 + 0.75f * j) * scale,
                        SpriteEffects.None, 0f
                    );
                    if (roted && j == 4) { j = -1; roted = false; }
                }
            }

            // Main star draw — small scale like StarofOrder
            bool mainRoted = true;
            for (int j = 0; j < 5; j++)
            {
                Main.spriteBatch.Draw(
                    texture, drawPos, null,
                    drawColor,
                    (mainRoted ? 0 : MathHelper.PiOver2) + Projectile.rotation,
                    origin,
                    new Vector2(1 - 0.12f * j, 1 + 0.75f * j) * Projectile.scale * 0.2f * Main.rand.NextFloat(0.8f, 1.1f),
                    SpriteEffects.None, 0f
                );
                if (mainRoted && j == 4) { j = -1; mainRoted = false; }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnKill(int timeLeft)
        {
            int points = 6;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f).RotatedBy(Projectile.rotation));
            for (int k = 0; k < points; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2, DustID.GemAmethyst, velocity * 6);
                dust.scale = 1.5f + Main.rand.NextFloat() * 0.5f;
                dust.noGravity = true;
                dust.color = mainColor;
                dust.fadeIn = 0.5f;
            }
        }
    }
}