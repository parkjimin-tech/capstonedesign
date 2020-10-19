using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    string[] CardSuitLetter;
    List<string> CardList;
    List<string> PlayerHand;
    List<string> AI1Hand;
    List<string> AI2Hand;
    List<string> AI3Hand;
    
    private void Awake()
    {
        CardSuitLetter = new string[]{"S","D","H","C"};
        CardList = new List<string>();
        PlayerHand = new List<string>();
        AI1Hand = new List<string>();
        AI2Hand = new List<string>();
        AI3Hand = new List<string>();
    }
    void Start()
    {
        string s;
        for (int i = 0;i < 4; i++)
        {
            for (int j = 1; j < 14; j++)
            {
                s = CardSuitLetter[i] + j.ToString();
                CardList.Add(s);
            }
        }

        List<string> tCards = new List<string>();

        int ndx;
        while (CardList.Count > 0)
        {
            ndx = Random.Range(0, CardList.Count);
            tCards.Add(CardList[ndx]);
            CardList.RemoveAt(ndx);
        }
        CardList = tCards;

        //for (int i = 0; i < CardList.Count; i++)
        //    print(CardList[i]);

        for (int i = 0; i < 7; i++)
        {
            ndx = Random.Range(0, CardList.Count);
            PlayerHand.Add(CardList[ndx]);
            CardList.RemoveAt(ndx);

            ndx = Random.Range(0, CardList.Count);
            AI1Hand.Add(CardList[ndx]);
            CardList.RemoveAt(ndx);

            ndx = Random.Range(0, CardList.Count);
            AI2Hand.Add(CardList[ndx]);
            CardList.RemoveAt(ndx);

            ndx = Random.Range(0, CardList.Count);
            AI3Hand.Add(CardList[ndx]);
            CardList.RemoveAt(ndx);
        }

        ndx = Random.Range(0, CardList.Count);
        string firstcard = CardList[ndx];
        CardList.RemoveAt(ndx);

        string hand = "";
        for (int i = 0; i < PlayerHand.Count; i++)
        {
            hand += PlayerHand[i] + " ";
        }
        print(hand);
        hand = "";

        for (int i = 0; i < AI1Hand.Count; i++)
        {
            hand += AI1Hand[i] + " ";
        }
        print(hand);
        hand = "";

        for (int i = 0; i < AI2Hand.Count; i++)
        {
            hand += AI2Hand[i] + " ";
        }
        print(hand);
        hand = "";

        for (int i = 0; i < AI3Hand.Count; i++)
        {
            hand += AI3Hand[i] + " ";
        }
        print(hand);
        hand = "";

        print(firstcard);
    }
}
