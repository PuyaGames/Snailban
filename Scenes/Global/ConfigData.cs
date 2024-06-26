using System;
using System.Collections.Generic;
using Godot;

public partial class ConfigData : Node
{
    public static string user_data_path = "user://user_data.json";
    // All .tscn file path of Elements
    public static Dictionary<int, FElementBean> ElementBeanDict = new Dictionary<int, FElementBean>();
    public static Dictionary<int, FMapBean> MapBeanDict = new Dictionary<int, FMapBean>();
    public static Godot.Collections.Array<string> SnailTexturePaths = new Godot.Collections.Array<string>();

    public override void _Ready()
	{
        LoadMapData();
        LoadElementData();
        LoadSnailTexturePaths();
	}

    public void LoadMapData()
    {
        string FilePath = MyPaths.GenMapDataPath("map_table.txt");
        Dictionary<string, List<string>> Dict = MyMethods.LoadCsv(FilePath);

        List<string> Numbers = Dict["ID"];
        List<string> Filenames = Dict["FILENAME"];
        if (Numbers.Count != Filenames.Count)
        {
            GD.PrintErr("CSV file format error! File: ", FilePath);
            return;
        }

        for (int i = 0; i < Filenames.Count; i++)
        {
            FMapBean MapData = LoadMapBeanFromFile(MyPaths.GenMapDataPath(Filenames[i]));
            MapBeanDict.Add(Int32.Parse(Numbers[i]), MapData);
        }
    }

    public void LoadElementData()
    {
        string FilePath = MyPaths.GenDataPath("element_table.txt");
        Dictionary<string, List<string>> Dict = MyMethods.LoadCsv(FilePath);
        
        List<string> Ids = Dict["ID"];
        List<string> Names = Dict["NAME"];
        List<string> Paths = Dict["PATH"];
        if ((Ids.Count == Names.Count && Names.Count == Paths.Count) == false)
        {
            GD.PrintErr("CSV file format error! File: ", FilePath);
            return;
        }

        for (int i = 0; i < Ids.Count; i++)
        {
            int Id = Int32.Parse(Ids[i]);
            FElementBean MyElementBean = new FElementBean(Id, Names[i], Paths[i]);
            ElementBeanDict.Add(Id, MyElementBean);
        }
    }

    public FMapBean LoadMapBeanFromFile(string FilePath)
    {
        Godot.Collections.Dictionary MapDataDict = MyMethods.LoadJson(FilePath);
        
        FMapBean RetVal = new FMapBean();
        RetVal.Id = (int)MapDataDict["id"];
        RetVal.Name = (string)MapDataDict["name"];
        RetVal.NextLevel = (int)MapDataDict["next_level"];
        Godot.Collections.Array Array = (Godot.Collections.Array)MapDataDict["matrix"];
        RetVal.Matrix = new int[8, 8];

        for (int i = 0; i < Array.Count; i++)
        {
            Godot.Collections.Array Columns = (Godot.Collections.Array)Array[i];
            for (int j = 0; j < Columns.Count; j++)
            {
                RetVal.Matrix[i, j] = (int)Columns[j];
            }
        }

        return RetVal;
    }

    public void LoadSnailTexturePaths()
    {
        List<string> SnailTextureFiles = MyMethods.LoadTxtToList(MyPaths.GenDataPath("snail_textures.txt"));
        foreach (string SnailTextureFile in SnailTextureFiles)
        {
            SnailTexturePaths.Add(MyPaths.GenTexturePath(SnailTextureFile));
        }
    }
}
