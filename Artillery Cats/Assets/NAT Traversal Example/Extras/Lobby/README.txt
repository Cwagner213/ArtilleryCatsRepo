The ExampleLobby scene contains an example implementation of UNet lobby combined with NAT Traversal.

Instructions:
	1. Build and press the "Enable Match Maker" button and then "Create Internet Match"
	2. Run in the editor and press the "Enable Match Maker" and then "Find Internet Match"
	3. Click the button for the match to join it (should be the top button)
	4. Press the "START" button on the host to become ready.
	5. Press the "START" button on the client to become ready.
	6. When both players are ready the OnlineScene is automatically loaded.

Next Steps:
	
	Check out the NATLobbyManager.cs and NATLobbyPlayer.cs scripts for details. They are almost exact copies of UNET's NetworkLobbyManager and NetworkLobbyPlayer so you can read Unity's documentation for those components for more info. 