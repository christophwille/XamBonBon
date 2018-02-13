using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamBonBon.Services
{
	public interface IQrScanningService
	{
		Task<string> ScanAsync();
	}

}
