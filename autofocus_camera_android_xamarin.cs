public bool OnTouch(Android.Views.View v, MotionEvent e)
{
    if (camera != null)
    {
        var parameters = camera.GetParameters();
        camera.CancelAutoFocus();
        focusIndicator.Visibility = ViewStates.Gone;
        Rect focusRect = CalculateTapArea(e.GetX(), e.GetY(), 1f);
        
        if (parameters.FocusMode != Android.Hardware.Camera.Parameters.FocusModeAuto)
        {
            parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeAuto;
        }
        if (parameters.MaxNumFocusAreas > 0)
        {
            List<Area> mylist = new List<Area>();
            mylist.Add(new Android.Hardware.Camera.Area(focusRect, 1000));
            parameters.FocusAreas = mylist;
        }

        try
        {
            focusIndicator.Visibility = ViewStates.Gone;
            camera.CancelAutoFocus();
            camera.SetParameters(parameters);
            camera.StartPreview();
            camera.AutoFocus(new AutoFocusCallback(this, e.GetX(), e.GetY()));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.Write(ex.StackTrace);
        }
        return true;
    }
    else
    {
        return false;
    } 
}
        
private Rect CalculateTapArea(float x, float y, float coefficient)
{
    var focusAreaSize = 300;
    int areaSize = focusAreaSize * (int)coefficient;

    int centerX = (int)(x / textureView.Width - 1000);
    int centerY = (int)(y / textureView.Height - 1000);

    int left = clamp(centerX - areaSize / 2, -1000, 1000);
    int top = clamp(centerY - areaSize / 2, -1000, 1000);

    RectF rectF = new RectF(left, top, left + areaSize, top + areaSize);

    return new Rect((int)Math.Round(rectF.Left), (int)Math.Round(rectF.Top), (int)Math.Round(rectF.Right), (int)Math.Round(rectF.Bottom));
}
        
private int clamp(int x, int min, int max)
{
    if (x > max)
    {
        return max;
    }
    if (x < min)
    {
        return min;
    }
    return x;
}
    
public class AutoFocusCallback : Java.Lang.Object, IAutoFocusCallback
{
    CameraPage cameraPage;
    float tapX;
    float tapY;

    public AutoFocusCallback(CameraPage cameraPageRef, float xPoint, float yPoint)
    {
        cameraPage = cameraPageRef;
        tapX = xPoint;
        tapY = yPoint;
    }

    public void OnAutoFocus(bool success, Android.Hardware.Camera camera)
    {
        var parameters = camera.GetParameters();
        var supportedFocusModes = parameters.SupportedFocusModes;
        
        var supportedFocusMode = cameraPage.GetSupportedFocusMode();
        parameters.FocusMode = supportedFocusMode;

        if (supportedFocusModes != null && supportedFocusModes.Any())
        {
            if (parameters.MaxNumFocusAreas > 0)
            {
                parameters.FocusAreas = null; 
            }
            if (success)
            {
                TextView focusIndicatorTV = this.cameraPage.focusIndicator;
                focusIndicatorTV.SetX(tapX);
                focusIndicatorTV.SetY(tapY);
                focusIndicatorTV.Visibility = ViewStates.Visible;
            }
            else
            {
                TextView focusIndicatorTV = this.cameraPage.focusIndicator;
                focusIndicatorTV.Visibility = ViewStates.Gone;
            }
            camera.SetParameters(parameters);
            camera.StartPreview();
        }
    }
}
