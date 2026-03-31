using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class MaliceBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        // ai[0] = beam length (set on spawn)
        // ai[1] = fade timer
        public ref float BeamLength => ref Projectile.ai[0];
        public ref float FadeTimer => ref Projectile.ai[1];

        private const float MaxBeamLength = 2400f;
        private const int FadeDuration = 20;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = FadeDuration;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            FadeTimer++;

            // Keep beam at spawn position — it doesn't move
            Projectile.velocity = Vector2.Zero;

            // Add red light along the beam
            float fadeProgress = 1f - FadeTimer / FadeDuration;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (float i = 0; i < BeamLength; i += 60f)
            {
                Lighting.AddLight(Projectile.Center + dir * i, fadeProgress * 1.5f, 0f, 0f);
            }

            // Beam dust particles while fading
            if (Main.rand.NextBool(2))
            {
                float randDist = Main.rand.NextFloat(BeamLength);
                Vector2 dustPos = Projectile.Center + dir * randDist;
                Dust dust = Dust.NewDustDirect(dustPos, 1, 1, DustID.RedTorch);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 1.2f) * fadeProgress;
                dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
                dust.color = new Color(255, 50, 50) with { A = 0 };
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            // Only hit NPCs that are along the beam line
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 beamEnd = Projectile.Center + dir * BeamLength;
            float _ = float.NaN;
            bool onBeam = Collision.CheckAABBvLineCollision(
                target.Hitbox.TopLeft(), target.Hitbox.Size(),
                Projectile.Center, beamEnd, 8f, ref _);
            return onBeam ? null : false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Neutronium/Content/Particles/BloomLineThick").Value;
            float fadeProgress = 1f - FadeTimer / FadeDuration;
            Vector2 dir = Projectile.rotation.ToRotationVector2();

            Color beamColor = new Color(255, 30, 30) with { A = 0 };
            Color coreColor = new Color(255, 200, 200) with { A = 0 };

            Vector2 beamStart = Projectile.Center - Main.screenPosition;

            // Outer glow — wide and soft
            Main.EntitySpriteDraw(
                texture,
                beamStart,
                null,
                beamColor * 0.35f * fadeProgress,
                Projectile.rotation + MathHelper.PiOver2,
                new Vector2(texture.Width / 2f, 0f),
                new Vector2(0.08f, BeamLength / texture.Height) * 1f,
                SpriteEffects.None,
                0);

            // Bright core — slim
            Main.EntitySpriteDraw(
                texture,
                beamStart,
                null,
                coreColor * fadeProgress,
                Projectile.rotation + MathHelper.PiOver2,
                new Vector2(texture.Width / 2f, 0f),
                new Vector2(0.03f, BeamLength / texture.Height) * 1f,
                SpriteEffects.None,
                0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnExplosion(target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            // Explosion at beam end if no enemy was hit
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            SpawnExplosion(Projectile.Center + dir * BeamLength);
        }

        private void SpawnExplosion(Vector2 position)
        {
            // Screen shake
            Terraria.Graphics.CameraModifiers.PunchCameraModifier punch =
                new Terraria.Graphics.CameraModifiers.PunchCameraModifier(
                    position, Main.rand.NextVector2Unit(), 8f, 10f, 20, 1000f);
            Main.instance.CameraModifiers.Add(punch);

            // Explosion dust burst
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.RedTorch);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1f, 2.5f);
                dust.velocity = Main.rand.NextVector2Circular(12f, 12f);
                dust.color = new Color(255, 50, 50) with { A = 0 };
            }

            // Bright flash dust
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.RedTorch);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 1.5f);
                dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                dust.color = Color.White with { A = 0 };
            }

            // Explosion light
            Lighting.AddLight(position, 3f, 0.5f, 0.5f);
        }
    }
}