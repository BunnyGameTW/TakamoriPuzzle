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
    public AudioClip SE_puzzle1;
    public AudioClip SE_puzzle2;
    public AudioClip SE_puzzle3;
    public AudioClip SE_puzzle4;
    public AudioClip SE_transitionIn;
    public AudioClip SE_transitionOut;

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
