using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class CrykalsChoirTrail : ModProjectile
    {
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";
        
        private const int TrailLength = 8; // how many old positions to store

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255; // invisible projectile itself
            Projectile.timeLeft = 10; // short-lived
            Projectile.ownerHitCheck = true; // important for melee
            Projectile.usesLocalNPCImmunity = true;

            // Initialize oldPos array manually
            Projectile.oldPos = new Vector2[TrailLength];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Neutronium/Content/Projectiles/CrykalsChoirTrail").Value;
            
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition;
                float alpha = (float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length;
                Color color = Color.Cyan * alpha;
                
                Main.spriteBatch.Draw(texture, drawPos, null, color, Projectile.rotation,
                    texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
            // Kill the trail when swing is done
            if (player.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            // Track blade tip instead of player center
            Projectile.Center = player.itemLocation;
            Projectile.rotation = player.itemRotation;

            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            
            Projectile.oldPos[0] = Projectile.Center;
            Projectile.friendly = false; // trail itself shouldn't hit anything
            Projectile.timeLeft = 2; // keep alive while swinging
        }
    }
}