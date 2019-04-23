using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct DicePanelParam
{
    public string campName;
    public Sprite campIcon;
    public int diceRolls;
    public int diceResult;
    public DicePanelParam(string campName,Sprite campIcon,int diceRolls,int diceResult)
    {
        this.campName = campName;
        this.campIcon = campIcon;
        this.diceRolls = diceRolls;
        this.diceResult = diceResult;
    }

}

public class BattleDicePanel : UINode {
    public Animator ResultAnimator;
    public Animator RollDiceAnimator;
    [Header("进攻方属性")]
    [SerializeField]
    public Text attackCampName;
    [SerializeField]
    public Image attackCampIcon;
    [SerializeField]
    public Text attackCampBattleUnitCount;
    [SerializeField]
    public Text attackCampResult;
    public RectTransform attackResultTransform;
    [Space(10)]
    [Header("防守方属性")]
    public Text defenceCampName;
    [SerializeField]
    public Image defenceCampIcon;
    [SerializeField]
    public Text defenceCampBattleUnitCount;
    [SerializeField]
    public Text defenceCampResult;
    public RectTransform defenceResultTransform;

    public void RefreshAttackCampInfo(DicePanelParam dicePanelParam)
    {
        attackCampName.text = dicePanelParam.campName;
        attackCampIcon.sprite = dicePanelParam.campIcon;
        attackCampBattleUnitCount.text =string.Format("× {0}", dicePanelParam.diceRolls.ToString());
        attackCampResult.text = dicePanelParam.diceResult.ToString();
    }
    public void RefreshDefenceCampInfo(DicePanelParam dicePanelParam)
    {
        defenceCampName.text = dicePanelParam.campName;
        defenceCampIcon.sprite = dicePanelParam.campIcon;
        defenceCampBattleUnitCount.text = string.Format("× {0}", dicePanelParam.diceRolls.ToString());
        defenceCampResult.text = dicePanelParam.diceResult.ToString();
    }

    public float GetResultAniTime()
    {
        float rollDiceTime = RollDiceAnimator.runtimeAnimatorController.animationClips[0].length;
        float resultTime = ResultAnimator.runtimeAnimatorController.animationClips[0].length;
        foreach (AnimationClip aniClip in RollDiceAnimator.runtimeAnimatorController.animationClips)
        {
            Debug.Log("摇骰子动画名字是" + aniClip.name);
            Debug.Log("时间是" + aniClip.length);
        }
        foreach (AnimationClip aniClip in ResultAnimator.runtimeAnimatorController.animationClips)
        {
            Debug.Log("Result动画名字是" + aniClip.name);
            Debug.Log("时间是" + aniClip.length);
        }

        return  rollDiceTime+resultTime;   
    }


    public void ShowSelf()
    {
        attackResultTransform.anchoredPosition = new Vector3(2.5f,80f,0f);
        defenceResultTransform.anchoredPosition = new Vector3(2.5f,80f,0f);
        gameObject.SetActive(true);
    }

  


}
