using System;

public static partial class StageInfoTable
{
    public partial class Row
    {
        private int[] SmallMonsterIndex {  get { return Array.ConvertAll(s_smallMonsterType.Split(','), item => Convert.ToInt32(item)); } }
        private int[] MiddleBossIndex { get { return Array.ConvertAll(s_mBossMonsterType.Split(','), item => Convert.ToInt32(item)); } }

        public MonsterInfoTable.Row[] SmallMonsterList
        {
            get
            {
                var indexs = SmallMonsterIndex;

                MonsterInfoTable.Row[] result = new MonsterInfoTable.Row[indexs.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = MonsterInfoTable.Find_i_index(indexs[i]);

                return result;
            }
        }

        public MonsterInfoTable.Row[] MiddleBossList
        {
            get
            {
                var indexs = MiddleBossIndex;

                MonsterInfoTable.Row[] result = new MonsterInfoTable.Row[indexs.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = MonsterInfoTable.Find_i_index(indexs[i]);

                return result;
            }
        }

        public MonsterInfoTable.Row BossList{get{return MonsterInfoTable.Find_i_index(Convert.ToInt32(s_bossMonsterType));}}

    }
}

public static partial class MonsterInfoTable
{
    public partial class Row
    {
        public int[] Size
        {
            get
            {
                return Array.ConvertAll(s_size.Split('*'), item => Convert.ToInt32(item));
            }
        }

        //public int Width {get { return Size[0]; }}
        //public int Height { get { return Size[1]; } }
    }
}

