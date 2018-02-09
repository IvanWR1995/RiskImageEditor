using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Drawing;

namespace RisksImageEditor
{
    [Serializable]
    class ORControl : ControlOperator, ICalculate,IVariable
    {
        Label PropabilityOutput;
        Point NewLocation;

        public event Action EndOfEdit;
        public event Action<IVariable> DeleteVariable;
        public event Action<ICalculate> DeleteControl;
        public double Value { get { return propability; } }
              
        public ORControl(Point point):this()
        {
            Location = new Point(point.X - ElementPanel.Size.Width / 2, point.Y - ElementPanel.Size.Height / 2);
            
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
        }
        public ORControl(SerializationInfo info, StreamingContext context)
            : this()
        {
            base.Deserealize(info, context);
            PropabilityOutput.Text =(string)propability.ToString("0.###E-00").Clone();
            EmitMove(Location);
            
        }
        public ORControl()
        {
            NewLocation = new Point();
            ComentsInput.Size = new Size(100, 50);
            ComentsInput.Location = new Point(15, 35);
            

            PropabilityOutput = new Label();
            PropabilityOutput.Size = new Size(33, 20);
            PropabilityOutput.Location = new Point(75, 90);
            PropabilityOutput.AutoSize = true;
            PropabilityOutput.Parent = this;

            PropabilityLabel.Location = new Point(15, 90);
            PropabilityLabel.Size = new Size(58, 13);
           
            Size = new Size(140, 155);
            ElementPanel.Size = Size;
           
            System.Drawing.Drawing2D.GraphicsPath Path = new System.Drawing.Drawing2D.GraphicsPath();
            Path.AddArc(0, 0, ElementPanel.Width - 10, 100, 180, 180);
            Path.AddLine(0,50, 0,ElementPanel.Height);
            Path.AddArc(0, ElementPanel.Height - 40, ElementPanel.Width - 10, 35, 180, 180);
            Path.AddLine(ElementPanel.Width - 10, ElementPanel.Height, ElementPanel.Width - 10, 50);
            
            Region myRegion = new Region(Path);
          
            this.Region = myRegion;
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
            ElementPanel.Controls.Add(ComentsInput);
            ElementPanel.Controls.Add(PropabilityOutput);
            ElementPanel.Controls.Add(PropabilityLabel);
           
            
        }
        public void RemoveVariable(Edge sender)
        {
            if (VariableList.Contains(sender))
                VariableList.Remove(sender);
           
        }
        public void Calculate()
        {
            Task.Factory.StartNew(() =>
            {
                double local_propability = 0;
                int ListCount = VariableList.Count;
                double multiply = 1;
                List<string> list = new List<string>();
                for (int i = 1; i < ListCount; i++)
                    if (VariableList[i].Variable != null)
                        Comb(0, ListCount, i, string.Empty, ref list);
                foreach (string str in list)
                {
                    for (int j = 0; j < ListCount; j++)
                    {
                        if (VariableList[j].Variable != null)
                        {
                            if (str.Contains(j.ToString()))
                                multiply *= (1 - VariableList[j].Variable.Value);
                            else
                                multiply *= VariableList[j].Variable.Value;
                        }
                    }
                    local_propability += multiply;
                    multiply = 1;

                }
                for (int j = 0; j < ListCount; j++)
                    if (VariableList[j].Variable != null)
                        multiply *= VariableList[j].Variable.Value;
                local_propability += multiply;
                lock (LockObj)
                {
                    propability = local_propability;


                    this.Invoke((Action)delegate
                    {
                        PropabilityOutput.Text = (string)propability.ToString("0.###E-00").Clone();

                    });
                    if (EndOfEdit != null)
                        EndOfEdit();
                }
            });
        }
        public void Recalculate()
        {
            int CountVariable = (from iteam in VariableList
                                 where iteam.Variable != null && iteam.Variable.Value!=0
                                select iteam).Count();
            CountEndsOfEdit++;
            if (CountEndsOfEdit < CountVariable)
                return;
            CountEndsOfEdit = 0;
            Calculate();
        }
        void Comb(int Start, int N, int K, string OldComb, ref List<string> list)
        {
            string NewComb;
            for (int i = Start; i != N - K + 1; i++)
            {
                NewComb = String.Join("|",OldComb, i.ToString());
                if (K == 1)
                    list.Add(NewComb);
                else
                    Comb(i + 1, N, K - 1, NewComb, ref list);
            }
        }
  
        
        override public void MouseMove(object sender, MouseEventArgs e)
        {

            Point Delta = new Point();
            if (IsSelected && e.Button == MouseButtons.Left)
            {
                Delta.X = (e.X - LastLocation.X);
                Delta.Y = (e.Y - LastLocation.Y);
                NewLocation.X = Location.X + Delta.X + 69;
                NewLocation.Y = Location.Y + Delta.Y +76;
                Location = NewLocation;
                EmitMove(NewLocation);
                
            }
            GetInfoEvent(this,String.Format("Element:Or Value:{0}", propability));

        }
    }
}
