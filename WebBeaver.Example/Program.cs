﻿using System.Security.Cryptography.X509Certificates;
using WebBeaver.Core;
using WebBeaver.Net;
using WebBeaver.Example.Controller;
using Newtonsoft.Json.Linq;

// Create an http server
Http server = new Http(80);

// Create an https server
//
/*Https server = new Https(443);
server.Certificate(
	// Add cert for https
	new X509Certificate2(Http.RootDirectory + "/localhost.pfx", "admin")
);
*/
// Create a new router
Router router = new Router(server);

// All static file requests will go to the 'public' folder
router.Static("public");

// Set the template engine handler.
// This template engine will run when response.Render is called.
//
router.SetTemplateEngine((file, args) =>
{
	// Parse the file contents using scriban
	//
	Scriban.Template template = Scriban.Template.Parse(file);

	// Return our rendered result.
	return template.Render(args);
});

// Add middleware that will parse our body
router.middleware += (Request req, Response res) =>
{
	if (req.Headers.ContainsKey("Content-Type"))
	{
		string contentType = req.Headers["Content-Type"];

		switch (contentType)
		{
			case "text/json":
				req.body = JObject.Parse(req.body.ToString() ?? "{}");
				break;
		}
	}
	return true; // Continue
};

// Import our routes
router.Import<HomeController>();
router.Import<ApiController>();

// Start the http server
server.Start();