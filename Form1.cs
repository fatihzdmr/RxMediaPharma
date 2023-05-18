using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Collections;

namespace RxMediaPharma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void doldur(string a1, string a3, string a4) 
        {
            SQLiteConnection sqlLitebaglan = new SQLiteConnection(@"DataSource=" + Application.StartupPath + "/rxsample.db;Version=3;");
            sqlLitebaglan.Open();
            string sorgu =
                "select i.ID," +
            "i.ILAC_ADI," +
            "if2.OLCU," +
            "ia.AMBALAJ, ia.BARKOD," +
            "i.FIRMA," +
            "ia.FIYAT, ia.FIYATTARIH, ia.KAMUFIYATI, ia.KAMUODENEN, ia.DEPOCU, ia.IMALATCI, ia.JENERIKORIJINAL," +
            "if2.SGKETKINKODU," +
            "i.ATCKODU, i.RECETE," +
            "ia.AMBALAJRESIM, " +
            "if2.KUB " +
            //"iem.MIKTAR, iem.BIRIM," +
            //"em.ETKINMADDE " +
            "from ILACLAR i inner join " +
            "ILAC_FORM if2 on i.ID = if2.ILAC_ID inner join " +
            "ILAC_AMBALAJ ia on if2.ID = ia.ILAC_FORM_ID " +
            "where i.ID = " + a1 + "  and if2.OLCU like '%" + a3 + "%' and ia.AMBALAJ like '%" + a4 + "%'";
            SQLiteCommand sql = new SQLiteCommand(sorgu, sqlLitebaglan);



            sql.Connection = sqlLitebaglan;
            SQLiteDataReader sdr = sql.ExecuteReader();
            while (sdr.Read())
            {
                lblIlacAd.Text = sdr["ILAC_ADI"].ToString();
                lblIlacOlcu.Text = sdr["OLCU"].ToString();
                lblAmbalaj.Text = sdr["AMBALAJ"].ToString();
                lblBarkod.Text = sdr["BARKOD"].ToString();
                lblFirma.Text = sdr["FIRMA"].ToString();
                lblFiyat.Text = sdr["FIYAT"].ToString();
                lblFiyatTarih.Text = "(" + sdr["FIYATTARIH"].ToString() + ")";
                lblKamuFiyat.Text = sdr["KAMUFIYATI"].ToString();
                lblKamuOdenen.Text = sdr["KAMUODENEN"].ToString();
                if (lblKamuFiyat.Text != "" && lblKamuOdenen.Text != "")
                {
                    lblFiyatFarki.Text = "(" + (Convert.ToDouble(lblKamuFiyat.Text) - Convert.ToDouble(lblKamuOdenen.Text)).ToString() + ") FİYAT FARKI ";
                }
                else
                {
                    lblFiyatFarki.Text = "";
                }
                lblDepocuFiyat.Text = sdr["DEPOCU"].ToString();
                lblImalatciFiyat.Text = sdr["IMALATCI"].ToString();
                lblKdv.Text = "%8";//sdr[""].ToString();
                lblOrigin.Text = sdr["JENERIKORIJINAL"].ToString();
                lblSgkKodu.Text = sdr["SGKETKINKODU"].ToString();
                lblAtcKodu.Text = sdr["ATCKODU"].ToString();
                lblRecete.Text = sdr["RECETE"].ToString();
                kup = sdr["KUB"].ToString();

                byte[] imageDatas = null;

                try
                {

                    imageDatas = (byte[])sdr["AMBALAJRESIM"];
                }
                catch (Exception)
                {
                    imageDatas = null;
                }




                if (imageDatas != null)
                {
                    MemoryStream stream = new MemoryStream(imageDatas);
                    Image image = Image.FromStream(stream);
                    pictureBox1.Image = image;
                }

                Zen.Barcode.Code128BarcodeDraw brc = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                pictureBox2.Image = brc.Draw(sdr["BARKOD"].ToString(), 200);
                lblBarkodGoster.Text = sdr["BARKOD"].ToString();

                if (!sonGezilenler.Contains(sdr["BARKOD"].ToString()))
                {
                    sonGezilenler.Add(sdr["BARKOD"].ToString());
                }
                
            }


            listView2.Items.Clear();

            SQLiteDataAdapter adapter2 = new SQLiteDataAdapter(
                "select i.ID," +
                "if2.ILAC_ID," +
                "iem.MIKTAR , iem.BIRIM," +
                "em.ETKINMADDE " +
                "from ILACLAR i inner join " +
                "ILAC_FORM if2 on i.ID = if2.ILAC_ID inner join " +
                "ILAC_AMBALAJ ia on if2.ID = ia.ILAC_FORM_ID inner join " +
                "ILAC_ETKIN_MADDELER iem on if2.ID = iem.ILAC_FORM_ID inner JOIN " +
                "ETKIN_MADDELER em on iem.ETKIN_MADDE = em.ID " +
                "where i.ID = " + a1 + "  and if2.OLCU like '%" + a3 + "%' and ia.AMBALAJ like '%" + a4 + "%'", sqlLitebaglan);

            DataSet dataSet = new DataSet();
            adapter2.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {

                string madde = row["ETKINMADDE"].ToString();
                string miktar = row["MIKTAR"].ToString();
                string birim = row["BIRIM"].ToString();

                string[] ekle = { madde + " - " + miktar + " " + birim };

                var satir = new ListViewItem(ekle);
                listView2.Items.Add(satir);
                //listView1.Items[0].Selected = true;
            }

            sqlLitebaglan.Close();

        }
        public void ara(string aranan)
        {
            try
            {
                SQLiteConnection sqlLitebaglan = new SQLiteConnection(@"DataSource=" + Application.StartupPath + "/rxsample.db;Version=3;");
                sqlLitebaglan.Open();
                string sorgu = "SELECT i.ID, " +
                    "i.ILAC_ADI," +
                    "if2.OLCU," +
                    "ia.AMBALAJ  " +
                    "FROM ILACLAR i INNER JOIN " +
                    "ILAC_FORM if2 ON i.ID = if2.ILAC_ID  INNER JOIN " +
                    "ILAC_AMBALAJ ia ON if2.ID = ia.ILAC_FORM_ID " +
                    "where i.ILAC_ADI like '%" + aranan + "%' or ia.BARKOD like '%" + aranan + "%' ORDER BY i.ID ,if2.OLCU ";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sorgu, sqlLitebaglan);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Label lbl = new Label();
                    lbl.Text = row.ToString();
                    string ids = row["ID"].ToString().ToUpper();
                    string isim = row["ILAC_ADI"].ToString().ToUpper();
                    string olcu = row["OLCU"].ToString().ToUpper();
                    string[] ambalaj = row["AMBALAJ"].ToString().ToUpper().Split('/');
                    string[] ekle = { ids + "-" + isim + "-" + olcu + "-" + ambalaj[0] };

                    var satir = new ListViewItem(ekle);
                    listView1.Items.Add(satir);
                    listView1.Items[0].Selected = true;


                }

                string[] numa2 = listView1.SelectedItems[0].SubItems[0].Text.Split('-');
                string a1 = numa2[0];
                string a2 = numa2[1].ToLower();
                string a3 = numa2[2].ToLower();
                string a4 = numa2[3].ToLower();

                doldur(a1, a3, a4);

                sqlLitebaglan.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("Sonuç Bulunamadı");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Baglanti bl = new Baglanti();
            //bl.sqlLitebaglan();

            SQLiteConnection sqlLitebaglan = new SQLiteConnection(@"DataSource=" + Application.StartupPath + "/rxsample.db;Version=3;");
            sqlLitebaglan.Open();

            SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT i.ID, i.ILAC_ADI,if2.OLCU,ia.AMBALAJ  FROM ILACLAR i INNER JOIN ILAC_FORM if2 ON i.ID = if2.ILAC_ID  INNER JOIN ILAC_AMBALAJ ia ON if2.ID = ia.ILAC_FORM_ID  ORDER BY i.ID ,if2.OLCU ", sqlLitebaglan);

            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Label lbl = new Label();
                lbl.Text = row.ToString();
                string ids = row["ID"].ToString().ToUpper();
                string isim = row["ILAC_ADI"].ToString().ToUpper();
                string olcu = row["OLCU"].ToString().ToUpper();
                string[] ambalaj = row["AMBALAJ"].ToString().ToUpper().Split('/');
                string[] ekle = { ids + "-" + isim + "-" + olcu + "-" + ambalaj[0] };

                var satir = new ListViewItem(ekle);
                listView1.Items.Add(satir);
                listView1.Items[0].Selected = true;

                
            }

            string[] numa2 = listView1.SelectedItems[0].SubItems[0].Text.Split('-');
            string a1 = numa2[0];
            string a2 = numa2[1].ToLower();
            string a3 = numa2[2].ToLower();
            string a4 = numa2[3].ToLower();

            doldur(a1, a3, a4);

            sqlLitebaglan.Close();

        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            string aranan = txtAra.Text;
            listView1.Items.Clear();

            ara(aranan);

            string brkd = lblBarkod.Text;
            if (favoriler.Contains(brkd))
            {
                btnFavori.Text = "Favorilerden Çıkar";
                btnFavori.BackColor = Color.Yellow;
            }
            else
            {

                btnFavori.BackColor = SystemColors.Control;
                btnFavori.Text = "Favorilere Ekle";
            }

        }

        private List<string> sonGezilenler = new List<string>();
        private List<string> favoriler = new List<string>();
        private int sira=-1;
        string kup = "";
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBox1.Image = null;

            sira++;
            string[] numa2 = listView1.SelectedItems[0].SubItems[0].Text.Split('-');

            
            string a1 = numa2[0];
            string a2 = numa2[1].ToLower();
            string a3 = numa2[2].ToLower();
            string a4 = numa2[3].ToLower();

            doldur(a1, a3, a4);

            string brkd = lblBarkod.Text;


            if (favoriler.Contains(brkd))
            {
                btnFavori.Text = "Favorilerden Çıkar";
                btnFavori.BackColor = Color.Yellow;
            }
            else
            {
               
                btnFavori.BackColor = SystemColors.Control;
                btnFavori.Text = "Favorilere Ekle";
            }


        }

        private void btnBarkodKopyala_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblBarkod.Text);
            MessageBox.Show("Barkod panoya Kopyalandı " + lblBarkod.Text);
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            if (sira > 0)
            {
                listView1.Items.Clear();
               sira--;
                ara(sonGezilenler[sira].ToString());
                 
            }
            else 
            {
                if (sonGezilenler.Count > 1)
                {
                    sira = sonGezilenler.Count - 1;
                    listView1.Items.Clear();
                    ara(sonGezilenler[sira].ToString());
                }
                
            }
        }

        private void btnIleri_Click(object sender, EventArgs e)
        {
            if (sira < sonGezilenler.Count - 1)
            {
                listView1.Items.Clear();
               sira++;
                ara(sonGezilenler[sira].ToString());
                
            }
            else
            {
                sira = 0;
                listView1.Items.Clear();
                ara(sonGezilenler[sira].ToString());
            }
        }

        private void btnFavori_Click(object sender, EventArgs e)
        {
            string brkd = lblBarkod.Text;
            

            if (!favoriler.Contains(brkd))
            {
                favoriler.Add(brkd);
                btnFavori.BackColor = Color.Yellow;
                btnFavori.Text = "Favorilerden Çıkart";
            }
            else
            {
                favoriler.Remove(brkd);
                btnFavori.BackColor = SystemColors.Control;
                btnFavori.Text = "Favorilere ekle";
            }

        }

        private void btnKupur_Click(object sender, EventArgs e)
        {
            Form2 fr = new Form2();
            fr.mesaj = kup;
            fr.ShowDialog();
        }

        private void txtAra_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtAra.CharacterCasing = CharacterCasing.Upper;
            if (e.KeyChar == 'i')
            {
                e.KeyChar = 'İ';
            }
            else if (e.KeyChar == 'ı')
            {
                e.KeyChar = 'I';
            }
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            string aranan = txtAra.Text;
            listView1.Items.Clear();

            ara(aranan);

            string brkd = lblBarkod.Text;
            if (favoriler.Contains(brkd))
            {
                btnFavori.Text = "Favorilerden Çıkar";
                btnFavori.BackColor = Color.Yellow;
            }
            else
            {

                btnFavori.BackColor = SystemColors.Control;
                btnFavori.Text = "Favorilere Ekle";
            }
        }
    }
}