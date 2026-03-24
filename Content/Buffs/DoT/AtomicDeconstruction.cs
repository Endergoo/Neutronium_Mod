using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Buffs.DoT;

namespace Neutronium.Content.Buffs.DoT
{
    public class AtomicDeconstruction : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // DR
            npc.defense = (int)(npc.defense * 0.8f);

            // Damage over time — every 30 ticks (0.5 seconds)
           if (npc.buffTime[buffIndex] % 15 == 0)
            {
                npc.SimpleStrikeNPC(75, 0, false, 0f, DamageClass.Default, true);
            }

            // Dripping effect
            if (Main.rand.NextBool(3))
            {
                Dust drip = Dust.NewDustDirect(
                    npc.position,
                    npc.width,
                    npc.height,
                    DustID.GemAmethyst
                );
                drip.noGravity = false; // affected by gravity so it drips down
                drip.scale = Main.rand.NextFloat(0.6f, 1.2f);
                drip.color = new Color(180, 50, 255) with { A = 0 };
                drip.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(1f, 3f)); // falls downward
                drip.fadeIn = Main.rand.NextFloat(0.3f, 0.6f);
            }

            // Floating upward particles
            if (Main.rand.NextBool(4))
            {
                Dust float1 = Dust.NewDustDirect(
                    npc.position,
                    npc.width,
                    npc.height,
                    DustID.GemAmethyst
                );
                float1.noGravity = true;
                float1.scale = Main.rand.NextFloat(0.4f, 0.8f);
                float1.color = new Color(220, 100, 255) with { A = 0 };
                float1.velocity = new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-2f, -4f)); // floats upward
                float1.fadeIn = Main.rand.NextFloat(0.5f, 1f);
            }
            // Faint purple light on the enemy
            Lighting.AddLight(npc.Center, new Vector3(0.3f, 0f, 0.5f));
        }
    }
}