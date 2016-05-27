using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Axiom.Component.OpenAsset.Collada.Tests
{
	[TestFixture]
	public class AssetTestFixture
	{
		#region Helpers
		private static global::Collada Deserialize( string collada )
		{
			byte[] bytes = ( new System.Text.UTF8Encoding() ).GetBytes( collada );

			var stream = new MemoryStream( bytes );

			var serializer = new XmlSerializer( typeof( global::Collada ) );
			// Declare an object variable of the type to be deserialized.
			global::Collada i;

			// Call the Deserialize method to restore the object's state.
			i = (global::Collada)serializer.Deserialize( stream );
			return i;
		}
		#endregion Helpers

		[Test]
		public void contributorElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><contributor></contributor></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.contributor );
		}

		[Test]
		public void createdElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><created>2009-01-18T04:21:00Z</created></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.created );
		}

		[Test]
		public void keywordsElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><keywords></keywords></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.keywords );
		}

		[Test]
		public void modifiedElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><modified>2009-01-18T04:21:00Z</modified></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.modified );
		}

		[Test]
		public void revisionElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><revision></revision></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.revision );
		}

		[Test]
		public void subjectElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><subject></subject></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.subject );
		}

		[Test]
		public void titleElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><title></title></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.title );
		}

		[Test]
		public void unitElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><unit></unit></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.unit );
		}

		[Test]
		public void up_axisElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset><up_axis>Y_UP</up_axis></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i.asset );
			Assert.IsNotNull( i.asset.up_axis );
		}

	}
}
