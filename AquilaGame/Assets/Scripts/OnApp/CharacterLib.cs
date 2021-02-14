using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLib : MonoBehaviour
{
    public List<List<GameObject>> Protypes = new List<List<GameObject>>();
    [Header("王国领属")]
    public List<GameObject> MaleHerosBlue = new List<GameObject>();
    public List<GameObject> FemaleHerosBlue = new List<GameObject>();
    [Header("帝国领属")]
    public List<GameObject> MaleHerosRed = new List<GameObject>();
    public List<GameObject> FemaleHerosRed = new List<GameObject>();
    [Header("教会领属")]
    public List<GameObject> MaleHerosYellow = new List<GameObject>();
    public List<GameObject> FemaleHerosYellow = new List<GameObject>();
    [Header("城镇领属")]
    public List<GameObject> MaleHerosGreen = new List<GameObject>();
    public List<GameObject> FemaleHerosGreen = new List<GameObject>();
    [Header("亡骨领属")]
    public List<GameObject> Monstars = new List<GameObject>();
    private void Awake()
    {
        Protypes.Add(MaleHerosBlue);
        Protypes.Add(FemaleHerosBlue);
        Protypes.Add(MaleHerosRed);
        Protypes.Add(FemaleHerosRed);
        Protypes.Add(MaleHerosYellow);
        Protypes.Add(FemaleHerosYellow);
        Protypes.Add(MaleHerosGreen);
        Protypes.Add(FemaleHerosGreen);
        Protypes.Add(Monstars);
        OnGame.characterLib = this;
        OnGame.Log("角色库建立完成");
    }
}
