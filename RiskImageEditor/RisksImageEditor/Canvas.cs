using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace RisksImageEditor
{
    class CanvasPanel:Panel
    {
        public CanvasPanel()
        {
            DoubleBuffered = true;
            AutoSize = true;
                
            
        }
        /*
        private Bitmap bmpCanvas;
    public  PictureBox picture;
    public CanvasPanel()
    {

    // this.SetStyle(ControlStyles.UserPaint, true);
     // this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    //  this.SetStyle(ControlStyles.ResizeRedraw, true);
      bmpCanvas = new Bitmap(500, 500);
        
      picture = new PictureBox();
      picture.Size = new Size(500, 500);
      picture.Location = Location;
      picture.Parent = this;
      picture.Visible=false;
      
    }
    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
   //   bmpCanvas = new Bitmap(bmpCanvas, Size.Width, Size.Height);
      //picture.Size = new Size(Size.Width, Size.Height);
    }
    public Bitmap CanvasBitmap { get { return bmpCanvas; } }
    public Graphics CanvasGraphics { get { return Graphics.FromImage(bmpCanvas); } }
    public void MyPaint(Pen pen, Point Begin, Point End)
    {
        Graphics gr = Graphics.FromImage(bmpCanvas);
        pen.Color = Color.Aquamarine;
        gr.DrawRectangle(Pens.Red, 10, 10, 50, 50);
        gr.DrawLine(pen, Begin, End);
        picture.Visible = true;
        picture.Image = bmpCanvas;
       // picture.Invalidate();
     
    }
    protected override void OnPaint(PaintEventArgs e)
    {
      //Graphics gr = Graphics.FromImage(bmpCanvas);
      //gr.DrawRectangle(Pens.Red, 10, 10, 50, 50);
      //CanvasGraphics.FillRectangle(new SolidBrush(BackColor), e.ClipRectangle);
     // e.Graphics.DrawImage(bmpCanvas, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
      //bmpCanvas = new Bitmap(this.Width, this.Height);
     // picture.Image = bmpCanvas;
     
      base.OnPaint(e);
    }*/
    }
}
