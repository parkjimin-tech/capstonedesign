using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDefProspector
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}

public class LayOutProspector : MonoBehaviour
{
    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;
    public Vector2 multiplier;
    public List<SlotDefProspector> slotDefs;
    public SlotDefProspector drawPile;
    public SlotDefProspector discardPile;
    public string[] sortingLayerNames = new string[] {"Row0", "Row1", "Row2", "Row3", "Draw", "Discard"};

    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0];

        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        SlotDefProspector tSD;
        PT_XMLHashList slotX = xml["slot"];

        for(int i = 0; i < slotX.Count; i++)
        {
            tSD = new SlotDefProspector();
            if(slotX[i].HasAtt("type"))
            {
                tSD.type = slotX[i].att("type");
            }
            else
            {
                tSD.type = "slot";
            }

            tSD.x = float.Parse(slotX[i].att("x"));
            tSD.y = float.Parse(slotX[i].att("y"));
            tSD.layerID = int.Parse(slotX[i].att("layer"));
            tSD.layerName = sortingLayerNames[tSD.layerID];

            switch(tSD.type)
            {
            case "slot":
                tSD.faceUp = (slotX[i].att("faceup") == "1");
                tSD.id = int.Parse(slotX[i].att("id"));
                if(slotX[i].HasAtt("hiddenby"))
                {
                    string[] hiding = slotX[i].att("hiddenby").Split(',');
                    foreach(var s in hiding)
                    {
                        tSD.hiddenBy.Add(int.Parse(s));
                    }
                }
                slotDefs.Add(tSD);
                break;
            case "drawpile":
                tSD.stagger.x = float.Parse(slotX[i].att("xstagger"));
                drawPile = tSD;
                break;
            case "discardpile":
                discardPile = tSD;
                break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
