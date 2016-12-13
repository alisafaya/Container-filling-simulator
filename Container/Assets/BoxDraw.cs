using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class BoxDraw : MonoBehaviour {

    static List<Kutu> kutular = new List<Kutu>();

    static List<Kutu> sonuc_sira = new List<Kutu>();
    static List<Color> renkler = new List<Color>();// her kutuya farklı renk vermek için önceki renkleri listte tutup aynı varmı kontrol edicez
    void Start () {

        System.Random rnd = new System.Random();

        for (int i = 1; i < 50; i++)//i tane rasgele kutu at
        {
            int j = rnd.Next(1, 25);// aynı kutudan en fazla j tane olabilir

            int en = rnd.Next(30, 100);
            int boy = rnd.Next(30,100);
            int derinlik = rnd.Next(50,100);


            Color color;
            bool renkdowhile = false;// renk aynımı diye her renk atamasında kontrol et aynıysa yeniden ata yine kontrol et yeni atanan renkten yoksa döngüden çık
            do
            {
                color = new Color(((float)rnd.Next(20, 80)) / 50, ((float)rnd.Next(20, 80) / 50), ((float)rnd.Next(20, 80) / 50));// rengi ata

                renkdowhile = false;// durdurma kriterini sıfırla
               
                foreach (var item in renkler)
                {                   
                    if (item == color)
                    {
                        renkdowhile = true;// aynı renk var do while bir daha dönsün                       
                    }
                }
            } while (renkdowhile);
            renkler.Add(color);


            for (int a = 0; a < j; a++)
            {
                kutular.Add(new Kutu(en, boy, derinlik, false, color));
            }
        }



       

        Kutu katman = new Kutu(234, 234, 1200, true, new Color(1, 1, 1));// konteyner
        katman.konumAyarla(0, 0, 0);

        GameObject konteyner = GameObject.CreatePrimitive(PrimitiveType.Cube);

        konteyner.transform.position = new Vector3(katman.y + (katman.en / 2), katman.z + (katman.boy / 2), katman.x + (katman.derinlik / 2));
        konteyner.transform.localScale = new Vector3(katman.en, katman.boy, katman.derinlik);
        yerlestir(katman);

        /*---- Hata Kontrolü -----*/
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
        /*---- Hata Kontrolü -----*/


        foreach (var item in sonuc_sira)// ekrana çizdirme
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(item.y + (item.en / 2), item.z + (item.boy / 2), item.x + (item.derinlik / 2));
            cube.transform.localScale = new Vector3((item.en - 1), (item.boy - 1), (item.derinlik - 1));
            cube.GetComponent<Renderer>().material.SetColor("_Color", item.color);
        }


        Debug.Log("Tüm kutuların sayisi: " + kutular.Count.ToString() + " ve Hacmi :" + kutular.Sum(x => x.hacim).ToString());
        Debug.Log("Yerleştirilen kutu sayisi :" + sonuc_sira.Count.ToString() + ", hacmi :" + sonuc_sira.Sum(m => m.hacim).ToString());
      



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

        for (int i = 0; i < kutular.Count; i++)// ilk elemanı referansa ata hata varmı kontrol et varsa refesansı null 
            //yap ve bir sonraki elemana geç onu ata hata yoksa break ile döngüden çık
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


        if (referans == null)// eklenebilecek bir kutu yoksa fonksiyondan çık
        {
            return;
        }

        referans.konumAyarla(katman.x, katman.y, katman.z);// referansı katmanın 0 0 0 noktasına yerleştir

        for (int i = 1; i < kutular.Count; i++)// tüm kutuları dolan referansla aynı genişlik ve yükseklikte bir kutu varsa onuda bloğa ekle 
        {// böylece elde edilebilecek en büyük bloğu elde etmiş oluyoruz 

            if (referans.en == kutular[i].en && referans.boy == kutular[i].boy)
            {

                if ((blokDerinlik + kutular[i].derinlik) < katman.derinlik)
                {
                    kutular[i].konumAyarla((blok[blok.Count - 1].x + blok[blok.Count - 1].derinlik), blok[blok.Count - 1].y, blok[blok.Count - 1].z);


                    blok.Add(kutular[i]);
                    blokDerinlik += kutular[i].derinlik;
                    kutular.Remove(kutular[i]);
                }
            }
            else if (referans.en == kutular[i].kutuCevir(1).en && referans.boy == kutular[i].kutuCevir(1).boy)// en ve boyu değiştirerek kontrol et
            {
                kutular[i] = kutular[i].kutuCevir(1);
                if ((blokDerinlik + kutular[i].derinlik) < katman.derinlik)
                {
                    kutular[i].konumAyarla((blok[blok.Count - 1].x + blok[blok.Count - 1].derinlik), blok[blok.Count - 1].y, blok[blok.Count - 1].z);


                    blok.Add(kutular[i]);
                    blokDerinlik += kutular[i].derinlik;
                    kutular.Remove(kutular[i]);
                }
            }
            else if (referans.en == kutular[i].kutuCevir(2).en && referans.boy == kutular[i].kutuCevir(2).boy)//en ve derinliği değiştirerek kontrol et
            {
                kutular[i] = kutular[i].kutuCevir(2);
                if ((blokDerinlik + kutular[i].derinlik) < katman.derinlik)
                {
                    kutular[i].konumAyarla((blok[blok.Count - 1].x + blok[blok.Count - 1].derinlik), blok[blok.Count - 1].y, blok[blok.Count - 1].z);


                    blok.Add(kutular[i]);
                    blokDerinlik += kutular[i].derinlik;
                    kutular.Remove(kutular[i]);
                }
            }
            else if (referans.en == kutular[i].kutuCevir(3).en && referans.boy == kutular[i].kutuCevir(3).boy)// boy ve derinliği değiştirerek kontrol et
            {
                kutular[i] = kutular[i].kutuCevir(3);
                if ((blokDerinlik + kutular[i].derinlik) < katman.derinlik)
                {
                    kutular[i].konumAyarla((blok[blok.Count - 1].x + blok[blok.Count - 1].derinlik), blok[blok.Count - 1].y, blok[blok.Count - 1].z);


                    blok.Add(kutular[i]);
                    blokDerinlik += kutular[i].derinlik;
                    kutular.Remove(kutular[i]);
                }
            }
           

        }

        foreach (var item in blok)
        {
            sonuc_sira.Add(item);
        }

        Kutu katmanyan = new Kutu(katman.en - referans.en, katman.boy, blokDerinlik, true, new Color(1,1,1));// diğer katmanlar belirleniyor ve yerleştir yeniden çağrılıyor
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
