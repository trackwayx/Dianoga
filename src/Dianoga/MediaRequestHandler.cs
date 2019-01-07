using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dianoga.Invokers.MediaCacheAsync;
using Sitecore.Configuration;
using Sitecore.Resources.Media;
using Sitecore.Rules.Conditions;

namespace Dianoga
{
	public class MediaRequestHandler : Sitecore.Resources.Media.MediaRequestHandler
	{
		protected override bool DoProcessRequest(HttpContext context, MediaRequest request, Media media)
		{
			if (context?.Request.AcceptTypes != null && (context.Request.AcceptTypes).Contains("image/webp"))
			{
				request.Options.CustomOptions["extension"] = "webp";
			}

			return base.DoProcessRequest(context, request, media);
		}

		private static bool AcceptWebP(HttpContext context)
		{
			return context?.Request.AcceptTypes != null && (context.Request.AcceptTypes).Contains("image/webp");
		}

		protected override void SendMediaHeaders(Media media, HttpContext context)
		{
			base.SendMediaHeaders(media, context);

			if (MediaManager.Cache is OptimizingMediaCache)
			{
				// CDNs should only cache the optimized version of the media, so we disallow public caching until the optimization is finished.
				if (Settings.MediaResponse.Cacheability != HttpCacheability.NoCache
					&& ((OptimizingMediaCache)MediaManager.Cache).IsCurrentlyOptimizing(media))
					context.Response.Cache.SetCacheability(HttpCacheability.Private);
			}
		}
	}
}
