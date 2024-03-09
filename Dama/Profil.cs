using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TurkDamasi
{
    public partial class Profil : Form
    {
        public Profil()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            
            frmOyun f1 = new frmOyun();
            f1.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection b = new SqlConnection("Data Source=USER-BILGISAYAR\\SQLEXPRESS; Initial Catalog=OyunDB; Integrated Security=True;");
            b.Open();
            string kayit = "SELECT O_Kadi,O_Puan from OYUNCULAR order by O_Puan desc";
            //select O_KAdi, O_Puan from OYUNCULAR
            SqlCommand sec = new SqlCommand(kayit, b);
            SqlDataAdapter da = new SqlDataAdapter(sec);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            b.Close();

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult cikti = MessageBox.Show("Oturum kapatılsın mı ?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (cikti == DialogResult.Yes)
            {
                Giris g1 = new Giris();
                g1.Show();
                this.Hide();
            }
           
        }
    }
}
