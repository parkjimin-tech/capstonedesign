using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour
{
    static public Prospector S;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;

    public Vector3 layoutCenter;
    public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
    public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
    public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
    public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
    public float reloadDelay = 2f;
    public Text gameOverText, roundResultText, highScoreText;

    [Header("Set Dynamically")]
    public Deck deck;
    public LayOutProspector layout;
    public List<CardProspector> drawpile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;
    public FloatingScore fsRun;

    void Awake() 
    {
        S = this;
        SetUpUITexts();
    }

    void SetUpUITexts()
    {
        GameObject go = GameObject.Find("High Score");
        if(go != null)
        {
            highScoreText = go.GetComponent<Text>();
            int highScore = ScoreManager.HIGH_SCORE;
            string hScore = "High Score : " + highScore.ToString("N0");
            highScoreText.text = hScore;
        }

        go = GameObject.Find("GameOver");
        if (go != null)
        {
            gameOverText = go.GetComponent<Text>();
        }

        go = GameObject.Find("RoundResult");
        if (go != null)
        {
            roundResultText = go.GetComponent<Text>();
        }

        ShowResultUI(false);
    }

    void ShowResultUI(bool show)
    {
        gameOverText.gameObject.SetActive(show);
        roundResultText.gameObject.SetActive(show);
    }

    void Start()
    {
        Scoreboard.S.score = ScoreManager.SCORE;

        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);

        Deck.Shuffle(ref deck.cards);

        Card c;
        for(int cNum = 0; cNum < deck.cards.Count; cNum++)
        {
            c = deck.cards[cNum];
            c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        }

        layout = GetComponent<LayOutProspector>();
        layout.ReadLayout(layoutXML.text);

        drawpile = convertListCardsToListCardProspector(deck.cards);
        LayoutGame();
    }

    List<CardProspector> convertListCardsToListCardProspector(List<Card> lCD)
    {
        List<CardProspector> lCP = new List<CardProspector>();
        CardProspector tCP;
        foreach(var tCD in lCD)
        {
            tCP = tCD as CardProspector;
            lCP.Add(tCP);
        }
        return lCP;
    }

    CardProspector Draw()
    {
        CardProspector cd = drawpile[0];
        drawpile.RemoveAt(0);
        return cd;
    }

    void LayoutGame()
    {
        if(layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.position = layoutCenter;
        }

        CardProspector cp;
        foreach(var tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUP = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3(
                layout.multiplier.x * tSD.x, 
                layout.multiplier.y * tSD.y, 
                -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDefs = tSD;
            cp.state = eCardState.tableau;

            cp.SetSortingLayerName(tSD.layerName);

            tableau.Add(cp);
        }

        foreach(var tCP in tableau)
        {
            foreach(var hid in tCP.slotDefs.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }

        MoveToTarget(Draw());

        UpdateDrawPile();
    }

    CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach(var tCP in tableau)
        {
            if(tCP.layoutID == layoutID)
            {
                return tCP;
            }
        } 
        return null;
    }

    void SetTableauFaces()
    {
        foreach(var cd in tableau)
        {
            bool faceUp = true;
            foreach(var cover in cd.hiddenBy)
            {
                if(cover.state == eCardState.tableau)
                {
                    faceUp = false;
                }
            }
            cd.faceUP = faceUp;
        }
    }

    void MoveToDisCard(CardProspector cd)
    {
        cd.state = eCardState.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID + 0.5f);
        cd.faceUP = true;
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    void MoveToTarget(CardProspector cd)
    {
        if(target != null) MoveToDisCard(target);
        target = cd;
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;
        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID + 0.5f);

        cd.faceUP = true;
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }

    void UpdateDrawPile()
    {
        CardProspector cd;
        for(int i = 0; i < drawpile.Count; i++)
        {
            cd = drawpile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.stagger;
             cd.transform.localPosition = new Vector3(
            layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
            layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
            -layout.discardPile.layerID + 0.1f * i);

            cd.faceUP = false;
            cd.state = eCardState.drawpile;
            cd.SetSortingLayerName(layout.drawPile.layerName);
            cd.SetSortOrder(-10 * i);
        }
    }

    public void CardClicked(CardProspector cd)
    {
        switch(cd.state)
        {
        case eCardState.target:
            break;
        case eCardState.drawpile:
            MoveToTarget(Draw());
            UpdateDrawPile();
            ScoreManager.EVENT(eScoreEvent.draw);
            FloatingScoreHandler(eScoreEvent.draw);
            break;
        case eCardState.tableau:
            bool validMatch = true;
            if(!cd.faceUP)
            {
                validMatch = false;
            }
            if(!AdjacentRank(cd, target))
            {
                validMatch = false;
            }
            if(!validMatch) return;

            tableau.Remove(cd);
            MoveToTarget(cd);
            SetTableauFaces();
            ScoreManager.EVENT(eScoreEvent.mine);
            FloatingScoreHandler(eScoreEvent.mine);
            break;
        }

        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if(tableau.Count == 0)
        {
            GameOver(true);
            return;
        }

        if(drawpile.Count > 0)
        {
            return;
        }

        foreach(var cd in tableau)
        {
            if(AdjacentRank(cd, target))
            {
                return;
            }
        }

        GameOver(false);
    }

    void GameOver(bool won)
    {
        int score = ScoreManager.SCORE;
        if (fsRun != null) score += fsRun.score;

        if(won)
        {
            gameOverText.text = "Round Over";
            roundResultText.text = "You Won this round!\nRound Score:" + score;
            ShowResultUI(true);
            ScoreManager.EVENT(eScoreEvent.gameWin);
            FloatingScoreHandler(eScoreEvent.gameWin);
        }
        else
        {
            gameOverText.text = "Game Over";
            if(ScoreManager.HIGH_SCORE <= score)
            {
                string str = "You got the high score!\nHigh Score:" + score;
                roundResultText.text = str;
            }
            else
            {
                roundResultText.text = "Your final score was:" + score;
            }
            
            ShowResultUI(true);

            ScoreManager.EVENT(eScoreEvent.gameloss);
            FloatingScoreHandler(eScoreEvent.gameloss);
        }

        Invoke("ReloadLevel", reloadDelay);
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    bool AdjacentRank(CardProspector c0, CardProspector c1)
    {
        if(!c0.faceUP || !c1.faceUP) return false;

        int diff = Mathf.Abs(c0.rank - c1.rank);

        if(diff == 1 || diff == 12) return true;

        return false;
    }

    void FloatingScoreHandler(eScoreEvent evt)
    {
        List<Vector2> fsPts;

        switch (evt)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameloss:
                if(fsRun != null)
                {
                    fsPts = new List<Vector2>();
                    fsPts.Add(fsPosRun);
                    fsPts.Add(fsPosMid2);
                    fsPts.Add(fsPosEnd);
                    fsRun.reportFinishTo = Scoreboard.S.gameObject;
                    fsRun.Init(fsPts, 0, 1);
                    fsRun.fontSizes = new List<float>(new float[] { 29, 36, 4 });
                    fsRun = null;
                }
                break;
            case eScoreEvent.mine:
                FloatingScore fs;

                Vector2 p0 = Input.mousePosition;
                p0.x /= Screen.width;
                p0.y /= Screen.height;

                fsPts = new List<Vector2>();
                fsPts.Add(p0);
                fsPts.Add(fsPosMid);
                fsPts.Add(fsPosRun);

                fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
                fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
                if (fsRun != null)
                {
                    fsRun = fs;
                    fsRun.reportFinishTo = null;
                }
                else
                {
                    fs.reportFinishTo = fsRun.gameObject;
                }
                break;
        }
    }
}
