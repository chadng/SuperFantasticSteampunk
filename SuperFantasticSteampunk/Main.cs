using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace SuperFantasticSteampunk
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main (string[] args)
		{
			NSApplication.Init();
			
			using (var p = new NSAutoreleasePool()) {
				NSApplication.SharedApplication.Delegate = new AppDelegate();
				NSApplication.Main(args);
			}


		}
	}

	class AppDelegate : NSApplicationDelegate
	{
		Game1 game;

		public override void FinishedLaunching(MonoMac.Foundation.NSObject notification)
		{
			Logger.Start();
			try
			{
				game = new Game1();
				game.Run(Microsoft.Xna.Framework.GameRunBehavior.Synchronous);
			}
			catch (Exception e)
			{
				Logger.Log(e);
			}
			Logger.Finish();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
		{
			return true;
		}
	}
}


