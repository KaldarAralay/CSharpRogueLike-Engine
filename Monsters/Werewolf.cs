using RogueSharp.DiceNotation;
using RogueSharpSadConsoleSamples.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueSharpSadConsoleSamples.Monsters
{
    public class Werewolf : Monster
    {
        public static Werewolf Create(int level)
        {
            int health = Dice.Roll("2D5");
            return new Werewolf
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.KoboldColor,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Werewolf",
                Speed = 14,
                Symbol = 'W'
            };
        }
    }
}
