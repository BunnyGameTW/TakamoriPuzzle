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
    private float BGMVolime = 1;
    private float SEVolime = 1;
    private SystemLanguage language = SystemLanguage.English;

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
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
    }

    /** 監聽改變音效音量 */
    public void onChangeSEVolime() {
        float value = sliderSE.value;
        SEVolime = value;
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
        DataManager.instance.renewLocalization(DataManager.instance.getLanguageName(language));
    }

    /** 開啟視窗 */
    public void onOpenPopup() {
        BGMVolime = DataManager.instance.getBGMVolime();
        SEVolime = DataManager.instance.getSEVolime();
        language = DataManager.instance.getLanguage();
        
        sliderBGM.value = BGMVolime;
        sliderSE.value = SEVolime;
        dropdownLanguage.value = getLanguageIndex(language);
    }

    /** 關閉視窗 */
    public void onClosePopup() {
        DataManager.instance.setBGMVolime(BGMVolime);
        DataManager.instance.setSEVolime(SEVolime);
        DataManager.instance.setLanguage(language);
        this.gameObject.SetActive(false);
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
}
