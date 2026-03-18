using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class CrykalsChoirTrail : ModProjectile
    {
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
            Texture2D texture = Terraria.GameContent.TextureAssets.Item[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);

            // Draw the trail
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin;
                float alpha = (float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length;
                Color color = Color.Cyan * alpha;
                Main.spriteBatch.Draw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0f);
            }

            return false; // don't draw the invisible projectile itself
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter;
            Projectile.rotation = player.itemRotation;

            // Shift the old positions
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            Projectile.oldPos[0] = Projectile.Center;
        }
    }
}