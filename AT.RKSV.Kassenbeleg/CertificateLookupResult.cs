using System;
using System.Collections.Generic;
using System.Text;

namespace AT.RKSV.Kassenbeleg
{
	public class CertificateLookupResult
	{
		public CertificateLookupResult(string errorMessage)
		{
			ErrorMessage = errorMessage;
			Found = false;
		}

		public CertificateLookupResult(string cn, byte[] certBin)
		{
			CN = cn;
			CertificateBinary = certBin;
			Found = true;
		}

		public bool Found { get; private set; }
		public string ErrorMessage { get; private set; }

		public string CN { get; private set; }
		public byte[] CertificateBinary { get; private set; }
	}
}
