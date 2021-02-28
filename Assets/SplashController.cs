using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    public CanvasGroup cg1;
    public CanvasGroup cg2;
    public CanvasGroup cg3;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        cg1.alpha = 0;
        cg2.alpha = 0;
        cg3.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 1f)
        {
            cg1.alpha = 0;
            cg2.alpha = 0;
            cg3.alpha = 0;
        }else if (timer < 2f)
        {
            cg1.alpha = timer - 1f;
            cg2.alpha = 0;
            cg3.alpha = 0;
        }else if (timer < 4f)
        {
            cg1.alpha = 1;
            cg2.alpha = 0;
            cg3.alpha = 0;
        }else if (timer < 5f)
        {
            cg1.alpha = Mathf.Abs(5f - timer);
            cg2.alpha = 0;
            cg3.alpha = 0;
        }else if (timer < 6f)
        {
            cg1.alpha = 0;
            cg2.alpha = timer - 5f;
            cg3.alpha = 0;
        }else if (timer < 8f)
        {
            cg1.alpha = 0;
            cg2.alpha = 1;
            cg3.alpha = 0;
        }else if(timer < 9)
        {
            cg1.alpha = 0;
            cg2.alpha = Mathf.Abs(9f - timer);
            cg3.alpha = 0;
        }else if (timer < 10f)
        {
            cg1.alpha = 0;
            cg2.alpha = 0;
            cg3.alpha = timer - 9f;
        }else if (timer < 12f)
        {
            cg1.alpha = 0;
            cg2.alpha = 0;
            cg3.alpha = 1;
        }else if(timer < 13)
        {
            cg1.alpha = 0;
            cg2.alpha = 0;
            cg3.alpha = Mathf.Abs(13f - timer);
        }else if(timer < 13.5f)
        {
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}