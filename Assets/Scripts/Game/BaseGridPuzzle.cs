using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class BaseGridPuzzle : BasePuzzle
{
    public GameObject tile;                     // 方塊預置物
    protected GameObject[,] tileObjectArray;    // 方塊物件清單
    protected float tileBetweenPx = 3.0f;       // 方塊之間間隔
	protected int puzzleGridX = 3;              // 格子X軸數量
	protected int puzzleGridY = 3;              // 格子Y軸數量

    // 生命週期 --------------------------------------------------------------------------------------------------------------

    // 初始化
    public override void init(Sprite image) {
        base.init(image);
        puzzleGridX = DataManager.instance.puzzleGridX;
        puzzleGridY = DataManager.instance.puzzleGridY;
    }
    
    // 開始遊戲
    public override void startPuzzle() {
        Debug.Log("Start Sliding Puzzle!");
        createPuzzleTiles();
        jugglePuzzle(() => {
            isPuzzleActive = true;
        });
    }

    // 結束遊戲
    public override void finishPuzzle() {
        isPuzzleActive = false;
        Debug.Log("Puzzle complete!");
        clearPuzzleTile();
        if (finishCallback != null) {
            finishCallback();
        }
    }

    // 觸碰處理 --------------------------------------------------------------------------------------------------------------
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 創造謎題方塊 */
    protected virtual void createPuzzleTiles() {
        float gridWidth = puzzleImage.rect.width/puzzleGridX;
        float gridHeight = puzzleImage.rect.height/puzzleGridY;
        Vector2 gridUnit = new Vector2(gridWidth/puzzleImage.pixelsPerUnit, gridHeight/puzzleImage.pixelsPerUnit);
        Vector3 scale = this.transform.localScale;
	    Vector3 position;
        GameObject tmepObject;
        BasePuzzleGridTile tmepTile;

        tileObjectArray = new GameObject[puzzleGridX, puzzleGridY];

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

                tileObjectArray[i,j] = tmepObject;
                tmepObject.transform.localPosition = position;

                tmepTile = tmepObject.GetComponent<BasePuzzleGridTile>();
                tmepTile.init(tempSprite, gridUnit);
            }
        }
    }

    
    /** 洗謎題盤面 */
    protected virtual void jugglePuzzle(System.Action callback) {
        if (callback != null) {
            callback();
        }
    }

    /** 檢查是否獲勝 */
    protected virtual bool checkPuzzleComplete() {
        int completeCount = puzzleGridX * puzzleGridY;
        BasePuzzleGridTile tmepTile;

        for(int j = 0; j < puzzleGridY; j++){
			for(int i = 0; i < puzzleGridX; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<BasePuzzleGridTile>();
                if (tmepTile.checkTileCorrect()) {
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
    protected virtual void clearPuzzleTile() {
        for(int j = 0; j < puzzleGridY; j++) {
			for(int i = 0; i < puzzleGridX; i++) {
                GameObject temp = tileObjectArray[i, j].gameObject;
                Destroy(temp);
                tileObjectArray[i, j] = null;
            }
        }
    }
    
}
