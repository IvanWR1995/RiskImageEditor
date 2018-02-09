using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Drawing.Drawing2D;



namespace RisksImageEditor
{

    public partial class Form1 : Form
    {
        BaseControl NewControl;
        Edge NewEdge;
        List<Edge> ListEdge;
        SlideToolBar slider;
        EventHandler EndEditEvent;
        bool IsErrorShow;

       // CanvasPanel Canvas;
        
        public EventHandler<double> Compute;
        public Form1()
        {
            NewControl = null;
            NewEdge = null;
            InitializeComponent();
            ListEdge = new List<Edge>();
            //this.Scroll+=ScrollEvent;
            VerticalScroll.Minimum = 0;
            
            Text = "Risk Image Editor";
            toolStripButton1.Image = Properties.Resources.ComputeRisk;
            PropabilityBtn.Image = Properties.Resources.CalcPropability;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
        {

        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
             
      /*      System.Drawing.Pen p = new Pen(Brushes.Black, 4);
            e.Graphics.DrawPath(p, edge.EdgePath);
            e.Graphics.FillPath(Brushes.Black, edge.EdgePath);*/
        }
        private void ErrorLeaf(Leaf sender, string ErrorMsg)
        {
            if (!IsErrorShow)
            {
                MessageBox.Show("Incorrect input!", "Warning"
                        , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                IsErrorShow = true;
            }
        }
        private void ScrollEvent(Object sender, ScrollEventArgs arg)
        {


            if (arg.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                this.slider.Location = new Point(slider.Location.X + (arg.NewValue - arg.OldValue), slider.Location.Y);
               
            }
            else if (arg.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                this.slider.Location = new Point(slider.Location.X, slider.Location.Y + (arg.NewValue - arg.OldValue) );
                
            }
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            slider = new SlideToolBar(new Size(200, this.Size.Height
                - menuStrip1.Size.Height - toolStrip1.Size.Height - statusStrip1.Size.Height-40), 
                new Point(0, menuStrip1.Size.Height + toolStrip1.Size.Height));
            
            slider.CreateBtnClick += SlideToolBar_CreateControl;
            slider.CreateEdgeBtnClick += SlideToolBar_CreateEdge;
            slider.OpenCloseClick += SlideToolBar_OpenClose;
            Controls.Add(slider);
            
            MainCanvas.Size= new Size(Size.Width - slider.Size.Width -17 , Size.Height - 38
                - menuStrip1.Size.Height - toolStrip1.Size.Height - statusStrip1.Size.Height);
            MainCanvas.Location = new Point(slider.Size.Width, menuStrip1.Size.Height + toolStrip1.Size.Height);
            Canvas = new CanvasPanel();
            Canvas.AutoScroll = false;
            Canvas.Size = new Size(Size.Width - slider.Size.Width - 40, Size.Height - 80
                - menuStrip1.Size.Height - toolStrip1.Size.Height-statusStrip1.Size.Height);
            Canvas.Location = new Point(0,0);
           // ((CanvasPanel)Canvas).picture.Size = new Size(Canvas.Size.Width,Canvas.Height);
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.MouseMove += Form1_MouseMove;
            MainCanvas.Controls.Add(Canvas);
            /*VerticalScroll.Maximum = Canvas.Size.Height;
            VerticalScroll.SmallChange = Canvas.Size.Height/20;
            VerticalScroll.LargeChange = Canvas.Size.Height / 20;*/
        }
        private void SlideToolBar_OpenClose(object sender, Size sz)
        {
            MainCanvas.Size = new Size(Size.Width - sz.Width  - 17, Size.Height - 38
                - menuStrip1.Size.Height - toolStrip1.Size.Height - statusStrip1.Size.Height);
            MainCanvas.Location = new Point(sz.Width, menuStrip1.Size.Height + toolStrip1.Size.Height);
       //     toolStripStatusLabel1.Text = "hi"; 
        }
       
        
        private void SlideToolBar_CreateControl(object sender, BaseControl Control)
        {
            NewControl = Control;

        }
        private void SlideToolBar_CreateEdge(object sender, Edge edge)
        {
            NewEdge = edge;
        }
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left )
            {
                if (NewControl != null)
                {
                   
                    NewControl.Location = e.Location;
                    Canvas.Controls.Add(NewControl);
                    foreach (Edge edge in ListEdge)
                    {
                        NewControl.Move += edge.ControlMove;
                        edge.Move += ((ILinked)NewControl).IsLinked;
                        
                    }
                    Compute += NewControl.ComputeRisk;
                    NewControl.GetInfoEvent += GetInfo;
                    NewControl.RemoveControl += RemoveControl; 
                    NewControl.LeaveEvent += LeaveElement;

                    if (NewControl is Leaf)
                    {
                        Leaf tmp = ((Leaf)NewControl);
                        EndEditEvent += tmp.EndOfEditMethod;
                        tmp.ErrorEvent += ErrorLeaf;
                    }
                    NewControl = null;
                }
                else if (NewEdge != null)
                {
                    NewEdge.Location = e.Location;
                    Canvas.MouseDown += NewEdge.MouseDown;
                    Canvas.MouseMove += NewEdge.MouseMove;
                    Canvas.MouseUp += NewEdge.MouseUp;
                    Canvas.Paint += NewEdge.Paint;
                    foreach (Control control in Canvas.Controls)
                    {
                        if (control is ILinked)
                        {
                            NewEdge.Move += ((ILinked)control).IsLinked;
                            ((BaseControl)control).Move += NewEdge.ControlMove;
                        }   

                    }
                    NewEdge.RemoveEdge += RemoveEdge;
                    ListEdge.Add(NewEdge);
                    NewEdge = null;
                    Canvas.Invalidate();
                }
            }
            
        }
        private void GetInfo(object sender, string info)
        {
            toolStripStatusLabel1.Text = info;
        }
        private void LeaveElement(object sender,EventArgs args )
        {
            toolStripStatusLabel1.Text = String.Empty;
        }
        private void ClearEdgesList()
        {
            for (int index = 0; index != ListEdge.Count; index++)
            {
                Canvas.MouseDown -= ListEdge[index].MouseDown;
                Canvas.MouseMove -= ListEdge[index].MouseMove;
                Canvas.MouseUp -= ListEdge[index].MouseUp;
                Canvas.Paint -= ListEdge[index].Paint;
                foreach (Control control in Canvas.Controls)
                {
                    if (control is ILinked)
                    {
                        ListEdge[index].Move -= ((ILinked)control).IsLinked;
                        ((BaseControl)control).Move -= ListEdge[index].ControlMove;
                    }

                }
            }
            ListEdge.Clear();
        }
        private void RemoveEdge(Edge sender)
        {
            Canvas.MouseDown -= sender.MouseDown;
            Canvas.MouseMove -= sender.MouseMove;
            Canvas.MouseUp   -= sender.MouseUp;
            Canvas.Paint     -= sender.Paint;
            foreach (Control control in Canvas.Controls)
            {
                if (control is ILinked)
                {
                    sender.Move -= ((ILinked)control).IsLinked;
                    ((BaseControl)control).Move -= sender.ControlMove;
                }

            }
             ListEdge.Remove(sender);
        }
        private void RemoveControl(BaseControl sender)
        {
            foreach (Edge edge in ListEdge)
            {

                sender.Move -= edge.ControlMove;
                edge.Move -= ((ILinked)sender).IsLinked;
            }
            Compute -= sender.ComputeRisk;
            sender.GetInfoEvent -= GetInfo;
            sender.RemoveControl -= RemoveControl;
            sender.LeaveEvent -= LeaveElement;
            if (sender is Leaf)
            {
                Leaf tmp = ((Leaf)sender);
                EndEditEvent -= tmp.EndOfEditMethod;
                tmp.ErrorEvent -= ErrorLeaf;

            }
            Controls.Remove(sender);
            sender.Dispose();
            
           
        }
        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
         
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            double Price = 0;
            string input = toolStripTextBox1.Text.Replace('.', ',');
            if (double.TryParse(input, out Price)&& Compute != null)
                Compute(toolStripTextBox2, Price);
            else if (Compute != null)
                MessageBox.Show("Incorrect input", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryFormatter binFormater = new BinaryFormatter();
                List<ISerializable> ListSerializeObj = Canvas.Controls.OfType<ISerializable>().ToList();
                ListSerializeObj = ListSerializeObj.Concat(ListEdge.ToList<ISerializable>()).ToList<ISerializable>();
                using( Stream fstream = new FileStream(SaveDialog.FileName,FileMode.Create,FileAccess.Write,FileShare.None ))
                    binFormater.Serialize(fstream, ListSerializeObj);
                    
 
            }
        }
        private void AddControl()
        {
            if (NewControl != null)
            {
                foreach (Edge edge in ListEdge)
                {
                    NewControl.Move += edge.ControlMove;
                    edge.Move += ((ILinked)NewControl).IsLinked;

                }
                Compute += NewControl.ComputeRisk;
                NewControl.GetInfoEvent += GetInfo;
                NewControl.RemoveControl += RemoveControl;
                NewControl.LeaveEvent += LeaveElement;
                if (NewControl is Leaf)
                {
                    Leaf tmp = ((Leaf)NewControl);
                    EndEditEvent += tmp.EndOfEditMethod;
                    tmp.ErrorEvent += ErrorLeaf;
                }
                    
            
                Canvas.Controls.Add(NewControl);
                NewControl.EmitMove();
                NewControl = null;
            }
        }
        private void AddEdge()
        {
            Canvas.MouseDown += NewEdge.MouseDown;
            Canvas.MouseMove += NewEdge.MouseMove;
            Canvas.MouseUp += NewEdge.MouseUp;
            Canvas.Paint += NewEdge.Paint;
            foreach (Control control in Canvas.Controls)
            {
                if (control is ILinked)
                {
                    NewEdge.Move += ((ILinked)control).IsLinked;
                    ((BaseControl)control).Move += NewEdge.ControlMove;
                }

            }
            NewEdge.RemoveEdge += RemoveEdge;
            NewEdge.EmitMove();
            ListEdge.Add(NewEdge);
            NewEdge = null;
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ISerializable> DeserializeObj;
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                BinaryFormatter binFormater = new BinaryFormatter();
                
                using (Stream fstream = File.OpenRead(OpenDialog.FileName))
                {

                    try
                    {
                        DeserializeObj = (List<ISerializable>)binFormater.Deserialize(fstream);
                    }
                    catch 
                    {
                        MessageBox.Show("Error when opening file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ClearEdgesList();
                    Canvas.Controls.Clear();
                    int Count = DeserializeObj.Count;
                    for (int index = 0; index != Count; index++)
                        if (DeserializeObj[index] is Edge)
                        {
                            NewEdge = (Edge)DeserializeObj[index];
                            AddEdge();
                           

                        }
                        else
                        {

                            NewControl = (BaseControl)DeserializeObj[index];
                            AddControl();

                        }
                    

                }
               Canvas.Invalidate();

            }

        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            
        }

        private void PropabilityBtn_Click(object sender, EventArgs e)
        {
            if (EndEditEvent != null)
            {
                IsErrorShow = false;
                EndEditEvent(this, e);
            }
        }

        

       
    }
}
