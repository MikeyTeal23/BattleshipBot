using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Battleships.Player.Interface;
using static BattleshipBot.Direction;

namespace BattleshipBot
{
    public class MyBot : IBattleshipsBot
    {
        private IGridSquare lastTarget;
        public Position lastTargetPosition;
        private int seed;
        public int minShipLengthLeft;
        public List<GridSquare> PrevTargets = new List<GridSquare>();
        public List<Position> PossibleTargets = new List<Position>();
        public List<Position> DiagonalPositionList = new List<Position>();
        public Dictionary<Position, PositionStatus> GameHistory = new Dictionary<Position, PositionStatus>();

        public List<Position> ACStart = new List<Position>();
        public List<Position> BStart = new List<Position>();
        public List<Position> DStart = new List<Position>();
        public List<Position> SStart = new List<Position>();
        public List<Position> PBStart = new List<Position>();

        public List<int> LengthList = new List<int> { 5, 4, 3, 3, 2 };

        public Random RandomSeed = new Random();

        public IEnumerable<IShipPosition> GetShipPositions()
        {
            minShipLengthLeft = 2;
            lastTarget = null; // Forget all our history when we start a new game
            lastTargetPosition = null;

            BoardHelper boardHelper = new BoardHelper();

            PrevTargets.Clear();
            PossibleTargets.Clear();

            GameHistory = boardHelper.GenerateNewPositionHistory();
            DiagonalPositionList = boardHelper.GenerateDiagonalBoard();

            ACStart = boardHelper.GeneratePossibleShipPositions(5);
            BStart = boardHelper.GeneratePossibleShipPositions(4);
            DStart = boardHelper.GeneratePossibleShipPositions(3);
            SStart = boardHelper.GeneratePossibleShipPositions(3);
            PBStart = boardHelper.GeneratePossibleShipPositions(2);

            Random random = new Random();

            List<List<Position>> startList = new List<List<Position>> { ACStart, BStart, DStart, SStart, PBStart };

            List<IShipPosition> shipsList = new List<IShipPosition>();

            List<Position> blockedArea = new List<Position>();

            List<Position> possibleShipArea = new List<Position>();

            List<Tuple<Position, Directions>> possibleStartPositions = new List<Tuple<Position, Directions>>();

            for (int i = 0; i < 5; i++)
            {
                foreach (var position in blockedArea)
                {
                    if (startList[i].Contains(position))
                    {
                        position.RemoveFromList(startList[i]);
                    }
                }

                foreach (var position in startList[i])
                {
                    foreach (var direction in (Directions[])Enum.GetValues(typeof(Directions)))
                    {
                        possibleShipArea = boardHelper.CreateShipArea(position, direction, LengthList[i]);

                        if (!possibleShipArea.Intersect(blockedArea).Any())
                        {
                            possibleStartPositions.Add(new Tuple<Position, Directions>(position, direction));
                        }
                    }
                }

                int choice = random.Next(0, possibleStartPositions.Count);
                Position chosenPosition = possibleStartPositions[choice].Item1;
                Directions dir = possibleStartPositions[choice].Item2;
                shipsList.Add(GetShipPosition(chosenPosition, dir, LengthList[i]));

                blockedArea.AddRange(boardHelper.CreateBlockedArea(chosenPosition, dir, LengthList[i]));

                possibleStartPositions.Clear();
            }
            return shipsList;
        }

        private static ShipPosition GetShipPosition(Position position, Directions direction, int length)
        {
            char startRow, endRow;
            int startColumn, endColumn;

            startRow = position.Row;
            startColumn = position.Column;

            if (direction == Directions.Horizontal)
            {
                endRow = startRow;
                endColumn = startColumn + (length - 1);
            }
            else
            {
                endRow = (char)(startRow + (char)(length - 1));
                endColumn = startColumn;
            }
            return new ShipPosition(new GridSquare(startRow, startColumn + 1), new GridSquare(endRow, endColumn + 1));
        }

        public IGridSquare SelectTarget()
        {
            var nextTarget = GetNextTarget();
            lastTarget = nextTarget;
            return nextTarget;
        }

        private IGridSquare GetNextTarget()
        {
            seed++;

            if (PossibleTargets.Count > 0)
            {
                // THIS RUNS IF THE BOT HAS FOUND A SHIP
                var gridChoice = PossibleTargets[0];
                gridChoice.RemoveFromList(DiagonalPositionList);
                PossibleTargets.RemoveAt(0);

                return new GridSquare(gridChoice.Row, gridChoice.Column + 1);
            }
            else
            {
                // THIS RUNS IF THE BOT IS NOT HUNTING DOWN A SHIP, SO IT IS SAFE TO 
                // RUN CheckBoardForNewImpossiblePositions
                ShipLocatorHelper shipLocatorHelper = new ShipLocatorHelper();
                minShipLengthLeft = shipLocatorHelper.GetMinimumShipSizeLeft(GameHistory);

                PositionAvailabilityHelper positionAvailability = new PositionAvailabilityHelper();
                GameHistory = positionAvailability.CheckBoardForNewImpossiblePositions(GameHistory, minShipLengthLeft);
                DiagonalPositionList = positionAvailability.UpdateDiagonalList(GameHistory, DiagonalPositionList);

                var random = new Random(RandomSeed.Next(1, 10000));
                var choice = random.Next(0, DiagonalPositionList.Count);
                var gridChoice = DiagonalPositionList[choice];
                gridChoice.RemoveFromList(DiagonalPositionList);
                return new GridSquare(gridChoice.Row, gridChoice.Column + 1);
            }
        }

        public void HandleShotResult(IGridSquare square, bool wasHit)
        {
            var col = square.Column - 1; //taken 1 off col so 10 passes as 9 and is one element in string
            var row = square.Row;        //and so it can match with the grid list format

            var position = new Position(col, row);
            if (wasHit)
            {
                GameHistory[position] = PositionStatus.Hit;

                List<Position> possibleChoicesList = position.AdjacentSquares();

                List<Position> notPossibleChoicesList = position.DiagonalSquares();

                foreach (var choice in possibleChoicesList)
                {
                    if (GameHistory[choice] == PositionStatus.Available)
                    {
                        PossibleTargets.Add(choice);
                    }
                }

                foreach (var notChoice in notPossibleChoicesList)
                {
                    GameHistory[notChoice] = PositionStatus.Impossible;
                    notChoice.RemoveFromList(DiagonalPositionList);
                    notChoice.RemoveFromList(PossibleTargets);
                }
            }
            else
            {
                GameHistory[position] = PositionStatus.Miss;
            }
        }

        public void HandleOpponentsShot(IGridSquare square)
        {
            // Ignore what our opponent does
        }

        public string Name => "TestBot3002";
    }
}