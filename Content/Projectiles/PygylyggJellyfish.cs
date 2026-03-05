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

    public override void PostAI()
        {
            // Only check the most recently added dust (more efficient)
            for (int i = Main.dust.Length - 1; i >= Math.Max(0, Main.dust.Length - 10); i--)
            {
                Dust dust = Main.dust[i];
                if (dust != null && dust.active && dust.type == DustID.Torch && 
                    dust.position.Between(Projectile.position, Projectile.position + Projectile.Size))
                {
                    dust.active = false;
                    break; // Found and removed the fire dust, can stop searching
                }
            }
            
            // Your blue effects
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                        DustID.BlueFairy, Projectile.velocity.X * 0.5f, 
                        Projectile.velocity.Y * 0.5f);
            Lighting.AddLight(Projectile.Center, new Vector3(0.0f, 0.5f, 1.5f));
        }
}