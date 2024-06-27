let peerConnection = new RTCPeerConnection();
let remoteStream;
let offer;
let answer;
let imageCapture;
let canvas;
let captureFrameInterval;

let init = async () => {
    remoteStream = new MediaStream();

    peerConnection.ontrack = (event) => {
        event.streams[0].getTracks().forEach((track) => {
            remoteStream.addTrack(track);
        });
    };

    peerConnection.onconnectionstatechange = () => {
        if (peerConnection.connectionState === 'connected') {
            startCaptureFrame();
        }
        else if (peerConnection.connectionState === 'disconnected' || peerConnection.connectionState === 'closed') {
            clearInterval(captureFrameInterval);
        }
    };
}

let createAnswer = async (message) => {
    offer = message;

    peerConnection.onicecandidate = async (event) => {
        if (event.candidate) {
            window.chrome.webview.postMessage(peerConnection.localDescription);
            const data = {};
            data["message"] = "Answer copied to clipboard. Connection State: " + peerConnection.connectionState;
            window.chrome.webview.postMessage(data);
        }
    };

    await peerConnection.setRemoteDescription(offer);

    answer = await peerConnection.createAnswer();
    await peerConnection.setLocalDescription(answer);
}

let startCaptureFrame = async () => {
    const tracks = remoteStream.getVideoTracks();
    imageCapture = new ImageCapture(tracks[0]);
    canvas = document.createElement('canvas');
    captureFrameInterval = setInterval(captureFrame, 33);
}

let captureFrame = async () => {
    imageCapture.grabFrame().then((imageBitmap) => {
        console.log("Grabbed frame:", imageBitmap);
        canvas.width = imageBitmap.width;
        canvas.height = imageBitmap.height;
        canvas.getContext("2d").drawImage(imageBitmap, 0, 0);
        const data = {};
        data["image"] = canvas.toDataURL('image/png');
        window.chrome.webview.postMessage(data);
    })

}

init();