using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Neutronium.Core.Utils
{
    public static class DrawingUtils
    {
        public static void DrawItemGlowmaskSingleFrame(this Item item, SpriteBatch spriteBatch, float rotation, Texture2D glowTexture)
        {
            spriteBatch.Draw(
                glowTexture,
                new Vector2(
                    item.position.X - Main.screenPosition.X + item.width / 2f,
                    item.position.Y - Main.screenPosition.Y + item.height / 2f),
                null,
                Color.White,
                rotation,
                glowTexture.Size() / 2f,
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}