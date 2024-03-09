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
    public partial class Kayit : Form
    {
        public Kayit()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = ""; 
            textBox5.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox2.Text == "" && textBox3.Text == "" && textBox4.Text == "") 
            {
                MessageBox.Show("Lütfen Tüm Alanları Doldurunuz...");
            }
            else if (textBox4.Text == "")
            {
                MessageBox.Show("Kullanıcı Adı Boş Bırakılamaz !");
            }
            else if (textBox1.Text=="")
            {
                MessageBox.Show("Lütfen Adınızı Giriniz...");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Şifre Boş Bırakılamaz...");
            }
            else if (textBox3.Text == "")
            {
                MessageBox.Show("Lütfen E-Posta Adresi Giriniz...");
            }

            else if (textBox3.Text != textBox5.Text)
            {
                MessageBox.Show("Birinci ve İkinci şifreler aynı değil !","HATA !",MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

           
            else
            {

                SqlConnection a = new SqlConnection("Data Source=USER-BILGISAYAR\\SQLEXPRESS; Initial Catalog=OyunDB; Integrated Security=True;");
                a.Open();
                SqlCommand b = new SqlCommand("insert into OYUNCULAR(O_KAdi,O_Adi,O_Sifre,O_Email) values('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "')", a);
                b.ExecuteNonQuery();
                MessageBox.Show(" " + textBox1.Text + " Kayıt Başarılı Girişe Yönlendiriliyorsunuz...", "Kayıt Başarılı...",
             MessageBoxButtons.OK);
                Giris g = new Giris();
                g.Show();
                this.Hide();

            //    MessageBox.Show(" " + textBox1.Text + " Kullanıcı Adı Kullanımda Lütfen Başka Bir Kullanıcı Adı Seçiniz.", "..:: HATA ::..",
            //MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}
