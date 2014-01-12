//---------------------------------------------------------------------------------------------------------------------
// <summary>
//   Cordova command to scan a barcode using the device's camera.
// </summary>
//---------------------------------------------------------------------------------------------------------------------
namespace Cordova.Extension.Commands
{
    using System;
    using System.Windows;
    using BloxLab.BarcodeScannerHelper;
    using Microsoft.Phone.Controls;
    using WPCordovaClassLib.Cordova;
    using WPCordovaClassLib.Cordova.Commands;
    using WPCordovaClassLib.Cordova.JSON;
    using ZXing;

    /// <summary>
    /// Class that represents a command to scan a barcode using the device's camera.
    /// </summary>
    public class BarcodeScanner: BaseCommand
    {
        /// <summary>
        /// The name of the barcode scanner key.  
        /// </summary>
        internal const string BarcodeScannerKey = "BarcodeScanner";

        /// <summary>
        /// The method that initiates the camera to scan barcode.
        /// </summary>
        /// <param name="options">
        /// This parameter is not used.
        /// </param>
        public void Scan(string options)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (Application.Current.Resources.Contains(BarcodeScannerKey))
                {
                    Application.Current.Resources.Remove(BarcodeScannerKey);
                }

                Application.Current.Resources.Add(BarcodeScannerKey, this);
                var applicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (applicationFrame != null)
                {
                    applicationFrame.Navigate(new Uri("/Plugins/org.bloxlab.barcodescanner/CameraScan.xaml", UriKind.Relative));
                }
                else
                {
                    this.ResolveWithError("No application frame.");
                }
            });
        }

        /// <summary>
        /// Method to return the scan result to the calllee.
        /// </summary>
        /// <param name="barcode">
        /// The scan result containing information regarding the barcode.
        /// </param>
        internal void ResolveWithBarcode(Result barcode)
        {
            Application.Current.Resources.Remove(BarcodeScannerKey);
            var result = JsonHelper.Serialize(new BarcodResultInfo(barcode));
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, result));
        }

        /// <summary>
        /// Method to return the error to the calllee.
        /// </summary>
        /// <param name="error">
        /// The error message.
        /// </param>
        internal void ResolveWithError(string error)
        {
            Application.Current.Resources.Remove(BarcodeScannerKey);
            DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, error));
        }
    }
}
