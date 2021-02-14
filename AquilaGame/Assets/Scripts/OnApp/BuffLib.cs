using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLib : MonoBehaviour
{
    public int buffCount = 52;
    const string buffClassNamePrefix = "BUFF_";
    public Sprite OccupationBranchIcon = null;
    public List<Sprite> icons = new List<Sprite>();
    public List<string> names = new List<string>();
    public List<GameObject> bufObjs = new List<GameObject>();
    public Dictionary<string, System.Type> buffs = new Dictionary<string, System.Type>();
    private void Awake()
    {
        for (int i = 0; i < buffCount; i++)
        {

            System.Type type = System.Type.GetType(buffClassNamePrefix + i.ToString());
            if (type != null)
            {
                if (buffs.ContainsKey(names[i]))
                {
                    OnGame.Log("错误：状态库中已经存在一个叫做" + names[i] + "的状态");
                }
                else
                {
                    buffs.Add(names[i], type);
                }
            }
            else
            {
                OnGame.Log("名为BUFF_" + i.ToString() + "的状态不存在");
            }

            /*
            写到这的时候我用了程序自动生成代码，我觉得虽然傻但是很有意思，放这缅怀一下
            tempsw.WriteLine("//BUFF_" + i.ToString() + ": " + names[i]);
            tempsw.WriteLine("public class BUFF_" + i.ToString() + " : Buff\n{");
            tempsw.WriteLine("    public BUFF_" + i.ToString() + "(CharacterScript cs, int time, float val) : base(" + i.ToString() + ", cs, time, val) { }");
            tempsw.WriteLine("    public override string Info()\n    {\n    return \"\";\n    }\n}\n");
            */
        }
        OnGame.buffLib = this;
        OnGame.Log("Buff库建立连接");
    }

}
