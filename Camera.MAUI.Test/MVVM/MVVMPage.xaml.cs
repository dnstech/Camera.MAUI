namespace Camera.MAUI.Test;

public partial class MVVMPage : ContentPage
{
	public MVVMPage()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		if (this.BindingContext is CameraViewModel vm && e.GetPosition(this.cameraView) is Point position)
		{
			// Translate the camera viewport size into a ratio from left to right, top to bottom (0.0 to 1.0).
			vm.FocalPoint = new Rect(position.X / this.cameraView.Width, position.Y / this.cameraView.Height, 0.05, 0.05);

        }
    }
}