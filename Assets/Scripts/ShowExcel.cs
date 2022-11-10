using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowExcel : MonoBehaviour
{
    int id;
    public TextMeshProUGUI textPro;
    public Text text;
    public ScriptableObject content;
    Entity_content entity_Content;
    // Start is called before the first frame update
    void Start()
    {
        id = 0;
        entity_Content = (Entity_content)content;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        text.text = entity_Content.sheets[0].list[id].zh;
        textPro.text = entity_Content.sheets[0].list[id].zh;
        id++;
    }
}
