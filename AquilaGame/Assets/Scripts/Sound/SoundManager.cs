using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public enum CharacterSoundType
{
    Attack,
    Walk,
    Birth,
    Death
}
static public class Sound
{
    static public SoundManager manager = null;
    static public AudioClip ExternBGMClip = null;
    public static void Play(AudioClip clip)
    {
        if (manager != null)
            manager.PlayClip(clip);
    }
    public static void Play(AudioClip clip, object obj)
    {
        if (manager != null)
            manager.PlayInterruptableClip(clip, obj);
    }
    public static void PlayStatic(int track, AudioClip clip)
    {
        if (manager != null)
            manager.PlayStaticClip(track, clip);
    }
    public static void PlayDamage(int damage)
    {
        if (manager != null)
            manager.PlayDamageSound(damage);
    }
    public static void PlayHeal()
    {
        if (manager != null)
            manager.PlayHealSound();
    }
    public static void PlayBuff()
    {
        if (manager != null)
            manager.PlayBuffSound();
    }
    public static void PlayClick()
    {
        if (manager != null)
            manager.PlayClick();
    }
    public static void StartDice()
    {
        if (manager != null)
            manager.PlayDice(true);
    }
    public static void StopDice()
    {
        if (manager != null)
            manager.PlayDice(false);
    }

    public static void LoadBGM(string path)
    {
        if (manager != null)
            manager.StartLoadBGM(path);
    }

}

public class SoundManager : MonoBehaviour
{
    //默认的8条持续播放音轨
    //0-BGM
    [SerializeField] AudioSource[] StaticASList = new AudioSource[8];
    [SerializeField] GameObject ASPrefab = null;
    [SerializeField] AudioClip DefaultAudioClip = null;
    List<SoundBase> ASList = new List<SoundBase>();
    Queue<int> avaliableAS = new Queue<int>();
    [SerializeField] List<AudioClip> DamageSounds = new List<AudioClip>();
    [SerializeField] AudioClip HealSound = null;
    [SerializeField] AudioClip BuffSound = null;
    [SerializeField] AudioClip DiceSound = null;
    [SerializeField] AudioClip OnClickUISound = null;
    private void Awake()
    {
        Sound.manager = this;
        OnGame.Log("音频控制器已连接");
    }
    // Start is called before the first frame update
    void Start()
    {
        if (DefaultAudioClip != null)
            PlayStaticClip(0, DefaultAudioClip);
    }

    public void PlayStaticClip(int i, AudioClip clip)
    {
        if (clip == null) return;
        if (StaticASList[i].clip != null)
            StaticASList[i].Stop();
        StaticASList[i].clip = clip;
        StaticASList[i].Play();
    }
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        int i;
        if (avaliableAS.Count > 0)
        {
            i = avaliableAS.Dequeue();
            ASList[i].Play(clip);
        }
        else
        {
            i = ASList.Count;
            ASList.Add(Instantiate(ASPrefab, transform).GetComponent<SoundBase>());
            ASList[i].Init(i);
            ASList[i].Play(clip);
        }
    }

    public void PlayInterruptableClip(AudioClip clip, object obj)
    {
        //clip是空的，那就不管
        if (clip == null) return;
        //如果这个脚本已经在播放声音了，更新这个声音
        foreach (var v in ASList)
        {
            if (v.Using == obj)
            {
                v.Play(clip);
                return;
            }
        }
        //如果等待队列里还有剩下的就分配一个给这个新的
        if (avaliableAS.Count > 0)
        {
            int i = avaliableAS.Dequeue();
            ASList[i].Using = obj;
            ASList[i].Play(clip);
        }
        //没有的话就申请一个新的
        else
        {
            int i = ASList.Count;
            ASList.Add(Instantiate(ASPrefab, transform).GetComponent<SoundBase>());
            ASList[i].Init(i);
            ASList[i].Using = obj;
            ASList[i].Play(clip);
        }

    }
    public void PlayHealSound()
    {
        PlayClip(HealSound);
    }
    public void PlayBuffSound()
    {
        PlayClip(BuffSound);
    }
    public void PlayClick()
    {
        PlayClip(OnClickUISound);
    }
    public void PlayDice(bool isPlay)
    {
        if (isPlay)
            PlayStaticClip(1, DiceSound);
        else
            StaticASList[1].Stop();
    }
    public void PlayDamageSound(int damage)
    {
        if (damage <= 1)
        {
            PlayClip(DamageSounds[0]);
            return;
        }
        switch (damage)
        {
            case 2:
            case 3:
            case 4:
                PlayClip(DamageSounds[1]);
                return;
            case 5:
            case 6:
            case 7:
            case 8:
                PlayClip(DamageSounds[2]);
                return;
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
                PlayClip(DamageSounds[3]);
                return;
            default:
                PlayClip(DamageSounds[4]);
                return;
        }
    }



    public void Recycle(int i)
    {
        avaliableAS.Enqueue(i);
        ASList[i].Using = null;
    }

    public void StartLoadBGM(string path)
    {
        StartCoroutine("LoadBGM", path);
    }

    public IEnumerator LoadBGM(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            StopCoroutine("LoadBGM");
            yield return 0;
        }
        AudioType type = AudioType.WAV;
        string[] ss = path.Split('.');
        switch (ss[ss.Length - 1].ToLower())
        {
            case "ogg":
                type = AudioType.OGGVORBIS;
                break;
            case "wav":
                type = AudioType.WAV;
                break;
            default:
                StopCoroutine("LoadBGM");
                yield return 0;
                break;
        }
        UnityWebRequest Req = UnityWebRequestMultimedia.GetAudioClip(path, type);
        yield return Req.SendWebRequest();
        if (Req.isNetworkError || Req.isHttpError)
        {
            OnScene.Report("加载BGM失败：" + path);
        }
        else
        {
            Sound.ExternBGMClip = DownloadHandlerAudioClip.GetContent(Req);
            PlayStaticClip(0, Sound.ExternBGMClip);
        }
    }
}
