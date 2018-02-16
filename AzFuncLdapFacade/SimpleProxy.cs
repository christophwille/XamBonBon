
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AT.RKSV.Kassenbeleg;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AzFuncLdapFacade
{
	public static class SimpleProxy
	{
		/*
			Problem statement: 
			  Doing lookups each & every time via LDAP on the client (phone) is inefficient, error-prone, and generates a ton of load on the LDAP servers
			Solution idea:
			  Call a Web API to do the validation. The API can cache the certificates for a period of time, thus making it more efficient. (Caching NOT impl)
			Data minimization:
			  Instead of sending the QR code, only send the JWS hash (plus cert#, authority, sig). That way the amount, date et al is kept on the client.
			  The server only ever knows how many bons were validated for a merchant, but nothing else.
		*/
		[FunctionName("SimpleProxy")]
		public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
		{
			// TODO: Proper error handling, proper monitoring (Application Insights)
			log.Info("C# HTTP trigger function processed a request.");

			string requestBody = new StreamReader(req.Body).ReadToEnd();
			var data = JsonConvert.DeserializeObject<VerificationParameters>(requestBody);

			// Short-circuit out of here if signature is invalid anyways
			byte[] signature = Convert.FromBase64String(data.Signature);
			if (64 != signature.Length)
			{
				return new BadRequestObjectResult("Signature is not 64 bytes in length");
			}

			// TODO: A-Trust hardcoded, would be data.Authority switch
			// TODO: Here we would be adding the caching logic for the certificates (hash of authority & cert# for lookup)
			var certificateLookupResult = CertificateLookup.ATrust(data.CertificateNumber);

			// TODO: Assuming valid lookup, would need checking certificateLookupResult.Found
			var cert = new X509Certificate2(certificateLookupResult.CertificateBinary);

			// https://stackoverflow.com/a/38235996/141927
			using (ECDsa ecdsa = cert.GetECDsaPublicKey())
			{
				if (ecdsa != null)
				{
					bool valid = ecdsa.VerifyHash(Convert.FromBase64String(data.HashToVerify), signature);
					return (ActionResult)new OkObjectResult(valid);
				}
				else
				{
					return new NotFoundResult();
				}
			}
		}
	}

	/*  Sample:
		{
		"authority": "notusedatm",
		"certificatenumber": 2065058440,
		"hashtoverify": "RQvAE3zXoCVonYn8D5KIOZbcTxM+O0lOtOXBTqWarqM=",
		"signature": "irdxIo1TAowB1OzpU+dgeAS887k8AuT09jrcMjZx95xHzbKp5pLQcupkbpZK5UxtDaxj08+8bRO30Y4wxiwonw=="
		}
	*/
	public class VerificationParameters
	{
		[JsonProperty("authority")]
		public string Authority { get; set; }
		[JsonProperty("certificatenumber")]
		public int CertificateNumber { get; set; }
		[JsonProperty("hashtoverify")]
		public string HashToVerify { get; set; }
		[JsonProperty("signature")]
		public string Signature { get; set; }
	}
}
