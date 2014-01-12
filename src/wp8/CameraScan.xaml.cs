//---------------------------------------------------------------------------------------------------------------------
// <summary>
//   The camera view used to scan the barcode.
// </summary>
//---------------------------------------------------------------------------------------------------------------------
namespace BloxLab.BarcodeScannerHelper
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Threading;
    using Cordova.Extension.Commands;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;
    using ZXing;

    /// <summary>
    /// The camera view used to scan the barcode.
    /// </summary>
    public partial class CameraScan : PhoneApplicationPage
    {
        #region Private Member Variables

        /// <summary>
        /// The <see cref="PhotoCamera"/> instance.
        /// </summary>
        private PhotoCamera camera;

        /// <summary>
        /// The bitmap source used by the barcode reader to look for a barcode.
        /// </summary>
        private WriteableBitmap previewBuffer;

        /// <summary>
        /// The <see cref="IBarcodeReader"/> implementation used to detect and decode the barcode.
        /// </summary>
        private IBarcodeReader barcodeReader;

        /// <summary>
        /// The <see cref="DispatcherTimer"/> instance used to control the scan barcode intervals.
        /// </summary>
        private DispatcherTimer scanTimer;

        /// <summary>
        /// The value indicating whether a barcode has been detected.
        /// </summary>
        private bool barcodeFound;

        /// <summary>
        /// The <see cref="BarcodeScanner"/> instance.
        /// </summary>
        private BarcodeScanner command =
            Application.Current.Resources[BarcodeScanner.BarcodeScannerKey] as BarcodeScanner;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="CameraScan"/> instance.
        /// </summary>
        public CameraScan()
        {
            InitializeComponent();
        }
        
        #endregion

        #region Base Overrides

        /// <summary>
        /// The camera scan view has loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender object is ignored.
        /// </param>
        /// <param name="e">
        /// The event arguments are not used.
        /// </param>
        private void CameraScanLoaded(object sender, RoutedEventArgs e)
        {
            this.SetupScanAreaLayout();
        }

        /// <summary>
        /// Navigated to this view.
        /// </summary>
        /// <param name="e">
        /// The event arguments are not used.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.camera = new PhotoCamera();
            this.camera.Initialized += this.CameraInitialised;
            this.camera.AutoFocusCompleted +=
                new EventHandler<CameraOperationCompletedEventArgs>(this.CameraAutoFocusCompleted);

            //Display the camera feed in the UI
            this.vbCamera.SetSource(this.camera);

            // This timer will be used to scan the camera buffer every 250ms and scan for any barcodes
            this.scanTimer = new DispatcherTimer();
            this.scanTimer.Interval = TimeSpan.FromMilliseconds(250);
            this.scanTimer.Tick += (o, r) => this.ScanForBarcode();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Navigating away from this view,
        /// </summary>
        /// <param name="e">
        /// The event arguments are not used.
        /// </param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //we're navigating away from this page, we won't be scanning any barcodes
            this.scanTimer.Stop();

            if (this.camera != null)
            {
                // Cleanup
                this.camera.Dispose();
                this.camera.Initialized -= this.CameraInitialised;
            }
        }
        
        #endregion

        #region Event Handlers

        /// <summary>
        /// The camera object has been initialised.
        /// </summary>
        /// <param name="sender">
        /// The sender object is not used.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void CameraInitialised(object sender, CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    this.camera.FlashMode = FlashMode.Off;
                    this.camera.Focus();

                    var pixelWidth = (int)this.camera.PreviewResolution.Width;
                    var pixelHeight = (int)this.camera.PreviewResolution.Height;
                    this.previewBuffer = new WriteableBitmap(pixelWidth, pixelHeight);

                    this.barcodeReader = new BarcodeReader();
                    this.barcodeReader.Options.TryHarder = true;
                    this.barcodeReader.ResultFound += this.BarcodeReaderResultFound;

                    this.scanTimer.Start();
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.ResolveWithError("Unable to initialize the camera");
                });
            }
        }

        /// <summary>
        /// The auto focus has completed event.
        /// </summary>
        /// <param name="sender">
        /// This parameter is ignored.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void CameraAutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded && !this.barcodeFound)
            {
                // no barcode was found so lets try again
                this.camera.Focus();
            }
        }

        /// <summary>
        /// The barcode has been found.
        /// </summary>
        /// <param name="barcode">
        /// The <see cref="Result"/> instance to represent the scan result.
        /// </param>
        private void BarcodeReaderResultFound(Result barcode)
        {
            this.barcodeFound = true;
            VibrateController.Default.Start(TimeSpan.FromMilliseconds(100));
            this.ResolveWithBarcode(barcode);
        }

        /// <summary>
        /// The cancel button has been clicked.
        /// </summary>
        /// <param name="sender">
        /// The Cancel button.
        /// </param>
        /// <param name="e">
        /// The event arguments are ignored.
        /// </param>
        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Scans for a barcode in the camera snapshot.
        /// </summary>
        private void ScanForBarcode()
        {
            // grab a camera snapshot
            this.camera.GetPreviewBufferArgb32(this.previewBuffer.Pixels);
            this.previewBuffer.Invalidate();

            // scan the captured snapshot for barcodes
            this.barcodeReader.Decode(this.previewBuffer);

        }

        /// <summary>
        /// Sets up the layout for the barcode scan area.
        /// </summary>
        private void SetupScanAreaLayout()
        {
            this.bdrScanner.Height = this.bdrScanner.ActualWidth;
            this.bdrScanner.BorderThickness = new Thickness(9);
            this.lnScanner.X1 = 0;
            this.lnScanner.X2 = this.bdrScanner.ActualWidth;
            var linePosition = this.bdrScanner.Height / 2;
            this.lnScanner.Y1 = linePosition;
            this.lnScanner.Y2 = linePosition;
        }

        /// <summary>
        /// Method to return the scan result to the barcode scanner command.
        /// </summary>
        /// <param name="barcode">
        /// The scan result containing information regarding the barcode.
        /// </param>
        private void ResolveWithBarcode(Result barcode)
        {
            if (command != null)
            {
                command.ResolveWithBarcode(barcode);
            }

            // goes back to the page that initiated the barcode scanner.
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Method to return the error to the barcode scanner command.
        /// </summary>
        /// <param name="barcode">
        /// The scan result containing information regarding the barcode.
        /// </param>
        private void ResolveWithError(string error)
        {
            if (command != null)
            {
                command.ResolveWithError(error);
            }

            // goes back to the page that initiated the barcode scanner.
            this.NavigationService.GoBack();
        }

        #endregion
    }
}