// Example of setting the flag when Moon Lord is defeated
using Terraria.ModLoader;
using Terraria;

public class NeutroniumWorldSystem : ModSystem
{
    public static bool MoonLordDefeated = false;

    public override void OnWorldLoad()
    {
        MoonLordDefeated = false; // Reset on world load
    }

    public override void OnWorldUnload()
    {
        MoonLordDefeated = false; // Reset on world unload
    }

    public override void PostUpdateWorld()
    {
        if (NPC.downedMoonlord)
        {
            MoonLordDefeated = true;
        }
    }
}