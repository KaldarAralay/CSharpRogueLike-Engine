using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RogueSharp.Random;
using RogueSharpSadConsoleSamples.Core;
using RogueSharpSadConsoleSamples.Items;
using RogueSharpSadConsoleSamples.Systems;
using Console = SadConsole.Consoles.Console;
using RLNET;

namespace RogueSharpSadConsoleSamples
{
   public class RogueGame : Game
   {
      private readonly GraphicsDeviceManager _graphics;

      private static readonly int _screenWidth = 100;
      private static readonly int _screenHeight = 70;
      private static readonly int _mapWidth = 80;
      private static readonly int _mapHeight = 48;
      private static readonly int _messageWidth = 80;
      private static readonly int _messageHeight = 11;
      private static readonly int _statWidth = 20;
      private static readonly int _statHeight = 70;
      private static readonly int _inventoryWidth = 80;
      private static readonly int _inventoryHeight = 11;

      private static Console _mapConsole;
      private static Console _messageConsole;
      private static Console _statConsole;
      private static Console _inventoryConsole;
       

      public static int _mapLevel = 1;
      private static bool _renderRequired = true;

      private static InputState _inputState;

      public static Player Player { get; set; }
      public static DungeonMap DungeonMap { get; private set; }
      public static MessageLog MessageLog { get; private set; }
      public static MessageLog MapMessageLog { get; private set; }
      public static CommandSystem CommandSystem { get; private set; }
      public static SchedulingSystem SchedulingSystem { get; private set; }
      public static TargetingSystem TargetingSystem { get; private set; }
      public static IRandom Random { get; private set; }

      public RogueGame()
      {


         
         int seed = (int) DateTime.UtcNow.Ticks;
         Random = new DotNetRandom( seed );
         string consoleTitle = "Sean's Roguelike Engine v0.02 - Level 1 - seed " +seed ;



         MessageLog = new MessageLog();
            MapMessageLog = new MessageLog();
         MessageLog.Add( "The rogue arrives on level 1" );
         MessageLog.Add( $"Level created with seed '{seed}'" );
           
            MapMessageLog.Add("Welcome to Cult of Draconis");
            MapMessageLog.Add("Deep in the Caverns of Mar, an evil cult has succeeded");
            MapMessageLog.Add("in summoning the ancient dragonlord Draconis.");
            MapMessageLog.Add("It is up to you to fight your way through the caverns,");
            MapMessageLog.Add("Destroy the 4 power stones and kill Draconis");
            MapMessageLog.Add("The power stones and Draconis reside on level 5.");
            MapMessageLog.Add("To descend stairs ' > ' press the period key ' . '");
           


            Player = new Player();
         SchedulingSystem = new SchedulingSystem();

         MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 20, 13, 7, _mapLevel );
         DungeonMap = mapGenerator.CreateMap();

         CommandSystem = new CommandSystem();
         TargetingSystem = new TargetingSystem();

         Player.Item1 = new RevealMapScroll();
         Player.Item2 = new RevealMapScroll();

         _inputState = new InputState();

         _graphics = new GraphicsDeviceManager( this );
         this.Window.Title = consoleTitle;

         Content.RootDirectory = "Content";
         var sadConsoleComponent = new SadConsole.EngineGameComponent( this, () => {
            using ( var stream = System.IO.File.OpenRead( "Fonts/Cheepicus12.font" ) )
               SadConsole.Engine.DefaultFont = SadConsole.Serializer.Deserialize<SadConsole.Font>( stream );

            SadConsole.Engine.DefaultFont.ResizeGraphicsDeviceManager( _graphics, _screenWidth, _screenHeight, 0, 0 );
            SadConsole.Engine.UseMouse = true;
            SadConsole.Engine.UseKeyboard = true;

            _mapConsole = new Console( _mapWidth, _mapHeight );
            _messageConsole = new Console( _messageWidth, _messageHeight );
            _statConsole = new Console( _statWidth, _statHeight );
            _inventoryConsole = new Console( _inventoryWidth, _inventoryHeight );

            _mapConsole.Position = new Point( 0, _inventoryHeight );
            _messageConsole.Position = new Point( 0, _screenHeight - _messageHeight );
            _statConsole.Position = new Point( _mapWidth, 0 );
            _inventoryConsole.Position = new Point( 0, 0 );


             SadConsole.Engine.ConsoleRenderStack.Add( _mapConsole );
            SadConsole.Engine.ConsoleRenderStack.Add( _messageConsole );
            SadConsole.Engine.ConsoleRenderStack.Add( _statConsole );
            SadConsole.Engine.ConsoleRenderStack.Add( _inventoryConsole );

            SadConsole.Engine.ActiveConsole = _mapConsole;
         } );

         Components.Add( sadConsoleComponent );
      }

      protected override void Initialize()
      {
         IsMouseVisible = true;

         base.Initialize();
      }


      protected override void Update( GameTime gameTime )
      {
         bool didPlayerAct = false;
         _inputState.Update( gameTime );

         if ( TargetingSystem.IsPlayerTargeting )
         {
            _renderRequired = true;
            TargetingSystem.HandleInput( _inputState );
         }
         else if ( CommandSystem.IsPlayerTurn )
         {
            if ( _inputState.IsKeyPressed( Keys.Up ) )
            {
                    MapMessageLog.Add("");
                    didPlayerAct = CommandSystem.MovePlayer( Direction.Up );
            }
            else if ( _inputState.IsKeyPressed( Keys.Down ) )
            {
                    MapMessageLog.Add("");
                    didPlayerAct = CommandSystem.MovePlayer( Direction.Down );
            }
            else if ( _inputState.IsKeyPressed( Keys.Left ) )
            {
               didPlayerAct = CommandSystem.MovePlayer( Direction.Left );
            }
            else if ( _inputState.IsKeyPressed( Keys.Right ) )
            {
               didPlayerAct = CommandSystem.MovePlayer( Direction.Right );
            }
            else if ( _inputState.IsKeyPressed( Keys.Escape ) )
            {
               this.Exit();
            }
            else if ( _inputState.IsKeyPressed( Keys.OemPeriod ) )
            {
               if ( DungeonMap.CanMoveDownToNextLevel() )
               {
                  MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel );
                  DungeonMap = mapGenerator.CreateMap();
                  MessageLog = new MessageLog();
                  CommandSystem = new CommandSystem();
                  this.Window.Title = $"Sean's RogueLike Engine 3 - Level {_mapLevel}";
                  didPlayerAct = true;
               }
            }
            else
            {
               didPlayerAct = CommandSystem.HandleInput( _inputState );
            }

            if ( didPlayerAct )
            {
               _renderRequired = true;
               CommandSystem.EndPlayerTurn();
            }
         }
         else
         {
            CommandSystem.ActivateMonsters();
            _renderRequired = true;
         }
         base.Update( gameTime );
      }



      protected override void Draw( GameTime gameTime )
      {
         if ( _renderRequired )
         {
            GraphicsDevice.Clear( Color.Black );

            _mapConsole.CellData.Clear();
            _messageConsole.CellData.Clear();
            _statConsole.CellData.Clear();
            _inventoryConsole.CellData.Clear();

            MessageLog.Draw( _messageConsole );
                 
            DungeonMap.Draw( _mapConsole, _statConsole, _inventoryConsole );
                MapMessageLog.Draw( _mapConsole );
            MessageLog.Draw( _messageConsole );
            TargetingSystem.Draw( _mapConsole );


                _inventoryConsole.CellData.Print(0, 10, "________________________________________________________________________________", Color.Green);

                _messageConsole.CellData.Print(0, 0, "--------------------------------------------------------------------------------", Color.Green);

                _statConsole.CellData.Print(0, 0, "|", Color.Green);
                _statConsole.CellData.Print(0, 1, "|", Color.Green);
                _statConsole.CellData.Print(0, 2, "|", Color.Green);
                _statConsole.CellData.Print(0, 3, "|", Color.Green);
                _statConsole.CellData.Print(0, 4, "|", Color.Green);
                _statConsole.CellData.Print(0, 5, "|", Color.Green);
                _statConsole.CellData.Print(0, 6, "|", Color.Green);
                _statConsole.CellData.Print(0, 7, "|", Color.Green);
                _statConsole.CellData.Print(0, 8, "|", Color.Green);
                _statConsole.CellData.Print(0, 9, "|", Color.Green);
                _statConsole.CellData.Print(0, 10, "|", Color.Green);
                _statConsole.CellData.Print(0, 11, "|", Color.Green);
                _statConsole.CellData.Print(0, 12, "|", Color.Green);
                _statConsole.CellData.Print(0, 13, "|", Color.Green);
                _statConsole.CellData.Print(0, 14, "|", Color.Green);
                _statConsole.CellData.Print(0, 15, "|", Color.Green);
                _statConsole.CellData.Print(0, 16, "|", Color.Green);
                _statConsole.CellData.Print(0, 17, "|", Color.Green);
                _statConsole.CellData.Print(0, 18, "|", Color.Green);
                _statConsole.CellData.Print(0, 19, "|", Color.Green);
                _statConsole.CellData.Print(0, 20, "|", Color.Green);
                _statConsole.CellData.Print(0, 21, "|", Color.Green);
                _statConsole.CellData.Print(0, 22, "|", Color.Green);
                _statConsole.CellData.Print(0, 23, "|", Color.Green);
                _statConsole.CellData.Print(0, 24, "|", Color.Green);
                _statConsole.CellData.Print(0, 25, "|", Color.Green);
                _statConsole.CellData.Print(0, 26, "|", Color.Green);
                _statConsole.CellData.Print(0, 27, "|", Color.Green);
                _statConsole.CellData.Print(0, 28, "|", Color.Green);
                _statConsole.CellData.Print(0, 29, "|", Color.Green);
                _statConsole.CellData.Print(0, 30, "|", Color.Green);
                _statConsole.CellData.Print(0, 31, "|", Color.Green);
                _statConsole.CellData.Print(0, 32, "|", Color.Green);
                _statConsole.CellData.Print(0, 33, "|", Color.Green);
                _statConsole.CellData.Print(0, 34, "|", Color.Green);
                _statConsole.CellData.Print(0, 35, "|", Color.Green);
                _statConsole.CellData.Print(0, 36, "|", Color.Green);
                _statConsole.CellData.Print(0, 37, "|", Color.Green);
                _statConsole.CellData.Print(0, 38, "|", Color.Green);
                _statConsole.CellData.Print(0, 39, "|", Color.Green);
                _statConsole.CellData.Print(0, 40, "|", Color.Green);
                _statConsole.CellData.Print(0, 41, "|", Color.Green);
                _statConsole.CellData.Print(0, 42, "|", Color.Green);
                _statConsole.CellData.Print(0, 43, "|", Color.Green);
                _statConsole.CellData.Print(0, 44, "|", Color.Green);
                _statConsole.CellData.Print(0, 45, "|", Color.Green);
                _statConsole.CellData.Print(0, 46, "|", Color.Green);
                _statConsole.CellData.Print(0, 47, "|", Color.Green);
                _statConsole.CellData.Print(0, 48, "|", Color.Green);
                _statConsole.CellData.Print(0, 49, "|", Color.Green);
                _statConsole.CellData.Print(0, 50, "|", Color.Green);
                _statConsole.CellData.Print(0, 51, "|", Color.Green);
                _statConsole.CellData.Print(0, 52, "|", Color.Green);
                _statConsole.CellData.Print(0, 53, "|", Color.Green);
                _statConsole.CellData.Print(0, 54, "|", Color.Green);
                _statConsole.CellData.Print(0, 55, "|", Color.Green);
                _statConsole.CellData.Print(0, 56, "|", Color.Green);
                _statConsole.CellData.Print(0, 57, "|", Color.Green);
                _statConsole.CellData.Print(0, 58, "|", Color.Green);
                _statConsole.CellData.Print(0, 59, "|", Color.Green);

                _statConsole.CellData.Print(0, 1, "|", Color.Green);

                if (RogueGame._mapLevel == 1) { 
                _statConsole.CellData.Print(1, 60, "Location:", Color.Green);
                    _statConsole.CellData.Print(1, 62, "Desert Pass", Color.Tan);



                }else if (RogueGame._mapLevel == 2)
                {
                    _statConsole.CellData.Print(1, 60, "Location:", Color.Green);
                    _statConsole.CellData.Print(1, 62, "Cavern Base Level 1", Color.Red);



                }
                else if (RogueGame._mapLevel == 3)
                {
                    _statConsole.CellData.Print(1, 60, "Location:", Color.Green);
                    _statConsole.CellData.Print(1, 62, "Cavern Base Level 2", Color.Red);



                }
                else if (RogueGame._mapLevel == 4)
                {
                    _statConsole.CellData.Print(1, 60, "Location:", Color.Green);
                    _statConsole.CellData.Print(1, 62, "Cavern Base Level 3", Color.Red);



                }
                else if (RogueGame._mapLevel == 5)
                {
                    _statConsole.CellData.Print(1, 60, "Location:", Color.Green);
                    _statConsole.CellData.Print(1, 62, "Summoning Chambers", Color.Red);



                }



                base.Draw( gameTime );
         }
      }
   }
}
