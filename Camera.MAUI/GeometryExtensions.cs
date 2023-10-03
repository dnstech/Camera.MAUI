namespace Camera.MAUI
{
    using System;

    public static class GeometryExtensions
    {
        public static Point Offset(this Point source, Point dxdy) => new(source.X + dxdy.X, source.Y + dxdy.Y);

        public static Point BottomRight(this Rect rect) => new Point(rect.Right, rect.Bottom);

        public static Rect ToRectFromCenterPoint(this Point center, Size size) => new Rect(center.Offset(size.Width * -0.5, size.Height * -0.5), size);

        public static Point ToRatioOf(this Point position, Size size) => new Point(position.X / size.Width, position.Y / size.Height);

        public static Rect ToRatioOf(this Rect rect, Size size)
        {
            var topLeft = ToRatioOf(rect.Location, size);
            var bottomRight = ToRatioOf(new Point(rect.Right, rect.Bottom), size);
            return new Rect(topLeft, new(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
        }

        public static Point FromRatioOf(this Point position, Size size, bool clamp = false) => 
            clamp ? new Point(Math.Clamp(position.X * size.Width, 0, size.Width), Math.Clamp(position.Y * size.Height, 0, size.Height)) :
                new Point(position.X * size.Width, position.Y * size.Height);

        public static Rect FromRatioOf(this Rect rect, Size size, bool clamp = false)
        {
            var topLeft = FromRatioOf(rect.Location, size, clamp);
            var bottomRight = FromRatioOf(new Point(rect.Right, rect.Bottom), size, clamp);
            return new Rect(topLeft, new(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));
        }

        public static Rect ToRectFromCenterPointAsRatioOf(this Point point, Size frameSize, double focalSize)
        {
            var size = frameSize.IsZero ?
                new Size(focalSize, focalSize) :
                frameSize.Width > frameSize.Height ?
                    new Size(frameSize.Height / frameSize.Width * focalSize, focalSize) :
                    new Size(focalSize, frameSize.Width / frameSize.Height * focalSize);
            return new Rect(new Point(
                point.X - (size.Width * 0.5),
                point.Y - (size.Height * 0.5)),
                size);
        }

        public static Point PreviewToCamera(this Point previewPoint, Size previewSize, Size cameraFrameSize)
        {
            // Always in relation to input frame until last moment
            // Resize to output frame by a ratio.
            var sizeRatio = cameraFrameSize.Width > cameraFrameSize.Height ? cameraFrameSize.Height / previewSize.Height : cameraFrameSize.Width / previewSize.Width;
            var offsetCoordinate = previewPoint.Offset(previewSize.Width * -0.5, previewSize.Height * -0.5); // Set center as origin
            var outputCoordinate = new Point(offsetCoordinate.X * sizeRatio, offsetCoordinate.Y * sizeRatio); // Scale
            outputCoordinate = outputCoordinate.Offset(cameraFrameSize.Width * 0.5, cameraFrameSize.Height * 0.5); // Reset origin back to top left
            return outputCoordinate;
        }

        /// <summary>Calculates the position on the camera frame in relation to the preview frame as a ratio of the preview frame, where the preview frame is centered and scaled with AspectToFill (i.e. aspect ratio preserved, scaled to fill).</summary>
        public static Point CameraToPreview(this Point cameraPoint, Size cameraFrameSize, Size previewFrame)
        {
            // Calculate the size ratio based on the input and output frames
            var widthRatio = previewFrame.Width / cameraFrameSize.Width;
            var heightRatio = previewFrame.Height / cameraFrameSize.Height;

            // Use the larger of the two ratios to ensure the camera frame fills the preview frame
            var sizeRatio = Math.Max(widthRatio, heightRatio);

            // Offset the input coordinate to set the center as the origin
            var offsetCoordinate = cameraPoint.Offset(cameraFrameSize.Width * -0.5, cameraFrameSize.Height * -0.5);

            // Scale the offset coordinate by the size ratio
            var outputCoordinate = new Point(offsetCoordinate.X * sizeRatio, offsetCoordinate.Y * sizeRatio);

            // Reset the origin back to the top left
            outputCoordinate = outputCoordinate.Offset(previewFrame.Width * 0.5, previewFrame.Height * 0.5);

            // Return the ratio of the output coordinate to the output frame
            return outputCoordinate;
        }
    }
}
