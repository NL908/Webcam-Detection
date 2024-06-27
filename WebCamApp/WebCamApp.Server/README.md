Prerequisites:

- Visual Studio 2022 or later

- .NET 7 SDK

- Node.js (version 16 or later) and npm

- A webcam connected

  

Access the Application:

- The backend should be running at http://localhost:8080.

- The frontend should be running at http://localhost:5173.

Webcam:

- If multiple webcams are available, the default webcam will be used.

  

Viewing Results:

- The processed images (with face and eye detection) will be displayed in the React frontend.

- Face detections are marked with red squares, and eye detections are marked with green squares.

How to Run:

1. Open the WebCamApp.sln file in Visual Studio.

2. Restore NuGet dependencies.

3. Press 'F5' or click the 'Start' button in Visual Studio to run the C# backend application.

4. Open command in "webcam-react-frontend" folder, type "npm run dev" to start the front-end.

5. Go to http://localhost:5173 with Chrome or Edge.