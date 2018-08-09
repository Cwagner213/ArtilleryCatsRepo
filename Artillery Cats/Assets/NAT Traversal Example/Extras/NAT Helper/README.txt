The ExampleNATHelperOnly scene contains an example implementation punchthrough and port-forwarding without any UNet integration.
This can be a useful guide for using NAT Traversal with other networking systems that are not UNET.

Instructions:
	1. Build and run.
	2. Press play to also run in editor.
	3. Click "Listen for Punchthrough" in editor to start listening for punchthrough attempts.
	4. Copy the Host GUID that is displayed in the text box.
	5. Paste the GUID into the text box in the build, and press the "Punchthrough" button.
	6. Observe the console for message "NATHelper: Hole is punched on port: XXXXX"
		* At this point a hole has been punched and you should be able to connect over it using any socket based networking system.

Next Steps:
	Take a look at NATHelperTester.cs to see how it works.
	