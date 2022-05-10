using cancelaareceb.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cancelaareceb
{
    public partial class Form1 : Form
    {


        static string sessionId = "";
        static string routeId = "";

        static System.Net.Http.HttpClientHandler hnd = new System.Net.Http.HttpClientHandler();
        static System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(hnd);

        string uri;
        
        public Form1()
        {
            InitializeComponent();
            client.DefaultRequestHeaders.ExpectContinue = false;
            client.DefaultRequestHeaders.ConnectionClose = false;

            hnd.UseCookies = false;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            logar();

        }

        private async void logar()
        {
            uri = textBox4.Text;
         

            DadosLogin login = new DadosLogin()

            {
                CompanyDB = textBox3.Text,
                Password = textBox2.Text,
                UserName = textBox1.Text
             


            };


            try
            {

              
                System.Net.Http.StringContent cont = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

                System.Net.Http.HttpResponseMessage response = await client.PostAsync(uri, cont); 

                //response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                IEnumerable<string> sessionData = response.Headers.GetValues("Set-Cookie");

                foreach (string sessionDataItem in sessionData)
                {
                    if (sessionDataItem.Contains("B1SESSION"))
                        sessionId = sessionDataItem.Split('=')[1].Split(';')[0];
                    if (sessionDataItem.Contains("ROUTEID"))
                        routeId = sessionDataItem.Split(';')[0].Split(';')[0];
                }

                Console.WriteLine(responseBody);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            cancelareceber();
        }

        private async void cancelareceber()
        {
            List<int> listadocentry = new List<int>() { 6227, 6228, 6229 };

            foreach (var item in listadocentry)
            {

                try
                {


                    System.Net.Http.StringContent cont = new System.Net.Http.StringContent("", Encoding.UTF8, "application/json");

                    string sessionCookie = "B1SESSION=" + sessionId + "; " + routeId;

                    cont.Headers.Add("Cookie", sessionCookie);

                    System.Net.Http.HttpResponseMessage response = await client.PostAsync("https://SRVEVOSAP01:50000/b1s/v1/IncomingPayments(" + item + ")/Cancel", cont);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseBody);

                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", ex.Message);
                }
            }


        }
            
           
    }
}
