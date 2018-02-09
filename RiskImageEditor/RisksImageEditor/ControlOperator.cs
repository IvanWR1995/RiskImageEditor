using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Windows.Forms;


namespace RisksImageEditor
{
    [Serializable]
    class ControlOperator : BaseControl, ILinked//,IVariable
    {
        protected List<Edge> VariableList;
        protected Object LockObj;
        protected int CountEndsOfEdit;
        public ControlOperator()
        {
            LockObj = new Object();
            VariableList = new List<Edge>();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
           // info.AddValue("VariableList", VariableList);
            

        }
        public override void Deserealize(SerializationInfo info, StreamingContext context)
        {
            base.Deserealize(info, context);
           // VariableList = (List<Edge>)info.GetValue("VariableList",VariableList.GetType());


        }
        
        public void IsLinked(Edge sender, Point Begin, Point End)
        {
            Region reg = this.Region.Clone();
            reg.Translate(Location.X, Location.Y);
            if (reg.IsVisible(Begin)) 
               sender.Link((IVariable)this);
            else if (reg.IsVisible(End) )
            {
                if (!VariableList.Contains(sender) )
                {
                   
                    
                    VariableList.Add(sender);
                    sender.Link((ICalculate)this);
                }
                

            }
            else if (VariableList.Contains(sender))
            {
                ICalculate Calculate = ((ICalculate)this);
                Calculate.RemoveVariable(sender);
                
                sender.Unlink(Calculate);
            }
            else if (!reg.IsVisible(Begin) && sender.Variable == this)
            {
               
                sender.Unlink((IVariable)this);

            }
           
        }
        public bool IsAlreadyLink(Edge edge)
        {
            var AlreadyLink= VariableList.Where((e) => e.Variable == edge.Variable && e!=edge).Select(e => true);
            foreach (var index in AlreadyLink)
                return true;
            return false; 
        }
        
    }
}
