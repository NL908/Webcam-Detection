Prerequisites:

- Visual Studio 2022 or later

- .NET 7 SDK

- A webcam connected

Access the Application:

- WPF running through Visual Studio

- Open "browser.html" for the browser-end

Webcam:

- If multiple webcams are available, the default webcam will be used.

How to Run:

1. Open the WebCamApp.sln file in Visual Studio.

2. Restore NuGet dependencies.

3. Open solution property by right click on the solution, go to "Properties", under "Common Properties" select "Startup Project", select "Single startup projectPress" and select "WPFApp".
Press 'F5' or click the 'Start' button in Visual Studio to run the WPF App.

4. Using Chrome or Edge to open browser.html for the browser-end.

5. Allow camera access, now should there be a webcam feed in the browser.

6. Click "Create Offer" in the browser, the Offer JSON string is copied to the clipboard.

7. Click "Create Answer" in the WPF App while having the Offer JSON in your clipboard, the Answer JSON string is copied to the clipboard.

8. Click "Add Answer" in the broswer while having the Answer JSON in your clipboard. The WPF App and the browser are now connected through WebRTC.

9. The processed detection video frame is now displayed in the WPF App.