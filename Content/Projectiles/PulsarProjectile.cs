using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;
using Neutronium.Content.Projectiles;
using Terraria.Audio;

namespace Neutronium.Content.Projectiles;

public class PulsarProjectile : ModProjectile
{
    public float rotationAngle = 0f; // Angle for the rotating beams
    public const float beamDistance = 50f; // Distance of the beams from the yoyo center

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.aiStyle = ProjAIStyleID.Yoyo; // Standard yoyo AI
        Projectile.friendly = true;
        Projectile.penetrate = -1; // Don't disappear after hitting enemies
        Projectile.timeLeft = 600; // How long the yoyo stays out
        Projectile.damage = 1;
        Projectile.localNPCHitCooldown = 50;

    }

    public override void AI()
    {
        // Add rotating beam logic
        rotationAngle += 0.1f; // Adjust the speed of rotation

        if (rotationAngle >= MathHelper.TwoPi)
        {
            rotationAngle = 0f; // Reset the rotation angle after a full rotation
        }

        EmitRotatingBeams(); // Emit the rotating beams from the yoyo center
        Lighting.AddLight(Projectile.Center, 0f, 0.3f, 0.6f);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        // Check if the NPC is currently in invincibility frames (i-frames)
        if (target.immune[Projectile.owner] > 0)
        {
            return; // Do not apply the buffs if the NPC is in i-frames
        }

        // Apply the buffs only if the NPC is not in i-frames
        target.AddBuff(BuffID.Electrified, 180);
        target.AddBuff(BuffID.Frostburn, 180);
    }

    private void EmitRotatingBeams()
    {
        // Calculate the positions for the rotating beams
        Vector2 beamOffset1 = new Vector2((float)Math.Cos(rotationAngle) * beamDistance, (float)Math.Sin(rotationAngle) * beamDistance);
        Vector2 beamOffset2 = new Vector2((float)Math.Cos(rotationAngle + MathHelper.Pi) * beamDistance, (float)Math.Sin(rotationAngle + MathHelper.Pi) * beamDistance);

        // Create two rotating beams
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + beamOffset1, Vector2.Zero, ModContent.ProjectileType<PulsarBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + beamOffset2, Vector2.Zero, ModContent.ProjectileType<PulsarBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        // Add dust between the center and the beams
        CreateDustConnection(Projectile.Center, Projectile.Center + beamOffset1);
        CreateDustConnection(Projectile.Center, Projectile.Center + beamOffset2);
    }

    /// <param name="start">The starting point of the dust line.</param>
    /// <param name="end">The ending point of the dust line.</param>
    private void CreateDustConnection(Vector2 start, Vector2 end)
    {
        int dustAmount = 10; // Number of dust particles along the line
        for (int i = 0; i <= dustAmount; i++)
        {
            float lerpFactor = i / (float)dustAmount; // Interpolates between 0 and 1
            Vector2 position = Vector2.Lerp(start, end, lerpFactor); // Linearly interpolate the position

            // Create a dust particle
            Dust dust = Dust.NewDustPerfect(position, DustID.Electric, Vector2.Zero, 150, new Color(0, 255, 255), 0.8f);
            dust.noGravity = true;  // Prevents the dust from falling
            dust.velocity = Vector2.Zero; // Ensures the dust stays stationary
            dust.noLight = true; // Prevents the dust from emitting light
            dust.fadeIn = -1f; // Makes the dust fade out instantly
            dust.scale *= 0.6f; // Reduces the dust's visual size
        }
    }

}
