using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipBot
{
    class PositionAvailabilityHelper
    {
        public Dictionary<Position, PositionStatus> CheckBoardForNewImpossiblePositions(Dictionary<Position, PositionStatus> positionDict, int minShipSize)
        {
            foreach (var position in positionDict.Keys.ToList())
            {
                if (GetNumberOfAvailableHorizontalSpaces(position, positionDict) < minShipSize &&
                    GetNumberOfAvailableVerticalSpaces(position, positionDict) < minShipSize &&
                    positionDict[position] == PositionStatus.Available)
                {
                    positionDict[position] = PositionStatus.Impossible;
                }
            }
            return positionDict;
        }
        
        public int GetNumberOfAvailableHorizontalSpaces(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            return GetNumberOfAvailableSpacesLeft(position, positionDict)
                    + GetNumberOfAvailableSpacesRight(position, positionDict)
                    - 1;
        }

        public int GetNumberOfAvailableVerticalSpaces(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            return GetNumberOfAvailableSpacesAbove(position, positionDict)
                    + GetNumberOfAvailableSpacesBelow(position, positionDict)
                    - 1;
        }

        public int GetNumberOfAvailableSpacesRight(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            var counter = 0;
            while (positionDict[position] == PositionStatus.Available)
            {
                position = position.PositionRight();
                counter++;
                if (!position.IsOnBoard()) { break; }
            }
            return counter;
        }

        public int GetNumberOfAvailableSpacesLeft(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            var counter = 0;
            while (positionDict[position] == PositionStatus.Available)
            {
                position = position.PositionLeft();
                counter++;
                if (!position.IsOnBoard()) { break; }
            }
            return counter;
        }

        public int GetNumberOfAvailableSpacesAbove(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            var counter = 0;
            while (positionDict[position] == PositionStatus.Available)
            {
                position = position.PositionAbove();
                counter++;
                if (!position.IsOnBoard()) { break; }
            }
            return counter;
        }

        public int GetNumberOfAvailableSpacesBelow(Position position, Dictionary<Position, PositionStatus> positionDict)
        {
            var counter = 0;
            while (positionDict[position] == PositionStatus.Available)
            {
                position = position.PositionBelow();
                counter++;
                if (!position.IsOnBoard()) { break; }
            }
            return counter;
        }

        public List<Position> UpdateDiagonalList(Dictionary<Position, PositionStatus> positionDict, List<Position> diagonalList)
        {
            List<Position> updatedDiagonalList = new List<Position>();

            foreach (var position in diagonalList)
            {
                if (positionDict[position] == PositionStatus.Available) { updatedDiagonalList.Add(position); }
            }
            return updatedDiagonalList;
        }
    }
}
