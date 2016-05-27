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
	public class ColladaTestFixture
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
		[ExpectedArgumentNullException()]
		public void versionRequired_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema'></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i );
		}

		[Test]
		public void versionAttribute_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i );
			Assert.AreEqual( "1.4.1", i.version );
		}

		[Test]
		public void assetElement_Required_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i );
			Assert.AreEqual( "1.4.1", i.version );
			Assert.IsNotNull( i.asset );
		}

		[Test]
		public void assetElement_Test()
		{
			string collada = "<?xml version='1.0'?><COLLADA xmlns='http://www.collada.org/2005/11/COLLADASchema' version='1.4.1'><asset></asset></COLLADA>";
			global::Collada i = Deserialize( collada );

			Assert.IsNotNull( i );
			Assert.AreEqual( "1.4.1", i.version );
			Assert.IsNotNull( i.asset );
		}


	}
}
