using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
 
                

 
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

       

        public Form1()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------
        map mymap = new map();
        //-----------------------------------------------------
        [Serializable]
        class Couple_Sentance {//chera struct nemishe????/

               public string Farsi;
               public string English;
            
            }
        
        //-----------------------------------------------------
        [Serializable]
        class map {

           
           public List<Couple_Sentance> CouplesList = new List<Couple_Sentance>();
           public int CurrentCoupleIndex = 0;//zero based
           public List<int> problem_indexes = new List<int>();  
           public void break2sentences(string textcontent)
           {//and add couples to list



               //hazfe akharin . age dasht
               if (textcontent != "")
               {
                   if (textcontent[textcontent.Length - 1] == '.')//if akharish . bood//chon split() baad "." ra ham ozv mide! 
                   {
                       int lastindex = textcontent.Length - 1;//last . index
                       textcontent = textcontent.Remove(lastindex);
                   }
               }




               if (textcontent == "")
               { //chon split() empty ra ham ozv mide!           
                   MessageBox.Show("No input");
                   return;
               }

               //vali "....." filter nemishe  vali "." filter mishe
               //-------------------------------------------------------




            string[] allsen = textcontent.Split(".".ToCharArray());
            foreach (string s in allsen)
            {   
               Couple_Sentance temp=new Couple_Sentance();
               temp.English=s;
               temp.Farsi="";

               CouplesList.Add(temp);
            }
           
           }

            
            public bool MarkCurrentAsProblem() {
                //sets current couple as problem
                //it saves real cuple index into problem list

                //chek if was not problem before, prefvent duplicates
                for (int i = 0; i < problem_indexes.Count; i++)
                {
                    if (problem_indexes[i] == CurrentCoupleIndex)
                    {
                        return false;
                    }
                }

                

                problem_indexes.Add(CurrentCoupleIndex);
                return true;
            
            }

            public void RemoveCurrentProblem()
            {   //make it normal if current sentence index is in problem list

                for (int i = 0; i < problem_indexes.Count; i++)
                {
                    if (problem_indexes[i] == CurrentCoupleIndex)
                    {
                        problem_indexes.RemoveAt(i);
                    }
                }
            }
            
           
            public Couple_Sentance Jump_to_Couple(int destination_index)
            {

                if (destination_index < CouplesList.Count() && destination_index > -1)
                {
                    CurrentCoupleIndex=destination_index;
                    return CouplesList[destination_index];
                }
                else
                {
                    MessageBox.Show("No such index");
                    return null;
                }



            
            }

            public Couple_Sentance NextCouple()
            {

                if (CurrentCoupleIndex < CouplesList.Count() - 1)
                {
                    this.CurrentCoupleIndex++;
                    return CouplesList[CurrentCoupleIndex];
                }
                else
                {
                    MessageBox.Show("No next sentence");
                    return null;
                }
            }

            public Couple_Sentance BeforeCouple()
            {


                if (CurrentCoupleIndex > 0)
                {
                    this.CurrentCoupleIndex--;
                    return CouplesList[CurrentCoupleIndex];
                }
                else
                {
                    MessageBox.Show("No last sentence");
                    return null;
                          
                }
            }

            public Couple_Sentance Current_Sentance()
           {
                return CouplesList[CurrentCoupleIndex];                     
           
           
           }
                       
            
            public void Save_Couple(string farsi) { //saves changes to farsi section of current section 

                CouplesList[CurrentCoupleIndex].Farsi = farsi;
            
            }
        }

        //-----------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {

            if (File.Exists("data.bin"))
            {

                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = File.OpenRead("data.bin"))
                {
                    mymap = (map)binFormat.Deserialize(fStream);

                    for(int i=0;i<mymap.problem_indexes.Count;i++){
                      this.listBox1.Items.Add(mymap.CouplesList[mymap.problem_indexes[i]].English);
                    }

                    richTextBox1.Text = mymap.Current_Sentance().English;
                    richTextBox2.Text = mymap.Current_Sentance().Farsi;
                }
            }
           
        }
               
        private void button1_Click(object sender, EventArgs e)
        {
            mymap.NextCouple();
            richTextBox1.Text = mymap.Current_Sentance().English;
            richTextBox2.Text = mymap.Current_Sentance().Farsi;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mymap.BeforeCouple();
            richTextBox1.Text = mymap.Current_Sentance().English;
            richTextBox2.Text = mymap.Current_Sentance().Farsi;
        }

        private void button3_Click(object sender, EventArgs e)  {
            mymap = null;//lazeme????
            mymap = new map();
            
            mymap.break2sentences(richTextBox1.Text);
            richTextBox1.Text = mymap.Current_Sentance().English;

            listBox1.Items.Clear();
            richTextBox2.Text = "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
          mymap.Save_Couple(richTextBox2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            string final_result="";
            foreach(Couple_Sentance sen in mymap.CouplesList)
              final_result+=sen.Farsi;

            Clipboard.SetText(final_result);

        }

       
        private void button8_Click(object sender, EventArgs e)
        {
            if(mymap.MarkCurrentAsProblem())
            listBox1.Items.Add(mymap.Current_Sentance().English);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            mymap.RemoveCurrentProblem();

            //update listbox by reading again from problem list
            listBox1.Items.Clear();
            foreach (int i in mymap.problem_indexes) {             
                 listBox1.Items.Add( mymap.CouplesList[i].English);
            }

        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if ( listBox1.SelectedIndex < 0)
                return;


            //* alistboxx seleted index is of listbox items  not real sentence index in problem_indexes, so it is converted to real senence index

            //make real index
            int real_index = mymap.problem_indexes[listBox1.SelectedIndex];

            //jump to it current, makes it current sentence
            mymap.Jump_to_Couple(real_index);

            //
            richTextBox1.Text = mymap.Current_Sentance().English;
            richTextBox2.Text = mymap.Current_Sentance().Farsi;

             





        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            // Store object in a local file.
            using (Stream fStream = new FileStream("data.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, mymap);
            }
        }

       
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //this short cut is equal to save button+next button

            if (e.KeyData == Keys.Left)
            {
                
                mymap.Save_Couple(richTextBox2.Text);
                mymap.NextCouple();
                richTextBox1.Text = mymap.Current_Sentance().English;
                richTextBox2.Text = mymap.Current_Sentance().Farsi;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
