using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Drawing;

namespace RisksImageEditor
{
    [Serializable]
    class AndControl : ControlOperator, ICalculate,IVariable
    {
        Label PropabilityOutput;
        public event Action EndOfEdit;
        public event Action<IVariable> DeleteVariable;
        public event Action<ICalculate> DeleteControl;
        public double Value { get { return propability; } }
        
        public AndControl(Point point):this()
        {
            Location = new Point(point.X - ElementPanel.Size.Width / 2, point.Y - ElementPanel.Size.Height / 2);
            
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            
        }
        public AndControl(SerializationInfo info, StreamingContext context):this()
        {
            base.Deserealize(info, context);
            PropabilityOutput.Text = propability.ToString("0.###E-00"); 
            if (Moved != null)
                Moved(this, new Point(Location.X + 69, Location.Y + 64));
            
        }
        public AndControl()
        {
            ComentsInput.Size = new Size(100, 50);
            ComentsInput.Location = new Point(15, 35);
           
            PropabilityOutput = new Label();
            PropabilityOutput.Size = new Size(33, 20);
            PropabilityOutput.Location = new Point(75, 90);
            PropabilityOutput.AutoSize = true;
            PropabilityOutput.Parent = this;
           
            PropabilityLabel.Location = new Point(15, 90);
            PropabilityLabel.Size = new Size(58, 13);
            
            Size = new Size(140, 130);
            ElementPanel.Size = Size;
            
            System.Drawing.Drawing2D.GraphicsPath Path = new System.Drawing.Drawing2D.GraphicsPath();
            Path.AddArc(0, 0, ElementPanel.Width-10, ElementPanel.Height-30, 180, 180);
            Point[] Lines = new Point[4];
            Lines[0].X = 0;
            Lines[0].Y = 50;
            Lines[1].X = 0;
            Lines[1].Y = ElementPanel.Height;
            Lines[2].X = ElementPanel.Width - 10;
            Lines[2].Y = ElementPanel.Height;
            Lines[3].X = ElementPanel.Width - 10;
            Lines[3].Y = 50;
            Path.AddLines(Lines);
            Region myRegion = new Region(Path);
            this.Region = myRegion;
            ElementPanel.Controls.Add(ComentsInput);
            ElementPanel.Controls.Add(PropabilityOutput);
            ElementPanel.Controls.Add(PropabilityLabel);
            ElementPanel.MouseEnter += (object obj, EventArgs args) => { GetInfoEvent(this, String.Format("Element:And Value:{0}", propability)); };
            ElementPanel.MouseClick += ((object sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (DeleteVariable != null)
                        DeleteVariable((IVariable)this);
                    if (DeleteControl != null)
                        DeleteControl((ICalculate)this);
                    CallRemoveControl();
                    
                }
            });
           
            
        }

        public void Calculate()
        {
            Task.Factory.StartNew(() =>
            {
                int VariableCount = VariableList.Count;
                double local_propability = 1;
                for (int i = 0; i < VariableCount; i++)
                    if (VariableList[i].Variable != null)
                        local_propability *= VariableList[i].Variable.Value;
                lock (LockObj)
                {
                    propability = local_propability;
                    if (EndOfEdit != null)
                        EndOfEdit();


                    this.Invoke((Action)delegate
                    {
                        PropabilityOutput.Text = (string)local_propability.ToString("0.###E-00").Clone();

                    });
                }

            });
        }
        public void Recalculate()
        {
            int CountVariable = (from iteam in VariableList
                                 where iteam.Variable != null
                                 select iteam).Count();
            CountEndsOfEdit++;
            if (CountEndsOfEdit < CountVariable)
                return;
            CountEndsOfEdit = 0;
            Calculate();
            
        }
       
        public void RemoveVariable(Edge sender)
        {
            if(VariableList.Contains(sender))
               VariableList.Remove(sender);
           
        }
        override public void MouseMove(object sender, MouseEventArgs e)
        {

            Point Delta = new Point();
            if (IsSelected && e.Button == MouseButtons.Left)
            {

                Delta.X = (e.X - LastLocation.X);
                Delta.Y = (e.Y - LastLocation.Y);
                Point LastLocationCtrl = new Point(Location.X,Location.Y);
                Location = new Point(Location.X + Delta.X+69 , Location.Y + Delta.Y  + 64);
                if (Moved != null)
                    Moved(this, new Point(Location.X + 69, Location.Y + 64));
            
            }
            
        }
    }
}
