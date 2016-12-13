using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class BoxDraw : MonoBehaviour {

    static List<Kutu> kutular = new List<Kutu>();

    static List<Kutu> sonuc_sira = new List<Kutu>();
    void Start () {

        System.Random rnd = new System.Random();

        for (int i = 1; i < 50; i++)
        {
            int j = rnd.Next(1, 25);

            int en = rnd.Next(30, 100);
            int boy = rnd.Next(30,100);
            int derinlik = rnd.Next(50,110);
            Color color = new Color(((float)rnd.Next(20, 80)) / 50, ((float)rnd.Next(20, 80) / 50), ((float)rnd.Next(20, 80) / 50));
            for (int a = 0; a < j; a++)
            {
                kutular.Add(new Kutu(en, boy, derinlik, false,color));
            }
        }
        

        
        Debug.Log("kutular sayisi: " + kutular.Count);
        Kutu katman = new Kutu(234, 234, 1200, true, new Color(1, 1, 1));
        katman.konumAyarla(0, 0, 0);
        GameObject konteyner = GameObject.CreatePrimitive(PrimitiveType.Cube);

        konteyner.transform.position = new Vector3(katman.y + (katman.en / 2), katman.z + (katman.boy / 2), katman.x + (katman.derinlik / 2));
        konteyner.transform.localScale = new Vector3(katman.en, katman.boy, katman.derinlik);
        yerlestir(katman);

        List<Kutu> hatalar = new List<Kutu>();

        foreach (var item1 in sonuc_sira)
        {
            foreach (var item2 in sonuc_sira)
            {

                if (sonuc_sira.IndexOf(item1) != sonuc_sira.IndexOf(item2) && item1.x == item2.x && item1.y == item2.y && item1.z == item2.z)
                {
                    hatalar.Add(item2);
                }

            }
        }

        foreach (var item in hatalar)
        {
            sonuc_sira.Remove(item);
        }

        foreach (var item in sonuc_sira)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(item.y + (item.en / 2), item.z + (item.boy / 2), item.x + (item.derinlik / 2));
            cube.transform.localScale = new Vector3((item.en - 1), (item.boy - 1), (item.derinlik - 1));
            cube.GetComponent<Renderer>().material.SetColor("_Color", item.color);
        }
        Debug.Log("Sonuc Sayisi :" + sonuc_sira.Count);
        Debug.Log("kutu hacmi :" + sonuc_sira.Sum(m => m.hacim));



    }
	
	// Update is called once per frame
	void Update () {
	
	}

    
    static void yerlestir(Kutu katman)
    {
        //Debug.Log("Katman ozellikleri" + katman.derinlik + " " + katman.en + " " + katman.boy + " " + katman.x + " " + katman.y + " " + katman.z);

        kutular = kutular.OrderByDescending(x => x.hacim).ToList();
        float blokDerinlik = 0f;
        if (kutular.Count == 0)
        {
            return;
        }
        Kutu referans = null;
        List<Kutu> blok = new List<Kutu>();

        for (int i = 0; i < kutular.Count; i++)
        {
            referans = kutular[i];
            if (referans.boy < katman.boy && referans.derinlik < katman.derinlik && referans.en < katman.en)
            {
                blok.Add(referans);
                blokDerinlik += referans.derinlik;
                kutular.Remove(referans);
                break;
            }
            else
            {
                referans = null;
            }

        }

        if (referans == null)
        {
            return;
        }

        referans.konumAyarla(katman.x, katman.y, katman.z);

        for (int i = 1; i < kutular.Count; i++)
        {

            if (referans.en == kutular[i].en && referans.boy == kutular[i].boy)
            {

                if ((blokDerinlik + kutular[i].derinlik) > katman.derinlik)
                {
                    break;
                }

                kutular[i].konumAyarla((blok[blok.Count - 1].x + blok[blok.Count - 1].derinlik), blok[blok.Count - 1].y, blok[blok.Count - 1].z);


                blok.Add(kutular[i]);
                blokDerinlik += kutular[i].derinlik;
                kutular.Remove(kutular[i]);

            }

        }

        foreach (var item in blok)
        {
            sonuc_sira.Add(item);
        }

        Kutu katmanyan = new Kutu(katman.en - referans.en, katman.boy, blokDerinlik, true, new Color(1,1,1));
        katmanyan.konumAyarla(katman.x, (referans.en + katman.y), katman.z);

        Kutu katmanon = new Kutu(katman.en, katman.boy, (katman.derinlik - blokDerinlik), true, new Color(1, 1, 1));
        katmanon.konumAyarla((blokDerinlik + katman.x), katman.y, katman.z);

        Kutu katmanust = new Kutu(referans.en, (katman.boy - referans.boy), blokDerinlik, true, new Color(1, 1, 1));
        katmanust.konumAyarla(katman.x, katman.y, (katman.z + referans.boy));


        yerlestir(katmanyan);
        yerlestir(katmanon);
        yerlestir(katmanust);

    }

}
