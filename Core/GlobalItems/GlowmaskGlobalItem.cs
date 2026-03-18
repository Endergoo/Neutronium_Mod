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

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.ModItem is IGlowmaskItem glowItem)
            {
                Texture2D glowTex = glowItem.GlowTexture;
                if (glowTex == null) return;

                spriteBatch.Draw(glowTex, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}