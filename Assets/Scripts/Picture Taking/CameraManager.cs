using Futulabs.HoloFramework.ImageCapture;
using Zenject;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.Windows.Speech;
using System.Linq;
#if WINDOWS_UWP
using Windows.Storage;
using Windows.System;
using System.Collections.Generic;
using System;
using System.IO;
#endif

public class CameraManager : ICameraManager
{
    private PhotoCapture        _photoCaptureObject;
    private CameraParameters    _cameraParameters;

    private bool                _startingPhotoMode          = false;
    private bool                _readyToTakePicture         = false;
    private bool                _takingPicture              = false;

    private int                 _currentPhotoNumber         = 0;
    private string              _imageName;
    private string              _filePath;

    private KeywordRecognizer   _keywordRecognizer;

    private Text                _infoText;

    #region Keywords
    private string[]            _keywords                   = { "Start camera", "Take picture" };
    #endregion

    #region Info strings
    private const string        _startingPhotoModeString    = "Starting photo mode";
    private const string        _takingPictureString        = "Taking picture";
    private const string        _readyToTakePictureString   = "Ready to take picture";
    private const string        _failedToStartString        = "Failed to start photo mode";
    #endregion

    public CameraManager([Inject(Id = "Info text")] Text infoText)
    {
        _infoText = infoText;

        _startingPhotoMode = true;
        _infoText.text = _startingPhotoModeString;
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        _keywordRecognizer = new KeywordRecognizer(_keywords);
        _keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    public void TakePictureToDisk()
    {
        _imageName = string.Format("SampleImage{0}.jpg", _currentPhotoNumber);
        _filePath = System.IO.Path.Combine(Application.persistentDataPath, _imageName);

        _photoCaptureObject.TakePhotoAsync(_filePath, PhotoCaptureFileOutputFormat.JPG, OnPhotoCapturedToDisk);
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (!_startingPhotoMode)
        {
            if (args.text == _keywords[0] && !_readyToTakePicture && !_takingPicture)
            {
                _startingPhotoMode = true;
                _infoText.text = _startingPhotoModeString;
                _photoCaptureObject.StartPhotoModeAsync(_cameraParameters, OnPhotoModeStarted);
            }
            else if (args.text == _keywords[1] && _readyToTakePicture)
            {
                _readyToTakePicture = false;
                _takingPicture = true;
                _infoText.text = _takingPictureString;
                TakePictureToDisk();
            }
        }
    }

#region Photo callbacks
    private void OnPhotoCaptureCreated(PhotoCapture photoCaptureObject)
    {
        _photoCaptureObject = photoCaptureObject;

        // Get highest available resolution
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();

        // Setup camera parameters
        _cameraParameters = new CameraParameters();
        _cameraParameters.hologramOpacity = 0.0f;
        _cameraParameters.cameraResolutionWidth = cameraResolution.width;
        _cameraParameters.cameraResolutionHeight = cameraResolution.height;
        _cameraParameters.pixelFormat = CapturePixelFormat.NV12;

        _photoCaptureObject.StartPhotoModeAsync(_cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            _readyToTakePicture = true;
            _infoText.text = _readyToTakePictureString;
        }
        else
        {
            Debug.LogError("Failed to start photo mode");
            _readyToTakePicture = false;
            _infoText.text = _failedToStartString;
        }
        _startingPhotoMode = false;
    }

    private void OnPhotoCapturedToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
    {
        if (result.success)
        {
            Debug.Log("Captured image to memory. Uploading to texture.");
        }
        else
        {
            Debug.LogError("Failed to capture image to memory.");
        }
        _takingPicture = false;
        _readyToTakePicture = true;
        _infoText.text = _readyToTakePictureString;
    }

    private void OnPhotoCapturedToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            _currentPhotoNumber++;
            Debug.Log("Saved image to disk.");
#if WINDOWS_UWP
            string cameraRollFolder = KnownFolders.CameraRoll.Path;
            File.Move(_filePath, Path.Combine(cameraRollFolder, _imageName));
#endif
        }
        else
        {
            Debug.LogError("Failed to save image to disk");
        }
        _takingPicture = false;
        _readyToTakePicture = true;
        _infoText.text = _readyToTakePictureString;
    }
#endregion
}
