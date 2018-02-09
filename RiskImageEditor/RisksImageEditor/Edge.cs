using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace RisksImageEditor
{
    [Serializable]
    class Edge : IMove, ISerializable
    {
        Point  BeginPoint, EndPoint,LastLocation;
        GraphicsPath PathLine,BeginEllips,EndEllips,ForInvalidate;
        public delegate void EndsEdge(Edge sender,Point Begin ,Point End);
        public event EndsEdge Move;
        public event Action<Edge> RemoveEdge;
        int FlagVisible;
        ICalculate calculate;
        IVariable variable;
        Region region;
        public Edge(SerializationInfo info, StreamingContext context)
        {
            FlagVisible = 0;
            region = new Region();
            ForInvalidate = new GraphicsPath();
            variable = null;
            calculate = null;
            pen = new Pen(Color.Black, 7);
            PathLine = new GraphicsPath();
            BeginEllips = new GraphicsPath();
            EndEllips = new GraphicsPath();

            BeginPoint = (Point)info.GetValue("BeginPoint",BeginPoint.GetType());
            EndPoint = (Point)info.GetValue("EndPoint", EndPoint.GetType());
            LastLocation = (Point)info.GetValue("LastLocation", LastLocation.GetType());
            try
            {
                calculate = (ICalculate)info.GetValue("calculate", ((BaseControl)calculate).GetType());
            }
            catch
            {
                calculate = null;
            }
            try
            {
                variable = (IVariable)info.GetValue("variable", variable.GetType());
            }
            catch {
                variable = null;
            }
            BeginEllips.AddEllipse(BeginPoint.X - 10, BeginPoint.Y - 10, 20, 20);
            PathLine.AddLine(BeginPoint, EndPoint);
            EndEllips.AddEllipse(EndPoint.X - 10, EndPoint.Y - 10, 20, 20);
            
            if(Move != null)
                Move(this, BeginPoint, EndPoint);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BeginPoint", BeginPoint);
            info.AddValue("LastLocation", LastLocation);
            info.AddValue("EndPoint", EndPoint);
         //   info.AddValue("calculate", calculate);
           // info.AddValue("variable", variable);
        }
        public  void Deserealize(SerializationInfo info, StreamingContext context)
        {
            BeginPoint = (Point)info.GetValue("BeginPoint", BeginPoint.GetType());
            LastLocation = (Point)info.GetValue("LastLocation", LastLocation.GetType());
            EndPoint = (Point)info.GetValue("EndPoint", EndPoint.GetType());
         //   calculate = (ICalculate)info.GetValue("calculate", calculate.GetType());
           // variable = (IVariable)info.GetValue("variable", variable.GetType());

        }
        public IVariable Variable
        {
            get { return variable; } 
        }
        public ICalculate Calculate
        {
            get { return calculate; }
        }
        [NonSerialized]
        Pen pen;
        public new Point  Location
        {
            set {
                    BeginPoint = new Point(value.X - 18, value.Y + 50);
                    EndPoint = new Point(value.X + 18, value.Y - 50);

                    BeginEllips.AddEllipse(BeginPoint.X - 10, BeginPoint.Y- 10, 20, 20);
                    PathLine.AddLine(BeginPoint, EndPoint);
                    EndEllips.AddEllipse(EndPoint.X - 10, EndPoint.Y-10, 20, 20);
                    
            }
           
        }
        public Point BeginLocation
        {
            set 
            { 
                BeginPoint = value;
                BeginEllips = new GraphicsPath();
                PathLine = new GraphicsPath();
                BeginEllips.AddEllipse(BeginPoint.X - 10, BeginPoint.Y - 10, 20, 20);
                PathLine.AddLine(BeginPoint, EndPoint);
            }
            get { return BeginPoint; }
        }
        public Point EndLocation
        {
            set 
            {
                EndPoint = value;
                EndEllips = new GraphicsPath();
                PathLine = new GraphicsPath();
                EndEllips.AddEllipse(EndPoint.X - 10, EndPoint.Y - 10, 20, 20);
                PathLine.AddLine(BeginPoint, EndPoint);
            }
            get { return EndPoint; }
        }
        public Edge()
        {
            FlagVisible = 0;
            region = new Region();
            variable = null;
            calculate = null;
            pen = new Pen(Color.Black, 6);
            PathLine = new GraphicsPath();
            BeginEllips = new GraphicsPath();
            EndEllips = new GraphicsPath();
            if (Move != null)
                Move(this, BeginPoint, EndPoint);
           
           
        }

        
        public Edge(Point point):this()
        {
            Location = point;

        }
        public void Unlink(ICalculate control)
        {
            if (calculate != null)
            {
                if (((BaseControl)calculate).Moved != null)
                    ((BaseControl)calculate).Moved -= MoveCalculateElement;
                calculate = null;
            }
        
        }
        public void Unlink(IVariable control)
        {
            if (variable != null)
            {
                if (((BaseControl)variable).Moved!=null)
                    ((BaseControl)variable).Moved -= MoveValueElement;
                variable = null;
                
            }
        
        }
        public void Link(ICalculate control)
        {
            if ((variable != null && variable == ((IVariable)control)))
                return;
            calculate = control;
            ((BaseControl)calculate).Moved += MoveCalculateElement;
            calculate.DeleteControl+= Unlink;
            if (calculate != null && variable != null)
            {
                variable.EndOfEdit += calculate.Recalculate;
               
              
            }
        }

        
        public void Link(IVariable control)
        {
            if ((calculate != null && ((IVariable)calculate) == control))
                return;
            variable = control;
            ((BaseControl)variable).Moved += MoveValueElement;
            variable.DeleteVariable += Unlink;
            if (calculate != null && variable != null)
            {
                variable.EndOfEdit += calculate.Recalculate;
               
              
            }
        }
        void MoveCalculateElement(Object sender,Point Delta)
        {
            region = new Region();
            EndPoint = Delta;
            EndEllips.Reset();
            EndEllips.AddEllipse(EndPoint.X - 10, EndPoint.Y - 10, 20, 20);
            region.Union(PathLine);
            PathLine.Reset();
            PathLine.AddLine(BeginPoint, EndPoint);
            region.Union(PathLine);
            ((Control)sender).Parent.Invalidate(region,false);
          
        }
        void MoveValueElement(Object sender, Point Delta)
        {
            BeginPoint = Delta;
            BeginEllips.Reset();
            BeginEllips.AddEllipse(BeginPoint.X - 10, BeginPoint.Y - 10, 20, 20);
                   

            PathLine.Reset();
            PathLine.AddLine(BeginPoint, EndPoint);
            ((Control)sender).Parent.Invalidate();
        }
        
        public void MouseMove(object sender, MouseEventArgs e)
        {
            Point Delta = new Point();
            
            if (FlagVisible != 0)
            {
                Delta.X = e.X - LastLocation.X;
                Delta.Y = e.Y - LastLocation.Y;
            }
            switch (FlagVisible)
            {
                case 1:
                    BeginPoint.X += Delta.X;
                    BeginPoint.Y += Delta.Y;
                    BeginEllips =  new GraphicsPath();
                    BeginEllips.AddEllipse(BeginPoint.X - 10, BeginPoint.Y - 10, 20, 20);
                    break;
                case 2:
                    EndPoint.X += Delta.X;
                    EndPoint.Y += Delta.Y;
                    EndEllips.Reset();
                    EndEllips.AddEllipse(EndPoint.X - 10, EndPoint.Y - 10, 20, 20);
                    break;
                case 3:
                    BeginPoint.X += Delta.X;
                    BeginPoint.Y += Delta.Y;
                    EndPoint.X += Delta.X;
                    EndPoint.Y += Delta.Y;
                    BeginEllips.Reset();
                    EndEllips.Reset();
                    BeginEllips.AddEllipse(BeginPoint.X - 10, BeginPoint.Y - 10, 20, 20);
                    EndEllips.AddEllipse(EndPoint.X - 10, EndPoint.Y - 10, 20, 20);
                    break;
            }
            if (FlagVisible != 0)
            {
                PathLine.Reset();
                PathLine.AddLine(BeginPoint, EndPoint);
                LastLocation = e.Location;
                
                if(sender is CanvasPanel )
                    ((Control)sender).Invalidate();
            }
        }
        public void EmitMove()
        {
            if (Move != null)
                Move(this, BeginPoint, EndPoint);
        }
        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (FlagVisible != 0)
                FlagVisible = 0;
            EmitMove();
            
        }
        public void Paint(object sender, PaintEventArgs e)
        {
            pen.EndCap = LineCap.ArrowAnchor;
            pen.StartCap = LineCap.RoundAnchor;
            e.Graphics.DrawLine(pen, BeginPoint,EndPoint);
            
           
        }
        public void ControlMove(object sender, EventArgs e)
        {
            if (Move != null)
            {
                Move(this, BeginPoint, EndPoint);
            }
        }
        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (BeginEllips.IsVisible(e.Location))
                    FlagVisible = 1;
                else if (EndEllips.IsVisible(e.Location))
                    FlagVisible = 2;
                else if (PathLine.IsOutlineVisible(e.Location,pen))
                    FlagVisible = 3;
                if (FlagVisible != 0)
                    LastLocation = e.Location;
               
            }
            else if (e.Button == MouseButtons.Right && PathLine.IsOutlineVisible(e.Location, pen))
            {
                if (calculate != null)
                {
                    calculate.RemoveVariable(this);
                    
                }
                if (RemoveEdge != null)
                {
                    RemoveEdge(this);
                    ((Control)sender).Invalidate();
                }


            }
        }
    }
}
