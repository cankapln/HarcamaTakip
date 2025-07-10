using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HarcamaTakip
{
    public partial class Form1 : Form
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;database=harcamatakip;user=root;pwd=''");
        MySqlCommand cmd = new MySqlCommand();
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            

            VerileriListele();

            MySqlDataReader dr;
            conn.Open();
            string qry = string.Format("select kategori_adi from kategoriler");
            cmd.Connection = conn;
            cmd.CommandText = qry;
            dr = cmd.ExecuteReader();
            while (dr.Read()) {
                cmbKategori.Items.Add(dr["kategori_adi"].ToString());
            }
            conn.Close();
            cmd.Dispose();

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cmbKategori.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtTutar.Text) || string.IsNullOrWhiteSpace(txtAciklama.Text))
            {
                MessageBox.Show("Lütfen boş alan bırakmayın.");
                return;
            }

            if (!decimal.TryParse(txtTutar.Text, out decimal amount))
            {
                MessageBox.Show("Geçerli bir tutar giriniz (örn: 123.45)");
                return;
            }

            string exp = txtAciklama.Text.Replace("'", "''");
            string category = cmbKategori.SelectedItem.ToString().Replace("'", "''");
            string tarih = dtpTarih.Value.ToString("yyyy-MM-dd");

            string qry = string.Format(
                "INSERT INTO harcamalar (tutar, aciklama, kategori, tarih) VALUES ({0}, '{1}', '{2}', '{3}')",
                amount, exp, category, tarih
            );

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(qry, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kayıt başarıyla eklendi.");

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void VerileriListele()
        {
            try
            {
                conn.Open();
                string sorgu = "SELECT id, tutar, aciklama, kategori, tarih FROM harcamalar ORDER BY tarih DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvHarcamalar.DataSource = dt;

                
                dgvHarcamalar.Columns["id"].HeaderText = "ID";
                dgvHarcamalar.Columns["tutar"].HeaderText = "Tutar";
                dgvHarcamalar.Columns["aciklama"].HeaderText = "Açıklama";
                dgvHarcamalar.Columns["kategori"].HeaderText = "Kategori";
                dgvHarcamalar.Columns["tarih"].HeaderText = "Tarih";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            VerileriListele();
        }
    }
}
