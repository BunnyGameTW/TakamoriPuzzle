using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzle : MonoBehaviour
{
    public Sprite puzzleImage;       // 謎題貼圖
	public int puzzleGridX = 3;         // 格子X軸數量
	public int puzzleGridY = 3;         // 格子Y軸數量
    public float tileBetweenPx = 1.0f;  // 方塊之間間隔
    public GameObject tile;             // 方塊預置物

    private GameObject[,] tileObjectArray;      // 方塊物件清單
	private Vector3[,] tilePosArray;            // 方塊座標陣列
	private SlidingPuzzleTile emptyTile;        // 空方塊
    private bool isPuzzleActive = false;        // 謎題是否開始

    // 生命週期 --------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        isPuzzleActive = false;
        if (puzzleImage) {
            startPuzzle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPuzzleActive) {
            return;
        }
        onMouseDown();
        onTouchDown();
    }

    // 初始化
    public void init(Sprite image, int gridX, int gridY) {
        puzzleImage = image;
        puzzleGridX = gridX;
        puzzleGridY = gridY;
        startPuzzle();
    }

    // 開始遊戲
    public void startPuzzle() {
        createPuzzleTiles();
        jugglePuzzle();
        isPuzzleActive = true;
    }

    // 結束遊戲
    public void finishPuzzle() {
        isPuzzleActive = false;
        emptyTile.gameObject.SetActive(true);
        Debug.Log("Puzzle complete!");
    }

    // 觸碰處理 --------------------------------------------------------------------------------------------------------------

    /** 滑鼠觸碰 */
    void onMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit) {
                handleTileHit(hit);
            }
        }
    }

    /** 手機觸碰 */
    void onTouchDown() {
        // TODO: 手機觸碰
    }

    /** 處理方塊碰撞 */
    private void handleTileHit(RaycastHit2D hit) {
        if (!hit) {
            return;
        }
        SlidingPuzzleTile tmepTile = hit.transform.gameObject.GetComponent<SlidingPuzzleTile>();
        if (tmepTile) {
            moveTileToEmptyPos(tmepTile);
            if (checkPuzzleComplete()) {
                finishPuzzle();
            }
        }
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
        SlidingPuzzleTile tmepTile;

        tileObjectArray = new GameObject[puzzleGridX, puzzleGridY];
        tilePosArray = new Vector3[puzzleGridX, puzzleGridY];

        for(int j = 0; j < puzzleGridX; j++){
			for(int i = 0; i < puzzleGridY; i++) {
                Sprite tempSprite = Sprite.Create(
                    puzzleImage.texture,
                    new Rect(i * gridWidth, j * gridHeight, gridWidth, gridHeight),
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

                tmepTile = tmepObject.GetComponent<SlidingPuzzleTile>();
                tmepTile.init(new Vector2Int(i,j), tempSprite, gridUnit);
            }
        }
    }

    /** 洗謎題盤面 */
    private void jugglePuzzle() {
        int juggleCount = 4 * puzzleGridX * puzzleGridY; // 洗牌次數
        int count, randX, randY;
        GameObject tmepObject = tileObjectArray[puzzleGridX - 1, 0]; // 右下角為空格
        SlidingPuzzleTile tmepTile;

        emptyTile = tmepObject.GetComponent<SlidingPuzzleTile>();
        emptyTile.gameObject.SetActive(false);
        //Debug.Log(emptyTile.getNowGridPos());

        count = 0;
        while(count < juggleCount) {
            randX = UnityEngine.Random.Range(0, puzzleGridX);
            randY = UnityEngine.Random.Range(0, puzzleGridY);
            tmepTile = tileObjectArray[randX, randY].GetComponent<SlidingPuzzleTile>();
            if (moveTileToEmptyPos(tmepTile)) {
                count++;
            }
		}
    }

    /** 移動方塊到空位置 */
    private bool moveTileToEmptyPos(SlidingPuzzleTile thisTile) {
        if (checkTileCanMove(thisTile)) {
            GameObject tempObject = emptyTile.gameObject;
            Vector2Int tempPos = emptyTile.getNowGridPos();
            Vector2Int targetPos = thisTile.getNowGridPos();

            tileObjectArray[tempPos.x, tempPos.y] = thisTile.gameObject;
            tileObjectArray[targetPos.x, targetPos.y] = tempObject;

            thisTile.transform.localPosition = tilePosArray[tempPos.x, tempPos.y];
            emptyTile.transform.localPosition = tilePosArray[targetPos.x, targetPos.y];
            
            thisTile.setNowGridPos(tempPos);
            emptyTile.setNowGridPos(targetPos);
            //Debug.Log(emptyTile.getNowGridPos());
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

    /** 檢查是否獲勝 */
    private bool checkPuzzleComplete() {
        int completeCount = puzzleGridX * puzzleGridY;
        SlidingPuzzleTile tmepTile;

        for(int j = 0; j < puzzleGridX; j++){
			for(int i = 0; i < puzzleGridY; i++) {
                tmepTile = tileObjectArray[i, j].GetComponent<SlidingPuzzleTile>();
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
}
