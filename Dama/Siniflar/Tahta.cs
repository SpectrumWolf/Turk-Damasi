using System;
using System.Windows.Forms;
using System.Drawing;

namespace TurkDamasi
{
	public class Tahta
	{
		//Static de�i�kenler --------------------------------------------------
		public static Bitmap		bmpOyunTahtasi;
		public static Bitmap		bmpSiyahKare;
		public static Bitmap		bmpBeyazKare;
		//De�i�kenler ---------------------------------------------------------
		private	Tas[,]				taslar = new Tas[8,8]; // tahtan�n durumu
		public int					KareGenisligi = 75; //tahta resminin bir karesinin geni�li�i
		public int					CerceveGenisligi = 10; //tahtan�n �er�eve geni�li�i
		public PictureBox			pbOyunAlani; //Oyun alan�n�n �st�ne �izildi�i picturebox
		private TasTasima			tasima = TasTasima.TasimaYok;
		private TasRengi			hamleSirasi = TasRengi.Beyaz;
		//Kurucu fonksyon -----------------------------------------------------
		public Tahta(PictureBox oyunAlani)
		{
			this.pbOyunAlani = oyunAlani;
			if(this.pbOyunAlani!=null)
			{
				this.pbOyunAlani.MouseLeave+=new EventHandler(pbOyunAlani_MouseLeave);
				this.pbOyunAlani.MouseMove+=new MouseEventHandler(pbOyunAlani_MouseMove);
				this.pbOyunAlani.MouseDown+=new MouseEventHandler(pbOyunAlani_MouseDown);
			}
		}
		//Public Static Fonksyonlar
		public static void GrafikleriAl(string grafikDizini)
		{
			//Oyun tahtas�n�n g�r�nt�s�n� al
			Tahta.bmpOyunTahtasi = new Bitmap(grafikDizini + "tahta.gif");
			//Karelerin g�r�nt�s�n� al
			Tahta.bmpBeyazKare = new Bitmap(grafikDizini + "beyazKare.gif");
			Tahta.bmpSiyahKare = new Bitmap(grafikDizini + "kirmiziKare.gif");
		}
		//Public �ye fonksyonlar -----------------------------------------------
		public void YeniOyunHazirla()
		{
			//ta�lar�n tamam�n� bo�alt
			taslariTemizle();
			//yeni ta�lar� olu�tur
			for(byte x=0; x<8; x++)
			{
				taslar[x,1] = new Tas(this, TasRengi.Siyah, x, 1);
				taslar[x,2] = new Tas(this, TasRengi.Siyah, x, 2);
				taslar[x,5] = new Tas(this, TasRengi.Beyaz, x, 5);
				taslar[x,6] = new Tas(this, TasRengi.Beyaz, x, 6);
			}
		}
		//---------------------------------------------------------------------
		public void Ciz()
		{
			//Bo� bir Bitmap olu�tur
			Bitmap goruntu = new Bitmap(620,620);
			//Grafik nesnesi olu�tur
			Graphics grafik = Graphics.FromImage(goruntu);
			//Oyun tahtas�n� �iz
			grafik.DrawImage(bmpOyunTahtasi, 0, 0);
			//T�m ta�lar� �iz
			for(byte x=0; x<8; x++)
				for(byte y=0; y<8; y++)
					if(taslar[x,y]!=null)
						taslar[x,y].Ciz(this, grafik);
			//�izilen g�r�nt�y� PictureBox'da g�ster
			pbOyunAlani.Image = goruntu;
		}
		//Private �ye fonksyonlar ---------------------------------------------
		private void taslariTemizle()
		{
			for(byte x=0; x<8; x++)
				for(byte y=0; y<8; y++)
					taslar[x,y] = null;
		}
		//Public �zellikler ---------------------------------------------------
		public Tas[,] Taslar { get { return taslar; } }
		//Mouse Olaylar� ------------------------------------------------------
		private int aktifKareX = -1; //Mouse'un �zerinde oldu�u karenin koordinatlar�.
		private int aktifKareY = -1; 
		private int seciliKareX = -1; //Bir ta�� ta��mak i�in �nce se�ilmeli. Se�ilen
		private int seciliKareY = -1; //kare farkl� renkte �er�evelendirilir.
		//---------------------------------------------------------------------
		private void pbOyunAlani_MouseMove(object sender, MouseEventArgs e)
		{
			//Mouse'un �u anda �zerinde bulundu�u kare bulunuyor
			int mx = (e.X-CerceveGenisligi)/75;
			int my = (e.Y-CerceveGenisligi)/75;
			//E�er aktif kareden fakl�ysa bu kareyi aktif kare yap
			if(aktifKareX!=mx || aktifKareY!=my)
			{
				aktifCerceveTasi(aktifKareX, aktifKareY, mx, my);
				aktifKareX = mx;
				aktifKareY = my;
			}
		}
		//---------------------------------------------------------------------
		private void pbOyunAlani_MouseLeave(object sender, EventArgs e)
		{
			//Mouse oyun alan�n� terketmi�se t�m kareler pasif olsun
			aktifCerceveTasi(aktifKareX, aktifKareY, -1, -1);
			aktifKareX = -1;
			aktifKareY = -1;
		}
		//---------------------------------------------------------------------
		private void pbOyunAlani_MouseDown(object sender, MouseEventArgs e)
		{
			//Mouse'un �u anda �zerinde bulundu�u kare bulunuyor
			int mx = (e.X-CerceveGenisligi)/75;
			int my = (e.Y-CerceveGenisligi)/75;

			//T�klanan kare kurallara uygunsa devam et
			if((mx>=0) && (mx<=7) && (my>=0) && (my<=7))
			{
				//Ta��ma durumuna g�re hareket ediliyor
				switch(tasima)
				{
					case TasTasima.TasimaYok: // ta��ma yoksa
						//T�klanan karede s�ras� gelen oyuncunun ta�� varsa bu ta�� se�
						if(taslar[mx,my]!=null)
							if(taslar[mx,my].Renk==hamleSirasi)
								if(seciliKareX!=mx || seciliKareY!=my)
								{
									kareSecIptal(seciliKareX, seciliKareY);
									kareSec(mx, my);
									seciliKareX = mx;
									seciliKareY = my;
									//ta��ma durumunu de�i�tir
									tasima = TasTasima.TasSecili;
								}
						break;
					case TasTasima.TasSecili:
						if(taslar[seciliKareX,seciliKareY].Tasi((byte)mx,(byte)my))
						{
							//Ta� yenilmi� mi?
							if(taslar[mx,my].SonYenilenTas!=null)
							{
								//Yenilen ta�� tahtadan kald�r
								taslar[taslar[mx,my].SonYenilenTas.X,taslar[mx,my].SonYenilenTas.Y] = null;
								if(taslar[mx,my].TasYiyebilir())
								{
									//bu ta�� oynamas� zorunludur (bunu yap)
								}
								else //ta� yiyebilecek durumda de�ilse oyun s�ras�n� kar��ya ver
									hamleSirasi = (TasRengi)(1-(int)hamleSirasi);
							}
							else //Ta� yeme yoksa hamle s�ras�n� de�i�tir							
								hamleSirasi = (TasRengi)(1-(int)hamleSirasi);
							//Hi�bir kareyi se�me
							seciliKareX = -1;
							seciliKareY = -1;
							//Ta��ma durumunu s�f�rla
							tasima = TasTasima.TasimaYok;
							//Tahtay� yeniden �iz
							this.Ciz();
						}
						break;
				}
			}
		}
		//---------------------------------------------------------------------
		private void aktifCerceveTasi(int eskiX, int eskiY, int yeniX, int yeniY)
		{
			//Picturebox'dan resmi al
			Bitmap goruntu;
			try
			{
				goruntu = (Bitmap)pbOyunAlani.Image;
			}
			catch
			{
				return;
			}
            //Grafik nesnesi olu�tur
			Graphics grafik = Graphics.FromImage(goruntu);
			//�nceki kare pasif hale getiriliyor
			if((eskiX>=0) && (eskiX<=7) && (eskiY>=0) && (eskiY<=7))
			{
				//karenin rengi bulunuyor
				Bitmap Kare1 = ((eskiX+eskiY)%2==0)?(bmpBeyazKare):(bmpSiyahKare);
				//Bulunan renkte bir kare �iziliyor
				grafik.DrawImage(Kare1,
					eskiX*KareGenisligi+CerceveGenisligi,
					eskiY*KareGenisligi+CerceveGenisligi);
				//Varsa �st�ne ta� �iziliyor
				if(taslar[eskiX,eskiY]!=null)
					taslar[eskiX,eskiY].Ciz(this, grafik);
				//E�er bu kare se�ili kare ise tekrar se�
				if(seciliKareX==eskiX && seciliKareY==eskiY)
					kareSec(eskiX, eskiY);
			}
			//Yeni kare aktif hale getiriliyor
			if((yeniX>=0) && (yeniX<=7) && (yeniY>=0) && (yeniY<=7))
				grafik.DrawRectangle(new Pen(Color.Black,3.0f),
					yeniX*KareGenisligi+CerceveGenisligi+1,
					yeniY*KareGenisligi+CerceveGenisligi+1,
					KareGenisligi-3, KareGenisligi-3);
			//Oyun alan�n�n g�r�nt�s�n� g�ncelle
			this.pbOyunAlani.Refresh();
		}
		//---------------------------------------------------------------------
		private void kareSec(int x, int y)
		{
			if((x>=0) && (x<=7) && (y>=0) && (y<=7))
			{
				//Picturebox'dan resmi al
				Bitmap goruntu;
				try
				{
					goruntu = (Bitmap)pbOyunAlani.Image;
				}
				catch
				{
					return;
				}
				//Grafik nesnesi olu�tur
				Graphics grafik = Graphics.FromImage(goruntu);
				//Kare k�rm�z� olarak �er�evelendiriliyor
				if((x>=0) && (x<=7) && (y>=0) && (y<=7))
					grafik.DrawRectangle(new Pen(Color.Red,3.0f),
						x*KareGenisligi+CerceveGenisligi+1,
						y*KareGenisligi+CerceveGenisligi+1,
						KareGenisligi-3, KareGenisligi-3);
				//Oyun alan�n�n g�r�nt�s�n� g�ncelle
				this.pbOyunAlani.Refresh();
			}
		}
		//---------------------------------------------------------------------
		private void kareSecIptal(int x, int y)
		{
			if((x>=0) && (x<=7) && (y>=0) && (y<=7))
			{
				//Picturebox'dan resmi al
				Bitmap goruntu;
				try
				{
					goruntu = (Bitmap)pbOyunAlani.Image;
				}
				catch
				{
					return;
				}
				//Grafik nesnesi olu�tur
				Graphics grafik = Graphics.FromImage(goruntu);
				//karenin rengi bulunuyor
				Bitmap Kare1 = ((x+y)%2==0)?(bmpBeyazKare):(bmpSiyahKare);
				//Bulunan renkte bir kare �iziliyor
				grafik.DrawImage(Kare1,
					x*KareGenisligi+CerceveGenisligi,
					y*KareGenisligi+CerceveGenisligi);
				//Varsa �st�ne ta� �iziliyor
				if(taslar[x,y]!=null)
					taslar[x,y].Ciz(this, grafik);
				//Oyun alan�n�n g�r�nt�s�n� g�ncelle
				this.pbOyunAlani.Refresh();
			}
		}
	}
}
