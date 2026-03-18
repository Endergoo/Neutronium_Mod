using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Neutronium.Core.Utils;

namespace Neutronium.Core.GlobalItems
{
    public class GlowmaskGlobalItem : GlobalItem
    {
        // PostDrawInWorld removed — each item handles its own ground glow via PostDrawInWorld in the item class

       public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
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
                    new Vector2(glowTex.Width / 2f, glowTex.Height / 2f),
                    scale,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}