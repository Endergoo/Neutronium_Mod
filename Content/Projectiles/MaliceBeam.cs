using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Terraria.Audio;

namespace Neutronium.Content.Projectiles
{
    public class MaliceBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public ref float BeamLength => ref Projectile.ai[0];
        public ref float FadeTimer => ref Projectile.ai[1];

        private const float FadeDuration = 20f;
        private bool exploded = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)FadeDuration;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // only hit once
        }

        public override void AI()
        {
            FadeTimer++;
            Projectile.velocity = Vector2.Zero;

            // Spawn explosion on first frame
            if (FadeTimer == 1f && !exploded)
            {
                exploded = true;
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                SpawnExplosion(Projectile.Center + dir * BeamLength);
            }

            // Red light along beam
            float fadeProgress = 1f - FadeTimer / FadeDuration;
            Vector2 beamDir = Projectile.rotation.ToRotationVector2();
            for (float i = 0; i < BeamLength; i += 80f)
                Lighting.AddLight(Projectile.Center + beamDir * i, fadeProgress * 1.5f, 0f, 0f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Only deal damage on first frame
            if (FadeTimer > 1f)
                return false;

            float collisionPoint = 0f;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + dir * BeamLength,
                20f,
                ref collisionPoint
            );
        }

        private void SpawnExplosion(Vector2 position)
        {

            // Screen shake
            PunchCameraModifier punch = new PunchCameraModifier(
                position, Main.rand.NextVector2Unit(), 8f, 10f, 20, 1000f);
            Main.instance.CameraModifiers.Add(punch);

            // Spawn explosion visual projectile
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                position,
                Vector2.Zero,
                ModContent.ProjectileType<MaliceExplosion>(),
                0,
                0f,
                Projectile.owner
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Neutronium/Content/Particles/BloomLineThick").Value;
            float fadeProgress = 1f - FadeTimer / FadeDuration;

            Color beamColor = new Color(255, 30, 30) with { A = 0 };
            Color coreColor = new Color(255, 200, 200) with { A = 0 };

            Vector2 beamStart = Projectile.Center - Main.screenPosition;

            // Outer glow
            Main.EntitySpriteDraw(
                texture,
                beamStart,
                null,
                beamColor * 0.35f * fadeProgress,
                Projectile.rotation + MathHelper.PiOver2,
                new Vector2(texture.Width / 2f, texture.Height),
                new Vector2(0.06f, BeamLength / texture.Height),
                SpriteEffects.None,
                0);

            // Bright core
            Main.EntitySpriteDraw(
                texture,
                beamStart,
                null,
                coreColor * fadeProgress,
                Projectile.rotation + MathHelper.PiOver2,
                new Vector2(texture.Width / 2f, texture.Height),
                new Vector2(0.02f, BeamLength / texture.Height),
                SpriteEffects.None,
                0);

            return false;
        }
    }
}