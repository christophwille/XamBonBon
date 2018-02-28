using System;
using System.Collections.Generic;
using System.Text;

namespace AT.RKSV.Kassenbeleg
{
	public static class AlgorithmusKennzeichen
	{
		// https://www.ris.bka.gv.at/GeltendeFassung.wxe?Abfrage=Bundesnormen&Gesetzesnummer=20009390&FassungVom=2017-04-01
		// §20 Geschlossenes System
		public const string VdaGeschlossenesSystem = "R1-AT0";

		public const string VdaATrust = "R1-AT1";
		public const string VdaGlobaltrust = "R1-AT2";
		public const string VdaPrimesign = "R1-AT3";
	}
}
