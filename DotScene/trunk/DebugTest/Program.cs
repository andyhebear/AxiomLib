using Axiom.Core;

namespace Axiom.Game.DirectX9
{
	internal static class Program
	{
		private static void Main()
		{
			using ( var engine = new Root() )
			{
				engine.RenderSystem = engine.RenderSystems[ 0 ];
				using ( var renderWindow = engine.Initialize( true ) )
				{
					var game = new Game( engine, renderWindow );
					game.OnLoad();
					engine.FrameRenderingQueued += game.OnRenderFrame;
					engine.StartRendering();
					game.OnUnload();
				}
			}
		}
	}
}
