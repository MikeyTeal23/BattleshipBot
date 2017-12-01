using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleshipBot.Direction;

namespace BattleshipBot
{
    class ShipLocatorHelper
    {
        public int GetMinimumShipSizeLeft(Dictionary<Position, PositionStatus> gameHistory)
        {
            List<int> shipSizeList = new List<int>();

            foreach (var position in gameHistory.Keys)
            {
                if (position.IsUpperLeftCornerOfShip(gameHistory))
                {
                    shipSizeList.Add(GetShipLength(position, gameHistory));
                }
            }

            if (!shipSizeList.Contains(2)) { return 2; }
            if (!shipSizeList.Contains(3)) { return 3; }
            if (shipSizeList.Contains(3))
            {
                shipSizeList.Remove(3);
                if (!shipSizeList.Contains(3)) { return 3; }
            }
            if (!shipSizeList.Contains(4)) { return 4; }
            return 5;
        }

        public int GetShipLength(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            var counter = 0;
            var direction = position.GetDirectionOfAdjacentHit(positionDict);

            if (direction == Directions.Vertical)
            {
                while (positionDict[position] == PositionStatus.Hit)
                {
                    position = position.PositionBelow();
                    counter++;
                    if (!position.IsOnBoard()) { break; }
                }
            }
            else
            {
                while (positionDict[position] == PositionStatus.Hit)
                {
                    position = position.PositionRight();
                    counter++;
                    if (!position.IsOnBoard()) { break; }
                }
            }
            return counter;
        }
    }
}
