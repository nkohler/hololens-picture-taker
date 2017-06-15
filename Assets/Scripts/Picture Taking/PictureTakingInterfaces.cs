
namespace Futulabs.HoloFramework.ImageCapture
{
    /// <summary>
    /// Interface for classes that manage the webcamera
    /// </summary>
    public interface ICameraManager
    {
        /// <summary>
        /// Take a picture that is saved to disk and moved to a folder where it can be retrieved externally
        /// </summary>
        void TakePictureToDisk();
    }
}
