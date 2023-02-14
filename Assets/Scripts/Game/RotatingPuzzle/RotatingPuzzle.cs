using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RotatingPuzzle : BasePuzzle
{
    public float tileBetweenPx = 3.0f;  // 方塊之間間隔
    public GameObject tile;             // 方塊預置物

    private GameObject[,] tileObjectArray;          // 方塊物件清單
	private Vector3[,] tilePosArray;                // 方塊座標陣列
	private RotatingPuzzleTile emptyTile;            // 空方塊
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

    // 開始遊戲
    public override void startPuzzle() {
        Debug.Log("Start Rotating Puzzle!");
        createPuzzleTiles();
        jugglePuzzle();
        isPuzzleActive = true;
    }

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

    /** 處理方塊碰撞 */
    protected override void handleTileHit(RaycastHit2D hit) {
        if (!hit) {
            return;
        }
        RotatingPuzzleTile tmepTile = hit.transform.gameObject.GetComponent<RotatingPuzzleTile>();
        if (tmepTile) {
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
    }
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 快速完成謎題 */
    public override void quickFinishPuzzle() {
        RotatingPuzzleTile tmepTile;
        Vector2Int goalPos;
        for(int j = 0; j < puzzleGridY; j++){
			for(int i = 0; i < puzzleGridX; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<RotatingPuzzleTile>();
                goalPos = tmepTile.getGoalGridPos();
                tmepTile.setNowGridPos(goalPos);
                tmepTile.transform.localPosition = tilePosArray[goalPos.x, goalPos.y];
            }
        }
        finishPuzzle();
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 創造謎題方塊 */
    private void createPuzzleTiles()
    {
        float gridWidth = puzzleImage.rect.width/puzzleGridX;
        float gridHeight = puzzleImage.rect.height/puzzleGridY;
        Vector2 gridUnit = new Vector2(gridWidth/puzzleImage.pixelsPerUnit, gridHeight/puzzleImage.pixelsPerUnit);
        Vector3 scale = this.transform.localScale;
	    Vector3 position;
        GameObject tmepObject;
        RotatingPuzzleTile tmepTile;

        tileObjectArray = new GameObject[puzzleGridX, puzzleGridY];
        tilePosArray = new Vector3[puzzleGridX, puzzleGridY];

        for(int j = 0; j < puzzleGridY; j++) {
			for(int i = 0; i < puzzleGridX; i++) {
                Sprite tempSprite = Sprite.Create(
                    puzzleImage.texture,
                    new Rect(
                        i * gridWidth + tileBetweenPx * 0.5f,
                        j * gridHeight + tileBetweenPx * 0.5f,
                        gridWidth - tileBetweenPx,
                        gridHeight - tileBetweenPx
                    ),
                    new Vector2(0.5f, 0.5f)
                );

                position = new Vector3((i - (puzzleGridX - 1) * 0.5f) * gridUnit.x, 
                                        (j - (puzzleGridY - 1) * 0.5f) * gridUnit.y, 
                                        0.0f);
                tmepObject = Instantiate(tile, Vector3.zero, Quaternion.identity) as GameObject;
                tmepObject.transform.localScale = scale;
				tmepObject.gameObject.transform.parent = this.transform;

                tilePosArray[i,j] = position;
                tileObjectArray[i,j] = tmepObject;
                tmepObject.transform.localPosition = position;

                tmepTile = tmepObject.GetComponent<RotatingPuzzleTile>();
                tmepTile.init(new Vector2Int(i,j), tempSprite, gridUnit);
            }
        }
    }

    /** 洗謎題盤面 */
    private void jugglePuzzle() {
        int juggleCount = 2 * puzzleGridX * puzzleGridY; // 洗牌次數
        int count;
        Vector2Int randTile = new Vector2Int(0, 0);
        Vector2Int beforeTile = new Vector2Int(-1, -1);
        GameObject tmepObject = tileObjectArray[puzzleGridX - 1, 0]; // 右下角為空格
        RotatingPuzzleTile tmepTile;

        emptyTile = tmepObject.GetComponent<RotatingPuzzleTile>();
        emptyTile.gameObject.SetActive(false);

        count = 0;
        while(count < juggleCount) {
            randTile.x = UnityEngine.Random.Range(0, puzzleGridX);
            randTile.y = UnityEngine.Random.Range(0, puzzleGridY);
            tmepTile = tileObjectArray[randTile.x, randTile.y].GetComponent<RotatingPuzzleTile>();
            if (randTile != beforeTile && checkTileCanMove(tmepTile)) {
                beforeTile = emptyTile.getNowGridPos();
                exchangeTilePos(tmepTile, emptyTile);
                exchangeTileData(tmepTile, emptyTile);
                count++;
            }
		}
    }

    /** 互換方塊資料 */
    private void exchangeTileData(RotatingPuzzleTile tileA, RotatingPuzzleTile tileB) {
        GameObject tempObject = tileB.gameObject;
        Vector2Int tempPos = tileB.getNowGridPos();
        Vector2Int targetPos = tileA.getNowGridPos();

        tileObjectArray[tempPos.x, tempPos.y] = tileA.gameObject;
        tileObjectArray[targetPos.x, targetPos.y] = tempObject;
        
        tileA.setNowGridPos(tempPos);
        tileB.setNowGridPos(targetPos);
    }

    /** 互換方塊位置 */
    private void exchangeTilePos(RotatingPuzzleTile tileA, RotatingPuzzleTile tileB, bool isTween = false, System.Action callback = null) {
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
    private bool moveTileToEmptyPos(RotatingPuzzleTile thisTile, System.Action callback = null) {
        if (checkTileCanMove(thisTile)) {
            exchangeTilePos(thisTile, emptyTile, true, callback);
            exchangeTileData(thisTile, emptyTile);
            return true;
        }
        return false;
    }

    /** 檢查方塊是否可移動 */
    private bool checkTileCanMove(RotatingPuzzleTile thisTile)
	{
        if (thisTile != emptyTile
        && Vector2.Distance(thisTile.getNowGridPos(), emptyTile.getNowGridPos()) == 1) {
            return true;
        }
		return false;
	}

    /** 檢查是否獲勝 */
    private bool checkPuzzleComplete() {
        int completeCount = puzzleGridX * puzzleGridY;
        RotatingPuzzleTile tmepTile;

        for(int j = 0; j < puzzleGridY; j++){
			for(int i = 0; i < puzzleGridX; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<RotatingPuzzleTile>();
                if (tmepTile.checkGridCorrect()) {
                    completeCount--;
                }
            }
        }
        if (completeCount <= 0) {
            return true;
        }
        return false;
    }

    /** 清除拼圖 */
    private void clearPuzzleTile() {
        for(int j = 0; j < puzzleGridY; j++) {
			for(int i = 0; i < puzzleGridX; i++) {
                GameObject temp = tileObjectArray[i, j].gameObject;
                Destroy(temp);
                tileObjectArray[i, j] = null;
            }
        }
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
        RotatingPuzzleTile tmepTile;
        float delay = 0.02f;
        int count = 0;
        for(int j = puzzleGridY-1; j >= 0; j--){
			for(int i = 0; i < puzzleGridX; i++) {
                float delayTime = delay * count;
                tmepTile = tileObjectArray[i, j].GetComponent<RotatingPuzzleTile>();
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
