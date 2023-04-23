using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class JointController : MonoBehaviour {

    public GameObject[] joint = new GameObject[8];  //[jointB 1,2,3,4 jointA 1.2.3.4]

    private float[] gene = new float[48];
    private float[] angle = new float[12];
    private float[] mangle = new float[12];

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 48; i++)
        {
            gene[i] = GeneManager.gene[Convert.ToInt32(name), i];
        }
        StartCoroutine(Changemotion(1, 0));
    }
	
	// Update is called once per frame
	void Update () {
        Controljoint();
    }

    IEnumerator Changemotion(float delay, int num)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 12 * num; i < 12 * num + 11; i++) angle[i - 12 * num] = gene[i];
        StartCoroutine(Changemotion(1f, (num + 1) % 4));
    }

    public void Controljoint()
    {
        //jointA, jointB x축 회전
        for (int i = 0; i < 8; i++)
        {
            if (!float.IsNaN(mangle[i]))
            {
                if (mangle[i] < angle[i] /*&& angle[i] > 0*/)
                {
                    joint[i].transform.Rotate(1, 0, 0);
                    mangle[i] += 0.5f;
                }
                else if (mangle[i] > angle[i] /*&& angle[i] < 0*/)
                {
                    joint[i].transform.Rotate(-1, 0, 0);
                    mangle[i] -= 0.5f;
                }
            }

        }
        //jointA y축 회전
        for (int i = 8; i < 12; i++)
        {
            if (!float.IsNaN(mangle[i]))
            {
                if (mangle[i] < angle[i] /*&& angle[i] > 0*/)
                {
                    joint[i - 8].transform.Rotate(0, 1, 0);
                    mangle[i] += 0.5f;
                }
                else if (mangle[i] > angle[i] /*&& angle[i] < 0*/)
                {
                    joint[i - 8].transform.Rotate(0, -1, 0);
                    mangle[i] -= 0.5f;
                }
            }
        }
    }
}
