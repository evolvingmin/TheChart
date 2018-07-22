// This code automatically generated by TableCodeGen
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using C2TPro;

public static partial class MonsterInfoTable
{
	public partial class Row
	{
		public int i_index;
		public string s_size;
		public string s_name;
		public int i_hp;
		public int i_attack;
		public float f_attackSpeed;
		public string s_attackType;
		public string s_attackRange;
		public int i_defence;
		public float f_moveSpeed;
		public string s_appearance;
		public string s_bullet;
		public float f_bulletSpeed;
        public float f_dropRate;
        public float f_dropRateSupplies;
        public float f_dropRateRearmetals;
        public float f_dropRateBioMetals;

    }

	static List<Row> rowList = new List<Row>();
	static bool isLoaded = false;

	public static bool IsLoaded()
	{
		return isLoaded;
	}

    public static void Clear()
    {   
        isLoaded = false;
        rowList.Clear();
    }

	public static List<Row> GetRowList()
	{
        StaticLoad();
		return rowList;
	}

	public static void Load(string csv)
	{
		rowList.Clear();
		string[][] grid = CsvParser.Parse(csv);
		for(int i = 1 ; i < grid.Length ; i++)
		{
			if(grid[i][0].StartsWith("#"))
				continue;	// skip comment

			Row row = new Row();
			row.i_index = string.IsNullOrEmpty(grid[i][0]) ? 0 : Convert.ToInt32(grid[i][0]);
			row.s_size = grid[i][1];
			row.s_name = grid[i][2];
			row.i_hp = string.IsNullOrEmpty(grid[i][3]) ? 0 : Convert.ToInt32(grid[i][3]);
			row.i_attack = string.IsNullOrEmpty(grid[i][4]) ? 0 : Convert.ToInt32(grid[i][4]);
			row.f_attackSpeed = string.IsNullOrEmpty(grid[i][5]) ? 0 : Convert.ToSingle(grid[i][5]);
			row.s_attackType = grid[i][6];
			row.s_attackRange = grid[i][7];
			row.i_defence = string.IsNullOrEmpty(grid[i][8]) ? 0 : Convert.ToInt32(grid[i][8]);
			row.f_moveSpeed = string.IsNullOrEmpty(grid[i][9]) ? 0 : Convert.ToSingle(grid[i][9]);
			row.s_appearance = grid[i][10];
			row.s_bullet = grid[i][11];
			row.f_bulletSpeed = string.IsNullOrEmpty(grid[i][12]) ? 0 : Convert.ToSingle(grid[i][12]);

			rowList.Add(row);
		}
		isLoaded = true;
	}

	public static void StaticLoad()
	{
		if(!isLoaded) 
            Load(Resources.Load<TextAsset>("Table/MonsterInfo").text);
	}


	public static int NumRows()
	{
        StaticLoad();
		return rowList.Count;
	}

	public static Row GetAt(int i)
	{
        StaticLoad();
		if(rowList.Count <= i)
			return null;
		return rowList[i];
	}

	public static Row Find_i_index(int find)
	{
        StaticLoad();
		return rowList.Find(x => x.i_index == find);
	}
	public static List<Row> FindAll_i_index(int find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.i_index == find);
	}
	public static Row Find_s_size(string find)
	{
        StaticLoad();
		return rowList.Find(x => x.s_size == find);
	}
	public static List<Row> FindAll_s_size(string find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.s_size == find);
	}
	public static Row Find_s_name(string find)
	{
        StaticLoad();
		return rowList.Find(x => x.s_name == find);
	}
	public static List<Row> FindAll_s_name(string find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.s_name == find);
	}
	public static Row Find_i_hp(int find)
	{
        StaticLoad();
		return rowList.Find(x => x.i_hp == find);
	}
	public static List<Row> FindAll_i_hp(int find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.i_hp == find);
	}
	public static Row Find_i_attack(int find)
	{
        StaticLoad();
		return rowList.Find(x => x.i_attack == find);
	}
	public static List<Row> FindAll_i_attack(int find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.i_attack == find);
	}
	public static Row Find_f_attackSpeed(float find)
	{
        StaticLoad();
		return rowList.Find(x => x.f_attackSpeed == find);
	}
	public static List<Row> FindAll_f_attackSpeed(float find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.f_attackSpeed == find);
	}
	public static Row Find_s_attackType(string find)
	{
        StaticLoad();
		return rowList.Find(x => x.s_attackType == find);
	}
	public static List<Row> FindAll_s_attackType(string find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.s_attackType == find);
	}
	public static Row Find_s_attackRange(string find)
	{
        StaticLoad();
		return rowList.Find(x => x.s_attackRange == find);
	}
	public static List<Row> FindAll_s_attackRange(string find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.s_attackRange == find);
	}
	public static Row Find_i_defence(int find)
	{
        StaticLoad();
		return rowList.Find(x => x.i_defence == find);
	}
	public static List<Row> FindAll_i_defence(int find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.i_defence == find);
	}
	public static Row Find_f_moveSpeed(float find)
	{
        StaticLoad();
		return rowList.Find(x => x.f_moveSpeed == find);
	}
	public static List<Row> FindAll_f_moveSpeed(float find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.f_moveSpeed == find);
	}
	public static Row Find_s_appearance(string find)
	{
        StaticLoad();
		return rowList.Find(x => x.s_appearance == find);
	}
	public static List<Row> FindAll_s_appearance(string find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.s_appearance == find);
	}
	public static Row Find_s_bullet(string find)
	{
        StaticLoad();
		return rowList.Find(x => x.s_bullet == find);
	}
	public static List<Row> FindAll_s_bullet(string find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.s_bullet == find);
	}
	public static Row Find_f_bulletSpeed(float find)
	{
        StaticLoad();
		return rowList.Find(x => x.f_bulletSpeed == find);
	}
	public static List<Row> FindAll_f_bulletSpeed(float find)
	{
        StaticLoad();
		return rowList.FindAll(x => x.f_bulletSpeed == find);
	}

}