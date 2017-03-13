public static void SetCameraDisplayOrientation(Activity activity, Android.Hardware.Camera camera)
{
    int cameraId = -1;
    CameraInfo info = new CameraInfo();

    for (int i = 0; i < 1; i++)
    {
        if (info.Facing == CameraInfo.CameraFacingBack)
        {
            cameraId = i;
            break;
        }
    }
    
    GetCameraInfo(cameraId, info);
    
    var rotation = activity.WindowManager.DefaultDisplay.Rotation;
    int degrees = 0;
    switch (rotation)
    {
        case SurfaceOrientation.Rotation0: degrees = 0; break;
        case SurfaceOrientation.Rotation90: degrees = 90; break;
        case SurfaceOrientation.Rotation180: degrees = 180; break;
        case SurfaceOrientation.Rotation270: degrees = 270; break;
    }

    int result;
    if (info.Facing == CameraInfo.CameraFacingFront)
    {
        result = (info.Orientation + degrees) % 360;
        result = (360 - result) % 360;  // compensate the mirror
    }
    else
    {  // back-facing
        result = (info.Orientation - degrees + 360) % 360;
    }
    camera.SetDisplayOrientation(result);
}