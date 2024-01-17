namespace TimesheetApp.Controls.Timesheet;

public class RadialProgressbarDrawable : IDrawable
{
    private readonly RadialProgressBarControl _radialProgressBarControl;

    public RadialProgressbarDrawable(RadialProgressBarControl radialProgressBarControl) => _radialProgressBarControl = radialProgressBarControl;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var progress = _radialProgressBarControl.Progress;
        var color = _radialProgressBarControl.ProgressColor;
        var isClockWise = progress > 0;

        var progressAngle = progress / 100;
        var centerX = dirtyRect.Width / 2;
        var centerY = dirtyRect.Height / 2;
        var radius = dirtyRect.Width / 2 - 7.5f;

        var startAngle = 90f;
        var endAngle = startAngle - (float)Math.Round(progressAngle * 360, 1);

        canvas.SaveState();

        canvas.StrokeSize = 15f;
        canvas.StrokeColor = _radialProgressBarControl.BackColor;
        canvas.DrawCircle(centerX, centerY, radius);

        if (progress == 100)
        {
            canvas.FillColor = new Color(color.Red, color.Green, color.Blue, 0.1f);
            canvas.FillCircle(centerX, centerY, radius);
            canvas.StrokeColor = color;
            canvas.DrawCircle(centerX, centerY, radius);
        }
        else
        {
            var path = new PathF();
            var innerRadius = radius * 2 + 7.5f;

            path.MoveTo(centerX, centerY);
            path.AddArc(centerX - radius, centerY - radius, innerRadius, innerRadius, startAngle, endAngle, isClockWise);

            canvas.FillColor = new Color(color.Red, color.Green, color.Blue, 0.1f);
            canvas.FillPath(path);

            canvas.StrokeColor = color;
            canvas.DrawArc(centerX - radius, centerY - radius, radius * 2, radius * 2, startAngle, endAngle, isClockWise, false);
        }
        canvas.RestoreState();
    }
}