using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace RisksImageEditor
{
    class SlideToolBar:UserControl
    {
        Panel ToolPanel;
        Button HideShowBtn;
        Button OrCreateBtn, AndCreateBtn, LeafCreateBtn, EdgeCreateBtn; 
        Size ShowSz;
        bool IsShow;
        public EventHandler<BaseControl> CreateBtnClick;
        public EventHandler<Edge> CreateEdgeBtnClick;
        public EventHandler<Size> OpenCloseClick;
        public Size OpenSize
        {
            get { return ShowSz; }
        }
        
        public SlideToolBar(Size sz, Point location)
        {
           
            IsShow = false;
            Location = location;
            Size = new Size(50, sz.Height);
            
            ShowSz = sz;
            ToolPanel = new Panel();
            ToolPanel.Size = new Size(sz.Width-60, sz.Height);
            ToolPanel.Parent = this;
            ToolPanel.Location = new Point(0, 0);
            ToolPanel.BackColor = SystemColors.ControlLight;
            BackColor = SystemColors.ControlLight;
            ToolPanel.Visible = false;
                
            HideShowBtn = new Button();
            HideShowBtn.Size = new Size(50,50);
            HideShowBtn.Location = new Point(20, ToolPanel.Size.Height / 2);
            HideShowBtn.Text = ">";
            HideShowBtn.Visible = true;
            HideShowBtn.Click += HideShowBtn_Click;
            HideShowBtn.BackColor = SystemColors.ControlDark;
            HideShowBtn.FlatAppearance.BorderColor = SystemColors.ControlDark;
            HideShowBtn.FlatStyle = FlatStyle.Flat;
            System.Drawing.Drawing2D.GraphicsPath Path = new System.Drawing.Drawing2D.GraphicsPath();
            Path.AddArc(10, 0, 50,50, -90, -180);
            HideShowBtn.Region = new Region(Path);

            OrCreateBtn = new Button();
            OrCreateBtn.Size = new Size(120, 100);
            OrCreateBtn.Location = new Point(20, 40);
            OrCreateBtn.Image = Properties.Resources.OR;
            OrCreateBtn.Visible = false;
            OrCreateBtn.Parent = ToolPanel;
            OrCreateBtn.Click += OrCreateBtn_Click; 

            AndCreateBtn = new Button();
            AndCreateBtn.Size = new Size(120, 105);
            AndCreateBtn.Location = new Point(20, 160);
            AndCreateBtn.Image = Properties.Resources.And;
            AndCreateBtn.Visible = false;
            AndCreateBtn.Parent = ToolPanel;
            AndCreateBtn.Click += AndCreateBtn_Click; 

            LeafCreateBtn = new Button();
            LeafCreateBtn.Size = new Size(120, 80);
            LeafCreateBtn.Location = new Point(20, 280);
            LeafCreateBtn.Image = Properties.Resources.El;
            LeafCreateBtn.Visible = false;
            LeafCreateBtn.Parent = ToolPanel;
            LeafCreateBtn.Click += LeafCreateBtn_Click;

            EdgeCreateBtn = new Button();
            EdgeCreateBtn.Size = new Size(120, 80);
            EdgeCreateBtn.Location = new Point(20, 400);
            EdgeCreateBtn.Image = Properties.Resources.Edge;
            EdgeCreateBtn.Visible = false;
            EdgeCreateBtn.Parent = ToolPanel;
            EdgeCreateBtn.Click += EdgeCreateBtn_Click;
            
            
            HideShowBtn.Parent = this;
            Controls.Add(HideShowBtn);
            ToolPanel.Controls.Add(OrCreateBtn);
            ToolPanel.Controls.Add(AndCreateBtn);
            CreateBtnClick += (Object obj, BaseControl e) => { };

            
            
        }
        public void EdgeCreateBtn_Click(Object obj, EventArgs e)
        {
            if (CreateEdgeBtnClick != null)
                CreateEdgeBtnClick(this,new Edge());
        }
        public void OrCreateBtn_Click(Object obj, EventArgs e)
        {
            CreateBtnClick(this, new ORControl());
        }
        public void AndCreateBtn_Click(Object obj, EventArgs e)
        {
            CreateBtnClick(this, new AndControl());
        }
        public void LeafCreateBtn_Click(Object obj, EventArgs e)
        {
            CreateBtnClick(this, new Leaf());
        }
        
        public void HideShowBtn_Click(Object obj, EventArgs e)
        {
            if (IsShow)
            {
                HideShowBtn.Text = ">";
                Size = new Size(HideShowBtn.Size.Width,ShowSz.Height);
                ToolPanel.Visible = false;
                HideShowBtn.Location= new Point(20 , ToolPanel.Size.Height / 2);
                IsShow = false;
                OrCreateBtn.Visible = false;
                AndCreateBtn.Visible = false;
                LeafCreateBtn.Visible = false;
                EdgeCreateBtn.Visible = false;
             
            }
            else
            {
                HideShowBtn.Text = "<";
                Size = ShowSz;
                ToolPanel.Visible = true;
                HideShowBtn.Location = new Point(Size.Width - 30, ToolPanel.Size.Height / 2);
                OrCreateBtn.Visible = true;
                AndCreateBtn.Visible = true;
                LeafCreateBtn.Visible = true;
                EdgeCreateBtn.Visible = true;
                IsShow = true;
            }
            if (OpenCloseClick != null)
                OpenCloseClick(this,Size);

        }
    }
}
