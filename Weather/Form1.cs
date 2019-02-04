using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Weather
{
    public partial class frmWeather : Form
    {
        private XmlDocument xmlDoc;
        private Bitmap ikonice;
        public string imegrada;

        public frmWeather()
        {
            InitializeComponent();
        }

        private void frmWeather_Load(object sender, EventArgs e)
        {
            //ucitavanje xml dokumenta sa linka i ucitavanje slike ikonice.png iz filesystem-a
            xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load("http://fhmzbih.gov.ba/RSS/FHMZBIH1.xml");
                ikonice = (Bitmap)Bitmap.FromFile("ikonice.png");
            }
            catch
            {
                MessageBox.Show("Greska pri ucitavanju");
                this.Close();
            }

            //selektujemo sve "grad" cvorove i kreiramo listu stringova, koja sadrzi imena gradova
            XmlNodeList gradoviNodeList = xmlDoc.SelectNodes("//vremenska/grad");
            List<string> gradovi = new List<string>();
            //popunjavamo listu na osnovu atributa "naziv" svakog cvora
            foreach (XmlNode gradNode in gradoviNodeList)
                gradovi.Add(gradNode.Attributes["naziv"].Value);

            //postavljanje datasourca na nasu listu gradova
            

        }


        private void button1_Click(object sender, EventArgs e)
        {

            imegrada = (sender as Button).Text;
            lblGrad.Text = imegrada;

            
            ucitajDanas(imegrada);
            ucitajSutra(imegrada);
            ucitajPrekosutra(imegrada);

        }

  

        //pupunjava kolonu za danas na osnovu imena grada, ime se prosljedjuje kao argument
        private void ucitajDanas(string grad)
        {
            //oznacavamo cvor grada
            XmlNode gradNode = xmlDoc.SelectSingleNode("//vremenska/grad[@naziv='" + grad + "']");

            //svaki grad ima ili prognozadanas ili vrijemedanas za svaki grad, probamo jedno pa drugo
            XmlNode danasNode = gradNode.SelectSingleNode("prognozadanas");
            if(danasNode == null)
                danasNode = gradNode.SelectSingleNode("vrijemedanas");

            //ucitavanja min i max temperature, te opisa vremena iz xml-a
            string tempMin = danasNode.SelectSingleNode("mintemp").InnerText + "°C";
            string tempMax = danasNode.SelectSingleNode("maxtemp").InnerText + "°C";
            string vrijeme = danasNode.SelectSingleNode("prijepodne").InnerText;

            //popunjavamo labele na osnovu prethodno preuzetih vrijednosti, kako bi dobili odgovarajucu sliku pozivamo funkciju ikonica
            lblVrijemeDanas.Text = vrijeme;
            picDanas.Image = ikonica(vrijeme);
            lblMinTempDanas.Text = tempMin;
            lblMaxTempDanas.Text = tempMax;
        }

        //isto kao i prethodna funkcija, samo za kolonu sutra
        private void ucitajSutra(string grad)
        {
            XmlNode gradNode = xmlDoc.SelectSingleNode("//vremenska/grad[@naziv='" + grad + "']");

            XmlNode sutraNode = gradNode.SelectSingleNode("sutra");
            string tempMin = sutraNode.SelectSingleNode("mintemp").InnerText + "°C";
            string tempMax = sutraNode.SelectSingleNode("maxtemp").InnerText + "°C";
            string vrijeme = sutraNode.SelectSingleNode("poslijepodne").InnerText;

            lblVrijemeSutra.Text = vrijeme;
            picSutra.Image = ikonica(vrijeme);
            lblMinTempSutra.Text = tempMin;
            lblMaxTempSutra.Text = tempMax;
        }

        //kao prethnodne dvije
        private void ucitajPrekosutra(string grad)
        {
            XmlNode gradNode = xmlDoc.SelectSingleNode("//vremenska/grad[@naziv='" + grad + "']");

            XmlNode pSutraNode = gradNode.SelectSingleNode("prekosutra");
            string tempMin = pSutraNode.SelectSingleNode("mintemp").InnerText + "°C";
            string tempMax = pSutraNode.SelectSingleNode("maxtemp").InnerText + "°C";
            string vrijeme = pSutraNode.SelectSingleNode("poslijepodne").InnerText;

            lblVrijemePSutra.Text = vrijeme;
            picPSutra.Image = ikonica(vrijeme);
            lblMinTempPSutra.Text = tempMin;
            lblMaxTempPSutra.Text = tempMax;
        }

        //vraca bitmap sa slikom koja predstavlja odredjeno vrijeme, biramp sliku na osnovu indexa
        private Bitmap ikonica(int index)
        {
            Bitmap ikona = new Bitmap(192, 192);    //novi bmp koji ce ova funkcija vratiti
            Graphics g = Graphics.FromImage(ikona); //graphics objekt tog bitmapa, omogucava "crtanje" po bitmapu
            int x = (index % 5) * 192;  //na osnovu indexa odredjujemo u kojoj koloni se nalazi trazena slicica (index % 5), a potom mnozimo sa 192, jer je svaka slicica siroka tacno 192 pixela
            int y = (index / 5) * 174;  //na osnovu indexa odredjujemo u kojem redu se nalazi trazena slicica (index / 5), a potom mnozimo sa 174, jer je svaka slicica visoka tacno 174 pixela
            //precrtajemo sliku sa bitmapa ikonice, u bitmap ikona, prvi rectangle predstavlja područje na koje će se precrtati (u ovom slucaju preko citave slika, a drugi rectangle je područje sa kojeg ce se precrtati (x i y izracunati, sirina uvijek 192, visina uvijek 174), zadnji argument naznacuje da su dimenzije u pixelima 
            g.DrawImage(ikonice, new Rectangle(0, 0, 192, 192), new Rectangle(x, y, 192, 174), GraphicsUnit.Pixel);
            g.Dispose();    //oslobadjamo graphics objekt

            return ikona;
        }

        //na osnovu opisa vremena poziva i prethodnu funkciju sa potrebnim argumentima, ako opis nema odredjnu slicicu, vraca null
        private Bitmap ikonica(string vrijeme)
        {
            if (vrijeme == "oblačno s kišom")
                return ikonica(6);
            else if (vrijeme == "oblačno")
                return ikonica(3);
            else if (vrijeme == "pretežnovedro")
                return ikonica(1);
            else if (vrijeme == "vedro")
                return ikonica(0);
            else if (vrijeme == "pretežno vedro")
                return ikonica(1);
            else if (vrijeme == "umjereno oblačno")   
                return ikonica(2);
            else if (vrijeme == "umjerenooblačno")
                return ikonica(2);
            else if (vrijeme == "sunčano")
                return ikonica(0);
            else if (vrijeme == "pretežno oblačno")
                return ikonica(4);
            else if (vrijeme == "pretežno oblačno uz pljusak kiše")
                return ikonica(13);
            else if (vrijeme == "umjerenooblačno")
                return ikonica(3);
            else if (vrijeme == "oblačno sa snijegom")
                return ikonica(11);
                else
                return null;
        }

        private void picDanas_Click(object sender, EventArgs e)
        {

        }

    
    }
}
