using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 聲音管理器
public class SoundManager: SingletonMono<SoundManager>
{
    public AudioSource musicSource;
    public AudioSource effectSource;

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        init(() => {
            AudioClip bgm = ResManager.loadAudioClip(RES_PATH.BGM.TITLE);
            SoundManager.instance.playBGM(bgm);
        });
    }

    public void init(System.Action callback) {
        StartCoroutine(preloadSound(callback));
    }

    IEnumerator preloadSound(System.Action callback) {
        yield return ResManager.asyncLoadAudioClip(RES_PATH.BGM.TITLE);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.BGM.XMAS);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.FINISH);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.PAGE);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.PAGE_2);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.PUZZLE_1);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.PUZZLE_2);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.PUZZLE_3);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.PUZZLE_4);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.TRANSITION_IN);
        yield return ResManager.asyncLoadAudioClip(RES_PATH.SE.TRANSITION_OUT);
        if (callback != null) {
            callback();
        }
    }

    void Update()
    {

    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------
    
    /** 播放音樂 */
    public void playBGM(AudioClip music) {
        if (musicSource.isPlaying == true) {
            musicSource.Stop();
        }
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.volume = DataManager.instance.getBGMVolime();
        musicSource.Play();
    }

    /** 播放音效 */
    public void playSE(AudioClip sound) {
        effectSource.PlayOneShot(sound, DataManager.instance.getSEVolime());
    }

    /** 設定音樂音量 */
    public void setBGMVolime(float value) {
        musicSource.volume = value;
    }

    /** 設定音效音量 */
    public void setSEVolime(float value) {
        effectSource.volume = value;
    }
}
