using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TableHandler
{
    public enum StreamMode
    {
        AppData,
        Resource,
        Sample
    }

    public struct ColumnInfo
    {
        public int Index;
        public string Type;
        public string name;
    }

    public class Row
    {
        private Table table = null;
        private List<string> data = new List<string>();

        public Row(Table table, List<string> data)
        {
            this.table = table;
            this.data.AddRange(data);
        }

        public T Get<T>(string name)
        {
            int targetIndex = table.GetIndex(name);

            if(targetIndex == -1)
            {
                T returnValue = (T)Convert.ChangeType(0, typeof(T));
                return returnValue;
            }
            else
            {

                T returnValue = (T)Convert.ChangeType(data[targetIndex], typeof(T));
                return returnValue;
            }
        }
    
        public Dictionary<string, T> QueryByContainedString<T>(string value)
        {
            Dictionary<string, T> returnTable = new Dictionary<string, T>();

            List<string> matched = new List<string>();
            foreach (var key in table.ColumnHeader.Keys)
            {
                if(key.Contains(value))
                {
                    matched.Add(key);
                }
            }

            foreach (var key in matched)
            {
                returnTable.Add(key, Get<T>(key));
            }

            return returnTable;
        }

        public T GetAt<T>(int index)
        {
            return (T)Convert.ChangeType(data[index], typeof(T));
        }

        public List<string> GetAllData()
        {
            return data;
        }

        public bool Replace(List<string> newValue)
        {
            data.Clear();
            data.AddRange(newValue);
            return true;
        }

        public void ReplaceColumn(string columnName, string newValue)
        {
            int targetIndex = table.GetIndex(columnName);

            if(targetIndex == -1)
            {
                Debug.LogWarning("TargetColumn Does not Exist" + columnName);
            }
            else
            {
                data[table.GetIndex(columnName)] = newValue;
            }
        }

        public void Save()
        {
            table.Save();
        }
    }

    public class Table
    {
        private Dictionary<string, ColumnInfo> columnHeader = new Dictionary<string, ColumnInfo>();
        private List<Row> rows = new List<Row>();
        private string path = null;
        private string name = "";
        //private List<List<string>> data = new List<List<string>>();
            
        public List<Row> Rows { get { return rows; } }
        public Dictionary<string, ColumnInfo> ColumnHeader {  get { return columnHeader; } }

        public Table(string TableName, StreamMode LoadMode)
        {
            name = TableName;
            switch (LoadMode)
            {
                case StreamMode.AppData:
                    path = AppDataPath;
                    break;
                case StreamMode.Resource:
                    path = ResourcePath;
                    break;
                case StreamMode.Sample:
                    path = SamplePath;
                    break;
            }

            path += ( TableName + ".csv" );

            if (!System.IO.File.Exists(path))
            {
                // Create
                // 테이블 참조하는 스타일이 다르다. 지금 가정은 무조건 ㅇ=
                switch (LoadMode)
                {
                    case StreamMode.AppData:
                        // 엡데이타에 없으면 샘플을 불러오자.
                        path = SamplePath + TableName + ".csv";
                        break;
                    case StreamMode.Resource:
                        // Error! break;
                        UnityEngine.Debug.LogWarning("The table not exist in Resource! " + TableName);
                        break;
                }
            }

            using (CsvFile.CsvFileReader reader = new CsvFile.CsvFileReader(path))
            {
                int index = 0;
                List<string> column = new List<string>();
                while (reader.ReadRow(column))
                {
                    // Column Header.
                    if (index == 0)
                    {
                        for (int i = 0; i < column.Count; i++)
                        {
                            if (column[i].Contains("s_"))
                            {
                                columnHeader.Add(column[i].Replace("s_", ""),new ColumnInfo() { Index = i, Type = "string", name = column[i] });
                            }
                            else if (column[i].Contains("i_"))
                            {
                                columnHeader.Add(column[i].Replace("i_", ""), new ColumnInfo() { Index = i, Type = "int", name = column[i] });
                            }
                            else if (column[i].Contains("f_"))
                            {
                                columnHeader.Add(column[i].Replace("f_", ""), new ColumnInfo() { Index = i, Type = "float", name = column[i] });
                            }
                            else if( column[i].Contains("b_"))
                            {
                                columnHeader.Add(column[i].Replace("b_", ""), new ColumnInfo() { Index = i, Type = "bool", name = column[i] });
                            }
                            else
                            {
                                columnHeader.Add(column[i], new ColumnInfo() { Index = i, Type = "string", name = column[i] });
                            }
                        }
                    }
                    // Datas
                    else
                    {
                        rows.Add(new Row(this, column));
                    }
                    index++;
                }
            }
        }
        
        public void Add(List<string> item)
        {
            int newIndex = Rows.Count + 1;
            item[0] = Convert.ToString(newIndex);
            rows.Add(new Row(this, item));
        }

        public bool Save(StreamMode SaveMode = StreamMode.AppData)
        {
            switch (SaveMode)
            {
                case StreamMode.AppData:
                    path = (AppDataPath+ name + ".csv" );
                    break;
                case StreamMode.Resource:
                case StreamMode.Sample:
                    return false;
            }

            return Save(path);
        }

        public bool Save(string path)
        {
            if(!System.IO.File.Exists(path))
            {
                if (!System.IO.Directory.Exists(AppDataPath))
                {
                    System.IO.Directory.CreateDirectory(AppDataPath);
                }

                using (System.IO.FileStream fs = System.IO.File.Create(path))
                {
                }
            }
            
            using (var writer = new CsvFile.CsvFileWriter(path))
            {
                List<string> columns = new List<string>(columnHeader.Count);

                var headerEnumerator = columnHeader.Values.GetEnumerator();
                while(headerEnumerator.MoveNext())
                {
                    var current = headerEnumerator.Current;
                    columns.Add(current.name);
                }

                writer.WriteRow(new List<string>(columns));
                columns.Clear();

                foreach (Row row in rows)
                {
                    columns.AddRange(row.GetAllData());
                    writer.WriteRow(new List<string>(columns));
                    columns.Clear();
                }
            }

            return true;
        }

        public Row GetAt(int index)
        {
            return rows[index];
        }

        public Row FindRow<T>(string columnName, T condition)
        {
            
            return rows.Find(x => Compare(x.Get<T>(columnName), condition));
        }

        public bool Replace<T>(string columnName, T condition, List<string> newValue)
        {
            Row target = FindRow(columnName, condition);
            return target.Replace(newValue);
        }

        // 현 테이블에 있는 모든 Data를 제거한다.
        public void Refresh()
        {
            rows.Clear();
        }

        public List<Row> FindRows<T>(string columnName, T condition)
        {
            return rows.FindAll(x => Compare(x.Get<T>(columnName), condition));
        }

        public ColumnInfo GetColumnInfo(string name)
        {
            return columnHeader[name];
        }

        public int GetIndex(string name)
        {
            //Debug.LogFormat("({0}) GetIndex From ({1})", name, this.name);
            if(columnHeader.ContainsKey(name))
            {
                return columnHeader[name].Index;
            }
            else
            {
                return -1;
            }
        }

        public bool Compare<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }
    }

    // 필요한 기능 1 : 테이블을 로드하고, 이를 메모리에 케시 해두고 손쉽게 접근 가능해야 함.
    // 필요한 기능 2 : 로드한 테이블의 개별 데이터에 접근이 쉬워야 함.

    // 필요한 기능 3 : 로드한 테이블의 일부 내용을 수정하고, 이를 메모리/파일에 저장해야 함.
    // 필요한 기능 4 : 수정된 테이블에 다시 접근하여 데이터를 가져올 때, 수정된 부분이 반영되야 함.
    private static Dictionary<string, Table> tableMap = new Dictionary<string, Table>();

    public static string AppDataPath { get { return string.Format("{0}/Table/", UnityEngine.Application.persistentDataPath); } }
    public static string SamplePath =   "Assets/ReturnToEarth/Resources/Table/Sample/";
    public static string ResourcePath =   "Assets/ReturnToEarth/Resources/Table/";

    private static Table GenerateTable(string tableName, StreamMode LoadSteam)
    {
        return new Table(tableName, LoadSteam);
    }

    public static Table Get(string tableName, StreamMode LoadSteam)
    {
        if(!tableMap.ContainsKey(tableName))
        {
            Table newTable = GenerateTable(tableName, LoadSteam);
            tableMap.Add(tableName, newTable);
        }

        return tableMap[tableName];
    }


    public static bool Save(string tableName, StreamMode saveMode = StreamMode.AppData)
    {
        bool ReturnValue = false;
        if (!tableMap.ContainsKey(tableName))
        {
            //Warning!
            return ReturnValue;
        }

        switch (saveMode)
        {
            case StreamMode.AppData:
                ReturnValue = tableMap[tableName].Save(string.Format("{0}{1}.csv", AppDataPath, tableName));
                break;
            case StreamMode.Resource:
            case StreamMode.Sample:
                Debug.Log("Not Supported : (" + tableName + ") " + saveMode);
                break;
        }

        return ReturnValue;
    }
}
