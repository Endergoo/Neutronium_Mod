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
        // Base projectile texture must exist, but it's invisible
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";
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
            // Smooth slow gyration
            float sine = (float)Math.Sin(Time * 0.2f * Projectile.scale);
            Projectile.rotation = sine * 0.15f;

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

            // Soft star sparks
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

            // --- Layered bloom trail ---
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float progress = 1f - i / (float)Projectile.oldPos.Length;
                float scale = Projectile.scale * progress * 0.8f;
                Color color = mainColor * progress * 0.5f;

                // Draw 3 rotated layers per old position
                for (int j = 0; j < 3; j++)
                {
                    float rotationOffset = MathHelper.TwoPi / 3 * j + Projectile.rotation;
                    Main.spriteBatch.Draw(texture, pos, null, color, rotationOffset, origin, scale, SpriteEffects.None, 0f);
                }
            }

            // --- Main projectile draw ---
            for (int j = 0; j < 3; j++)
            {
                float rotationOffset = MathHelper.TwoPi / 3 * j + Projectile.rotation;
                Main.spriteBatch.Draw(texture, drawPos, null, mainColor, rotationOffset, origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            return false; // Skip default draw
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