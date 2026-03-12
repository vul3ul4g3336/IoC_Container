using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NLog_DI
{
    public partial class Form1 : Form
    {
        public ILogger<Form1> Logger { get; set; }

        public Runner Runner { get; set; }

        //public Form1()
        //{
        //    this.InitializeComponent();
        //}

        public Form1(ILogger<Form1> logger)
        {
            this.InitializeComponent();
            this.Logger = logger;
            //this.Runner = runner;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var name = button.Name;

            this.Logger.LogInformation("{name} 按鈕被按了", name);
            this.Logger.LogInformation("執行更新");

           // this.Runner.DoAction(name);

            this.Logger.LogInformation("完成");
        }
    }
}
