using System;
using System.Drawing;

namespace TurkDamasi
{
	public class Tas
	{
		//Static De�i�kenler --------------------------------------------------
		public static Bitmap			bmpBeyazTas;
		public static Bitmap			bmpSiyahTas;
		public static Bitmap			bmpBeyazDama;
		public static Bitmap			bmpSiyahDama;
		//De�i�kenler ---------------------------------------------------------
		public TasRengi					Renk; //Ta��n rengi (Beyaz veya Siyah)
		public TasTipi					Tip; //Ta��n tipi (Normal veya Dama)
		private Tas						yenilenTas = null; //bu ta� taraf�ndan en son yenilen ya�
		private byte					x,y; //Ta��n tahdadaki konumu (8x8 i�in)
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
		//Public �ye fonksyonlar ----------------------------------------------
		public void Ciz(Tahta tahta, Graphics grafik)
		{
			//�izilecek ta��n g�r�nt�s� belirleniyor
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
			//Oyun alan�na ta��n g�r�nt�s� �iziliyor
			grafik.DrawImage(b,
				tahta.CerceveGenisligi+tahta.KareGenisligi*x,
				tahta.CerceveGenisligi+tahta.KareGenisligi*y);
		}
		//---------------------------------------------------------------------
		public bool Tasi(byte x, byte y)
		{
			//�nceki yenilen ta�� iptal et (hamle sonunda d��ar�dan kontrol edilebilir)
			yenilenTas = null;
			//�u anda ta� yiyebilir mi bunu ��ren
			bool ilkBastaYiyebilir = TasYiyebilir();
			//Hata Nedenleri: Bu durumlarda false d�nder
			//-> Do�rusal olmayan bi konumda hareket ediyor (hem x hem y farkl�)
			//-> Gidilmek istenen konum bo� de�il
			//-> Gidilmek istenen konum ile �imdiliki konum ayn�
			//-> Gidilmek istenen konum tahtan�n s�n�rlar� i�inde de�il
			if( (x!=this.x && y!=this.y) || (tahta.Taslar[x,y]!=null) ||
				(x==this.x && y==this.y) || (x<0 || x>7 || y<0 || y>7) ) return false;

			//Hata Nedenleri: Bu durumlarda false d�nder
			//-> Gidilecek konum ile �u andaki konum aras�nda kendi renginde ta� var
			//-> Aral�ks�z olarak iki ta� �st�nden atlamaya �al���l�yor
			//-> Ayn� anda birden �ok ta� yemek istiyor (s�rayla yemesi gerek oyuna g�re)
			//-> Dama olmad��� halde geri gitmeye �al���yor
			//-> Dama olmad��� halde ta� yemeden 2 kare birden gitmi�
			//-> Dama olmad��� halde 2 kareden fazla gitmi�
			int xDegisim = 0; //�imdiki konumundan istenilen konuma giderken
			int yDegisim = 0; //x ve y koordinatlar�n�n her ad�mdaki de�i�im miktarlar�
			if(x!=this.x) //x koordinat� farkl�ysa x de�i�ir
				xDegisim = (x>this.x)?(1):(-1);
			else //y koordinat� farkl�ysa y de�i�ir
				yDegisim = (y>this.y)?(1):(-1);
			if(Tip!=TasTipi.Dama) //Dama de�ilse ve geri gidiyorsa hata ver
				if((Renk==TasRengi.Beyaz && yDegisim>0) ||
					(Renk==TasRengi.Siyah && yDegisim<0)) return false;
			//�imdiki konumdan hedef konuma ilerle ve kontrol et
			int hareketMiktari = 1;
			int atlananTasSayisi = 0; //�st�nden atlanan ta� say�s�
			for(int ix = this.x+xDegisim, iy = this.y+yDegisim;	//ba�lang�� konumundan ba�la
				ix!=x || iy!=y;									//hedefe gelene kadar ilerle
				ix+=xDegisim, iy+=yDegisim)						//her ad�mda x veya y'yi de�i�tir
			{
				if(tahta.Taslar[ix,iy]!=null) //tahtan�n bu noktas� bo� de�ilse
				{
					if(tahta.Taslar[ix,iy].Renk==this.Renk || //bu ta� kendisiyle ayn� renkte ise
						atlananTasSayisi==1) return false; //ya da birden �ok ta� �st�nden atlan�yorsa
					atlananTasSayisi++;
					yenilenTas = tahta.Taslar[ix,iy];
				}
				hareketMiktari++;
			}
			//dama de�ilse ve ta� yemeden 1 kareden fazla gitmi�se ya da toplamda
			//2 kareden fazla gitmi�se hata ver
			if(Tip!=TasTipi.Dama)
				if((hareketMiktari==2 && atlananTasSayisi==0) || (hareketMiktari>2))
					return false;

			//�lk ba�ta ta� yiyebilece�i halde ta� yememi�se hata ver
			if(ilkBastaYiyebilir && yenilenTas==null) return false;

			//hata yoksa hamleyi yap
			tahta.Taslar[this.x,this.y] = null; //eski yerinden kald�r
			this.x = x; //yeni yerini belirle
			this.y = y;
			tahta.Taslar[x,y] = this; //ta�� tahtaya b�rak
			//e�er son s�raya ula�m��sa dama yap
			if(Tip!=TasTipi.Dama)
				if(y==0 || y==7)
					Tip = TasTipi.Dama;
			return true;
		}
		//---------------------------------------------------------------------
		public bool TasYiyebilir()
		{
			//E�er bu ta� �u anda ta� yiyebilecek pozisyonda ise true d�nderir
			if(Tip==TasTipi.Normal) //Dama olmayan bir ta� i�in..
			{
				//sa��nda yiyecek ta� varsa true d�nder
				if(x<6)
					if(tahta.Taslar[x+1,y]!=null && tahta.Taslar[x+2,y]==null)
						if(tahta.Taslar[x+1,y].Renk!=this.Renk)
							return true;
				//solunda yiyecek ta� varsa true d�nder
				if(x>1)
					if(tahta.Taslar[x-1,y]!=null && tahta.Taslar[x-2,y]==null)
						if(tahta.Taslar[x-1,y].Renk!=this.Renk)
							return true;
				//�n�nde yiyecek ta� varsa true d�nder
				if(Renk==TasRengi.Beyaz) //beyaz ta� i�in
				{
					if(y>1)
						if(tahta.Taslar[x,y-1]!=null && tahta.Taslar[x,y-2]==null)
							if(tahta.Taslar[x,y-1].Renk!=this.Renk)
								return true;
				}
				else //siyah ta� i�in
				{
					if(y<6)
						if(tahta.Taslar[x,y+1]!=null && tahta.Taslar[x,y+2]==null)
							if(tahta.Taslar[x,y+1].Renk!=this.Renk)
								return true;
				}
			}
			else //Dama olan bir ta� i�in
			{
			}
			return false;
		}
		//Public �zellikler ---------------------------------------------------
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
