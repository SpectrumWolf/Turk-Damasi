using System;
using System.Windows.Forms;
using System.Drawing;

namespace TurkDamasi
{
	public class Tahta
	{
		//Static deðiþkenler --------------------------------------------------
		public static Bitmap		bmpOyunTahtasi;
		public static Bitmap		bmpSiyahKare;
		public static Bitmap		bmpBeyazKare;
		//Deðiþkenler ---------------------------------------------------------
		private	Tas[,]				taslar = new Tas[8,8]; // tahtanýn durumu
		public int					KareGenisligi = 75; //tahta resminin bir karesinin geniþliði
		public int					CerceveGenisligi = 10; //tahtanýn çerçeve geniþliði
		public PictureBox			pbOyunAlani; //Oyun alanýnýn üstüne çizildiði picturebox
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
			//Oyun tahtasýnýn görüntüsünü al
			Tahta.bmpOyunTahtasi = new Bitmap(grafikDizini + "tahta.gif");
			//Karelerin görüntüsünü al
			Tahta.bmpBeyazKare = new Bitmap(grafikDizini + "beyazKare.gif");
			Tahta.bmpSiyahKare = new Bitmap(grafikDizini + "kirmiziKare.gif");
		}
		//Public Üye fonksyonlar -----------------------------------------------
		public void YeniOyunHazirla()
		{
			//taþlarýn tamamýný boþalt
			taslariTemizle();
			//yeni taþlarý oluþtur
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
			//Boþ bir Bitmap oluþtur
			Bitmap goruntu = new Bitmap(620,620);
			//Grafik nesnesi oluþtur
			Graphics grafik = Graphics.FromImage(goruntu);
			//Oyun tahtasýný çiz
			grafik.DrawImage(bmpOyunTahtasi, 0, 0);
			//Tüm taþlarý çiz
			for(byte x=0; x<8; x++)
				for(byte y=0; y<8; y++)
					if(taslar[x,y]!=null)
						taslar[x,y].Ciz(this, grafik);
			//Çizilen görüntüyü PictureBox'da göster
			pbOyunAlani.Image = goruntu;
		}
		//Private Üye fonksyonlar ---------------------------------------------
		private void taslariTemizle()
		{
			for(byte x=0; x<8; x++)
				for(byte y=0; y<8; y++)
					taslar[x,y] = null;
		}
		//Public Özellikler ---------------------------------------------------
		public Tas[,] Taslar { get { return taslar; } }
		//Mouse Olaylarý ------------------------------------------------------
		private int aktifKareX = -1; //Mouse'un üzerinde olduðu karenin koordinatlarý.
		private int aktifKareY = -1; 
		private int seciliKareX = -1; //Bir taþý taþýmak için önce seçilmeli. Seçilen
		private int seciliKareY = -1; //kare farklý renkte çerçevelendirilir.
		//---------------------------------------------------------------------
		private void pbOyunAlani_MouseMove(object sender, MouseEventArgs e)
		{
			//Mouse'un þu anda üzerinde bulunduðu kare bulunuyor
			int mx = (e.X-CerceveGenisligi)/75;
			int my = (e.Y-CerceveGenisligi)/75;
			//Eðer aktif kareden faklýysa bu kareyi aktif kare yap
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
			//Mouse oyun alanýný terketmiþse tüm kareler pasif olsun
			aktifCerceveTasi(aktifKareX, aktifKareY, -1, -1);
			aktifKareX = -1;
			aktifKareY = -1;
		}
		//---------------------------------------------------------------------
		private void pbOyunAlani_MouseDown(object sender, MouseEventArgs e)
		{
			//Mouse'un þu anda üzerinde bulunduðu kare bulunuyor
			int mx = (e.X-CerceveGenisligi)/75;
			int my = (e.Y-CerceveGenisligi)/75;

			//Týklanan kare kurallara uygunsa devam et
			if((mx>=0) && (mx<=7) && (my>=0) && (my<=7))
			{
				//Taþýma durumuna göre hareket ediliyor
				switch(tasima)
				{
					case TasTasima.TasimaYok: // taþýma yoksa
						//Týklanan karede sýrasý gelen oyuncunun taþý varsa bu taþý seç
						if(taslar[mx,my]!=null)
							if(taslar[mx,my].Renk==hamleSirasi)
								if(seciliKareX!=mx || seciliKareY!=my)
								{
									kareSecIptal(seciliKareX, seciliKareY);
									kareSec(mx, my);
									seciliKareX = mx;
									seciliKareY = my;
									//taþýma durumunu deðiþtir
									tasima = TasTasima.TasSecili;
								}
						break;
					case TasTasima.TasSecili:
						if(taslar[seciliKareX,seciliKareY].Tasi((byte)mx,(byte)my))
						{
							//Taþ yenilmiþ mi?
							if(taslar[mx,my].SonYenilenTas!=null)
							{
								//Yenilen taþý tahtadan kaldýr
								taslar[taslar[mx,my].SonYenilenTas.X,taslar[mx,my].SonYenilenTas.Y] = null;
								if(taslar[mx,my].TasYiyebilir())
								{
									//bu taþý oynamasý zorunludur (bunu yap)
								}
								else //taþ yiyebilecek durumda deðilse oyun sýrasýný karþýya ver
									hamleSirasi = (TasRengi)(1-(int)hamleSirasi);
							}
							else //Taþ yeme yoksa hamle sýrasýný deðiþtir							
								hamleSirasi = (TasRengi)(1-(int)hamleSirasi);
							//Hiçbir kareyi seçme
							seciliKareX = -1;
							seciliKareY = -1;
							//Taþýma durumunu sýfýrla
							tasima = TasTasima.TasimaYok;
							//Tahtayý yeniden çiz
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
            //Grafik nesnesi oluþtur
			Graphics grafik = Graphics.FromImage(goruntu);
			//Önceki kare pasif hale getiriliyor
			if((eskiX>=0) && (eskiX<=7) && (eskiY>=0) && (eskiY<=7))
			{
				//karenin rengi bulunuyor
				Bitmap Kare1 = ((eskiX+eskiY)%2==0)?(bmpBeyazKare):(bmpSiyahKare);
				//Bulunan renkte bir kare çiziliyor
				grafik.DrawImage(Kare1,
					eskiX*KareGenisligi+CerceveGenisligi,
					eskiY*KareGenisligi+CerceveGenisligi);
				//Varsa üstüne taþ çiziliyor
				if(taslar[eskiX,eskiY]!=null)
					taslar[eskiX,eskiY].Ciz(this, grafik);
				//Eðer bu kare seçili kare ise tekrar seç
				if(seciliKareX==eskiX && seciliKareY==eskiY)
					kareSec(eskiX, eskiY);
			}
			//Yeni kare aktif hale getiriliyor
			if((yeniX>=0) && (yeniX<=7) && (yeniY>=0) && (yeniY<=7))
				grafik.DrawRectangle(new Pen(Color.Black,3.0f),
					yeniX*KareGenisligi+CerceveGenisligi+1,
					yeniY*KareGenisligi+CerceveGenisligi+1,
					KareGenisligi-3, KareGenisligi-3);
			//Oyun alanýnýn görüntüsünü güncelle
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
				//Grafik nesnesi oluþtur
				Graphics grafik = Graphics.FromImage(goruntu);
				//Kare kýrmýzý olarak çerçevelendiriliyor
				if((x>=0) && (x<=7) && (y>=0) && (y<=7))
					grafik.DrawRectangle(new Pen(Color.Red,3.0f),
						x*KareGenisligi+CerceveGenisligi+1,
						y*KareGenisligi+CerceveGenisligi+1,
						KareGenisligi-3, KareGenisligi-3);
				//Oyun alanýnýn görüntüsünü güncelle
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
				//Grafik nesnesi oluþtur
				Graphics grafik = Graphics.FromImage(goruntu);
				//karenin rengi bulunuyor
				Bitmap Kare1 = ((x+y)%2==0)?(bmpBeyazKare):(bmpSiyahKare);
				//Bulunan renkte bir kare çiziliyor
				grafik.DrawImage(Kare1,
					x*KareGenisligi+CerceveGenisligi,
					y*KareGenisligi+CerceveGenisligi);
				//Varsa üstüne taþ çiziliyor
				if(taslar[x,y]!=null)
					taslar[x,y].Ciz(this, grafik);
				//Oyun alanýnýn görüntüsünü güncelle
				this.pbOyunAlani.Refresh();
			}
		}
	}
}
