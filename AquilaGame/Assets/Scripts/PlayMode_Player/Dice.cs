using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    int faces;
    public Dice(int _faces)
    {
        faces = _faces;
    }
    public int roll(int count = 1)
    {
        if (faces == 0)
            return 0;
        int ret = 0;
        for (int i = 0; i < count; i++)
        {
            ret += Random.Range(1, faces + 1);
        }
        return ret;
    }
}
