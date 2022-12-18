using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPopup : MonoBehaviour
{
    public Slider sliderBGM;
    public Slider sliderSE;
    public TMP_Dropdown dropdownLanguage;
    public Toggle toggleSkipPuzzle;
    public Toggle toggleAutoPlay;
    
    public TMP_Text textTitle;
    public TMP_Text textBGM;
    public TMP_Text textSE;
    public TMP_Text textLanguage;
    public TMP_Text textSkipPuzzle;
    public TMP_Text textAutoPlay;
    public TMP_Text textReset;
    public TMP_Text textCredit;
    public TMP_Text textProgramming;
    public TMP_Text textArt;
    public TMP_Text textTranslation;
    public ScrollRect scroll;
    private float BGMVolime = 1.0f;
    private float SEVolime = 1.0f;
    private SystemLanguage language = SystemLanguage.English;
    private bool skipPassPuzzle = false;
    private bool autoPlayDialog = false;

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        onOpenPopup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 監聽改變音樂音量 */
    public void onChangeBGMVolime() {
        float value = sliderBGM.value;
        BGMVolime = value;
        SoundManager.instance.setBGMVolime(BGMVolime);
    }

    /** 監聽改變音效音量 */
    public void onChangeSEVolime() {
        float value = sliderSE.value;
        SEVolime = value;
        SoundManager.instance.setSEVolime(SEVolime);
    }

    /** 監聽改變語言 */
    public void onChangeLanguage() {
        int value = dropdownLanguage.value;
        switch(value) {
            case 0: {
                language = SystemLanguage.English;
            } break;
            case 1: {
                language = SystemLanguage.ChineseTraditional;
            } break;
            case 2: {
                language = SystemLanguage.Japanese;
            } break;
            default: {
                language = SystemLanguage.English;
            } break;
        }
        changeUIText(DataManager.instance.getLanguageCode(language));
    }

    /** 監聽改變跳過已解謎題 */
    public void onChangeSkipPassPuzzle() {
        bool value = toggleSkipPuzzle.isOn;
        skipPassPuzzle = value;
    }

    /** 監聽改變自動播放對話 */
    public void onChangeAutoPlayDialog() {
        bool value = toggleAutoPlay.isOn;
        autoPlayDialog = value;
    }

    /** 重置設定 */
    public void resetSetting() {
        BGMVolime = 1.0f;
        SEVolime = 1.0f;
        skipPassPuzzle = false;
        autoPlayDialog = false;

        sliderBGM.value = BGMVolime;
        sliderSE.value = SEVolime;
        toggleSkipPuzzle.isOn = skipPassPuzzle;
        toggleAutoPlay.isOn = autoPlayDialog;
    }
    Animator animator;
    /** 開啟視窗 */
    public void onOpenPopup() {
        BGMVolime = DataManager.instance.BGMVolime;
        SEVolime = DataManager.instance.SEVolime;
        language = DataManager.instance.language;
        skipPassPuzzle = DataManager.instance.skipPassPuzzle;
        autoPlayDialog = DataManager.instance.autoPlayDialog;
        
        sliderBGM.value = BGMVolime;
        sliderSE.value = SEVolime;
        dropdownLanguage.value = getLanguageIndex(language);
        toggleSkipPuzzle.isOn = skipPassPuzzle;
        toggleAutoPlay.isOn = autoPlayDialog;
        
        changeUIText(DataManager.instance.getLanguageCode(language));

        animator.SetBool("isShow", true);
        scroll.content.GetComponent<RectTransform>().anchoredPosition =
            new Vector3(scroll.content.GetComponent<RectTransform>().anchoredPosition.x, 0);
    }

    /** 關閉視窗 */
    public void onClosePopup() {
        DataManager.instance.BGMVolime = BGMVolime;
        DataManager.instance.SEVolime = SEVolime;
        DataManager.instance.language = language;
        DataManager.instance.skipPassPuzzle = skipPassPuzzle;
        DataManager.instance.autoPlayDialog = autoPlayDialog;
        animator.SetBool("isShow", false);
        //this.gameObject.SetActive(false);
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 取得語言index */
    private int getLanguageIndex(SystemLanguage systemLanguage) {
        switch(systemLanguage) {
            case SystemLanguage.English:
                return 0;
            case SystemLanguage.ChineseTraditional:
                return 1;
            case SystemLanguage.Japanese:
                return 2;
            default:
                return 0;
        }
    }

    /** 更換UI語言 */
    private void changeUIText(string languageCode) {
        LoadExcel data = LoadExcel.instance;
        textTitle.text =        data.getObjectValue("uiText", "name", "setting", languageCode);
        textBGM.text =          data.getObjectValue("uiText", "name", "bgm", languageCode);
        textSE.text =           data.getObjectValue("uiText", "name", "se", languageCode);
        textLanguage.text =     data.getObjectValue("uiText", "name", "language", languageCode);
        textSkipPuzzle.text =   data.getObjectValue("uiText", "name", "skipPuzzle", languageCode);
        textAutoPlay.text =     data.getObjectValue("uiText", "name", "storyAuto", languageCode);
        textReset.text =        data.getObjectValue("uiText", "name", "reset", languageCode);
        textCredit.text =       data.getObjectValue("uiText", "name", "credit", languageCode);
        textProgramming.text =  data.getObjectValue("uiText", "name", "programming", languageCode);
        textArt.text =          data.getObjectValue("uiText", "name", "art", languageCode);
        textTranslation.text =  data.getObjectValue("uiText", "name", "translation", languageCode);
    }
}
