using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

public class MLDT : ModSystem
{
    public override void Load()
    {
        On_NPC.NPCLoot += OnNPCLoot;
    }

    public override void Unload()
    {
        On_NPC.NPCLoot -= OnNPCLoot;
    }

    private void OnNPCLoot(On_NPC.orig_NPCLoot orig, NPC npc)
    {
        orig(npc);

        if (npc.type == NPCID.MoonLordCore)
        {
            NeutroniumWorldSystem.MoonLordDefeated = true;

            // Call the refactored ore generation method.
            NeutroniumOreSystem.GenerateOre();

            // Optional: Notify the player that the ore has been generated.
            Main.NewText("Neutrons have begun bambarding the planet...", Color.Purple);
        }
    }
}