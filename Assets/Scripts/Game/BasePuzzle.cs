using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class BasePuzzle : MonoBehaviour
{
    protected Sprite puzzleImage;                   // 謎題貼圖
    protected bool isPuzzleActive = false;          // 謎題是否開始
    protected System.Action finishCallback = null;  // 謎題完成callback

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPuzzleActive) {
            return;
        }
        //判斷平台
		#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                onTouchDown();
            }
		#else
            if(!EventSystem.current.IsPointerOverGameObject()) {
                onMouseDown();
            }
		#endif
    }

    // 初始化
    public virtual void init(Sprite image) {
        puzzleImage = image;
        this.GetComponent<SpriteRenderer>().sprite = puzzleImage;
    }

    // 開始遊戲
    public virtual void startPuzzle() {
        isPuzzleActive = true;
    }

    // 結束遊戲
    public virtual void finishPuzzle() {
        isPuzzleActive = false;
        if (finishCallback != null) {
            finishCallback();
        }
    }

    // 觸碰處理 --------------------------------------------------------------------------------------------------------------

    /** 滑鼠觸碰 */
    void onMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            handleRaycast(Input.mousePosition);
        }
    }

    /** 手機觸碰 */
    void onTouchDown() {
        if (Input.touchCount != 1) {
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began) {
            handleRaycast(touch.position);
        }
    }

    /** 處理觸碰 */
    private void handleRaycast(Vector3 position) {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit) {
            handleTileHit(hit);
        }
    }

    /** 處理方塊碰撞 */
    protected void handleTileHit(RaycastHit2D hit) {
        if (!hit) {
            return;
        }
        GameObject tmepTile = hit.transform.gameObject;
        handleTouchTile(tmepTile);
    }

    /** 處理觸碰方塊 */
    protected abstract void handleTouchTile(GameObject obj);
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 設定謎題完成callback */
    public void setFinishPuzzleCallback(System.Action callback) {
        finishCallback = callback;
    }

    /** 快速完成謎題 */
    public abstract void quickFinishPuzzle();

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------
}
