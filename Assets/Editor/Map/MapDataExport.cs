using System.IO;
using UnityEditor;
using UnityEngine;

class MapDataExport
{
    static readonly string _sourceDir = "./Assets/Scenes/";
    static readonly string _destDir = "./Assets/Resources/NavMap/";

    [MenuItem("Tools/Map/ExportServerNavMap")]
    public static void Execute()
    {
        string[] files = Directory.GetFiles(_sourceDir, "NavMap_*.png", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string assetName = Path.GetFileNameWithoutExtension(file);
            Export(file, "" + assetName);
        }
    }

    static void Export(string file, string assertName)
    {
        if (File.Exists(file) == false)
        {
            Debug.Log("Not find file: " + file);
            return;
        }
        FileStream br = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (br == null)
        {
            return;
        }
        Texture2D texture = new Texture2D(32, 32);
        byte[] datas = new byte[br.Length];
        br.Read(datas, 0, datas.Length);
        texture.LoadImage(datas);
        int width = texture.width;
        int height = texture.height;
        OStream os = new OStream(sizeof(int) + sizeof(int) + sizeof(byte) * width * height);
        os.Append(width);
        os.Append(height);
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                byte value;
                if (texture.GetPixel(x, y).r < 0.3f)
                {
                    //不可行走、不可跌落
                    value = 0;
                }
                else if (texture.GetPixel(x, y).r < 0.8f)
                {
                    //不可行走、可跌落
                    value = 1;
                }
                else
                {
                    //可行走、不可跌落
                    value = 2;
                }
                os.Append(value);
            }
        }

        assertName = assertName.Replace("NavMap_", "");
        string fileName = _destDir + assertName + ".bytes";
        FileStream bw = File.Create(fileName);
        if (bw != null)
        {
            bw.Write(os.Cache, 0, os.Length);
            bw.Close();
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}