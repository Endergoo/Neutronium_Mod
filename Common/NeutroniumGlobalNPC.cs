using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Neutronium.Content.Buffs;
using Neutronium.Content.Buffs.DoT;


public class NeutroniumGlobalNPC : GlobalNPC
{
    private const float DebuffIconScale = 0.5f;
    private const float DebuffIconOffset = 16f;

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (npc.HasBuff(ModContent.BuffType<AtomicDeconstruction>()))
        {
            Texture2D icon = ModContent.Request<Texture2D>("Neutronium/Content/Buffs/DoT/AtomicDeconstruction").Value;
            Vector2 drawPos = npc.Top - screenPos - new Vector2(icon.Width * DebuffIconScale / 2f, icon.Height * DebuffIconScale + DebuffIconOffset);
            spriteBatch.Draw(icon, drawPos, null, Color.White, 0f, Vector2.Zero, DebuffIconScale, SpriteEffects.None, 0f);
        }
    }














    //Loot
     public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Corruptor)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorruptorMass>(), 30)); // 1 in 30
            }
}