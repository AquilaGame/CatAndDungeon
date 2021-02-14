using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BGMCtrler : MonoBehaviour
{
    [SerializeField] Dropdown Dp = null;
    // Start is called before the first frame update
    List<string> FileURLs = new List<string>();
    void Start()
    {
        List<string> paths = new List<string>();
        System.IO.DirectoryInfo savefolder = new System.IO.DirectoryInfo(OnGame.BGMPath);
        System.IO.FileInfo[] files = savefolder.GetFiles("*.ogg");
        for (int i = 0; i < files.Length; i++)
        {
            FileURLs.Add(files[i].FullName);
            paths.Add(files[i].Name);
        }
        files = savefolder.GetFiles("*.wav");
        for (int i = 0; i < files.Length; i++)
        {
            FileURLs.Add(files[i].FullName);
            paths.Add(files[i].Name);
        }
        Dp.ClearOptions();
        Dp.AddOptions(paths);
        Dp.onValueChanged.AddListener(OnClick);
    }

    void OnClick(int val)
    {
        Sound.LoadBGM(FileURLs[val]);
    }


}
