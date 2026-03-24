using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Neutronium.Content.Buffs;
using Neutronium.Content.Buffs.DoT;


namespace Neutronium.Common
{
    public class NeutroniumGlobalNPC : GlobalNPC
    {
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.HasBuff(ModContent.BuffType<AtomicDeconstruction>()))
            {
                Texture2D icon = ModContent.Request<Texture2D>("Neutronium/Content/Buffs/DoT/AtomicDeconstruction").Value;
                Vector2 drawPos = npc.Top - screenPos - new Vector2(icon.Width / 2f, icon.Height + 4f);
                spriteBatch.Draw(icon, drawPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}