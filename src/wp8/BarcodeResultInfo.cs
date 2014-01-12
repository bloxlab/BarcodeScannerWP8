//---------------------------------------------------------------------------------------------------------------------
// <summary>
//   Class to represent the barcode result.
// </summary>
//---------------------------------------------------------------------------------------------------------------------
namespace BloxLab.BarcodeScannerHelper
{
    using System.Runtime.Serialization;
    using ZXing;

    /// <summary>
    /// Class to represent the barcode result.
    /// </summary>
    /// <remarks>
    /// Used for serialization purposes - Zxing Result object is not serializable.
    /// </remarks>
    [DataContract()]
    public sealed class BarcodResultInfo
    {
        #region Properties

        /// <summary>
        /// Gets the barcode's text value.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; private set; }

        /// <summary>
        /// Gets the barcode's format.
        /// </summary>
        [DataMember(Name = "format")]
        public string Format { get; private set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodResultInfo"/> class.
        /// </summary>
        /// <param name="barcode">
        /// The <see cref="Result"/> representing the barcode result.
        /// </param>
        public BarcodResultInfo(Result barcode)
        {
            this.Text = barcode.Text;
            this.Format = barcode.BarcodeFormat.ToString();
        }
        
        #endregion
    }
}
