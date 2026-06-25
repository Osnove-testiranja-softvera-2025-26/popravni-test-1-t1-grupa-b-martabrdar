using NUnit.Framework;
using NUnit.Framework.Constraints;
using OTS2026_PT1_GrupaA;
using OTS2026_PT1_GrupaA.Exceptions;
using OTS2026_PT1_GrupaA.Models;
using System.ComponentModel;

namespace OTS2026_PT1_GrupaA.Test
{
    [TestFixture]
    public class GameTest
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game(new Position(10, 13), new Position(5, 6));
        }

        //BlackBox tehnika - klasa ekvivalencije(validna klasa)
        [Test]
        public void Game_ValidPlayerAndBoatPositions_GameCreated()
        {
            Game newGame = new Game(new Position(10, 13), new Position(5, 6));

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(10, 13)));
        }
        
        //BlackBox tehnika - klasa ekvivalencije(nevalidna klasa)
        [Test]
        public void Game_BoatPositionNotInLand_ThrowsException()
        {
            InvalidPlayerPositionException ex = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() =>
            {
                new Game(new Position(10, 13), new Position(5, 20));
            }));

            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
        }

        //BlackBox tehnika - klasa ekvivalencije(nevalidna klasa)
        [Test]
        public void Game_PlayerPositionNotInLand_ThrowsException()
        {
            InvalidPlayerPositionException ex = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() =>
            {
                new Game(new Position(5, 20), new Position(5, 6));
            }));

            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
        }

        // BlackBox tehnika - klase ekvivalencije i analiza granicnih vrednosti
        [TestCase(0, 0, true)]
        [TestCase(24, 12, true)]
        [TestCase(10, 13, true)]
        [TestCase(19, 19, true)]
        public void ValidatePosition_PositionInLand_ReturnsTrue(int x, int y, bool expected)
        {
            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.EqualTo(expected));
        }

        //BlackBox tehnika - analiza granicnij vrednosti
        [TestCase(-1, 0, false)]
        [TestCase(30, 0, false)]
        [TestCase(0, -1, false)]
        [TestCase(0, 30, false)]
        public void ValidatePosition_PositionOutsideMap_ReturnsFalse(int x, int y, bool expected)
        {
            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.EqualTo(expected));
        }

        //BlackBox tehnika - klase ekvivalencije (nevalidne)
        [TestCase(29, 0, false)]
        [TestCase(0, 13, false)]
        public void ValidatePosition_PositionInInvalidZone_ReturnsFalse(int x, int y, bool expected)
        {
            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.EqualTo(expected));
        }

        //BlackBox tehnika - klase ekvivalencije
        [Test]
        public void ValidatePosition_PositionInPondWithoutBoat_ReturnsFalse()
        {
            bool result = game.ValidatePosition(new Position(5, 20));

            Assert.That(result, Is.False);
        }

        //BlackBox tehnika - klase ekvivalencije
        [Test]
        public void ValidatePosition_PositionInPondWithBoat_ReturnsTrue()
        {
            game.Player.HasBoat = true;

            bool result = game.ValidatePosition(new Position(5, 20));

            Assert.That(result, Is.True);
        }

        //BlackBox tehnika - klase ekvivalencije
        [Test]
        public void ValidatePosition_NullPosition_ReturnsFalse()
        {
            bool result = game.ValidatePosition(null);

            Assert.That(result, Is.False);
        }

        [Test]
        public void MovePlayer_ValidRightMove_PlayerPositionChanged()
        {
            game.MovePlayer(Move.Right);

            Assert.That(game.Player.Position, Is.EqualTo(new Position(11, 13)));
        }

        [Test]
        public void MovePlayer_ValidateRightMove_PlayerPositionUnchanged()
        {
            Game newGame = new Game(new Position(0,0), new Position (5,6));

            newGame.MovePlayer(Move.Up);

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(0, 0)));
        }

        [Test]
        public void MovePlayer_MoveToPondWithoutBoat_PlayerPositionUnchanged()
        {
            Game newGame = new Game(new Position(10, 19), new Position(5, 6));

            newGame.MovePlayer(Move.Down);

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(10, 19)));
        }

        [Test]
        public void MovePlayer_MoveToPondWithBoat_PlayerPositionChanged()
        {
            Game newGame = new Game(new Position(10, 19), new Position(5, 6));
            newGame.Player.HasBoat = true;

            newGame.MovePlayer(Move.Down);

            Assert.That(newGame.Player.Position, Is.EqualTo(new Position(10, 20)));
        }

        [Test]
        public void ResolvePlayerPosition_PlayerOnBait_BaitIncreased()
        {
            game.Map.Fields[10, 13].Content = FieldContent.Bait;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfBait, Is.EqualTo(1));
        }

        [Test]
        public void ResolveplayerPosition_playerOnBait_FieldBecomesEmpty()
        {
            game.Map.Fields[10, 13].Content = FieldContent.Bait;

            game.ResolvePlayerPosition();

            Assert.That(game.Map.Fields[10, 13].Content, Is.EqualTo(FieldContent.Empty));
        }
      
        [Test]
        public void ResolvePlayerPosition_PlayerOnBoat_HasBoatBecomesTrue()
        {
            game.Map.Fields[10, 13].Content = FieldContent.Boat;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.HasBoat, Is.True);
        }

        [Test]
        public void ResolvePlayerPosition_PlayerOnFishWithBait_FishIncreased()
        {
            game.Player.Position = new Position(5, 20);
            game.Player.HasBoat = true;
            game.Player.AmountOfBait = 1;
            game.Map.Fields[5, 20].Content = FieldContent.Fish;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfFish, Is.EqualTo(1));
        }

        [Test]
        public void ResolvePlayerPosition_PlayerOnFishWithBait_BaitDecreased()
        {
            game.Player.Position = new Position(5, 20);
            game.Player.HasBoat = true;
            game.Player.AmountOfBait = 1;
            game.Map.Fields[5, 20].Content = FieldContent.Fish;

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfBait, Is.EqualTo(0));
        }

        //BlacBox tehnika - Pairwise testiranje (Pict)
        [TestCase(16, 0, false, Game.Score.Good)]
        [TestCase(15, 0, false, Game.Score.Bad)]
        [TestCase(9, 12, true, Game.Score.Good)]
        [TestCase(8, 12, true, Game.Score.Average)]
        [TestCase(8, 11, true, Game.Score.Bad)]
        [TestCase(8, 12, false, Game.Score.Bad)]

        public void CalculateIncome_ValidPlayerState_ReturnsExpectedScore(int amountOfFish, int amountOfBait,bool hasBoat, Game.Score expected)
        {
            game.Player.AmountOfFish = amountOfFish;
            game.Player.AmountOfBait = amountOfBait;
            game.Player.HasBoat = hasBoat;

            Game.Score result = game.CalculateIncome();
            Assert.That(result, Is.EqualTo(expected));
        }

    }
}