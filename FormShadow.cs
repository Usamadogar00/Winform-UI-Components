
namespace UI.Components
{



// Add shadow effect (Windows 10+)
  if (Environment.OSVersion.Version.Major >= 10)
  {
      DwmDropShadow.ApplyShadow(this.Handle);


  }

private static class DwmDropShadow
     {
         [DllImport("dwmapi.dll")]
         private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

         private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
         private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

         public static void ApplyShadow(IntPtr handle)
         {
             int preference = 2; // Rounded corners
             DwmSetWindowAttribute(handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(int));
         }
     }
}
