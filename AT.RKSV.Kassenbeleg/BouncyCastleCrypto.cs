using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace AT.RKSV.Kassenbeleg
{
	public static class BouncyCastleCrypto
	{
		public static bool VerifySignature(byte[] certificate, byte[] signature, byte[] data)
		{
			var cert = new X509CertificateParser().ReadCertificate(certificate);

			// https://stackoverflow.com/questions/12263641/digital-signature-verification-using-bouncycastle-ecdsa-with-sha-256-c-sharp
			// https://stackoverflow.com/a/29574548/141927
			ECPublicKeyParameters ecPublic = (ECPublicKeyParameters)cert.GetPublicKey();

			ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
			signer.Init(false, ecPublic);
			signer.BlockUpdate(data, 0, data.Length);

			return signer.VerifySignature(derEncodeSignature(signature));
		}

		// https://stackoverflow.com/a/37593464/141927
		private static byte[] derEncodeSignature(byte[] signature)
		{
			byte[] r = signature.RangeSubset(0, (signature.Length / 2));
			byte[] s = signature.RangeSubset((signature.Length / 2), (signature.Length / 2));

			MemoryStream stream = new MemoryStream();
			DerOutputStream der = new DerOutputStream(stream);

			Asn1EncodableVector v = new Asn1EncodableVector();
			v.Add(new DerInteger(new BigInteger(1, r)));
			v.Add(new DerInteger(new BigInteger(1, s)));
			der.WriteObject(new DerSequence(v));

			return stream.ToArray();
		}
	}
	
	public static class ArrayExtensions
	{
		public static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
		{
			T[] subset = new T[length];
			Array.Copy(array, startIndex, subset, 0, length);
			return subset;
		}
	}
}
