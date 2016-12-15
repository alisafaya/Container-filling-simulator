using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.IO;

public class BoxDraw : MonoBehaviour {

    static List<Kutu> kutular = new List<Kutu>();
    static int conut;
    public GameObject panel;

    static List<Kutu> sonuc_sira = new List<Kutu>();
    void Start()
    {
        System.Random rnd = new System.Random();


        string dosya_yolu = Application.dataPath + "/veriler.txt"; Debug.Log(dosya_yolu);
        //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
        FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
        //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
        //2.parametre dosyanın açılacağını,
        //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
        StreamReader sw = new StreamReader(fs);
        //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
        string yazi = sw.ReadLine();
        while (yazi != null)
        {
            Debug.Log(yazi);
            var liste = yazi.Split(' ');
            if (liste.Count() == 5)
            {
                int j = Convert.ToInt32(liste[4]);// aynı kutudan en fazla j tane olabilir

                int en = Convert.ToInt32(liste[3]);
                int boy = Convert.ToInt32(liste[1]);
                int derinlik = Convert.ToInt32(liste[2]);
                Color color = new Color(((float)rnd.Next(20, 80)) / 50, ((float)rnd.Next(20, 80) / 50), ((float)rnd.Next(20, 80) / 50));
                for (int a = 0; a < j; a++)
                {
                    kutular.Add(new Kutu(liste[0], en, boy, derinlik, false, color));
                }
            }
            yazi = sw.ReadLine();
        }
        //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
        //Son satır okunduktan sonra okuma işlemini bitirdik
        sw.Close();
        fs.Close();


        conut = kutular.Count;
        Debug.Log("kutular sayisi: " + kutular.Count);
        Kutu katman = new Kutu("Kat", 234, 234, 1200, true, new Color(1, 1, 1));
        katman.konumAyarla(0, 0, 0);
        yerlestir(katman);


        string raporyolu = Application.dataPath+"/rapor.txt";
        //İşlem yapacağımız dosyanın yolunu belirtiyoruz.
        if (!File.Exists(raporyolu))
        {
            File.Create(raporyolu);
        }
        fs = new FileStream(raporyolu, FileMode.OpenOrCreate, FileAccess.Write);

        StreamWriter swriter = new StreamWriter(fs);
        swriter.WriteLine("Adım" + "\t\t" + "Kod" + "\t" + " X " + "\t" + " Y " + "\t" + " Z " + "\t" + "Genişlik" + "\t" + "Yükseklik" + "\t" + "Derinlik");
        int adim = 1;
        foreach (var item1 in sonuc_sira)
        {
            swriter.WriteLine(adim++ + "\t\t" + item1.kod + "\t" + item1.x + "\t" + item1.y + "\t" + item1.z + "\t" + item1.en + "\t\t" + item1.boy + "\t\t" + item1.derinlik);
            swriter.Flush();
        }
        sw.Close();
        fs.Close();

        sayac = sonuc_sira.Count;
        panel.GetComponent<Text>().text =
            ("Veri sayisi: " + conut) + '\n' +
            ("Eklenen kutu Sayisi :" + sonuc_sira.Count) + '\n' +
            ("Eklenen kutu hacmi :" + sonuc_sira.Sum(m => m.hacim)).ToString() + '\n' +
            ("Yüzde doluluk oranı:" + string.Format("{0:0.00}", (sonuc_sira.Sum(m => m.hacim) / (234 * 234 * 1200)) * 100)) + '%' + '\n'+
            ("Yuzde Yerlestirme orani :" + string.Format("{0:0.00}", ((double)sonuc_sira.Count / (double)conut) * 100)) + '%'; 
        Time.timeScale = 0.5f;
    }
	
	static int sayac = 0;
	static int i = 0;
	// Update is called once per frame
	void FixedUpdate () {
		if ( i < sayac )
		{
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(sonuc_sira[i].y + (sonuc_sira[i].en / 2), sonuc_sira[i].z + (sonuc_sira[i].boy / 2), sonuc_sira[i].x + (sonuc_sira[i].derinlik / 2));
            cube.transform.localScale = new Vector3((sonuc_sira[i].en - 1), (sonuc_sira[i].boy - 1), (sonuc_sira[i].derinlik - 1));
            cube.GetComponent<Renderer>().material.SetColor("_Color", sonuc_sira[i].color);
			i++;
		}
        if (i == sayac)
        {
            string raporyolu = Application.dataPath + "/rapor.txt";
            System.Diagnostics.Process.Start(raporyolu);
            i++;
        }
    }

    
    static void yerlestir(Kutu katman)
    {
        //Debug.Log("Katman ozellikleri" + katman.derinlik + " " + katman.en + " " + katman.boy + " " + katman.x + " " + katman.y + " " + katman.z);

        kutular = kutular.OrderByDescending(x => x.hacim).ToList();
      
        float blokboy = 0f;
       
        if (kutular.Count == 0)
        {
            return;
        }
        Kutu referans = null;
        List<Kutu> blok = new List<Kutu>();

        for (int i = 0; i < kutular.Count; i++)
        {
            referans = kutular[i];
            if (referans.boy <= katman.boy && referans.derinlik <= katman.derinlik && referans.en <= katman.en)
            {
                blok.Add(referans);
                blokboy += referans.boy;
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

            if (referans.en == kutular[i].en && referans.boy == kutular[i].boy && referans.derinlik == kutular[i].derinlik)
            {

                if ((blokboy + kutular[i].boy) >= katman.boy)
                {
                    break;
                }
                kutular[i].konumAyarla(blok[blok.Count - 1].x , blok[blok.Count - 1].y, blok[blok.Count - 1].z+ blok[blok.Count - 1].boy);


                blok.Add(kutular[i]);
                blokboy += kutular[i].boy;
                kutular.Remove(kutular[i]);
               
            }

        }
		

        foreach (var item in blok)
        {
            sonuc_sira.Add(item);
        }

        Kutu katmanyan = new Kutu("Kat",katman.en - referans.en, katman.boy, referans.derinlik, true, new Color(1,1,1));
        katmanyan.konumAyarla(katman.x, (referans.en + katman.y), katman.z);

        Kutu katmanon = new Kutu("Kat", katman.en, katman.boy, (katman.derinlik - referans.derinlik), true, new Color(1, 1, 1));
        katmanon.konumAyarla((referans.derinlik + katman.x), katman.y, katman.z);

        Kutu katmanust = new Kutu("Kat",referans.en, (katman.boy - blokboy), referans.derinlik, true, new Color(1, 1, 1));
        katmanust.konumAyarla(katman.x, katman.y, (katman.z + blokboy));


		yerlestir(katmanust);
        yerlestir(katmanyan);
        yerlestir(katmanon);
        

    }

}
