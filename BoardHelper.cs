using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleshipBot.Direction;

namespace BattleshipBot
{
    class BoardHelper
    {
        public Dictionary<Position, PositionStatus> GenerateNewPositionHistory()
        {
            Dictionary<Position, PositionStatus> positionDict = new Dictionary<Position, PositionStatus>();
            var positionList = GenerateNewBoard();

            foreach(var position in positionList)
            {
                positionDict.Add(position, PositionStatus.Available);
            }
            return positionDict;
        }

        public List<Position> GenerateNewBoard()
        {
            List<Position> PositionList = new List<Position>();

            for (char c = 'A'; c < 'K'; c++)
            {
                for (int i = 0; i < 10; i++)
                {
                    PositionList.Add(new Position(i, c));
                }
            }
            return PositionList;
        }

        public List<Position> GenerateDiagonalBoard()
        {
            List<Position> PositionList = GenerateNewBoard().Where(p => (p.Column + (int)p.Row) % 2 == 0).ToList();
            return PositionList;
        }

        public List<Position> GeneratePossibleShipPositions(int length)
        {
            List<Position> PositionList = new List<Position>();

            for (char c = 'A'; c < (char)('K' - length); c++)
            {
                for (int i = 0; i < 10 - length; i++)
                {
                    PositionList.Add(new Position(i, c));
                }
            }
            return PositionList;
        }

        public List<Position> CreateBlockedArea(Position position, Directions dir, int length)
        {
            List<Position> shipArea = CreateShipArea(position, dir, length);
            List<Position> blockedArea = new List<Position>();

            foreach (var p in shipArea)
            {
                blockedArea.AddRange(p.DiagonalSquares());
                blockedArea.AddRange(p.AdjacentSquares());
            }

            return blockedArea;
        }

        public List<Position> CreateShipArea(Position position, Directions dir, int length)
        {
            List<Position> positionList = new List<Position>();

            if (dir == Directions.Horizontal)
            {
                for (int i = 0; i < length; i++)
                {
                    positionList.Add(position.ShiftPositionRight(i));
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    positionList.Add(position.ShiftPositionDown(i));
                }
            }

            return positionList;
        }
    }
}
