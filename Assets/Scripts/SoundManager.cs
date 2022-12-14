using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 聲音管理器
public class SoundManager: SingletonMono<SoundManager>
{
    public AudioSource musicSource;
    public AudioSource effectSource;

    public AudioClip BGM_title;
    public AudioClip BGM_xmas;

    public AudioClip SE_finish;
    public AudioClip SE_page;
    public AudioClip SE_page2;
    public AudioClip[] SE_puzzles;
    public AudioClip SE_transitionIn;
    public AudioClip SE_transitionOut;

    private float fadeInDuration = 0.75f;
    private float fadeOutDuration = 0.75f;
    private Coroutine BMGEvent = null;

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------
    
    /** 播放音樂 */
    public void playBGM(AudioClip music) {
        if (music == musicSource.clip) {
            return;
        }
        if (BMGEvent != null) {
            StopCoroutine(BMGEvent);
        }
        BMGEvent = StartCoroutine(changeBGM(music));
    }

    /** 停止音樂 */
    public void stopBGM(AudioClip music) {
        if (musicSource.isPlaying == false) {
            return;
        }
        if (BMGEvent != null) {
            StopCoroutine(BMGEvent);
        }
        BMGEvent = StartCoroutine(fadeOutStopBGM());
    }

    /** 播放音效 */
    public void playSE(AudioClip sound) {
        effectSource.PlayOneShot(sound, DataManager.instance.SEVolime);
    }
    public void playSE(AudioClip sound, float value)
    {
        effectSource.PlayOneShot(sound, DataManager.instance.SEVolime * value);
    }
    /** 設定音樂音量 */
    public void setBGMVolime(float value) {
        musicSource.volume = value;
    }

    /** 設定音效音量 */
    public void setSEVolime(float value) {
        effectSource.volume = value;
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 切換音樂 */
    private IEnumerator changeBGM(AudioClip music) {
        if (musicSource.isPlaying == true) {
            yield return fadeBGM(musicSource.volume, 0, fadeOutDuration);
        }
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
        yield return fadeBGM(0, DataManager.instance.BGMVolime, fadeInDuration);
        BMGEvent = null;
        yield break;
    }

    /** 淡出停止音樂 */
    private IEnumerator fadeOutStopBGM() {
        if (musicSource.isPlaying == true) {
            yield return fadeBGM(musicSource.volume, 0, fadeOutDuration);
        }
        musicSource.Stop();
        yield break;
    }

    /** 淡入淡出音樂 */
    private IEnumerator fadeBGM(float beginVolume, float finalVolume, float duration) {
        float currentTime = 0;
        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(beginVolume, finalVolume, Easing.Tween(currentTime/duration, EASE_TYPE.Linear));
            yield return null;
        }
        yield break;
    }

}
