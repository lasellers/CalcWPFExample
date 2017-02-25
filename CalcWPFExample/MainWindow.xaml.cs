using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls; // xaml-based System.Windows.Controls.ListViewItem not System.Windows.Forms.ListViewItem 
//using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CalcWPFExample.Models;

namespace CalcWPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int sum = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void MainWindow_Loaded(object sender, EventArgs e)
        {
            loadMemory();
        }

        public void MainWindow_Unloaded(object sender, EventArgs e)
        {
            saveMemory();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "1";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "2";
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "3";
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "4";
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "5";
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "6";
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "7";
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "8";
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "9";
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            this.textBox1.Text += "0";
        }

        private void giveCleanFocusForInput()
        {
            this.textBox1.Text = "";
            this.textBox1.Focus();

            this.listView1.Items.MoveCurrentToLast();
            this.listView1.ScrollIntoView(this.listView1.Items.CurrentItem);
        }

        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CalcValidate.ValidateValueIsNumeric(this.textBox1.Text))
                {
                    int num = Int32.Parse(this.textBox1.Text.ToString());
                    this.sum -= num;
                    string summary = this.sum.ToString();
                    var row = new CalcResultsFormat()
                    { Operation = "-", Value = this.textBox1.Text, Summary = summary };

                    this.listView1.Items.Add(row);
                }
            }
            catch (FormatException ev)
            {
                Debug.WriteLine("FormatException " + ev.Message);
            }

            this.giveCleanFocusForInput();

            this.saveMemory();
        }

        private void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CalcValidate.ValidateValueIsNumeric(this.textBox1.Text))
                {
                    int num = Int32.Parse(this.textBox1.Text.ToString());
                    this.sum += num;
                    string summary = this.sum.ToString();

                    var row = new CalcResultsFormat()
                    { Operation = "+", Value = this.textBox1.Text, Summary = summary };
                    this.listView1.Items.Add(row);
                }
            }
            catch (FormatException ev)
            {
                Debug.WriteLine("FormatException " + ev.Message);
            }
            catch (OverflowException ev)
            {
                Debug.WriteLine("OverflowException " + ev.Message);
            }

            this.giveCleanFocusForInput();

            this.saveMemory();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            this.clearMemory();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// leave the header
        /// </summary>
        private void clearListView()
        {
            int count = this.listView1.Items.Count;
            while (count >= 1)
            {
                this.listView1.Items.RemoveAt(0);
                count--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void clearMemory()
        {
            this.sum = 0;
            this.textBox1.Text = "";

            this.clearListView();

            this.textBox1.Focus();
        }


        //
        private char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
        private char delimiter = ',';

        /// <summary>
        /// 
        /// </summary>
        public void loadMemory()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CalcStorageFormat));

                CalcStorageFormat memory;

                FileStream fs = new FileStream("calcmemory.xml", FileMode.Open, FileAccess.Read);
                XmlReader reader = XmlReader.Create(fs);
                try
                {
                    memory = (CalcStorageFormat)serializer.Deserialize(reader);
                }
                finally
                {
                    reader.Dispose();
                    fs.Close();
                }

                //
                this.sum = memory.sum;

                //
                this.textBox1.Text = "";

                //
                this.clearListView();
                for (int i = 0; i < memory.history.Count; i++)
                {
                    string line = memory.history[i];

                    string[] els = line.Split(this.delimiter);

                    try
                    {
                        var row = new CalcResultsFormat()
                        { Operation = els[0], Value = els[1], Summary = els[2] };
                        this.listView1.Items.Add(row);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        Debug.WriteLine("IndexOutOfRangeException " + ex.Message);
                    }
                }

            }
            catch (InvalidOperationException ex)
            {
                // catch XmlSerializer
                Debug.WriteLine("InvalidOperationException " + ex.Message);
            }

            this.giveCleanFocusForInput();
        }

        /// <summary>
        /// 
        /// </summary>
        public void saveMemory()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalcStorageFormat));
            CalcStorageFormat memory = new CalcStorageFormat();
            memory.sum = sum;

            memory.history = new List<string>();
            foreach (CalcResultsFormat item in listView1.Items)
            {
                string line = item.Operation + "," + item.Value + "," + item.Summary;
                memory.history.Add(line);
            }

            /*
            using (FileStream fs = new FileStream("memory", FileMode.Create, FileAccess.Write)
            {
                  serializer.Serialize(fs, memory);
            }
            */

            FileStream fs = new FileStream("calcmemory.xml", FileMode.Create, FileAccess.Write);
            try
            {
                serializer.Serialize(fs, memory);
            }
            finally
            {
                fs.Dispose();
            }

        }

    }
}
