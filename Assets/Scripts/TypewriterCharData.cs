using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 字元資料
struct CharacterData {
    public Color32 color;           // 顏色
    public Vector2 position;        // 座標
    public float rotation;          // 角度
    public float scale;             // 大小

    public bool isAnimateFinish;    // 動畫是否開始
    public float alphaTimer;        // 透明度計時器
    public float positionTimer;     // 座標計時器
    public float rotationTimer;     // 角度計時器
    public float scaleTimer;        // 縮放計時器
};

static class TypewriterCharData {
    // 外部呼叫 --------------------------------------------------------------------------------------------------------------

    /** 重設字元物件資料 */
    static public void resetCharData(CharacterData item) {
        item.color = new Color32(255, 255, 255, 255);
        item.position = Vector3.zero;
        item.rotation = 0f;
        item.scale = 1.0f;

        item.isAnimateFinish = true;
        item.alphaTimer = 0;
        item.positionTimer = 0;
        item.rotationTimer = 0;
        item.scaleTimer = 0;
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

    /** 設定字元網格轉換 */
    static public bool setCharacterTransform(TMP_TextInfo textInfo, int index, Vector2 position, float rotation, float scale) {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int materialIndex = charInfo.materialReferenceIndex;
        int verticeIndex = charInfo.vertexIndex;
        TMP_MeshInfo meshInfo = textInfo.meshInfo[materialIndex];
        Vector3[] sourceVertices = meshInfo.vertices;
        Vector3 offset = (sourceVertices[0 + verticeIndex] + sourceVertices[2 + verticeIndex]) / 2;
        if (charInfo.elementType == TMP_TextElementType.Sprite) {
            verticeIndex = charInfo.spriteIndex;
        }

        if (charInfo.isVisible) {
            meshInfo.vertices[0 + verticeIndex] = meshInfo.vertices[0 + verticeIndex] - offset;
            meshInfo.vertices[1 + verticeIndex] = meshInfo.vertices[1 + verticeIndex] - offset;
            meshInfo.vertices[2 + verticeIndex] = meshInfo.vertices[2 + verticeIndex] - offset;
            meshInfo.vertices[3 + verticeIndex] = meshInfo.vertices[3 + verticeIndex] - offset;

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
