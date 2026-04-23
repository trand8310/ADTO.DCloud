using System.IO;
using ADTOSharp.Extensions;
using ADTOSharp.UI;
using SkiaSharp;


namespace ADTO.DCloud.Graphics
{
    public interface IImageValidator
    {
        void Validate(byte[] imageBytes);
        
        void ValidateDimensions(byte[] imageBytes, int maxWidth, int maxHeight);
    }

    public class SkiaSharpImageValidator : DCloudDomainServiceBase, IImageValidator
    {
        public void Validate(byte[] imageBytes)
        {
            var skImage = SKImage.FromEncodedData(imageBytes);
            
            if (skImage == null)
            {
                throw new UserFriendlyException(L("IncorrectImageFormat"));
            }
        }

        public void ValidateDimensions(byte[] imageBytes, int maxWidth, int maxHeight)
        {
            var skImage = SKImage.FromEncodedData(imageBytes);
            
            if (skImage == null)
            {
                throw new UserFriendlyException(L("IncorrectImageFormat"));
            }
            
            if (skImage.Width > maxWidth || skImage.Height > maxHeight)
            {
                throw new UserFriendlyException(L("IncorrectImageDimensions"));
            }
        }
    }
}
