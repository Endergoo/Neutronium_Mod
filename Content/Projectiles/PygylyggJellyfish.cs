using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Neutronium.Content.Projectiles;
public class PygylyggJellyfish : ModProjectile
{
        public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 300;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.aiStyle = ProjAIStyleID.Bounce; // Keep the bounce AI
    }

    public override bool PreAI()
    {
        // Store the current dust count
        int previousDustCount = Main.dust.Count;
        
        // Let the bounce AI run
        return true;
    }

    public override void AI()
    {
        // After the bounce AI runs, remove any fire dust it created
        for (int i = 0; i < Main.dust.Count; i++)
        {
            Dust dust = Main.dust[i];
            // Check if this dust was just spawned by the bounce AI (Torch/fire dust)
            if (dust.active && dust.type == DustID.Torch && 
                dust.position.Between(Projectile.position, Projectile.position + Projectile.Size))
            {
                dust.active = false; // Remove the fire dust
            }
        }
        
        // Add your blue effects
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.BlueFairy, Projectile.velocity.X * 0.5f, 
                    Projectile.velocity.Y * 0.5f);
        Lighting.AddLight(Projectile.Center, new Vector3(0.0f, 0.5f, 1.5f));
    }
}