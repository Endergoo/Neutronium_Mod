using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Neutronium.Content.Players;   

namespace Neutronium.Content.Buffs
{
    public class CelestialRegen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // doesn’t save on exit
            Main.debuff[Type] = false; // it’s a positive buff
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Add a small heal over time effect
            // Optional: Use ModPlayer to track stacking
            var modPlayer = player.GetModPlayer<NeutroniumPlayer>();
            
            // Apply regeneration directly
            player.lifeRegen += (int)(modPlayer.celestialRegenStack * player.statLifeMax2 / 60f);
        }
    }
}