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
    public partial class Giris : Form
    {
        public Giris()
        {
            InitializeComponent();
        }
           // SqlConnection Bağlantıyı sağlamak için kullanıyoruz.
        //SqlConnection baglan = new SqlConnection();
        private void button1_Click(object sender, EventArgs e)
        {


           // label3.Visible = true;
            SqlConnection bagla = new SqlConnection("Data Source=USER-BILGISAYAR\\SQLEXPRESS; Initial Catalog=OyunDB; Integrated Security=True;");
            bagla.Open();
            SqlCommand get = new SqlCommand("SELECT * FROM OYUNCULAR WHERE O_KAdi='" + textBox1.Text + "' and O_Sifre='" + textBox2.Text + "'", bagla);
            SqlDataReader r = get.ExecuteReader();
            while (r.Read())
            {
                if (textBox1.Text == r["O_KAdi"].ToString() && textBox2.Text == r["O_Sifre"].ToString())
                {
                    if (textBox1.Text == "admin")
                    {
                        // label3.Visible = false;
                        MessageBox.Show("Admin Giriş Başarılı...");
                        admin p1 = new admin();
                        this.Hide();
                        p1.Show();
                        break;
                        
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Girişi Başarılı...");
                        Profil p2 = new Profil();
                        this.Hide();
                        p2.Show();
                        break;
                    }

              
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya Şifre hatalı...", "..:: HATA ::..",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               
                
            }
            
           // label3.Text = "Kullanıcı Adı veya Şifre Hatalı...";
            r.Close();
            bagla.Close();
            //// Boş değer girilmesini engelliyoruz.
            //if (textBox1.Text=="" || textBox2.Text=="")
            //{
            //    MessageBox.Show("Lütfen Tüm Alanları Doldurunuz !", "..:: HATA ::..",
            //    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            //}
            //try
            //{
            //    // Sql bağlantı kuruluyor.
            //    baglan.ConnectionString = "Server=USER-BILGISAYAR\\SQLEXPRESS;Database=OyunDB;Trusted_Connection=True;";
            //    baglan.Open(); // Bağlantı açıldı
            //    // Sorgumuz. 
            //    string sql = "SELECT * FROM OYUNCULAR WHERE O_KAdi=@O_KAdi AND O_Sifre=@O_Sifre";
            //    SqlParameter prms1 = new SqlParameter("@O_KAdi", textBox1.Text); // kullanıcı adı 
            //    SqlParameter prms2 = new SqlParameter("@O_Sifre", textBox2.Text);// sifre parametre
            //    SqlCommand cmd = new SqlCommand(sql, baglan); // baglantı
            //    cmd.Parameters.Add(prms1); // 
            //    cmd.Parameters.Add(prms2);
            //    DataTable dt = new DataTable();
            //    SqlDataAdapter da = new SqlDataAdapter(cmd);
            //    da.Fill(dt);
                
            //    if (dt.Rows.Count > 0)
            //    {
            //        if (textBox1.Text == "admin")
            //        {
            //            MessageBox.Show("Hoşgeldin " + textBox1.Text);
            //            admin a2 = new admin();
            //            a2.Show();
            //            this.Hide();
            //        }
            //        else
            //        {
            //            MessageBox.Show("Hoşgeldin " + textBox1.Text);
            //            Profil p2 = new Profil();
            //            p2.Show();
            //            this.Hide();
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Veritabanında böyle bir kullanıcı bulunamadı");
            //        //break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            Kayit k1 = new Kayit();
            k1.Show();
            this.Hide();
        }

        
    }
}
