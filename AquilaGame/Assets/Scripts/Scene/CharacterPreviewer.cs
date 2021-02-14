using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreviewer : MonoBehaviour
{
    [SerializeField] Transform TargetTrans = null;
    [SerializeField] Camera PreviewCamera = null;
    GameObject model = null;
    BirthGroupType LastGroupType = BirthGroupType.KingdomMale;
    public void SetCam(BirthGroupType birthGroupType,OccupationType occupationType)
    {
        if (model != null)
            Destroy(model);
        model = Instantiate(OnGame.characterLib.Protypes[(int)birthGroupType][(int)occupationType], TargetTrans);
        if (((int)LastGroupType + (int)birthGroupType) % 2 == 1)
            model.GetComponent<CharacterScript>().PlaySound(CharacterSoundType.Birth);
        model.GetComponent<CharacterScript>().enabled = false;
        model.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        model.GetComponent<CharacterAnimCtrl>().isPreView = true;
        LastGroupType = birthGroupType;
    }
    public void SetRot(float f)
    {
        TargetTrans.localEulerAngles = new Vector3(0, f * 360.0f, 0);
    }
    public void OnDisable()
    {
        if (model != null)
            Destroy(model);
    }

    private void Awake()
    {
        OnGame.characterPreviewer = this;
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
