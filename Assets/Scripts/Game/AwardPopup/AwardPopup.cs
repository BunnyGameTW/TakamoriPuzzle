using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AwardPopup : MonoBehaviour
{
    public Image awardImage;       // 獎勵cg
    public TextMeshProUGUI textTitle;
    private bool isAnimate = false;         // 是否在動畫中
    private System.Action touchCallback;    // 觸碰事件
    Animator ani;

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    public void init(Sprite image, string titleString) {
        awardImage.sprite = image;
        textTitle.text = titleString;
        ani = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 顯示動畫 */
    public void show() {
        isAnimate = true;
        this.gameObject.SetActive(true);
        StartCoroutine(handleShowEffect());
        ani.SetBool("isShow", false);
    }

    /** 觸碰螢幕 */
    public void onTouch() {
        if (isAnimate) {
            return;
        }
        if (touchCallback != null) {
            touchCallback();
        }
    }

    /** 設定觸碰事件 */
    public void setTouchCallback(System.Action callback) {
        touchCallback = callback;
    }


    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 處理動畫效果 */
    private IEnumerator handleShowEffect() {
        yield return new WaitForSeconds(1.0f);
        isAnimate = false;
    }
}
