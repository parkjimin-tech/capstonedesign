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

        eCB.SetSortingLayerName("20");
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

        //Utils.tr(Utils.RoundToPlaces(Time.time), "Player.TakeTurn");

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

        foreach (CardBartok tCB in validCards)
        {
            if (Bartok.S.AttackValidPlay(tCB))
            {
                attackvalidCards.Add(tCB);
            }
        }

        if (attackvalidCards.Count == 0 && Bartok.attack_stack > 0)
		{
			for (int i = 0; i < Bartok.attack_stack; i++)
			{
				cb = AddCard(Bartok.S.Draw());
				cb.callbackPlayer = this;
			}
			Bartok.attack_stack = 0;
			return;
		}

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
                Bartok.S.jack = 1 * Bartok.S.queen;
                break;
            case 12:
                Bartok.S.queen *= -1;
                break;
            case 13:
                Bartok.S.king = 3 * Bartok.S.queen;
                break;
        }
    }

    public CardBartok selectCard(List<CardBartok> lCD)
    {
        List<CardBartok> suitfindList;
        List<CardBartok> rankfindList;
        List<List<CardBartok>> suitCount = new List<List<CardBartok>>(4);
        List<List<CardBartok>> rankCount = new List<List<CardBartok>>(13);

        suitCount[0] = lCD.FindAll(tmpsuit => tmpsuit.suit == "C");
        suitCount[1] = lCD.FindAll(tmpsuit => tmpsuit.suit == "D");
        suitCount[2] = lCD.FindAll(tmpsuit => tmpsuit.suit == "H");
        suitCount[3] = lCD.FindAll(tmpsuit => tmpsuit.suit == "S");

        int suitidx = -1;
        switch (Bartok.S.targetCard.suit)
        {
            case "C":
                suitidx = 0;
                break;
            case "D":
                suitidx = 1;
                break;
            case "H":
                suitidx = 2;
                break;
            case "S":
                suitidx = 3;
                break;
        }

        for (int i = 1; i <= 13; i++)
        {
            rankCount[i - 1] = lCD.FindAll(tmprank => tmprank.rank == i);
        }

        suitfindList = lCD.FindAll(tmpsuit => tmpsuit.suit == Bartok.S.targetCard.suit);
        rankfindList = lCD.FindAll(tmprank => tmprank.rank == Bartok.S.targetCard.rank);

        if(suitCount[suitidx].Count > 0 && rankCount[Bartok.S.targetCard.rank - 1].Count > 0)
        {
            if (suitCount[suitidx].Count > lCD.Count / 2)
            {
                return lCD[suitCount[suitidx].Count];
            }
            else
            {
                
                return lCD.Find(tmprank => tmprank.rank == lCD.Max().rank);
            }
        }

        foreach (CardBartok tCB in lCD)
        {
            if (Bartok.S.targetCard.rank == tCB.rank && Bartok.S.targetCard.suit == tCB.suit)
            {
                if (suitCount[suitidx].Count > lCD.Count / 2)
                {
                    return tCB;
                }
            }
            else if (Bartok.S.targetCard.rank == tCB.rank || Bartok.S.targetCard.suit == tCB.suit)
            {
                return tCB;
            }
        }

        if (Bartok.S.targetCard.rank == lCD[0].rank)
        {
            return lCD[0];
        }
        return lCD[0];
    }

    public void CBCallback(CardBartok tCB)
    {
        //Utils.tr(Utils.RoundToPlaces(Time.time), "Player.CBCallback()", tCB.name, "Player " + playerNum);
        Bartok.S.PassTurn();
    }
}
