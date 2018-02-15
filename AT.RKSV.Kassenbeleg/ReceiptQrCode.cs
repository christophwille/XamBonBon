using System;
using System.Collections.Generic;
using System.Text;

namespace AT.RKSV.Kassenbeleg
{
	public class ReceiptQrCode
	{
		private const int ExpectedParts = 14;

		private const int IdxCiperSuite = 1;
		private const int IdxCashRegisterId = 2;
		private const int IdxReceiptId = 3;
		private const int IdxDate = 4;
		private const int IdxBetragSatzNormal = 5;
		private const int IdxBetragSatzErmaessigt1 = 6;
		private const int IdxBetragSatzErmaessigt2 = 7;
		private const int IdxBetragSatzNull = 8;
		private const int IdxBetragSatzBesonders = 9;
		private const int IdxEncryptedTurnoverCounter = 10;
		private const int IdxCertificateSerial = 11;
		private const int IdxPreviousReceiptHash = 12;
		private const int IdxSignatureValue = 13;

		private readonly string _qrCode;
		private readonly bool _isValidQrCode;
		private readonly string[] _qrCodeParts;

		public ReceiptQrCode(string qrCode)
		{
			_qrCode = qrCode;

			if (!String.IsNullOrWhiteSpace(_qrCode))
			{
				_qrCodeParts = _qrCode.Split('_');
				_isValidQrCode = (_qrCodeParts.Length == ExpectedParts);
			}
			else
			{
				_isValidQrCode = false;
			}
		}

		public bool IsValid => _isValidQrCode;
		public string QrCode => _qrCode;

		private string GetElement(int idx)
		{
			if (!_isValidQrCode) return null;
			return _qrCodeParts[idx];
		}

		public string CipherSuite => GetElement(IdxCiperSuite);
		public string CashRegisterId => GetElement(IdxCashRegisterId);
		public string ReceiptId => GetElement(IdxReceiptId);
		public string Date => GetElement(IdxDate);
		public string CertificateSerial => GetElement(IdxCertificateSerial);
		public string SignatureValue => GetElement(IdxSignatureValue);

		public string GetDataForHashing()
		{
			if (!_isValidQrCode) return null;

			// qrcode: last index of '_', take left, SHA256 => original hash
			return _qrCode.Substring(0, _qrCode.LastIndexOf('_'));
		}
	}
}
