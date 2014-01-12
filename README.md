BarcodeScannerWP8
=================

Cordova/PhoneGap plugin to enable barcode scanning on Windows Phone 8 using the device's camera.

* The plugin uses the native library [ZXing.Net](http://zxingnet.codeplex.com/) to decode the barcodes.

* The look and feel of the barcode scanner view is consistent with the PhoneGap Build [Android/iOS barcode scanner plugin](https://github.com/phonegap-build/BarcodeScanner/tree/9270025f71891b2f46a38b7bc3d1223b4955dce2)

Licence
=======
MIT

Installation
============

Using CLI:
* cordova plugin add org.bloxlab.barcodescanner (or substitute org.bloxlab.barcodescanner with https://github.com/bloxlab/BarcodeScannerWP8.git)
* cordova build wp8


Usage
=====

Call the method window.cordova.plugins.barcodeScanner.scan, passing in a success and fail call back. Example:

            function scanBarcode() {
                function onSuccess(result) {
                    console.log("Barcode is: " + result.text);
                    console.log("Barcode format: " + result.format);
                }

                function onFail(error) {
                    console.log(error);
                }

                window.cordova.plugins.barcodeScanner.scan(onSuccess, onFail);
            }


(Device will vibrate after detecting a barcode.)
