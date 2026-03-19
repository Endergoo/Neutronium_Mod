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
            // --- Smooth sine rotation ---
            float sine = (float)Math.Sin(Time * 0.55f * Projectile.scale);
            Projectile.rotation = sine * 0.3f;

            // --- Early curved movement ---
            if (Time < 20)
            {
                Projectile.extraUpdates = 1; // smooth path
                Projectile.velocity = Projectile.velocity.RotatedBy(0.03f * Projectile.direction);
            }
            else
            {
                Projectile.extraUpdates = 0;

                // Simple homing
                NPC target = FindClosestNPC(600f);
                if (target != null)
                {
                    Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.08f);
                }
            }

            // --- Particle sparks ---
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
                dust.scale = 0.8f + Main.rand.NextFloat() * 0.4f;
                dust.color = mainColor * 0.75f;
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
            Texture2D texture = TextureAssets.Extra[98].Value; // glowing circle
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2f;

            // --- Layered trail ---
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float scale = Projectile.scale * (1f - i / (float)Projectile.oldPos.Length);

                // vertical bloom
                Main.spriteBatch.Draw(texture, pos, null, mainColor * 0.5f, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                // horizontal stretch
                Main.spriteBatch.Draw(texture, pos, null, mainColor * 0.5f, Projectile.rotation, origin, new Vector2(scale, scale * 0.25f), SpriteEffects.None, 0f);
            }

            // --- Main projectile draw ---
            Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, origin, new Vector2(Projectile.scale, Projectile.scale * 0.25f), SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
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