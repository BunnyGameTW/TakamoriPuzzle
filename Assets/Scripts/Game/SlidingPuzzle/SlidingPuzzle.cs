using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SlidingPuzzle : BaseGridPuzzle
{
	private Vector3[,] tilePosArray;                // 方塊座標陣列
	private SlidingPuzzleTile emptyTile;            // 空方塊
    Sequence tweener = null;    // 補間事件

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    // void Start() {}

    void OnDestroy() {
        if (tweener != null) {
            tweener.Kill();
        }
    }

    // Update is called once per frame
    // void Update() {}

    // 結束遊戲
    public override void finishPuzzle() {
        isPuzzleActive = false;
        runFinishEffect(() => {
            Debug.Log("Puzzle complete!");
            clearPuzzleTile();
            if (finishCallback != null) {
                finishCallback();
            }
        });
    }

    // 觸碰處理 --------------------------------------------------------------------------------------------------------------

    /** 處理觸碰方塊 */
    protected override void handleTouchTile(GameObject obj) {
        SlidingPuzzleTile tmepTile = obj.GetComponent<SlidingPuzzleTile>();
        if (tmepTile == null) {
            return;
        }
        moveTileToEmptyPos(tmepTile, () => {
            SoundManager.instance.playSE(
                SoundManager.instance.SE_puzzles[Random.Range(0, SoundManager.instance.SE_puzzles.Length)]);
            if (!isPuzzleActive) {
                finishPuzzle();
            }
        });
        if (checkPuzzleComplete()) {
            isPuzzleActive = false;
        }
    }
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 快速完成謎題 */
    public override void quickFinishPuzzle() {
        SlidingPuzzleTile tmepTile;
        Vector2Int goalPos;
        for(int j = 0; j < puzzleGridY; j++){
			for(int i = 0; i < puzzleGridX; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<SlidingPuzzleTile>();
                goalPos = tmepTile.getGoalGridPos();
                tmepTile.setNowGridPos(goalPos);
                tmepTile.transform.localPosition = tilePosArray[goalPos.x, goalPos.y];
            }
        }
        finishPuzzle();
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 創造謎題方塊 */
    protected override void createPuzzleTiles() {
        base.createPuzzleTiles();
        SlidingPuzzleTile tmepTile;
        tilePosArray = new Vector3[puzzleGridX, puzzleGridY];
        for(int j = 0; j < puzzleGridY; j++) {
			for(int i = 0; i < puzzleGridX; i++) {
                tilePosArray[i,j] = tileObjectArray[i,j].transform.localPosition;
                tmepTile = tileObjectArray[i,j].GetComponent<SlidingPuzzleTile>();
                tmepTile.setGoalGridPos(new Vector2Int(i,j));
                tmepTile.setNowGridPos(new Vector2Int(i,j));
            }
        }
    }

    /** 洗謎題盤面 */
    protected override void jugglePuzzle(System.Action callback) {
        int juggleCount = 2 * puzzleGridX * puzzleGridY; // 洗牌次數
        int count;
        Vector2Int randTile = new Vector2Int(0, 0);
        Vector2Int beforeTile = new Vector2Int(-1, -1);
        GameObject tmepObject = tileObjectArray[juggleRect.x + juggleRect.width - 1, juggleRect.y]; // 右下角為空格
        SlidingPuzzleTile tmepTile;

        emptyTile = tmepObject.GetComponent<SlidingPuzzleTile>();
        emptyTile.gameObject.SetActive(false);

        if (DataManager.instance.cheatingMode) {
            juggleCount = 1;
        }

        count = 0;
        while(count < juggleCount || checkPuzzleComplete()) {
            randTile.x = UnityEngine.Random.Range(juggleRect.x, juggleRect.x + juggleRect.width);
            randTile.y = UnityEngine.Random.Range(juggleRect.y, juggleRect.y + juggleRect.height);
            tmepTile = tileObjectArray[randTile.x, randTile.y].GetComponent<SlidingPuzzleTile>();
            if (randTile != beforeTile && checkTileCanMove(tmepTile)) {
                beforeTile = emptyTile.getNowGridPos();
                exchangeTilePos(tmepTile, emptyTile);
                exchangeTileData(tmepTile, emptyTile);
                count++;
            }
		}
        if (callback != null) {
            callback();
        }
    }

    /** 互換方塊資料 */
    private void exchangeTileData(SlidingPuzzleTile tileA, SlidingPuzzleTile tileB) {
        GameObject tempObject = tileB.gameObject;
        Vector2Int tempPos = tileB.getNowGridPos();
        Vector2Int targetPos = tileA.getNowGridPos();

        tileObjectArray[tempPos.x, tempPos.y] = tileA.gameObject;
        tileObjectArray[targetPos.x, targetPos.y] = tempObject;
        
        tileA.setNowGridPos(tempPos);
        tileB.setNowGridPos(targetPos);
    }

    /** 互換方塊位置 */
    private void exchangeTilePos(SlidingPuzzleTile tileA, SlidingPuzzleTile tileB, bool isTween = false, System.Action callback = null) {
        GameObject tempObject = tileB.gameObject;
        Vector2Int tempPos = tileB.getNowGridPos();
        Vector2Int targetPos = tileA.getNowGridPos();
        bool isTargetPos = false;

        if (tileA.getGoalGridPos() == tileB.getNowGridPos()) {
            isTargetPos = true;
        }

        if (isTween) {
            tileA.runMoveEffect(tilePosArray[tempPos.x, tempPos.y], isTargetPos, callback);
        }
        else {
            tileA.transform.localPosition = tilePosArray[tempPos.x, tempPos.y];
        }
        tileB.transform.localPosition = tilePosArray[targetPos.x, targetPos.y];
    }

    /** 移動方塊到空位置 */
    private bool moveTileToEmptyPos(SlidingPuzzleTile thisTile, System.Action callback = null) {
        if (checkTileCanMove(thisTile)) {
            exchangeTilePos(thisTile, emptyTile, true, callback);
            exchangeTileData(thisTile, emptyTile);
            return true;
        }
        return false;
    }

    /** 檢查方塊是否可移動 */
    private bool checkTileCanMove(SlidingPuzzleTile thisTile)
	{
        if (thisTile != emptyTile
        && Vector2.Distance(thisTile.getNowGridPos(), emptyTile.getNowGridPos()) == 1) {
            return true;
        }
		return false;
	}

    /** 結束效果 */
    private void runFinishEffect(System.Action callback = null) {
        emptyTile.runFadeInEffect(() => {
            SoundManager.instance.playSE(SoundManager.instance.SE_finish);
            handleFinishEffect(() => {
                SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
                sprite.color = new Color(1, 1, 1, 0);
                sprite.enabled = true;
                handleFadeIn(callback);
            });
        });
    }

    /** 處理結束效果 */
    private void handleFinishEffect(System.Action callback = null) {
        SlidingPuzzleTile tmepTile;
        float delay = 0.02f;
        int count = 0;
        for(int j = puzzleGridY-1; j >= 0; j--){
			for(int i = 0; i < puzzleGridX; i++) {
                float delayTime = delay * count;
                tmepTile = tileObjectArray[i, j].GetComponent<SlidingPuzzleTile>();
                if (count + 1 < (puzzleGridY * puzzleGridX)) {
                    tmepTile.runShineEffect(delayTime);
                }
                else {
                    tmepTile.runShineEffect(delayTime, callback);
                }
                count++;
            }
        }
    }

    /** 處理淡入動畫 */
    private void handleFadeIn(System.Action callback = null) {
        tweener = DOTween.Sequence();
        tweener.AppendInterval(0.1f);
        tweener.Append(DOTween.To(
            () => { return this.GetComponent<SpriteRenderer>().color; },
            (value) => { this.GetComponent<SpriteRenderer>().color = value; },
            new Color(1, 1, 1, 1),
            0.5f
        ).SetEase(Ease.InOutQuart));
        tweener.AppendInterval(0.1f);
        tweener.AppendCallback(() => {
            tweener = null;
            if (callback != null) {
                callback();
            }
        });
        tweener.Play();
    }
}
