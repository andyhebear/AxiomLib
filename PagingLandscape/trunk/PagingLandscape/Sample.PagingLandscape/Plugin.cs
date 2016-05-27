using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axiom.Samples.PagingLandscape
{
	public class Plugin : SamplePlugin
	{
		private PLSample sample;
		public override void Initialize()
		{
			sample = new PLSample();
			Name = sample.Metadata[ "Title" ] + " Sample";
			AddSample( sample );
		}
	}
}
