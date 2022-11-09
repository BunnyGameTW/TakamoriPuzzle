using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPuzzle : MonoBehaviour
{

    public Texture2D puzzleImage;         // 謎題貼圖
	public int puzzleGridX = 3;         // 格子X軸數量
	public int puzzleGridY = 3;         // 格子Y軸數量
    public float tileBetweenPx = 1.0f;  // 方塊之間間隔
    public GameObject tile;             // 方塊預置物

    private GameObject[,] tileObjectArray;                      // 方塊物件陣列
	private List<Vector3>  tilePosList = new List<Vector3>();   // 方塊座標清單

    // 生命週期 --------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        CreatePuzzleTiles();
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
        float gridWidth = puzzleImage.width/puzzleGridX;
        float gridHeight = puzzleImage.height/puzzleGridY;

        tileObjectArray = new GameObject[puzzleGridX, puzzleGridY];

        for(int j = 0; j < puzzleGridX; j++){
			for(int i = 0; i < puzzleGridY; i++) {
                position = new Vector3(
                                        (i - (puzzleGridX - 1) * 0.5f) * tileSpriteRenderer.size.x / puzzleGridX, 
                                        (j - (puzzleGridY - 1) * 0.5f) * tileSpriteRenderer.size.y / puzzleGridY, 
                                        0.0f
                                    );
                tileObjectArray[i,j] = Instantiate(tile, position, Quaternion.identity) as GameObject;
				tileObjectArray[i,j].gameObject.transform.parent = this.transform;

                tmepSpriteRenderer = tileObjectArray[i,j].GetComponent<SpriteRenderer>();
                tmepSpriteRenderer.sprite = Sprite.Create(
                                                            puzzleImage,
                                                            new Rect(i * gridWidth, j * gridHeight, gridWidth, gridHeight),
                                                            new Vector2(0.5f, 0.5f)
                                                        );
            }
        }
    }
}
