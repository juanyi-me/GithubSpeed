using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using System.Security.Policy;

namespace Github
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string ApiUrl = "https://raw.hellogithub.com/hosts.json";
        public string Hostsurl = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\drivers\etc\hosts";

        class Github : Form1
        {
            async public Task<string> HttpClient()
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUrl);
                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                    string a = await response.Content.ReadAsStringAsync();
                    return a;
                }

            }
            public async Task<string> GetJSON()
            {
                string a = await HttpClient();
                return a;
            }
            public string GetData(string data)
            {
                JsonDocument b = JsonDocument.Parse(data);
                JsonElement root = b.RootElement;
                string output = "";
                foreach (JsonElement elements in root.EnumerateArray())
                {
                    var array = elements.EnumerateArray();
                    string ip = array.ElementAt(0).ToString();
                    string url = array.ElementAt(1).ToString();
                    output += ip + " " + url + "\n";
                }
                return output;
            }
            public void setText(string data, string startMark, string endMark)
            {
                string content = File.ReadAllText(Hostsurl);
                int startPos = content.IndexOf(startMark);
                int endPos = content.IndexOf(endMark);

                if (startPos > -1 && endPos > -1)
                {
                    content = content.Remove(startPos + startMark.Length, endPos - (startPos + startMark.Length));

                    startPos = content.IndexOf(startMark);

                    content = content.Insert(startPos + startMark.Length, $"\n{data}\n");
                    using (FileStream f = File.Open(Hostsurl, FileMode.Truncate))
                    {
                        f.Write(Encoding.UTF8.GetBytes(content), 0, Encoding.UTF8.GetBytes(content).Length);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(Hostsurl))
                    {
                        sw.WriteLine($"{startMark}\n{data}\n{endMark}");
                    }

                }

            }
        }
        public void showText()
        {
            string content = File.ReadAllText(Hostsurl);
            richTextBox1.Text = content;
        }
        public void saveText()
        {
            using (FileStream f = File.Open(Hostsurl, FileMode.Truncate))
            {
                f.Write(Encoding.UTF8.GetBytes(richTextBox1.Text), 0, Encoding.UTF8.GetBytes(richTextBox1.Text).Length);
            }
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void clearText(string startMark, string endMark)
        {
            string content = File.ReadAllText(Hostsurl);
            int startPos = content.IndexOf(startMark);
            int endPos = content.IndexOf(endMark);

            if (startPos > -1 && endPos > -1)
            {
                content = content.Remove(startPos + startMark.Length, endPos - (startPos + startMark.Length));

                using (FileStream f = File.Open(Hostsurl, FileMode.Truncate))
                {
                    f.Write(Encoding.UTF8.GetBytes(content), 0, Encoding.UTF8.GetBytes(content).Length);
                }
                MessageBox.Show("删除规则成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                showText();
            }
            else
            {
                MessageBox.Show($"删除规则失败，无法找到标识符{startMark}和{endMark}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        async private void button1_Click(object sender, EventArgs e)
        {
            Github github = new Github();
            try
            {
                string json = await github.GetJSON();
                string jsonString = github.GetData(json);
                github.setText(jsonString, "#juanyi Github start", "#juanyi Github end");
                MessageBox.Show("获取成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                showText();
            }
            catch (Exception err)
            {
                MessageBox.Show("获取失败" + err.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            showText();
            label2.Text = Hostsurl;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveText();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearText("#juanyi Github start", "#juanyi Github end");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 指定要打开的网页链接
            string url = "https://jyblog.cn";

            // 使用默认浏览器打开网页
            Process.Start(url);
        }
    }
}
