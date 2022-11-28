using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 字元資料
struct CharacterData {
    public Vector2 basePosition;    // 座標
    public float baseRotation;      // 角度
    public float baseScale;         // 大小
    public Color32 baseColor;       // 顏色
    public Vector2 targetPosition;  
    public float targetRotation;    
    public float targetScale;       
    public Color32 targetColor;     

    public bool isAnimateFinish;    // 動畫是否開始
    public float timer;             // 計時器
    public float duration;          // 總時間
    public float positionStart;     // 座標計開始
    public float rotationStart;     // 角度計開始
    public float scaleStart;        // 縮放計開始
    public float alphaStart;        // 透明度開始
    public float colorStart;        // 顏色開始
};

static class TypewriterCharData {
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 重設字元物件資料 */
    static public void resetCharData(out CharacterData item) {
        item.basePosition = Vector3.zero;
        item.baseRotation = 0f;
        item.baseScale = 1.0f;
        item.baseColor = new Color32(255, 255, 255, 255);
        item.targetPosition = Vector3.zero;
        item.targetRotation = 0f;
        item.targetScale = 1.0f;
        item.targetColor = new Color32(255, 255, 255, 255);

        item.isAnimateFinish = true;
        item.timer = 0;
        item.duration = 0;
        item.positionStart = 0;
        item.rotationStart = 0;
        item.scaleStart = 0;
        item.alphaStart = 0;
        item.colorStart = 0;
    }

    /** 重設字元頂點位置 */
    static public bool resetCharacterVertex(TMP_TextInfo textInfo, int index) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.vertices[0 + verticeIndex] = charInfo.vertex_BL.position;
            meshInfo.vertices[1 + verticeIndex] = charInfo.vertex_TL.position;
            meshInfo.vertices[2 + verticeIndex] = charInfo.vertex_TR.position;
            meshInfo.vertices[3 + verticeIndex] = charInfo.vertex_BR.position;
            return true;
        }
        return false;
    }

    /** 設定字元透明度 */
    static public bool setCharacterAlpha(TMP_TextInfo textInfo, int index, byte alpha) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.colors32[0 + verticeIndex].a = alpha;
            meshInfo.colors32[1 + verticeIndex].a = alpha;
            meshInfo.colors32[2 + verticeIndex].a = alpha;
            meshInfo.colors32[3 + verticeIndex].a = alpha;
            return true;
        }
        return false;
    }

    /** 設定字元顏色 */
    static public bool setCharacterColor(TMP_TextInfo textInfo, int index, Color32 color) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.colors32[0 + verticeIndex] = color;
            meshInfo.colors32[1 + verticeIndex] = color;
            meshInfo.colors32[2 + verticeIndex] = color;
            meshInfo.colors32[3 + verticeIndex] = color;
            return true;
        }
        return false;
    }

    /** 設定字元網格轉換 */
    static public bool setCharacterTransform(TMP_TextInfo textInfo, int index, Vector3 position, float rotation, float scale) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        Vector3[] sourceVertices = meshInfo.vertices;
        // Vector3 offset = (sourceVertices[0 + verticeIndex] + sourceVertices[2 + verticeIndex]) / 2;
        Vector3 offset = (charInfo.vertex_BL.position + charInfo.vertex_TR.position) / 2;
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.vertices[0 + verticeIndex] = charInfo.vertex_BL.position - offset;
            meshInfo.vertices[1 + verticeIndex] = charInfo.vertex_TL.position - offset;
            meshInfo.vertices[2 + verticeIndex] = charInfo.vertex_TR.position - offset;
            meshInfo.vertices[3 + verticeIndex] = charInfo.vertex_BR.position - offset;

            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(0f, 0f, rotation), scale * Vector3.one);

            meshInfo.vertices[0 + verticeIndex] = matrix.MultiplyPoint3x4(meshInfo.vertices[0 + verticeIndex]);
            meshInfo.vertices[1 + verticeIndex] = matrix.MultiplyPoint3x4(meshInfo.vertices[1 + verticeIndex]);
            meshInfo.vertices[2 + verticeIndex] = matrix.MultiplyPoint3x4(meshInfo.vertices[2 + verticeIndex]);
            meshInfo.vertices[3 + verticeIndex] = matrix.MultiplyPoint3x4(meshInfo.vertices[3 + verticeIndex]);

            meshInfo.vertices[0 + verticeIndex] += offset;
            meshInfo.vertices[1 + verticeIndex] += offset;
            meshInfo.vertices[2 + verticeIndex] += offset;
            meshInfo.vertices[3 + verticeIndex] += offset;
            return true;
        }
        return false;
    }
}
