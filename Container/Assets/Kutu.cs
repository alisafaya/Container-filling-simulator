using System;
using UnityEngine;

class Kutu
{
    public float en { get; set; }
    public float boy { get; set; }
    public float derinlik { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float hacim { get; set; }
    public Color color { get; set; }

    public Kutu(float en, float boy, float derinlik, bool katmanmi , Color color)
    {
       // float tolerans = katmanmi == true ? 0 : -2;
        if (!katmanmi)
        {
            if (en > derinlik)
            {
                float gecici = derinlik;
                derinlik = en;
                en = gecici;
            }
            if (boy > derinlik)
            {
                float gecici = derinlik;
                derinlik = boy;
                boy = gecici;
            }
            if (en > boy)
            {
                float gecici = en;
                en = boy;
                boy = gecici;
            } 
        }
        this.hacim = en * boy * derinlik;
        this.en = en;
        this.boy = boy;
        this.derinlik = derinlik;
        this.color = color;

    }
    public void konumAyarla(float _x, float _y, float _z)
    {
        this.x = _x;        
        this.y = _y ;
        this.z = _z ;
    }
}