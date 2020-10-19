using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDefBartok
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>(); // Unused
    public float rot;
    public string type = "slot";
    public Vector2 stagger;
    public int player;
    public Vector3 pos;
}

public class LayoutBartok : MonoBehaviour
{
    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;
    public Vector2 multiplier;

    public List<SlotDefBartok> slotDefs;
    public SlotDefBartok drawpile;
    public SlotDefBartok discardpile;
    public SlotDefBartok target;

    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0];

        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        SlotDefBartok tSD;
        PT_XMLHashList slotX = xml["slot"];

        for (int i = 0; i < slotX.Count; i++)
        {
            tSD = new SlotDefBartok();
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
            tSD.pos = new Vector3(tSD.x * multiplier.x, tSD.y * multiplier.y, 0);

            tSD.layerID = int.Parse(slotX[i].att("layer"));
            tSD.layerName = tSD.layerID.ToString();

            switch (tSD.type)
            {
                case "slot":
                    break;

                case "drawpile":
                    tSD.stagger.x = float.Parse(slotX[i].att("xstagger"));
                    drawpile = tSD;
                    break;

                case "discardpile":
                    discardpile = tSD;
                    break;

                case "target":
                    target = tSD;
                    break;

                case "hand":
                    tSD.player = int.Parse(slotX[i].att("player"));
                    tSD.rot = float.Parse(slotX[i].att("rot"));
                    slotDefs.Add(tSD);
                    break;
            }
        }
    }
}
