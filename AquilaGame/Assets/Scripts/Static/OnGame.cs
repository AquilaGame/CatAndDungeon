using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnGame
{
    public static SkillLib MagicLib = null;
    public static BuffLib buffLib = null;
    public static CursorLib cursorLib = null;
    public static UserCursor userCursor = new UserCursor();
    public static CharacterLib characterLib = null;
    public static SummonLib summonLib = null;
    public static CharacterPreviewer characterPreviewer = null;
    public const string CharecterSavePath = @"saves\Charecters\";
    public const string CharecterFileExtName = @".ch";
    public const string defaultPath = @"default\";
    public const string defaultMapName = @"map" + MapSet.MapSaveFileExtName;
    public const string BGMPath = @"BGMs\";
    readonly static string[] errStrList = new string[] {"/", "\"", @"\", @"/", ":", "*", "?", "<", ">", "|", "\r\n" };
    const string GameLogPath = "gameLog.txt";
    static System.IO.StreamWriter logStreamWriter = new System.IO.StreamWriter(GameLogPath);
    public const float WorldScale = 1.7f;
    public static bool CheckFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }
        foreach (string errStr in errStrList)
        {
            if (fileName.Contains(errStr))
            {
                return false;
            }
        }
        return true;
    }
    public static void Log(string s)
    {
        logStreamWriter.WriteLine(System.DateTime.Now.ToString() + "|:   " + s);
        logStreamWriter.Flush();
    }

}
