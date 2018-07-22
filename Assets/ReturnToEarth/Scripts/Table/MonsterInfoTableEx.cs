using System;

public static partial class MonsterInfoTable
{
    public partial class Row
    {
        public StarShip.Board.LineType LineType
        {
            get
            {
                return (StarShip.Board.LineType)Enum.Parse(typeof(StarShip.Board.LineType), s_attackRange,true);
            }
        }

    }
}
public static partial class OwnedWeaponsTable
{
    public partial class Row
    {
        public StarShip.Board.LineType LineType
        {
            get
            {
                return (StarShip.Board.LineType)Enum.Parse(typeof(StarShip.Board.LineType), attack_target, true);
            }
        }

    }
}
