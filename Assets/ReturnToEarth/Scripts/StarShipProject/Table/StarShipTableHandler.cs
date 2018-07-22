using System;
using UnityEngine;
using System.Collections.Generic;

// 변경, 테이블을 로드하고, 그걸 해석해서 데이터를 돌려주는 클레스를 따로 만들자.

namespace StarShip
{
    public class DataProvider
    {
        public enum DataType
        {
            Dictionary,
            TableHandlerRow
        }

        private TableHandler.Row currentRow = null;
        private Dictionary<string, string> currentDictionary = null;

        private TableHandler.Table blueprintTable = null;
        private string currentBluePrint = "";
        private DataType dataType;

        public DataProvider(string TargetBluePrint, TableHandler.Row OwnedRow)
        {
            currentRow = OwnedRow;
            currentBluePrint = TargetBluePrint;
            blueprintTable = TableHandler.Get(TargetBluePrint, TableHandler.StreamMode.Resource);
            dataType = DataType.TableHandlerRow;
        }

        public DataProvider(string TargetBluePrint, Dictionary<string,string> generated)
        {
            currentDictionary = generated;
            currentBluePrint = TargetBluePrint;
            blueprintTable = TableHandler.Get(TargetBluePrint, TableHandler.StreamMode.Resource);
            dataType = DataType.Dictionary;
        }

        public void Update(TableHandler.Row ReplaceRow)
        {
            currentRow = ReplaceRow;
            dataType = DataType.TableHandlerRow;
        }

        public T Get<T>(string name)
        {

            string blueprintIndex = "";
            switch (dataType)
            {
                case DataType.Dictionary:
                    blueprintIndex = currentDictionary["blueprintIndex"];
                    break;
                case DataType.TableHandlerRow:
                    if (currentRow == null || blueprintTable == null)
                    {
                        Debug.LogWarning("OwnedRow is not linked with blueprint");
                        T returnValue = (T)Convert.ChangeType(0, typeof(T));
                        return returnValue;
                    }
                    blueprintIndex  = currentRow.Get<string>("blueprintIndex");
                    break;
            }

            return blueprintTable.FindRow("index", blueprintIndex).Get<T>(name);
        }

        /*
           _SB = 스텟의 기본값 블루프린트 테이블에 존재
           _SR = 스텟 증가폭, 블루프린트 테이블에 존재

           _SC = 스탯의 보정치, owned 테이블에 SB들을 대체하여 존재
           _SP = 스탯 포인트. 몇포인트의 스탯포인트가 투자되었는지 나타나는 값, owned 테이블에 SR 항목들을 대체하여 존재
        */
        public float GetStat(string StatName)
        {
            string ParseTarget = Get<string>(string.Format("{0}_SB", StatName));
            float StatBase = 0.0f;
            float StatRise = Get<float>(string.Format("{0}_SR", StatName));
            float StatCorrection = 0.0f;
            float StatPoint = 0.0f;
            string CSFormat = string.Format("{0}_SC", StatName);
            string CPFormat = string.Format("{0}_SP", StatName);

            switch (dataType)
            {
                case DataType.Dictionary:
                    if (currentDictionary.ContainsKey(CSFormat))
                        StatCorrection = float.Parse(currentDictionary[CSFormat]);

                    if (currentDictionary.ContainsKey(CPFormat))
                        StatPoint = float.Parse(currentDictionary[CPFormat]);

                    break;
                case DataType.TableHandlerRow:
                    StatCorrection = currentRow.Get<float>(CSFormat);
                    StatPoint = currentRow.Get<float>(CPFormat);
                    break;
            }

            float Result = 0.0f;

            if (ParseTarget.Contains(","))
            {
                StatBase = Array.ConvertAll(ParseTarget.Split(','), Parsed => Convert.ToInt32(Parsed))[0];
            }
            else
            {
                StatBase = float.Parse(ParseTarget);
            }

            Result = ( StatBase + StatCorrection ) + StatRise * StatPoint;

            Debug.LogFormat("Stat Data({0}), Result = {1}, Base = {2}, Rise = {3}, Correction = {4}, Point = {5}",
                StatName, Result, StatBase, StatRise, StatCorrection, StatPoint);

            return ( StatBase + StatCorrection ) + StatRise * StatPoint;
        }
    }
}
