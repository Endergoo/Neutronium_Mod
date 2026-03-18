using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Core.GlobalItems
{
    public class GlowmaskGlobalItem : GlobalItem
    {
        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            // Only draw glow for items that have a registered glowmask
            if (item.ModItem is IGlowmaskItem glowItem)
            {
                Texture2D glowTex = glowItem.GlowTexture;
                if (glowTex == null) return;

                spriteBatch.Draw(
                    glowTex,
                    new Vector2(
                        item.position.X - Main.screenPosition.X + item.width / 2f,
                        item.position.Y - Main.screenPosition.Y + item.height / 2f),
                    null,
                    Color.White,
                    rotation,
                    glowTex.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}