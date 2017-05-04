using RogueSharp.DiceNotation;
using RogueSharpSadConsoleSamples.Behaviors;
using RogueSharpSadConsoleSamples.Core;
using RogueSharpSadConsoleSamples.Systems;

namespace RogueSharpSadConsoleSamples.Monsters
{
   public class Imp : Monster
   {
      public static Imp Create( int level )
      {
         int health = Dice.Roll( "4D5" );
         return new Imp {
            Attack = Dice.Roll( "1D2" ) + level / 3,
            AttackChance = Dice.Roll( "10D5" ),
            Awareness = 10,
            Color = Colors.ImpColor,
            Defense = Dice.Roll( "1D2" ) + level / 3,
            DefenseChance = Dice.Roll( "10D4" ),
            Gold = Dice.Roll( "1D20" ),
            Health = health,
            MaxHealth = health,
            Name = "Imp",
            Speed = 14,
            Symbol = 'i'
         };
      }

      public override void PerformAction( CommandSystem commandSystem )
      {
         var splitOozeBehavior = new SplitOoze();
         if ( !splitOozeBehavior.Act( this, commandSystem ) )
         {
            base.PerformAction( commandSystem );
         }
      }
   }
}
