using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RotatingPuzzle : BaseGridPuzzle
{
	private Vector3[,] tilePosArray;                // 方塊座標陣列
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
        RotatingPuzzleTile tmepTile = obj.GetComponent<RotatingPuzzleTile>();
        SpriteRenderer tmepSpr = obj.GetComponent<SpriteRenderer>();
        if (tmepTile == null) {
            return;
        }
        tmepSpr.sortingOrder = 1;
        tmepTile.runRotateTile(90, () => {
            tmepSpr.sortingOrder = 0;
            SoundManager.instance.playSE(
                SoundManager.instance.SE_puzzles[Random.Range(0, SoundManager.instance.SE_puzzles.Length)]);
            checkTileCorrect(tmepTile);
        });
    }

    /** 檢查方塊是否正確 */
    private void checkTileCorrect(RotatingPuzzleTile tile) {
        if (checkPuzzleComplete()) {
            isPuzzleActive = false;
            tile.runCorrectEffect(() => {
                finishPuzzle();
            });
        }
        else {
            tile.runMistakeEffect();
        }
    }
    
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 快速完成謎題 */
    public override void quickFinishPuzzle() {
        RotatingPuzzleTile tmepTile;
        for(int j = 0; j < puzzleGridY; j++){
			for(int i = 0; i < puzzleGridX; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<RotatingPuzzleTile>();
                tmepTile.clearTweener();
                tmepTile.setTileAngle(0);
            }
        }
        finishPuzzle();
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 創造謎題方塊 */
    protected override void createPuzzleTiles() {
        base.createPuzzleTiles();
    }

    /** 洗謎題盤面 */
    protected override void jugglePuzzle(System.Action callback) {
        Vector2Int randRange = new Vector2Int(1, 8);
        int ROTATE_ANGLE = 90;
        RotatingPuzzleTile tmepTile;
        int randAngle;
        int count = puzzleGridX * puzzleGridY;
        for(int j = 0; j < puzzleGridY; j++) {
			for(int i = 0; i < puzzleGridX; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<RotatingPuzzleTile>();
                randAngle = UnityEngine.Random.Range(randRange.x, randRange.y);
                tmepTile.setTileAngle(ROTATE_ANGLE * randAngle);
                tmepTile.runJuggleEffect(() => {
                    count--;
                    if (count <= 0) {
                        if (callback != null) {
                            callback();
                        }
                    }
                });
            }
        }
    }

    /** 結束效果 */
    private void runFinishEffect(System.Action callback = null) {
        tweener = DOTween.Sequence();
        tweener.AppendInterval(0.5f);
        tweener.AppendCallback(() => {
            tweener = null;
            SoundManager.instance.playSE(SoundManager.instance.SE_finish);
            handleFinishEffect(() => {
                SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
                sprite.color = new Color(1, 1, 1, 0);
                sprite.enabled = true;
                handleFadeIn(callback);
            });
        });
        tweener.Play();
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
