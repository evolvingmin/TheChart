using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace StarShip
{
    public static class TableEditor
    {
        [MenuItem("ReturnToEarth/Table/ResetTable")]
        public static void ResetTable()
        {
            string path = TableHandler.AppDataPath;

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
        [MenuItem("ReturnToEarth/Table/OpenTableLocation")]
        public static void OpenTableLocation()
        {
            System.Diagnostics.Process.Start("explorer.exe", @Path.GetFullPath(TableHandler.AppDataPath));
        }
    }
}


