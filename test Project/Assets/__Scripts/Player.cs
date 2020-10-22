using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerType
{
    human,
    ai
}

[System.Serializable]
public class Player
{
    public PlayerType type = PlayerType.ai;
    public int playerNum;
    public List<CardBartok> hand;
    public SlotDefBartok handSlotdef;

    public CardBartok AddCard(CardBartok eCB)
    {
        if (hand == null) hand = new List<CardBartok>();
        hand.Add(eCB);

        if(type == PlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();

            cards = cards.OrderBy(cd => cd.rank).ToArray();

            hand = new List<CardBartok>(cards);
        }

        eCB.SetSortingLayerName("10");
        eCB.eventualSortLayer = handSlotdef.layerName;

        FanHand();
        return eCB;
    }

    public CardBartok RemoveCard(CardBartok cb)
    {
        if (hand == null || !hand.Contains(cb)) return null;
        hand.Remove(cb);
        FanHand();
        return cb;
    }

    public void FanHand()
    {
        float startRot = handSlotdef.rot;
        if(hand.Count > 1)
        {
            startRot += Bartok.S.handFanDegrees * (hand.Count - 1) / 2;
        }

        Vector3 pos;
        float rot;
        Quaternion rotQ;

        for (int i = 0; i < hand.Count; i++)
        {
            rot = startRot - Bartok.S.handFanDegrees * i;
            rotQ = Quaternion.Euler(0, 0, rot);

            pos = Vector3.up * CardBartok.CARD_HEIGHT / 2f;
            pos = rotQ * pos;

            pos += handSlotdef.pos;

            pos.z = -0.5f * i;

            if(Bartok.S.phase != TurnPhase.idle)
            {
                hand[i].timeStart = 0;
            }

            hand[i].MoveTo(pos, rotQ);
            hand[i].state = CBState.toHand;

            //hand[i].transform.localPosition = pos;
            //hand[i].transform.rotation = rotQ;
            //hand[i].state = CBState.hand;

            hand[i].faceUP = (type == PlayerType.human);

            hand[i].eventualSortOrder = i * 4;
            //hand[i].SetSortOrder(i * 4);
        }
    }

    public void TakeTurn()
    {
        if (Bartok.S.jack != 0) Bartok.S.jack = 0;
        if (Bartok.S.king != 0) Bartok.S.king = 0;

        Utils.tr(Utils.RoundToPlaces(Time.time), "Player.TakeTurn");

        if (type == PlayerType.human) return;

		Bartok.S.phase = TurnPhase.waiting;
        CardBartok cb;

        List<CardBartok> validCards = new List<CardBartok>();
        List<CardBartok> attackvalidCards = new List<CardBartok>();

        foreach (CardBartok tCB in hand)
        {
            if(Bartok.S.ValidPlay(tCB))
            {
                validCards.Add(tCB);
            }
        }

        foreach (CardBartok tCB in hand)
        {
            if (Bartok.S.AttackValidPlay(tCB))
            {
                attackvalidCards.Add(tCB);
            }
        }

        //if (attackvalidCards.Count == 0 && Bartok.attack_stack > 0)
        //{
        //    for (int i = 0; i < Bartok.attack_stack; i++)
        //    {
        //        cb = AddCard(Bartok.S.Draw());
        //        cb.callbackPlayer = this;
        //    }
        //    Bartok.attack_stack = 0;
        //    return;
        //}

        if (validCards.Count == 0)
        {
            cb = AddCard(Bartok.S.Draw());
            cb.callbackPlayer = this;
            return;
        }

        cb = validCards[Random.Range(0, validCards.Count)];
        RemoveCard(cb);
        Bartok.S.MoveToTarget(cb);
        cb.callbackPlayer = this;
        switch (cb.rank)
        {
            case 1:
                if (cb.suit == "S")
                {
                    Bartok.attack_stack += 5;
                }
                else Bartok.attack_stack += 3;
                break;
            case 2:
                Bartok.attack_stack += 2;
                break;
            case 3:
                Bartok.attack_stack = 0;
                break;
            case 11:
                Bartok.S.jack = 1;
                break;
            case 12:
                Bartok.S.queen *= -1;
                break;
            case 13:
                Bartok.S.king = 3 * Bartok.S.queen;
                break;
        }
    }

    public void CBCallback(CardBartok tCB)
    {
        Utils.tr(Utils.RoundToPlaces(Time.time), "Player.CBCallback()", tCB.name, "Player " + playerNum);
        Bartok.S.PassTurn();
    }
}
