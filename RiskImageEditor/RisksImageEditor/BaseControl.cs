using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace RisksImageEditor
{
    
    class BaseControl : UserControl,IMove,ISerializable
    {
       
       protected  TextBox ComentsInput;
      
       protected  Label PropabilityLabel;
       
       protected  Panel ElementPanel;
       public  double propability;
       protected bool IsSelected;
       protected Point LastLocation;
       public EventHandler<Point> Moved;
       public EventHandler<string> GetInfoEvent;
       public EventHandler LeaveEvent;
       public bool IsMouseDownFlag;
       public event Action<BaseControl> RemoveControl;
      // public delegate void EditEnd();
       public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
            info.AddValue("propability", propability);
            info.AddValue("Coments", ComentsInput.Text);
            
        }
       public virtual void Deserealize(SerializationInfo info, StreamingContext context)
        {
            base.Location = (Point)info.GetValue("Location", Location.GetType());
            propability = info.GetDouble("propability");
            ComentsInput.Text = info.GetString("Coments");

            
        }
        
       public BaseControl()
       {
           IsMouseDownFlag = false;
           LastLocation = new Point();
           IsSelected = false;
           ComentsInput = new TextBox();
           ComentsInput.Multiline = true;
           ComentsInput.Parent = this;
           ComentsInput.ScrollBars = ScrollBars.Vertical;

           PropabilityLabel = new Label();
           PropabilityLabel.Text = "Probability:";

           ElementPanel = new Panel();
           ElementPanel.BackColor = SystemColors.ControlDark;
           ElementPanel.Location = new Point(0, 0);
           ElementPanel.Parent = this;
           ElementPanel.MouseDown += MouseDown;
           ElementPanel.MouseMove += MouseMove;
           ElementPanel.MouseUp += MouseUp;
           ElementPanel.MouseLeave += (object obj, EventArgs args) =>
           {
               if (LeaveEvent != null)
                   LeaveEvent(obj, args);
           };
        }

       protected void CallRemoveControl()
       {
           if (RemoveControl != null)
            RemoveControl(this);
          
       }
       virtual public void MouseMove(object sender, MouseEventArgs e)
       {
          
           
       }
       public void MouseDown(object sender, MouseEventArgs e)
       {
           if (e.Button == MouseButtons.Left)
           {
               IsSelected = true;
               LastLocation = e.Location;
              
               IsMouseDownFlag = true;   
           }
       }
       public void MouseUp(object sender, MouseEventArgs e)
       {
           if(IsSelected)
            IsSelected = false;
       }
       public  void ComputeRisk(Object obj, double risk)
       {
           if (IsMouseDownFlag)
           {
               ((ToolStripTextBox)obj).Text = (risk * propability).ToString();
               
               IsMouseDownFlag = false;
           }
       }
       
       public new Point Location  
       {
           get { return base.Location; }
           set { base.Location = new Point(value.X - ElementPanel.Size.Width / 2, value.Y - ElementPanel.Size.Height / 2); }
        }
       /*public void IsLinked(Edge sender, Point Begin, Point End)
       {
           Region reg = this.Region.Clone();
           reg.Translate(Location.X, Location.Y);
           // правильная логика
         /*  if (reg.IsVisible(Begin) )
               sender.Link((IVariable)this);
           else if(reg.IsVisible(End))
               sender.Link((ICalculate)this);*/

       //}
       public void EmitMove()
       {
           if (Moved != null)
               Moved(this, new Point(Location.X + 69, Location.Y + 64));
       }
       public void EmitMove(Point NewPoint)
       {
           if (Moved != null)
               Moved(this, NewPoint);
       }
    }
}
