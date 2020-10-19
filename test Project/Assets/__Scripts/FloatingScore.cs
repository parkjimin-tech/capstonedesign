using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eFSState
{
    idle,
    pre,
    active,
    post
}

public class FloatingScore : MonoBehaviour
{
   [Header("Set Dynamically")]
   public eFSState state = eFSState.idle;

   [SerializeField]
   protected int _score = 0;
   public string scoreString;

   public int score
   {
       get
       {
           return _score;
       }
       set
       {
           _score = value;
           scoreString = _score.ToString("N0");
           GetComponent<Text>().text = scoreString;

           GameObject go = GameObject.Find("enemy1");
           
       }
   }

   public List<Vector2> bezierPts;
   public List<float> fontSizes;
   public float timeStart = -1f;
   public float timeDuration = 1f;
   public string easingCurve = Easing.InOut;

   public GameObject reportFinishTo = null;

   private RectTransform rectTrans;
   private Text txt;

   public void Init(List<Vector2> ePts, float eTimes = 0, float eTimeD = 1)
   {
       rectTrans = GetComponent<RectTransform>();
       rectTrans.anchoredPosition = Vector2.zero;

       txt = GetComponent<Text>();

       bezierPts = new List<Vector2>(ePts);

       if(ePts.Count == 1)
       {
           transform.position = ePts[0];
           return;
       }

       if(eTimes == 0) eTimes = Time.time;
       timeStart = eTimes;
       timeDuration = eTimeD;

       state = eFSState.pre;
   }

   public void FScallback(FloatingScore fs)
   {
       score += fs.score;
   }

    private void Update() 
    {
        if(state == eFSState.idle) return;

        float u = (Time.time - timeStart) / timeDuration;
        float uC = Easing.Ease(u, easingCurve);

        if(u < 0)
        {
            state = eFSState.pre;
            txt.enabled = false;
        }
        else
        {
            if(u >= 1)
            {
                uC = 1;
                state = eFSState.post;
                if(reportFinishTo != null)
                {
                    reportFinishTo.SendMessage("FScallback", this);
                    Destroy(gameObject);
                }
                else
                {
                    state = eFSState.idle;
                }
            }
            else
            {
                state = eFSState.active;
                txt.enabled = true;
            }

            Vector2 pos = Utils.Bezier(uC, bezierPts);
            rectTrans.anchorMin = rectTrans.anchorMax = pos;

            if(fontSizes != null && fontSizes.Count > 0)
            {
                int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
                GetComponent<Text>().fontSize = size;
            }
        }
    }
}
