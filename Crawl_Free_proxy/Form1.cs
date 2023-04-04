using RestSharp;
using Newtonsoft.Json;
namespace Crawl_Free_proxy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ReadFileAsync(int.Parse(txtpage.Text), int.Parse(txtmax.Text));
        }
        private async void ReadFileAsync(int page,int max)
        {
            var task = Task.Run(() => {
                using (var client= new RestClient())
                {
                    UpdateStatus("Đang tải peoxy...");
                    var rq = new RestRequest($"https://proxylist.geonode.com/api/proxy-list?limit={max}&page={page}&sort_by=lastChecked&sort_type=desc", Method.Get);
                    var respone= client.ExecuteGet(rq);
                    Root? data =JsonConvert.DeserializeObject<Root>(respone.Content??"");
                    UpdateStatus("Đang phân loại proxy...");
                    foreach (var item in data?.data??new List<Datum>())
                    {
                        Phanloai(item);
                    }
                }
            });

            // thực hiện các tác vụ khác ở đây

            // chờ cho tác vụ đọc file hoàn thành
            await task;

            UpdateStatus("Done");
            this.Invoke((MethodInvoker)delegate {
                lbhttp.Text += $"({txthttp.Lines.Count()})";
                lbs4.Text += $"({txtsocks4.Lines.Count()})";
                lbs5.Text += $"({txts5.Lines.Count()})";
            });

        }
        void Phanloai(Datum proxy)
        {
            if (proxy.protocols != null)
            {
                if (proxy.protocols[0].Contains("socks5"))
                {
                    this.Invoke((MethodInvoker)delegate {
                        txts5.AppendText(proxy.ip + ':' + proxy.port + Environment.NewLine);
                    });
                }
               else if (proxy.protocols[0].Contains("socks4"))
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtsocks4.AppendText(proxy.ip + ':' + proxy.port + Environment.NewLine);
                    });
                }
                else if (proxy.protocols[0].Contains("http"))
                {
                    this.Invoke((MethodInvoker)delegate {
                        txthttp.AppendText(proxy.ip + ':' + proxy.port + Environment.NewLine);
                    });
                }
            }
        }
        private void UpdateStatus(string status)
        {
            // cập nhật trạng thái ở đây
            // sử dụng phương thức Invoke để gọi UpdateStatusLabel trên luồng giao diện
            this.Invoke((MethodInvoker)delegate {
                label1.Text = status;
            });
        }

       

    }
}