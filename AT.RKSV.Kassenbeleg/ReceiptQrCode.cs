using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NeoSmart.Utils;

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

		public string BetragSatzNormal => GetElement(IdxBetragSatzNormal);
		public string BetragSatzErmaessigt1 => GetElement(IdxBetragSatzErmaessigt1);
		public string BetragSatzErmaessigt2 => GetElement(IdxBetragSatzErmaessigt2);
		public string BetragSatzNull => GetElement(IdxBetragSatzNull);
		public string BetragSatzBesonders => GetElement(IdxBetragSatzBesonders);

		public string CertificateSerial => GetElement(IdxCertificateSerial);
		public int CertificateSerialAsDecimal => Convert.ToInt32(CertificateSerial, 16);
		public string SignatureValue => GetElement(IdxSignatureValue);

		public byte[] GetJwsHash()
		{
			if (!_isValidQrCode) return null;

			// last index of '_', take left, add jws header with dot, SHA256 => original hash
			string data = _qrCode.Substring(0, _qrCode.LastIndexOf('_'));
			string dataUrlEncoded = UrlBase64.Encode(System.Text.Encoding.UTF8.GetBytes(data));
			string dataForHashing = $"{DefaultJwsHeader}.{dataUrlEncoded}"; // header.data format, both header and data Base64 URL encoded

			byte[] dataForHashingAsBytes = System.Text.Encoding.UTF8.GetBytes(dataForHashing);

			using (var algorithm = SHA256.Create())
			{
				return algorithm.ComputeHash(dataForHashingAsBytes);
			}
		}

		private const string DefaultJwsHeader = "eyJhbGciOiJFUzI1NiJ9"; // Seite 42 der Spezifikation - Header Base64 URL encoded für R1

		// Allgemein: https://openid.net/specs/draft-jones-json-web-signature-04.html#DefiningECDSA
		//
		// Spezifikation 3.1 Erstellung der JWS-Signatur (Sicherheitseinrichtung funktionsfähig)
		//    "Wird die Signatur manuell erstellt (ohne Verwendung einer JWS-Bibliothek) muss darauf geachtet werden, dass der Signaturwert korrekt
		//    formatiert ist. So verwendet z.B. Java ASN.1 für die Kodierung und die DER Darstellung für die Repräsentation des Signaturwerts. Der
		//    JWS-Standard3 verlangt aber die einfache Konkatenierung der beiden Teilelemente des Signaturwertes R und S: R|S. Im Muster-Code ist
		//    dies in den unten angegebenen Beispielen ersichtlich."
		public bool ValidateSignature(byte[] certificateBytes)
		{
			var cert = new X509Certificate2(certificateBytes);

			// https://stackoverflow.com/a/38235996/141927
			using (ECDsa ecdsa = cert.GetECDsaPublicKey())
			{
				if (ecdsa != null)
				{
					byte[] signature = Convert.FromBase64String(SignatureValue);
					if (64 != signature.Length)
					{
						return false;
					}

					// Add: Check ob Cert zum Datum der Belegerstellung gültig war

					return ecdsa.VerifyHash(GetJwsHash(), signature);
				}
			}

			return false;
		}
	}
}
