using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Neutronium.Core.Utils;

namespace Neutronium.Core.DrawLayers
{
    public class HeldItemGlowLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() =>
            new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.HeldItem?.ModItem is not IGlowmaskItem glowItem)
                return;

            Texture2D glowTex = glowItem.GlowTexture;
            if (glowTex == null)
                return;

            Vector2 position = player.itemLocation - Main.screenPosition
                + new Vector2(player.HeldItem.width / 2f, player.HeldItem.height / 2f);

            DrawData drawData = new DrawData(
                glowTex,
                position,
                null,
                Color.White,
                player.itemRotation,
                glowTex.Size() / 2f,
                player.HeldItem.scale,
                player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);

            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}