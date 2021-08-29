using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using xiaoye97;
using HarmonyLib;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace ReformColorDraw
{
    [BepInPlugin("yzanh.DSP.ReformColorDraw", "ReformColorDraw", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
        static int nrows = 160;
        static int ncols = 300;
        static int nbase = 0;
        static int sbase = 162800;
        static int rdelta = 1000;
        static int cdelta = 1;

        public static Color[] colors;
        public static int[,] assignment;

        //Config
        static BepInEx.Configuration.ConfigEntry<string> out_loc_config;
        static string out_folder_path;

        void Start()
        {
            out_loc_config = Config.Bind<string>("config", "Out_Foloder_Abs_Path", "C:/asdas", "图片位(绝对路径)");

            // init pp
            out_folder_path = out_loc_config.Value;
            InitColors();
            InitAssignment();

            Harmony.CreateAndPatchAll(typeof(ReformColorDraw.Plugin), (string)null);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                for (int i = 0; i < 16; i++)
                    GameMain.localPlanet.factory.platformSystem.reformCustomColors[i] = colors[i];
                for (int row = 0; row < nrows; row++)
                {
                    for (int col = 0; col < ncols; col++)
                    {   
                        // row col in 160* 300
                        // assignment is in 300 * 160
                        int index_ = CalculateIndex(row, col);
                        int color_ = assignment[col, row];  // because 300 * 160
                        GameMain.localPlanet.factory.platformSystem.reformData[index_] = (byte)((int)GameMain.localPlanet.factory.platformSystem.reformData[index_] & 224 | color_ & 31);
                    }
                }
            }
        }
        void InitColors()
        {
            colors = new Color[16];
            
            System.Drawing.Color[] saved_colors = null;
            string full_cluster_path = Path.Combine(new string[] { out_folder_path, "cluster.xml" });
            Stream stream_cluster = File.Open(full_cluster_path, FileMode.Open);
            BinaryFormatter formatter_cluster = new BinaryFormatter();
            saved_colors = (System.Drawing.Color[])formatter_cluster.Deserialize(stream_cluster);
            
            for (int idx = 0; idx < 16; idx++)
            {
                int i = 16 + idx;
                float r = (float)saved_colors[i].R / 255f;
                float g = (float)saved_colors[i].G / 255f;
                float b = (float)saved_colors[i].B / 255f;
                colors[idx] = new Color(r, g, b, 0f);
            }
            stream_cluster.Close();
        }

        void InitAssignment()
        {
            assignment = null;
            string full_assign_path = Path.Combine(new string[] { out_folder_path, "assign.xml" });
            Stream stream_assign = File.Open(full_assign_path, FileMode.Open);
            BinaryFormatter formatter_assign = new BinaryFormatter();
            assignment = (int[,])formatter_assign.Deserialize(stream_assign);
            stream_assign.Close();
        }

        static int CalculateIndex(int row, int col)
        {
            if (row <  nrows / 2)  // north
                return nbase + (nrows / 2 - 1 - row) * rdelta + col * cdelta;
            return sbase + (row - nrows / 2) * rdelta + col * cdelta;
        } 
    }

}

