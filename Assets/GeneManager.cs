using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class GeneManager : MonoBehaviour {

    public static float[,] gene = new float[50, 48];    //gene(each 48 for 50)
    public GameObject[] creatures = new GameObject[50]; //dragons (information)
    public GameObject gameobject;   //dragons (object)
    public GameObject spawnpoint;   //locate where dragons generated

    public Text generation;
    public Text bestdistence;
    public Text bestgene;
    public Text bestvariance;

    private float[] dis = new float[50];    //거리
    public float[,] ypos = new float[50, 30]; // 변위
    public float[,] ydev = new float[50, 30]; // 편차
    public float[] yvar = new float[50]; // 분산

    // Use this for initialization
    void Start () {
        StreamWriter record = new StreamWriter("test.txt");
        record.Close();

        //generate random gene when initialize
        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 48; y++) gene[x, y] = UnityEngine.Random.Range(-90, 90);
            for (int y = 0; y < 30; y++) ypos[x, y] = 0;
        }
        StartCoroutine(generate(1, 0, 0, new float[48]));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //배열 섞기 함수
    public static void ShuffleArray<T>(T[] array)
    {
        int random1;
        int random2;

        T tmp;

        for (int index = 0; index < array.Length; ++index)
        {
            random1 = UnityEngine.Random.Range(0, array.Length);
            random2 = UnityEngine.Random.Range(0, array.Length);

            tmp = array[random1];
            array[random1] = array[random2];
            array[random2] = tmp;
        }
    }

    //개체별 y position 측정
    IEnumerator measure(int index)
    {
        for (int i = 0; i < 50; i++)
        {
            ypos[i, index] = creatures[i].transform.position.y;
            if (ypos[i, index] > 7f) creatures[i].transform.position = new Vector3(-900, 3.4f, creatures[i].transform.position.z);
        }

        yield return new WaitForSeconds(1);
        if (index < 29) StartCoroutine(measure(index + 1));
    }

    //개체 생성 및 유전 알고리즘 적용
    IEnumerator generate(int g, float bd, float bv, float[] bg)
    {
        float bestdis = 0f;
        float bestvar = 0f;
        float[] bestg = new float[48];

        //화면출력
        generation.text = g.ToString();
        string a = ""; for (int i = 0; i < 48; i++) a += bg[i].ToString() + ' ';
        bestgene.text = a;
        bestdistence.text = bd.ToString();
        bestvariance.text = bv.ToString();

        //생성
        for (int i = 0; i < 50; i++)
        {
            creatures[i] = Instantiate(gameobject, spawnpoint.transform.position + new Vector3(0, 0, 20 * i - 500), spawnpoint.transform.rotation);
            creatures[i].transform.Rotate(0, 45, 0);
            creatures[i].name = i.ToString();
        }
        //y position 측정 시작
        StartCoroutine(measure(0));

        //30초 후
        yield return new WaitForSeconds(30);
        
        for (int i = 0; i < 50; i++)
        {
            //거리
            dis[i] = creatures[i].transform.position.x;

            //편차,분산
            yvar[i] = 0;
            for (int j = 0; j < 30; j++)
            {
                ydev[i, j] = ypos[i, j] - 3.4f;
                yvar[i] += ydev[i, j] * ydev[i, j];
            }
            yvar[i] /= 30f;

            //제거
            Destroy(creatures[i]);
        }

        //상위 20%(10개) 추출
        int[] topindex = new int[10];
        for (int x = 0; x < 10; x++)
        {
            float GOF = 0f;     //Goodness Of Fit
            int maxindex = 0;
            for (int y = 0; y < 50; y++)
            {
                if(dis[y] >= 0 && GOF <  dis[y]*dis[y] /*/ yvar[y]*/ && creatures[y].transform.position.y > 2.5f)
                {
                    GOF = dis[y] * dis[y] /*/ yvar[y]*/;
                    maxindex = y;
                }
            }
            //best properites
            if (x == 0)
            {
                bestdis = dis[maxindex];
                bestvar = yvar[maxindex];
                for (int k = 0; k < 48; k++) bestg[k] = gene[maxindex, k];
            }

            topindex[x] = maxindex;
            dis[maxindex] = 0;
        }

        //유전 알고리즘=======================================================
        int[] piv = new int[3];
        float[,] newgene = new float[50, 48];
        /*
        //상위 40%는 그대로 유지
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 48; y++)
            {
                newgene[x, y] = gene[topindex[x], y];
            }
        }
        */
        for (int x = 0; x < 50; x++)
        {
            //선택
            ShuffleArray(topindex);
            int[] gindex = { topindex[0], topindex[1] };
            float[,] gn = new float[2, 48];
            for (int i = 0; i < 48; i++) gn[0, i] = gene[gindex[0], i];
            for (int i = 0; i < 48; i++) gn[1, i] = gene[gindex[1], i];

            //교차
            for (int i = 0; i < 3; i++) piv[i] = Convert.ToInt32(UnityEngine.Random.Range(1, 49));
            //정렬
            if (piv[0] > piv[1])
            {
                int tmp;
                tmp = piv[0];
                piv[0] = piv[1];
                piv[1] = tmp;
            }
            if (piv[0] > piv[2])
            {
                int tmp;
                tmp = piv[0];
                piv[0] = piv[1];
                piv[1] = tmp;
            }
            if (piv[1] > piv[2])
            {
                int tmp;
                tmp = piv[0];
                piv[0] = piv[1];
                piv[1] = tmp;
            }

            for (int y = 0; y < 48; y++)
            {
                //대입
                if ((piv[0] < y && y <= piv[1]) || piv[2] < y) newgene[x, y] = gn[1, y];
                else if (y <= piv[0] || (piv[1] < y && y < piv[2])) newgene[x, y] = gn[0, y];
                //변이
                int mutrate = 10;
                int mut = Convert.ToInt32(UnityEngine.Random.Range(1, 100));
                if (mutrate >= mut) newgene[x, y] = UnityEngine.Random.Range(-90, 90);
            }
        }

        for (int x = 0; x < 50; x++) for (int y = 0; y < 48; y++) gene[x, y] = newgene[x, y];
        //========================================================================
        
        //파일로 출력
        File.AppendAllText("test.txt", g.ToString() + ',' + bestdis.ToString() + ',' + bestvar.ToString() + "\r\n");

        StartCoroutine(generate(g + 1, bestdis, bestvar, bestg));
    }
}