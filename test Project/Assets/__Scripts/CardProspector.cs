using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardProspector : Card
{
    [Header("Set Dynamically : CardProspector")]
    public eCardState state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    public int layoutID;
    public SlotDefProspector slotDefs;

    override public void OnMouseUpAsButton() 
    {
        Prospector.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}
