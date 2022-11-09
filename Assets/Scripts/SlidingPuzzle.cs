using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzle : MonoBehaviour
{

    public Texture2D puzzleImage;       // 謎題貼圖
	public int puzzleGridX = 3;         // 格子X軸數量
	public int puzzleGridY = 3;         // 格子Y軸數量
    public float tileBetweenPx = 1.0f;  // 方塊之間間隔
    public GameObject tile;             // 方塊預置物

    private List<GameObject> tileObjectList;    // 方塊物件清單
	private Vector3[,] tilePosArray;            // 方塊座標陣列
	private SlidingPuzzleTile emptyTile;        // 空方塊

    // 生命週期 --------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        if (puzzleImage) {
            CreatePuzzleTiles();
            jugglePuzzle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 內部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 創造謎題方塊 */
    private void CreatePuzzleTiles()
    {
	    Vector3 position;
        SpriteRenderer tileSpriteRenderer = tile.GetComponent<SpriteRenderer>();
        SpriteRenderer tmepSpriteRenderer;
        GameObject tmepObject;
        SlidingPuzzleTile tmepTile;
        float gridWidth = puzzleImage.width/puzzleGridX;
        float gridHeight = puzzleImage.height/puzzleGridY;

        tileObjectList = new List<GameObject>();
        tilePosArray = new Vector3[puzzleGridX, puzzleGridY];

        for(int j = 0; j < puzzleGridX; j++){
			for(int i = 0; i < puzzleGridY; i++) {
                position = new Vector3((i - (puzzleGridX - 1) * 0.5f) * tileSpriteRenderer.size.x / puzzleGridX, 
                                        (j - (puzzleGridY - 1) * 0.5f) * tileSpriteRenderer.size.y / puzzleGridY, 
                                        0.0f);
                tilePosArray[i,j] = position;
                tmepObject = Instantiate(tile, position, Quaternion.identity) as GameObject;
				tmepObject.gameObject.transform.parent = this.transform;
                tileObjectList.Add(tmepObject);

                tmepSpriteRenderer = tmepObject.GetComponent<SpriteRenderer>();
                tmepSpriteRenderer.sprite = Sprite.Create(puzzleImage,
                                                            new Rect(i * gridWidth, j * gridHeight, gridWidth, gridHeight),
                                                            new Vector2(0.5f, 0.5f));

                tmepTile = tmepObject.GetComponent<SlidingPuzzleTile>();
                tmepTile.setGoalGridPos(new Vector2Int(i,j));
                tmepTile.setNowGridPos(new Vector2Int(i,j));
            }
        }
    }

    /** 洗謎題盤面 */
    private void jugglePuzzle() {
        int juggleCount = 3 * puzzleGridX * puzzleGridY; // 洗牌次數
        int emptyTileCount = puzzleGridX - 1; // 右下角為空格
        int count, rand;
        SlidingPuzzleTile tmepTile;
        GameObject tmepObject;

        tmepObject = tileObjectList[emptyTileCount];
        emptyTile = tmepObject.GetComponent<SlidingPuzzleTile>();
        emptyTile.setGameActive(false);
        emptyTile.gameObject.SetActive(false);
        //Debug.Log(emptyTile.getNowGridPos());

        count = 0;
        while(count < juggleCount) {
            rand = UnityEngine.Random.Range(0, tileObjectList.Count);
            tmepTile = tileObjectList[rand].GetComponent<SlidingPuzzleTile>();
            if (moveTileToEmptyPos(tmepTile)) {
                count++;
            }
		}
    }

    /** 判斷方塊是否可移動 */
    private bool checkTileCanMove(SlidingPuzzleTile thisTile)
	{
        if (thisTile != emptyTile
        && Vector2.Distance(thisTile.getNowGridPos(), emptyTile.getNowGridPos()) == 1) {
            return true;
        }
		return false;
	}

    /** 移動方塊到空位置 */
    private bool moveTileToEmptyPos(SlidingPuzzleTile thisTile) {
        if (checkTileCanMove(thisTile)) {
            Vector2Int tempPos = emptyTile.getNowGridPos();
            Vector2Int targetPos = thisTile.getNowGridPos();
            thisTile.transform.position = tilePosArray[(int)tempPos.x, (int)tempPos.y];
            emptyTile.transform.position = tilePosArray[(int)targetPos.x, (int)targetPos.y];
            thisTile.setNowGridPos(tempPos);
            emptyTile.setNowGridPos(targetPos);
            //Debug.Log(emptyTile.getNowGridPos());
            return true;
        }
        return false;
    }
}
