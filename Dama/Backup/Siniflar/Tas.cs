using System;
using System.Drawing;

namespace TurkDamasi
{
	public class Tas
	{
		//Static Deðiþkenler --------------------------------------------------
		public static Bitmap			bmpBeyazTas;
		public static Bitmap			bmpSiyahTas;
		public static Bitmap			bmpBeyazDama;
		public static Bitmap			bmpSiyahDama;
		//Deðiþkenler ---------------------------------------------------------
		public TasRengi					Renk; //Taþýn rengi (Beyaz veya Siyah)
		public TasTipi					Tip; //Taþýn tipi (Normal veya Dama)
		private Tas						yenilenTas = null; //bu taþ tarafýndan en son yenilen yaþ
		private byte					x,y; //Taþýn tahdadaki konumu (8x8 için)
		private Tahta					tahta;
		//Kurucu fonksyon -----------------------------------------------------
		public Tas(Tahta Tahta, TasRengi Renk, byte X, byte Y)
		{
			this.tahta = Tahta;
			this.Renk = Renk;
			this.Tip = TasTipi.Normal;
			this.x = X;
			this.y = Y;
		}
		//Public static fonksyonlar -------------------------------------------
		public static void GrafikleriAl(string grafikDizini)
		{
			Tas.bmpBeyazTas = new Bitmap(grafikDizini + "beyazTas.png");
			Tas.bmpBeyazDama = new Bitmap(grafikDizini + "beyazDama.png");
			Tas.bmpSiyahTas = new Bitmap(grafikDizini + "siyahTas.png");
			Tas.bmpSiyahDama = new Bitmap(grafikDizini + "siyahDama.png");
		}
		//Public Üye fonksyonlar ----------------------------------------------
		public void Ciz(Tahta tahta, Graphics grafik)
		{
			//Çizilecek taþýn görüntüsü belirleniyor
			Bitmap b = null;
			if(Renk==TasRengi.Beyaz)
			{
				if(Tip==TasTipi.Normal)
					b = bmpBeyazTas;
				else
					b = bmpBeyazDama;
			}
			else
			{
				if(Tip==TasTipi.Normal)
					b = bmpSiyahTas;
				else
					b = bmpSiyahDama;
			}
			//Oyun alanýna taþýn görüntüsü çiziliyor
			grafik.DrawImage(b,
				tahta.CerceveGenisligi+tahta.KareGenisligi*x,
				tahta.CerceveGenisligi+tahta.KareGenisligi*y);
		}
		//---------------------------------------------------------------------
		public bool Tasi(byte x, byte y)
		{
			//Önceki yenilen taþý iptal et (hamle sonunda dýþarýdan kontrol edilebilir)
			yenilenTas = null;
			//Þu anda taþ yiyebilir mi bunu öðren
			bool ilkBastaYiyebilir = TasYiyebilir();
			//Hata Nedenleri: Bu durumlarda false dönder
			//-> Doðrusal olmayan bi konumda hareket ediyor (hem x hem y farklý)
			//-> Gidilmek istenen konum boþ deðil
			//-> Gidilmek istenen konum ile þimdiliki konum ayný
			//-> Gidilmek istenen konum tahtanýn sýnýrlarý içinde deðil
			if( (x!=this.x && y!=this.y) || (tahta.Taslar[x,y]!=null) ||
				(x==this.x && y==this.y) || (x<0 || x>7 || y<0 || y>7) ) return false;

			//Hata Nedenleri: Bu durumlarda false dönder
			//-> Gidilecek konum ile þu andaki konum arasýnda kendi renginde taþ var
			//-> Aralýksýz olarak iki taþ üstünden atlamaya çalýþýlýyor
			//-> Ayný anda birden çok taþ yemek istiyor (sýrayla yemesi gerek oyuna göre)
			//-> Dama olmadýðý halde geri gitmeye çalýþýyor
			//-> Dama olmadýðý halde taþ yemeden 2 kare birden gitmiþ
			//-> Dama olmadýðý halde 2 kareden fazla gitmiþ
			int xDegisim = 0; //þimdiki konumundan istenilen konuma giderken
			int yDegisim = 0; //x ve y koordinatlarýnýn her adýmdaki deðiþim miktarlarý
			if(x!=this.x) //x koordinatý farklýysa x deðiþir
				xDegisim = (x>this.x)?(1):(-1);
			else //y koordinatý farklýysa y deðiþir
				yDegisim = (y>this.y)?(1):(-1);
			if(Tip!=TasTipi.Dama) //Dama deðilse ve geri gidiyorsa hata ver
				if((Renk==TasRengi.Beyaz && yDegisim>0) ||
					(Renk==TasRengi.Siyah && yDegisim<0)) return false;
			//þimdiki konumdan hedef konuma ilerle ve kontrol et
			int hareketMiktari = 1;
			int atlananTasSayisi = 0; //üstünden atlanan taþ sayýsý
			for(int ix = this.x+xDegisim, iy = this.y+yDegisim;	//baþlangýç konumundan baþla
				ix!=x || iy!=y;									//hedefe gelene kadar ilerle
				ix+=xDegisim, iy+=yDegisim)						//her adýmda x veya y'yi deðiþtir
			{
				if(tahta.Taslar[ix,iy]!=null) //tahtanýn bu noktasý boþ deðilse
				{
					if(tahta.Taslar[ix,iy].Renk==this.Renk || //bu taþ kendisiyle ayný renkte ise
						atlananTasSayisi==1) return false; //ya da birden çok taþ üstünden atlanýyorsa
					atlananTasSayisi++;
					yenilenTas = tahta.Taslar[ix,iy];
				}
				hareketMiktari++;
			}
			//dama deðilse ve taþ yemeden 1 kareden fazla gitmiþse ya da toplamda
			//2 kareden fazla gitmiþse hata ver
			if(Tip!=TasTipi.Dama)
				if((hareketMiktari==2 && atlananTasSayisi==0) || (hareketMiktari>2))
					return false;

			//Ýlk baþta taþ yiyebileceði halde taþ yememiþse hata ver
			if(ilkBastaYiyebilir && yenilenTas==null) return false;

			//hata yoksa hamleyi yap
			tahta.Taslar[this.x,this.y] = null; //eski yerinden kaldýr
			this.x = x; //yeni yerini belirle
			this.y = y;
			tahta.Taslar[x,y] = this; //taþý tahtaya býrak
			//eðer son sýraya ulaþmýþsa dama yap
			if(Tip!=TasTipi.Dama)
				if(y==0 || y==7)
					Tip = TasTipi.Dama;
			return true;
		}
		//---------------------------------------------------------------------
		public bool TasYiyebilir()
		{
			//Eðer bu taþ þu anda taþ yiyebilecek pozisyonda ise true dönderir
			if(Tip==TasTipi.Normal) //Dama olmayan bir taþ için..
			{
				//saðýnda yiyecek taþ varsa true dönder
				if(x<6)
					if(tahta.Taslar[x+1,y]!=null && tahta.Taslar[x+2,y]==null)
						if(tahta.Taslar[x+1,y].Renk!=this.Renk)
							return true;
				//solunda yiyecek taþ varsa true dönder
				if(x>1)
					if(tahta.Taslar[x-1,y]!=null && tahta.Taslar[x-2,y]==null)
						if(tahta.Taslar[x-1,y].Renk!=this.Renk)
							return true;
				//önünde yiyecek taþ varsa true dönder
				if(Renk==TasRengi.Beyaz) //beyaz taþ için
				{
					if(y>1)
						if(tahta.Taslar[x,y-1]!=null && tahta.Taslar[x,y-2]==null)
							if(tahta.Taslar[x,y-1].Renk!=this.Renk)
								return true;
				}
				else //siyah taþ için
				{
					if(y<6)
						if(tahta.Taslar[x,y+1]!=null && tahta.Taslar[x,y+2]==null)
							if(tahta.Taslar[x,y+1].Renk!=this.Renk)
								return true;
				}
			}
			else //Dama olan bir taþ için
			{
			}
			return false;
		}
		//Public Özellikler ---------------------------------------------------
		public byte X
		{
			set	{ x = value; }
			get	{ return x; }
		}
		public byte Y
		{
			set	{ y = value; }
			get	{ return y;	}
		}
		public Tas SonYenilenTas
		{
			get { return yenilenTas; }
		}
	}
}
