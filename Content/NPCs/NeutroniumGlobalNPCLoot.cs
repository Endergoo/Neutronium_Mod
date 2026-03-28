using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Neutronium.Content.Items.Accessories;

namespace Neutronium.Content.NPCs
{
    public class NeutroniumGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Corruptor)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorruptorMass>(), 20)); // 1 in 20
            }

            // Add more drops here as you make more items
            // if (npc.type == NPCID.WhateverEnemy)
            // {
            //     npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<YourItem>(), 10));
            // }
        }
    }
}