using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Neutronium.Content.Rarities
{
    public class NeutronTouched : ModRarity
    {
        public override Color RarityColor => new Color(180, 50, 255);

        public static Color BloomClr = new Color(80, 0, 120, 0);
        public static Color TextClr = new Color(180, 50, 255, 50);

        public static void Draw(Item item, SpriteBatch spriteBatch, string text, int X, int Y, Color textColor, Color lightColor, float rotation,
        Vector2 origin, Vector2 baseScale, float time, DynamicSpriteFont font)
        {
            var fontSize = font.MeasureString(text);

            textColor.A = 0;

            // Pulsing glow orbiting the text
            float pulsing = 1.5f + (float)Math.Sin(time * 5f);
            for (float f = 0f; f < MathHelper.TwoPi; f += 0.79f)
            {
                ChatManager.DrawColorCodedString(
                    spriteBatch, font, text,
                    new Vector2(X, Y) + new Vector2(pulsing, 0f).RotatedBy(f + time * 2f % MathHelper.TwoPi),
                    textColor * 0.5f, rotation, origin, baseScale);
            }

            textColor.A = 255;

            // Shadow
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, new Vector2(X, Y), textColor * 2f, rotation, origin, baseScale);

            // Dark outline
            ChatManager.DrawColorCodedString(spriteBatch, font, text, new Vector2(X, Y), new Color(40, 0, 60), rotation, origin, baseScale);

            // Shine sweep
            float shineWidth = 40f;
            float shineSpeed = 80f;
            float shineDisp = time * shineSpeed;
            float shinePos = shineDisp % (fontSize.X + shineWidth);

            float charOffsetX = 0f;
            for (int i = 0; i < text.Length; i++)
            {
                string c = text[i].ToString();
                Vector2 charSize = font.MeasureString(c);

                Vector2 charPos = new Vector2(X, Y) + new Vector2(charOffsetX, 0f);

                float centerX = charPos.X + (charSize.X * baseScale.X) / 2f + 10.5f;
                float dist = Math.Abs(centerX - (X + shinePos - shineWidth * 0.15f));
                float intensity = 1f - MathHelper.Clamp(dist / shineWidth, 0f, 1f);

                if (intensity > 0f)
                {
                    Color shineColor = new Color(220, 150, 255) * intensity * 1.5f;
                    ChatManager.DrawColorCodedString(
                        spriteBatch, font, c, charPos, shineColor, rotation, origin, baseScale);
                }

                charOffsetX += charSize.X - text.Length * 0.0085f;
            }
        }

        public static void Draw(Item item, string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale)
        {
            Draw(item, Main.spriteBatch, text, X, Y, TextClr, BloomClr, rotation, origin, baseScale,
                Main.GlobalTimeWrappedHourly, FontAssets.MouseText.Value);
        }

        public static void Draw(Item item, DrawableTooltipLine line)
        {
            Draw(item, line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale);
        }
    }
}