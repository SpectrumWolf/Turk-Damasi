using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace TurkDamasi
{
	public class frmOyun : System.Windows.Forms.Form
	{
		//---------------------------------------------------------------------
		private Tahta oyunTahtasi;
		//---------------------------------------------------------------------
		private System.Windows.Forms.PictureBox pbGoruntu;
		private System.ComponentModel.Container components = null;
		public frmOyun()
		{
			InitializeComponent();
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pbGoruntu = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pbGoruntu
			// 
			this.pbGoruntu.Location = new System.Drawing.Point(8, 8);
			this.pbGoruntu.Name = "pbGoruntu";
			this.pbGoruntu.Size = new System.Drawing.Size(620, 620);
			this.pbGoruntu.TabIndex = 0;
			this.pbGoruntu.TabStop = false;
			// 
			// frmOyun
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(640, 638);
			this.Controls.Add(this.pbGoruntu);
			this.Name = "frmOyun";
			this.Text = "Türk Damasý Oyunu";
			this.Load += new System.EventHandler(this.frmOyun_Load);
			this.ResumeLayout(false);

		}
		#endregion
		//---------------------------------------------------------------------
		[STAThread]
		static void Main() 
		{
			if(onHazirliklariYap())
			{
				Application.Run(new frmOyun());
			}
			else
			{
				MessageBox.Show("Grafikleri bulunamadýðý için oyun baþlatýlamadý.","hata!");
			}
		}
		//---------------------------------------------------------------------
		private static bool onHazirliklariYap()
		{
			try
			{
				string grafikDizini = System.IO.Directory.GetCurrentDirectory() + "\\grafik\\";
				Tahta.GrafikleriAl(grafikDizini);
				Tas.GrafikleriAl(grafikDizini);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				return false;
			}
			return true;
		}
		//---------------------------------------------------------------------
		private void frmOyun_Load(object sender, System.EventArgs e)
		{
			oyunTahtasi = new Tahta(pbGoruntu);
			oyunTahtasi.YeniOyunHazirla();
			oyunTahtasi.Ciz();
		}
	}
}
