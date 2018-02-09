using System;
using System.ComponentModel;
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
    class Leaf : BaseControl, IVariable,ILinked
    {
        [NonSerialized]
        MaskedTextBox PropabilityInput;
        public event Action EndOfEdit;
        public event Action<IVariable> DeleteVariable;
        public event Action<Leaf,string> ErrorEvent;
        public double Value 
        {
            get
            {
                return propability; 
            } 
        }
        public Leaf(Point point):this()
        {
            Location = new Point(point.X - ElementPanel.Size.Width / 2, point.Y - ElementPanel.Size.Height / 2);
            
            
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            PropabilityInput.Text = propability.ToString();
            
        }
        public Leaf(SerializationInfo info, StreamingContext context)
           : this()
        {
            base.Deserealize(info, context);
            PropabilityInput.Text = propability.ToString();
            if (Moved != null)
                Moved(this, new Point(Location.X + ComentsInput.Size.Width, Location.Y + ComentsInput.Size.Height));
            
        }
        public  Leaf()
        {
            propability = 0;//Debug
            ComentsInput.Size = new Size(100, 50);
            ComentsInput.Location = new Point(45, 15);

            PropabilityInput = new MaskedTextBox("0.000");
            PropabilityInput.PromptChar = '0';
            PropabilityInput.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            PropabilityInput.Size = new Size(33, 20);
            PropabilityInput.Location = new Point(100, 70);
            PropabilityInput.Parent = this;
            
            PropabilityLabel.Size = new Size(58, 13);
            PropabilityLabel.Location = new Point(42, 76);
            Size = new Size(200, 100);
            ElementPanel.Size = Size;
            System.Drawing.Drawing2D.GraphicsPath Path = new System.Drawing.Drawing2D.GraphicsPath();
            Path.AddEllipse(0, 0, ElementPanel.Width, ElementPanel.Height);
            Region myRegion = new Region(Path);
            this.Region = myRegion;
            ElementPanel.Controls.Add(ComentsInput);
            ElementPanel.Controls.Add(PropabilityInput);
            ElementPanel.Controls.Add(PropabilityLabel);
            ElementPanel.MouseClick += ((object sender, MouseEventArgs e)=>
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (DeleteVariable != null)
                        DeleteVariable((IVariable)this);
                    CallRemoveControl();
                
                }
            });
            
            
            
        }
        public void IsLinked(Edge sender, Point Begin, Point End)
        {
            Region reg = this.Region.Clone();
            reg.Translate(Location.X , Location.Y);
            if (reg.IsVisible(End) && sender.Variable != this)
                MessageBox.Show("The end of the edge can not be connected to the Leaf!", "Warning",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);

            else if (reg.IsVisible(Begin) && sender.Variable != this)
                sender.Link((IVariable)this);
            else if (!reg.IsVisible(Begin) && sender.Variable == this)
            {
                sender.Unlink((IVariable)this);

            }

        }
        public void EndOfEditMethod(object sender, EventArgs e) 
        {
            string str = PropabilityInput.Text;
            if (!double.TryParse(PropabilityInput.Text.Replace('.',','), out propability) || propability > 1)
            {
                PropabilityInput.Font = new Font(PropabilityInput.Font, FontStyle.Underline);
                PropabilityInput.ForeColor = Color.Red;
                if (ErrorEvent != null)
                    ErrorEvent(this,"Incorrect input!");
                    
                return;
            }
            else if (PropabilityInput.ForeColor == Color.Red)
            {
                PropabilityInput.Font = new Font(PropabilityInput.Font, FontStyle.Regular);
                PropabilityInput.ForeColor = Color.Black;
            }
            if (EndOfEdit != null)
                EndOfEdit();
                

        }
        override  public void MouseMove(object sender, MouseEventArgs e)
        {

            Point Delta = new Point();
            if (IsSelected && e.Button == MouseButtons.Left)
            {

                Delta.X = (e.X - LastLocation.X);
                Delta.Y = (e.Y - LastLocation.Y);
                Location = new Point(Location.X + Delta.X + ComentsInput.Size.Width, Location.Y + Delta.Y + ComentsInput.Size.Height);
                 if (Moved != null)
                    Moved(this, new Point(Location.X + ComentsInput.Size.Width, Location.Y + ComentsInput.Size.Height));
            }
            GetInfoEvent(this, String.Format("Element:Leaf Value:{0}", propability));

        }
       


    }

}
