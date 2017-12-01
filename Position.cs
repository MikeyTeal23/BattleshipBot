using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleshipBot.Direction;

namespace BattleshipBot
{
    public class Position
    {
        public string Square { get; set; }
        public char Row { get; set; }
        public int Column { get; set; }

        public Position(int column, char row)
        {
            Column = column;
            Row = row;
            Square = column.ToString() + row.ToString();
        }

        public Position(string square)
        {
            Column = (int)char.GetNumericValue(square[1]);
            Row = Convert.ToChar(square[0]);
            Square = square;
        }

        public bool IsOnBoard()
        {
            return (Column >= 0 && Column <= 9 && Row >= 'A' && Row <= 'J');
        }

        public Position PositionLeft()
        {
            return new Position(this.Column-1, this.Row);
        }
        public Position PositionBelow()
        {
            return new Position(this.Column, (char)(this.Row+1));
        }
        public Position PositionAbove()
        {
            return new Position(this.Column, (char)(this.Row-1));
        }
        public Position PositionRight()
        {
            return new Position(this.Column+1, this.Row);
        }

        public Position PositionUL()
        {
            return new Position(this.Column-1, (char)(this.Row-1));
        }
        public Position PositionUR()
        {
            return new Position(this.Column+1, (char)(this.Row-1));
        }
        public Position PositionDL()
        {
            return new Position(this.Column-1, (char)(this.Row+1));
        }
        public Position PositionDR()
        {
            return new Position(this.Column+1, (char)(this.Row+1));
        }

        public Position ShiftPositionRight(int length)
        {
            return new Position(this.Column+length, this.Row);
        }
        public Position ShiftPositionDown(int length)
        {
            return new Position(this.Column, (char)(this.Row + length));
        }

        public List<Position> AdjacentSquares()
        {
            List<Position> adjSquares = new List<Position>();
            if (Row > 'A') { adjSquares.Add(this.PositionAbove()); }
            if (Row < 'J') { adjSquares.Add(this.PositionBelow()); }
            if (Column > 0) { adjSquares.Add(this.PositionLeft()); }
            if (Column < 9) { adjSquares.Add(this.PositionRight()); }
            return adjSquares;
        }

        public List<Position> DiagonalSquares()
        {
            List<Position> diagSquares = new List<Position>();
            if (Row > 'A' && Column > 0) { diagSquares.Add(this.PositionUL()); }
            if (Row > 'A' && Column < 9) { diagSquares.Add(this.PositionUR()); }
            if (Row < 'J' && Column > 0) { diagSquares.Add(this.PositionDL()); }
            if (Row < 'J' && Column < 9) { diagSquares.Add(this.PositionDR()); } 
            return diagSquares;
        }

        public bool IsUpperLeftCornerOfShip(Dictionary<Position, PositionStatus> positionDict)
        {

            List<Position> upperAndLeftSquareList = new List<Position>();
            if (Row > 'A') { upperAndLeftSquareList.Add(this.PositionAbove()); }
            if (Column > 0) { upperAndLeftSquareList.Add(this.PositionLeft()); }

            return AdjacentSquares().Where(p => positionDict[p] == PositionStatus.Hit).ToList().Count() == 1
                   && upperAndLeftSquareList.Where(p => positionDict[p] == PositionStatus.Hit).ToList().Count() == 0;
        }

        public Directions GetDirectionOfAdjacentHit(Dictionary<Position, PositionStatus> positionDict)
        {
            return positionDict.Keys.Contains(PositionBelow()) ? Directions.Vertical : Directions.Horizontal;
        }

        public void RemoveFromList(List<Position> list)
        {
            if (list.Contains(this))
            {
                list.RemoveAt(list.IndexOf(this));
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            Position p = obj as Position;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Row == p.Row && Column == p.Column;
        }

        public bool Equals(Position p)
        {
            // Return true if the fields match:
            return Row == p.Row && Column == p.Column;
        }

        public static bool operator ==(Position a, Position b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Column == b.Column && a.Row == b.Row;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = (hashCode * 397) ^ Column;
                var rowHashCode =
                    !string.IsNullOrEmpty(Row.ToString()) ?
                        Row.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Row;
                return hashCode;
            }
        }
    }
}