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
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";
        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
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
            // Slight sine wobble
            float sine = (float)Math.Sin(Time * 0.2f);
            Projectile.rotation = sine * 0.3f;

            // Phase 1: curved movement
            if (Time < 20)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.03f * Projectile.direction);
            }
            // Phase 2: homing
            else
            {
                NPC target = FindClosestNPC(600f);
                if (target != null)
                {
                    Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.08f);
                }
            }

            // Simple dust trail
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.GemAmethyst
                );
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            Time++;
        }

        // Simple homing helper
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
            Texture2D tex = ModContent.Request<Texture2D>("Terraria/Images/Projectile_88").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale;

            // Main vertical line
            Main.spriteBatch.Draw(
                tex,
                drawPos,
                null,
                Color.Purple * 0.7f,
                Projectile.rotation,
                tex.Size() / 2f,
                new Vector2(0.2f, 1f) * scale, // narrow width, tall height
                SpriteEffects.None,
                0f
            );

            // Horizontal line
            Main.spriteBatch.Draw(
                tex,
                drawPos,
                null,
                Color.Purple * 0.7f,
                Projectile.rotation,
                tex.Size() / 2f,
                new Vector2(1f, 0.2f) * scale, // wide width, short height
                SpriteEffects.None,
                0f
            );

            // Optional: add small glow or trail with alpha
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float trailScale = scale * (1f - i / (float)Projectile.oldPos.Length);

                // Vertical trail
                Main.spriteBatch.Draw(tex, pos, null, Color.Purple * 0.4f, Projectile.rotation, tex.Size() / 2f, new Vector2(0.2f, 1f) * trailScale, SpriteEffects.None, 0f);
                // Horizontal trail
                Main.spriteBatch.Draw(tex, pos, null, Color.Purple * 0.4f, Projectile.rotation, tex.Size() / 2f, new Vector2(1f, 0.2f) * trailScale, SpriteEffects.None, 0f);
            }

            return false; // skip default draw
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Simple debuff (replace later if you want custom)
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, velocity);
                dust.noGravity = true;
                dust.scale = 1.5f;
            }
        }
    }
}