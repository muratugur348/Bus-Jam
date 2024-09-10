using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using AssetKits.ParticleImage;
using BusJam.Scripts.Runtime;
using BusJam.Scripts.Runtime.Managers.LevelManager;

//using ElephantSDK;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public static event Action OnIncreaseTutorialPhaseValue;

    [Header("Tutorial")] public GameObject handUI;
    readonly WaitForSeconds tutorialWaitTime = new WaitForSeconds(2);
    public int tutorialPhase;
    public List<StickmanParent> tutorialStickmanParents = new List<StickmanParent>();

    private Vector3 _startScale;
    private ILevelManager _levelManager;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        _levelManager = Locator.Instance.Resolve<ILevelManager>();

        if ( _levelManager.GetFakeLevelIndex() > 1)
            yield break;


        _levelManager.isPlayingTutorial = true;
        _startScale = handUI.transform.localScale;
        for (int i = 1; i < tutorialStickmanParents.Count; i++)
        {
            tutorialStickmanParents[i].isBlockedToClick = true;
        }

        StartTutorialPhase();
    }

    private void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #region Tutorial

    private void StartTutorialPhase()
    {
        StartCoroutine(TutorialPhase());
    }

    private void HandScale()
    {
        handUI.transform.DOScale(handUI.transform.localScale * 1.1f, .3f).OnComplete(() =>
        {
            handUI.transform.DOScale(handUI.transform.localScale / 1.1f, .3f).OnComplete(() => { HandScale(); });
        });
    }

    private IEnumerator TutorialPhase()
    {
        if (tutorialPhase == 0)
        {
            if (!handUI.activeSelf)
                handUI.SetActive(true);

            tutorialStickmanParents[0].isBlockedToClick = false;
            handUI.transform.DOKill(false);
            handUI.transform.localScale = _startScale;

            handUI.transform.position = Camera.main.WorldToScreenPoint(tutorialStickmanParents[0].transform.position);
            HandScale();
        }
        else if (tutorialPhase == 1)
        {
            tutorialStickmanParents[1].isBlockedToClick = false;
            handUI.transform.DOKill(false);
            handUI.transform.localScale = _startScale;
            handUI.transform.DOMove(Camera.main.WorldToScreenPoint(tutorialStickmanParents[1].transform.position)
                                  , 0.5f)
                .OnComplete(() => { HandScale(); });
        }
        else if (tutorialPhase == 2)
        {
            tutorialStickmanParents[2].isBlockedToClick = false;
            handUI.transform.DOKill(false);
            handUI.transform.localScale = _startScale;
            handUI.transform.DOMove(Camera.main.WorldToScreenPoint(tutorialStickmanParents[2].transform.position)
                                   , 0.5f)
                .OnComplete(() => { HandScale(); });
        }


        yield return new WaitForSeconds(0.1f);
    }

    public void IncreaseTutorialPhaseValue()
    {
        if (!_levelManager.isPlayingTutorial)
            return;
        if (tutorialPhase < 3)
        {
            tutorialPhase++;
            //Elephant.Event("tutorial_step_passed", tutorialPhase, Params.New().Set("current_coin", ((long)ScoreManager.Instance.coin).ToString()).Set("current_level", LevelManager.Instance.totalPlayedLevel));
            PlayerPrefs.SetInt("tutorialPhase", tutorialPhase);
            StopAllCoroutines();
            StartCoroutine(TutorialPhase());
        }

        if (tutorialPhase == 3)
        {
            handUI.transform.DOKill(false);
            handUI.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => { handUI.SetActive(false); });
            for (int i = 0; i < tutorialStickmanParents.Count; i++)
            {
                tutorialStickmanParents[i].isBlockedToClick = false;
            }

            //headerParent.transform.DOScale(Vector3.zero,0.3f);
            _levelManager.isPlayingTutorial = false;
        }
    }

    #endregion
}