using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Neutronium
{
    public class GlobalPlayer : ModPlayer
    {
        public float StellarDamage = 0f;
    
    public override void ResetEffects()
        {
            StellarDamage = 0f;
        }
    }
}