using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartBtn : MonoBehaviour
{
    public Image fadeImage;

    public void GoMainScene(float duration, string nextScene = null)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, duration).OnComplete(() =>
        {
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
        });
    }
}
