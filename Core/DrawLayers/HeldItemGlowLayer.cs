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

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            drawInfo.drawPlayer.itemAnimation > 0 &&
            drawInfo.drawPlayer.HeldItem?.ModItem is IGlowmaskItem;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            if (player.HeldItem?.ModItem is not IGlowmaskItem glowItem)
                return;

            Texture2D glowTex = glowItem.GlowTexture;
            if (glowTex == null)
                return;

            // Copy exactly how vanilla draws the held item sprite
            SpriteEffects effects = player.direction == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            // This is the origin vanilla uses for swing-style items
            Vector2 origin = new Vector2(
                player.direction == 1 ? 0f : glowTex.Width,
                glowTex.Height);

            Vector2 position = new Vector2(
                (int)(player.itemLocation.X - Main.screenPosition.X),
                (int)(player.itemLocation.Y - Main.screenPosition.Y));

            DrawData drawData = new DrawData(
                glowTex,
                position,
                null,
                Color.White,
                player.itemRotation,
                origin,
                player.HeldItem.scale,
                effects,
                0);

            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}