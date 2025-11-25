namespace UI.Components
{

public class RoundCorner{


  private void Main_Load(object sender, EventArgs e)
  {
      ApplyRoundedCorners(20);
  }


private void ApplyRoundedCorners(int radius)
     {
         if (radius == 0)
         {
             this.Region = null;
             return;
         }

         GraphicsPath path = new GraphicsPath();
         path.StartFigure();
         path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
         path.AddLine(radius, 0, this.Width - radius, 0);
         path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90);
         path.AddLine(this.Width, radius, this.Width, this.Height - radius);
         path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90);
         path.AddLine(this.Width - radius, this.Height, radius, this.Height);
         path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90);
         path.CloseFigure();
         this.Region = new Region(path);
     }

}

}
