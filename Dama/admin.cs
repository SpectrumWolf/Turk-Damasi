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
    public partial class admin : Form
    {
        public admin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult cikti = MessageBox.Show("Çıkmak İstediğinizden Emin misiniz ?", "Çıkış", MessageBoxButtons.YesNo,MessageBoxIcon.Question); 
            if (cikti == DialogResult.Yes) 
            {
                Giris g1 = new Giris();
                g1.Show();
                this.Hide(); 
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection b = new SqlConnection("Data Source=USER-BILGISAYAR\\SQLEXPRESS; Initial Catalog=OyunDB; Integrated Security=True;");
            b.Open();
            string kayit = "SELECT O_KAdi,O_Adi,O_Sifre,O_Email,O_Puan from OYUNCULAR order by O_KAdi asc ";//order by O_Puan desc
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
           
            
            DialogResult cikti = MessageBox.Show("Oyuncuyu Silmek İstediğinizden Emin misiniz ?", "Sil ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cikti == DialogResult.Yes)
            {
                SqlConnection b = new SqlConnection("Data Source=USER-BILGISAYAR\\SQLEXPRESS; Initial Catalog=OyunDB; Integrated Security=True;");
                b.Open();
                SqlCommand c = new SqlCommand("Delete From OYUNCULAR where O_KAdi='" + textBox1.Text + "'", b);
                c.ExecuteNonQuery();
                //SqlCommand sec = new SqlCommand(sil, b);
               // SqlDataAdapter da = new SqlDataAdapter(sec);
                b.Close();
            }
        }
    }
}
